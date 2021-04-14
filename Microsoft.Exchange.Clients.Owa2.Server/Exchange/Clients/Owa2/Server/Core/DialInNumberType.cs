using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(JsonFaultResponse))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class DialInNumberType
	{
		[DataMember]
		public string RegionName { get; set; }

		[DataMember]
		public string Number { get; set; }

		[DataMember]
		public string Language { get; set; }
	}
}
