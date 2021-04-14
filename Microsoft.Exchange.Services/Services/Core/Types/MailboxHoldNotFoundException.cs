using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class MailboxHoldNotFoundException : ServicePermanentException
	{
		public MailboxHoldNotFoundException(Enum messageId) : base(ResponseCodeType.ErrorMailboxHoldNotFound, messageId)
		{
		}

		public MailboxHoldNotFoundException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorMailboxHoldNotFound, messageId, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007SP1;
			}
		}
	}
}
