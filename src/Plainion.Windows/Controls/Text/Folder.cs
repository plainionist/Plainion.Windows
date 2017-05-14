using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;

namespace Plainion.Windows.Controls.Text
{
    public sealed class Folder : AbstractStoreItem<FolderId>
    {
        private ObservableCollection<DocumentId> myDocuments;
        private ObservableCollection<Folder> myChildren;

        public Folder(IEnumerable<DocumentId> documents)
            : this(documents, Enumerable.Empty<Folder>())
        {
        }

        public Folder(IEnumerable<DocumentId> documents, IEnumerable<Folder> children)
        {
            myDocuments = new ObservableCollection<DocumentId>(documents);
            WeakEventManager<ObservableCollection<DocumentId>, NotifyCollectionChangedEventArgs>.AddHandler(myDocuments, "CollectionChanged", OnCollectionChanged);

            myChildren = new ObservableCollection<Folder>(children);
            WeakEventManager<ObservableCollection<Folder>, NotifyCollectionChangedEventArgs>.AddHandler(myChildren, "CollectionChanged", OnCollectionChanged);

            Snapshot = new FolderSnapshot(this);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MarkAsModified();
        }

        /// <summary>
        /// Order is preserved
        /// </summary>
        public IList<DocumentId> Documents { get { return myDocuments; } }

        /// <summary>
        /// Order is preserved
        /// </summary>
        public IList<Folder> Children { get { return myChildren; } }

        internal FolderSnapshot Snapshot { get; private set; }

        protected internal override void MarkAsSaved()
        {
            base.MarkAsSaved();

            Snapshot = new FolderSnapshot(this);
        }
    }

    // snapshot from folder from last saved state
    internal class FolderSnapshot
    {
        public FolderSnapshot(Folder folder)
        {
            Documents = folder.Documents.ToList();
            Children = folder.Children.ToList();
        }

        public IReadOnlyCollection<DocumentId> Documents { get; private set; }

        public IReadOnlyCollection<Folder> Children { get; private set; }
    }
}
