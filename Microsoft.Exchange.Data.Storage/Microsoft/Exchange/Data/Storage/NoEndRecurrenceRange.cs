using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NoEndRecurrenceRange : RecurrenceRange
	{
		public NoEndRecurrenceRange(ExDateTime startDate)
		{
			this.StartDate = startDate;
		}

		public override bool Equals(RecurrenceRange value)
		{
			return value is NoEndRecurrenceRange && base.Equals(value);
		}

		public override string ToString()
		{
			return string.Format("Starts {0}, no end date", this.StartDate.ToString(DateTimeFormatInfo.InvariantInfo));
		}
	}
}
