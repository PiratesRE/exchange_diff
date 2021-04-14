using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LocationServicesDisabledException : ServicePermanentException
	{
		public LocationServicesDisabledException() : base(ResponseCodeType.ErrorLocationServicesDisabled, (CoreResources.IDs)2451443999U)
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
