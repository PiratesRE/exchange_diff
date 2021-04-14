using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnifiedMailboxSupportedOnlyWithMicrosoftAccountException : ServicePermanentException
	{
		public UnifiedMailboxSupportedOnlyWithMicrosoftAccountException(Enum messageId) : base(ResponseCodeType.ErrorUnifiedMailboxSupportedOnlyWithMicrosoftAccount, messageId)
		{
		}

		public UnifiedMailboxSupportedOnlyWithMicrosoftAccountException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorUnifiedMailboxSupportedOnlyWithMicrosoftAccount, messageId, innerException)
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
