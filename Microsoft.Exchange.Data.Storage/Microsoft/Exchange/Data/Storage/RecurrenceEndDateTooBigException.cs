using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecurrenceEndDateTooBigException : RecurrenceException
	{
		public RecurrenceEndDateTooBigException(ExDateTime endDate, LocalizedString message) : base(message)
		{
			this.EndDate = endDate;
		}

		public readonly ExDateTime EndDate;

		public static readonly ExDateTime MaximumAllowedEndDate = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 4500, 9, 1, 0, 0, 0);
	}
}
