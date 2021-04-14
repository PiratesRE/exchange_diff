using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class NoApplicableProxyCASServersAvailableException : ServicePermanentException
	{
		public NoApplicableProxyCASServersAvailableException() : base((CoreResources.IDs)4164112684U)
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
