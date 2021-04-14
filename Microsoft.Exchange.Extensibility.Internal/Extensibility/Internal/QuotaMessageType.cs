using System;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal enum QuotaMessageType
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
