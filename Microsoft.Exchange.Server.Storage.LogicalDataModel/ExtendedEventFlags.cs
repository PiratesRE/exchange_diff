using System;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	[Flags]
	public enum ExtendedEventFlags : long
	{
		None = 0L,
		NoReminderPropertyModified = 1L,
		NoCIPropertyModified = 2L,
		RetentionTagModified = 4L,
		RetentionPropertiesModified = 8L,
		MoveDestination = 16L,
		AppointmentTimeNotModified = 64L,
		AppointmentFreeBusyNotModified = 128L,
		PublicFolderMailbox = 512L,
		FolderPermissionChanged = 2048L,
		NonIPMFolder = 1073741824L,
		IPMFolder = 2147483648L,
		NeedGroupExpansion = 4294967296L,
		InferenceProcessingNeeded = 8589934592L,
		ModernRemindersChanged = 17179869184L,
		FolderIsNotPartOfContentIndexing = 34359738368L,
		TimerEventFired = 68719476736L,
		InternalAccessFolder = 137438953472L
	}
}
