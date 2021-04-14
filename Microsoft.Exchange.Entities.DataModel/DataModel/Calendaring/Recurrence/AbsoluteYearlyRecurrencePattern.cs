using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class AbsoluteYearlyRecurrencePattern : RecurrencePattern
	{
		public int DayOfMonth { get; set; }

		public int Month { get; set; }

		public override RecurrencePatternType Type
		{
			get
			{
				return RecurrencePatternType.AbsoluteYearly;
			}
		}
	}
}
