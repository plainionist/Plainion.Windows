using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Plainion.IO;

namespace Plainion.Windows.Controls.Text
{
    public class FileSystemDocumentStore : DocumentStore
    {
        private IDirectory myRoot;
        private Index myIndex;

        private class Index
        {
            private IFile myFile;

            public Index(IFile file)
            {
                myFile = file;
            }

            public Folder Root { get; private set; }

            public IEnumerable<DocumentId> EnumerateDocuments()
            {
                throw new NotImplementedException();
            }

            public void Save(Folder folder)
            {
                throw new NotImplementedException();
            }

            public void Delete(FolderId id)
            {
                throw new NotImplementedException();
            }
        }

        public FileSystemDocumentStore(IDirectory root)
        {
            Contract.RequiresNotNull(root, "root");

            myRoot = root;

            myIndex = new Index(root.File("Index"));
        }

        public override IReadOnlyCollection<Document> Search(string text)
        {
            return myIndex.EnumerateDocuments()
                .Select(id => Get(id))
                .Where(doc => DocumentFacade.Search(doc.Body.ContentStart, doc.Body.ContentEnd, text, DocumentFacade.FindFlags.None, CultureInfo.InvariantCulture) != null)
                .ToList();
        }

        protected override Document GetCore(DocumentId id)
        {
            using (var reader = new BinaryReader(GetMetaFile(id).Stream(FileAccess.Read)))
            {
                var meta = new StoreItemMetaInfo<DocumentId>(id);
                meta.Created = new DateTime(reader.ReadInt64());
                meta.LastModified = new DateTime(reader.ReadInt64());

                var doc = new Document(meta, () => ReadContent(GetBodyFile(id)));
                doc.Title = reader.ReadString();

                var count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    doc.Tags.Add(reader.ReadString());
                }

                return doc;
            }
        }

        private static FlowDocument ReadContent(IFile file)
        {
            var doc = new FlowDocument();

            using (var stream = file.Stream(FileAccess.Read))
            {
                // seek header
                stream.Seek(1 + sizeof(decimal) + sizeof(long), SeekOrigin.Begin);

                var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                range.Load(stream, DataFormats.Rtf);
            }

            return doc;
        }

        protected override Folder GetRootCore()
        {
            return myIndex.Root;
        }

        protected override void SaveCore(Folder folder)
        {
            myIndex.Save(folder);
        }

        protected override void SaveCore(Document document)
        {
            using (var writer = new BinaryWriter(GetMetaFile(document.Id).Stream(FileAccess.Read)))
            {
                writer.Write(document.Created.Ticks);
                writer.Write(document.LastModified.Ticks);

                writer.Write(document.Title);

                writer.Write(document.Tags.Count);
                foreach (var tag in document.Tags)
                {
                    writer.Write(tag);
                }
            }

            using (var stream = GetBodyFile(document.Id).Stream(FileAccess.Write))
            {
                var range = new TextRange(document.Body.ContentStart, document.Body.ContentEnd);
                range.Save(stream, DataFormats.Rtf);
            }
        }

        private IFile GetBodyFile(DocumentId id)
        {
            return myRoot.File(id.Value + ".body");
        }

        private IFile GetMetaFile(DocumentId id)
        {
            return myRoot.File(id.Value + ".meta");
        }

        protected override void DeleteCore(DocumentId id)
        {
            GetBodyFile(id).Delete();
            GetMetaFile(id).Delete();
        }

