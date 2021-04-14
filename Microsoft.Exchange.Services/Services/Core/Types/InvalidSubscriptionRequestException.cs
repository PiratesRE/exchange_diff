using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidSubscriptionRequestException : ServicePermanentException
	{
		public InvalidSubscriptionRequestException() : base((CoreResources.IDs)3647226175U)
		{
		}

		public InvalidSubscriptionRequestException(Enum messageId) : base(ResponseCodeType.ErrorInvalidSubscriptionRequest, messageId)
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
