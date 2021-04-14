using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Core.Types
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class WeatherServiceDisabledException : ServicePermanentException
	{
		public WeatherServiceDisabledException() : base(ResponseCodeType.ErrorWeatherServiceDisabled, (CoreResources.IDs)4210036349U)
		{
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Current;
			}
		}
	}
}
