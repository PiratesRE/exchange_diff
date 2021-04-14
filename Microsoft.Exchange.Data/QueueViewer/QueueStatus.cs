using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public enum QueueStatus
	{
		[LocDescription(DataStrings.IDs.QueueStatusNone)]
		None,
		[LocDescription(DataStrings.IDs.QueueStatusActive)]
		Active,
		[LocDescription(DataStrings.IDs.QueueStatusReady)]
		Ready,
		[LocDescription(DataStrings.IDs.QueueStatusRetry)]
		Retry,
		[LocDescription(DataStrings.IDs.QueueStatusSuspended)]
		Suspended,
		[LocDescription(DataStrings.IDs.QueueStatusConnecting)]
		Connecting
	}
}
