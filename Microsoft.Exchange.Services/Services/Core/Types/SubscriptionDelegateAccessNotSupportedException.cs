using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class SubscriptionDelegateAccessNotSupportedException : ServicePermanentException
	{
		public SubscriptionDelegateAccessNotSupportedException() : base((CoreResources.IDs)3640136739U)
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
