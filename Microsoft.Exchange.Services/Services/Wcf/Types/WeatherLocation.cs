using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WeatherLocation
	{
		[DataMember]
		public WeatherLocationId Id { get; set; }

		[DataMember]
		public string ErrorMessage { get; set; }

		[DataMember]
		public WeatherCurrentConditions Conditions { get; set; }

		[DataMember]
		public WeatherMultidayForecast MultidayForecast { get; set; }
	}
}
