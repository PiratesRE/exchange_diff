using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public sealed class EndDateRecurrenceRange : RecurrenceRange
	{
		public ExDateTime EndDate { get; set; }

		public override RecurrenceRangeType Type
		{
			get
			{
				return RecurrenceRangeType.EndDate;
			}
		}
	}
}
