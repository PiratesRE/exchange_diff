using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IWeatherConfigurationCache
	{
		bool IsFeatureEnabled { get; }

		bool IsRestrictedCulture(string culture);

		string WeatherServiceUrl { get; }
	}
}
