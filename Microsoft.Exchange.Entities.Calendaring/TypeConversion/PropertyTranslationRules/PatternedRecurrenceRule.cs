using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.Converters.Recurrence;
using Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyAccessors.StorageAccessors;
using Microsoft.Exchange.Entities.DataModel.Calendaring;
using Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence;
using Microsoft.Exchange.Entities.TypeConversion.Converters;
using Microsoft.Exchange.Entities.TypeConversion.PropertyTranslationRules;

namespace Microsoft.Exchange.Entities.Calendaring.TypeConversion.PropertyTranslationRules
{
	internal class PatternedRecurrenceRule : PropertyTranslationRule<ICalendarItem, IEvent, PropertyDefinition, Recurrence, PatternedRecurrence>
	{
		public PatternedRecurrenceRule() : base(CalendarItemAccessors.Recurrence, Event.Accessors.PatternedRecurrence, null, null)
		{
		}

		public override IConverter<Recurrence, PatternedRecurrence> GetLeftToRightConverter(ICalendarItem left, IEvent right)
		{
			return new RecurrenceConverter(left.Session.ExTimeZone);
		}

		public override IConverter<PatternedRecurrence, Recurrence> GetRightToLeftConverter(ICalendarItem left, IEvent right)
		{
			return new RecurrenceConverter(left.Session.ExTimeZone);
		}
	}
}
