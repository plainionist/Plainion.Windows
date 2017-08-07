using Plainion.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public class FileSystemDocumentStore : DocumentStore
    {
        private IDirectory myRoot;
        private Index myIndex;

        private class Index
        {
            private FileSystemDocumentStore myStore;
            private IFile myFile;

            public Index(FileSystemDocumentStore store, IFile file)
            {
                myStore = store;
                myFile = file;
            }

            public Folder Root { get; private set; }

            public void Initialize()
            {
                if (myFile.Exists)
                {
                    Root = Load();
                }
                else
                {
                    Root = new Folder();
                }
            }

            public void Save()
            {
                using (var writer = new BinaryWriter(myFile.Stream(FileAccess.Write)))
                {
                    Save(writer, Root);
                }
            }

            private void Save(BinaryWriter writer, Folder node)
            {
                writer.Write(node.Id.Value.ToString());
                writer.Write(node.Created.Ticks);
                writer.Write(node.LastModified.Ticks);
                writer.Write(node.Title ?? string.Empty);

                writer.Write(node.Entries.Count);
                foreach (var entry in node.Entries)
                {
                    var folder = entry as Folder;
                    if (folder == null)
                    {
                        writer.Write(false);
                        writer.Write(((Document)entry).Id.Value.ToString());
                    }
                    else
                    {
                        writer.Write(true);
                        Save(writer, folder);
                    }
                }
            }

            private Folder Load()
            {
                using (var reader = new BinaryReader(myFile.Stream(FileAccess.Read)))
                {
                    return Read(reader);
                }
            }

            private Folder Read(BinaryReader reader)
            {
                var folderId = Guid.Parse(reader.ReadString());
                var created = new DateTime(reader.ReadInt64());
                var modified = new DateTime(reader.ReadInt64());

                var meta = new StoreItemMetaInfo<FolderId>(new FolderId(folderId),created,modified);

                var folder = new Folder(meta);
                folder.Title = reader.ReadString();

                var count = reader.ReadInt32();
                for (int i = 0; i < count; ++i)
                {
                    var isFolder = reader.ReadBoolean();
                    if (isFolder)
                    {
                        var child = Read(reader);
                        folder.Entries.Add(child);
                    }
                    else
                    {
                        var docId = Guid.Parse(reader.ReadString());
                        folder.Entries.Add(myStore.GetCore(new DocumentId(docId)));
                    }
                }

                return folder;
            }
        }

        public FileSystemDocumentStore(IDirectory root)
        {
            Contract.RequiresNotNull(root, "root");

            myRoot = root;
        }

        // this design allows us to provide an async version later on if creation of index and loading
        // document meta data takes to long 
        public void Initialize()
        {
            myIndex = new Index(this, myRoot.File("Index"));
            myIndex.Initialize();
        }

        public override IReadOnlyCollection<Document> Search(string text)
        {
            Contract.Invariant(myIndex != null, "DocumentStore not initialized");

            return myIndex.Root.Enumerate()
                .OfType<Document>()
                .Where(doc => DocumentOperations.Search(doc.Body, doc.Body.ContentStart, text, SearchMode.Initial).Any())
                .ToList();
        }

        protected override Document GetCore(DocumentId id)
        {
            Contract.Invariant(myIndex != null, "DocumentStore not initialized");

            using (var reader = new BinaryReader(GetMetaFile(id).Stream(FileAccess.Read)))
            {
                var created = new DateTime(reader.ReadInt64());
                var modified = new DateTime(reader.ReadInt64());
                var meta = new StoreItemMetaInfo<DocumentId>(id,created,modified);

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
                var range = new TextRange(doc.ContentStart, doc.ContentEnd);
                range.Load(stream, DataFormats.Rtf);
            }

            return doc;
        }

        protected override Folder GetRoot()
        {
            Contract.Invariant(myIndex != null, "DocumentStore not initialized");

            return myIndex.Root;
        }

        protected override void SaveRoot()
        {
            Contract.Invariant(myIndex != null, "DocumentStore not initialized");

            myIndex.Save();
        }

        protected override void Save(Document document)
        {
            Contract.Invariant(myIndex != null, "DocumentStore not initialized");

            using (var writer = new BinaryWriter(GetMetaFile(document.Id).Stream(FileAccess.Write)))
            {
                writer.Write(document.Created.Ticks);
                writer.Write(document.LastModified.Ticks);

                writer.Write(document.Title ?? string.Empty);

                writer.Write(document.Tags.Count);
                foreach (var tag in document.Tags)
                {
                    writer.Write(tag);
                }
            }

            using (var stream = GetBodyFile(document.Id).Stream(FileAccess.Write))
            {
                var range = new TextRange(document.Body.ContentStart, document.Body.ContentEnd);
                if (!range.IsEmpty)
                {
                    range.Save(stream, DataFormats.Rtf);
                }
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
            Contract.Invariant(myIndex != null, "DocumentStore not initialized");

            GetBodyFile(id).Delete();
            GetMetaFile(id).Delete();
        }
    }
}
