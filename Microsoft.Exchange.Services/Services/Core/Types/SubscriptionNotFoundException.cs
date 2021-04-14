using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SubscriptionNotFoundException : ServicePermanentException
	{
		public SubscriptionNotFoundException() : base((CoreResources.IDs)2884324330U)
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
