using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCertsResponse
	{
		[DataMember]
		public EmailAddressWrapper[][] InvalidRecipients { get; set; }

		[DataMember]
		public string[][] ValidRecipients { get; set; }

		[DataMember]
		public string[] CertsRawData { get; set; }

		[DataMember]
		public GetCertsErrorStatus ErrorStatus { get; set; }

		[DataMember]
		public string[] ErrorDetails { get; set; }
	}
}
