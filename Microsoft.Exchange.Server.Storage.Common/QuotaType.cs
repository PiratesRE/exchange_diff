using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public enum QuotaType
	{
		Undefined,
		StorageWarningLimit,
		StorageOverQuotaLimit,
		StorageShutoff,
		DumpsterWarningLimit,
		DumpsterShutoff,
		MailboxMessagesPerFolderCountWarningQuota,
		MailboxMessagesPerFolderCountReceiveQuota,
		DumpsterMessagesPerFolderCountWarningQuota,
		DumpsterMessagesPerFolderCountReceiveQuota,
		FolderHierarchyChildrenCountWarningQuota,
		FolderHierarchyChildrenCountReceiveQuota,
		FolderHierarchyDepthWarningQuota,
		FolderHierarchyDepthReceiveQuota,
		FoldersCountWarningQuota,
		FoldersCountReceiveQuota,
		NamedPropertiesCountQuota,
		MaxQuotaType
	}
}
