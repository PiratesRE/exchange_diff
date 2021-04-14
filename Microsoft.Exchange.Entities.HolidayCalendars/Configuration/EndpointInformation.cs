using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class EndpointInformation
	{
		public EndpointInformation(Uri baseUrl, IReadOnlyDictionary<int, int> calendarVersionMapping, IReadOnlyList<CultureInfo> availableCultures, Version dropVersion)
		{
			this.BaseUrl = baseUrl;
			this.CalendarVersionMapping = calendarVersionMapping;
			this.AvailableCultures = availableCultures;
			this.DropVersion = dropVersion;
		}

		public Uri BaseUrl { get; private set; }

		public IReadOnlyDictionary<int, int> CalendarVersionMapping { get; private set; }

		public IReadOnlyList<CultureInfo> AvailableCultures { get; private set; }

		public Version DropVersion { get; private set; }
	}
}
