using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.Items;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.EntitySets
{
	internal abstract class ItemReference<TEntity> : StorageEntitySetScope<IStoreSession>, IItemReference<TEntity>, IEntityReference<TEntity> where TEntity : class, IItem
	{
		protected ItemReference(IStorageEntitySetScope<IStoreSession> scope, string itemKey) : base(scope)
		{
			this.ItemKey = itemKey;
		}

		protected ItemReference(IStorageEntitySetScope<IStoreSession> scope, StoreId itemStoreId, IStoreSession session) : base(scope)
		{
			this.ItemKey = base.IdConverter.ToStringId(itemStoreId, session);
		}

		public string ItemKey { get; private set; }

		public IAttachments Attachments
		{
			get
			{
				return new Attachments(this, this, this.GetAttachmentDataProvider(), null);
			}
		}

		public StoreId GetContainingFolderId()
		{
			if (this.containingFolderId == null)
			{
				StoreObjectId objectId = base.IdConverter.ToStoreObjectId(this.ItemKey);
				this.containingFolderId = base.StoreSession.GetParentFolderId(objectId);
			}
			return this.containingFolderId;
		}

		string IEntityReference<!0>.GetKey()
		{
			return this.ItemKey;
		}

		TEntity IEntityReference<!0>.Read(CommandContext context)
		{
			return default(TEntity);
		}

		protected virtual AttachmentDataProvider GetAttachmentDataProvider()
		{
			return new AttachmentDataProvider(this, base.IdConverter.ToStoreObjectId(this.ItemKey));
		}

		private StoreId containingFolderId;
	}
}
