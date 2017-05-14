using System;

namespace Plainion.Windows.Controls.Text
{
    public class DocumentId : AbstractStoreItemId
    {
        public DocumentId()
        {
        }

        internal DocumentId(Guid id)
            : base(id)
        {
        }
    }
}
