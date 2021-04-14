using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public enum MessageStatus
	{
		[LocDescription(DataStrings.IDs.MessageStatusNone)]
		None,
		[LocDescription(DataStrings.IDs.MessageStatusActive)]
		Active,
		[LocDescription(DataStrings.IDs.MessageStatusPendingRemove)]
		PendingRemove,
		[LocDescription(DataStrings.IDs.MessageStatusPendingSuspend)]
		PendingSuspend,
		[LocDescription(DataStrings.IDs.MessageStatusReady)]
		Ready,
		[LocDescription(DataStrings.IDs.MessageStatusRetry)]
		Retry,
		[LocDescription(DataStrings.IDs.MessageStatusSuspended)]
		Suspended,
		[LocDescription(DataStrings.IDs.MessageStatusLocked)]
		Locked
	}
}
