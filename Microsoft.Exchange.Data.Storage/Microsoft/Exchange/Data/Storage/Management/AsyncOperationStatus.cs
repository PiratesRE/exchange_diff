using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum AsyncOperationStatus
	{
		[SeverityLevel(SeverityLevel.Information)]
		[LocDescription(ServerStrings.IDs.RequestStateQueued)]
		Queued,
		[LocDescription(ServerStrings.IDs.RequestStateInProgress)]
		[SeverityLevel(SeverityLevel.Information)]
		InProgress,
		[LocDescription(ServerStrings.IDs.RequestStateSuspended)]
		[SeverityLevel(SeverityLevel.Information)]
		Suspended,
		[LocDescription(ServerStrings.IDs.RequestStateCompleted)]
		[SeverityLevel(SeverityLevel.Information)]
		Completed,
		[LocDescription(ServerStrings.IDs.RequestStateFailed)]
		[SeverityLevel(SeverityLevel.Error)]
		Failed,
		[LocDescription(ServerStrings.IDs.RequestStateCertExpiring)]
		[SeverityLevel(SeverityLevel.Warning)]
		CertExpiring,
		[SeverityLevel(SeverityLevel.Error)]
		[LocDescription(ServerStrings.IDs.RequestStateCertExpired)]
		CertExpired,
		[LocDescription(ServerStrings.IDs.RequestStateWaitingForFinalization)]
		[SeverityLevel(SeverityLevel.Information)]
		WaitingForFinalization,
		[LocDescription(ServerStrings.IDs.RequestStateCreated)]
		[SeverityLevel(SeverityLevel.Information)]
		Created,
		[LocDescription(ServerStrings.IDs.RequestStateCompleting)]
		[SeverityLevel(SeverityLevel.Information)]
		Completing,
		[LocDescription(ServerStrings.IDs.RequestStateRemoving)]
		[SeverityLevel(SeverityLevel.Information)]
		Removing
	}
}
