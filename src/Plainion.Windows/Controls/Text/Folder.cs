using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Plainion.Windows.Controls.Text
{
    public class Folder : AbstractStoreItem<FolderId>
    {
        private ObservableCollection<DocumentId> myDocuments;
        private ObservableCollection<Folder> myChildren;

        public Folder()
        {
            myDocuments = new ObservableCollection<DocumentId>();
            myDocuments.CollectionChanged += OnCollectionChanged;

            myChildren = new ObservableCollection<Folder>();
            myChildren.CollectionChanged += OnCollectionChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MarkAsModified();
        }

        /// <summary>
        /// Order of documents is preserved
        /// </summary>
        public IList<DocumentId> Documents { get { return myDocuments; } }

        public IList<Folder> Children { get { return myChildren; } }
    }
}
