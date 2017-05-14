using System;
using System.Collections.Generic;
using System.Linq;

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

            // save modified folders and documents
            myDocuments.Values.Where(doc => doc.IsModified).ForEach(SaveCore);
            if (folders.Any(f => f.IsModified))
            {
                SaveRootCore();
            }

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
        /// Only handle the document. Related folder will be handled separately, if necessary.
        /// </summary>
        protected abstract void SaveCore(Document document);

        /// <summary>
        /// Only handle the document. Related folder will be handled separately.
        /// </summary>
        protected abstract void DeleteCore(DocumentId id);

        /// <summary>
        /// Called during SaveChanges() if at least one folder has changed.
        /// Full folder hierarchy has to be saved with this single call.
        /// </summary>
        protected abstract void SaveRootCore();
    }
}
