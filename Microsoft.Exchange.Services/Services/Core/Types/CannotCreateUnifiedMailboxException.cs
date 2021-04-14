using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotCreateUnifiedMailboxException : ServicePermanentException
	{
		public CannotCreateUnifiedMailboxException(Enum messageId) : base(ResponseCodeType.ErrorCannotCreateUnifiedMailbox, messageId)
		{
		}

		public CannotCreateUnifiedMailboxException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotCreateUnifiedMailbox, messageId, innerException)
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
