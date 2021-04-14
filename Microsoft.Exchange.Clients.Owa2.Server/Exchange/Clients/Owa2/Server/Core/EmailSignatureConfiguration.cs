using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class EmailSignatureConfiguration
	{
		[DataMember]
		public bool AutoAddSignature { get; set; }

		[DataMember]
		public string SignatureText { get; set; }

		[DataMember]
		public bool UseDesktopSignature { get; set; }

		[DataMember]
		public string DesktopSignatureText { get; set; }
	}
}
