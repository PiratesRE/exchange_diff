using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum MeetingInquiryAction
	{
		SendCancellation,
		ReviveMeeting,
		SendUpdateForMaster,
		MeetingAlreadyExists,
		ExistingOccurrence,
		HasDelegates,
		DeletedVersionNotFound,
		PairedCancellationFound,
		FailedToRevive
	}
}
