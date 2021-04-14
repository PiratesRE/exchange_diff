using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum CalendarProcessingSteps
	{
		None = 0,
		PropsCheck = 1,
		PrincipalWantsCopyChecked = 2,
		NeedsCustomForm = 4,
		PrincipalHadTombstone = 8,
		CreatedOnPrincipal = 16,
		LookedForOutOfDate = 32,
		ChangedMtgType = 64,
		UpdatedCalItem = 128,
		CopiedOldProps = 256,
		CounterProposalSet = 512,
		SendAutoResponse = 1024,
		RevivedException = 2048,
		ProcessedMeetingForwardNotification = 4096
	}
}
