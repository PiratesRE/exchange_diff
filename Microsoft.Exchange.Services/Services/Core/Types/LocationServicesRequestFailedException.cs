using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LocationServicesRequestFailedException : ServicePermanentException
	{
		public LocationServicesRequestFailedException(Exception innerException) : base(ResponseCodeType.ErrorLocationServicesRequestFailed, (CoreResources.IDs)2653243941U, innerException)
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
