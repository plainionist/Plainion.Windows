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
        private TextRange myBodyObserver;
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
                if (myBodyObserver == null)
                {
                    myBodyObserver = new TextRange(myBody.Value.ContentStart, myBody.Value.ContentEnd);
                    WeakEventManager<TextRange, EventArgs>.AddHandler(myBodyObserver, "Changed", OnBodyChanged);
                }
                return myBody.Value;
            }
        }

        private void OnBodyChanged(object sender, EventArgs e)
        {
            MarkAsModified();
        }

        public IList<string> Tags { get { return myTags; } }
    }
}
