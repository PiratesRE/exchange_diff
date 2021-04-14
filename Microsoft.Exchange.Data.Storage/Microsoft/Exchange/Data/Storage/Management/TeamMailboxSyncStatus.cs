using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum TeamMailboxSyncStatus
	{
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusNotAvailable)]
		NotAvailable,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusSucceeded)]
		Succeeded,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusFailed)]
		Failed,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusDocumentSyncFailureOnly)]
		DocumentSyncFailureOnly,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusMembershipSyncFailureOnly)]
		MembershipSyncFailureOnly,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusMaintenanceSyncFailureOnly)]
		MaintenanceSyncFailureOnly,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusDocumentAndMembershipSyncFailure)]
		DocumentAndMembershipSyncFailure,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusMembershipAndMaintenanceSyncFailure)]
		MembershipAndMaintenanceSyncFailure,
		[LocDescription(ServerStrings.IDs.TeamMailboxSyncStatusDocumentAndMaintenanceSyncFailure)]
		DocumentAndMaintenanceSyncFailure
	}
}
