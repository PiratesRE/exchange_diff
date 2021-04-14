using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class FindPlacesRequest
	{
		[DataMember]
		public string Query { get; set; }

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public PlacesSource Sources { get; set; }

		[DataMember]
		public int MaxResults { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public double? Latitude { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public double? Longitude { get; set; }

		[DataMember]
		public string Culture { get; set; }
	}
}
