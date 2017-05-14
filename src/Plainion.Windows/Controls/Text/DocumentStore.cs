using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    public abstract class DocumentStore
    {
        public abstract Document Get(DocumentId id);

        public abstract Folder Root { get; }

        public void SaveChanges()
        {
        }

        public void RejectChanges()
        {
        }

        public abstract IReadOnlyCollection<Document> Search(string text);

        protected abstract void Save(Document document);

        protected abstract void Delete(DocumentId id);
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
