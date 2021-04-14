using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class UrlResolver
	{
		public UrlResolver(EndpointInformation endpointInformation)
		{
			if (endpointInformation == null)
			{
				throw new ArgumentNullException("endpointInformation");
			}
			this.endpointInformation = endpointInformation;
		}

		public Uri ResolveUrl(Uri holidayCalendarUrl, CultureInfo culture)
		{
			if (holidayCalendarUrl == null)
			{
				throw new ArgumentNullException("holidayCalendarUrl");
			}
			if (holidayCalendarUrl.Scheme != "holidays" || holidayCalendarUrl.Host != "outlook" || holidayCalendarUrl.Segments.Length < 2)
			{
				throw new InvalidHolidayCalendarUrlException("The holiday calendar url is invalid. {0}", new object[]
				{
					holidayCalendarUrl
				});
			}
			string s = holidayCalendarUrl.Segments[1].TrimEnd(new char[]
			{
				'/'
			});
			int num;
			if (!int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out num))
			{
				throw new InvalidHolidayCalendarUrlException("Holiday Calendar id is not valid. {0}", new object[]
				{
					holidayCalendarUrl
				});
			}
			if (holidayCalendarUrl.Segments.Length > 2)
			{
				string name = holidayCalendarUrl.Segments[2].TrimEnd(new char[]
				{
					'/'
				});
				try
				{
					culture = CultureInfo.GetCultureInfo(name);
				}
				catch (CultureNotFoundException)
				{
					throw new InvalidHolidayCalendarUrlException("Specified holiday calendar culture is invalid. {0}", new object[]
					{
						holidayCalendarUrl
					});
				}
			}
			CultureInfo cultureInfo = this.FindSupportedCulture(culture, this.endpointInformation.AvailableCultures);
			if (cultureInfo == null)
			{
				throw new NoSupportedHolidayCalendarCultureException("Culture {0} is not supported by the given endpoint {1}", new object[]
				{
					culture,
					this.endpointInformation.BaseUrl
				});
			}
			UriBuilder uriBuilder = new UriBuilder(this.endpointInformation.BaseUrl);
			uriBuilder.Port = -1;
			UriBuilder uriBuilder2 = uriBuilder;
			uriBuilder2.Path += string.Format("{0}/{1}.ics", cultureInfo.Name, num);
			return uriBuilder.Uri;
		}

		private CultureInfo FindSupportedCulture(CultureInfo culture, IEnumerable<CultureInfo> availableCultures)
		{
			CultureInfo cultureInfo = culture;
			while (cultureInfo != null)
			{
				foreach (CultureInfo cultureInfo2 in availableCultures)
				{
					if (cultureInfo.Equals(cultureInfo2))
					{
						return cultureInfo2;
					}
				}
				if (!cultureInfo.Equals(cultureInfo.Parent))
				{
					cultureInfo = cultureInfo.Parent;
					continue;
				}
				break;
			}
			return null;
		}

		public const string HolidayCalendarPredefinedHost = "outlook";

		private readonly EndpointInformation endpointInformation;
	}
}
