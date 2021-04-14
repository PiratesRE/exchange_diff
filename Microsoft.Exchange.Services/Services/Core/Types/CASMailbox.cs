using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CASMailbox : SetCASMailbox
	{
		[DataMember]
		public bool ActiveSyncEnabled { get; set; }

		[DataMember]
		public string ExternalImapSettings { get; set; }

		[DataMember]
		public string ExternalPopSettings { get; set; }

		[DataMember]
		public string ExternalSmtpSettings { get; set; }

		[DataMember]
		public bool ImapEnabled { get; set; }

		[DataMember]
		public string InternalImapSettings { get; set; }

		[DataMember]
		public string InternalPopSettings { get; set; }

		[DataMember]
		public string InternalSmtpSettings { get; set; }

		[DataMember]
		public bool MAPIEnabled { get; set; }

		[DataMember]
		public bool OWAEnabled { get; set; }

		[DataMember]
		public bool PopEnabled { get; set; }
	}
}
