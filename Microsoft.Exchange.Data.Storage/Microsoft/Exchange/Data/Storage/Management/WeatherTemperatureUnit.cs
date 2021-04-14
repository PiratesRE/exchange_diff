using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum WeatherTemperatureUnit
	{
		[LocDescription(ServerStrings.IDs.WeatherUnitDefault)]
		Default,
		[LocDescription(ServerStrings.IDs.WeatherUnitCelsius)]
		Celsius,
		[LocDescription(ServerStrings.IDs.WeatherUnitFahrenheit)]
		Fahrenheit
	}
}
