using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetWeatherForecastResponse : BaseJsonResponse
	{
		internal GetWeatherForecastResponse()
		{
		}

		[DataMember(IsRequired = true)]
		public WeatherLocation[] WeatherLocations { get; set; }

		[DataMember(IsRequired = false)]
		public string ErrorMessage { get; set; }

		[DataMember(IsRequired = true)]
		public int PollingWindowInMinutes { get; set; }

		[DataMember(IsRequired = true)]
		public TemperatureUnit TemperatureUnit { get; set; }
	}
}
