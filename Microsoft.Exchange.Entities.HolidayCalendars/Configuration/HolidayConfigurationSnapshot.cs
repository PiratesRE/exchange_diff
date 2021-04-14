using System;
using Microsoft.Exchange.Entities.HolidayCalendars.Configuration.Exceptions;
using Microsoft.Exchange.HolidayCalendars;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Entities.HolidayCalendars.Configuration
{
	internal class HolidayConfigurationSnapshot
	{
		public HolidayConfigurationSnapshot(VariantConfigurationSnapshot configurationSnapshot) : this(configurationSnapshot.HolidayCalendars.HostConfiguration)
		{
		}

		public HolidayConfigurationSnapshot(IHostSettings hostSettings)
		{
			this.hostSettings = hostSettings;
		}

		public Uri CalendarEndpoint
		{
			get
			{
				if (this.endpointUrl != null)
				{
					return this.endpointUrl;
				}
				if (string.IsNullOrWhiteSpace(this.hostSettings.Endpoint))
				{
					throw new NoEndpointConfigurationFoundException("Endpoint configuration is not available. Endpoint: '{0}'", new object[]
					{
						this.hostSettings.Endpoint
					});
				}
				string endpoint = this.hostSettings.Endpoint;
				if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
				{
					throw new InvalidHolidayCalendarEndpointUrlException("User is enabled without valid EndPoint configuration URL. {0}", new object[]
					{
						endpoint
					});
				}
				this.endpointUrl = new Uri(endpoint);
				return this.endpointUrl;
			}
		}

		public int ConfigurationFetchTimeout
		{
			get
			{
				return this.hostSettings.Timeout;
			}
		}

		private readonly IHostSettings hostSettings;

		private Uri endpointUrl;
	}
}