        protected override void DeleteCore(FolderId id)
        {
            myIndex.Delete(id);
        }
    }

    //public class FileSystemDocumentStore : IDocumentStore
    //{
    //    private Root myRoot;

    //    private class Root
    //    {
    //        private IDirectory myRoot;

    //        public Root(IDirectory root)
    //        {
    //            myRoot = root;
    //        }

    //        public IFile GetFile(DocumentPath path)
    //        {
    //            return myRoot.File(path.AsPath.Substring(1));
    //        }

    //        public IDirectory GetDirectory(DocumentPath path)
    //        {
    //            return myRoot.Directory(path.AsPath.Substring(1));
    //        }

    //        public DocumentPath GetDocumentPath(IFile file)
    //        {
    //            var path = file.Path
    //                .Substring(myRoot.Path.Length)
    //                .Replace('\\', '/');
    //            return DocumentPath.Parse(path);
    //        }

    //        public IEnumerable<IFile> All()
    //        {
    //            return myRoot.EnumerateFiles("*", SearchOption.AllDirectories);
    //        }
    //    }

    //    private class DocumentSerializer
    //    {
    //        private const byte Version = 1;

    //        public static void Write(Document document, IFile file)
    //        {
    //            using(var stream = file.Stream(FileAccess.Write))
    //            {
    //                using(var writer = new BinaryWriter(stream, Encoding.Default, true))
    //                {
    //                    writer.Write(Version);

    //                    writer.Write(document.Position);     // position in folder
    //                    writer.Write(DateTime.UtcNow.Ticks); // last modified
    //                }

    //                var range = new TextRange(document.Body.ContentStart, document.Body.ContentEnd);
    //                range.Save(stream, DataFormats.Rtf);
    //            }
    //        }

    //        internal static Document Read(IFile file, DocumentPath path)
    //        {
    //            using(var stream = file.Stream(FileAccess.Read))
    //            {
    //                using(var reader = new BinaryReader(stream, Encoding.Default, true))
    //                {
    //                    var version = reader.ReadByte();

    //                    Contract.Invariant(version == Version, "Version not supported: " + version);

    //                    var position = reader.ReadDecimal();

    //                    return new Document(path, () => ReadContent(file))
    //                    {
    //                        Position = position
    //                    };
    //                }
    //            }
    //        }

    //        private static FlowDocument ReadContent(IFile file)
    //        {
    //            var doc = new FlowDocument();

    //            using(var stream = file.Stream(FileAccess.Read))
    //            {
    //                // seek header
    //                stream.Seek(1 + sizeof(decimal) + sizeof(long), SeekOrigin.Begin);

    //                var range = new TextRange(doc.ContentStart, doc.ContentEnd);
    //                range.Load(stream, DataFormats.Rtf);
    //            }

    //            return doc;
    //        }
    //    }

    //    public FileSystemDocumentStore(IDirectory root)
    //    {
    //        Contract.RequiresNotNull(root, "root");

    //        myRoot = new Root(root);
    //    }

    //    public Document Create(DocumentPath path)
    //    {
    //        var file = myRoot.GetFile(path);

    //        Contract.Requires(!file.Exists, "Document already exists at: " + path.AsPath);

    //        file.Create();

    //        return new Document(path, () => new FlowDocument());
    //    }

    //    public Document Get(DocumentPath path)
    //    {
    //        var file = myRoot.GetFile(path);

    //        Contract.Requires(file.Exists, "Document does not exist: " + path.AsPath);

    //        return DocumentSerializer.Read(file, path);
    //    }

    //    public void Save(Document document)
    //    {
    //        var file = myRoot.GetFile(document.Path);

    //        if(!file.Exists)
    //        {
    //            file.Create();
    //        }

    //        DocumentSerializer.Write(document, file);
    //    }

    //    public void Delete(Document document)
    //    {
    //        var file = myRoot.GetFile(document.Path);

    //        if(file.Exists)
    //        {
    //            file.Delete();
    //        }
    //    }

    //    public Document Move(Document source, DocumentPath target)
    //    {
    //        var file = myRoot.GetFile(target);

    //        Contract.Requires(file.Exists, "Document does not exist: " + target.AsPath);

    //        file.MoveTo(myRoot.GetDirectory(target));

    //        var body = source.Body;
    //        return new Document(target, () => body);
    //    }

    //    public IEnumerable<Document> All
    //    {
    //        get
    //        {
    //            return myRoot
    //                .All()
    //                .Select(f => DocumentSerializer.Read(f, myRoot.GetDocumentPath(f)));
    //        }
    //    }

    //    public IReadOnlyCollection<Document> Search(string text)
    //    {
    //        return myRoot
    //            .All()
    //            .Select(f => DocumentSerializer.Read(f, myRoot.GetDocumentPath(f)))
    //            .Where(doc => DocumentFacade.Search(doc.Body.ContentStart, doc.Body.ContentEnd, text, DocumentFacade.FindFlags.None, CultureInfo.InvariantCulture) != null)
    //            .ToList();
    //    }
    //}
}
