﻿using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public enum ResponseCodeType
	{
		NoError,
		ErrorAccessDenied,
		ErrorAccessModeSpecified,
		ErrorAccountDisabled,
		ErrorAddDelegatesFailed,
		ErrorAddressSpaceNotFound,
		ErrorADOperation,
		ErrorADSessionFilter,
		ErrorADUnavailable,
		ErrorAutoDiscoverFailed,
		ErrorAffectedTaskOccurrencesRequired,
		ErrorAttachmentNestLevelLimitExceeded,
		ErrorAttachmentSizeLimitExceeded,
		ErrorArchiveFolderPathCreation,
		ErrorArchiveMailboxNotEnabled,
		ErrorArchiveMailboxServiceDiscoveryFailed,
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
		ErrorCallerIsInvalidADAccount,
		ErrorCannotArchiveCalendarContactTaskFolderException,
		ErrorCannotArchiveItemsInPublicFolders,
		ErrorCannotArchiveItemsInArchiveMailbox,
		ErrorCannotCreateCalendarItemInNonCalendarFolder,
		ErrorCannotCreateContactInNonContactFolder,
		ErrorCannotCreatePostItemInNonMailFolder,
		ErrorCannotCreateTaskInNonTaskFolder,
		ErrorCannotDeleteObject,
		ErrorCannotDisableMandatoryExtension,
		ErrorCannotGetSourceFolderPath,
		ErrorCannotGetExternalEcpUrl,
		ErrorCannotOpenFileAttachment,
		ErrorCannotDeleteTaskOccurrence,
		ErrorCannotEmptyFolder,
		ErrorCannotSetCalendarPermissionOnNonCalendarFolder,
		ErrorCannotSetNonCalendarPermissionOnCalendarFolder,
		ErrorCannotSetPermissionUnknownEntries,
		ErrorCannotSpecifySearchFolderAsSourceFolder,
		ErrorCannotUseFolderIdForItemId,
		ErrorCannotUseItemIdForFolderId,
		ErrorChangeKeyRequired,
		ErrorChangeKeyRequiredForWriteOperations,
		ErrorClientDisconnected,
		ErrorClientIntentInvalidStateDefinition,
		ErrorClientIntentNotFound,
		ErrorConnectionFailed,
		ErrorContainsFilterWrongType,
		ErrorContentConversionFailed,
		ErrorContentIndexingNotEnabled,
		ErrorCorruptData,
		ErrorCreateItemAccessDenied,
		ErrorCreateManagedFolderPartialCompletion,
		ErrorCreateSubfolderAccessDenied,
		ErrorCrossMailboxMoveCopy,
		ErrorCrossSiteRequest,
		ErrorDataSizeLimitExceeded,
		ErrorDataSourceOperation,
		ErrorDelegateAlreadyExists,
		ErrorDelegateCannotAddOwner,
		ErrorDelegateMissingConfiguration,
		ErrorDelegateNoUser,
		ErrorDelegateValidationFailed,
		ErrorDeleteDistinguishedFolder,
		ErrorDeleteItemsFailed,
		ErrorDeleteUnifiedMessagingPromptFailed,
		ErrorDistinguishedUserNotSupported,
		ErrorDistributionListMemberNotExist,
		ErrorDuplicateInputFolderNames,
		ErrorDuplicateUserIdsSpecified,
		ErrorEmailAddressMismatch,
		ErrorEventNotFound,
		ErrorExceededConnectionCount,
		ErrorExceededSubscriptionCount,
		ErrorExceededFindCountLimit,
		ErrorExpiredSubscription,
		ErrorExtensionNotFound,
		ErrorFolderCorrupt,
		ErrorFolderNotFound,
		ErrorFolderPropertRequestFailed,
		ErrorFolderSave,
		ErrorFolderSaveFailed,
		ErrorFolderSavePropertyError,
		ErrorFolderExists,
		ErrorFreeBusyGenerationFailed,
		ErrorGetServerSecurityDescriptorFailed,
		ErrorImContactLimitReached,
		ErrorImGroupDisplayNameAlreadyExists,
		ErrorImGroupLimitReached,
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
		ErrorInvalidContactEmailAddress,
		ErrorInvalidContactEmailIndex,
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
		ErrorInvalidImContactId,
		ErrorInvalidImDistributionGroupSmtpAddress,
		ErrorInvalidImGroupId,
		ErrorInvalidIndexedPagingParameters,
		ErrorInvalidInternetHeaderChildNodes,
		ErrorInvalidItemForOperationArchiveItem,
		ErrorInvalidItemForOperationCreateItemAttachment,
		ErrorInvalidItemForOperationCreateItem,
		ErrorInvalidItemForOperationAcceptItem,
		ErrorInvalidItemForOperationDeclineItem,
		ErrorInvalidItemForOperationCancelItem,
		ErrorInvalidItemForOperationExpandDL,
		ErrorInvalidItemForOperationRemoveItem,
		ErrorInvalidItemForOperationSendItem,
		ErrorInvalidItemForOperationTentative,
		ErrorInvalidLogonType,
		ErrorInvalidLikeRequest,
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
		ErrorInvalidRetentionTagTypeMismatch,
		ErrorInvalidRetentionTagInvisible,
		ErrorInvalidRetentionTagInheritance,
		ErrorInvalidRetentionTagIdGuid,
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
		ErrorMailboxHoldNotFound,
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
		ErrorMultiLegacyMailboxAccess,
		ErrorNameResolutionMultipleResults,
		ErrorNameResolutionNoMailbox,
		ErrorNameResolutionNoResults,
		ErrorNoApplicableProxyCASServersAvailable,
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
		ErrorNoPublicFolderServerAvailable,
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
		ErrorPromptPublishingOperationFailed,
		ErrorPropertyValidationFailure,
		ErrorProxiedSubscriptionCallFailure,
		ErrorProxyCallFailed,
		ErrorProxyGroupSidLimitExceeded,
		ErrorProxyRequestNotAllowed,
		ErrorProxyRequestProcessingFailed,
		ErrorProxyServiceDiscoveryFailed,
		ErrorProxyTokenExpired,
		ErrorPublicFolderMailboxDiscoveryFailed,
		ErrorPublicFolderOperationFailed,
		ErrorPublicFolderRequestProcessingFailed,
		ErrorPublicFolderServerNotFound,
		ErrorPublicFolderSyncException,
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
		ErrorSubscriptionUnsubscribed,
		ErrorSyncFolderNotFound,
		ErrorTeamMailboxNotFound,
		ErrorTeamMailboxNotLinkedToSharePoint,
		ErrorTeamMailboxUrlValidationFailed,
		ErrorTeamMailboxNotAuthorizedOwner,
		ErrorTeamMailboxActiveToPendingDelete,
		ErrorTeamMailboxFailedSendingNotifications,
		ErrorTeamMailboxErrorUnknown,
		ErrorTimeIntervalTooBig,
		ErrorTimeoutExpired,
		ErrorTimeZone,
		ErrorToFolderNotFound,
		ErrorTokenSerializationDenied,
		ErrorTooManyObjectsOpened,
		ErrorUpdatePropertyMismatch,
		ErrorUnifiedMessagingDialPlanNotFound,
		ErrorUnifiedMessagingReportDataNotFound,
		ErrorUnifiedMessagingPromptNotFound,
		ErrorUnifiedMessagingRequestFailed,
		ErrorUnifiedMessagingServerNotFound,
		ErrorUnableToGetUserOofSettings,
		ErrorUnableToRemoveImContactFromGroup,
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
		ErrorValueOutOfRange,
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
		ErrorMessageTrackingNoSuchDomain,
		ErrorUserWithoutFederatedProxyAddress,
		ErrorInvalidOrganizationRelationshipForFreeBusy,
		ErrorInvalidFederatedOrganizationId,
		ErrorInvalidExternalSharingSubscriber,
		ErrorInvalidSharingData,
		ErrorInvalidSharingMessage,
		ErrorNotSupportedSharingMessage,
		ErrorApplyConversationActionFailed,
		ErrorInboxRulesValidationError,
		ErrorOutlookRuleBlobExists,
		ErrorRulesOverQuota,
		ErrorNewEventStreamConnectionOpened,
		ErrorMissedNotificationEvents,
		ErrorDuplicateLegacyDistinguishedName,
		ErrorInvalidClientAccessTokenRequest,
		ErrorNoSpeechDetected,
		ErrorUMServerUnavailable,
		ErrorRecipientNotFound,
		ErrorRecognizerNotInstalled,
		ErrorSpeechGrammarError,
		ErrorInvalidManagementRoleHeader,
		ErrorLocationServicesDisabled,
		ErrorLocationServicesRequestTimedOut,
		ErrorLocationServicesRequestFailed,
		ErrorLocationServicesInvalidRequest,
		ErrorWeatherServiceDisabled,
		ErrorMailboxScopeNotAllowedWithoutQueryString,
		ErrorArchiveMailboxSearchFailed,
		ErrorGetRemoteArchiveFolderFailed,
		ErrorFindRemoteArchiveFolderFailed,
		ErrorGetRemoteArchiveItemFailed,
		ErrorExportRemoteArchiveItemsFailed,
		ErrorInvalidPhotoSize,
		ErrorSearchQueryHasTooManyKeywords,
		ErrorSearchTooManyMailboxes,
		ErrorInvalidRetentionTagNone,
		ErrorDiscoverySearchesDisabled,
		ErrorCalendarSeekToConditionNotSupported,
		ErrorCalendarIsGroupMailboxForAccept,
		ErrorCalendarIsGroupMailboxForDecline,
		ErrorCalendarIsGroupMailboxForTentative,
		ErrorCalendarIsGroupMailboxForSuppressReadReceipt,
		ErrorOrganizationAccessBlocked,
		ErrorInvalidLicense,
		ErrorMessagePerFolderCountReceiveQuotaExceeded
	}
}
