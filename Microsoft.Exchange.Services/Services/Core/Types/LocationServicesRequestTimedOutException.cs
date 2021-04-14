using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LocationServicesRequestTimedOutException : ServicePermanentException
	{
		public LocationServicesRequestTimedOutException() : base(ResponseCodeType.ErrorLocationServicesRequestTimedOut, (CoreResources.IDs)4226485813U)
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
