using System;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public class Document
    {
        private IDocumentStore myStore;
        private Lazy<FlowDocument> myBody;
        private bool myAutoSave;
        private TextRange myBodyObserver;

        public Document(IDocumentStore store, DocumentPath path, Func<FlowDocument> reader)
        {
            Contract.RequiresNotNull(store, "store");
            Contract.RequiresNotNull(path, "path");
            Contract.RequiresNotNull(reader, "reader");

            myStore = store;
            Path = path;
            myBody = new Lazy<FlowDocument>(reader);
        }

        public DocumentPath Path { get; private set; }

        public FlowDocument Body { get { return myBody.Value; } }

        public bool AutoSave
        {
            get { return myAutoSave; }
            set
            {
                if(myAutoSave == value)
                {
                    return;
                }

                // http://stackoverflow.com/questions/1655208/detect-flowdocument-change-and-scroll
                if(myBodyObserver != null)
                {
                    myBodyObserver.Changed -= OnBodyChanged;
                    myBodyObserver = null;
                }

                myAutoSave = value;

                if(myAutoSave)
                {
                    myBodyObserver = new TextRange(myBody.Value.ContentStart, myBody.Value.ContentEnd);
                    myBodyObserver.Changed += OnBodyChanged;
                }
            }
        }

        private void OnBodyChanged(object sender, EventArgs e)
        {
            myStore.Save(this);
        }

        //public IList<string> Tags { get; private set; }
    }
}
