using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class LocationServicesInvalidRequestException : ServicePermanentException
	{
		public LocationServicesInvalidRequestException(Enum messageId) : base(ResponseCodeType.ErrorLocationServicesInvalidRequest, messageId)
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
