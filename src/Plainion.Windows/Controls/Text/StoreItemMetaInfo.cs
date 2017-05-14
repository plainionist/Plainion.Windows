using System;

namespace Plainion.Windows.Controls.Text
{
    public class StoreItemMetaInfo<TId> where TId : AbstractStoreItemId, new()
    {
        public StoreItemMetaInfo()
        {
            Id = new TId();
            Created = DateTime.UtcNow;
            LastModified = DateTime.UtcNow;
        }

        /// <summary>
        /// Used recreate an existing DocumentStore item from store.
        /// No properties will be set except id.
        /// </summary>
        public StoreItemMetaInfo(TId id)
        {
            Id = id;
        }

        public TId Id { get; private set; }

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }
    }
}
