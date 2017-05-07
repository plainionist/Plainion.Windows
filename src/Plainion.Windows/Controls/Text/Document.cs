using System;
using System.Windows;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public class Document
    {
        private Lazy<FlowDocument> myBody;
        private decimal myPosition;
        private TextRange myBodyObserver;

        public Document(DocumentPath path, Func<FlowDocument> reader)
        {
            Contract.RequiresNotNull(path, "path");
            Contract.RequiresNotNull(reader, "reader");

            Path = path;
            myBody = new Lazy<FlowDocument>(reader);
        }

        public DocumentPath Path { get; private set; }

        public bool IsModified { get; set; }

        /// <summary>
        /// Defines relative position to siblings. If multiple documents in the same folder have same position
        /// the order is undefined.
        /// </summary>
        public decimal Position
        {
            get { return myPosition; }
            set
            {
                if(myPosition == value)
                {
                    return;
                }

                myPosition = value;

                IsModified = true;
            }
        }

        public FlowDocument Body
        {
            get
            {
                if(myBodyObserver == null)
                {
                    myBodyObserver = new TextRange(myBody.Value.ContentStart, myBody.Value.ContentEnd);
                    WeakEventManager<TextRange, EventArgs>.AddHandler(myBodyObserver, "Changed", OnBodyChanged);
                }
                return myBody.Value;
            }
        }

        private void OnBodyChanged(object sender, EventArgs e)
        {
            IsModified = true;
        }

        //public IList<string> Tags { get; private set; }
    }
}
