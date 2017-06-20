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

                if (myFile.Exists)
                {
                    Root = Load();
                }
                else
                {
                    Root = new Folder();
                }
            }

            public Folder Root { get; private set; }

            public IEnumerable<DocumentId> EnumerateDocuments()
            {
                return Root.Enumerate().SelectMany(f => f.Documents);
            }

            private Folder Load()
            {
                throw new NotImplementedException();
            }

            public void Save()
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

        protected override Folder GetRoot()
        {
            return myIndex.Root;
        }

        protected override void SaveRoot()
        {
            myIndex.Save();
        }

        protected override void Save(Document document)
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

        protected override void Delete(DocumentId id)
        {
            GetBodyFile(id).Delete();
            GetMetaFile(id).Delete();
        }
    }
}
