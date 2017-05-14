using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace Plainion.Windows.Controls.Text
{
    public sealed class Folder : AbstractStoreItem<FolderId>
    {
        private ObservableCollection<DocumentId> myDocuments;
        private ObservableCollection<Folder> myChildren;

        public Folder()
            : this(new StoreItemMetaInfo<FolderId>())
        {
        }

        public Folder(StoreItemMetaInfo<FolderId> meta)
            : base(meta)
        {
            myDocuments = new ObservableCollection<DocumentId>();
            WeakEventManager<ObservableCollection<DocumentId>, NotifyCollectionChangedEventArgs>.AddHandler(myDocuments, "CollectionChanged", OnCollectionChanged);

            myChildren = new ObservableCollection<Folder>();
            WeakEventManager<ObservableCollection<Folder>, NotifyCollectionChangedEventArgs>.AddHandler(myChildren, "CollectionChanged", OnCollectionChanged);
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
    }
}
