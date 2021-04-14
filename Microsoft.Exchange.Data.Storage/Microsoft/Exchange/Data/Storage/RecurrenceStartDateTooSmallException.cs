using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecurrenceStartDateTooSmallException : RecurrenceException
	{
		public RecurrenceStartDateTooSmallException(ExDateTime startDate, LocalizedString message) : base(message)
		{
			this.StartDate = startDate;
		}

		public static readonly ExDateTime MinimumAllowedStartDate = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, 1601, 4, 1, 0, 0, 0);

		public readonly ExDateTime StartDate;
	}
}
