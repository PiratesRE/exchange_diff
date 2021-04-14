using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WeatherMultidayForecast
	{
		[DataMember]
		public WeatherProviderAttribution Attribution { get; set; }

		[DataMember]
		public WeatherDailyConditions[] DailyForecasts { get; set; }
	}
}
