using System;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public enum RecipientStatus
	{
		[LocDescription(DataStrings.IDs.RecipientStatusNone)]
		None,
		[LocDescription(DataStrings.IDs.RecipientStatusComplete)]
		Complete,
		[LocDescription(DataStrings.IDs.RecipientStatusReady)]
		Ready,
		[LocDescription(DataStrings.IDs.RecipientStatusRetry)]
		Retry,
		[LocDescription(DataStrings.IDs.RecipientStatusLocked)]
		Locked
	}
}
