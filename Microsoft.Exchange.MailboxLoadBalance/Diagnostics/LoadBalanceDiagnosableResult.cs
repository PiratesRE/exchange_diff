using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Diagnostics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal struct LoadBalanceDiagnosableResult
	{
		[DataMember]
		public DirectoryIdentity DatabaseToDrain { get; set; }

		[DataMember]
		public BatchName DrainBatchName { get; set; }

		[DataMember]
		public QueueManagerDiagnosticData QueueManager { get; set; }

		[DataMember]
		public IList<BandMailboxRebalanceData> RebalanceResults { get; set; }

		[DataMember]
		public SoftDeletedMailboxRemovalResult SoftDeletedMailboxRemovalResult { get; set; }

		[DataMember]
		public SoftDeletedMoveHistoryResult SoftDeletedMoveHistoryResult { get; set; }
	}
}
