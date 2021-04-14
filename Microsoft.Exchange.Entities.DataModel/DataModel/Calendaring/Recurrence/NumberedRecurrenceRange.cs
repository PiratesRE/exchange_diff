using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class NumberedRecurrenceRange : RecurrenceRange
	{
		public int NumberOfOccurrences { get; set; }

		public override RecurrenceRangeType Type
		{
			get
			{
				return RecurrenceRangeType.Numbered;
			}
		}
	}
}
