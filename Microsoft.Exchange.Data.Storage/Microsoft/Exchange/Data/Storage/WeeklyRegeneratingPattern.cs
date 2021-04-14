using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WeeklyRegeneratingPattern : RegeneratingPattern
	{
		public WeeklyRegeneratingPattern(int recurrenceInterval)
		{
			base.RecurrenceInterval = recurrenceInterval;
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			return value is WeeklyRegeneratingPattern && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
		}

		internal override LocalizedString When()
		{
			if (base.RecurrenceInterval == 1)
			{
				return ClientStrings.TaskWhenWeeklyRegeneratingPattern;
			}
			return ClientStrings.TaskWhenNWeeksRegeneratingPattern(base.RecurrenceInterval);
		}
	}
}
