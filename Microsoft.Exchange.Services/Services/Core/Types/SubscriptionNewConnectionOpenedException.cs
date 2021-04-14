using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SubscriptionNewConnectionOpenedException : ServicePermanentException
	{
		public SubscriptionNewConnectionOpenedException() : base(ResponseCodeType.ErrorNewEventStreamConnectionOpened, (CoreResources.IDs)2943900075U)
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
