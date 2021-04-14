using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class SeriesMasterIdTranslationRule : IStorageTranslationRule<ICalendarItemBase, Event>, ITranslationRule<ICalendarItemBase, Event>
	{
		static SeriesMasterIdTranslationRule()
		{
			SeriesMasterIdTranslationRule.NprInstanceAccessor = new DefaultStoragePropertyAccessor<ICalendarItem, string>(SeriesMasterIdTranslationRule.NprSeriesMasterId, false);
		}

		public SeriesMasterIdTranslationRule(IdConverter idConverter = null)
		{
			this.StorageDependencies = new Microsoft.Exchange.Data.PropertyDefinition[]
			{
				SeriesMasterIdTranslationRule.ItemClass,
				SeriesMasterIdTranslationRule.ItemId,
				SeriesMasterIdTranslationRule.NprSeriesMasterId
			};
			this.StoragePropertyGroup = null;
			this.EntityProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
			{
				SchematizedObject<EventSchema>.SchemaInstance.SeriesMasterIdProperty
			};
			this.IdConverter = (idConverter ?? IdConverter.Instance);
		}

		public IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> EntityProperties { get; private set; }

		public IdConverter IdConverter { get; private set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> StorageDependencies { get; private set; }

		public PropertyChangeMetadata.PropertyGroup StoragePropertyGroup { get; private set; }

		public void FromLeftToRightType(ICalendarItemBase left, Event right)
		{
			this.FromLeftToRight(right, delegate(out string value)
			{
				return this.TryGetValue(left, out value);
			});
		}

		public void FromPropertyValues(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, Event right)
		{
			this.FromLeftToRight(right, delegate(out string value)
			{
				int index;
				object obj;
				if (propertyIndices.TryGetValue(SeriesMasterIdTranslationRule.ItemClass, out index) && (obj = values[index]) is string)
				{
					string itemClass = (string)obj;
					if (ObjectClass.IsCalendarItemOccurrence(itemClass) || ObjectClass.IsRecurrenceException(itemClass))
					{
						return this.TryGetSeriesMasterIdForPrInstance(propertyIndices, values, session, out value);
					}
					if (ObjectClass.IsCalendarItem(itemClass))
					{
						return this.TryGetSeriesMasterIdForNprInstance(propertyIndices, values, session, out value);
					}
				}
				value = null;
				return false;
			});
		}

		public void FromRightToLeftType(ICalendarItemBase left, Event right)
		{
			ICalendarItem calendarItem = left as ICalendarItem;
			if (calendarItem != null && right.IsPropertySet(SchematizedObject<EventSchema>.SchemaInstance.SeriesMasterIdProperty))
			{
				SeriesMasterIdTranslationRule.NprInstanceAccessor.Set(calendarItem, right.SeriesMasterId);
			}
		}

		public bool TryGetValue(ICalendarItemBase container, out string value)
		{
			ICalendarItemOccurrence calendarItemOccurrence = container as ICalendarItemOccurrence;
			if (calendarItemOccurrence != null)
			{
				value = this.IdConverter.ToStringId(calendarItemOccurrence.MasterId, calendarItemOccurrence.Session);
				return true;
			}
			ICalendarItem calendarItem = container as ICalendarItem;
			if (calendarItem != null && !string.IsNullOrEmpty(calendarItem.SeriesId))
			{
				return SeriesMasterIdTranslationRule.NprInstanceAccessor.TryGetValue(calendarItem, out value);
			}
			value = null;
			return false;
		}

		protected virtual bool TryGetSeriesMasterIdForPrInstance(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, out string seriesMasterId)
		{
			int index;
			object obj;
			if (propertyIndices.TryGetValue(ItemSchema.Id, out index) && (obj = values[index]) is StoreId)
			{
				StoreObjectId storeObjectId = StoreId.GetStoreObjectId((StoreId)obj);
				StoreObjectId storeId = StoreObjectId.FromProviderSpecificId(storeObjectId.ProviderLevelItemId, StoreObjectType.CalendarItem);
				seriesMasterId = this.IdConverter.ToStringId(storeId, session);
				return true;
			}
			seriesMasterId = null;
			return false;
		}

		protected virtual bool TryGetSeriesMasterIdForNprInstance(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IStoreSession session, out string seriesMasterId)
		{
			int index;
			object obj;
			if (propertyIndices.TryGetValue(SeriesMasterIdTranslationRule.NprSeriesMasterId, out index) && (obj = values[index]) is string)
			{
				seriesMasterId = (string)obj;
				return true;
			}
			seriesMasterId = null;
			return false;
		}

		private void FromLeftToRight(Event entity, SeriesMasterIdTranslationRule.TryGetValueFunc<string> tryGetMasterId)
		{
			string seriesMasterId;
			if (tryGetMasterId != null && tryGetMasterId(out seriesMasterId))
			{
				entity.SeriesMasterId = seriesMasterId;
			}
		}

		private static readonly Microsoft.Exchange.Data.PropertyDefinition ItemClass = StoreObjectSchema.ItemClass;

		private static readonly Microsoft.Exchange.Data.PropertyDefinition ItemId = ItemSchema.Id;

		private static readonly IStoragePropertyAccessor<ICalendarItem, string> NprInstanceAccessor;

		private static readonly StorePropertyDefinition NprSeriesMasterId = CalendarItemSchema.SeriesMasterId;

		public delegate bool TryGetValueFunc<TValue>(out TValue value);
	}
}
