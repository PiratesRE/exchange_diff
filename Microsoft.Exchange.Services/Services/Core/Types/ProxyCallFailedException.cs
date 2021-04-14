using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyCallFailedException : ServicePermanentException
	{
		public ProxyCallFailedException() : base((CoreResources.IDs)3032417457U)
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
