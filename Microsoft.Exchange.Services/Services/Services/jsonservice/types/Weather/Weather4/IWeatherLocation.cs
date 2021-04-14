using System;

namespace Microsoft.Exchange.Services.Services.jsonservice.types.Weather.Weather4
{
	public interface IWeatherLocation
	{
		string Attribution { get; set; }

		WeatherConditions Conditions { get; set; }

		string DegreeUnit { get; set; }

		long EntityId { get; set; }

		string ErrorMessage { get; set; }

		WeatherForecast[] Forecast { get; set; }

		string FullName { get; set; }

		double Latitude { get; set; }

		double Longitude { get; set; }

		string Name { get; set; }

		string ShortAttribution { get; set; }

		string Url { get; set; }
	}
}
