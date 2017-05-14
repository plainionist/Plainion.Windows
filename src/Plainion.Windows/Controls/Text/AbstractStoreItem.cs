using System;

namespace Plainion.Windows.Controls.Text
{
    public abstract class AbstractStoreItem<TId> where TId : AbstractStoreItemId, new()
    {
        private string myTitle;

        protected AbstractStoreItem(StoreItemMetaInfo<TId> meta)
        {
            Id = meta.Id;
            Created = meta.Created;
            LastModified = meta.LastModified;
        }

        public TId Id { get; private set; }

        public string Title
        {
            get { return myTitle; }
            set { SetProperty(ref myTitle, value); }
        }

        private bool SetProperty<T>(ref T storage, T value)
        {
            if (object.Equals(storage, value))
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

        internal void MarkAsSaved()
        {
            IsModified = false;
        }

        public bool IsModified { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime LastModified { get; private set; }
    }
}
