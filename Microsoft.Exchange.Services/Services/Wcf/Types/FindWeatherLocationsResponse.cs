using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindWeatherLocationsResponse : BaseJsonResponse
	{
		internal FindWeatherLocationsResponse()
		{
		}

		[DataMember(IsRequired = false)]
		public string ErrorMessage { get; set; }

		[DataMember(IsRequired = true)]
		public WeatherLocationId[] WeatherLocationIds { get; set; }
	}
}
