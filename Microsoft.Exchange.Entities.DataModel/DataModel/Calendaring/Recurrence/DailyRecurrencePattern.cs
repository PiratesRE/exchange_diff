using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class DailyRecurrencePattern : RecurrencePattern
	{
		public override RecurrencePatternType Type
		{
			get
			{
				return RecurrencePatternType.Daily;
			}
		}
	}
}
