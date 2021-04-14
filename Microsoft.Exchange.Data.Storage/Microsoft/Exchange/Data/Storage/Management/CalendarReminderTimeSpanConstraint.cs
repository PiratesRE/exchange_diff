using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class CalendarReminderTimeSpanConstraint : PropertyDefinitionConstraint
	{
		public TimeSpan MaxTimeSpan
		{
			get
			{
				return CalendarReminderTimeSpanConstraint.MaxCalendarReminderTimeSpan;
			}
		}

		public override PropertyConstraintViolationError Validate(object value, PropertyDefinition propertyDefinition, IPropertyBag propertyBag)
		{
			TimeSpan t = (TimeSpan)value;
			if (t < TimeSpan.Zero)
			{
				return new PropertyConstraintViolationError(ServerStrings.ErrorCalendarReminderNegative(value.ToString()), propertyDefinition, value, this);
			}
			if (t > CalendarReminderTimeSpanConstraint.MaxCalendarReminderTimeSpan)
			{
				return new PropertyConstraintViolationError(ServerStrings.ErrorCalendarReminderTooLarge(value.ToString()), propertyDefinition, value, this);
			}
			if (t.Seconds != 0 || t.Milliseconds != 0)
			{
				return new PropertyConstraintViolationError(ServerStrings.ErrorCalendarReminderNotMinutes(value.ToString()), propertyDefinition, value, this);
			}
			return null;
		}

		private static readonly TimeSpan MaxCalendarReminderTimeSpan = new TimeSpan(1059202, 23, 59, 0);
	}
}
