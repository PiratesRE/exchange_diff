using System;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	public interface IWeatherForecast
	{
		int High { get; set; }

		int Low { get; set; }

		int PrecipitationCertainty { get; set; }

		int SkyCodeDay { get; set; }

		string SkyTextDay { get; set; }
	}
}
