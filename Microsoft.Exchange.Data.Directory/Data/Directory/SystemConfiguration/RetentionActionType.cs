using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RetentionActionType
	{
		[LocDescription(DirectoryStrings.IDs.MoveToDeletedItems)]
		MoveToDeletedItems = 1,
		[LocDescription(DirectoryStrings.IDs.MoveToFolder)]
		MoveToFolder,
		[LocDescription(DirectoryStrings.IDs.SoftDelete)]
		DeleteAndAllowRecovery,
		[LocDescription(DirectoryStrings.IDs.PermanentlyDelete)]
		PermanentlyDelete,
		[LocDescription(DirectoryStrings.IDs.Tag)]
		MarkAsPastRetentionLimit,
		[LocDescription(DirectoryStrings.IDs.MoveToArchive)]
		MoveToArchive
	}
}
