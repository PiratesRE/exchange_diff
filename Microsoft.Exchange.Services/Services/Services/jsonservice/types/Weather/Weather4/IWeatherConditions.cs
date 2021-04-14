using System;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	public interface IWeatherConditions
	{
		int SkyCode { get; set; }

		string SkyText { get; set; }

		int Temperature { get; set; }
	}
}
