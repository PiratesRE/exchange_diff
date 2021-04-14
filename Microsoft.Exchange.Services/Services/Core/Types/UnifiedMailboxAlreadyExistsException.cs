using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class UnifiedMailboxAlreadyExistsException : ServicePermanentException
	{
		public UnifiedMailboxAlreadyExistsException(Enum messageId) : base(ResponseCodeType.ErrorUnifiedMailboxAlreadyExists, messageId)
		{
		}

		public UnifiedMailboxAlreadyExistsException(Enum messageId, Exception innerException) : base(ResponseCodeType.ErrorUnifiedMailboxAlreadyExists, messageId, innerException)
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
