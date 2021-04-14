using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion.Converters;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules
{
	internal class StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema> : IStorageTranslationRule<!0, !1>, ITranslationRule<TStoreObject, TEntity> where TStoreObject : IStoreObject where TEntity : StorageEntity<TEntitySchema> where TEntitySchema : StorageEntitySchema, new()
	{
		public StorageIdTranslationRule(IdConverter idConverter)
		{
			this.StorageDependencies = new Microsoft.Exchange.Data.PropertyDefinition[]
			{
				StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema>.ItemId,
				StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema>.FolderId
			};
			this.StoragePropertyGroup = null;
			Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[] array = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[1];
			Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[] array2 = array;
			int num = 0;
			TEntitySchema schemaInstance = SchematizedObject<TEntitySchema>.SchemaInstance;
			array2[num] = schemaInstance.IdProperty;
			this.EntityProperties = array;
			this.IdConverter = idConverter;
		}

		public IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> EntityProperties { get; private set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> StorageDependencies { get; private set; }

		public PropertyChangeMetadata.PropertyGroup StoragePropertyGroup { get; private set; }

		private protected IdConverter IdConverter { protected get; private set; }

		public void FromLeftToRightType(TStoreObject left, TEntity right)
		{
			this.FromLeftToRight(right, left.Session, delegate(out StoreId value)
			{
				value = left.Id;
				return value != null;
			});
		}

		public void FromPropertyValues(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, TEntity right)
		{
			this.FromLeftToRight(right, session, delegate(out StoreId value)
			{
				int index;
				value = (propertyIndices.TryGetValue(StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema>.ItemId, out index) ? (values[index] as StoreId) : null);
				if (value == null && propertyIndices.TryGetValue(StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema>.FolderId, out index))
				{
					value = (values[index] as StoreId);
				}
				return value != null;
			});
		}

		public void FromRightToLeftType(TStoreObject left, TEntity right)
		{
		}

		private void FromLeftToRight(TEntity entity, IStoreSession session, StorageIdTranslationRule<TStoreObject, TEntity, TEntitySchema>.TryGetValueFunc<StoreId> getId)
		{
			StoreId storeId;
			if (getId != null && getId(out storeId))
			{
				string changeKey;
				entity.Id = this.IdConverter.ToStringId(storeId, session, out changeKey);
				entity.ChangeKey = changeKey;
				entity.StoreId = storeId;
			}
		}

		private static readonly Microsoft.Exchange.Data.PropertyDefinition ItemId = ItemSchema.Id;

		private static readonly Microsoft.Exchange.Data.PropertyDefinition FolderId = FolderSchema.Id;

		public delegate bool TryGetValueFunc<TValue>(out TValue value);
	}
}
