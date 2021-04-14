using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidPushSubscriptionUrlException : ServicePermanentException
	{
		public InvalidPushSubscriptionUrlException() : base(CoreResources.IDs.ErrorInvalidPushSubscriptionUrl)
		{
		}

		public InvalidPushSubscriptionUrlException(Exception innerException) : base(CoreResources.IDs.ErrorInvalidPushSubscriptionUrl, innerException)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2007;
			}
		}
	}
}
