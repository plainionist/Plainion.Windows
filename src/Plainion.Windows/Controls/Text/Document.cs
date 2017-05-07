using System;
using System.Windows.Documents;

namespace Plainion.Windows.Controls.Text
{
    public class Document
    {
        private Lazy<FlowDocument> myBody;

        public Document( DocumentPath path, Func<FlowDocument> reader)
        {
            Contract.RequiresNotNull(path, "path");
            Contract.RequiresNotNull(reader, "reader");

            Path = path;
            myBody = new Lazy<FlowDocument>(reader);
        }

        public DocumentPath Path { get; private set; }

        public FlowDocument Body { get { return myBody.Value; } }

        //public IList<string> Tags { get; private set; }
    }
}
