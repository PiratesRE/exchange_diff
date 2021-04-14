using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailCertificate
	{
		[DataMember]
		public string FriendlyName { get; set; }

		[DataMember]
		public string Thumbprint { get; set; }

		[DataMember]
		public string Subject { get; set; }

		[DataMember]
		public string Issuer { get; set; }
	}
}
