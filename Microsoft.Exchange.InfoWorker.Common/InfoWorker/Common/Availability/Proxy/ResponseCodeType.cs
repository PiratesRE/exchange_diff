﻿using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[Serializable]
	public enum ResponseCodeType
	{
		NoError,
		ErrorAccessDenied,
		ErrorAccountDisabled,
		ErrorAddDelegatesFailed,
		ErrorAddressSpaceNotFound,
		ErrorADOperation,
		ErrorADSessionFilter,
		ErrorADUnavailable,
		ErrorAutoDiscoverFailed,
		ErrorAffectedTaskOccurrencesRequired,
		ErrorAttachmentSizeLimitExceeded,
		ErrorAvailabilityConfigNotFound,
		ErrorBatchProcessingStopped,
		ErrorCalendarCannotMoveOrCopyOccurrence,
		ErrorCalendarCannotUpdateDeletedItem,
		ErrorCalendarCannotUseIdForOccurrenceId,
		ErrorCalendarCannotUseIdForRecurringMasterId,
		ErrorCalendarDurationIsTooLong,
		ErrorCalendarEndDateIsEarlierThanStartDate,
		ErrorCalendarFolderIsInvalidForCalendarView,
		ErrorCalendarInvalidAttributeValue,
		ErrorCalendarInvalidDayForTimeChangePattern,
		ErrorCalendarInvalidDayForWeeklyRecurrence,
		ErrorCalendarInvalidPropertyState,
		ErrorCalendarInvalidPropertyValue,
		ErrorCalendarInvalidRecurrence,
		ErrorCalendarInvalidTimeZone,
		ErrorCalendarIsCancelledForAccept,
		ErrorCalendarIsCancelledForDecline,
		ErrorCalendarIsCancelledForRemove,
		ErrorCalendarIsCancelledForTentative,
		ErrorCalendarIsDelegatedForAccept,
		ErrorCalendarIsDelegatedForDecline,
		ErrorCalendarIsDelegatedForRemove,
		ErrorCalendarIsDelegatedForTentative,
		ErrorCalendarIsNotOrganizer,
		ErrorCalendarIsOrganizerForAccept,
		ErrorCalendarIsOrganizerForDecline,
		ErrorCalendarIsOrganizerForRemove,
		ErrorCalendarIsOrganizerForTentative,
		ErrorCalendarOccurrenceIndexIsOutOfRecurrenceRange,
		ErrorCalendarOccurrenceIsDeletedFromRecurrence,
		ErrorCalendarOutOfRange,
		ErrorCalendarMeetingRequestIsOutOfDate,
		ErrorCalendarViewRangeTooBig,
		ErrorCannotCreateCalendarItemInNonCalendarFolder,
		ErrorCannotCreateContactInNonContactFolder,
		ErrorCannotCreatePostItemInNonMailFolder,
		ErrorCannotCreateTaskInNonTaskFolder,
		ErrorCannotDeleteObject,
		ErrorCannotOpenFileAttachment,
		ErrorCannotDeleteTaskOccurrence,
		ErrorCannotSetCalendarPermissionOnNonCalendarFolder,
		ErrorCannotSetNonCalendarPermissionOnCalendarFolder,
		ErrorCannotSetPermissionUnknownEntries,
		ErrorCannotUseFolderIdForItemId,
		ErrorCannotUseItemIdForFolderId,
		ErrorChangeKeyRequired,
		ErrorChangeKeyRequiredForWriteOperations,
		ErrorConnectionFailed,
		ErrorContainsFilterWrongType,
		ErrorContentConversionFailed,
		ErrorCorruptData,
		ErrorCreateItemAccessDenied,
		ErrorCreateManagedFolderPartialCompletion,
		ErrorCreateSubfolderAccessDenied,
		ErrorCrossMailboxMoveCopy,
		ErrorDataSizeLimitExceeded,
		ErrorDataSourceOperation,
		ErrorDelegateAlreadyExists,
		ErrorDelegateCannotAddOwner,
		ErrorDelegateMissingConfiguration,
		ErrorDelegateNoUser,
		ErrorDelegateValidationFailed,
		ErrorDeleteDistinguishedFolder,
		ErrorDeleteItemsFailed,
		ErrorDistinguishedUserNotSupported,
		ErrorDistributionListMemberNotExist,
		ErrorDuplicateInputFolderNames,
		ErrorDuplicateUserIdsSpecified,
		ErrorEmailAddressMismatch,
		ErrorEventNotFound,
		ErrorExpiredSubscription,
		ErrorExternalFacingCAS,
		ErrorFolderCorrupt,
		ErrorFolderNotFound,
		ErrorFolderPropertRequestFailed,
		ErrorFolderSave,
		ErrorFolderSaveFailed,
		ErrorFolderSavePropertyError,
		ErrorFolderExists,
		ErrorFreeBusyGenerationFailed,
		ErrorGetServerSecurityDescriptorFailed,
		ErrorImpersonateUserDenied,
		ErrorImpersonationDenied,
		ErrorImpersonationFailed,
		ErrorIncorrectSchemaVersion,
		ErrorIncorrectUpdatePropertyCount,
		ErrorIndividualMailboxLimitReached,
		ErrorInsufficientResources,
		ErrorInternalServerError,
		ErrorInternalServerTransientError,
		ErrorInvalidAccessLevel,
		ErrorInvalidArgument,
		ErrorInvalidAttachmentId,
		ErrorInvalidAttachmentSubfilter,
		ErrorInvalidAttachmentSubfilterTextFilter,
		ErrorInvalidAuthorizationContext,
		ErrorInvalidChangeKey,
		ErrorInvalidClientSecurityContext,
		ErrorInvalidCompleteDate,
		ErrorInvalidCrossForestCredentials,
		ErrorInvalidDelegatePermission,
		ErrorInvalidDelegateUserId,
		ErrorInvalidExcludesRestriction,
		ErrorInvalidExpressionTypeForSubFilter,
		ErrorInvalidExtendedProperty,
		ErrorInvalidExtendedPropertyValue,
		ErrorInvalidFolderId,
		ErrorInvalidFolderTypeForOperation,
		ErrorInvalidFractionalPagingParameters,
		ErrorInvalidFreeBusyViewType,
		ErrorInvalidId,
		ErrorInvalidIdEmpty,
		ErrorInvalidIdMalformed,
		ErrorInvalidIdMalformedEwsLegacyIdFormat,
		ErrorInvalidIdMonikerTooLong,
		ErrorInvalidIdNotAnItemAttachmentId,
		ErrorInvalidIdReturnedByResolveNames,
		ErrorInvalidIdStoreObjectIdTooLong,
		ErrorInvalidIdTooManyAttachmentLevels,
		ErrorInvalidIdXml,
		ErrorInvalidIndexedPagingParameters,
		ErrorInvalidInternetHeaderChildNodes,
		ErrorInvalidItemForOperationCreateItemAttachment,
		ErrorInvalidItemForOperationCreateItem,
		ErrorInvalidItemForOperationAcceptItem,
		ErrorInvalidItemForOperationDeclineItem,
		ErrorInvalidItemForOperationCancelItem,
		ErrorInvalidItemForOperationExpandDL,
		ErrorInvalidItemForOperationRemoveItem,
		ErrorInvalidItemForOperationSendItem,
		ErrorInvalidItemForOperationTentative,
		ErrorInvalidMailbox,
		ErrorInvalidManagedFolderProperty,
		ErrorInvalidManagedFolderQuota,
		ErrorInvalidManagedFolderSize,
		ErrorInvalidMergedFreeBusyInterval,
		ErrorInvalidNameForNameResolution,
		ErrorInvalidOperation,
		ErrorInvalidNetworkServiceContext,
		ErrorInvalidOofParameter,
		ErrorInvalidPagingMaxRows,
		ErrorInvalidParentFolder,
		ErrorInvalidPercentCompleteValue,
		ErrorInvalidPermissionSettings,
		ErrorInvalidPhoneCallId,
		ErrorInvalidPhoneNumber,
		ErrorInvalidUserInfo,
		ErrorInvalidPropertyAppend,
		ErrorInvalidPropertyDelete,
		ErrorInvalidPropertyForExists,
		ErrorInvalidPropertyForOperation,
		ErrorInvalidPropertyRequest,
		ErrorInvalidPropertySet,
		ErrorInvalidPropertyUpdateSentMessage,
		ErrorInvalidProxySecurityContext,
		ErrorInvalidPullSubscriptionId,
		ErrorInvalidPushSubscriptionUrl,
		ErrorInvalidRecipients,
		ErrorInvalidRecipientSubfilter,
		ErrorInvalidRecipientSubfilterComparison,
		ErrorInvalidRecipientSubfilterOrder,
		ErrorInvalidRecipientSubfilterTextFilter,
		ErrorInvalidReferenceItem,
		ErrorInvalidRequest,
		ErrorInvalidRestriction,
		ErrorInvalidRoutingType,
		ErrorInvalidScheduledOofDuration,
		ErrorInvalidSchemaVersionForMailboxVersion,
		ErrorInvalidSecurityDescriptor,
		ErrorInvalidSendItemSaveSettings,
		ErrorInvalidSerializedAccessToken,
		ErrorInvalidServerVersion,
		ErrorInvalidSid,
		ErrorInvalidSIPUri,
		ErrorInvalidSmtpAddress,
		ErrorInvalidSubfilterType,
		ErrorInvalidSubfilterTypeNotAttendeeType,
		ErrorInvalidSubfilterTypeNotRecipientType,
		ErrorInvalidSubscription,
		ErrorInvalidSubscriptionRequest,
		ErrorInvalidSyncStateData,
		ErrorInvalidTimeInterval,
		ErrorInvalidUserOofSettings,
		ErrorInvalidUserPrincipalName,
		ErrorInvalidUserSid,
		ErrorInvalidUserSidMissingUPN,
		ErrorInvalidValueForProperty,
		ErrorInvalidWatermark,
		ErrorIPGatewayNotFound,
		ErrorIrresolvableConflict,
		ErrorItemCorrupt,
		ErrorItemNotFound,
		ErrorItemPropertyRequestFailed,
		ErrorItemSave,
		ErrorItemSavePropertyError,
		ErrorLegacyMailboxFreeBusyViewTypeNotMerged,
		ErrorLocalServerObjectNotFound,
		ErrorLogonAsNetworkServiceFailed,
		ErrorMailboxConfiguration,
		ErrorMailboxDataArrayEmpty,
		ErrorMailboxDataArrayTooBig,
		ErrorMailboxLogonFailed,
		ErrorMailboxMoveInProgress,
		ErrorMailboxStoreUnavailable,
		ErrorMailRecipientNotFound,
		ErrorMailTipsDisabled,
		ErrorManagedFolderAlreadyExists,
		ErrorManagedFolderNotFound,
		ErrorManagedFoldersRootFailure,
		ErrorMeetingSuggestionGenerationFailed,
		ErrorMessageDispositionRequired,
		ErrorMessageSizeExceeded,
		ErrorMimeContentConversionFailed,
		ErrorMimeContentInvalid,
		ErrorMimeContentInvalidBase64String,
		ErrorMissingArgument,
		ErrorMissingEmailAddress,
		ErrorMissingEmailAddressForManagedFolder,
		ErrorMissingInformationEmailAddress,
		ErrorMissingInformationReferenceItemId,
		ErrorMissingItemForCreateItemAttachment,
		ErrorMissingManagedFolderId,
		ErrorMissingRecipients,
		ErrorMissingUserIdInformation,
		ErrorMoreThanOneAccessModeSpecified,
		ErrorMoveCopyFailed,
		ErrorMoveDistinguishedFolder,
		ErrorNameResolutionMultipleResults,
		ErrorNameResolutionNoMailbox,
		ErrorNameResolutionNoResults,
		ErrorNoCalendar,
		ErrorNoDestinationCASDueToKerberosRequirements,
		ErrorNoDestinationCASDueToSSLRequirements,
		ErrorNoDestinationCASDueToVersionMismatch,
		ErrorNoFolderClassOverride,
		ErrorNoFreeBusyAccess,
		ErrorNonExistentMailbox,
		ErrorNonPrimarySmtpAddress,
		ErrorNoPropertyTagForCustomProperties,
		ErrorNoPublicFolderReplicaAvailable,
		ErrorNoRespondingCASInDestinationSite,
		ErrorNotDelegate,
		ErrorNotEnoughMemory,
		ErrorObjectTypeChanged,
		ErrorOccurrenceCrossingBoundary,
		ErrorOccurrenceTimeSpanTooBig,
		ErrorOperationNotAllowedWithPublicFolderRoot,
		ErrorParentFolderIdRequired,
		ErrorParentFolderNotFound,
		ErrorPasswordChangeRequired,
		ErrorPasswordExpired,
		ErrorPhoneNumberNotDialable,
		ErrorPropertyUpdate,
		ErrorPropertyValidationFailure,
		ErrorProxiedSubscriptionCallFailure,
		ErrorProxyCallFailed,
		ErrorProxyGroupSidLimitExceeded,
		ErrorProxyRequestNotAllowed,
		ErrorProxyRequestProcessingFailed,
		ErrorProxyTokenExpired,
		ErrorPublicFolderRequestProcessingFailed,
		ErrorPublicFolderServerNotFound,
		ErrorQueryFilterTooLong,
		ErrorQuotaExceeded,
		ErrorReadEventsFailed,
		ErrorReadReceiptNotPending,
		ErrorRecurrenceEndDateTooBig,
		ErrorRecurrenceHasNoOccurrence,
		ErrorRemoveDelegatesFailed,
		ErrorRequestAborted,
		ErrorRequestStreamTooBig,
		ErrorRequiredPropertyMissing,
		ErrorResolveNamesInvalidFolderType,
		ErrorResolveNamesOnlyOneContactsFolderAllowed,
		ErrorResponseSchemaValidation,
		ErrorRestrictionTooLong,
		ErrorRestrictionTooComplex,
		ErrorResultSetTooBig,
		ErrorInvalidExchangeImpersonationHeaderData,
		ErrorSavedItemFolderNotFound,
		ErrorSchemaValidation,
		ErrorSearchFolderNotInitialized,
		ErrorSendAsDenied,
		ErrorSendMeetingCancellationsRequired,
		ErrorSendMeetingInvitationsOrCancellationsRequired,
		ErrorSendMeetingInvitationsRequired,
		ErrorSentMeetingRequestUpdate,
		ErrorSentTaskRequestUpdate,
		ErrorServerBusy,
		ErrorServiceDiscoveryFailed,
		ErrorStaleObject,
		ErrorSubmissionQuotaExceeded,
		ErrorSubscriptionAccessDenied,
		ErrorSubscriptionDelegateAccessNotSupported,
		ErrorSubscriptionNotFound,
		ErrorSyncFolderNotFound,
		ErrorTimeIntervalTooBig,
		ErrorTimeoutExpired,
		ErrorTimeZone,
		ErrorToFolderNotFound,
		ErrorTokenSerializationDenied,
		ErrorUpdatePropertyMismatch,
		ErrorUnifiedMessagingDialPlanNotFound,
		ErrorUnifiedMessagingRequestFailed,
		ErrorUnifiedMessagingServerNotFound,
		ErrorUnableToGetUserOofSettings,
		ErrorUnsupportedSubFilter,
		ErrorUnsupportedCulture,
		ErrorUnsupportedMapiPropertyType,
		ErrorUnsupportedMimeConversion,
		ErrorUnsupportedPathForQuery,
		ErrorUnsupportedPathForSortGroup,
		ErrorUnsupportedPropertyDefinition,
		ErrorUnsupportedQueryFilter,
		ErrorUnsupportedRecurrence,
		ErrorUnsupportedTypeForConversion,
		ErrorUpdateDelegatesFailed,
		ErrorUserNotUnifiedMessagingEnabled,
		ErrorVoiceMailNotImplemented,
		ErrorVirusDetected,
		ErrorVirusMessageDeleted,
		ErrorWebRequestInInvalidState,
		ErrorWin32InteropError,
		ErrorWorkingHoursSaveFailed,
		ErrorWorkingHoursXmlMalformed,
		ErrorWrongServerVersion,
		ErrorWrongServerVersionDelegate,
		ErrorMissingInformationSharingFolderId,
		ErrorDuplicateSOAPHeader,
		ErrorSharingSynchronizationFailed,
		ErrorSharingNoExternalEwsAvailable,
		ErrorFreeBusyDLLimitReached,
		ErrorInvalidGetSharingFolderRequest,
		ErrorNotAllowedExternalSharingByPolicy,
		ErrorUserNotAllowedByPolicy,
		ErrorPermissionNotAllowedByPolicy,
		ErrorOrganizationNotFederated,
		ErrorMailboxFailover,
		ErrorInvalidExternalSharingInitiator,
		ErrorMessageTrackingPermanentError,
		ErrorMessageTrackingTransientError,
		ErrorMessageTrackingNoSuchDomain
	}
}
