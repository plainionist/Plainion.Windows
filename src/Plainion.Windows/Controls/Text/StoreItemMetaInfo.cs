using System;

namespace Plainion.Windows.Controls.Text
{
    public class StoreItemMetaInfo<TId> where TId : AbstractStoreItemId, new()
    {
        /// <summary>
        /// Creates a new instance of StoreItemMetaInfo.
        /// StoreItems created with instance are considered to be "modified".
        /// </summary>
        public StoreItemMetaInfo()
        {
            Id = new TId();
            Created = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
            IsNew = true;
        }

        /// <summary>
        /// Restores an instance of StoreItemMetaInfo from persisted information.
        /// StoreItems created with instance are considered to be "safed".
        /// </summary>
        public StoreItemMetaInfo(TId id, DateTime created, DateTime modified)
        {
            Id = id;
            Created = created;
            LastModified = modified;
        }

        public TId Id { get; private set; }

        public DateTime Created { get; private set; }

        public DateTime LastModified { get; private set; }

        /// <summary>
        /// If this flag is true it indicates that the related StoreItem was newly created.
        /// If this flag is false it means that the related StoreItem was restored from storage.
        /// </summary>
        public bool IsNew { get; private set; }
    }
}
