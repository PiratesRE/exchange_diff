using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.DataProviders;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules
{
	internal class StorageTimeZoneSensitiveTimeTranslationRule<TStoreObject, TEntity> : IStorageTranslationRule<!0, !1>, IPropertyValueCollectionTranslationRule<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, TEntity>, ITranslationRule<TStoreObject, TEntity> where TStoreObject : IStoreObject where TEntity : IPropertyChangeTracker<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition>
	{
		public StorageTimeZoneSensitiveTimeTranslationRule(IStoragePropertyAccessor<TStoreObject, ExDateTime> storageTimeAccessor, IStoragePropertyAccessor<TStoreObject, ExTimeZone> storageTimeZoneAccessor, EntityPropertyAccessor<TEntity, ExDateTime> entityTimeAccessor, EntityPropertyAccessor<TEntity, string> entityIntendedTimeZoneIdAccessor, DateTimeHelper dateTimeHelper)
		{
			this.Helper = dateTimeHelper;
			this.storageTimeAccessor = storageTimeAccessor;
			this.storageTimeZoneAccessor = storageTimeZoneAccessor;
			this.entityTimeAccessor = entityTimeAccessor;
			this.entityIntendedTimeZoneIdAccessor = entityIntendedTimeZoneIdAccessor;
			this.StorageDependencies = this.storageTimeAccessor.Dependencies.Union(this.storageTimeZoneAccessor.Dependencies);
			this.StoragePropertyGroup = this.storageTimeAccessor.PropertyChangeMetadataGroup;
			this.EntityProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
			{
				this.entityTimeAccessor.PropertyDefinition,
				this.entityIntendedTimeZoneIdAccessor.PropertyDefinition
			};
		}

		public IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> EntityProperties { get; private set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> StorageDependencies { get; private set; }

		public PropertyChangeMetadata.PropertyGroup StoragePropertyGroup { get; private set; }

		private protected DateTimeHelper Helper { protected get; private set; }

		public void FromLeftToRightType(TStoreObject left, TEntity right)
		{
			this.FromLeftToRight(right, delegate(out ExDateTime value)
			{
				return this.storageTimeAccessor.TryGetValue(left, out value);
			}, delegate(out ExTimeZone value)
			{
				return this.storageTimeZoneAccessor.TryGetValue(left, out value);
			});
		}

		public void FromPropertyValues(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, TEntity right)
		{
			this.FromLeftToRight(right, delegate(out ExDateTime value)
			{
				IPropertyValueCollectionAccessor<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, ExDateTime> propertyValueCollectionAccessor = this.storageTimeAccessor as IPropertyValueCollectionAccessor<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, ExDateTime>;
				if (propertyValueCollectionAccessor == null)
				{
					value = default(ExDateTime);
					return false;
				}
				return propertyValueCollectionAccessor.TryGetValue(propertyIndices, values, out value);
			}, delegate(out ExTimeZone value)
			{
				IPropertyValueCollectionAccessor<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, ExTimeZone> propertyValueCollectionAccessor = this.storageTimeZoneAccessor as IPropertyValueCollectionAccessor<TStoreObject, Microsoft.Exchange.Data.PropertyDefinition, ExTimeZone>;
				if (propertyValueCollectionAccessor == null)
				{
					value = null;
					return false;
				}
				return propertyValueCollectionAccessor.TryGetValue(propertyIndices, values, out value);
			});
		}

		public void FromRightToLeftType(TStoreObject left, TEntity right)
		{
			ExDateTime exDateTime;
			if (this.storageTimeAccessor.Readonly)
			{
				ExTimeZone value;
				if (!this.storageTimeZoneAccessor.Readonly && this.GetTimeZone(right, this.entityIntendedTimeZoneIdAccessor, out value))
				{
					this.storageTimeZoneAccessor.Set(left, value);
					return;
				}
			}
			else if (this.entityTimeAccessor.TryGetValue(right, out exDateTime))
			{
				ExTimeZone exTimeZone;
				if (this.GetTimeZone(right, this.entityIntendedTimeZoneIdAccessor, out exTimeZone))
				{
					if (!this.storageTimeZoneAccessor.Readonly)
					{
						this.storageTimeZoneAccessor.Set(left, exTimeZone);
					}
					ExDateTime value2 = exTimeZone.ConvertDateTime(exDateTime);
					this.storageTimeAccessor.Set(left, value2);
					return;
				}
				this.storageTimeAccessor.Set(left, exDateTime);
			}
		}

		private bool GetTimeZone(TEntity entity, IPropertyAccessor<TEntity, string> accessor, out ExTimeZone timeZone)
		{
			timeZone = null;
			string id;
			if (accessor.TryGetValue(entity, out id))
			{
				this.Helper.TryParseTimeZoneId(id, out timeZone);
				if (timeZone == null)
				{
					throw new InvalidRequestException(Strings.ErrorInvalidTimeZoneId(id));
				}
			}
			return timeZone != null;
		}

		private void FromLeftToRight(TEntity entity, StorageTimeZoneSensitiveTimeTranslationRule<TStoreObject, TEntity>.TryGetValueFunc<ExDateTime> timeGetter, StorageTimeZoneSensitiveTimeTranslationRule<TStoreObject, TEntity>.TryGetValueFunc<ExTimeZone> timeZoneGetter)
		{
			ExDateTime value;
			if (timeGetter != null && timeGetter(out value))
			{
				this.entityTimeAccessor.Set(entity, value);
			}
			ExTimeZone exTimeZone;
			if (timeZoneGetter != null && timeZoneGetter(out exTimeZone))
			{
				this.entityIntendedTimeZoneIdAccessor.Set(entity, exTimeZone.Id);
			}
		}

		private readonly EntityPropertyAccessor<TEntity, string> entityIntendedTimeZoneIdAccessor;

		private readonly EntityPropertyAccessor<TEntity, ExDateTime> entityTimeAccessor;

		private readonly IStoragePropertyAccessor<TStoreObject, ExDateTime> storageTimeAccessor;

		private readonly IStoragePropertyAccessor<TStoreObject, ExTimeZone> storageTimeZoneAccessor;

		public delegate bool TryGetValueFunc<TValue>(out TValue value);
	}
}
