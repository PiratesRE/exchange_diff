using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class WeatherLocationId
	{
		[DataMember]
		public double Latitude { get; set; }

		[DataMember]
		public double Longitude { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string EntityId { get; set; }
	}
}
