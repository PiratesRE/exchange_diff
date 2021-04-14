using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MapiExtendedEventFlags : ulong
	{
		None = 0UL,
		NoReminderPropertyModified = 1UL,
		NoContentIndexingPropertyModified = 2UL,
		RetentionTagModified = 4UL,
		RetentionPropertiesModified = 8UL,
		MoveDestination = 16UL,
		ExcludeFromHierarchy = 32UL,
		AppointmentTimeNotModified = 64UL,
		AppointmentFreeBusyNotModified = 128UL,
		IrmRestrictedItem = 256UL,
		PublicFolderMailbox = 512UL,
		FolderPermissionChanged = 2048UL,
		NonIPMFolder = 1073741824UL,
		IPMFolder = 2147483648UL,
		NeedGroupExpansion = 4294967296UL,
		InferenceProcessingNeeded = 8589934592UL,
		ModernRemindersChanged = 17179869184UL,
		FolderIsNotPartOfContentIndexing = 34359738368UL,
		TimerEventFired = 68719476736UL,
		InternalAccessFolder = 137438953472UL
	}
}
