using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class NameResolutionMultipleResultsException : ServicePermanentException
	{
		public NameResolutionMultipleResultsException() : base(CoreResources.IDs.ErrorNameResolutionMultipleResults)
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
