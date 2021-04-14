using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class NoRespondingCASInDestinationSiteException : ServicePermanentException
	{
		public NoRespondingCASInDestinationSiteException() : base((CoreResources.IDs)4252309617U)
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
