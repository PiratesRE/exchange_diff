using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DailyRecurrencePattern : IntervalRecurrencePattern
	{
		public DailyRecurrencePattern()
		{
		}

		public DailyRecurrencePattern(int recurrenceInterval)
		{
			base.RecurrenceInterval = recurrenceInterval;
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			return value is DailyRecurrencePattern && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
		}

		internal override LocalizedString When()
		{
			LocalizedString result;
			if (base.RecurrenceInterval == 1)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.CalendarWhenDailyEveryDay;
				}
				else
				{
					result = ClientStrings.TaskWhenDailyEveryDay;
				}
			}
			else if (base.RecurrenceInterval == 2)
			{
				if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
				{
					result = ClientStrings.CalendarWhenEveryOtherDay;
				}
				else
				{
					result = ClientStrings.TaskWhenEveryOtherDay;
				}
			}
			else if (base.RecurrenceObjectType == RecurrenceObjectType.CalendarRecurrence)
			{
				result = ClientStrings.CalendarWhenDailyEveryNDays(base.RecurrenceInterval);
			}
			else
			{
				result = ClientStrings.TaskWhenDailyEveryNDays(base.RecurrenceInterval);
			}
			return result;
		}

		internal override RecurrenceType MapiRecurrenceType
		{
			get
			{
				return RecurrenceType.Daily;
			}
		}
	}
}
