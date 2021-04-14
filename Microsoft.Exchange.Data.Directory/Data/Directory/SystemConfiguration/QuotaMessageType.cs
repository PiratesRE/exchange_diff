using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum QuotaMessageType
	{
		WarningMailboxUnlimitedSize,
		WarningPublicFolderUnlimitedSize,
		WarningMailbox,
		WarningPublicFolder,
		ProhibitSendMailbox,
		ProhibitPostPublicFolder,
		ProhibitSendReceiveMailBox,
		WarningMailboxMessagesPerFolderCount,
		ProhibitReceiveMailboxMessagesPerFolderCount,
		WarningFolderHierarchyChildrenCount,
		ProhibitReceiveFolderHierarchyChildrenCountCount,
		WarningMailboxMessagesPerFolderUnlimitedCount,
		WarningFolderHierarchyChildrenUnlimitedCount,
		WarningFolderHierarchyDepth,
		ProhibitReceiveFolderHierarchyDepth,
		WarningFolderHierarchyDepthUnlimited,
		WarningFoldersCount,
		ProhibitReceiveFoldersCount,
		WarningFoldersCountUnlimited
	}
}
