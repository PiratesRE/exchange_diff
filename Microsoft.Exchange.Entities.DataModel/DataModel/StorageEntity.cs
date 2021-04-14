using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;

namespace Microsoft.Exchange.Entities.DataModel
{
	public abstract class StorageEntity<TSchema> : Entity<TSchema>, IStorageEntity, IEntity, IPropertyChangeTracker<PropertyDefinition>, IVersioned where TSchema : StorageEntitySchema, new()
	{
		public string ChangeKey
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<string>(schema.ChangeKeyProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<string>(schema.ChangeKeyProperty, value);
			}
		}

		internal StoreId StoreId
		{
			get
			{
				TSchema schema = base.Schema;
				return base.GetPropertyValueOrDefault<StoreId>(schema.StoreIdProperty);
			}
			set
			{
				TSchema schema = base.Schema;
				base.SetPropertyValue<StoreId>(schema.StoreIdProperty, value);
			}
		}

		public new static class Accessors
		{
			// Note: this type is marked as 'beforefieldinit'.
			static Accessors()
			{
				TSchema schemaInstance = SchematizedObject<TSchema>.SchemaInstance;
				StorageEntity<TSchema>.Accessors.ChangeKey = new EntityPropertyAccessor<IStorageEntity, string>(schemaInstance.ChangeKeyProperty, (IStorageEntity entity) => entity.ChangeKey, delegate(IStorageEntity entity, string s)
				{
					entity.ChangeKey = s;
				});
				TSchema schemaInstance2 = SchematizedObject<TSchema>.SchemaInstance;
				StorageEntity<TSchema>.Accessors.StoreId = new EntityPropertyAccessor<StorageEntity<TSchema>, StoreId>(schemaInstance2.StoreIdProperty, (StorageEntity<TSchema> entity) => entity.StoreId, delegate(StorageEntity<TSchema> entity, StoreId id)
				{
					entity.StoreId = id;
				});
			}

			public static readonly EntityPropertyAccessor<IStorageEntity, string> ChangeKey;

			internal static readonly EntityPropertyAccessor<StorageEntity<TSchema>, StoreId> StoreId;
		}
	}
}
