using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum MapiPropertiesIndex
	{
		RequestStatus,
		JobProcessingState,
		MRSServerName,
		AllowedToFinishMove,
		CancelRequest,
		ExchangeGuid,
		ArchiveGuid,
		LastUpdateTimestamp,
		CreationTimestamp,
		JobType,
		Flags,
		SourceDatabase,
		TargetDatabase,
		DoNotPickUntilTimestamp,
		RequestType,
		SourceArchiveDatabase,
		TargetArchiveDatabase,
		Priority,
		SourceExchangeGuid,
		TargetExchangeGuid,
		RehomeRequest,
		InternalFlags,
		PartitionHint,
		PoisonCount,
		FailureType,
		WorkloadType,
		TotalNamedProps,
		TotalSettableProperties = 26,
		RequestGuid = 26,
		MessageID,
		TotalProperties
	}
}
