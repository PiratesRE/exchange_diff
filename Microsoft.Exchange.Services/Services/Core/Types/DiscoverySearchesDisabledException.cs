using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class DiscoverySearchesDisabledException : ServicePermanentException
	{
		public DiscoverySearchesDisabledException() : base(ResponseCodeType.ErrorDiscoverySearchesDisabled, CoreResources.IDs.ErrorDiscoverySearchesDisabled)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2012;
			}
		}
	}
}
