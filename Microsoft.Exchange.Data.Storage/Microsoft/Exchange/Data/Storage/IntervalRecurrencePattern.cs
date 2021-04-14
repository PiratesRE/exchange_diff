using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class IntervalRecurrencePattern : RecurrencePattern
	{
		public int RecurrenceInterval
		{
			get
			{
				return this.recurrenceInterval;
			}
			protected set
			{
				if (value < 1 || value > StorageLimits.Instance.RecurrenceMaximumInterval)
				{
					throw new ArgumentOutOfRangeException(ServerStrings.ExInvalidRecurrenceInterval(value), "RecurrenceInterval");
				}
				this.recurrenceInterval = value;
			}
		}

		public override bool Equals(RecurrencePattern value, bool ignoreCalendarTypeAndIsLeapMonth)
		{
			if (!(value is IntervalRecurrencePattern))
			{
				return false;
			}
			IntervalRecurrencePattern intervalRecurrencePattern = (IntervalRecurrencePattern)value;
			return intervalRecurrencePattern.RecurrenceInterval == this.recurrenceInterval;
		}

		private int recurrenceInterval = 1;
	}
}
