using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(JsonFaultResponse))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class AcpInformationType
	{
		[DataMember]
		public string ParticipantPassCode { get; set; }

		[DataMember(IsRequired = false)]
		public string[] TollFreeNumbers { get; set; }

		[DataMember(IsRequired = false)]
		public string TollNumber { get; set; }
	}
}
