using System;

namespace Plainion.Windows.Controls.Text
{
    public class AbstractStoreItemId
    {
        public AbstractStoreItemId()
        {
            Value = Guid.NewGuid();
        }

        public Guid Value { get; private set; }
    }
}
