using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyGroupSidLimitExceededException : ServicePermanentException
	{
		public ProxyGroupSidLimitExceededException() : base(CoreResources.IDs.ErrorProxyGroupSidLimitExceeded)
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
