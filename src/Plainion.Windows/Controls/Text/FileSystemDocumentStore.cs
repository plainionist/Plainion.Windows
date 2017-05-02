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
        private IFileSystem myFS;

        public FileSystemDocumentStore(IFileSystem fileSystem)
        {
            Contract.RequiresNotNull(fileSystem, "fileSystem");

            myFS = fileSystem;
        }

        public Document Create(DocumentPath path)
        {
            var file = myFS.File(path.AsPath);

            Contract.Requires(!file.Exists, "Document already exists at: " + path.AsPath);

            file.Create();

            return new Document(this, path, () => new FlowDocument());
        }

        public Document Get(DocumentPath path)
        {
            var file = myFS.File(path.AsPath);

            Contract.Requires(file.Exists, "Document does not exist: " + path.AsPath);

            return new Document(this, path, () => ReadDocument(file));
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
            var file = myFS.File(document.Path.AsPath);

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
            var file = myFS.File(document.Path.AsPath);

            if(file.Exists)
            {
                file.Delete();
            }
        }

        public Document Move(Document source, DocumentPath target)
        {
            var file = myFS.File(target.AsPath);

            Contract.Requires(file.Exists, "Document does not exist: " + target.AsPath);

            file.MoveTo(myFS.Directory(target.AsPath));

            var body = source.Body;
            return new Document(this, target, () => body);
        }

        public IReadOnlyCollection<Document> Search(string text)
        {
            return myFS.Directory("/")
                .EnumerateFiles("*", SearchOption.AllDirectories)
                .Select(f => Tuple.Create(DocumentPath.Parse(f.Path), ReadDocument(f)))
                .Where(t => DocumentFacade.Search(t.Item2.ContentStart, t.Item2.ContentEnd, text, DocumentFacade.FindFlags.None, CultureInfo.InvariantCulture) != null)
                .Select(t => new Document(this, t.Item1, () => t.Item2))
                .ToList();
        }

        public IEnumerable<Document> All
        {
            get
            {
                //TODO: workaround - we should work with virtual FS and not with System.IO.Path
                return myFS.Directory(Path.GetFullPath("/"))
                    .EnumerateFiles("*", SearchOption.AllDirectories)
                    .Select(f => new Document(this, DocumentPath.Parse(f.Path), () => ReadDocument(f)));
            }
        }
    }
}
