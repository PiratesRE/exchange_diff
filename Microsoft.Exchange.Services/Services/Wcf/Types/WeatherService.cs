using System;
using System.Net;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	internal class WeatherService : IWeatherService
	{
		public WeatherService(IWeatherConfigurationCache weatherConfigurationCache)
		{
			this.weatherConfigurationCache = weatherConfigurationCache;
		}

		public string Get(Uri weatherServiceUri)
		{
			string result;
			using (WebClient webClient = new WebClient())
			{
				result = webClient.DownloadString(weatherServiceUri);
			}
			return result;
		}

		public void VerifyServiceAvailability(CallContext callContext)
		{
			if (!VariantConfiguration.GetSnapshot(callContext.AccessingADUser.GetContext(null), null, null).OwaClientServer.Weather.Enabled || !this.weatherConfigurationCache.IsFeatureEnabled)
			{
				throw FaultExceptionUtilities.CreateFault(new WeatherServiceDisabledException(), FaultParty.Sender);
			}
		}

		public string PartnerId
		{
			get
			{
				return "owa";
			}
		}

		public string BaseUrl
		{
			get
			{
				return this.weatherConfigurationCache.WeatherServiceUrl;
			}
		}

		private const string PartnerIdValue = "owa";

		private readonly IWeatherConfigurationCache weatherConfigurationCache;
	}
}
