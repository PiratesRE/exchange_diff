using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WeatherCurrentConditions
	{
		[DataMember]
		public WeatherProviderAttribution Attribution { get; set; }

		[DataMember]
		public int Temperature { get; set; }

		[DataMember]
		public int SkyCode { get; set; }

		[DataMember]
		public string SkyText { get; set; }
	}
}
