using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DailyRegeneratingPattern : RegeneratingPattern
	{
		public DailyRegeneratingPattern(int recurrenceInterval)
		{
			base.RecurrenceInterval = recurrenceInterval;
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			return value is DailyRegeneratingPattern && base.Equals(value, ignoreCalendarTypeAndIsLeapMonth);
		}

		internal override LocalizedString When()
		{
			if (base.RecurrenceInterval == 1)
			{
				return ClientStrings.TaskWhenDailyRegeneratingPattern;
			}
			return ClientStrings.TaskWhenNDaysRegeneratingPattern(base.RecurrenceInterval);
		}
	}
}
