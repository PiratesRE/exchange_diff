using System;

namespace Microsoft.Exchange.Management.Tasks
{
	public enum MailboxCorruptionType
	{
		None,
		SearchFolder,
		FolderView,
		AggregateCounts,
		ProvisionedFolder,
		ReplState,
		MessagePtagCn,
		MessageId,
		RuleMessageClass = 100,
		RestrictionFolder,
		FolderACL,
		UniqueMidIndex,
		CorruptJunkRule,
		MissingSpecialFolders,
		DropAllLazyIndexes,
		ImapId,
		LockedMoveTarget = 4096,
		ScheduledCheck = 8192,
		Extension1 = 32768,
		Extension2,
		Extension3,
		Extension4,
		Extension5
	}
}
