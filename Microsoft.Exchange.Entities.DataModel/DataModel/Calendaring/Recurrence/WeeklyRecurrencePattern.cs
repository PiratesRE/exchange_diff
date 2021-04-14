using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class WeeklyRecurrencePattern : RecurrencePattern
	{
		public DayOfWeek FirstDayOfWeek { get; set; }

		public override RecurrencePatternType Type
		{
			get
			{
				return RecurrencePatternType.Weekly;
			}
		}

		public ISet<DayOfWeek> DaysOfWeek { get; set; }
	}
}
