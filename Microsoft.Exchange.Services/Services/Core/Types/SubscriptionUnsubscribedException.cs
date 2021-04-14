using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SubscriptionUnsubscribedException : ServicePermanentException
	{
		public SubscriptionUnsubscribedException() : base(CoreResources.IDs.ErrorSubscriptionUnsubscribed)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010SP1;
			}
		}
	}
}
