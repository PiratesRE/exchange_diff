using System;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal enum ErrorCode
	{
		[ErrorCodeInformation(IsTransientError = true, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorLogSearchServiceDown)]
		LogSearchConnection,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = false, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorCrossForestMisconfiguration)]
		CrossForestMisconfiguration,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = false, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorCrossPremiseMisconfiguration, MultiMessageSearchMessage = Strings.IDs.TrackingErrorCrossPremiseMisconfigurationMultiMessageSearch)]
		CrossPremiseMisconfiguration,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = false, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorCrossForestAuthentication)]
		CrossForestAuthentication,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = false, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorCrossPremiseAuthentication)]
		CrossPremiseAuthentication,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = true, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingErrorBudgetExceeded, MultiMessageSearchMessage = Strings.IDs.TrackingErrorBudgetExceededMultiMessageSearch)]
		BudgetExceeded,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = true, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingErrorTimeBudgetExceeded)]
		TimeBudgetExceeded,
		[ErrorCodeInformation(IsTransientError = true, ShowToIWUser = true, ShowDetailToIW = true, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingTotalBudgetExceeded)]
		TotalBudgetExceeded,
		[ErrorCodeInformation(IsTransientError = true, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorConnectivity)]
		Connectivity,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = true, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingErrorLegacySender, MultiMessageSearchMessage = Strings.IDs.TrackingErrorLegacySenderMultiMessageSearch)]
		LegacySender,
		[ErrorCodeInformation(IsTransientError = true, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingTransientError, MultiMessageSearchMessage = Strings.IDs.TrackingTransientErrorMultiMessageSearch)]
		GeneralTransientFailure,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingPermanentError, MultiMessageSearchMessage = Strings.IDs.TrackingPermanentErrorMultiMessageSearch)]
		GeneralFatalFailure,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = false, ShowDetailToIW = false, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingErrorReadStatus)]
		ReadStatusError,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = true, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingLogVersionIncompatible)]
		LogVersionIncompatible,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = RequiredProperty.None, Message = Strings.IDs.TrackingErrorModerationDecisionLogsFromE14Rtm)]
		ModerationDecisionLogsFromE14Rtm,
		[ErrorCodeInformation(IsTransientError = true, ShowToIWUser = false, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorQueueViewerRpc)]
		QueueViewerConnectionFailure,
		[ErrorCodeInformation(IsTransientError = true, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Target), Message = Strings.IDs.TrackingErrorCASUriDiscovery)]
		CASUriDiscoveryFailure,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Data), Message = Strings.IDs.TrackingErrorInvalidADData)]
		InvalidADData,
		[ErrorCodeInformation(IsTransientError = false, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Data), Message = Strings.IDs.TrackingErrorPermanentUnexpected)]
		UnexpectedErrorPermanent,
		[ErrorCodeInformation(IsTransientError = true, ShowToIWUser = true, ShowDetailToIW = false, RequiredProperties = (RequiredProperty.Server | RequiredProperty.Domain | RequiredProperty.Data), Message = Strings.IDs.TrackingErrorTransientUnexpected)]
		UnexpectedErrorTransient
	}
}
