using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class CannotFindUnifiedMailboxException : ServicePermanentException
	{
		public CannotFindUnifiedMailboxException(Enum messageId) : base(ResponseCodeType.ErrorCannotFindUnifiedMailbox, messageId)
		{
		}

		public CannotFindUnifiedMailboxException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorCannotFindUnifiedMailbox, messageId, innerException)
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
