using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GetCertsInfoResponse
	{
		[DataMember]
		public string CertsRawData { get; set; }

		[DataMember]
		public bool IsValid { get; set; }

		[DataMember]
		public uint PolicyFlag { get; set; }

		[DataMember]
		public uint ChainValidityStatus { get; set; }

		[DataMember]
		public string DisplayedId { get; set; }

		[DataMember]
		public string DisplayName { get; set; }

		[DataMember]
		public string Issuer { get; set; }

		[DataMember]
		public string ChainData { get; set; }

		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public string SubjectKeyIdentifier { get; set; }
	}
}
