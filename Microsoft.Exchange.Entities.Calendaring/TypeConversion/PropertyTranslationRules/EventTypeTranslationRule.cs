using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class EventTypeTranslationRule : IStorageTranslationRule<ICalendarItemBase, IEvent>, IPropertyValueCollectionTranslationRule<ICalendarItemBase, Microsoft.Exchange.Data.PropertyDefinition, IEvent>, ITranslationRule<ICalendarItemBase, IEvent>
	{
		public EventTypeTranslationRule()
		{
			this.StorageDependencies = new Microsoft.Exchange.Data.PropertyDefinition[]
			{
				EventTypeTranslationRule.CalendarItemTypeProperty,
				EventTypeTranslationRule.NprSeriesId,
				EventTypeTranslationRule.ItemClass
			};
			this.StoragePropertyGroup = null;
			this.EntityProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
			{
				SchematizedObject<EventSchema>.SchemaInstance.TypeProperty
			};
		}

		public IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> EntityProperties { get; private set; }

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> StorageDependencies { get; private set; }

		public PropertyChangeMetadata.PropertyGroup StoragePropertyGroup { get; private set; }

		public void FromLeftToRightType(ICalendarItemBase left, IEvent right)
		{
			right.Type = EventTypeTranslationRule.GetEventType(() => left.CalendarItemType, () => left.ItemClass, () => left.SeriesId);
		}

		public void FromPropertyValues(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IEvent right)
		{
			object calendarItemTypeValue;
			Func<CalendarItemType> getCalendarItemType = delegate()
			{
				if (!this.TryGetPropertyFromPropertyIndices(propertyIndices, values, EventTypeTranslationRule.CalendarItemTypeProperty, out calendarItemTypeValue))
				{
					return CalendarItemType.Single;
				}
				return (CalendarItemType)calendarItemTypeValue;
			};
			object itemClass;
			Func<string> getItemClass = delegate()
			{
				if (!this.TryGetPropertyFromPropertyIndices(propertyIndices, values, EventTypeTranslationRule.ItemClass, out itemClass))
				{
					return null;
				}
				return (string)itemClass;
			};
			object seriesId;
			Func<string> getSeriesId = delegate()
			{
				if (!this.TryGetPropertyFromPropertyIndices(propertyIndices, values, EventTypeTranslationRule.NprSeriesId, out seriesId))
				{
					return null;
				}
				return (string)seriesId;
			};
			right.Type = EventTypeTranslationRule.GetEventType(getCalendarItemType, getItemClass, getSeriesId);
		}

		public void FromRightToLeftType(ICalendarItemBase left, IEvent right)
		{
		}

		internal static EventType GetEventType(Func<CalendarItemType> getCalendarItemType, Func<string> getItemClass, Func<string> getSeriesId)
		{
			EventType result;
			switch (getCalendarItemType())
			{
			case CalendarItemType.Single:
				if (ObjectClass.IsCalendarItemSeries(getItemClass()))
				{
					result = EventType.SeriesMaster;
				}
				else if (!string.IsNullOrEmpty(getSeriesId()))
				{
					result = EventType.Exception;
				}
				else
				{
					result = EventType.SingleInstance;
				}
				break;
			case CalendarItemType.Occurrence:
				result = EventType.Occurrence;
				break;
			case CalendarItemType.Exception:
				result = EventType.Exception;
				break;
			case CalendarItemType.RecurringMaster:
				result = EventType.SeriesMaster;
				break;
			default:
				throw new ArgumentOutOfRangeException("value");
			}
			return result;
		}

		private bool TryGetPropertyFromPropertyIndices(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, Microsoft.Exchange.Data.PropertyDefinition property, out object value)
		{
			int index;
			object obj;
			if (propertyIndices.TryGetValue(property, out index) && (obj = values[index]) != null && obj.GetType() == property.Type)
			{
				value = obj;
				return true;
			}
			value = null;
			return false;
		}

		private static readonly StorePropertyDefinition NprSeriesId = CalendarItemBaseSchema.SeriesId;

		private static readonly Microsoft.Exchange.Data.PropertyDefinition ItemClass = StoreObjectSchema.ItemClass;

		private static readonly StorePropertyDefinition CalendarItemTypeProperty = CalendarItemBaseSchema.CalendarItemType;
	}
}
