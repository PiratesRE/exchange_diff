using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RecurrenceHasNoOccurrenceException : RecurrenceException
	{
		public RecurrenceHasNoOccurrenceException(ExDateTime effectiveStartDate, ExDateTime effectiveEndDate, LocalizedString message) : base(message)
		{
			this.EffectiveEndDate = effectiveEndDate;
			this.EffectiveStartDate = effectiveStartDate;
		}

		public readonly ExDateTime EffectiveStartDate;

		public readonly ExDateTime EffectiveEndDate;
	}
}
