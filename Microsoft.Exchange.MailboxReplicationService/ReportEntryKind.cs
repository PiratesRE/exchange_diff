using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum ReportEntryKind
	{
		SourceConnection,
		TargetConnection,
		SignaturePreservation,
		StartingFolderHierarchyCreation,
		AggregatedSoftDeletedMessages,
		HierarchyChanges
	}
}
