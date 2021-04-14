using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class AbsoluteMonthlyRecurrencePattern : RecurrencePattern
	{
		public int DayOfMonth { get; set; }

		public override RecurrencePatternType Type
		{
			get
			{
				return RecurrencePatternType.AbsoluteMonthly;
			}
		}
	}
}
