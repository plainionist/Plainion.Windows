using System;

namespace Plainion.Windows.Controls.Text
{
    public abstract class AbstractStoreItemId
    {
        public AbstractStoreItemId()
            : this(Guid.NewGuid())
        {
        }

        protected AbstractStoreItemId(Guid id)
        {
            Value = id;
        }

        public Guid Value { get; private set; }
    }
}
