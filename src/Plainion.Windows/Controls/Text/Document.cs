using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public sealed class Document : AbstractStoreItem<DocumentId>
    {
        private Lazy<FlowDocument> myBody;
        private int myLastModifiedHashCode = -1;
        private ObservableCollection<string> myTags;

        public Document(Func<FlowDocument> reader)
            : this(new StoreItemMetaInfo<DocumentId>(), reader)
        {
        }

        public Document(StoreItemMetaInfo<DocumentId> meta, Func<FlowDocument> reader)
            : base(meta)
        {
            Contract.RequiresNotNull(reader, "reader");

            myBody = new Lazy<FlowDocument>(reader);

            myTags = new ObservableCollection<string>();
            myTags.CollectionChanged += OnTagsChanged;
        }

        private void OnTagsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            MarkAsModified();
        }

        public FlowDocument Body
        {
            get
            {
                if (myLastModifiedHashCode != -1)
                {
                    myLastModifiedHashCode = GetBodyHashCode();
                }
                return myBody.Value;
            }
        }

        private int GetBodyHashCode()
        {
            var range = new TextRange(myBody.Value.ContentStart, myBody.Value.ContentEnd);
            return range.Text.GetHashCode();
        }

        protected override bool CheckModified()
        {
            return myLastModifiedHashCode != GetBodyHashCode();
        }

        internal override void MarkAsSaved()
        {
            base.MarkAsSaved();

            myLastModifiedHashCode = GetBodyHashCode();
        }

        public IList<string> Tags { get { return myTags; } }
    }
}
