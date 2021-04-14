using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxiedSubscriptionCallFailureException : ServicePermanentException
	{
		public ProxiedSubscriptionCallFailureException() : base(CoreResources.IDs.ErrorProxiedSubscriptionCallFailure)
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
