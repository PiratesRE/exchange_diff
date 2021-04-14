using System;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Entities.DataModel.Calendaring.Recurrence
{
	public abstract class RecurrenceRange
	{
		public abstract RecurrenceRangeType Type { get; }

		public ExDateTime StartDate { get; set; }
	}
}
