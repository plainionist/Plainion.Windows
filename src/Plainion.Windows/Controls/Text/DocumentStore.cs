using System;
using System.Collections.Generic;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    public abstract class DocumentStore
    {
        private Folder myRoot;
        private Snapshot mySnapshot;

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
            var entries = Root.Enumerate().ToList();
            var folders = entries.OfType<Folder>().ToList();
            var documents = entries.OfType<Document>().ToList();

            var documentsToDelete = mySnapshot.Documents.Except(documents.Select(doc=>doc.Id));

            // delete folders and documents
            documentsToDelete.ForEach(Delete);

            // save modified folders and documents
            documents.Where(doc => doc.IsModified).ForEach(Save);
            if (folders.Any(f => f.IsModified))
            {
                SaveRoot();
            }

            // mark folders and documents as saved
            folders.ForEach(f => f.MarkAsSaved());
            documents.Where(doc => doc.IsModified).ForEach(doc => doc.MarkAsSaved());

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
