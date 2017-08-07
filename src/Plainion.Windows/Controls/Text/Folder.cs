using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Plainion.Windows.Controls.Text
{
    public sealed class Folder : AbstractStoreItem<FolderId>
    {
        private ObservableCollection<IStoreItem> myEntries;

        public Folder()
            : this(new StoreItemMetaInfo<FolderId>())
        {
        }

        /// <summary>
        /// Used to restore a folder from persistance.
        /// </summary>
        public Folder(StoreItemMetaInfo<FolderId> meta)
            : base(meta)
        {
            myEntries = new ObservableCollection<IStoreItem>();
            CollectionChangedEventManager.AddHandler(myEntries, OnCollectionChanged);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MarkAsModified();
        }

        /// <summary>
        /// Order is preserved
        /// </summary>
        public IList<IStoreItem> Entries { get { return myEntries; } }

        public void MoveEntry(int oldIndex, int newIndex)
        {
            myEntries.Move(oldIndex, newIndex);
        }
    }
}
