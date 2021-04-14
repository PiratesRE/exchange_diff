using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.TypeConversion.PropertyAccessors;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors
{
	internal sealed class StorageAttendeesPropertyAccessor : StoragePropertyAccessor<ICalendarItemBase, IList<Attendee>>
	{
		public StorageAttendeesPropertyAccessor() : base(false, null, null)
		{
		}

		protected override void PerformSet(ICalendarItemBase container, IList<Attendee> value)
		{
			AttendeeConverter converter = this.GetConverter(container);
			converter.ToXso(value, container);
		}

		protected override bool PerformTryGetValue(ICalendarItemBase container, out IList<Attendee> value)
		{
			AttendeeConverter converter = this.GetConverter(container);
			IEnumerable<Attendee> enumerable = converter.Convert(container.AttendeeCollection);
			value = ((enumerable == null) ? null : enumerable.ToList<Attendee>());
			return enumerable != null;
		}

		private AttendeeConverter GetConverter(ICalendarItemBase calendarItem)
		{
			return new AttendeeConverter(StorageAttendeesPropertyAccessor.ResponseTypeConverter, StorageAttendeesPropertyAccessor.TypeConverter, calendarItem);
		}

		private static readonly ResponseTypeConverter ResponseTypeConverter = default(ResponseTypeConverter);

		private static readonly AttendeeTypeConverter TypeConverter = default(AttendeeTypeConverter);
	}
}
