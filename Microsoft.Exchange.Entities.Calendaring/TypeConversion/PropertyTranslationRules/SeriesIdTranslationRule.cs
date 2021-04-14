using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class SeriesIdTranslationRule : IStorageTranslationRule<ICalendarItemBase, IEvent>, IPropertyValueCollectionTranslationRule<ICalendarItemBase, Microsoft.Exchange.Data.PropertyDefinition, IEvent>, ITranslationRule<ICalendarItemBase, IEvent>
	{
		static SeriesIdTranslationRule()
		{
			SeriesIdTranslationRule.NprInstanceAccessor = new DefaultStoragePropertyAccessor<ICalendarItemBase, string>(SeriesIdTranslationRule.NprSeriesId, false);
			SeriesIdTranslationRule.PrInstanceAccessor = new DefaultStoragePropertyAccessor<ICalendarItemBase, byte[]>(SeriesIdTranslationRule.CleanGlobalObjectId, false);
		}

		public SeriesIdTranslationRule()
		{
			this.StorageDependencies = new StorePropertyDefinition[]
			{
				SeriesIdTranslationRule.CalendarItemTypeProperty,
				SeriesIdTranslationRule.NprSeriesId,
				SeriesIdTranslationRule.CleanGlobalObjectId
			};
			this.StoragePropertyGroup = null;
			this.EntityProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
			{
				SchematizedObject<EventSchema>.SchemaInstance.SeriesIdProperty
			};
		}

		public IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> EntityProperties { get; private set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> StorageDependencies { get; private set; }

		public PropertyChangeMetadata.PropertyGroup StoragePropertyGroup { get; private set; }

		public void FromLeftToRightType(ICalendarItemBase left, IEvent right)
		{
			SeriesIdTranslationRule.FromLeftToRight(right, delegate(out string value)
			{
				return this.TryGetValue(left, out value);
			});
		}

		public void FromPropertyValues(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IEvent right)
		{
			SeriesIdTranslationRule.FromLeftToRight(right, delegate(out string value)
			{
				int index;
				object obj;
				if (propertyIndices.TryGetValue(SeriesIdTranslationRule.CalendarItemTypeProperty, out index) && (obj = values[index]) is CalendarItemType)
				{
					if ((CalendarItemType)obj == CalendarItemType.Single)
					{
						if (propertyIndices.TryGetValue(SeriesIdTranslationRule.NprSeriesId, out index) && values[index] is string)
						{
							value = (string)values[index];
							return true;
						}
					}
					else if (propertyIndices.TryGetValue(SeriesIdTranslationRule.CleanGlobalObjectId, out index) && values[index] is byte[])
					{
						value = new GlobalObjectId((byte[])values[index]).ToString();
						return true;
					}
				}
				value = null;
				return false;
			});
		}

		public void FromRightToLeftType(ICalendarItemBase left, IEvent right)
		{
			if (right.IsPropertySet(SchematizedObject<EventSchema>.SchemaInstance.SeriesIdProperty))
			{
				SeriesIdTranslationRule.NprInstanceAccessor.Set(left, right.SeriesId);
			}
		}

		public bool TryGetValue(ICalendarItemBase container, out string value)
		{
			if (container.CalendarItemType == CalendarItemType.Single)
			{
				return SeriesIdTranslationRule.NprInstanceAccessor.TryGetValue(container, out value);
			}
			byte[] globalObjectIdBytes;
			if (SeriesIdTranslationRule.PrInstanceAccessor.TryGetValue(container, out globalObjectIdBytes))
			{
				value = new GlobalObjectId(globalObjectIdBytes).ToString();
				return true;
			}
			value = null;
			return false;
		}

		private static void FromLeftToRight(IEvent entity, SeriesIdTranslationRule.TryGetValueFunc<string> tryGetSeriesId)
		{
			string seriesId;
			if (tryGetSeriesId != null && tryGetSeriesId(out seriesId))
			{
				entity.SeriesId = seriesId;
			}
		}

		private static readonly IStoragePropertyAccessor<ICalendarItemBase, string> NprInstanceAccessor;

		private static readonly IStoragePropertyAccessor<ICalendarItemBase, byte[]> PrInstanceAccessor;

		private static readonly StorePropertyDefinition NprSeriesId = CalendarItemBaseSchema.SeriesId;

		private static readonly StorePropertyDefinition CleanGlobalObjectId = CalendarItemBaseSchema.CleanGlobalObjectId;

		private static readonly StorePropertyDefinition CalendarItemTypeProperty = CalendarItemBaseSchema.CalendarItemType;

		public delegate bool TryGetValueFunc<TValue>(out TValue value);
	}
}
