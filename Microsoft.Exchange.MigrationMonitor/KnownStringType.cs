using System;

namespace Microsoft.Exchange.Servicelets.MigrationMonitor
{
	internal enum KnownStringType
	{
		None = -1,
		ServerName,
		DatabaseName,
		MailboxType,
		TenantName,
		MigrationType,
		MigrationStatus,
		BadItemKind,
		BadItemWkfTypeId,
		BadItemMessageClass,
		BadItemCategory,
		FailureType,
		FailureSide,
		RequestType,
		RequestStatus,
		RequestStatusDetail,
		RequestPriority,
		RequestBatchName,
		Version,
		RemoteHostName,
		TargetDeliveryDomain,
		RequestSyncStage,
		RequestJobType,
		RequestWorkloadType,
		MaxProviderDurationMethodName,
		OwnerResourceNameType,
		OwnerResourceTypeType,
		ResourceKeyType,
		LoadStateType,
		ReservationFailureReasonType,
		ReservationFailureResourceTypeType,
		ReservationFailureWLMResourceTypeType,
		PickupResultsType,
		LastScanFailureFailureType,
		DrumTestingTestType,
		DrumTestingObjectType,
		DrumTestingResultType,
		DrumTestingResultCategoryType,
		SyncProtocol,
		MigrationDirection,
		Locale,
		MigrationSkipSteps,
		MigrationBatchFlags,
		EndpointGuid,
		EndpointState,
		EndpointPermission,
		WatsonHash,
		DisconnectReason = 56,
		AppVersion,
		LocalServerName = 50
	}
}
