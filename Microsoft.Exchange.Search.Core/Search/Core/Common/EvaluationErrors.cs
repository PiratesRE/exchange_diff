using System;

namespace Microsoft.Exchange.Search.Core.Common
{
	internal enum EvaluationErrors
	{
		None,
		[LocDescription(Strings.IDs.EvaluationErrorsGenericError)]
		GenericError,
		[LocDescription(Strings.IDs.EvaluationErrorsTimeout)]
		Timeout,
		[LocDescription(Strings.IDs.EvaluationErrorsStaleEvent)]
		StaleEvent,
		[LocDescription(Strings.IDs.EvaluationErrorsMailboxOffline)]
		MailboxOffline,
		[LocDescription(Strings.IDs.EvaluationErrorsAttachmentLimitReached)]
		AttachmentLimitReached,
		[LocDescription(Strings.IDs.EvaluationErrorsMarsWriterTruncation)]
		MarsWriterTruncation,
		[LocDescription(Strings.IDs.EvaluationErrorsDocumentParserFailure)]
		DocumentParserFailure,
		[LocDescription(Strings.IDs.EvaluationErrorsAnnotationTokenError)]
		AnnotationTokenError,
		[LocDescription(Strings.IDs.EvaluationErrorsPoisonDocument)]
		PoisonDocument,
		[LocDescription(Strings.IDs.EvaluationErrorsRightsManagementFailure)]
		RightsManagementFailure,
		[LocDescription(Strings.IDs.EvaluationErrorsSessionUnavailable)]
		SessionUnavailable,
		[LocDescription(Strings.IDs.EvaluationErrorsMailboxQuarantined)]
		MailboxQuarantined,
		[LocDescription(Strings.IDs.EvaluationErrorsMailboxLocked)]
		MailboxLocked,
		[LocDescription(Strings.IDs.EvaluationErrorsNoSupport)]
		MapiNoSupport,
		[LocDescription(Strings.IDs.EvaluationErrorsLoginFailed)]
		LoginFailed,
		[LocDescription(Strings.IDs.EvaluationErrorsTextConversionFailure)]
		TextConversionFailure,
		MaxFailureId = 200,
		MonitoringFailure,
		IntentionalTestFailure,
		RecrawlWatermark,
		IntentionalTestTransientFailure,
		IntentionalTestItemTruncationFailure,
		NonExistentErrorCode = 999999999
	}
}
