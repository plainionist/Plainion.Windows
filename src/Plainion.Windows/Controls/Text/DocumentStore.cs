using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    public abstract class DocumentStore
    {
        private Dictionary<Guid, Document> myDocuments;

        protected DocumentStore()
        {
            myDocuments = new Dictionary<Guid, Document>();
        }

        public Document Get(DocumentId id)
        {
            Document doc;
            if (!myDocuments.TryGetValue(id.Value, out doc))
            {
                doc = GetCore(id);
                myDocuments.Add(id.Value, doc);
            }

            return doc;
        }

        public abstract Folder Root { get; }

        public void SaveChanges()
        {
            // - first collect items to potentially delete and to keep
            // - then detect items which are really to be deleted
            // - finally update the document cache
            var folders = EnumerateFolder(Root).ToList();
        }

        private IEnumerable<Folder> EnumerateFolder(Folder folder)
        {
            yield return folder;

            foreach (var child in folder.Children)
            {
                yield return child;
            }
        }

        public abstract IReadOnlyCollection<Document> Search(string text);

        protected abstract Document GetCore(DocumentId id);

        /// <summary>
        /// Only handle the folder itself (title, associated documents).
        /// Children will be handled separately.
        /// </summary>
        protected abstract void SaveCore(Folder folder);

        /// <summary>
        /// Only handle the document. Related folder will be handled separately, if necessary.
        /// </summary>
        protected abstract void SaveCore(Document document);

        /// <summary>
        /// Only handle the document. Related folder will be handled separately.
        /// </summary>
        protected abstract void DeleteCore(DocumentId id);

        /// <summary>
        /// Only handle the folder itself (title, associated documents).
        /// Children will be handled separately.
        /// </summary>
        protected abstract void DeleteCore(FolderId id);
    }

    public static class DocumentStoreAPI
    {
        public static Folder Folder(this DocumentStore self, DocumentId id)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNull(id, "id");

            return self.Root.Folder(id);
        }

        public static Folder Folder(this Folder folder, DocumentId id)
        {
            if (folder.Documents.Any(x => x == id))
            {
                return folder;
            }

            foreach (var child in folder.Children)
            {
                var found = child.Folder(id);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new document at the given path. Path elements are separated with "/".
        /// The last path element is the title of the document.
        /// </summary>
        public static Document Create(this DocumentStore self, string path)
        {
            return null;
        }
    }
}
