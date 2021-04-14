using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class InvalidNameForNameResolutionException : ServicePermanentException
	{
		public InvalidNameForNameResolutionException() : base((CoreResources.IDs)4279571010U)
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
