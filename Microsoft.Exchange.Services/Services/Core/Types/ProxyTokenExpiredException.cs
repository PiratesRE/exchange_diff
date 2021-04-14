using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class ProxyTokenExpiredException : ServicePermanentException
	{
		public ProxyTokenExpiredException() : base((CoreResources.IDs)3699987394U)
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
