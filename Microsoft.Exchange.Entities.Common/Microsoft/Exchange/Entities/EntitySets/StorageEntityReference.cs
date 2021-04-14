using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal abstract class StorageEntityReference<TEntitySet, TEntity, TStoreSession> : StorageEntitySetScope<TStoreSession>, IEntityReference<TEntity> where TEntitySet : IEntitySet<TEntity>, IStorageEntitySetScope<TStoreSession> where TEntity : class, IStorageEntity where TStoreSession : class, IStoreSession
	{
		protected StorageEntityReference(TEntitySet entitySet) : base(entitySet)
		{
			this.EntitySet = entitySet;
		}

		protected StorageEntityReference(TEntitySet entitySet, string entityKey) : this(entitySet)
		{
			this.entityKey = entityKey;
			this.storeId = base.IdConverter.ToStoreObjectId(this.entityKey);
		}

		protected StorageEntityReference(TEntitySet entitySet, StoreId entityStoreId) : this(entitySet)
		{
			this.storeId = entityStoreId;
			this.entityKey = base.IdConverter.ToStringId(entityStoreId, base.StoreSession);
		}

		public TEntitySet EntitySet { get; private set; }

		public StoreId GetStoreId()
		{
			if (this.storeId == null)
			{
				this.Resolve();
			}
			return this.storeId;
		}

		public string GetKey()
		{
			if (this.entityKey == null)
			{
				this.Resolve();
			}
			return this.entityKey;
		}

		public TEntity Read(CommandContext context)
		{
			TEntitySet entitySet = this.EntitySet;
			return entitySet.Read(this.GetKey(), context);
		}

		public override string ToString()
		{
			string result;
			if ((result = this.description) == null)
			{
				result = (this.description = string.Format("{0}{1}", this.EntitySet, this.GetRelativeDescription()));
			}
			return result;
		}

		protected virtual StoreId ResolveReference()
		{
			throw new InvalidOperationException();
		}

		protected virtual string GetRelativeDescription()
		{
			return '[' + this.GetKey() + ']';
		}

		private void Resolve()
		{
			StoreId storeId = this.ResolveReference();
			string text = base.IdConverter.ToStringId(storeId, base.StoreSession);
			this.storeId = storeId;
			this.entityKey = text;
		}

		private string description;

		private string entityKey;

		private StoreId storeId;
	}
}
