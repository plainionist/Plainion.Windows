using System;
using Plainion.Windows.Mvvm;

namespace Plainion.Windows.Controls.Text
{
    public abstract class AbstractStoreItem<TId> : BindableBase, IStoreItem where TId : AbstractStoreItemId, new()
    {
        private int mySuppressChangeTrackingGuards;
        private bool myIsModified;
        private string myTitle;

        protected AbstractStoreItem(StoreItemMetaInfo<TId> meta)
        {
            Id = meta.Id;
            Created = meta.Created;
            LastModified = meta.LastModified;

            if (meta.IsNew)
            {
                myIsModified = true;
            }
        }

        public TId Id { get; private set; }

        public string Title
        {
            get { return myTitle; }
            set { SetProperty(ref myTitle, value); }
        }

        protected override bool SetProperty<T>(ref T storage, T value, string propertyName = null)
        {
            var ret = base.SetProperty<T>(ref storage, value, propertyName);

            MarkAsModified();

            return ret;
        }

        protected void MarkAsModified()
        {
            if (mySuppressChangeTrackingGuards == 0)
            {
                myIsModified = true;
                LastModified = DateTime.UtcNow;
            }
        }

        internal virtual void MarkAsSaved()
        {
            myIsModified = false;
        }

        public bool IsModified { get { return myIsModified || CheckModified(); } }

        protected virtual bool CheckModified()
        {
            return false;
        }

        public DateTime Created { get; private set; }

        public DateTime LastModified { get; private set; }

        private class SuppressChangeTrackingGuard : IDisposable
        {
            private AbstractStoreItem<TId> myStoreItem;

            public SuppressChangeTrackingGuard(AbstractStoreItem<TId> storeItem)
            {
                myStoreItem = storeItem;

                myStoreItem.mySuppressChangeTrackingGuards++;
            }

            public void Dispose()
            {
                myStoreItem.mySuppressChangeTrackingGuards--;
                Contract.Invariant(myStoreItem.mySuppressChangeTrackingGuards >= 0, "mySuppressChangeTrackingGuards must not be smaller than zero");
            }
        }

        /// <summary>
        /// Used during deserialization to suppress change tracking.
        /// </summary>
        public IDisposable SuppressChangeTracking()
        {
            return new SuppressChangeTrackingGuard(this);
        }
    }
}
