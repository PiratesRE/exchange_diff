using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum MailboxCapabilities
	{
		PagedEnumerateChanges,
		RunIsInteg,
		ExtendedAclInformation,
		PagedEnumerateHierarchyChanges,
		FolderRules,
		FolderAcls,
		ExportFolders,
		MaxElement,
		PagedGetActions = 1011,
		ReplayActions = 1111
	}
}
