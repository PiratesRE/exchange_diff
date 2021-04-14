using System;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class NoEndRecurrenceRange : RecurrenceRange
	{
		public override RecurrenceRangeType Type
		{
			get
			{
				return RecurrenceRangeType.NoEnd;
			}
		}
	}
}
