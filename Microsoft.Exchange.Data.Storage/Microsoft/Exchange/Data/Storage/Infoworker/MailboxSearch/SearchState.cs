using System;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	public enum SearchState
	{
		[LocDescription(ServerStrings.IDs.SearchStateInProgress)]
		InProgress,
		[LocDescription(ServerStrings.IDs.SearchStateFailed)]
		Failed,
		[LocDescription(ServerStrings.IDs.SearchStateStopping)]
		Stopping,
		[LocDescription(ServerStrings.IDs.SearchStateStopped)]
		Stopped,
		[LocDescription(ServerStrings.IDs.SearchStateSucceeded)]
		Succeeded,
		[LocDescription(ServerStrings.IDs.SearchStatePartiallySucceeded)]
		PartiallySucceeded,
		[LocDescription(ServerStrings.IDs.EstimateStateInProgress)]
		EstimateInProgress,
		[LocDescription(ServerStrings.IDs.EstimateStateFailed)]
		EstimateFailed,
		[LocDescription(ServerStrings.IDs.EstimateStateStopping)]
		EstimateStopping,
		[LocDescription(ServerStrings.IDs.EstimateStateStopped)]
		EstimateStopped,
		[LocDescription(ServerStrings.IDs.EstimateStateSucceeded)]
		EstimateSucceeded,
		[LocDescription(ServerStrings.IDs.EstimateStatePartiallySucceeded)]
		EstimatePartiallySucceeded,
		[LocDescription(ServerStrings.IDs.SearchStateNotStarted)]
		NotStarted,
		[LocDescription(ServerStrings.IDs.SearchStateQueued)]
		Queued,
		[LocDescription(ServerStrings.IDs.SearchStateDeletionInProgress)]
		DeletionInProgress
	}
}
