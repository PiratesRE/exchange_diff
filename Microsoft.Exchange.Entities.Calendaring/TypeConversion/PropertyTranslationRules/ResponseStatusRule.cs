using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.PropertyBags;
using Microsoft.Exchange.Entities.TypeConversion;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class ResponseStatusRule : IStorageTranslationRule<ICalendarItemBase, IEvent>, IPropertyValueCollectionTranslationRule<ICalendarItemBase, Microsoft.Exchange.Data.PropertyDefinition, IEvent>, ITranslationRule<ICalendarItemBase, IEvent>
	{
		public ResponseStatusRule()
		{
			this.StorageDependencies = CalendarItemAccessors.ResponseType.Dependencies.Union(CalendarItemAccessors.ReplyTime.Dependencies);
			this.StoragePropertyGroup = PropertyChangeMetadata.PropertyGroup.Response;
			this.EntityProperties = new Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition[]
			{
				Event.Accessors.ResponseStatus.PropertyDefinition
			};
		}

		public IEnumerable<Microsoft.Exchange.Data.PropertyDefinition> StorageDependencies { get; private set; }

		public PropertyChangeMetadata.PropertyGroup StoragePropertyGroup { get; private set; }

		public IEnumerable<Microsoft.Exchange.Entities.DataModel.PropertyBags.PropertyDefinition> EntityProperties { get; private set; }

		public void FromLeftToRightType(ICalendarItemBase calendarItemBase, IEvent theEvent)
		{
			this.FromLeftToRight(theEvent, delegate(out ResponseType value)
			{
				return CalendarItemAccessors.ResponseType.TryGetValue(calendarItemBase, out value);
			}, delegate(out ExDateTime value)
			{
				return CalendarItemAccessors.ReplyTime.TryGetValue(calendarItemBase, out value);
			});
		}

		public void FromRightToLeftType(ICalendarItemBase calendarItemBase, IEvent theEvent)
		{
			ResponseStatus container;
			if (Event.Accessors.ResponseStatus.TryGetValue(theEvent, out container))
			{
				ResponseType value;
				if (ResponseStatus.Accessors.Response.TryGetValue(container, out value))
				{
					CalendarItemAccessors.ResponseType.Set(calendarItemBase, this.responseTypeConverter.Convert(value));
				}
				ExDateTime value2;
				if (ResponseStatus.Accessors.Time.TryGetValue(container, out value2))
				{
					CalendarItemAccessors.ReplyTime.Set(calendarItemBase, value2);
				}
			}
		}

		public void FromPropertyValues(IDictionary<Microsoft.Exchange.Data.PropertyDefinition, int> propertyIndices, IList values, IEvent right)
		{
			this.FromLeftToRight(right, delegate(out ResponseType value)
			{
				return CalendarItemAccessors.ResponseType.TryGetValue(propertyIndices, values, out value);
			}, delegate(out ExDateTime value)
			{
				return CalendarItemAccessors.ReplyTime.TryGetValue(propertyIndices, values, out value);
			});
		}

		private void FromLeftToRight(IEvent entity, ResponseStatusRule.TryGetValueFunc<ResponseType> responseGetter, ResponseStatusRule.TryGetValueFunc<ExDateTime> replyTimeGetter)
		{
			ResponseStatus responseStatus = null;
			ResponseType value;
			if (responseGetter != null && responseGetter(out value))
			{
				responseStatus = new ResponseStatus();
				ResponseStatus.Accessors.Response.Set(responseStatus, this.responseTypeConverter.Convert(value));
			}
			ExDateTime value2;
			if (replyTimeGetter != null && replyTimeGetter(out value2))
			{
				if (responseStatus == null)
				{
					responseStatus = new ResponseStatus();
				}
				ResponseStatus.Accessors.Time.Set(responseStatus, value2);
			}
			if (responseStatus != null)
			{
				Event.Accessors.ResponseStatus.Set(entity, responseStatus);
			}
		}

		private ResponseTypeConverter responseTypeConverter = default(ResponseTypeConverter);

		public delegate bool TryGetValueFunc<TValue>(out TValue value);
	}
}
