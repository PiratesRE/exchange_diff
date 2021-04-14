using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class RelativeMonthlyRecurrencePattern : RecurrencePattern
	{
		public ISet<DayOfWeek> DaysOfWeek { get; set; }

		public WeekIndex Index { get; set; }

		public override RecurrencePatternType Type
		{
			get
			{
				return RecurrencePatternType.RelativeMonthly;
			}
		}
	}
}
