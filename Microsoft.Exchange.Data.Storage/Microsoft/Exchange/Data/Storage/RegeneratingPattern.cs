using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RegeneratingPattern : IntervalRecurrencePattern
	{
		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			return value is RegeneratingPattern && ((RegeneratingPattern)value).RecurrenceInterval == base.RecurrenceInterval;
		}
	}
}
