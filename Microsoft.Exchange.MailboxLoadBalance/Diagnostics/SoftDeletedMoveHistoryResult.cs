using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class SoftDeletedMoveHistoryResult
	{
		[DataMember]
		public Guid MailboxGuid { get; set; }

		[DataMember]
		public Guid TargetDatabaseGuid { get; set; }

		[DataMember]
		public Guid SourceDatabaseGuid { get; set; }

		[DataMember]
		public SoftDeletedMoveHistory MoveHistory { get; set; }
	}
}
