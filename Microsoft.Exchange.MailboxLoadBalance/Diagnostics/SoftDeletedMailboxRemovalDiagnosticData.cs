using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SoftDeletedMailboxRemovalDiagnosticData : CmdletRequestDiagnosticData
	{
		public SoftDeletedMailboxRemovalDiagnosticData(Guid databaseGuid, Guid mailboxGuid)
		{
			this.MailboxGuid = mailboxGuid;
			this.DatabaseGuid = databaseGuid;
		}

		[DataMember]
		public Guid MailboxGuid { get; private set; }

		[DataMember]
		public Guid DatabaseGuid { get; private set; }
	}
}
