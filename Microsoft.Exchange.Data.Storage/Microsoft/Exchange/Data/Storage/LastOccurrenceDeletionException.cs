using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LastOccurrenceDeletionException : RecurrenceException
	{
		public LastOccurrenceDeletionException(ExDateTime dateId, LocalizedString message) : base(message)
		{
			this.DateId = dateId;
		}

		public readonly ExDateTime DateId;
	}
}
