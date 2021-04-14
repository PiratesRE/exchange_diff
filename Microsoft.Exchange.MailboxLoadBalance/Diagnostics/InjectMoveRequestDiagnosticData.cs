using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	internal class InjectMoveRequestDiagnosticData : CmdletRequestDiagnosticData
	{
		[DataMember]
		public bool ArchiveOnly { get; set; }

		[DataMember]
		public DirectoryMailbox Mailbox { get; set; }

		[DataMember]
		public bool Protect { get; set; }

		[DataMember]
		public DirectoryIdentity TargetDatabase { get; set; }
	}
}
