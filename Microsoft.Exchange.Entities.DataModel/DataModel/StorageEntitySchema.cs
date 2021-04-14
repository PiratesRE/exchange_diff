using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel
{
	public abstract class StorageEntitySchema : EntitySchema
	{
		protected StorageEntitySchema()
		{
			base.RegisterPropertyDefinition(StorageEntitySchema.StaticChangeKeyProperty);
			base.RegisterPropertyDefinition(StorageEntitySchema.StaticStoreIdProperty);
		}

		public TypedPropertyDefinition<string> ChangeKeyProperty
		{
			get
			{
				return StorageEntitySchema.StaticChangeKeyProperty;
			}
		}

		internal TypedPropertyDefinition<StoreId> StoreIdProperty
		{
			get
			{
				return StorageEntitySchema.StaticStoreIdProperty;
			}
		}

		private static readonly TypedPropertyDefinition<string> StaticChangeKeyProperty = new TypedPropertyDefinition<string>("StorageEntity.ChangeKey", null, true);

		private static readonly TypedPropertyDefinition<StoreId> StaticStoreIdProperty = new TypedPropertyDefinition<StoreId>("StorageEntity.StoreId", null, true);
	}
}
