using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public enum RequestJobTimestamp
	{
		None,
		Creation,
		Start,
		InitialSeedingCompleted,
		FinalSync,
		Completion,
		Suspended,
		LastUpdate,
		DoNotPickUntil,
		Failure,
		MailboxLocked,
		FailedDataGuarantee,
		StartAfter,
		CompleteAfter,
		LastProgressCheckpoint,
		DomainControllerUpdate,
		RequestCanceled,
		LastSuccessfulSourceConnection,
		LastSuccessfulTargetConnection,
		SourceConnectionFailure,
		TargetConnectionFailure,
		IsIntegStarted,
		LastServerBusyBackoff,
		ServerBusyBackoffUntil,
		MaxTimestamp
	}
}
