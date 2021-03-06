using System;

namespace Microsoft.Exchange.AirSync
{
	internal enum StatusCode
	{
		None,
		Success,
		Sync_Success = 1,
		Sync_ProtocolVersionMismatch,
		Sync_InvalidSyncKey,
		Sync_ProtocolError,
		Sync_ServerError,
		Sync_ClientServerConversion,
		Sync_Conflict,
		Sync_ObjectNotFound,
		Sync_OutOfDisk,
		Sync_NotificationGUID,
		Sync_NotificationsNotProvisioned,
		Sync_FolderHierarchyRequired,
		Sync_InvalidParameters,
		Sync_InvalidWaitTime,
		Sync_TooManyFolders,
		Sync_Retry,
		GetItemEstimate_Success = 1,
		GetItemEstimate_InvalidCollection,
		GetItemEstimate_UnprimedSyncState,
		GetItemEstimate_InvalidSyncKey,
		FolderCmd_Success = 1,
		FolderCmd_FolderAlreadyExistsError,
		FolderCmd_SpecialFolderError,
		FolderCmd_FolderNotFoundError,
		FolderCmd_ParentNotFoundError,
		FolderCmd_ExchangeServerError,
		FolderCmd_AccessDeniedError,
		FolderCmd_TimeOutError,
		FolderCmd_WrongSyncKeyError,
		FolderCmd_MisformattedRequestError,
		FolderCmd_UnknownError,
		MoveItems_InvalidSourceCollectionId = 1,
		MoveItems_InvalidDestinationCollectionId,
		MoveItems_Success,
		MoveItems_SameSourceAndDestinationIds,
		MoveItems_FailureInMoveOperation,
		MoveItems_ExistingItemWithSameNameAtDestination,
		MoveItems_LockedSourceOrDestinationItem,
		MeetingResponse_Success = 1,
		MeetingResponse_InvalidMeetingRequest,
		MeetingResponse_ErrorOccurredOnMailbox,
		MeetingResponse_ErrorOccurredOnExchangeServer,
		Search_Success = 1,
		Search_ProtocolError,
		Search_ServerError,
		Search_BadLink,
		Search_AccessDenied,
		Search_NotFound,
		Search_ConnectionFailed,
		Search_TooComplex,
		Search_IndexNotLoaded,
		Search_TimeOut,
		Search_NeedToFolderSync,
		Search_EndOfRetrieveableRangeWarning,
		Search_AccessBlocked,
		Search_CredentialsRequired,
		Settings_Success = 1,
		Settings_ProtocolError,
		Settings_AccessDenied,
		Settings_ServerUnavailable,
		Settings_InvalidArguments,
		Settings_ConflictingArguments,
		Settings_DeniedByPolicy,
		Ping_NoChanges = 1,
		Ping_Changes,
		Ping_SendParameters,
		Ping_Protocol,
		Ping_HbiOutOfRange,
		Ping_FoldersOutOfRange,
		Ping_FolderSyncRequired,
		Ping_ServerError,
		ItemOperations_Success = 1,
		ItemOperations_ProtocolError,
		ItemOperations_ServerError,
		ItemOperations_BadLink,
		ItemOperations_AccessDenied,
		ItemOperations_NotFound,
		ItemOperations_ConnectionFailed,
		ItemOperations_OutOfRange,
		ItemOperations_UnknownStore,
		ItemOperations_FileIsEmpty,
		ItemOperations_DataTooLarge,
		ItemOperations_IOFailure,
		ItemOperations_InvalidBodyPreference,
		ItemOperations_ConversionFailed,
		ItemOperations_InvalidAttachmentId,
		ItemOperations_AccessBlocked,
		ItemOperations_PartialSuccess,
		ItemOperations_CredentialsRequired,
		Provision_Success = 1,
		Provision_ProtocolError,
		Provision_ServerError,
		ResolveRecipients_Success = 1,
		ResolveRecipients_RecipientIsAmbiguous,
		ResolveRecipients_RecipientIsAmbiguousPartialList,
		ResolveRecipients_RecipientNotFound,
		ResolveRecipients_ProtocolError,
		ResolveRecipients_ServerError,
		ResolveRecipients_NoCertificate,
		ResolveRecipients_GlobalLimitHit,
		ResolveRecipients_CertificateEnumerationFailure,
		ValidateCert_Success = 1,
		ValidateCert_ProtocolError,
		ValidateCert_SignatureNotValidated,
		ValidateCert_FromUntrustedSource,
		ValidateCert_InvalidCertChain,
		ValidateCert_InvalidForSigning,
		ValidateCert_ExpiredOrInvalid,
		ValidateCert_InvalidTimePeriods,
		ValidateCert_PurposeError,
		ValidateCert_MissingInfo,
		ValidateCert_WrongRole,
		ValidateCert_NotMatch,
		ValidateCert_Revoked,
		ValidateCert_NoServerContact,
		ValidateCert_ChainRevoked,
		ValidateCert_NoRevocationStatus,
		ValidateCert_UnknowServerError,
		First140Error = 101,
		InvalidContent = 101,
		InvalidWBXML,
		InvalidXML,
		InvalidDateTime,
		InvalidCombinationOfIDs,
		InvalidIDs,
		InvalidMIME,
		DeviceIdMissingOrInvalid,
		DeviceTypeMissingOrInvalid,
		ServerError,
		ServerErrorRetryLater,
		ActiveDirectoryAccessDenied,
		MailboxQuotaExceeded,
		MailboxServerOffline,
		SendQuotaExceeded,
		MessageRecipientUnresolved,
		MessageReplyNotAllowed,
		MessagePreviouslySent,
		MessageHasNoRecipient,
		MailSubmissionFailed,
		MessageReplyFailed,
		AttachmentIsTooLarge,
		UserHasNoMailbox,
		UserCannotBeAnonymous,
		UserPrincipalCouldNotBeFound,
		UserDisabledForSync,
		UserOnNewMailboxCannotSync,
		UserOnLegacyMailboxCannotSync,
		DeviceIsBlockedForThisUser,
		AccessDenied,
		AccountDisabled,
		SyncStateNotFound,
		SyncStateLocked,
		SyncStateCorrupt,
		SyncStateAlreadyExists,
		SyncStateVersionInvalid,
		CommandNotSupported,
		VersionNotSupported,
		DeviceNotFullyProvisionable,
		RemoteWipeRequested,
		LegacyDeviceOnStrictPolicy,
		DeviceNotProvisioned,
		PolicyRefresh,
		InvalidPolicyKey,
		ExternallyManagedDevicesNotAllowed,
		NoRecurrenceInCalendar,
		UnexpectedItemClass,
		RemoteServerHasNoSSL,
		InvalidStoredRequest,
		ItemNotFound,
		TooManyFolders,
		NoFoldersFound,
		ItemsLostAfterMove,
		FailureInMoveOperation,
		MoveCommandDisallowedForNonPersistentMoveAction,
		MoveCommandInvalidDestinationFolder,
		AvailabilityTooManyRecipients = 160,
		AvailabilityDLLimitReached,
		AvailabilityTransientFailure,
		AvailabilityFailure,
		LastKnown140Error = 163,
		BodyPartPreferenceTypeNotSupported,
		DeviceInformationRequired,
		InvalidAccountId,
		AccountSendDisabled,
		IRM_FeatureDisabled,
		IRM_TransientError,
		IRM_PermanentError,
		IRM_InvalidTemplateID,
		IRM_OperationNotPermitted,
		NoPicture,
		PictureTooLarge,
		PictureLimitReached,
		BodyPart_ConversationTooLarge,
		MaximumDevicesReached,
		LastKnown141Error = 177,
		InvalidMimeBodyCombination,
		InvalidSmartForwardParameters,
		InvalidStartTime,
		InvalidEndTime,
		InvalidTimezoneRange,
		InvalidDateTimeFormat,
		InvalidTimezone,
		InvalidRecipients,
		LastKnown160Error = 185
	}
}
