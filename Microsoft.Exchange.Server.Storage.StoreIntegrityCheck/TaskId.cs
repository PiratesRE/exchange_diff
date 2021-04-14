using System;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public enum TaskId : uint
	{
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		None,
		[MapToManagement("SearchFolder", false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		SearchBacklinks,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		FolderView,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		AggregateCounts,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		ProvisionedFolder,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		ReplState,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		MessagePtagCn,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement("MessageId", false)]
		MidsetDeleted,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		RuleMessageClass = 100U,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		RestrictionFolder,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		FolderACL,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		UniqueMidIndex,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		CorruptJunkRule,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		MissingSpecialFolders,
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		[MapToManagement(null, false)]
		DropAllLazyIndexes,
		[RepairTaskAccessLevel(RepairTaskAccess.Support)]
		[MapToManagement(null, false)]
		ImapId,
		[RepairTaskAccessLevel(RepairTaskAccess.Test)]
		[MapToManagement(null, true)]
		InMemoryFolderHierarchy,
		[RepairTaskAccessLevel(RepairTaskAccess.Test)]
		[MapToManagement(null, true)]
		DiscardFolderHierarchyCache,
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		[MapToManagement(null, false)]
		LockedMoveTarget = 4096U,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		ScheduledCheck = 8192U,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		Extension1 = 32768U,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		Extension2,
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		[MapToManagement(null, false)]
		Extension3,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		Extension4,
		[MapToManagement(null, false)]
		[RepairTaskAccessLevel(RepairTaskAccess.Engineering)]
		Extension5
	}
}
