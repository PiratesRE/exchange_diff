using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	[Serializable]
	internal sealed class AffectedTaskOccurrencesRequiredException : ServicePermanentException
	{
		public AffectedTaskOccurrencesRequiredException() : base((CoreResources.IDs)2684918840U)
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
