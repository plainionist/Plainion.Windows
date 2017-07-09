using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace Plainion.Windows.Controls.Text
{
    public sealed class Folder : AbstractStoreItem<FolderId>
    {
        private ObservableCollection<Document> myDocuments;
        private ObservableCollection<Folder> myChildren;

        public Folder()
            : this(new StoreItemMetaInfo<FolderId>())
        {
        }

        public Folder(StoreItemMetaInfo<FolderId> meta)
            : base(meta)
        {
            myDocuments = new ObservableCollection<Document>();
            CollectionChangedEventManager.AddHandler(myDocuments, OnCollectionChanged);

            myChildren = new ObservableCollection<Folder>();
            CollectionChangedEventManager.AddHandler(myChildren, OnCollectionChanged);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MarkAsModified();
        }

        /// <summary>
        /// Order is preserved
        /// </summary>
        public IList<Document> Documents { get { return myDocuments; } }

        /// <summary>
        /// Order is preserved
        /// </summary>
        public IList<Folder> Children { get { return myChildren; } }
    }
}
