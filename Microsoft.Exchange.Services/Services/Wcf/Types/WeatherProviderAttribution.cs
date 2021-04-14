using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WeatherProviderAttribution
	{
		[DataMember]
		public string Text { get; set; }

		[DataMember]
		public string Link { get; set; }
	}
}
