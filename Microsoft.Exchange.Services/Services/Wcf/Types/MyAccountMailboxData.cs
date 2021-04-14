using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class MyAccountMailboxData : OptionsPropertyChangeTracker
	{
		[DataMember]
		public UnlimitedUnsignedInteger IssueWarningQuota { get; set; }

		[DataMember]
		public UnlimitedUnsignedInteger ProhibitSendQuota { get; set; }

		[DataMember]
		public UnlimitedUnsignedInteger ProhibitSendReceiveQuota { get; set; }

		[DataMember]
		public bool UseDatabaseQuotaDefaults { get; set; }
	}
}
