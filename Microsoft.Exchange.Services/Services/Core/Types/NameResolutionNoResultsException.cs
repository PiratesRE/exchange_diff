using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NameResolutionNoResultsException : ServicePermanentException
	{
		public NameResolutionNoResultsException() : base(CoreResources.IDs.ErrorNameResolutionNoResults)
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
