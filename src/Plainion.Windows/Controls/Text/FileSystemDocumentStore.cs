using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using Plainion.IO;

namespace Plainion.Windows.Controls.Text
{
    public class FileSystemDocumentStore : IDocumentStore
    {
        private Root myRoot;

        private class Root
        {
            private IDirectory myRoot;

            public Root(IDirectory root)
            {
                myRoot = root;
            }

            public IFile GetFile(DocumentPath path)
            {
                return myRoot.File(path.AsPath.Substring(1));
            }

            public IDirectory GetDirectory(DocumentPath path)
            {
                return myRoot.Directory(path.AsPath.Substring(1));
            }

            public DocumentPath GetDocumentPath(IFile file)
            {
                var path = file.Path
                    .Substring(myRoot.Path.Length)
                    .Replace('\\', '/');
                return DocumentPath.Parse(path);
            }

            public IEnumerable<IFile> All()
            {
                return myRoot.EnumerateFiles("*", SearchOption.AllDirectories);
            }
        }

        public FileSystemDocumentStore(IDirectory root)
        {
            Contract.RequiresNotNull(root, "root");

            myRoot = new Root(root);
        }

        public Document Create(DocumentPath path)
        {
            var file = myRoot.GetFile(path);

            Contract.Requires(!file.Exists, "Document already exists at: " + path.AsPath);

            file.Create();

            return new Document( path, () => new FlowDocument());
        }

        public Document Get(DocumentPath path)
        {
            var file = myRoot.GetFile(path);

            Contract.Requires(file.Exists, "Document does not exist: " + path.AsPath);

            return new Document( path, () => ReadDocument(file));
        }

        private FlowDocument ReadDocument(IFile file)
        {
            var doc = new FlowDocument();

            using(var stream = file.Stream(FileAccess.Read))
            {
                var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                range.Load(stream, DataFormats.Rtf);
            }

            return doc;
        }

        public void Save(Document document)
        {
            var file = myRoot.GetFile(document.Path);

            if(!file.Exists)
            {
                file.Create();
            }

            using(var stream = file.Stream(FileAccess.Write))
            {
                var range = new TextRange(document.Body.ContentStart, document.Body.ContentEnd);
                range.Save(stream, DataFormats.Rtf);
            }
        }

        public void Delete(Document document)
        {
            var file = myRoot.GetFile(document.Path);

            if(file.Exists)
            {
                file.Delete();
            }
        }

        public Document Move(Document source, DocumentPath target)
        {
            var file = myRoot.GetFile(target);

            Contract.Requires(file.Exists, "Document does not exist: " + target.AsPath);

            file.MoveTo(myRoot.GetDirectory(target));

            var body = source.Body;
            return new Document( target, () => body);
        }

        public IReadOnlyCollection<Document> Search(string text)
        {
            return myRoot
                .All()
                .Select(f => Tuple.Create(myRoot.GetDocumentPath(f), ReadDocument(f)))
                .Where(t => DocumentFacade.Search(t.Item2.ContentStart, t.Item2.ContentEnd, text, DocumentFacade.FindFlags.None, CultureInfo.InvariantCulture) != null)
                .Select(t => new Document( t.Item1, () => t.Item2))
                .ToList();
        }


        public IEnumerable<Document> All
        {
            get
            {
                return myRoot
                    .All()
                    .Select(f => new Document( myRoot.GetDocumentPath(f), () => ReadDocument(f)));
            }
        }
    }
}
