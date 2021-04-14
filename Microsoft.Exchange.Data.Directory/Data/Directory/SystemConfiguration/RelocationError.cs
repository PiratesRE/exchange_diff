using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	public enum RelocationError : byte
	{
		None,
		ADTransientError,
		GLSTransientError = 3,
		HighADReplicationLatency,
		RequestCanceled,
		UserExperienceCreateMailboxFailure,
		UserExperienceSetPasswordFailure,
		UserExperienceRemoveMailboxFailure,
		UserExperienceVerificationFailure,
		SyncFailureDueToSourceObjectBeingModified,
		ValidationTransientFailureEncountered,
		AddSidHistorySourcePdcTransferred,
		LastTransientError = 127,
		ADPermanentErrorSourceForest,
		ADPermanentErrorDestinationForest,
		GLSPermanentErrorTryGetTenantForestsByOrgGuid,
		GLSTenantNotFoundError,
		GLSPermanentError,
		ADUnkownSchemaAttribute,
		TooManyTransitions,
		RPCPermanentError,
		DestinationAuditingDisabled,
		MemberInAliasPermanentError,
		ObjectsWithCnfNameFoundInSource,
		UserExperiencePermanentFailure,
		ADDriverValidationFailed,
		StaleRelocation,
		DataValidationFailed,
		BlockingConstraintsDetected,
		DuplicateSamAccountNameInSource,
		MissingGlsDomainRecord,
		MissingMServDomainRecord,
		InvalidMXRecord,
		UserExperienceCreateMailboxPermanentFailure,
		UnclassifiedPermanentError = 255
	}
}
