using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UcwaUserConfiguration
	{
		[DataMember]
		public string SipUri { get; set; }

		[DataMember]
		public bool IsUcwaSupported { get; set; }

		[DataMember(IsRequired = false)]
		public string DiagnosticInfo { get; set; }

		public string UcwaUrl { get; set; }
	}
}
