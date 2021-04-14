using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ServiceDiscoveryFailedException : ServicePermanentException
	{
		public ServiceDiscoveryFailedException(Exception innerException) : base(CoreResources.IDs.ErrorProxyServiceDiscoveryFailed, innerException)
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
