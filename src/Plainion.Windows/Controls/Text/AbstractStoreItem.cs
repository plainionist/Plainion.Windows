using System;

namespace Plainion.Windows.Controls.Text
{
    public abstract class AbstractStoreItem<TId> where TId : AbstractStoreItemId, new()
    {
        private string myTitle;

        protected AbstractStoreItem()
        {
            Id = new TId();

            Created = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
        }

        public TId Id { get; private set; }

        public string Title
        {
            get { return myTitle; }
            set { SetProperty(ref myTitle, value); }
        }

        private bool SetProperty<T>(ref T storage, T value)
        {
            if(object.Equals(storage, value))
            {
                return false;
            }

            storage = value;

            MarkAsModified();

            return true;
        }

        protected void MarkAsModified()
        {
            IsModified = true;
            LastModified = DateTime.UtcNow;
        }

        public bool IsModified { get; set; }

        public DateTime Created { get; private set; }

        public DateTime LastModified { get; private set; }
    }
}
