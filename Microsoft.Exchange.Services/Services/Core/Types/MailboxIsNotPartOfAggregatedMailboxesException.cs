using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MailboxIsNotPartOfAggregatedMailboxesException : ServicePermanentException
	{
		public MailboxIsNotPartOfAggregatedMailboxesException(Enum messageId) : base(ResponseCodeType.ErrorMailboxIsNotPartOfAggregatedMailboxes, messageId)
		{
		}

		public MailboxIsNotPartOfAggregatedMailboxesException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorMailboxIsNotPartOfAggregatedMailboxes, messageId, innerException)
		{
		}

		public MailboxIsNotPartOfAggregatedMailboxesException(ResponseCodeType responseCodeType, LocalizedString message) : base(responseCodeType, message)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2013;
			}
		}
	}
}
