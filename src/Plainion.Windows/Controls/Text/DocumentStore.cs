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
                    myRoot = GetRoot();

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
            documentsToDelete.ForEach(Delete);

            // save modified folders and documents
            myDocuments.Values.Where(doc => doc.IsModified).ForEach(Save);
            if (folders.Any(f => f.IsModified))
            {
                SaveRoot();
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

        protected abstract Folder GetRoot();

        /// <summary>
        /// Called during SaveChanges() if at least one folder has changed.
        /// Full folder hierarchy has to be saved with this single call.
        /// </summary>
        protected abstract void SaveRoot();

        protected abstract Document GetCore(DocumentId id);

        /// <summary>
        /// Only handle the document. Related folder will be handled separately, if necessary.
        /// </summary>
        protected abstract void Save(Document document);

        /// <summary>
        /// Only handle the document. Related folder will be handled separately.
        /// </summary>
        protected abstract void Delete(DocumentId id);
    }
}
