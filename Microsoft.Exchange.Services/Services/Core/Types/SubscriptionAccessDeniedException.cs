using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SubscriptionAccessDeniedException : ServicePermanentException
	{
		public SubscriptionAccessDeniedException() : base((CoreResources.IDs)2662672540U)
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
