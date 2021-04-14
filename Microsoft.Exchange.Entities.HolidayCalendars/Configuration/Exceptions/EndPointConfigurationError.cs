using System;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions
{
	public enum EndPointConfigurationError : uint
	{
		UnableToFetchListOfCultures = 1U,
		UnableToFetchCalendarVersions,
		UrlDidNotResolveToHttpRequest,
		UrlSchemeNotSupported,
		VersionNumberError
	}
}
