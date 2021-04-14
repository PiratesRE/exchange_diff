using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class InvalidSubscriptionException : ServicePermanentException
	{
		public InvalidSubscriptionException() : base(CoreResources.IDs.ErrorInvalidSubscription)
		{
		}

		public InvalidSubscriptionException(Exception innerException) : base(CoreResources.IDs.ErrorInvalidSubscription, innerException)
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
