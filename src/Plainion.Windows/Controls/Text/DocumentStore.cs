using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public abstract class DocumentStore
    {
        private Folder myRoot;
        private Dictionary<Guid, Document> myDocuments;
        private Snapshot mySnapshot;

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

        public Folder Root
        {
            get
            {
                if (myRoot == null)
                {
                    myRoot = GetRootCore();

                    mySnapshot = new Snapshot(myRoot);
                }
                return myRoot;
            }
        }

        public void SaveChanges()
        {
            // collect items to potentially delete 
            var folders = Root.Enumerate().ToList();
            var documents = folders.SelectMany(f => f.Documents).ToList();

            var foldersToDelete = mySnapshot.Folders.Except(folders.Select(f => f.Id));
            var documentsToDelete = mySnapshot.Documents.Except(documents);

            // delete folders and documents
            documentsToDelete.ForEach(DeleteCore);
            foldersToDelete.ForEach(DeleteCore);

            // save modified folders and documents
            myDocuments.Values.Where(doc => doc.IsModified).ForEach(SaveCore);
            folders.Where(f => f.IsModified).ForEach(SaveCore);

            // update the document cache
            documentsToDelete.ForEach(id => myDocuments.Remove(id.Value));

            // mark folders and documents as saved
            folders.ForEach(f => f.MarkAsSaved());
            myDocuments.Values.Where(doc => doc.IsModified).ForEach(doc => doc.MarkAsSaved());

            // create new snapshot
            mySnapshot = new Snapshot(myRoot);
        }

        public abstract IReadOnlyCollection<Document> Search(string text);

        protected abstract Document GetCore(DocumentId id);

        protected abstract Folder GetRootCore();

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

        public static Folder Folder(this Folder self, DocumentId id)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNull(id, "id");

            if (self.Documents.Any(x => x == id))
            {
                return self;
            }

            foreach (var child in self.Children)
            {
                var found = child.Folder(id);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }

        public static IEnumerable<Folder> Enumerate(this Folder self)
        {
            Contract.RequiresNotNull(self, "self");

            yield return self;

            foreach (var child in self.Children.SelectMany(c => c.Enumerate()))
            {
                yield return child;
            }
        }

        /// <summary>
        /// Creates a new document at the given path. Path elements are separated with "/".
        /// The last path element is the title of the document.
        /// </summary>
        public static Document Create(this DocumentStore self, string path)
        {
            Contract.RequiresNotNull(self, "self");
            Contract.RequiresNotNullNotEmpty(path, "path");

            var elements = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            Contract.Requires(elements.Length > 0, "Path is pointing to root. Not a valid document path.");

            var folder = self.Root;
            foreach (var element in elements.Take(elements.Length - 1))
            {
                var child = folder.Children.FirstOrDefault(f => f.Title.Equals(element, StringComparison.OrdinalIgnoreCase));
                if (child == null)
                {
                    child = new Folder();
                    folder.Children.Add(child);
                }

                folder = child;
            }

            var documentTitle = elements[elements.Length - 1];

            Contract.Invariant(!folder.Documents
                .Select(id => self.Get(id))
                .Any(doc => doc.Title.Equals(documentTitle, StringComparison.OrdinalIgnoreCase)), "Document already exists");

            var document = new Document(() => new FlowDocument());
            document.Title = documentTitle;

            folder.Documents.Add(document.Id);

            return document;
        }

        internal static void ForEach<T>(this IEnumerable<T> self, Action<T> action)
        {
            foreach (var item in self)
            {
                action(item);
            }
        }
    }
}
