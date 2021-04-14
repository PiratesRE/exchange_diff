using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Services
{
	internal static class CoreResources
	{
		static CoreResources()
		{
			CoreResources.stringIDs.Add(580413482U, "ErrorCannotSaveSentItemInArchiveFolder");
			CoreResources.stringIDs.Add(844193848U, "ErrorMissingUserIdInformation");
			CoreResources.stringIDs.Add(2524429953U, "ErrorSearchConfigurationNotFound");
			CoreResources.stringIDs.Add(1087525243U, "ErrorCannotCreateContactInNonContactFolder");
			CoreResources.stringIDs.Add(1049269714U, "IrmFeatureDisabled");
			CoreResources.stringIDs.Add(1795180790U, "EwsProxyResponseTooBig");
			CoreResources.stringIDs.Add(3858003337U, "UpdateFavoritesUnableToDeleteFavoriteEntry");
			CoreResources.stringIDs.Add(754853510U, "ErrorUpdateDelegatesFailed");
			CoreResources.stringIDs.Add(1991349599U, "ErrorNoMailboxSpecifiedForSearchOperation");
			CoreResources.stringIDs.Add(1455468349U, "ErrorCannotApplyHoldOperationOnDG");
			CoreResources.stringIDs.Add(1944820597U, "ErrorInvalidExchangeImpersonationHeaderData");
			CoreResources.stringIDs.Add(1504384645U, "ExOrganizerCannotCallUpdateCalendarItem");
			CoreResources.stringIDs.Add(2498511721U, "IrmViewRightNotGranted");
			CoreResources.stringIDs.Add(93006148U, "UpdateNonDraftItemInDumpsterNotAllowed");
			CoreResources.stringIDs.Add(2252936850U, "ErrorIPGatewayNotFound");
			CoreResources.stringIDs.Add(2517173182U, "ErrorInvalidPropertyForOperation");
			CoreResources.stringIDs.Add(574561672U, "ErrorNameResolutionNoResults");
			CoreResources.stringIDs.Add(4225005690U, "ErrorInvalidItemForOperationCreateItemAttachment");
			CoreResources.stringIDs.Add(3599592070U, "Loading");
			CoreResources.stringIDs.Add(2339310738U, "ErrorItemSave");
			CoreResources.stringIDs.Add(3413891549U, "SubjectColon");
			CoreResources.stringIDs.Add(2181052460U, "ErrorInvalidItemForOperationExpandDL");
			CoreResources.stringIDs.Add(1978429817U, "MessageApplicationHasNoUserApplicationRoleAssigned");
			CoreResources.stringIDs.Add(3167358706U, "ErrorCalendarIsCancelledMessageSent");
			CoreResources.stringIDs.Add(1633083780U, "ErrorInvalidUserInfo");
			CoreResources.stringIDs.Add(2945703152U, "ErrorCalendarViewRangeTooBig");
			CoreResources.stringIDs.Add(495132450U, "ErrorCalendarIsOrganizerForRemove");
			CoreResources.stringIDs.Add(244533303U, "ErrorInvalidRecipientSubfilterComparison");
			CoreResources.stringIDs.Add(2476021338U, "ErrorPassingActingAsForUMConfig");
			CoreResources.stringIDs.Add(3060608191U, "ErrorUserWithoutFederatedProxyAddress");
			CoreResources.stringIDs.Add(3825363766U, "ErrorInvalidSendItemSaveSettings");
			CoreResources.stringIDs.Add(3533302998U, "ErrorWrongServerVersion");
			CoreResources.stringIDs.Add(3735354645U, "ErrorAssociatedTraversalDisallowedWithViewFilter");
			CoreResources.stringIDs.Add(4045771774U, "ErrorMailboxHoldIsNotPermitted");
			CoreResources.stringIDs.Add(4197444273U, "ErrorDuplicateSOAPHeader");
			CoreResources.stringIDs.Add(2744667914U, "ErrorInvalidValueForPropertyUserConfigurationName");
			CoreResources.stringIDs.Add(3510999536U, "ErrorIncorrectSchemaVersion");
			CoreResources.stringIDs.Add(143544280U, "ErrorImpersonationRequiredForPush");
			CoreResources.stringIDs.Add(3135900505U, "ErrorUnifiedMessagingPromptNotFound");
			CoreResources.stringIDs.Add(3227656327U, "ErrorCalendarMeetingRequestIsOutOfDate");
			CoreResources.stringIDs.Add(1234709444U, "MessageExtensionNotAllowedToCreateFAI");
			CoreResources.stringIDs.Add(2966054199U, "ErrorFolderCorrupt");
			CoreResources.stringIDs.Add(2306155022U, "ErrorManagedFolderNotFound");
			CoreResources.stringIDs.Add(1701713067U, "MessageManagementRoleHeaderCannotUseWithOtherHeaders");
			CoreResources.stringIDs.Add(2285125742U, "ErrorQueryFilterTooLong");
			CoreResources.stringIDs.Add(630435929U, "MessageApplicationUnableActAsUser");
			CoreResources.stringIDs.Add(2886480659U, "ErrorInvalidContactEmailIndex");
			CoreResources.stringIDs.Add(1248433804U, "MessageMalformedSoapHeader");
			CoreResources.stringIDs.Add(3629808665U, "ConversationItemQueryFailed");
			CoreResources.stringIDs.Add(4038759526U, "ErrorADOperation");
			CoreResources.stringIDs.Add(2633097826U, "ErrorCalendarIsOrganizerForAccept");
			CoreResources.stringIDs.Add(3049158008U, "ErrorCannotDeleteTaskOccurrence");
			CoreResources.stringIDs.Add(2291046867U, "ErrorTooManyContactsException");
			CoreResources.stringIDs.Add(3577190220U, "ErrorReadEventsFailed");
			CoreResources.stringIDs.Add(1264061593U, "descInvalidEIParameter");
			CoreResources.stringIDs.Add(3584287689U, "ErrorDuplicateLegacyDistinguishedName");
			CoreResources.stringIDs.Add(2886782397U, "MessageActingAsIsNotAValidEmailAddress");
			CoreResources.stringIDs.Add(1703911099U, "MessageInvalidServerVersionForJsonRequest");
			CoreResources.stringIDs.Add(706889665U, "ErrorCalendarCannotMoveOrCopyOccurrence");
			CoreResources.stringIDs.Add(1747094812U, "ErrorPeopleConnectionNotFound");
			CoreResources.stringIDs.Add(3407017993U, "ErrorCalendarMeetingIsOutOfDateResponseNotProcessedMessageSent");
			CoreResources.stringIDs.Add(2122949970U, "ErrorInvalidExcludesRestriction");
			CoreResources.stringIDs.Add(789479259U, "ErrorMoreThanOneAccessModeSpecified");
			CoreResources.stringIDs.Add(4062262029U, "ErrorCreateSubfolderAccessDenied");
			CoreResources.stringIDs.Add(1244610207U, "ErrorInvalidMailboxIdFormat");
			CoreResources.stringIDs.Add(275979752U, "ErrorCalendarIsCancelledForAccept");
			CoreResources.stringIDs.Add(1596102169U, "MessageApplicationRoleShouldPresentWhenUserRolePresent");
			CoreResources.stringIDs.Add(3078968203U, "ErrorInvalidUMSubscriberDataTimeoutValue");
			CoreResources.stringIDs.Add(602009568U, "ErrorSearchTimeoutExpired");
			CoreResources.stringIDs.Add(3471859246U, "descLocalServerConfigurationRetrievalFailed");
			CoreResources.stringIDs.Add(3156759755U, "ErrorInvalidContactEmailAddress");
			CoreResources.stringIDs.Add(1000223261U, "ErrorInvalidValueForPropertyStringArrayDictionaryKey");
			CoreResources.stringIDs.Add(3941855338U, "ErrorChangeKeyRequiredForWriteOperations");
			CoreResources.stringIDs.Add(4767764U, "ErrorMissingEmailAddress");
			CoreResources.stringIDs.Add(932372376U, "ErrorFullSyncRequiredException");
			CoreResources.stringIDs.Add(2058899143U, "ErrorADSessionFilter");
			CoreResources.stringIDs.Add(4170132598U, "ErrorDistinguishedUserNotSupported");
			CoreResources.stringIDs.Add(3681363043U, "ErrorCrossForestCallerNeedsADObject");
			CoreResources.stringIDs.Add(3422864683U, "ErrorSendMeetingInvitationsOrCancellationsRequired");
			CoreResources.stringIDs.Add(349902350U, "RuleErrorDuplicatedOperationOnTheSameRule");
			CoreResources.stringIDs.Add(2151362503U, "ErrorDeletePersonaOnInvalidFolder");
			CoreResources.stringIDs.Add(1407341086U, "ErrorCannotAddAggregatedAccountMailbox");
			CoreResources.stringIDs.Add(1108442436U, "ErrorExceededConnectionCount");
			CoreResources.stringIDs.Add(1153968262U, "ErrorFolderSavePropertyError");
			CoreResources.stringIDs.Add(590346139U, "ErrorCannotUsePersonalContactsAsRecipientsOrAttendees");
			CoreResources.stringIDs.Add(4004906780U, "ErrorInvalidItemForForward");
			CoreResources.stringIDs.Add(2538925974U, "ErrorChangeKeyRequired");
			CoreResources.stringIDs.Add(147036963U, "ErrorNotAcceptable");
			CoreResources.stringIDs.Add(2055932554U, "ErrorMessageTrackingNoSuchDomain");
			CoreResources.stringIDs.Add(1412463668U, "ErrorTraversalNotAllowedWithoutQueryString");
			CoreResources.stringIDs.Add(3276585407U, "ErrorOrganizationAccessBlocked");
			CoreResources.stringIDs.Add(3169826345U, "ErrorInvalidNumberOfMailboxSearch");
			CoreResources.stringIDs.Add(727968988U, "ErrorCreateManagedFolderPartialCompletion");
			CoreResources.stringIDs.Add(1486053208U, "UpdateFavoritesUnableToRenameFavorite");
			CoreResources.stringIDs.Add(170634829U, "ErrorActiveDirectoryTransientError");
			CoreResources.stringIDs.Add(2956059769U, "ErrorInvalidSubscriptionRequestAllFoldersWithFolderIds");
			CoreResources.stringIDs.Add(2990730164U, "ErrorInvalidOperationSendMeetingInvitationCancellationForPublicFolderItem");
			CoreResources.stringIDs.Add(593146268U, "ErrorIrresolvableConflict");
			CoreResources.stringIDs.Add(479697568U, "ErrorInvalidItemForReplyAll");
			CoreResources.stringIDs.Add(4266358168U, "ErrorPhoneNumberNotDialable");
			CoreResources.stringIDs.Add(4187771604U, "ErrorInvalidInternetHeaderChildNodes");
			CoreResources.stringIDs.Add(3459701324U, "ErrorInvalidExpressionTypeForSubFilter");
			CoreResources.stringIDs.Add(3931903304U, "MessageResolveNamesNotSufficientPermissionsToPrivateDLMember");
			CoreResources.stringIDs.Add(924348366U, "ErrorCannotSetNonCalendarPermissionOnCalendarFolder");
			CoreResources.stringIDs.Add(2126704764U, "ErrorParentFolderIdRequired");
			CoreResources.stringIDs.Add(1854021767U, "ErrorEventNotFound");
			CoreResources.stringIDs.Add(3731723330U, "ErrorVoiceMailNotImplemented");
			CoreResources.stringIDs.Add(3448951775U, "ErrorDeleteDistinguishedFolder");
			CoreResources.stringIDs.Add(2354781453U, "ErrorNoPermissionToSearchOrHoldMailbox");
			CoreResources.stringIDs.Add(3213161861U, "ErrorExchangeApplicationNotEnabled");
			CoreResources.stringIDs.Add(1634698783U, "ErrorResolveNamesInvalidFolderType");
			CoreResources.stringIDs.Add(2226715912U, "ErrorExceededFindCountLimit");
			CoreResources.stringIDs.Add(2771555298U, "MessageExtensionAccessActAsMailboxOnly");
			CoreResources.stringIDs.Add(3093510304U, "ErrorPasswordChangeRequired");
			CoreResources.stringIDs.Add(254805997U, "ErrorInvalidManagedFolderProperty");
			CoreResources.stringIDs.Add(1565810069U, "ErrorInvalidIdMalformedEwsLegacyIdFormat");
			CoreResources.stringIDs.Add(1538229710U, "ErrorSchemaViolation");
			CoreResources.stringIDs.Add(478602263U, "MessageInvalidMailboxContactAddressNotFound");
			CoreResources.stringIDs.Add(1293185920U, "ErrorInvalidIndexedPagingParameters");
			CoreResources.stringIDs.Add(361161677U, "ErrorUnsupportedPathForQuery");
			CoreResources.stringIDs.Add(3721795127U, "ErrorInvalidOperationDelegationAssociatedItem");
			CoreResources.stringIDs.Add(4256465912U, "ErrorRemoteUserMailboxMustSpecifyExplicitLocalMailbox");
			CoreResources.stringIDs.Add(1473115829U, "ErrorNoDestinationCASDueToVersionMismatch");
			CoreResources.stringIDs.Add(1805735881U, "ErrorInvalidValueForPropertyBinaryData");
			CoreResources.stringIDs.Add(2410622290U, "ErrorNotDelegate");
			CoreResources.stringIDs.Add(1829860367U, "ErrorCalendarInvalidDayForTimeChangePattern");
			CoreResources.stringIDs.Add(1988987848U, "ErrorInvalidPullSubscriptionId");
			CoreResources.stringIDs.Add(2571138389U, "ErrorCannotCopyPublicFolderRoot");
			CoreResources.stringIDs.Add(1967767132U, "MessageOperationRequiresUserContext");
			CoreResources.stringIDs.Add(2217412679U, "ErrorPromptPublishingOperationFailed");
			CoreResources.stringIDs.Add(2620420056U, "ErrorInvalidFractionalPagingParameters");
			CoreResources.stringIDs.Add(4236561690U, "ErrorPublicFolderMailboxDiscoveryFailed");
			CoreResources.stringIDs.Add(3162641137U, "ErrorUnableToRemoveImContactFromGroup");
			CoreResources.stringIDs.Add(1549704648U, "ErrorSendMeetingCancellationsRequired");
			CoreResources.stringIDs.Add(2011475698U, "MessageRecipientsArrayMustNotBeEmpty");
			CoreResources.stringIDs.Add(757111886U, "ErrorInvalidItemForOperationTentative");
			CoreResources.stringIDs.Add(2519519915U, "ErrorInvalidReferenceItem");
			CoreResources.stringIDs.Add(1314141112U, "IrmReachNotConfigured");
			CoreResources.stringIDs.Add(3907819958U, "ErrorMimeContentInvalidBase64String");
			CoreResources.stringIDs.Add(364289873U, "ErrorSentTaskRequestUpdate");
			CoreResources.stringIDs.Add(1527356366U, "ErrorFoundSyncRequestForNonAggregatedAccount");
			CoreResources.stringIDs.Add(3384523424U, "MessagePropertyIsDeprecatedForThisVersion");
			CoreResources.stringIDs.Add(3954262679U, "ErrorInvalidOperationContactsViewAssociatedItem");
			CoreResources.stringIDs.Add(3655513582U, "ErrorServerBusy");
			CoreResources.stringIDs.Add(3967405104U, "ConversationActionNeedRetentionPolicyTypeForSetRetentionPolicy");
			CoreResources.stringIDs.Add(2671356913U, "ErrorCannotDeletePublicFolderRoot");
			CoreResources.stringIDs.Add(3809605342U, "ErrorImGroupDisplayNameAlreadyExists");
			CoreResources.stringIDs.Add(384737734U, "NoServer");
			CoreResources.stringIDs.Add(948947750U, "ErrorInvalidImDistributionGroupSmtpAddress");
			CoreResources.stringIDs.Add(3640136739U, "ErrorSubscriptionDelegateAccessNotSupported");
			CoreResources.stringIDs.Add(273046712U, "RuleErrorItemIsNotTemplate");
			CoreResources.stringIDs.Add(2549623104U, "ErrorCannotSetPermissionUnknownEntries");
			CoreResources.stringIDs.Add(357277919U, "MessageIdOrTokenTypeNotFound");
			CoreResources.stringIDs.Add(2451443999U, "ErrorLocationServicesDisabled");
			CoreResources.stringIDs.Add(3773912990U, "MessageNotSupportedApplicationRole");
			CoreResources.stringIDs.Add(2636256287U, "ErrorPublicFolderSyncException");
			CoreResources.stringIDs.Add(1448063240U, "ErrorCalendarIsDelegatedForDecline");
			CoreResources.stringIDs.Add(435342351U, "ErrorUnsupportedODataRequest");
			CoreResources.stringIDs.Add(4039615479U, "ErrorDeepTraversalsNotAllowedOnPublicFolders");
			CoreResources.stringIDs.Add(2521448946U, "MessageCouldNotFindWeatherLocationsForSearchString");
			CoreResources.stringIDs.Add(2566235088U, "ErrorInvalidPropertyForSortBy");
			CoreResources.stringIDs.Add(3823874672U, "MessageCalendarUnableToGetAssociatedCalendarItem");
			CoreResources.stringIDs.Add(2841035169U, "ErrorSortByPropertyIsNotFoundOrNotSupported");
			CoreResources.stringIDs.Add(3991730990U, "ErrorNotSupportedSharingMessage");
			CoreResources.stringIDs.Add(1492851991U, "ErrorMissingInformationReferenceItemId");
			CoreResources.stringIDs.Add(1825729465U, "ErrorInvalidSIPUri");
			CoreResources.stringIDs.Add(3371984686U, "ErrorInvalidCompleteDateOutOfRange");
			CoreResources.stringIDs.Add(226219872U, "ErrorUnifiedMessagingDialPlanNotFound");
			CoreResources.stringIDs.Add(2688988465U, "MessageRecipientMustHaveRoutingType");
			CoreResources.stringIDs.Add(3793759002U, "MessageResolveNamesNotSufficientPermissionsToPrivateDL");
			CoreResources.stringIDs.Add(1964352390U, "MessageMissingUserRolesForOrganizationConfigurationRoleTypeApp");
			CoreResources.stringIDs.Add(368663972U, "ErrorInvalidUserSid");
			CoreResources.stringIDs.Add(1954916878U, "ErrorInvalidRecipientSubfilter");
			CoreResources.stringIDs.Add(248293106U, "ErrorSuffixSearchNotAllowed");
			CoreResources.stringIDs.Add(283029019U, "ErrorUnifiedMessagingReportDataNotFound");
			CoreResources.stringIDs.Add(1502984804U, "UpdateFavoritesFolderAlreadyInFavorites");
			CoreResources.stringIDs.Add(2329012714U, "MessageManagementRoleHeaderNotSupportedForOfficeExtension");
			CoreResources.stringIDs.Add(2614511650U, "OneDriveProAttachmentDataProviderName");
			CoreResources.stringIDs.Add(2961161516U, "ErrorCalendarInvalidAttributeValue");
			CoreResources.stringIDs.Add(3854873845U, "MessageInvalidRecurrenceFormat");
			CoreResources.stringIDs.Add(2449079760U, "ErrorInvalidAppApiVersionSupported");
			CoreResources.stringIDs.Add(4227165423U, "ErrorInvalidManagedFolderSize");
			CoreResources.stringIDs.Add(3279473776U, "ErrorTokenSerializationDenied");
			CoreResources.stringIDs.Add(3784063568U, "ErrorInvalidRequest");
			CoreResources.stringIDs.Add(2041209694U, "ErrorSubscriptionUnsubscribed");
			CoreResources.stringIDs.Add(1426183245U, "ErrorInvalidItemForOperationCancelItem");
			CoreResources.stringIDs.Add(684230472U, "IrmCorruptProtectedMessage");
			CoreResources.stringIDs.Add(920557414U, "ErrorCalendarIsGroupMailboxForAccept");
			CoreResources.stringIDs.Add(2933656041U, "ErrorMailboxSearchFailed");
			CoreResources.stringIDs.Add(1188755898U, "ErrorMailboxConfiguration");
			CoreResources.stringIDs.Add(382988367U, "RuleErrorNotSettable");
			CoreResources.stringIDs.Add(4177991609U, "ErrorCopyPublicFolderNotSupported");
			CoreResources.stringIDs.Add(3312780993U, "ErrorInvalidWatermark");
			CoreResources.stringIDs.Add(310545492U, "ErrorActingAsUserNotFound");
			CoreResources.stringIDs.Add(3438146603U, "ErrorDelegateMissingConfiguration");
			CoreResources.stringIDs.Add(1562596869U, "MessageCalendarUnableToUpdateAssociatedCalendarItem");
			CoreResources.stringIDs.Add(2555117076U, "MessageMissingMailboxOwnerEmailAddress");
			CoreResources.stringIDs.Add(3080514177U, "ErrorSentMeetingRequestUpdate");
			CoreResources.stringIDs.Add(2141183275U, "descInvalidTimeZone");
			CoreResources.stringIDs.Add(1733132070U, "ErrorInvalidOperationDisposalTypeAssociatedItem");
			CoreResources.stringIDs.Add(59180037U, "UpdateFavoritesMoveTypeMustBeSet");
			CoreResources.stringIDs.Add(436889836U, "ConversationActionNeedDeleteTypeForSetDeleteAction");
			CoreResources.stringIDs.Add(3616451054U, "ErrorInvalidProxySecurityContext");
			CoreResources.stringIDs.Add(570782166U, "ErrorInvalidValueForProperty");
			CoreResources.stringIDs.Add(1904020973U, "ErrorInvalidRestriction");
			CoreResources.stringIDs.Add(2909492621U, "RuleErrorInvalidAddress");
			CoreResources.stringIDs.Add(599310039U, "RuleErrorSizeLessThanZero");
			CoreResources.stringIDs.Add(3664749912U, "Orange");
			CoreResources.stringIDs.Add(3611326890U, "ErrorRecipientTypeNotSupported");
			CoreResources.stringIDs.Add(3632066599U, "ErrorInvalidIdTooManyAttachmentLevels");
			CoreResources.stringIDs.Add(523664899U, "ErrorExportRemoteArchiveItemsFailed");
			CoreResources.stringIDs.Add(1303377787U, "ErrorCannotSendMessageFromPublicFolder");
			CoreResources.stringIDs.Add(3694049238U, "MessageInsufficientPermissions");
			CoreResources.stringIDs.Add(2611688746U, "MessageCorrelationFailed");
			CoreResources.stringIDs.Add(3819492078U, "ErrorNoMailboxSpecifiedForHoldOperation");
			CoreResources.stringIDs.Add(610144303U, "ErrorTimeZone");
			CoreResources.stringIDs.Add(4260694481U, "ErrorSendAsDenied");
			CoreResources.stringIDs.Add(4292861306U, "MessageSingleOrRecurringCalendarItemExpected");
			CoreResources.stringIDs.Add(2226875331U, "ErrorSearchQueryCannotBeEmpty");
			CoreResources.stringIDs.Add(4136809189U, "ErrorMultipleMailboxesCurrentlyNotSupported");
			CoreResources.stringIDs.Add(4217637937U, "ErrorParentFolderNotFound");
			CoreResources.stringIDs.Add(143488278U, "ErrorDelegateCannotAddOwner");
			CoreResources.stringIDs.Add(3869946114U, "MessageCalendarInsufficientPermissionsToMoveMeetingCancellation");
			CoreResources.stringIDs.Add(73255155U, "ErrorImpersonateUserDenied");
			CoreResources.stringIDs.Add(2875907804U, "ErrorReadReceiptNotPending");
			CoreResources.stringIDs.Add(1676008137U, "ErrorInvalidRetentionTagIdGuid");
			CoreResources.stringIDs.Add(379663703U, "ErrorCannotCreateTaskInNonTaskFolder");
			CoreResources.stringIDs.Add(4074099229U, "MessageNonExistentMailboxNoSmtpAddress");
			CoreResources.stringIDs.Add(2523006528U, "ErrorSchemaValidation");
			CoreResources.stringIDs.Add(3264410200U, "MessageManagementRoleHeaderValueNotApplicable");
			CoreResources.stringIDs.Add(2540872182U, "MessageInvalidRuleVersion");
			CoreResources.stringIDs.Add(1174046717U, "ErrorUnsupportedMimeConversion");
			CoreResources.stringIDs.Add(463452338U, "ErrorCannotMovePublicFolderItemOnDelete");
			CoreResources.stringIDs.Add(966537145U, "ErrorInvalidItemForOperationArchiveItem");
			CoreResources.stringIDs.Add(3021008902U, "ErrorInvalidSearchQuerySyntax");
			CoreResources.stringIDs.Add(4179066588U, "ErrorInvalidValueForCountSystemQueryOption");
			CoreResources.stringIDs.Add(1067402124U, "ErrorFolderSaveFailed");
			CoreResources.stringIDs.Add(2435663882U, "MessageTargetMailboxNotInRoleScope");
			CoreResources.stringIDs.Add(2179607746U, "ErrorInvalidSearchId");
			CoreResources.stringIDs.Add(2674546476U, "ErrorInvalidOperationSyncFolderHierarchyForPublicFolder");
			CoreResources.stringIDs.Add(2624402344U, "ErrorItemCorrupt");
			CoreResources.stringIDs.Add(3120707856U, "ErrorServerTemporaryUnavailable");
			CoreResources.stringIDs.Add(2786380669U, "ErrorCannotArchiveCalendarContactTaskFolderException");
			CoreResources.stringIDs.Add(4123291671U, "ErrorInvalidItemForOperationSendItem");
			CoreResources.stringIDs.Add(68528320U, "ErrorAggregatedAccountAlreadyExists");
			CoreResources.stringIDs.Add(109614196U, "ErrorInvalidServerVersion");
			CoreResources.stringIDs.Add(1487884331U, "ErrorGroupingNonNullWithSuggestionsViewFilter");
			CoreResources.stringIDs.Add(1958477060U, "MessageInvalidMailboxNotPrivateDL");
			CoreResources.stringIDs.Add(1272021886U, "ErrorItemPropertyRequestFailed");
			CoreResources.stringIDs.Add(1706062739U, "ConversationActionNeedDestinationFolderForCopyAction");
			CoreResources.stringIDs.Add(2653243941U, "ErrorLocationServicesRequestFailed");
			CoreResources.stringIDs.Add(220777420U, "UnrecognizedDistinguishedFolderName");
			CoreResources.stringIDs.Add(559784827U, "ErrorInvalidSubfilterTypeNotRecipientType");
			CoreResources.stringIDs.Add(1701761470U, "ErrorInvalidPropertySet");
			CoreResources.stringIDs.Add(3625531057U, "UpdateFavoritesFolderCannotBeNull");
			CoreResources.stringIDs.Add(1326676491U, "ErrorCannotRemoveAggregatedAccountFromList");
			CoreResources.stringIDs.Add(3699987394U, "ErrorProxyTokenExpired");
			CoreResources.stringIDs.Add(3564002022U, "ErrorCannotCreateCalendarItemInNonCalendarFolder");
			CoreResources.stringIDs.Add(3732945645U, "ErrorInvalidOperationGroupByAssociatedItem");
			CoreResources.stringIDs.Add(2890836210U, "MessageCalendarUnableToCreateAssociatedCalendarItem");
			CoreResources.stringIDs.Add(896367800U, "ErrorMultiLegacyMailboxAccess");
			CoreResources.stringIDs.Add(3392207806U, "ErrorUnifiedMailboxAlreadyExists");
			CoreResources.stringIDs.Add(3619206730U, "ErrorInvalidPropertyAppend");
			CoreResources.stringIDs.Add(4261845811U, "ErrorObjectTypeChanged");
			CoreResources.stringIDs.Add(4252616528U, "ErrorSearchableObjectNotFound");
			CoreResources.stringIDs.Add(2498507918U, "ErrorEndTimeMustBeGreaterThanStartTime");
			CoreResources.stringIDs.Add(765833303U, "ErrorInvalidFederatedOrganizationId");
			CoreResources.stringIDs.Add(1583798271U, "MessageExtensionNotAllowedToUpdateFAI");
			CoreResources.stringIDs.Add(1335290147U, "ErrorValueOutOfRange");
			CoreResources.stringIDs.Add(3719196410U, "ErrorNotEnoughMemory");
			CoreResources.stringIDs.Add(3635256568U, "ErrorInvalidExtendedPropertyValue");
			CoreResources.stringIDs.Add(2524108663U, "ErrorMoveCopyFailed");
			CoreResources.stringIDs.Add(1985973150U, "GetClientExtensionTokenFailed");
			CoreResources.stringIDs.Add(3705244005U, "ErrorVirusDetected");
			CoreResources.stringIDs.Add(671866695U, "ErrorInvalidVotingResponse");
			CoreResources.stringIDs.Add(2296308088U, "RuleErrorInboxRulesValidationError");
			CoreResources.stringIDs.Add(1897020671U, "ErrorInvalidIdMonikerTooLong");
			CoreResources.stringIDs.Add(111518940U, "ErrorMultipleSearchRootsDisallowedWithSearchContext");
			CoreResources.stringIDs.Add(4142344047U, "ErrorUserNotUnifiedMessagingEnabled");
			CoreResources.stringIDs.Add(3206878473U, "ErrorCannotMovePublicFolderToPrivateMailbox");
			CoreResources.stringIDs.Add(751424501U, "ConversationActionAlwaysMoveNoPublicFolder");
			CoreResources.stringIDs.Add(1834319386U, "ErrorCallerIsInvalidADAccount");
			CoreResources.stringIDs.Add(3319799507U, "ErrorNoDestinationCASDueToSSLRequirements");
			CoreResources.stringIDs.Add(3995283118U, "ErrorInternalServerTransientError");
			CoreResources.stringIDs.Add(3659985571U, "ErrorInvalidParentFolder");
			CoreResources.stringIDs.Add(2565659540U, "ErrorArchiveFolderPathCreation");
			CoreResources.stringIDs.Add(4066246803U, "MessageCalendarInsufficientPermissionsToMoveItem");
			CoreResources.stringIDs.Add(2791864679U, "ErrorMessagePerFolderCountReceiveQuotaExceeded");
			CoreResources.stringIDs.Add(2643283981U, "ErrorDateTimeNotInUTC");
			CoreResources.stringIDs.Add(2798800298U, "ErrorInvalidAttachmentSubfilter");
			CoreResources.stringIDs.Add(2214456911U, "ErrorUserConfigurationDictionaryNotExist");
			CoreResources.stringIDs.Add(2918743951U, "FromColon");
			CoreResources.stringIDs.Add(2362895530U, "ErrorInvalidSubscriptionRequestNoFolderIds");
			CoreResources.stringIDs.Add(1115854773U, "ErrorCallerIsComputerAccount");
			CoreResources.stringIDs.Add(69571280U, "ErrorDeleteItemsFailed");
			CoreResources.stringIDs.Add(2014273875U, "ErrorNotApplicableOutsideOfDatacenter");
			CoreResources.stringIDs.Add(526527128U, "RuleErrorOutlookRuleBlobExists");
			CoreResources.stringIDs.Add(2843974690U, "descInvalidOofRequestPublicFolder");
			CoreResources.stringIDs.Add(21808504U, "ErrorMailboxIsNotPartOfAggregatedMailboxes");
			CoreResources.stringIDs.Add(2043815785U, "ErrorInvalidRetentionTagNone");
			CoreResources.stringIDs.Add(2448725207U, "MessageInvalidRoleTypeString");
			CoreResources.stringIDs.Add(2343198056U, "MessageInvalidMailboxRecipientNotFoundInActiveDirectory");
			CoreResources.stringIDs.Add(402485116U, "ErrorNoSyncRequestsMatchedSpecifiedEmailAddress");
			CoreResources.stringIDs.Add(2999374145U, "ErrorInvalidDestinationFolderForPostItem");
			CoreResources.stringIDs.Add(377617137U, "ErrorGetRemoteArchiveFolderFailed");
			CoreResources.stringIDs.Add(3410698111U, "RightsManagementMailboxOnlySupport");
			CoreResources.stringIDs.Add(122085112U, "ErrorMissingItemForCreateItemAttachment");
			CoreResources.stringIDs.Add(4160418372U, "ErrorFindRemoteArchiveFolderFailed");
			CoreResources.stringIDs.Add(2989650895U, "ErrorCalendarFolderIsInvalidForCalendarView");
			CoreResources.stringIDs.Add(3359997542U, "ErrorFindConversationNotSupportedForPublicFolders");
			CoreResources.stringIDs.Add(969219158U, "ErrorUserConfigurationBinaryDataNotExist");
			CoreResources.stringIDs.Add(1063299331U, "DefaultHtmlAttachmentHrefText");
			CoreResources.stringIDs.Add(3510846499U, "Green");
			CoreResources.stringIDs.Add(4005418156U, "ErrorItemNotFound");
			CoreResources.stringIDs.Add(2838198776U, "ErrorCannotEmptyFolder");
			CoreResources.stringIDs.Add(777220966U, "Yellow");
			CoreResources.stringIDs.Add(1967895810U, "ErrorInvalidSubscription");
			CoreResources.stringIDs.Add(4281412187U, "ErrorSchemaValidationColon");
			CoreResources.stringIDs.Add(707372475U, "ErrorDelegateNoUser");
			CoreResources.stringIDs.Add(107796140U, "RuleErrorMissingRangeValue");
			CoreResources.stringIDs.Add(2554577046U, "MessageWebMethodUnavailable");
			CoreResources.stringIDs.Add(395078619U, "ErrorUnsupportedQueryFilter");
			CoreResources.stringIDs.Add(968334519U, "ErrorCannotMovePublicFolderOnDelete");
			CoreResources.stringIDs.Add(3314483401U, "ErrorAccessModeSpecified");
			CoreResources.stringIDs.Add(654139516U, "ErrorInvalidPhotoSize");
			CoreResources.stringIDs.Add(2656700117U, "ErrorMultipleMailboxSearchNotSupported");
			CoreResources.stringIDs.Add(728324719U, "MessageManagementRoleHeaderNotSupportedForPartnerIdentity");
			CoreResources.stringIDs.Add(1457631314U, "ConversationActionInvalidFolderType");
			CoreResources.stringIDs.Add(259054457U, "ErrorUnsupportedSubFilter");
			CoreResources.stringIDs.Add(1408093181U, "ErrorInvalidComplianceId");
			CoreResources.stringIDs.Add(3843271914U, "ErrorCalendarCannotUpdateDeletedItem");
			CoreResources.stringIDs.Add(3782996725U, "ErrorInvalidOperationDistinguishedGroupByAssociatedItem");
			CoreResources.stringIDs.Add(3537364541U, "ErrorInvalidDelegatePermission");
			CoreResources.stringIDs.Add(594155080U, "ErrorInternalServerError");
			CoreResources.stringIDs.Add(2356362688U, "ErrorNoPublicFolderServerAvailable");
			CoreResources.stringIDs.Add(3978299680U, "ErrorInvalidPhoneCallId");
			CoreResources.stringIDs.Add(1793222072U, "ErrorInvalidGetSharingFolderRequest");
			CoreResources.stringIDs.Add(2927988853U, "ErrorCannotResolveOrganizationName");
			CoreResources.stringIDs.Add(809187661U, "ErrorUnsupportedCulture");
			CoreResources.stringIDs.Add(865206910U, "ErrorInvalidChangeKey");
			CoreResources.stringIDs.Add(3846347532U, "ErrorMimeContentConversionFailed");
			CoreResources.stringIDs.Add(2683464521U, "ErrorResolveNamesOnlyOneContactsFolderAllowed");
			CoreResources.stringIDs.Add(901489999U, "ErrorInvalidSchemaVersionForMailboxVersion");
			CoreResources.stringIDs.Add(2012012473U, "ErrorInvalidRequestQuotaExceeded");
			CoreResources.stringIDs.Add(768426321U, "MessageTokenRequestUnauthorized");
			CoreResources.stringIDs.Add(3910111167U, "MessageUserRoleNotApplicableForAppOnlyToken");
			CoreResources.stringIDs.Add(828992378U, "ErrorInvalidValueForPropertyKeyValueConversion");
			CoreResources.stringIDs.Add(3769371271U, "ErrorInvalidRetentionTagInheritance");
			CoreResources.stringIDs.Add(710925581U, "Conversation");
			CoreResources.stringIDs.Add(1338511205U, "ErrorCannotCreateUnifiedMailbox");
			CoreResources.stringIDs.Add(1897429247U, "ErrorMailTipsDisabled");
			CoreResources.stringIDs.Add(4222588379U, "ErrorMissingItemIdForCreateItemAttachment");
			CoreResources.stringIDs.Add(1762041369U, "ErrorInvalidMailbox");
			CoreResources.stringIDs.Add(4097108255U, "ErrorDelegateValidationFailed");
			CoreResources.stringIDs.Add(2715027708U, "ErrorUserPromptNeeded");
			CoreResources.stringIDs.Add(1898482716U, "RuleErrorMissingAction");
			CoreResources.stringIDs.Add(706596508U, "ErrorApplyConversationActionFailed");
			CoreResources.stringIDs.Add(60783832U, "ErrorInsufficientResources");
			CoreResources.stringIDs.Add(905739673U, "ErrorActingAsRequired");
			CoreResources.stringIDs.Add(2681298929U, "ErrorCalendarInvalidDayForWeeklyRecurrence");
			CoreResources.stringIDs.Add(549150802U, "ErrorMissingInformationEmailAddress");
			CoreResources.stringIDs.Add(4019544117U, "UpdateFavoritesFavoriteNotFound");
			CoreResources.stringIDs.Add(2484699530U, "ErrorCalendarDurationIsTooLong");
			CoreResources.stringIDs.Add(4252309617U, "ErrorNoRespondingCASInDestinationSite");
			CoreResources.stringIDs.Add(388443881U, "ErrorInvalidRecipients");
			CoreResources.stringIDs.Add(269577600U, "ErrorAppendBodyTypeMismatch");
			CoreResources.stringIDs.Add(514021796U, "ErrorDistributionListMemberNotExist");
			CoreResources.stringIDs.Add(3285224352U, "ErrorRequestTimeout");
			CoreResources.stringIDs.Add(3607262778U, "MessageApplicationHasNoRoleAssginedWhichUserHas");
			CoreResources.stringIDs.Add(3668888236U, "ErrorArchiveMailboxGetConversationFailed");
			CoreResources.stringIDs.Add(2851949310U, "ErrorClientIntentNotFound");
			CoreResources.stringIDs.Add(2489326695U, "ErrorNonExistentMailbox");
			CoreResources.stringIDs.Add(1164605313U, "ErrorVirusMessageDeleted");
			CoreResources.stringIDs.Add(175403818U, "ErrorCannotFindUnifiedMailbox");
			CoreResources.stringIDs.Add(1505256501U, "ErrorUnifiedMailboxSupportedOnlyWithMicrosoftAccount");
			CoreResources.stringIDs.Add(2833024077U, "GroupMailboxCreationFailed");
			CoreResources.stringIDs.Add(1233823477U, "ErrorInvalidSearchQueryLength");
			CoreResources.stringIDs.Add(391940363U, "ErrorCalendarInvalidPropertyState");
			CoreResources.stringIDs.Add(1850561764U, "ErrorAddDelegatesFailed");
			CoreResources.stringIDs.Add(3496891301U, "CcColon");
			CoreResources.stringIDs.Add(1062691260U, "ErrorCrossSiteRequest");
			CoreResources.stringIDs.Add(565625999U, "ErrorPublicFolderUserMustHaveMailbox");
			CoreResources.stringIDs.Add(3399410586U, "ErrorMessageTrackingTransientError");
			CoreResources.stringIDs.Add(1027490726U, "ErrorToFolderNotFound");
			CoreResources.stringIDs.Add(1637412134U, "ErrorDeleteUnifiedMessagingPromptFailed");
			CoreResources.stringIDs.Add(3914315493U, "UpdateFavoritesUnableToMoveFavorite");
			CoreResources.stringIDs.Add(3141449171U, "ErrorPeopleConnectionNoToken");
			CoreResources.stringIDs.Add(3848937923U, "ErrorCannotSpecifySearchFolderAsSourceFolder");
			CoreResources.stringIDs.Add(933080956U, "ErrorEmailAddressMismatch");
			CoreResources.stringIDs.Add(2419720676U, "ErrorUserConfigurationXmlDataNotExist");
			CoreResources.stringIDs.Add(2346704662U, "ErrorUnifiedMessagingRequestFailed");
			CoreResources.stringIDs.Add(1075303082U, "ErrorCreateItemAccessDenied");
			CoreResources.stringIDs.Add(2887245343U, "RuleErrorFolderDoesNotExist");
			CoreResources.stringIDs.Add(3485828594U, "ErrorInvalidImContactId");
			CoreResources.stringIDs.Add(3969305989U, "ErrorNoPropertyTagForCustomProperties");
			CoreResources.stringIDs.Add(2677919833U, "SentTime");
			CoreResources.stringIDs.Add(3279543955U, "MessageNonExistentMailboxGuid");
			CoreResources.stringIDs.Add(216781884U, "ErrorMaxRequestedUnifiedGroupsSetsExceeded");
			CoreResources.stringIDs.Add(3555230765U, "ErrorInvalidAppSchemaVersionSupported");
			CoreResources.stringIDs.Add(3522975510U, "ErrorInvalidLogonType");
			CoreResources.stringIDs.Add(131857255U, "MessageActAsUserRequiredForSuchApplicationRole");
			CoreResources.stringIDs.Add(3773356320U, "ErrorCalendarOutOfRange");
			CoreResources.stringIDs.Add(3975089319U, "ErrorContentIndexingNotEnabled");
			CoreResources.stringIDs.Add(4227151856U, "ErrorContentConversionFailed");
			CoreResources.stringIDs.Add(3426540703U, "ConversationIdNotSupported");
			CoreResources.stringIDs.Add(730941518U, "ConversationSupportedOnlyForMailboxSession");
			CoreResources.stringIDs.Add(3771523283U, "ErrorMoveDistinguishedFolder");
			CoreResources.stringIDs.Add(2092164778U, "ErrorMailboxCannotBeSpecifiedForPublicFolderRoot");
			CoreResources.stringIDs.Add(2805212767U, "IrmPreLicensingFailure");
			CoreResources.stringIDs.Add(734136355U, "MessageMissingUserRolesForLegalHoldRoleTypeApp");
			CoreResources.stringIDs.Add(430009573U, "ErrorMailboxVersionNotSupported");
			CoreResources.stringIDs.Add(560492804U, "ErrorRestrictionTooComplex");
			CoreResources.stringIDs.Add(3546363172U, "RuleErrorRecipientDoesNotExist");
			CoreResources.stringIDs.Add(3667869681U, "ErrorInvalidAggregatedAccountCredentials");
			CoreResources.stringIDs.Add(2653688977U, "descInvalidSecurityContext");
			CoreResources.stringIDs.Add(213621866U, "MessagePublicFoldersNotSupportedForNonIndexable");
			CoreResources.stringIDs.Add(3943930965U, "ErrorInvalidFilterNode");
			CoreResources.stringIDs.Add(1508237301U, "ErrorIrmUserRightNotGranted");
			CoreResources.stringIDs.Add(3956968185U, "descInvalidRequestType");
			CoreResources.stringIDs.Add(184315686U, "DowaNotProvisioned");
			CoreResources.stringIDs.Add(2652436543U, "ErrorRecurrenceEndDateTooBig");
			CoreResources.stringIDs.Add(629782913U, "ErrorInvalidItemForReply");
			CoreResources.stringIDs.Add(1342320011U, "UpdateFavoritesInvalidUpdateFavoriteOperationType");
			CoreResources.stringIDs.Add(2674011741U, "ErrorInvalidManagementRoleHeader");
			CoreResources.stringIDs.Add(200462199U, "ErrorCannotGetExternalEcpUrl");
			CoreResources.stringIDs.Add(1645715101U, "ErrorCannotCreateSearchFolderInPublicFolder");
			CoreResources.stringIDs.Add(3288028209U, "RuleErrorUnsupportedRule");
			CoreResources.stringIDs.Add(2518142400U, "ErrorMissingManagedFolderId");
			CoreResources.stringIDs.Add(1990145025U, "MessageInsufficientPermissionsToSend");
			CoreResources.stringIDs.Add(3098927940U, "ErrorInvalidCompleteDate");
			CoreResources.stringIDs.Add(2447591155U, "ErrorSearchFolderTimeout");
			CoreResources.stringIDs.Add(4089853131U, "ErrorCannotSetAggregatedAccount");
			CoreResources.stringIDs.Add(1962425675U, "ErrorInvalidPushSubscriptionUrl");
			CoreResources.stringIDs.Add(1669051638U, "ErrorCannotAddAggregatedAccount");
			CoreResources.stringIDs.Add(1718538996U, "ErrorCalendarIsGroupMailboxForDecline");
			CoreResources.stringIDs.Add(761574210U, "ErrorNameResolutionNoMailbox");
			CoreResources.stringIDs.Add(2225772284U, "ErrorCannotArchiveItemsInArchiveMailbox");
			CoreResources.stringIDs.Add(556297389U, "MowaNotProvisioned");
			CoreResources.stringIDs.Add(529689091U, "ErrorInvalidOperationSendAndSaveCopyToPublicFolder");
			CoreResources.stringIDs.Add(1998574567U, "ConversationActionNeedDestinationFolderForMoveAction");
			CoreResources.stringIDs.Add(718120058U, "ErrorViewFilterRequiresSearchContext");
			CoreResources.stringIDs.Add(1363870753U, "ErrorDelegateAlreadyExists");
			CoreResources.stringIDs.Add(2479091638U, "ErrorSubmitQueryBasedHoldTaskFailed");
			CoreResources.stringIDs.Add(2869245557U, "ErrorPeopleConnectFailedToReadApplicationConfiguration");
			CoreResources.stringIDs.Add(937093447U, "ErrorUnsupportedMapiPropertyType");
			CoreResources.stringIDs.Add(1829541172U, "ErrorApprovalRequestAlreadyDecided");
			CoreResources.stringIDs.Add(472949644U, "MessageCouldNotFindWeatherLocations");
			CoreResources.stringIDs.Add(3770755973U, "WhenColon");
			CoreResources.stringIDs.Add(70508874U, "ErrorNoGroupingForQueryString");
			CoreResources.stringIDs.Add(2651121857U, "ErrorInvalidIdStoreObjectIdTooLong");
			CoreResources.stringIDs.Add(3654265673U, "ErrorQuotaExceeded");
			CoreResources.stringIDs.Add(3601113588U, "ConversationActionNeedReadStateForSetReadStateAction");
			CoreResources.stringIDs.Add(4226485813U, "ErrorLocationServicesRequestTimedOut");
			CoreResources.stringIDs.Add(3349192959U, "ErrorCalendarInvalidPropertyValue");
			CoreResources.stringIDs.Add(978558141U, "ErrorManagedFolderAlreadyExists");
			CoreResources.stringIDs.Add(1008089967U, "ErrorLocationServicesInvalidSource");
			CoreResources.stringIDs.Add(2560374358U, "OnPremiseSynchorizedDiscoverySearch");
			CoreResources.stringIDs.Add(3859804741U, "ErrorInvalidOperationForAssociatedItems");
			CoreResources.stringIDs.Add(953197733U, "ErrorCorruptData");
			CoreResources.stringIDs.Add(39525862U, "ErrorCalendarInvalidTimeZone");
			CoreResources.stringIDs.Add(3281131813U, "ErrorInvalidOperationMessageDispositionAssociatedItem");
			CoreResources.stringIDs.Add(2662672540U, "ErrorSubscriptionAccessDenied");
			CoreResources.stringIDs.Add(2608213760U, "ErrorCannotReadRequestBody");
			CoreResources.stringIDs.Add(2070630207U, "ErrorNameResolutionMultipleResults");
			CoreResources.stringIDs.Add(866480793U, "ErrorInvalidExtendedProperty");
			CoreResources.stringIDs.Add(3760366944U, "EwsProxyCannotGetCredentials");
			CoreResources.stringIDs.Add(2761096871U, "UpdateFavoritesInvalidMoveFavoriteRequest");
			CoreResources.stringIDs.Add(1359116179U, "ErrorInvalidPermissionSettings");
			CoreResources.stringIDs.Add(1645280882U, "ErrorProxyServiceDiscoveryFailed");
			CoreResources.stringIDs.Add(598450895U, "ErrorInvalidItemForOperationAcceptItem");
			CoreResources.stringIDs.Add(2578390262U, "ErrorInvalidValueForPropertyDuplicateDictionaryKey");
			CoreResources.stringIDs.Add(516980747U, "ErrorExceededSubscriptionCount");
			CoreResources.stringIDs.Add(739553585U, "ErrorPermissionNotAllowedByPolicy");
			CoreResources.stringIDs.Add(48346381U, "MessageInsufficientPermissionsToSubscribe");
			CoreResources.stringIDs.Add(1803820018U, "ErrorInvalidValueForPropertyDate");
			CoreResources.stringIDs.Add(3322365201U, "ErrorUnsupportedRecurrence");
			CoreResources.stringIDs.Add(837503410U, "ErrorUserADObjectNotFound");
			CoreResources.stringIDs.Add(2020376324U, "ErrorCannotAttachSelf");
			CoreResources.stringIDs.Add(2938284467U, "ErrorMissingInformationSharingFolderId");
			CoreResources.stringIDs.Add(1762596806U, "ErrorCannotSetFromOnMeetingResponse");
			CoreResources.stringIDs.Add(3795663900U, "MessageInvalidOperationForPublicFolderItemsAddParticipantByItemId");
			CoreResources.stringIDs.Add(1439158331U, "ErrorInvalidItemForOperationCreateItem");
			CoreResources.stringIDs.Add(3788524313U, "ErrorInvalidPropertyForExists");
			CoreResources.stringIDs.Add(792522617U, "ErrorCannotSaveSentItemInPublicFolder");
			CoreResources.stringIDs.Add(3143473274U, "ErrorRestrictionTooLong");
			CoreResources.stringIDs.Add(789094727U, "ErrorUnsupportedPropertyDefinition");
			CoreResources.stringIDs.Add(3311760175U, "SharePointCreationFailed");
			CoreResources.stringIDs.Add(2935460503U, "ErrorDataSizeLimitExceeded");
			CoreResources.stringIDs.Add(628559436U, "ErrorFolderExists");
			CoreResources.stringIDs.Add(2930851601U, "ErrorUnifiedGroupAlreadyExists");
			CoreResources.stringIDs.Add(1580647852U, "MessageApplicationTokenOnly");
			CoreResources.stringIDs.Add(4047718788U, "ErrorSharingNoExternalEwsAvailable");
			CoreResources.stringIDs.Add(1661960732U, "RuleErrorEmptyValueFound");
			CoreResources.stringIDs.Add(852852329U, "ErrorOccurrenceCrossingBoundary");
			CoreResources.stringIDs.Add(3156121664U, "ErrorArchiveMailboxServiceDiscoveryFailed");
			CoreResources.stringIDs.Add(1064353045U, "ErrorInvalidAttachmentSubfilterTextFilter");
			CoreResources.stringIDs.Add(1966896516U, "ErrorGetSharingMetadataNotSupported");
			CoreResources.stringIDs.Add(1507828071U, "MessageRecipientMustHaveEmailAddress");
			CoreResources.stringIDs.Add(4094604515U, "ErrorInvalidRecipientSubfilterTextFilter");
			CoreResources.stringIDs.Add(3673396595U, "ErrorInvalidPropertyRequest");
			CoreResources.stringIDs.Add(1582215140U, "ErrorCalendarIsNotOrganizer");
			CoreResources.stringIDs.Add(3374101509U, "ErrorInvalidProvisionDeviceID");
			CoreResources.stringIDs.Add(4117821571U, "MessageCouldNotGetWeatherDataForLocation");
			CoreResources.stringIDs.Add(2326085984U, "ErrorTimeProposalMissingStartOrEndTimeError");
			CoreResources.stringIDs.Add(1946206036U, "ErrorInvalidSubfilterTypeNotAttendeeType");
			CoreResources.stringIDs.Add(3890629732U, "PropertyCommandNotSupportSet");
			CoreResources.stringIDs.Add(347738787U, "ErrorImpersonationFailed");
			CoreResources.stringIDs.Add(2884324330U, "ErrorSubscriptionNotFound");
			CoreResources.stringIDs.Add(2225154662U, "MessageCalendarInsufficientPermissionsToMoveMeetingRequest");
			CoreResources.stringIDs.Add(3107705007U, "ErrorInvalidIdMalformed");
			CoreResources.stringIDs.Add(1035957819U, "ErrorCalendarIsGroupMailboxForSuppressReadReceipt");
			CoreResources.stringIDs.Add(2940401781U, "ErrorCannotGetSourceFolderPath");
			CoreResources.stringIDs.Add(4083587704U, "ErrorWildcardAndGroupExpansionNotAllowed");
			CoreResources.stringIDs.Add(4077357270U, "UnsupportedInlineAttachmentContentType");
			CoreResources.stringIDs.Add(1170272727U, "RuleErrorUnexpectedError");
			CoreResources.stringIDs.Add(1601473907U, "MessageCalendarInsufficientPermissionsToDraftsFolder");
			CoreResources.stringIDs.Add(634294555U, "ErrorADUnavailable");
			CoreResources.stringIDs.Add(3260461220U, "ErrorInvalidPhoneNumber");
			CoreResources.stringIDs.Add(547900838U, "ErrorSoftDeletedTraversalsNotAllowedOnPublicFolders");
			CoreResources.stringIDs.Add(2687301200U, "ErrorCalendarIsDelegatedForTentative");
			CoreResources.stringIDs.Add(2952942328U, "ErrorFoldersMustBelongToSameMailbox");
			CoreResources.stringIDs.Add(2697731302U, "ErrorDataSourceOperation");
			CoreResources.stringIDs.Add(3573754788U, "ErrorCalendarMeetingIsOutOfDateResponseNotProcessed");
			CoreResources.stringIDs.Add(1475709851U, "MessageInvalidIdMalformedEwsIdFormat");
			CoreResources.stringIDs.Add(3699315399U, "ErrorPreviousPageNavigationCurrentlyNotSupported");
			CoreResources.stringIDs.Add(2058507107U, "ErrorCannotEmptyPublicFolderToDeletedItems");
			CoreResources.stringIDs.Add(1014449457U, "ErrorInvalidSharingData");
			CoreResources.stringIDs.Add(1746698887U, "MessageCalendarInsufficientPermissionsToMeetingMessageFolder");
			CoreResources.stringIDs.Add(2503843052U, "ErrorInvalidOperationCannotSpecifyItemId");
			CoreResources.stringIDs.Add(3187786876U, "ErrorCalendarIsGroupMailboxForTentative");
			CoreResources.stringIDs.Add(2913173341U, "ErrorMessageSizeExceeded");
			CoreResources.stringIDs.Add(3468080577U, "InvalidDateTimePrecisionValue");
			CoreResources.stringIDs.Add(3943872330U, "ErrorStaleObject");
			CoreResources.stringIDs.Add(3119664543U, "UpdateFavoritesUnableToAddFolderToFavorites");
			CoreResources.stringIDs.Add(1282299710U, "ErrorPasswordExpired");
			CoreResources.stringIDs.Add(3142918589U, "ErrorInvalidOperationCannotPerformOperationOnADRecipients");
			CoreResources.stringIDs.Add(157861094U, "ErrorTooManyObjectsOpened");
			CoreResources.stringIDs.Add(256399585U, "MessageInvalidMailboxInvalidReferencedItem");
			CoreResources.stringIDs.Add(3901728717U, "MessageApplicationHasNoGivenRoleAssigned");
			CoreResources.stringIDs.Add(3113724054U, "MessageRecipientsArrayTooLong");
			CoreResources.stringIDs.Add(3852956793U, "ErrorInvalidIdXml");
			CoreResources.stringIDs.Add(2271901695U, "ErrorCallerWithoutMailboxCannotUseSendOnly");
			CoreResources.stringIDs.Add(2535285679U, "ErrorArchiveMailboxSearchFailed");
			CoreResources.stringIDs.Add(2543409328U, "PostedOn");
			CoreResources.stringIDs.Add(4028591235U, "ErrorInvalidExternalSharingInitiator");
			CoreResources.stringIDs.Add(1627983613U, "ErrorMailboxStoreUnavailable");
			CoreResources.stringIDs.Add(2358398289U, "ErrorInvalidCalendarViewRestrictionOrSort");
			CoreResources.stringIDs.Add(3610830273U, "ErrorSavedItemFolderNotFound");
			CoreResources.stringIDs.Add(3335161738U, "ErrorCalendarOccurrenceIsDeletedFromRecurrence");
			CoreResources.stringIDs.Add(2985674644U, "ErrorMissingRecipients");
			CoreResources.stringIDs.Add(3997746891U, "ErrorTimeProposalInvalidInCreateItemRequest");
			CoreResources.stringIDs.Add(2990436390U, "ErrorCalendarIsDelegatedForRemove");
			CoreResources.stringIDs.Add(4151155219U, "ErrorInvalidLikeRequest");
			CoreResources.stringIDs.Add(271991716U, "MessageRecurrenceStartDateTooSmall");
			CoreResources.stringIDs.Add(4066404319U, "ErrorUnknownTimeZone");
			CoreResources.stringIDs.Add(1656583547U, "ErrorProxyGroupSidLimitExceeded");
			CoreResources.stringIDs.Add(2834376775U, "ErrorCannotRemoveAggregatedAccount");
			CoreResources.stringIDs.Add(1816334244U, "ErrorInvalidShape");
			CoreResources.stringIDs.Add(1812149170U, "ErrorInvalidLicense");
			CoreResources.stringIDs.Add(531497785U, "ErrorAccountDisabled");
			CoreResources.stringIDs.Add(1949840710U, "ErrorHoldIsNotFound");
			CoreResources.stringIDs.Add(1830098328U, "MessageMessageIsNotDraft");
			CoreResources.stringIDs.Add(3778961523U, "ErrorWrongServerVersionDelegate");
			CoreResources.stringIDs.Add(2868846894U, "OnBehalfOf");
			CoreResources.stringIDs.Add(1902653190U, "ErrorInvalidOperationForPublicFolderItems");
			CoreResources.stringIDs.Add(1069471396U, "ErrorCalendarCannotUseIdForRecurringMasterId");
			CoreResources.stringIDs.Add(3647226175U, "ErrorInvalidSubscriptionRequest");
			CoreResources.stringIDs.Add(4226852029U, "ErrorInvalidIdEmpty");
			CoreResources.stringIDs.Add(922305341U, "ErrorInvalidAttachmentId");
			CoreResources.stringIDs.Add(2329210449U, "ErrorBothQueryStringAndRestrictionNonNull");
			CoreResources.stringIDs.Add(1725658743U, "RuleErrorRuleNotFound");
			CoreResources.stringIDs.Add(1277300954U, "ErrorDiscoverySearchesDisabled");
			CoreResources.stringIDs.Add(1147653914U, "ErrorCalendarIsCancelledForTentative");
			CoreResources.stringIDs.Add(1564162812U, "ErrorRecurrenceHasNoOccurrence");
			CoreResources.stringIDs.Add(103255531U, "MessageNonExistentMailboxLegacyDN");
			CoreResources.stringIDs.Add(3137087456U, "ErrorNoDestinationCASDueToKerberosRequirements");
			CoreResources.stringIDs.Add(3395659933U, "ErrorFolderNotFound");
			CoreResources.stringIDs.Add(2923349632U, "ErrorCannotPinGroupIfNotAMember");
			CoreResources.stringIDs.Add(2118011096U, "MessageInsufficientPermissionsToSync");
			CoreResources.stringIDs.Add(2483737250U, "ErrorCalendarIsDelegatedForAccept");
			CoreResources.stringIDs.Add(2958727324U, "ErrorInvalidClientAccessTokenRequest");
			CoreResources.stringIDs.Add(2006869741U, "ErrorCalendarOccurrenceIndexIsOutOfRecurrenceRange");
			CoreResources.stringIDs.Add(124559532U, "MessageMissingUpdateDelegateRequestInformation");
			CoreResources.stringIDs.Add(492857424U, "ErrorCannotOpenFileAttachment");
			CoreResources.stringIDs.Add(234107130U, "ErrorInvalidFolderId");
			CoreResources.stringIDs.Add(2141227684U, "ErrorInvalidPropertyUpdateSentMessage");
			CoreResources.stringIDs.Add(1686445652U, "MessageCalendarInsufficientPermissionsToDefaultCalendarFolder");
			CoreResources.stringIDs.Add(784482022U, "IrmServerMisConfigured");
			CoreResources.stringIDs.Add(357046427U, "RuleErrorRulesOverQuota");
			CoreResources.stringIDs.Add(2890296403U, "ErrorNotAllowedExternalSharingByPolicy");
			CoreResources.stringIDs.Add(3792171687U, "ErrorCannotCreatePostItemInNonMailFolder");
			CoreResources.stringIDs.Add(3080652515U, "ErrorCannotEmptyCalendarOrSearchFolder");
			CoreResources.stringIDs.Add(3504612180U, "ErrorEmptyAggregatedAccountMailboxGuidStoredInSyncRequest");
			CoreResources.stringIDs.Add(3329761676U, "ErrorExpiredSubscription");
			CoreResources.stringIDs.Add(3795993851U, "ErrorODataAccessDisabled");
			CoreResources.stringIDs.Add(3558192788U, "ErrorCannotArchiveItemsInPublicFolders");
			CoreResources.stringIDs.Add(908888675U, "ErrorAssociatedTraversalDisallowedWithQueryString");
			CoreResources.stringIDs.Add(2980490932U, "ErrorCalendarIsOrganizerForDecline");
			CoreResources.stringIDs.Add(1228157268U, "ErrorMissingEmailAddressForManagedFolder");
			CoreResources.stringIDs.Add(1105778474U, "ErrorGetSharingMetadataOnlyForMailbox");
			CoreResources.stringIDs.Add(2292082652U, "MessageActingAsMustHaveRoutingType");
			CoreResources.stringIDs.Add(473053729U, "ErrorInvalidOperationAddItemToMyCalendar");
			CoreResources.stringIDs.Add(1581442160U, "ErrorSyncFolderNotFound");
			CoreResources.stringIDs.Add(471235856U, "ErrorInvalidSharingMessage");
			CoreResources.stringIDs.Add(1735870649U, "descInvalidRequest");
			CoreResources.stringIDs.Add(3640136612U, "ErrorUnsupportedServiceConfigurationType");
			CoreResources.stringIDs.Add(918293667U, "RuleErrorCreateWithRuleId");
			CoreResources.stringIDs.Add(3053756532U, "LoadExtensionCustomPropertiesFailed");
			CoreResources.stringIDs.Add(311942179U, "ErrorUserNotAllowedByPolicy");
			CoreResources.stringIDs.Add(2933471333U, "MessageCouldNotGetWeatherData");
			CoreResources.stringIDs.Add(2335200077U, "MessageMultipleApplicationRolesNotSupported");
			CoreResources.stringIDs.Add(3967923828U, "ErrorPropertyValidationFailure");
			CoreResources.stringIDs.Add(3789879302U, "ErrorInvalidOperationCalendarViewAssociatedItem");
			CoreResources.stringIDs.Add(266941361U, "ErrorInvalidUserPrincipalName");
			CoreResources.stringIDs.Add(124305755U, "ErrorMissedNotificationEvents");
			CoreResources.stringIDs.Add(3635708019U, "ErrorCannotRemoveAggregatedAccountMailbox");
			CoreResources.stringIDs.Add(2102429258U, "MessageCalendarUnableToUpdateMeetingRequest");
			CoreResources.stringIDs.Add(3014743008U, "ErrorInvalidValueForPropertyUserConfigurationPublicFolder");
			CoreResources.stringIDs.Add(3867216855U, "ErrorFolderSave");
			CoreResources.stringIDs.Add(2034497546U, "MessageResolveNamesNotSufficientPermissionsToContactsFolder");
			CoreResources.stringIDs.Add(1790760926U, "descMissingForestConfiguration");
			CoreResources.stringIDs.Add(1103930166U, "ErrorUnsupportedPathForSortGroup");
			CoreResources.stringIDs.Add(3836413508U, "ErrorContainsFilterWrongType");
			CoreResources.stringIDs.Add(1270269734U, "ErrorMailboxScopeNotAllowedWithoutQueryString");
			CoreResources.stringIDs.Add(2084976894U, "ErrorMessageTrackingPermanentError");
			CoreResources.stringIDs.Add(3912965805U, "ErrorCannotDeleteObject");
			CoreResources.stringIDs.Add(1670145257U, "MessageCallerHasNoAdminRoleGranted");
			CoreResources.stringIDs.Add(2692292357U, "ErrorIrmNotSupported");
			CoreResources.stringIDs.Add(707914022U, "ReferenceLinkSharedFrom");
			CoreResources.stringIDs.Add(295620541U, "SentColon");
			CoreResources.stringIDs.Add(1479620947U, "ErrorActingAsUserNotUnique");
			CoreResources.stringIDs.Add(3032287327U, "ErrorSearchQueryHasTooManyKeywords");
			CoreResources.stringIDs.Add(2370747299U, "ErrorFolderPropertyRequestFailed");
			CoreResources.stringIDs.Add(1795652632U, "ErrorMimeContentInvalid");
			CoreResources.stringIDs.Add(3469371317U, "ErrorSharingSynchronizationFailed");
			CoreResources.stringIDs.Add(2109751382U, "ErrorPublicFolderSearchNotSupportedOnMultipleFolders");
			CoreResources.stringIDs.Add(3753602229U, "ErrorNoFolderClassOverride");
			CoreResources.stringIDs.Add(836749070U, "ErrorUnsupportedTypeForConversion");
			CoreResources.stringIDs.Add(1535520491U, "ErrorInvalidItemForOperationDeclineItem");
			CoreResources.stringIDs.Add(3563948173U, "MessageCalendarInsufficientPermissionsToSaveCalendarItem");
			CoreResources.stringIDs.Add(237462827U, "ErrorRightsManagementException");
			CoreResources.stringIDs.Add(2440725179U, "ErrorOperationNotAllowedWithPublicFolderRoot");
			CoreResources.stringIDs.Add(189525348U, "ErrorInvalidIdReturnedByResolveNames");
			CoreResources.stringIDs.Add(2194994953U, "descNoRequestType");
			CoreResources.stringIDs.Add(3371251772U, "ErrorCalendarIsOrganizerForTentative");
			CoreResources.stringIDs.Add(2253496121U, "ErrorInvalidVotingRequest");
			CoreResources.stringIDs.Add(832351692U, "ErrorInvalidProvisionDeviceType");
			CoreResources.stringIDs.Add(2176189925U, "RuleErrorUnsupportedAddress");
			CoreResources.stringIDs.Add(1093593955U, "ErrorInvalidCallStatus");
			CoreResources.stringIDs.Add(2718542415U, "ErrorInvalidSid");
			CoreResources.stringIDs.Add(2580909644U, "ErrorManagedFoldersRootFailure");
			CoreResources.stringIDs.Add(1569114342U, "ErrorProxiedSubscriptionCallFailure");
			CoreResources.stringIDs.Add(1214042036U, "ErrorOccurrenceTimeSpanTooBig");
			CoreResources.stringIDs.Add(1110621977U, "MessageCalendarInsufficientPermissionsToMoveCalendarItem");
			CoreResources.stringIDs.Add(2943900075U, "ErrorNewEventStreamConnectionOpened");
			CoreResources.stringIDs.Add(2054881972U, "ErrorArchiveMailboxNotEnabled");
			CoreResources.stringIDs.Add(4180336284U, "ErrorCalendarCannotUseIdForOccurrenceId");
			CoreResources.stringIDs.Add(3579904699U, "ErrorAccessDenied");
			CoreResources.stringIDs.Add(1721078306U, "ErrorAttachmentSizeLimitExceeded");
			CoreResources.stringIDs.Add(1912743644U, "ErrorPropertyUpdate");
			CoreResources.stringIDs.Add(842243550U, "RuleErrorInvalidValue");
			CoreResources.stringIDs.Add(2756368512U, "ErrorInvalidManagedFolderQuota");
			CoreResources.stringIDs.Add(304669716U, "ErrorCreateDistinguishedFolder");
			CoreResources.stringIDs.Add(3684919469U, "ShowDetails");
			CoreResources.stringIDs.Add(3465339554U, "ToColon");
			CoreResources.stringIDs.Add(2832845860U, "ErrorCrossMailboxMoveCopy");
			CoreResources.stringIDs.Add(4273162695U, "FlagForFollowUp");
			CoreResources.stringIDs.Add(4264440001U, "ErrorGetStreamingEventsProxy");
			CoreResources.stringIDs.Add(2183377470U, "ErrorCannotSetCalendarPermissionOnNonCalendarFolder");
			CoreResources.stringIDs.Add(1686056205U, "SaveExtensionCustomPropertiesFailed");
			CoreResources.stringIDs.Add(3500594897U, "ErrorConnectionFailed");
			CoreResources.stringIDs.Add(2976928908U, "ErrorCannotUseLocalAccount");
			CoreResources.stringIDs.Add(810356415U, "descInvalidOofParameter");
			CoreResources.stringIDs.Add(2538042329U, "ErrorTimeRangeIsTooLarge");
			CoreResources.stringIDs.Add(2684918840U, "ErrorAffectedTaskOccurrencesRequired");
			CoreResources.stringIDs.Add(513058151U, "ErrorCannotGetAggregatedAccount");
			CoreResources.stringIDs.Add(917420962U, "AADIdentityCreationFailed");
			CoreResources.stringIDs.Add(217482359U, "ErrorDuplicateInputFolderNames");
			CoreResources.stringIDs.Add(4088802584U, "MessageNonExistentMailboxSmtpAddress");
			CoreResources.stringIDs.Add(1439726170U, "ErrorIncorrectUpdatePropertyCount");
			CoreResources.stringIDs.Add(2485795088U, "ErrorInvalidSerializedAccessToken");
			CoreResources.stringIDs.Add(4103342537U, "ErrorInvalidRoutingType");
			CoreResources.stringIDs.Add(3383701276U, "ErrorSendMeetingInvitationsRequired");
			CoreResources.stringIDs.Add(1500443603U, "ErrorInvalidIdNotAnItemAttachmentId");
			CoreResources.stringIDs.Add(1702622873U, "RightsManagementInternalLicensingDisabled");
			CoreResources.stringIDs.Add(247950989U, "MessageCannotUseItemAsRecipient");
			CoreResources.stringIDs.Add(815859081U, "ErrorItemSaveUserConfigurationExists");
			CoreResources.stringIDs.Add(687843280U, "MessageInvalidMailboxMailboxType");
			CoreResources.stringIDs.Add(2997278338U, "ErrorCalendarIsCancelledForDecline");
			CoreResources.stringIDs.Add(3510335548U, "ErrorClientIntentInvalidStateDefinition");
			CoreResources.stringIDs.Add(4105318492U, "ErrorInvalidRetentionTagInvisible");
			CoreResources.stringIDs.Add(1583125739U, "ErrorItemSavePropertyError");
			CoreResources.stringIDs.Add(3308334241U, "GetScopedTokenFailedWithInvalidScope");
			CoreResources.stringIDs.Add(1518836305U, "ErrorInvalidItemForOperationRemoveItem");
			CoreResources.stringIDs.Add(1858753018U, "RuleErrorMessageClassificationNotFound");
			CoreResources.stringIDs.Add(30276810U, "MessageUnableToLoadRBACSettings");
			CoreResources.stringIDs.Add(707594371U, "ErrorQueryLanguageNotValid");
			CoreResources.stringIDs.Add(4158023012U, "Purple");
			CoreResources.stringIDs.Add(133083912U, "InvalidMaxItemsToReturn");
			CoreResources.stringIDs.Add(4109493280U, "PostedTo");
			CoreResources.stringIDs.Add(607223707U, "ExchangeServiceResponseErrorNoResponse");
			CoreResources.stringIDs.Add(1569475421U, "ErrorPublicFolderOperationFailed");
			CoreResources.stringIDs.Add(444799972U, "ErrorBatchProcessingStopped");
			CoreResources.stringIDs.Add(1722439826U, "ErrorUnifiedMessagingServerNotFound");
			CoreResources.stringIDs.Add(4077186341U, "InstantSearchNullFolderId");
			CoreResources.stringIDs.Add(4210036349U, "ErrorWeatherServiceDisabled");
			CoreResources.stringIDs.Add(2710201884U, "descNotEnoughPrivileges");
			CoreResources.stringIDs.Add(3284680126U, "CalendarInvalidFirstDayOfWeek");
			CoreResources.stringIDs.Add(3021629811U, "Red");
			CoreResources.stringIDs.Add(3133201118U, "ErrorInvalidExternalSharingSubscriber");
			CoreResources.stringIDs.Add(2770848984U, "ErrorCannotUseFolderIdForItemId");
			CoreResources.stringIDs.Add(3336001063U, "ErrorExchange14Required");
			CoreResources.stringIDs.Add(3032417457U, "ErrorProxyCallFailed");
			CoreResources.stringIDs.Add(1788731128U, "ErrorOrganizationNotFederated");
			CoreResources.stringIDs.Add(2395476974U, "Blue");
			CoreResources.stringIDs.Add(1411102909U, "ErrorCannotDeleteSubfoldersOfMsgRootFolder");
			CoreResources.stringIDs.Add(1737721488U, "ErrorUpdatePropertyMismatch");
			CoreResources.stringIDs.Add(1160056179U, "ErrorIllegalCrossServerConnection");
			CoreResources.stringIDs.Add(822125946U, "ErrorImListMigration");
			CoreResources.stringIDs.Add(610335429U, "ErrorResponseSchemaValidation");
			CoreResources.stringIDs.Add(3829975538U, "ServerNotInSite");
			CoreResources.stringIDs.Add(3220333293U, "ErrorCannotAddAggregatedAccountToList");
			CoreResources.stringIDs.Add(1666265192U, "WhereColon");
			CoreResources.stringIDs.Add(708456719U, "ErrorInvalidApprovalRequest");
			CoreResources.stringIDs.Add(3010537222U, "ErrorIncorrectEncodedIdType");
			CoreResources.stringIDs.Add(4106572054U, "ErrorGetRemoteArchiveItemFailed");
			CoreResources.stringIDs.Add(827411151U, "ErrorInvalidImGroupId");
			CoreResources.stringIDs.Add(1835609958U, "ErrorInvalidRequestUnknownMethodDebug");
			CoreResources.stringIDs.Add(2675632227U, "ErrorBothViewFilterAndRestrictionNonNull");
			CoreResources.stringIDs.Add(2423603834U, "ErrorCannotUseItemIdForFolderId");
			CoreResources.stringIDs.Add(1262244671U, "ErrorCannotDisableMandatoryExtension");
			CoreResources.stringIDs.Add(1655535493U, "ErrorInvalidSyncStateData");
			CoreResources.stringIDs.Add(178029729U, "ErrorSubmissionQuotaExceeded");
			CoreResources.stringIDs.Add(2130715693U, "ErrorMessageDispositionRequired");
			CoreResources.stringIDs.Add(1395824819U, "ErrorSearchScopeCannotHavePublicFolders");
			CoreResources.stringIDs.Add(3763931121U, "ErrorRemoveDelegatesFailed");
			CoreResources.stringIDs.Add(2467205866U, "ErrorInvalidPagingMaxRows");
			CoreResources.stringIDs.Add(3016713339U, "RuleErrorMissingParameter");
			CoreResources.stringIDs.Add(1085788054U, "ErrorLocationServicesInvalidQuery");
			CoreResources.stringIDs.Add(1319006043U, "MessageOccurrenceNotFound");
			CoreResources.stringIDs.Add(1658627017U, "ErrorSearchFolderNotInitialized");
			CoreResources.stringIDs.Add(1064030279U, "FolderScopeNotSpecified");
			CoreResources.stringIDs.Add(3880436217U, "ErrorInvalidSubfilterType");
			CoreResources.stringIDs.Add(4289255106U, "ErrorDuplicateUserIdsSpecified");
			CoreResources.stringIDs.Add(1422139444U, "ErrorDelegateMustBeCalendarEditorToGetMeetingMessages");
			CoreResources.stringIDs.Add(3041888687U, "ErrorMismatchFolderId");
			CoreResources.stringIDs.Add(444235555U, "ErrorInvalidPropertyDelete");
			CoreResources.stringIDs.Add(3538999938U, "MessageActingAsMustHaveEmailAddress");
			CoreResources.stringIDs.Add(4064247940U, "ErrorCalendarIsCancelledForRemove");
			CoreResources.stringIDs.Add(884514945U, "ErrorCannotResolveODataUrl");
			CoreResources.stringIDs.Add(4006585486U, "ErrorCalendarEndDateIsEarlierThanStartDate");
			CoreResources.stringIDs.Add(3035123300U, "ErrorInvalidPercentCompleteValue");
			CoreResources.stringIDs.Add(4164112684U, "ErrorNoApplicableProxyCASServersAvailable");
			CoreResources.stringIDs.Add(106943791U, "IrmProtectedVoicemailFeatureDisabled");
			CoreResources.stringIDs.Add(1397740097U, "IrmExternalLicensingDisabled");
			CoreResources.stringIDs.Add(2132997082U, "ErrorExchangeConfigurationException");
			CoreResources.stringIDs.Add(2069536979U, "ErrorMailboxMoveInProgress");
			CoreResources.stringIDs.Add(2643780243U, "ErrorInvalidValueForPropertyXmlData");
			CoreResources.stringIDs.Add(668221183U, "RuleErrorDuplicatedPriority");
			CoreResources.stringIDs.Add(2622305962U, "ItemNotExistInPurgesFolder");
			CoreResources.stringIDs.Add(3313362701U, "MessageMissingUserRolesForMailboxSearchRoleTypeApp");
			CoreResources.stringIDs.Add(4279571010U, "ErrorInvalidNameForNameResolution");
			CoreResources.stringIDs.Add(3776856310U, "ErrorInvalidRecipientSubfilterOrder");
			CoreResources.stringIDs.Add(3146925354U, "ErrorMailboxContainerGuidMismatch");
			CoreResources.stringIDs.Add(1935400134U, "ErrorInvalidId");
			CoreResources.stringIDs.Add(195228379U, "ErrorNonPrimarySmtpAddress");
			CoreResources.stringIDs.Add(2280668014U, "ErrorSharedFolderSearchNotSupportedOnMultipleFolders");
			CoreResources.stringIDs.Add(1532804559U, "ErrorCalendarInvalidRecurrence");
			CoreResources.stringIDs.Add(4104292452U, "ErrorInvalidOperationSaveReplyForwardToPublicFolder");
			CoreResources.stringIDs.Add(4072847078U, "ErrorInvalidOrderbyThenby");
			CoreResources.stringIDs.Add(531605055U, "ErrorInvalidRetentionTagTypeMismatch");
			CoreResources.stringIDs.Add(608291240U, "ErrorRequiredPropertyMissing");
			CoreResources.stringIDs.Add(54420019U, "ErrorActiveDirectoryPermanentError");
			CoreResources.stringIDs.Add(360598592U, "IrmRmsError");
			CoreResources.stringIDs.Add(3654096821U, "ErrorNoPropertyUpdatesOrAttachmentsSpecified");
			CoreResources.stringIDs.Add(2137439660U, "ConversationActionNeedFlagForFlagAction");
			CoreResources.stringIDs.Add(178421269U, "ErrorAttachmentNestLevelLimitExceeded");
			CoreResources.stringIDs.Add(938097637U, "ErrorInvalidSmtpAddress");
		}

		public static LocalizedString ErrorCannotSaveSentItemInArchiveFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotSaveSentItemInArchiveFolder", "ExE485E1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingUserIdInformation
		{
			get
			{
				return new LocalizedString("ErrorMissingUserIdInformation", "ExF85CAB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchConfigurationNotFound
		{
			get
			{
				return new LocalizedString("ErrorSearchConfigurationNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotCreateContactInNonContactFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotCreateContactInNonContactFolder", "Ex8AFB0E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmFeatureDisabled
		{
			get
			{
				return new LocalizedString("IrmFeatureDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EwsProxyResponseTooBig
		{
			get
			{
				return new LocalizedString("EwsProxyResponseTooBig", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesUnableToDeleteFavoriteEntry
		{
			get
			{
				return new LocalizedString("UpdateFavoritesUnableToDeleteFavoriteEntry", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUpdateDelegatesFailed
		{
			get
			{
				return new LocalizedString("ErrorUpdateDelegatesFailed", "Ex76491C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoMailboxSpecifiedForSearchOperation
		{
			get
			{
				return new LocalizedString("ErrorNoMailboxSpecifiedForSearchOperation", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotApplyHoldOperationOnDG
		{
			get
			{
				return new LocalizedString("ErrorCannotApplyHoldOperationOnDG", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExchangeImpersonationHeaderData
		{
			get
			{
				return new LocalizedString("ErrorInvalidExchangeImpersonationHeaderData", "ExDEBB9C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExOrganizerCannotCallUpdateCalendarItem
		{
			get
			{
				return new LocalizedString("ExOrganizerCannotCallUpdateCalendarItem", "ExE5CA94", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmViewRightNotGranted
		{
			get
			{
				return new LocalizedString("IrmViewRightNotGranted", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateNonDraftItemInDumpsterNotAllowed
		{
			get
			{
				return new LocalizedString("UpdateNonDraftItemInDumpsterNotAllowed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIPGatewayNotFound
		{
			get
			{
				return new LocalizedString("ErrorIPGatewayNotFound", "Ex829B0D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyForOperation
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertyForOperation", "Ex2C4929", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNameResolutionNoResults
		{
			get
			{
				return new LocalizedString("ErrorNameResolutionNoResults", "Ex79F5CE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoConnectionSettingsAvailableForAggregatedAccount(string email)
		{
			return new LocalizedString("ErrorNoConnectionSettingsAvailableForAggregatedAccount", "", false, false, CoreResources.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString ErrorInvalidItemForOperationCreateItemAttachment
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationCreateItemAttachment", "ExDB14A3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Loading
		{
			get
			{
				return new LocalizedString("Loading", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorItemSave
		{
			get
			{
				return new LocalizedString("ErrorItemSave", "ExDDC5BD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubjectColon
		{
			get
			{
				return new LocalizedString("SubjectColon", "Ex506FE5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationExpandDL
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationExpandDL", "Ex0DD286", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageApplicationHasNoUserApplicationRoleAssigned
		{
			get
			{
				return new LocalizedString("MessageApplicationHasNoUserApplicationRoleAssigned", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsCancelledMessageSent
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsCancelledMessageSent", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidUserInfo
		{
			get
			{
				return new LocalizedString("ErrorInvalidUserInfo", "Ex5020C4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarViewRangeTooBig
		{
			get
			{
				return new LocalizedString("ErrorCalendarViewRangeTooBig", "Ex9AD1A2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsOrganizerForRemove
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsOrganizerForRemove", "ExAC0DE1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRecipientSubfilterComparison
		{
			get
			{
				return new LocalizedString("ErrorInvalidRecipientSubfilterComparison", "ExF399B3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPassingActingAsForUMConfig
		{
			get
			{
				return new LocalizedString("ErrorPassingActingAsForUMConfig", "Ex8BE355", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserWithoutFederatedProxyAddress
		{
			get
			{
				return new LocalizedString("ErrorUserWithoutFederatedProxyAddress", "ExC6130A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSendItemSaveSettings
		{
			get
			{
				return new LocalizedString("ErrorInvalidSendItemSaveSettings", "Ex196391", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWrongServerVersion
		{
			get
			{
				return new LocalizedString("ErrorWrongServerVersion", "ExF8DB35", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAssociatedTraversalDisallowedWithViewFilter
		{
			get
			{
				return new LocalizedString("ErrorAssociatedTraversalDisallowedWithViewFilter", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxHoldIsNotPermitted
		{
			get
			{
				return new LocalizedString("ErrorMailboxHoldIsNotPermitted", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateSOAPHeader
		{
			get
			{
				return new LocalizedString("ErrorDuplicateSOAPHeader", "Ex69584C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForPropertyUserConfigurationName
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyUserConfigurationName", "ExD121B8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIncorrectSchemaVersion
		{
			get
			{
				return new LocalizedString("ErrorIncorrectSchemaVersion", "Ex3734AB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImpersonationRequiredForPush
		{
			get
			{
				return new LocalizedString("ErrorImpersonationRequiredForPush", "Ex2184D5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnifiedMessagingPromptNotFound
		{
			get
			{
				return new LocalizedString("ErrorUnifiedMessagingPromptNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarMeetingRequestIsOutOfDate
		{
			get
			{
				return new LocalizedString("ErrorCalendarMeetingRequestIsOutOfDate", "Ex602910", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageExtensionNotAllowedToCreateFAI
		{
			get
			{
				return new LocalizedString("MessageExtensionNotAllowedToCreateFAI", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderCorrupt
		{
			get
			{
				return new LocalizedString("ErrorFolderCorrupt", "ExBB20FE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedFolderNotFound
		{
			get
			{
				return new LocalizedString("ErrorManagedFolderNotFound", "Ex481826", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageManagementRoleHeaderCannotUseWithOtherHeaders
		{
			get
			{
				return new LocalizedString("MessageManagementRoleHeaderCannotUseWithOtherHeaders", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorQueryFilterTooLong
		{
			get
			{
				return new LocalizedString("ErrorQueryFilterTooLong", "Ex9140DE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeProposal(string innerError)
		{
			return new LocalizedString("ErrorTimeProposal", "", false, false, CoreResources.ResourceManager, new object[]
			{
				innerError
			});
		}

		public static LocalizedString MessageApplicationUnableActAsUser
		{
			get
			{
				return new LocalizedString("MessageApplicationUnableActAsUser", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidContactEmailIndex
		{
			get
			{
				return new LocalizedString("ErrorInvalidContactEmailIndex", "ExCBD1E3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageMalformedSoapHeader
		{
			get
			{
				return new LocalizedString("MessageMalformedSoapHeader", "Ex68BB33", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationItemQueryFailed
		{
			get
			{
				return new LocalizedString("ConversationItemQueryFailed", "Ex1B61EC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorADOperation
		{
			get
			{
				return new LocalizedString("ErrorADOperation", "ExBF6F3C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsOrganizerForAccept
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsOrganizerForAccept", "Ex4F8BBD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmRmsErrorLocation(string uri)
		{
			return new LocalizedString("IrmRmsErrorLocation", "", false, false, CoreResources.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString ErrorCannotDeleteTaskOccurrence
		{
			get
			{
				return new LocalizedString("ErrorCannotDeleteTaskOccurrence", "Ex02031F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyContactsException
		{
			get
			{
				return new LocalizedString("ErrorTooManyContactsException", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReadEventsFailed
		{
			get
			{
				return new LocalizedString("ErrorReadEventsFailed", "ExD3DF4E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidEIParameter
		{
			get
			{
				return new LocalizedString("descInvalidEIParameter", "Ex3FE4B5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateLegacyDistinguishedName
		{
			get
			{
				return new LocalizedString("ErrorDuplicateLegacyDistinguishedName", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageActingAsIsNotAValidEmailAddress
		{
			get
			{
				return new LocalizedString("MessageActingAsIsNotAValidEmailAddress", "Ex31868D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidServerVersionForJsonRequest
		{
			get
			{
				return new LocalizedString("MessageInvalidServerVersionForJsonRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarCannotMoveOrCopyOccurrence
		{
			get
			{
				return new LocalizedString("ErrorCalendarCannotMoveOrCopyOccurrence", "ExECABF5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPeopleConnectionNotFound
		{
			get
			{
				return new LocalizedString("ErrorPeopleConnectionNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarMeetingIsOutOfDateResponseNotProcessedMessageSent
		{
			get
			{
				return new LocalizedString("ErrorCalendarMeetingIsOutOfDateResponseNotProcessedMessageSent", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExcludesRestriction
		{
			get
			{
				return new LocalizedString("ErrorInvalidExcludesRestriction", "ExFBE947", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMoreThanOneAccessModeSpecified
		{
			get
			{
				return new LocalizedString("ErrorMoreThanOneAccessModeSpecified", "Ex5B156F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCreateSubfolderAccessDenied
		{
			get
			{
				return new LocalizedString("ErrorCreateSubfolderAccessDenied", "Ex26CEA3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidMailboxIdFormat
		{
			get
			{
				return new LocalizedString("ErrorInvalidMailboxIdFormat", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsCancelledForAccept
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsCancelledForAccept", "ExE945D4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageApplicationRoleShouldPresentWhenUserRolePresent
		{
			get
			{
				return new LocalizedString("MessageApplicationRoleShouldPresentWhenUserRolePresent", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidUMSubscriberDataTimeoutValue
		{
			get
			{
				return new LocalizedString("ErrorInvalidUMSubscriberDataTimeoutValue", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchTimeoutExpired
		{
			get
			{
				return new LocalizedString("ErrorSearchTimeoutExpired", "ExEEADCE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorInvalidSizeRange(int minimumSize, int maximumSize)
		{
			return new LocalizedString("RuleErrorInvalidSizeRange", "Ex4FF3F2", false, true, CoreResources.ResourceManager, new object[]
			{
				minimumSize,
				maximumSize
			});
		}

		public static LocalizedString descLocalServerConfigurationRetrievalFailed
		{
			get
			{
				return new LocalizedString("descLocalServerConfigurationRetrievalFailed", "Ex2BE2B5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidContactEmailAddress
		{
			get
			{
				return new LocalizedString("ErrorInvalidContactEmailAddress", "Ex72374D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForPropertyStringArrayDictionaryKey
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyStringArrayDictionaryKey", "Ex2F0895", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorChangeKeyRequiredForWriteOperations
		{
			get
			{
				return new LocalizedString("ErrorChangeKeyRequiredForWriteOperations", "ExD38F2F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingEmailAddress
		{
			get
			{
				return new LocalizedString("ErrorMissingEmailAddress", "Ex0AACA0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFullSyncRequiredException
		{
			get
			{
				return new LocalizedString("ErrorFullSyncRequiredException", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorADSessionFilter
		{
			get
			{
				return new LocalizedString("ErrorADSessionFilter", "Ex82EE97", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDistinguishedUserNotSupported
		{
			get
			{
				return new LocalizedString("ErrorDistinguishedUserNotSupported", "Ex964408", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCrossForestCallerNeedsADObject
		{
			get
			{
				return new LocalizedString("ErrorCrossForestCallerNeedsADObject", "Ex126808", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSendMeetingInvitationsOrCancellationsRequired
		{
			get
			{
				return new LocalizedString("ErrorSendMeetingInvitationsOrCancellationsRequired", "ExFDD460", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorDuplicatedOperationOnTheSameRule
		{
			get
			{
				return new LocalizedString("RuleErrorDuplicatedOperationOnTheSameRule", "Ex6F0889", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDeletePersonaOnInvalidFolder
		{
			get
			{
				return new LocalizedString("ErrorDeletePersonaOnInvalidFolder", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotAddAggregatedAccountMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotAddAggregatedAccountMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExceededConnectionCount
		{
			get
			{
				return new LocalizedString("ErrorExceededConnectionCount", "Ex8D752F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderSavePropertyError
		{
			get
			{
				return new LocalizedString("ErrorFolderSavePropertyError", "Ex5E13B1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotUsePersonalContactsAsRecipientsOrAttendees
		{
			get
			{
				return new LocalizedString("ErrorCannotUsePersonalContactsAsRecipientsOrAttendees", "Ex136B8C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForForward
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForForward", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorChangeKeyRequired
		{
			get
			{
				return new LocalizedString("ErrorChangeKeyRequired", "Ex5996C7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotAcceptable
		{
			get
			{
				return new LocalizedString("ErrorNotAcceptable", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessageTrackingNoSuchDomain
		{
			get
			{
				return new LocalizedString("ErrorMessageTrackingNoSuchDomain", "Ex8E0EFF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTraversalNotAllowedWithoutQueryString
		{
			get
			{
				return new LocalizedString("ErrorTraversalNotAllowedWithoutQueryString", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrganizationAccessBlocked
		{
			get
			{
				return new LocalizedString("ErrorOrganizationAccessBlocked", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidNumberOfMailboxSearch
		{
			get
			{
				return new LocalizedString("ErrorInvalidNumberOfMailboxSearch", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCreateManagedFolderPartialCompletion
		{
			get
			{
				return new LocalizedString("ErrorCreateManagedFolderPartialCompletion", "ExBAD6DC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesUnableToRenameFavorite
		{
			get
			{
				return new LocalizedString("UpdateFavoritesUnableToRenameFavorite", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorActiveDirectoryTransientError
		{
			get
			{
				return new LocalizedString("ErrorActiveDirectoryTransientError", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSubscriptionRequestAllFoldersWithFolderIds
		{
			get
			{
				return new LocalizedString("ErrorInvalidSubscriptionRequestAllFoldersWithFolderIds", "Ex96F104", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationSendMeetingInvitationCancellationForPublicFolderItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationSendMeetingInvitationCancellationForPublicFolderItem", "Ex93893D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIrresolvableConflict
		{
			get
			{
				return new LocalizedString("ErrorIrresolvableConflict", "Ex926A33", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForReplyAll
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForReplyAll", "ExD5C17A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPhoneNumberNotDialable
		{
			get
			{
				return new LocalizedString("ErrorPhoneNumberNotDialable", "ExFCBEF5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidInternetHeaderChildNodes
		{
			get
			{
				return new LocalizedString("ErrorInvalidInternetHeaderChildNodes", "Ex032D34", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExpressionTypeForSubFilter
		{
			get
			{
				return new LocalizedString("ErrorInvalidExpressionTypeForSubFilter", "Ex259E33", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageResolveNamesNotSufficientPermissionsToPrivateDLMember
		{
			get
			{
				return new LocalizedString("MessageResolveNamesNotSufficientPermissionsToPrivateDLMember", "ExFBCE9A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetNonCalendarPermissionOnCalendarFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotSetNonCalendarPermissionOnCalendarFolder", "Ex54F76F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorParentFolderIdRequired
		{
			get
			{
				return new LocalizedString("ErrorParentFolderIdRequired", "ExA7F8D6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEventNotFound
		{
			get
			{
				return new LocalizedString("ErrorEventNotFound", "Ex901B01", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorVoiceMailNotImplemented
		{
			get
			{
				return new LocalizedString("ErrorVoiceMailNotImplemented", "Ex3EB0D9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeServiceResponseErrorNoResponseForType(string responseType)
		{
			return new LocalizedString("ExchangeServiceResponseErrorNoResponseForType", "", false, false, CoreResources.ResourceManager, new object[]
			{
				responseType
			});
		}

		public static LocalizedString ErrorAccountNotSupportedForAggregation(string email)
		{
			return new LocalizedString("ErrorAccountNotSupportedForAggregation", "", false, false, CoreResources.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString ErrorDeleteDistinguishedFolder
		{
			get
			{
				return new LocalizedString("ErrorDeleteDistinguishedFolder", "ExCA65B2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoPermissionToSearchOrHoldMailbox
		{
			get
			{
				return new LocalizedString("ErrorNoPermissionToSearchOrHoldMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExchangeApplicationNotEnabled
		{
			get
			{
				return new LocalizedString("ErrorExchangeApplicationNotEnabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorResolveNamesInvalidFolderType
		{
			get
			{
				return new LocalizedString("ErrorResolveNamesInvalidFolderType", "ExC8376A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExceededFindCountLimit
		{
			get
			{
				return new LocalizedString("ErrorExceededFindCountLimit", "ExC12DC8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageExtensionAccessActAsMailboxOnly
		{
			get
			{
				return new LocalizedString("MessageExtensionAccessActAsMailboxOnly", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordChangeRequired
		{
			get
			{
				return new LocalizedString("ErrorPasswordChangeRequired", "ExDD78FA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidManagedFolderProperty
		{
			get
			{
				return new LocalizedString("ErrorInvalidManagedFolderProperty", "ExA0963B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdMalformedEwsLegacyIdFormat
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdMalformedEwsLegacyIdFormat", "ExC7A6F0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSchemaViolation
		{
			get
			{
				return new LocalizedString("ErrorSchemaViolation", "ExA59402", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NewGroupMailboxFailed(string name, string error)
		{
			return new LocalizedString("NewGroupMailboxFailed", "", false, false, CoreResources.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString MessageInvalidMailboxContactAddressNotFound
		{
			get
			{
				return new LocalizedString("MessageInvalidMailboxContactAddressNotFound", "Ex8C5ACD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIndexedPagingParameters
		{
			get
			{
				return new LocalizedString("ErrorInvalidIndexedPagingParameters", "ExF15AA5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedPathForQuery
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedPathForQuery", "Ex3680D2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationDelegationAssociatedItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationDelegationAssociatedItem", "Ex2D7706", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRemoteUserMailboxMustSpecifyExplicitLocalMailbox
		{
			get
			{
				return new LocalizedString("ErrorRemoteUserMailboxMustSpecifyExplicitLocalMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoDestinationCASDueToVersionMismatch
		{
			get
			{
				return new LocalizedString("ErrorNoDestinationCASDueToVersionMismatch", "ExAFEEE0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForPropertyBinaryData
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyBinaryData", "Ex2E726C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotDelegate
		{
			get
			{
				return new LocalizedString("ErrorNotDelegate", "Ex850DEA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarInvalidDayForTimeChangePattern
		{
			get
			{
				return new LocalizedString("ErrorCalendarInvalidDayForTimeChangePattern", "ExB1A176", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidProperty(string property)
		{
			return new LocalizedString("ErrorInvalidProperty", "", false, false, CoreResources.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ErrorInvalidPullSubscriptionId
		{
			get
			{
				return new LocalizedString("ErrorInvalidPullSubscriptionId", "ExCBB4CC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotCopyPublicFolderRoot
		{
			get
			{
				return new LocalizedString("ErrorCannotCopyPublicFolderRoot", "Ex8C2BFD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageOperationRequiresUserContext
		{
			get
			{
				return new LocalizedString("MessageOperationRequiresUserContext", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPromptPublishingOperationFailed
		{
			get
			{
				return new LocalizedString("ErrorPromptPublishingOperationFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidFractionalPagingParameters
		{
			get
			{
				return new LocalizedString("ErrorInvalidFractionalPagingParameters", "Ex1CE32C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPublicFolderMailboxDiscoveryFailed
		{
			get
			{
				return new LocalizedString("ErrorPublicFolderMailboxDiscoveryFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnableToRemoveImContactFromGroup
		{
			get
			{
				return new LocalizedString("ErrorUnableToRemoveImContactFromGroup", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSendMeetingCancellationsRequired
		{
			get
			{
				return new LocalizedString("ErrorSendMeetingCancellationsRequired", "ExF3C1DA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRecipientsArrayMustNotBeEmpty
		{
			get
			{
				return new LocalizedString("MessageRecipientsArrayMustNotBeEmpty", "Ex79FA63", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationTentative
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationTentative", "ExC6A2C3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidReferenceItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidReferenceItem", "ExC62CDC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmReachNotConfigured
		{
			get
			{
				return new LocalizedString("IrmReachNotConfigured", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMimeContentInvalidBase64String
		{
			get
			{
				return new LocalizedString("ErrorMimeContentInvalidBase64String", "Ex822BE8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSentTaskRequestUpdate
		{
			get
			{
				return new LocalizedString("ErrorSentTaskRequestUpdate", "ExEBF2A6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFoundSyncRequestForNonAggregatedAccount
		{
			get
			{
				return new LocalizedString("ErrorFoundSyncRequestForNonAggregatedAccount", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessagePropertyIsDeprecatedForThisVersion
		{
			get
			{
				return new LocalizedString("MessagePropertyIsDeprecatedForThisVersion", "Ex2AFD57", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidUnifiedViewParameter(string parameterName)
		{
			return new LocalizedString("ErrorInvalidUnifiedViewParameter", "", false, false, CoreResources.ResourceManager, new object[]
			{
				parameterName
			});
		}

		public static LocalizedString ErrorInvalidOperationContactsViewAssociatedItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationContactsViewAssociatedItem", "Ex3A3CE8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServerBusy
		{
			get
			{
				return new LocalizedString("ErrorServerBusy", "Ex631C4E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionNeedRetentionPolicyTypeForSetRetentionPolicy
		{
			get
			{
				return new LocalizedString("ConversationActionNeedRetentionPolicyTypeForSetRetentionPolicy", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotDeletePublicFolderRoot
		{
			get
			{
				return new LocalizedString("ErrorCannotDeletePublicFolderRoot", "Ex1E59DE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImGroupDisplayNameAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorImGroupDisplayNameAlreadyExists", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoServer
		{
			get
			{
				return new LocalizedString("NoServer", "ExE10847", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidImDistributionGroupSmtpAddress
		{
			get
			{
				return new LocalizedString("ErrorInvalidImDistributionGroupSmtpAddress", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubscriptionDelegateAccessNotSupported
		{
			get
			{
				return new LocalizedString("ErrorSubscriptionDelegateAccessNotSupported", "ExE9CBB8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorItemIsNotTemplate
		{
			get
			{
				return new LocalizedString("RuleErrorItemIsNotTemplate", "ExA942E0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetPermissionUnknownEntries
		{
			get
			{
				return new LocalizedString("ErrorCannotSetPermissionUnknownEntries", "Ex0F0A47", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageIdOrTokenTypeNotFound
		{
			get
			{
				return new LocalizedString("MessageIdOrTokenTypeNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRightsManagementTemplateNotFound(string id)
		{
			return new LocalizedString("ErrorRightsManagementTemplateNotFound", "", false, false, CoreResources.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ErrorLocationServicesDisabled
		{
			get
			{
				return new LocalizedString("ErrorLocationServicesDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageNotSupportedApplicationRole
		{
			get
			{
				return new LocalizedString("MessageNotSupportedApplicationRole", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPublicFolderSyncException
		{
			get
			{
				return new LocalizedString("ErrorPublicFolderSyncException", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsDelegatedForDecline
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsDelegatedForDecline", "Ex51730C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedODataRequest
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedODataRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDeepTraversalsNotAllowedOnPublicFolders
		{
			get
			{
				return new LocalizedString("ErrorDeepTraversalsNotAllowedOnPublicFolders", "ExF3A6EB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCouldNotFindWeatherLocationsForSearchString
		{
			get
			{
				return new LocalizedString("MessageCouldNotFindWeatherLocationsForSearchString", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyForSortBy
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertyForSortBy", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarUnableToGetAssociatedCalendarItem
		{
			get
			{
				return new LocalizedString("MessageCalendarUnableToGetAssociatedCalendarItem", "ExD82E7F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSortByPropertyIsNotFoundOrNotSupported
		{
			get
			{
				return new LocalizedString("ErrorSortByPropertyIsNotFoundOrNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotSupportedSharingMessage
		{
			get
			{
				return new LocalizedString("ErrorNotSupportedSharingMessage", "ExB5C5A5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingInformationReferenceItemId
		{
			get
			{
				return new LocalizedString("ErrorMissingInformationReferenceItemId", "Ex927959", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSIPUri
		{
			get
			{
				return new LocalizedString("ErrorInvalidSIPUri", "Ex325805", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidCompleteDateOutOfRange
		{
			get
			{
				return new LocalizedString("ErrorInvalidCompleteDateOutOfRange", "Ex48AF3C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnifiedMessagingDialPlanNotFound
		{
			get
			{
				return new LocalizedString("ErrorUnifiedMessagingDialPlanNotFound", "ExCE21DC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRecipientMustHaveRoutingType
		{
			get
			{
				return new LocalizedString("MessageRecipientMustHaveRoutingType", "Ex3E2F67", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageResolveNamesNotSufficientPermissionsToPrivateDL
		{
			get
			{
				return new LocalizedString("MessageResolveNamesNotSufficientPermissionsToPrivateDL", "Ex30B3A4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyMailboxQueryObjects(int entryCount, int maxAllowedCount)
		{
			return new LocalizedString("TooManyMailboxQueryObjects", "", false, false, CoreResources.ResourceManager, new object[]
			{
				entryCount,
				maxAllowedCount
			});
		}

		public static LocalizedString MessageMissingUserRolesForOrganizationConfigurationRoleTypeApp
		{
			get
			{
				return new LocalizedString("MessageMissingUserRolesForOrganizationConfigurationRoleTypeApp", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidUserSid
		{
			get
			{
				return new LocalizedString("ErrorInvalidUserSid", "ExFA6FD9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRecipientSubfilter
		{
			get
			{
				return new LocalizedString("ErrorInvalidRecipientSubfilter", "Ex6BCAB8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSuffixSearchNotAllowed
		{
			get
			{
				return new LocalizedString("ErrorSuffixSearchNotAllowed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnifiedMessagingReportDataNotFound
		{
			get
			{
				return new LocalizedString("ErrorUnifiedMessagingReportDataNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesFolderAlreadyInFavorites
		{
			get
			{
				return new LocalizedString("UpdateFavoritesFolderAlreadyInFavorites", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageManagementRoleHeaderNotSupportedForOfficeExtension
		{
			get
			{
				return new LocalizedString("MessageManagementRoleHeaderNotSupportedForOfficeExtension", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OneDriveProAttachmentDataProviderName
		{
			get
			{
				return new LocalizedString("OneDriveProAttachmentDataProviderName", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarInvalidAttributeValue
		{
			get
			{
				return new LocalizedString("ErrorCalendarInvalidAttributeValue", "Ex347803", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidRecurrenceFormat
		{
			get
			{
				return new LocalizedString("MessageInvalidRecurrenceFormat", "Ex67F0C8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidAppApiVersionSupported
		{
			get
			{
				return new LocalizedString("ErrorInvalidAppApiVersionSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidManagedFolderSize
		{
			get
			{
				return new LocalizedString("ErrorInvalidManagedFolderSize", "Ex430991", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTokenSerializationDenied
		{
			get
			{
				return new LocalizedString("ErrorTokenSerializationDenied", "ExEEBC40", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidRequest", "ExC846B3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubscriptionUnsubscribed
		{
			get
			{
				return new LocalizedString("ErrorSubscriptionUnsubscribed", "Ex58FF9D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationCancelItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationCancelItem", "ExE44FBE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmCorruptProtectedMessage
		{
			get
			{
				return new LocalizedString("IrmCorruptProtectedMessage", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsGroupMailboxForAccept
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsGroupMailboxForAccept", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPropertyNotSupportCreate(string property)
		{
			return new LocalizedString("ErrorPropertyNotSupportCreate", "", false, false, CoreResources.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ErrorMailboxSearchFailed
		{
			get
			{
				return new LocalizedString("ErrorMailboxSearchFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxConfiguration
		{
			get
			{
				return new LocalizedString("ErrorMailboxConfiguration", "ExAA65D5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorNotSettable
		{
			get
			{
				return new LocalizedString("RuleErrorNotSettable", "Ex977AE9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCopyPublicFolderNotSupported
		{
			get
			{
				return new LocalizedString("ErrorCopyPublicFolderNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidWatermark
		{
			get
			{
				return new LocalizedString("ErrorInvalidWatermark", "Ex4803E3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorActingAsUserNotFound
		{
			get
			{
				return new LocalizedString("ErrorActingAsUserNotFound", "ExC5B2FD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDelegateMissingConfiguration
		{
			get
			{
				return new LocalizedString("ErrorDelegateMissingConfiguration", "Ex065950", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarUnableToUpdateAssociatedCalendarItem
		{
			get
			{
				return new LocalizedString("MessageCalendarUnableToUpdateAssociatedCalendarItem", "Ex675015", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageMissingMailboxOwnerEmailAddress
		{
			get
			{
				return new LocalizedString("MessageMissingMailboxOwnerEmailAddress", "Ex9E01C1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSentMeetingRequestUpdate
		{
			get
			{
				return new LocalizedString("ErrorSentMeetingRequestUpdate", "Ex71A9BA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidTimeZone
		{
			get
			{
				return new LocalizedString("descInvalidTimeZone", "ExBADDEF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationDisposalTypeAssociatedItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationDisposalTypeAssociatedItem", "Ex3BE185", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesMoveTypeMustBeSet
		{
			get
			{
				return new LocalizedString("UpdateFavoritesMoveTypeMustBeSet", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionNeedDeleteTypeForSetDeleteAction
		{
			get
			{
				return new LocalizedString("ConversationActionNeedDeleteTypeForSetDeleteAction", "Ex4822A0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidProxySecurityContext
		{
			get
			{
				return new LocalizedString("ErrorInvalidProxySecurityContext", "Ex6DFD0A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForProperty
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForProperty", "Ex0E5A87", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRestriction
		{
			get
			{
				return new LocalizedString("ErrorInvalidRestriction", "Ex377BAE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorInvalidAddress
		{
			get
			{
				return new LocalizedString("RuleErrorInvalidAddress", "ExF432C6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorSizeLessThanZero
		{
			get
			{
				return new LocalizedString("RuleErrorSizeLessThanZero", "ExB93225", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Orange
		{
			get
			{
				return new LocalizedString("Orange", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRecipientTypeNotSupported
		{
			get
			{
				return new LocalizedString("ErrorRecipientTypeNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdTooManyAttachmentLevels
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdTooManyAttachmentLevels", "Ex9944CB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExportRemoteArchiveItemsFailed
		{
			get
			{
				return new LocalizedString("ErrorExportRemoteArchiveItemsFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSendMessageFromPublicFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotSendMessageFromPublicFolder", "Ex4E013A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInsufficientPermissions
		{
			get
			{
				return new LocalizedString("MessageInsufficientPermissions", "Ex54669E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCorrelationFailed
		{
			get
			{
				return new LocalizedString("MessageCorrelationFailed", "ExED4639", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoMailboxSpecifiedForHoldOperation
		{
			get
			{
				return new LocalizedString("ErrorNoMailboxSpecifiedForHoldOperation", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeZone
		{
			get
			{
				return new LocalizedString("ErrorTimeZone", "Ex0F53B2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSendAsDenied
		{
			get
			{
				return new LocalizedString("ErrorSendAsDenied", "ExC8411C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultifactorRegistrationUnavailable(string appId)
		{
			return new LocalizedString("MultifactorRegistrationUnavailable", "", false, false, CoreResources.ResourceManager, new object[]
			{
				appId
			});
		}

		public static LocalizedString MessageSingleOrRecurringCalendarItemExpected
		{
			get
			{
				return new LocalizedString("MessageSingleOrRecurringCalendarItemExpected", "Ex95B08F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchQueryCannotBeEmpty
		{
			get
			{
				return new LocalizedString("ErrorSearchQueryCannotBeEmpty", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMultipleMailboxesCurrentlyNotSupported
		{
			get
			{
				return new LocalizedString("ErrorMultipleMailboxesCurrentlyNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorParentFolderNotFound
		{
			get
			{
				return new LocalizedString("ErrorParentFolderNotFound", "ExD04F86", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDelegateCannotAddOwner
		{
			get
			{
				return new LocalizedString("ErrorDelegateCannotAddOwner", "ExB2DEF5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToMoveMeetingCancellation
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToMoveMeetingCancellation", "ExC83B53", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImpersonateUserDenied
		{
			get
			{
				return new LocalizedString("ErrorImpersonateUserDenied", "ExD323C0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReadReceiptNotPending
		{
			get
			{
				return new LocalizedString("ErrorReadReceiptNotPending", "Ex2490C5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRetentionTagIdGuid
		{
			get
			{
				return new LocalizedString("ErrorInvalidRetentionTagIdGuid", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotCreateTaskInNonTaskFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotCreateTaskInNonTaskFolder", "ExB741C4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageNonExistentMailboxNoSmtpAddress
		{
			get
			{
				return new LocalizedString("MessageNonExistentMailboxNoSmtpAddress", "ExDE06C3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSchemaValidation
		{
			get
			{
				return new LocalizedString("ErrorSchemaValidation", "Ex98A4F8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageManagementRoleHeaderValueNotApplicable
		{
			get
			{
				return new LocalizedString("MessageManagementRoleHeaderValueNotApplicable", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidRuleVersion
		{
			get
			{
				return new LocalizedString("MessageInvalidRuleVersion", "ExCE2C4A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedMimeConversion
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedMimeConversion", "Ex4A3BBA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotMovePublicFolderItemOnDelete
		{
			get
			{
				return new LocalizedString("ErrorCannotMovePublicFolderItemOnDelete", "Ex2C5752", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationArchiveItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationArchiveItem", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSearchQuerySyntax
		{
			get
			{
				return new LocalizedString("ErrorInvalidSearchQuerySyntax", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForCountSystemQueryOption
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForCountSystemQueryOption", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderSaveFailed
		{
			get
			{
				return new LocalizedString("ErrorFolderSaveFailed", "Ex7238DA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorStringValueTooBig(int limit)
		{
			return new LocalizedString("RuleErrorStringValueTooBig", "ExA45ABF", false, true, CoreResources.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString MessageTargetMailboxNotInRoleScope
		{
			get
			{
				return new LocalizedString("MessageTargetMailboxNotInRoleScope", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSearchId
		{
			get
			{
				return new LocalizedString("ErrorInvalidSearchId", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationSyncFolderHierarchyForPublicFolder
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationSyncFolderHierarchyForPublicFolder", "ExA79391", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInternalServerErrorFaultInjection(string errorcode, string soapaction)
		{
			return new LocalizedString("ErrorInternalServerErrorFaultInjection", "", false, false, CoreResources.ResourceManager, new object[]
			{
				errorcode,
				soapaction
			});
		}

		public static LocalizedString ErrorItemCorrupt
		{
			get
			{
				return new LocalizedString("ErrorItemCorrupt", "Ex6F566D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorServerTemporaryUnavailable
		{
			get
			{
				return new LocalizedString("ErrorServerTemporaryUnavailable", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotArchiveCalendarContactTaskFolderException
		{
			get
			{
				return new LocalizedString("ErrorCannotArchiveCalendarContactTaskFolderException", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorReturnTooManyMailboxesFromDG(string dgName, int maxMailboxesToReturn)
		{
			return new LocalizedString("ErrorReturnTooManyMailboxesFromDG", "", false, false, CoreResources.ResourceManager, new object[]
			{
				dgName,
				maxMailboxesToReturn
			});
		}

		public static LocalizedString ErrorInvalidItemForOperationSendItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationSendItem", "ExCD2AED", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAggregatedAccountAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorAggregatedAccountAlreadyExists", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidServerVersion
		{
			get
			{
				return new LocalizedString("ErrorInvalidServerVersion", "ExFF89AD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGroupingNonNullWithSuggestionsViewFilter
		{
			get
			{
				return new LocalizedString("ErrorGroupingNonNullWithSuggestionsViewFilter", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidMailboxNotPrivateDL
		{
			get
			{
				return new LocalizedString("MessageInvalidMailboxNotPrivateDL", "Ex56C8F2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorItemPropertyRequestFailed
		{
			get
			{
				return new LocalizedString("ErrorItemPropertyRequestFailed", "Ex205ED8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionNeedDestinationFolderForCopyAction
		{
			get
			{
				return new LocalizedString("ConversationActionNeedDestinationFolderForCopyAction", "Ex4E6A21", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLocationServicesRequestFailed
		{
			get
			{
				return new LocalizedString("ErrorLocationServicesRequestFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnrecognizedDistinguishedFolderName
		{
			get
			{
				return new LocalizedString("UnrecognizedDistinguishedFolderName", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSubfilterTypeNotRecipientType
		{
			get
			{
				return new LocalizedString("ErrorInvalidSubfilterTypeNotRecipientType", "ExD18E5C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertySet
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertySet", "ExE5C0CC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesFolderCannotBeNull
		{
			get
			{
				return new LocalizedString("UpdateFavoritesFolderCannotBeNull", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotRemoveAggregatedAccountFromList
		{
			get
			{
				return new LocalizedString("ErrorCannotRemoveAggregatedAccountFromList", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProxyTokenExpired
		{
			get
			{
				return new LocalizedString("ErrorProxyTokenExpired", "Ex252BFB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotCreateCalendarItemInNonCalendarFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotCreateCalendarItemInNonCalendarFolder", "Ex7797D0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationGroupByAssociatedItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationGroupByAssociatedItem", "ExDB608C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarUnableToCreateAssociatedCalendarItem
		{
			get
			{
				return new LocalizedString("MessageCalendarUnableToCreateAssociatedCalendarItem", "Ex6A674D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMultiLegacyMailboxAccess
		{
			get
			{
				return new LocalizedString("ErrorMultiLegacyMailboxAccess", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnifiedMailboxAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorUnifiedMailboxAlreadyExists", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyAppend
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertyAppend", "Ex0102FB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorObjectTypeChanged
		{
			get
			{
				return new LocalizedString("ErrorObjectTypeChanged", "ExBF8119", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchableObjectNotFound
		{
			get
			{
				return new LocalizedString("ErrorSearchableObjectNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEndTimeMustBeGreaterThanStartTime
		{
			get
			{
				return new LocalizedString("ErrorEndTimeMustBeGreaterThanStartTime", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidFederatedOrganizationId
		{
			get
			{
				return new LocalizedString("ErrorInvalidFederatedOrganizationId", "Ex25CFAE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageExtensionNotAllowedToUpdateFAI
		{
			get
			{
				return new LocalizedString("MessageExtensionNotAllowedToUpdateFAI", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorValueOutOfRange
		{
			get
			{
				return new LocalizedString("ErrorValueOutOfRange", "ExBC5EF4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotEnoughMemory
		{
			get
			{
				return new LocalizedString("ErrorNotEnoughMemory", "Ex0BDAD7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExtendedPropertyValue
		{
			get
			{
				return new LocalizedString("ErrorInvalidExtendedPropertyValue", "ExB0CA9C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMoveCopyFailed
		{
			get
			{
				return new LocalizedString("ErrorMoveCopyFailed", "Ex76C12C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetClientExtensionTokenFailed
		{
			get
			{
				return new LocalizedString("GetClientExtensionTokenFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorVirusDetected
		{
			get
			{
				return new LocalizedString("ErrorVirusDetected", "ExB375DC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidVotingResponse
		{
			get
			{
				return new LocalizedString("ErrorInvalidVotingResponse", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorInboxRulesValidationError
		{
			get
			{
				return new LocalizedString("RuleErrorInboxRulesValidationError", "ExFE2837", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdMonikerTooLong
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdMonikerTooLong", "Ex5391B9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActionQueueDeserializationError(string key, string value, string typeName, string message)
		{
			return new LocalizedString("ActionQueueDeserializationError", "", false, false, CoreResources.ResourceManager, new object[]
			{
				key,
				value,
				typeName,
				message
			});
		}

		public static LocalizedString ErrorMultipleSearchRootsDisallowedWithSearchContext
		{
			get
			{
				return new LocalizedString("ErrorMultipleSearchRootsDisallowedWithSearchContext", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserNotUnifiedMessagingEnabled
		{
			get
			{
				return new LocalizedString("ErrorUserNotUnifiedMessagingEnabled", "Ex9ADB0A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotMovePublicFolderToPrivateMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotMovePublicFolderToPrivateMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionAlwaysMoveNoPublicFolder
		{
			get
			{
				return new LocalizedString("ConversationActionAlwaysMoveNoPublicFolder", "ExFD860E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCallerIsInvalidADAccount
		{
			get
			{
				return new LocalizedString("ErrorCallerIsInvalidADAccount", "ExCB60F2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoDestinationCASDueToSSLRequirements
		{
			get
			{
				return new LocalizedString("ErrorNoDestinationCASDueToSSLRequirements", "Ex73A10A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInternalServerTransientError
		{
			get
			{
				return new LocalizedString("ErrorInternalServerTransientError", "Ex92A2FF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidParentFolder
		{
			get
			{
				return new LocalizedString("ErrorInvalidParentFolder", "Ex329760", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveFolderPathCreation
		{
			get
			{
				return new LocalizedString("ErrorArchiveFolderPathCreation", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToMoveItem
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToMoveItem", "Ex8FB7D7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessagePerFolderCountReceiveQuotaExceeded
		{
			get
			{
				return new LocalizedString("ErrorMessagePerFolderCountReceiveQuotaExceeded", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDateTimeNotInUTC
		{
			get
			{
				return new LocalizedString("ErrorDateTimeNotInUTC", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidAttachmentSubfilter
		{
			get
			{
				return new LocalizedString("ErrorInvalidAttachmentSubfilter", "Ex41988F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMandatoryPropertyMissing(string property)
		{
			return new LocalizedString("ErrorMandatoryPropertyMissing", "", false, false, CoreResources.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString GetTenantFailed(string name, string error)
		{
			return new LocalizedString("GetTenantFailed", "", false, false, CoreResources.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString ErrorUserConfigurationDictionaryNotExist
		{
			get
			{
				return new LocalizedString("ErrorUserConfigurationDictionaryNotExist", "Ex48A876", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FromColon
		{
			get
			{
				return new LocalizedString("FromColon", "Ex67804C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSubscriptionRequestNoFolderIds
		{
			get
			{
				return new LocalizedString("ErrorInvalidSubscriptionRequestNoFolderIds", "ExE4CE3B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCallerIsComputerAccount
		{
			get
			{
				return new LocalizedString("ErrorCallerIsComputerAccount", "Ex3123DF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetGroupMailboxFailed(string name, string error)
		{
			return new LocalizedString("GetGroupMailboxFailed", "", false, false, CoreResources.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString ErrorDeleteItemsFailed
		{
			get
			{
				return new LocalizedString("ErrorDeleteItemsFailed", "Ex22718B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotApplicableOutsideOfDatacenter
		{
			get
			{
				return new LocalizedString("ErrorNotApplicableOutsideOfDatacenter", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorOutlookRuleBlobExists
		{
			get
			{
				return new LocalizedString("RuleErrorOutlookRuleBlobExists", "ExECF55C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidOofRequestPublicFolder
		{
			get
			{
				return new LocalizedString("descInvalidOofRequestPublicFolder", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidUrlQuery(string value)
		{
			return new LocalizedString("ErrorInvalidUrlQuery", "", false, false, CoreResources.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorMailboxIsNotPartOfAggregatedMailboxes
		{
			get
			{
				return new LocalizedString("ErrorMailboxIsNotPartOfAggregatedMailboxes", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRetentionTagNone
		{
			get
			{
				return new LocalizedString("ErrorInvalidRetentionTagNone", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidRoleTypeString
		{
			get
			{
				return new LocalizedString("MessageInvalidRoleTypeString", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidMailboxRecipientNotFoundInActiveDirectory
		{
			get
			{
				return new LocalizedString("MessageInvalidMailboxRecipientNotFoundInActiveDirectory", "Ex38F07E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoSyncRequestsMatchedSpecifiedEmailAddress
		{
			get
			{
				return new LocalizedString("ErrorNoSyncRequestsMatchedSpecifiedEmailAddress", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidDestinationFolderForPostItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidDestinationFolderForPostItem", "Ex3ABA0D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGetRemoteArchiveFolderFailed
		{
			get
			{
				return new LocalizedString("ErrorGetRemoteArchiveFolderFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RightsManagementMailboxOnlySupport
		{
			get
			{
				return new LocalizedString("RightsManagementMailboxOnlySupport", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingItemForCreateItemAttachment
		{
			get
			{
				return new LocalizedString("ErrorMissingItemForCreateItemAttachment", "Ex271EC4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFindRemoteArchiveFolderFailed
		{
			get
			{
				return new LocalizedString("ErrorFindRemoteArchiveFolderFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarFolderIsInvalidForCalendarView
		{
			get
			{
				return new LocalizedString("ErrorCalendarFolderIsInvalidForCalendarView", "Ex90EC10", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFindConversationNotSupportedForPublicFolders
		{
			get
			{
				return new LocalizedString("ErrorFindConversationNotSupportedForPublicFolders", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserConfigurationBinaryDataNotExist
		{
			get
			{
				return new LocalizedString("ErrorUserConfigurationBinaryDataNotExist", "ExDC22B0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultHtmlAttachmentHrefText
		{
			get
			{
				return new LocalizedString("DefaultHtmlAttachmentHrefText", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Green
		{
			get
			{
				return new LocalizedString("Green", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorItemNotFound
		{
			get
			{
				return new LocalizedString("ErrorItemNotFound", "ExB31B07", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotLinkMoreThanOneO365AccountToAnMsa(string existingLinkedAccount)
		{
			return new LocalizedString("ErrorCannotLinkMoreThanOneO365AccountToAnMsa", "", false, false, CoreResources.ResourceManager, new object[]
			{
				existingLinkedAccount
			});
		}

		public static LocalizedString ErrorCannotEmptyFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotEmptyFolder", "Ex3AAFA2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Yellow
		{
			get
			{
				return new LocalizedString("Yellow", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSubscription
		{
			get
			{
				return new LocalizedString("ErrorInvalidSubscription", "ExF6C668", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSchemaValidationColon
		{
			get
			{
				return new LocalizedString("ErrorSchemaValidationColon", "Ex736F42", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDelegateNoUser
		{
			get
			{
				return new LocalizedString("ErrorDelegateNoUser", "Ex1430F8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorMissingRangeValue
		{
			get
			{
				return new LocalizedString("RuleErrorMissingRangeValue", "Ex84FF39", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageWebMethodUnavailable
		{
			get
			{
				return new LocalizedString("MessageWebMethodUnavailable", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedQueryFilter
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedQueryFilter", "Ex3C1A1A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateLegacyDistinguishedNameFound(string legDN)
		{
			return new LocalizedString("ErrorDuplicateLegacyDistinguishedNameFound", "", false, false, CoreResources.ResourceManager, new object[]
			{
				legDN
			});
		}

		public static LocalizedString ErrorCannotMovePublicFolderOnDelete
		{
			get
			{
				return new LocalizedString("ErrorCannotMovePublicFolderOnDelete", "Ex60D2B0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAccessModeSpecified
		{
			get
			{
				return new LocalizedString("ErrorAccessModeSpecified", "ExD27C78", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPhotoSize
		{
			get
			{
				return new LocalizedString("ErrorInvalidPhotoSize", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMultipleMailboxSearchNotSupported
		{
			get
			{
				return new LocalizedString("ErrorMultipleMailboxSearchNotSupported", "ExA7218B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageManagementRoleHeaderNotSupportedForPartnerIdentity
		{
			get
			{
				return new LocalizedString("MessageManagementRoleHeaderNotSupportedForPartnerIdentity", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionInvalidFolderType
		{
			get
			{
				return new LocalizedString("ConversationActionInvalidFolderType", "ExF5AE11", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedSubFilter
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedSubFilter", "Ex1EF04B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidComplianceId
		{
			get
			{
				return new LocalizedString("ErrorInvalidComplianceId", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarCannotUpdateDeletedItem
		{
			get
			{
				return new LocalizedString("ErrorCalendarCannotUpdateDeletedItem", "ExE942E8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationDistinguishedGroupByAssociatedItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationDistinguishedGroupByAssociatedItem", "Ex543CAE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidDelegatePermission
		{
			get
			{
				return new LocalizedString("ErrorInvalidDelegatePermission", "ExE9381E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInternalServerError
		{
			get
			{
				return new LocalizedString("ErrorInternalServerError", "ExE0EC81", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoPublicFolderServerAvailable
		{
			get
			{
				return new LocalizedString("ErrorNoPublicFolderServerAvailable", "Ex22D031", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPhoneCallId
		{
			get
			{
				return new LocalizedString("ErrorInvalidPhoneCallId", "Ex92D768", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidGetSharingFolderRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidGetSharingFolderRequest", "ExB1BEBA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotResolveOrganizationName
		{
			get
			{
				return new LocalizedString("ErrorCannotResolveOrganizationName", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedCulture
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedCulture", "ExF0FD16", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidChangeKey
		{
			get
			{
				return new LocalizedString("ErrorInvalidChangeKey", "Ex590F94", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMimeContentConversionFailed
		{
			get
			{
				return new LocalizedString("ErrorMimeContentConversionFailed", "Ex06F3D8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorResolveNamesOnlyOneContactsFolderAllowed
		{
			get
			{
				return new LocalizedString("ErrorResolveNamesOnlyOneContactsFolderAllowed", "Ex89E325", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSchemaVersionForMailboxVersion
		{
			get
			{
				return new LocalizedString("ErrorInvalidSchemaVersionForMailboxVersion", "ExB35CA2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRequestQuotaExceeded
		{
			get
			{
				return new LocalizedString("ErrorInvalidRequestQuotaExceeded", "ExC14A12", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTokenRequestUnauthorized
		{
			get
			{
				return new LocalizedString("MessageTokenRequestUnauthorized", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageUserRoleNotApplicableForAppOnlyToken
		{
			get
			{
				return new LocalizedString("MessageUserRoleNotApplicableForAppOnlyToken", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForPropertyKeyValueConversion
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyKeyValueConversion", "Ex707887", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRetentionTagInheritance
		{
			get
			{
				return new LocalizedString("ErrorInvalidRetentionTagInheritance", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Conversation
		{
			get
			{
				return new LocalizedString("Conversation", "ExC6ED2D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotCreateUnifiedMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotCreateUnifiedMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailTipsDisabled
		{
			get
			{
				return new LocalizedString("ErrorMailTipsDisabled", "ExDFD164", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingItemIdForCreateItemAttachment
		{
			get
			{
				return new LocalizedString("ErrorMissingItemIdForCreateItemAttachment", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidMailbox
		{
			get
			{
				return new LocalizedString("ErrorInvalidMailbox", "ExDBBA0C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDelegateValidationFailed
		{
			get
			{
				return new LocalizedString("ErrorDelegateValidationFailed", "ExBB2997", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserPromptNeeded
		{
			get
			{
				return new LocalizedString("ErrorUserPromptNeeded", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorMissingAction
		{
			get
			{
				return new LocalizedString("RuleErrorMissingAction", "ExC05CF5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorApplyConversationActionFailed
		{
			get
			{
				return new LocalizedString("ErrorApplyConversationActionFailed", "ExEE0DC7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInsufficientResources
		{
			get
			{
				return new LocalizedString("ErrorInsufficientResources", "ExFACD4B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorActingAsRequired
		{
			get
			{
				return new LocalizedString("ErrorActingAsRequired", "ExC275EF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarInvalidDayForWeeklyRecurrence
		{
			get
			{
				return new LocalizedString("ErrorCalendarInvalidDayForWeeklyRecurrence", "Ex8952F9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingInformationEmailAddress
		{
			get
			{
				return new LocalizedString("ErrorMissingInformationEmailAddress", "Ex606D76", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesFavoriteNotFound
		{
			get
			{
				return new LocalizedString("UpdateFavoritesFavoriteNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarDurationIsTooLong
		{
			get
			{
				return new LocalizedString("ErrorCalendarDurationIsTooLong", "Ex7FDD46", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoRespondingCASInDestinationSite
		{
			get
			{
				return new LocalizedString("ErrorNoRespondingCASInDestinationSite", "Ex1634C2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRecipients
		{
			get
			{
				return new LocalizedString("ErrorInvalidRecipients", "ExA4DFCE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAppendBodyTypeMismatch
		{
			get
			{
				return new LocalizedString("ErrorAppendBodyTypeMismatch", "Ex64354B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDistributionListMemberNotExist
		{
			get
			{
				return new LocalizedString("ErrorDistributionListMemberNotExist", "ExF51352", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRequestTimeout
		{
			get
			{
				return new LocalizedString("ErrorRequestTimeout", "Ex5C1EFA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageApplicationHasNoRoleAssginedWhichUserHas
		{
			get
			{
				return new LocalizedString("MessageApplicationHasNoRoleAssginedWhichUserHas", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveMailboxGetConversationFailed
		{
			get
			{
				return new LocalizedString("ErrorArchiveMailboxGetConversationFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorClientIntentNotFound
		{
			get
			{
				return new LocalizedString("ErrorClientIntentNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonExistentMailbox
		{
			get
			{
				return new LocalizedString("ErrorNonExistentMailbox", "ExD4BB19", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorVirusMessageDeleted
		{
			get
			{
				return new LocalizedString("ErrorVirusMessageDeleted", "ExE1B402", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotFindUnifiedMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotFindUnifiedMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnifiedMailboxSupportedOnlyWithMicrosoftAccount
		{
			get
			{
				return new LocalizedString("ErrorUnifiedMailboxSupportedOnlyWithMicrosoftAccount", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GroupMailboxCreationFailed
		{
			get
			{
				return new LocalizedString("GroupMailboxCreationFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSearchQueryLength
		{
			get
			{
				return new LocalizedString("ErrorInvalidSearchQueryLength", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarInvalidPropertyState
		{
			get
			{
				return new LocalizedString("ErrorCalendarInvalidPropertyState", "ExA8B660", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAddDelegatesFailed
		{
			get
			{
				return new LocalizedString("ErrorAddDelegatesFailed", "ExFDCC08", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CcColon
		{
			get
			{
				return new LocalizedString("CcColon", "Ex449B53", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCrossSiteRequest
		{
			get
			{
				return new LocalizedString("ErrorCrossSiteRequest", "Ex3F6822", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPublicFolderUserMustHaveMailbox
		{
			get
			{
				return new LocalizedString("ErrorPublicFolderUserMustHaveMailbox", "ExCAB9A6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessageTrackingTransientError
		{
			get
			{
				return new LocalizedString("ErrorMessageTrackingTransientError", "ExA14CF6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorToFolderNotFound
		{
			get
			{
				return new LocalizedString("ErrorToFolderNotFound", "Ex97069E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDeleteUnifiedMessagingPromptFailed
		{
			get
			{
				return new LocalizedString("ErrorDeleteUnifiedMessagingPromptFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesUnableToMoveFavorite
		{
			get
			{
				return new LocalizedString("UpdateFavoritesUnableToMoveFavorite", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPeopleConnectionNoToken
		{
			get
			{
				return new LocalizedString("ErrorPeopleConnectionNoToken", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSpecifySearchFolderAsSourceFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotSpecifySearchFolderAsSourceFolder", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorEmailAddressMismatch
		{
			get
			{
				return new LocalizedString("ErrorEmailAddressMismatch", "Ex0EE79E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAggregatedAccountLimitReached(int limit)
		{
			return new LocalizedString("ErrorAggregatedAccountLimitReached", "", false, false, CoreResources.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString ErrorUserConfigurationXmlDataNotExist
		{
			get
			{
				return new LocalizedString("ErrorUserConfigurationXmlDataNotExist", "Ex886EE9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnifiedMessagingRequestFailed
		{
			get
			{
				return new LocalizedString("ErrorUnifiedMessagingRequestFailed", "Ex5F4492", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCreateItemAccessDenied
		{
			get
			{
				return new LocalizedString("ErrorCreateItemAccessDenied", "Ex01C220", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorFolderDoesNotExist
		{
			get
			{
				return new LocalizedString("RuleErrorFolderDoesNotExist", "Ex4685E4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidImContactId
		{
			get
			{
				return new LocalizedString("ErrorInvalidImContactId", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoPropertyTagForCustomProperties
		{
			get
			{
				return new LocalizedString("ErrorNoPropertyTagForCustomProperties", "ExD53820", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRightsManagementDuplicateTemplateId(string id)
		{
			return new LocalizedString("ErrorRightsManagementDuplicateTemplateId", "", false, false, CoreResources.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString SentTime
		{
			get
			{
				return new LocalizedString("SentTime", "Ex3CA30C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageNonExistentMailboxGuid
		{
			get
			{
				return new LocalizedString("MessageNonExistentMailboxGuid", "ExF743A6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMaxRequestedUnifiedGroupsSetsExceeded
		{
			get
			{
				return new LocalizedString("ErrorMaxRequestedUnifiedGroupsSetsExceeded", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidAppSchemaVersionSupported
		{
			get
			{
				return new LocalizedString("ErrorInvalidAppSchemaVersionSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidLogonType
		{
			get
			{
				return new LocalizedString("ErrorInvalidLogonType", "Ex5C410E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageActAsUserRequiredForSuchApplicationRole
		{
			get
			{
				return new LocalizedString("MessageActAsUserRequiredForSuchApplicationRole", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarOutOfRange
		{
			get
			{
				return new LocalizedString("ErrorCalendarOutOfRange", "Ex09A8D3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorContentIndexingNotEnabled
		{
			get
			{
				return new LocalizedString("ErrorContentIndexingNotEnabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorContentConversionFailed
		{
			get
			{
				return new LocalizedString("ErrorContentConversionFailed", "Ex0EA831", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationIdNotSupported
		{
			get
			{
				return new LocalizedString("ConversationIdNotSupported", "Ex5D7451", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationSupportedOnlyForMailboxSession
		{
			get
			{
				return new LocalizedString("ConversationSupportedOnlyForMailboxSession", "ExFF98D8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMoveDistinguishedFolder
		{
			get
			{
				return new LocalizedString("ErrorMoveDistinguishedFolder", "Ex1871EC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxCannotBeSpecifiedForPublicFolderRoot
		{
			get
			{
				return new LocalizedString("ErrorMailboxCannotBeSpecifiedForPublicFolderRoot", "Ex04F82B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmPreLicensingFailure
		{
			get
			{
				return new LocalizedString("IrmPreLicensingFailure", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageMissingUserRolesForLegalHoldRoleTypeApp
		{
			get
			{
				return new LocalizedString("MessageMissingUserRolesForLegalHoldRoleTypeApp", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxVersionNotSupported
		{
			get
			{
				return new LocalizedString("ErrorMailboxVersionNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRestrictionTooComplex
		{
			get
			{
				return new LocalizedString("ErrorRestrictionTooComplex", "Ex73969B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorRecipientDoesNotExist
		{
			get
			{
				return new LocalizedString("RuleErrorRecipientDoesNotExist", "Ex7BFD5A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidAggregatedAccountCredentials
		{
			get
			{
				return new LocalizedString("ErrorInvalidAggregatedAccountCredentials", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidSecurityContext
		{
			get
			{
				return new LocalizedString("descInvalidSecurityContext", "ExA3F1D9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessagePublicFoldersNotSupportedForNonIndexable
		{
			get
			{
				return new LocalizedString("MessagePublicFoldersNotSupportedForNonIndexable", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidFilterNode
		{
			get
			{
				return new LocalizedString("ErrorInvalidFilterNode", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIrmUserRightNotGranted
		{
			get
			{
				return new LocalizedString("ErrorIrmUserRightNotGranted", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidRequestType
		{
			get
			{
				return new LocalizedString("descInvalidRequestType", "ExF721AD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DowaNotProvisioned
		{
			get
			{
				return new LocalizedString("DowaNotProvisioned", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRecurrenceEndDateTooBig
		{
			get
			{
				return new LocalizedString("ErrorRecurrenceEndDateTooBig", "Ex3B4015", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForReply
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForReply", "ExC9FD00", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesInvalidUpdateFavoriteOperationType
		{
			get
			{
				return new LocalizedString("UpdateFavoritesInvalidUpdateFavoriteOperationType", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidManagementRoleHeader
		{
			get
			{
				return new LocalizedString("ErrorInvalidManagementRoleHeader", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotGetExternalEcpUrl
		{
			get
			{
				return new LocalizedString("ErrorCannotGetExternalEcpUrl", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchTooManyMailboxes(string errorHint, int mailboxCount, int maxAllowedMailboxes)
		{
			return new LocalizedString("ErrorSearchTooManyMailboxes", "", false, false, CoreResources.ResourceManager, new object[]
			{
				errorHint,
				mailboxCount,
				maxAllowedMailboxes
			});
		}

		public static LocalizedString ErrorCannotCreateSearchFolderInPublicFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotCreateSearchFolderInPublicFolder", "Ex76F926", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorUnsupportedRule
		{
			get
			{
				return new LocalizedString("RuleErrorUnsupportedRule", "ExD129BD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingManagedFolderId
		{
			get
			{
				return new LocalizedString("ErrorMissingManagedFolderId", "Ex341D82", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInsufficientPermissionsToSend
		{
			get
			{
				return new LocalizedString("MessageInsufficientPermissionsToSend", "Ex3C9088", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidCompleteDate
		{
			get
			{
				return new LocalizedString("ErrorInvalidCompleteDate", "ExE257EC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchFolderTimeout
		{
			get
			{
				return new LocalizedString("ErrorSearchFolderTimeout", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetAggregatedAccount
		{
			get
			{
				return new LocalizedString("ErrorCannotSetAggregatedAccount", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPushSubscriptionUrl
		{
			get
			{
				return new LocalizedString("ErrorInvalidPushSubscriptionUrl", "Ex53B39C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotAddAggregatedAccount
		{
			get
			{
				return new LocalizedString("ErrorCannotAddAggregatedAccount", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsGroupMailboxForDecline
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsGroupMailboxForDecline", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNameResolutionNoMailbox
		{
			get
			{
				return new LocalizedString("ErrorNameResolutionNoMailbox", "Ex5D7FF7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotArchiveItemsInArchiveMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotArchiveItemsInArchiveMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmRmsErrorCode(string code)
		{
			return new LocalizedString("IrmRmsErrorCode", "", false, false, CoreResources.ResourceManager, new object[]
			{
				code
			});
		}

		public static LocalizedString MowaNotProvisioned
		{
			get
			{
				return new LocalizedString("MowaNotProvisioned", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationSendAndSaveCopyToPublicFolder
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationSendAndSaveCopyToPublicFolder", "Ex4ADBB0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionNeedDestinationFolderForMoveAction
		{
			get
			{
				return new LocalizedString("ConversationActionNeedDestinationFolderForMoveAction", "ExCEB5BD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetFederatedDirectoryGroupFailed(string name, string error)
		{
			return new LocalizedString("GetFederatedDirectoryGroupFailed", "", false, false, CoreResources.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString ErrorViewFilterRequiresSearchContext
		{
			get
			{
				return new LocalizedString("ErrorViewFilterRequiresSearchContext", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDelegateAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorDelegateAlreadyExists", "ExB85A77", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubmitQueryBasedHoldTaskFailed
		{
			get
			{
				return new LocalizedString("ErrorSubmitQueryBasedHoldTaskFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPeopleConnectFailedToReadApplicationConfiguration
		{
			get
			{
				return new LocalizedString("ErrorPeopleConnectFailedToReadApplicationConfiguration", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedMapiPropertyType
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedMapiPropertyType", "ExB8162D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorApprovalRequestAlreadyDecided
		{
			get
			{
				return new LocalizedString("ErrorApprovalRequestAlreadyDecided", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCouldNotFindWeatherLocations
		{
			get
			{
				return new LocalizedString("MessageCouldNotFindWeatherLocations", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhenColon
		{
			get
			{
				return new LocalizedString("WhenColon", "Ex592F5E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoGroupingForQueryString
		{
			get
			{
				return new LocalizedString("ErrorNoGroupingForQueryString", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdStoreObjectIdTooLong
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdStoreObjectIdTooLong", "ExC5B225", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorQuotaExceeded
		{
			get
			{
				return new LocalizedString("ErrorQuotaExceeded", "Ex276CAA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionNeedReadStateForSetReadStateAction
		{
			get
			{
				return new LocalizedString("ConversationActionNeedReadStateForSetReadStateAction", "Ex632E59", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLocationServicesRequestTimedOut
		{
			get
			{
				return new LocalizedString("ErrorLocationServicesRequestTimedOut", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarInvalidPropertyValue
		{
			get
			{
				return new LocalizedString("ErrorCalendarInvalidPropertyValue", "Ex8AD342", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedFolderAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorManagedFolderAlreadyExists", "ExEC0F10", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLocationServicesInvalidSource
		{
			get
			{
				return new LocalizedString("ErrorLocationServicesInvalidSource", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OnPremiseSynchorizedDiscoverySearch
		{
			get
			{
				return new LocalizedString("OnPremiseSynchorizedDiscoverySearch", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationForAssociatedItems
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationForAssociatedItems", "Ex6863C6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCorruptData
		{
			get
			{
				return new LocalizedString("ErrorCorruptData", "Ex6C01DC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorInvalidDateRange(string startDateTime, string endDateTime)
		{
			return new LocalizedString("RuleErrorInvalidDateRange", "Ex805828", false, true, CoreResources.ResourceManager, new object[]
			{
				startDateTime,
				endDateTime
			});
		}

		public static LocalizedString ErrorCalendarInvalidTimeZone
		{
			get
			{
				return new LocalizedString("ErrorCalendarInvalidTimeZone", "ExB0CF21", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationMessageDispositionAssociatedItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationMessageDispositionAssociatedItem", "Ex33C0D8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubscriptionAccessDenied
		{
			get
			{
				return new LocalizedString("ErrorSubscriptionAccessDenied", "ExD4869E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotReadRequestBody
		{
			get
			{
				return new LocalizedString("ErrorCannotReadRequestBody", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNameResolutionMultipleResults
		{
			get
			{
				return new LocalizedString("ErrorNameResolutionMultipleResults", "ExD4D5C6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExtendedProperty
		{
			get
			{
				return new LocalizedString("ErrorInvalidExtendedProperty", "ExF51F0A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EwsProxyCannotGetCredentials
		{
			get
			{
				return new LocalizedString("EwsProxyCannotGetCredentials", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UpdateFavoritesInvalidMoveFavoriteRequest
		{
			get
			{
				return new LocalizedString("UpdateFavoritesInvalidMoveFavoriteRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPermissionSettings
		{
			get
			{
				return new LocalizedString("ErrorInvalidPermissionSettings", "Ex523233", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProxyServiceDiscoveryFailed
		{
			get
			{
				return new LocalizedString("ErrorProxyServiceDiscoveryFailed", "Ex84DFBC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationAcceptItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationAcceptItem", "Ex5E6DCB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForPropertyDuplicateDictionaryKey
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyDuplicateDictionaryKey", "ExE0001C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExceededSubscriptionCount
		{
			get
			{
				return new LocalizedString("ErrorExceededSubscriptionCount", "Ex61D62C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPermissionNotAllowedByPolicy
		{
			get
			{
				return new LocalizedString("ErrorPermissionNotAllowedByPolicy", "Ex150362", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInsufficientPermissionsToSubscribe
		{
			get
			{
				return new LocalizedString("MessageInsufficientPermissionsToSubscribe", "Ex69B609", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForPropertyDate
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyDate", "Ex130502", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedRecurrence
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedRecurrence", "Ex065C5D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserADObjectNotFound
		{
			get
			{
				return new LocalizedString("ErrorUserADObjectNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotAttachSelf
		{
			get
			{
				return new LocalizedString("ErrorCannotAttachSelf", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingInformationSharingFolderId
		{
			get
			{
				return new LocalizedString("ErrorMissingInformationSharingFolderId", "ExEBBBDE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetFromOnMeetingResponse
		{
			get
			{
				return new LocalizedString("ErrorCannotSetFromOnMeetingResponse", "Ex0E0324", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidOperationForPublicFolderItemsAddParticipantByItemId
		{
			get
			{
				return new LocalizedString("MessageInvalidOperationForPublicFolderItemsAddParticipantByItemId", "Ex20810A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationCreateItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationCreateItem", "Ex9BB6C4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyForExists
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertyForExists", "Ex0E0BE8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSaveSentItemInPublicFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotSaveSentItemInPublicFolder", "Ex0B0492", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExtensionNotFound(string extensionID)
		{
			return new LocalizedString("ErrorExtensionNotFound", "", false, false, CoreResources.ResourceManager, new object[]
			{
				extensionID
			});
		}

		public static LocalizedString ErrorRestrictionTooLong
		{
			get
			{
				return new LocalizedString("ErrorRestrictionTooLong", "Ex321685", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedPropertyDefinition
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedPropertyDefinition", "ExE641E6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImContactLimitReached(int limit)
		{
			return new LocalizedString("ErrorImContactLimitReached", "", false, false, CoreResources.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString SharePointCreationFailed
		{
			get
			{
				return new LocalizedString("SharePointCreationFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDataSizeLimitExceeded
		{
			get
			{
				return new LocalizedString("ErrorDataSizeLimitExceeded", "Ex515477", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderExists
		{
			get
			{
				return new LocalizedString("ErrorFolderExists", "ExF56EB5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPropertyNotSupportUpdate(string property)
		{
			return new LocalizedString("ErrorPropertyNotSupportUpdate", "", false, false, CoreResources.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString ErrorUnifiedGroupAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorUnifiedGroupAlreadyExists", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageApplicationTokenOnly
		{
			get
			{
				return new LocalizedString("MessageApplicationTokenOnly", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSharingNoExternalEwsAvailable
		{
			get
			{
				return new LocalizedString("ErrorSharingNoExternalEwsAvailable", "Ex3D67D4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPropertyNotSupportFilter(string property)
		{
			return new LocalizedString("ErrorPropertyNotSupportFilter", "", false, false, CoreResources.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString RuleErrorEmptyValueFound
		{
			get
			{
				return new LocalizedString("RuleErrorEmptyValueFound", "Ex14D20A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOccurrenceCrossingBoundary
		{
			get
			{
				return new LocalizedString("ErrorOccurrenceCrossingBoundary", "ExEE33A0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveMailboxServiceDiscoveryFailed
		{
			get
			{
				return new LocalizedString("ErrorArchiveMailboxServiceDiscoveryFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidAttachmentSubfilterTextFilter
		{
			get
			{
				return new LocalizedString("ErrorInvalidAttachmentSubfilterTextFilter", "Ex1FDE39", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGetSharingMetadataNotSupported
		{
			get
			{
				return new LocalizedString("ErrorGetSharingMetadataNotSupported", "Ex836DF7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRecipientMustHaveEmailAddress
		{
			get
			{
				return new LocalizedString("MessageRecipientMustHaveEmailAddress", "ExAC2952", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRecipientSubfilterTextFilter
		{
			get
			{
				return new LocalizedString("ErrorInvalidRecipientSubfilterTextFilter", "ExC1900B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertyRequest", "Ex84179E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsNotOrganizer
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsNotOrganizer", "ExA9638F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidProvisionDeviceID
		{
			get
			{
				return new LocalizedString("ErrorInvalidProvisionDeviceID", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCouldNotGetWeatherDataForLocation
		{
			get
			{
				return new LocalizedString("MessageCouldNotGetWeatherDataForLocation", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeProposalMissingStartOrEndTimeError
		{
			get
			{
				return new LocalizedString("ErrorTimeProposalMissingStartOrEndTimeError", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSubfilterTypeNotAttendeeType
		{
			get
			{
				return new LocalizedString("ErrorInvalidSubfilterTypeNotAttendeeType", "ExC66AB4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyCommandNotSupportSet
		{
			get
			{
				return new LocalizedString("PropertyCommandNotSupportSet", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImpersonationFailed
		{
			get
			{
				return new LocalizedString("ErrorImpersonationFailed", "Ex4CB570", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubscriptionNotFound
		{
			get
			{
				return new LocalizedString("ErrorSubscriptionNotFound", "Ex308BB5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToMoveMeetingRequest
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToMoveMeetingRequest", "Ex281B8F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdMalformed
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdMalformed", "Ex9C1B66", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsGroupMailboxForSuppressReadReceipt
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsGroupMailboxForSuppressReadReceipt", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotGetSourceFolderPath
		{
			get
			{
				return new LocalizedString("ErrorCannotGetSourceFolderPath", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWildcardAndGroupExpansionNotAllowed
		{
			get
			{
				return new LocalizedString("ErrorWildcardAndGroupExpansionNotAllowed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedInlineAttachmentContentType
		{
			get
			{
				return new LocalizedString("UnsupportedInlineAttachmentContentType", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorUnexpectedError
		{
			get
			{
				return new LocalizedString("RuleErrorUnexpectedError", "ExF7776A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToDraftsFolder
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToDraftsFolder", "Ex9573E3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorADUnavailable
		{
			get
			{
				return new LocalizedString("ErrorADUnavailable", "Ex182232", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPhoneNumber
		{
			get
			{
				return new LocalizedString("ErrorInvalidPhoneNumber", "Ex712A46", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSoftDeletedTraversalsNotAllowedOnPublicFolders
		{
			get
			{
				return new LocalizedString("ErrorSoftDeletedTraversalsNotAllowedOnPublicFolders", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsDelegatedForTentative
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsDelegatedForTentative", "Ex695CDA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFoldersMustBelongToSameMailbox
		{
			get
			{
				return new LocalizedString("ErrorFoldersMustBelongToSameMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDataSourceOperation
		{
			get
			{
				return new LocalizedString("ErrorDataSourceOperation", "ExDA7D8E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarMeetingIsOutOfDateResponseNotProcessed
		{
			get
			{
				return new LocalizedString("ErrorCalendarMeetingIsOutOfDateResponseNotProcessed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidIdMalformedEwsIdFormat
		{
			get
			{
				return new LocalizedString("MessageInvalidIdMalformedEwsIdFormat", "ExAB29BC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPreviousPageNavigationCurrentlyNotSupported
		{
			get
			{
				return new LocalizedString("ErrorPreviousPageNavigationCurrentlyNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotEmptyPublicFolderToDeletedItems
		{
			get
			{
				return new LocalizedString("ErrorCannotEmptyPublicFolderToDeletedItems", "Ex37E536", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSharingData
		{
			get
			{
				return new LocalizedString("ErrorInvalidSharingData", "Ex2BBC6A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToMeetingMessageFolder
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToMeetingMessageFolder", "ExB60528", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationCannotSpecifyItemId
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationCannotSpecifyItemId", "Ex38B50D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsGroupMailboxForTentative
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsGroupMailboxForTentative", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessageSizeExceeded
		{
			get
			{
				return new LocalizedString("ErrorMessageSizeExceeded", "ExA25687", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDateTimePrecisionValue
		{
			get
			{
				return new LocalizedString("InvalidDateTimePrecisionValue", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorStaleObject
		{
			get
			{
				return new LocalizedString("ErrorStaleObject", "ExBF6C57", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetFederatedDirectoryUserFailed(string name, string error)
		{
			return new LocalizedString("GetFederatedDirectoryUserFailed", "", false, false, CoreResources.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString UpdateFavoritesUnableToAddFolderToFavorites
		{
			get
			{
				return new LocalizedString("UpdateFavoritesUnableToAddFolderToFavorites", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPasswordExpired
		{
			get
			{
				return new LocalizedString("ErrorPasswordExpired", "Ex84A415", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationCannotPerformOperationOnADRecipients
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationCannotPerformOperationOnADRecipients", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTooManyObjectsOpened
		{
			get
			{
				return new LocalizedString("ErrorTooManyObjectsOpened", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidMailboxInvalidReferencedItem
		{
			get
			{
				return new LocalizedString("MessageInvalidMailboxInvalidReferencedItem", "Ex8886C6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageApplicationHasNoGivenRoleAssigned
		{
			get
			{
				return new LocalizedString("MessageApplicationHasNoGivenRoleAssigned", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRecipientsArrayTooLong
		{
			get
			{
				return new LocalizedString("MessageRecipientsArrayTooLong", "Ex173DA5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdXml
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdXml", "Ex2F9DBD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCallerWithoutMailboxCannotUseSendOnly
		{
			get
			{
				return new LocalizedString("ErrorCallerWithoutMailboxCannotUseSendOnly", "Ex760520", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveMailboxSearchFailed
		{
			get
			{
				return new LocalizedString("ErrorArchiveMailboxSearchFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PostedOn
		{
			get
			{
				return new LocalizedString("PostedOn", "Ex4A0682", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExternalSharingInitiator
		{
			get
			{
				return new LocalizedString("ErrorInvalidExternalSharingInitiator", "Ex425BF6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxStoreUnavailable
		{
			get
			{
				return new LocalizedString("ErrorMailboxStoreUnavailable", "ExAD00DA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidCalendarViewRestrictionOrSort
		{
			get
			{
				return new LocalizedString("ErrorInvalidCalendarViewRestrictionOrSort", "Ex500516", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSavedItemFolderNotFound
		{
			get
			{
				return new LocalizedString("ErrorSavedItemFolderNotFound", "Ex65F59A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarOccurrenceIsDeletedFromRecurrence
		{
			get
			{
				return new LocalizedString("ErrorCalendarOccurrenceIsDeletedFromRecurrence", "ExA3AC90", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingRecipients
		{
			get
			{
				return new LocalizedString("ErrorMissingRecipients", "Ex631099", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeProposalInvalidInCreateItemRequest
		{
			get
			{
				return new LocalizedString("ErrorTimeProposalInvalidInCreateItemRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsDelegatedForRemove
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsDelegatedForRemove", "Ex5432D1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidLikeRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidLikeRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRecurrenceStartDateTooSmall
		{
			get
			{
				return new LocalizedString("MessageRecurrenceStartDateTooSmall", "Ex30666F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnknownTimeZone
		{
			get
			{
				return new LocalizedString("ErrorUnknownTimeZone", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeServiceResponseErrorWithCode(string errorCode, string errorMessage)
		{
			return new LocalizedString("ExchangeServiceResponseErrorWithCode", "", false, false, CoreResources.ResourceManager, new object[]
			{
				errorCode,
				errorMessage
			});
		}

		public static LocalizedString ErrorProxyGroupSidLimitExceeded
		{
			get
			{
				return new LocalizedString("ErrorProxyGroupSidLimitExceeded", "Ex3A4700", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotRemoveAggregatedAccount
		{
			get
			{
				return new LocalizedString("ErrorCannotRemoveAggregatedAccount", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidShape
		{
			get
			{
				return new LocalizedString("ErrorInvalidShape", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidLicense
		{
			get
			{
				return new LocalizedString("ErrorInvalidLicense", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAccountDisabled
		{
			get
			{
				return new LocalizedString("ErrorAccountDisabled", "Ex9B575D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorHoldIsNotFound
		{
			get
			{
				return new LocalizedString("ErrorHoldIsNotFound", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageMessageIsNotDraft
		{
			get
			{
				return new LocalizedString("MessageMessageIsNotDraft", "Ex4EC065", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWrongServerVersionDelegate
		{
			get
			{
				return new LocalizedString("ErrorWrongServerVersionDelegate", "Ex1FAF18", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OnBehalfOf
		{
			get
			{
				return new LocalizedString("OnBehalfOf", "ExA4C7CC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationForPublicFolderItems
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationForPublicFolderItems", "ExE4739A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidDelegateUserId(string s)
		{
			return new LocalizedString("ErrorInvalidDelegateUserId", "ExF63986", false, true, CoreResources.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ErrorCalendarCannotUseIdForRecurringMasterId
		{
			get
			{
				return new LocalizedString("ErrorCalendarCannotUseIdForRecurringMasterId", "Ex39E629", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSubscriptionRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidSubscriptionRequest", "Ex781E33", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdEmpty
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdEmpty", "ExF5E1DE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidAttachmentId
		{
			get
			{
				return new LocalizedString("ErrorInvalidAttachmentId", "Ex2CB985", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorBothQueryStringAndRestrictionNonNull
		{
			get
			{
				return new LocalizedString("ErrorBothQueryStringAndRestrictionNonNull", "Ex1CF719", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorRuleNotFound
		{
			get
			{
				return new LocalizedString("RuleErrorRuleNotFound", "ExA39D9D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDiscoverySearchesDisabled
		{
			get
			{
				return new LocalizedString("ErrorDiscoverySearchesDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsCancelledForTentative
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsCancelledForTentative", "Ex062096", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRecurrenceHasNoOccurrence
		{
			get
			{
				return new LocalizedString("ErrorRecurrenceHasNoOccurrence", "Ex8B963C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageNonExistentMailboxLegacyDN
		{
			get
			{
				return new LocalizedString("MessageNonExistentMailboxLegacyDN", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoDestinationCASDueToKerberosRequirements
		{
			get
			{
				return new LocalizedString("ErrorNoDestinationCASDueToKerberosRequirements", "Ex862091", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderNotFound
		{
			get
			{
				return new LocalizedString("ErrorFolderNotFound", "ExB41165", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyVersionRequest(string prop, string version)
		{
			return new LocalizedString("ErrorInvalidPropertyVersionRequest", "", false, false, CoreResources.ResourceManager, new object[]
			{
				prop,
				version
			});
		}

		public static LocalizedString ErrorCannotPinGroupIfNotAMember
		{
			get
			{
				return new LocalizedString("ErrorCannotPinGroupIfNotAMember", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInsufficientPermissionsToSync
		{
			get
			{
				return new LocalizedString("MessageInsufficientPermissionsToSync", "ExB32262", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsDelegatedForAccept
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsDelegatedForAccept", "Ex516DAB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidClientAccessTokenRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidClientAccessTokenRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarOccurrenceIndexIsOutOfRecurrenceRange
		{
			get
			{
				return new LocalizedString("ErrorCalendarOccurrenceIndexIsOutOfRecurrenceRange", "ExF31924", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageMissingUpdateDelegateRequestInformation
		{
			get
			{
				return new LocalizedString("MessageMissingUpdateDelegateRequestInformation", "ExEB2E7B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotOpenFileAttachment
		{
			get
			{
				return new LocalizedString("ErrorCannotOpenFileAttachment", "Ex51CFA6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidFolderId
		{
			get
			{
				return new LocalizedString("ErrorInvalidFolderId", "ExE497FB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyUpdateSentMessage
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertyUpdateSentMessage", "Ex2684A9", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToDefaultCalendarFolder
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToDefaultCalendarFolder", "Ex4652AD", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmServerMisConfigured
		{
			get
			{
				return new LocalizedString("IrmServerMisConfigured", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorRulesOverQuota
		{
			get
			{
				return new LocalizedString("RuleErrorRulesOverQuota", "Ex1804B4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNotAllowedExternalSharingByPolicy
		{
			get
			{
				return new LocalizedString("ErrorNotAllowedExternalSharingByPolicy", "ExA27DCB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotCreatePostItemInNonMailFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotCreatePostItemInNonMailFolder", "Ex98D95C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotEmptyCalendarOrSearchFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotEmptyCalendarOrSearchFolder", "Ex3E1847", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidParameter(string parameter)
		{
			return new LocalizedString("ErrorInvalidParameter", "", false, false, CoreResources.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString ErrorEmptyAggregatedAccountMailboxGuidStoredInSyncRequest
		{
			get
			{
				return new LocalizedString("ErrorEmptyAggregatedAccountMailboxGuidStoredInSyncRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExpiredSubscription
		{
			get
			{
				return new LocalizedString("ErrorExpiredSubscription", "Ex7DB92C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorODataAccessDisabled
		{
			get
			{
				return new LocalizedString("ErrorODataAccessDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotArchiveItemsInPublicFolders
		{
			get
			{
				return new LocalizedString("ErrorCannotArchiveItemsInPublicFolders", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAssociatedTraversalDisallowedWithQueryString
		{
			get
			{
				return new LocalizedString("ErrorAssociatedTraversalDisallowedWithQueryString", "Ex4414E5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsOrganizerForDecline
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsOrganizerForDecline", "ExC4D4B5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissingEmailAddressForManagedFolder
		{
			get
			{
				return new LocalizedString("ErrorMissingEmailAddressForManagedFolder", "Ex9BC5A7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGetSharingMetadataOnlyForMailbox
		{
			get
			{
				return new LocalizedString("ErrorGetSharingMetadataOnlyForMailbox", "Ex02B446", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageActingAsMustHaveRoutingType
		{
			get
			{
				return new LocalizedString("MessageActingAsMustHaveRoutingType", "Ex560094", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationAddItemToMyCalendar
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationAddItemToMyCalendar", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSyncFolderNotFound
		{
			get
			{
				return new LocalizedString("ErrorSyncFolderNotFound", "Ex84FE57", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSharingMessage
		{
			get
			{
				return new LocalizedString("ErrorInvalidSharingMessage", "Ex147FF8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidRequest
		{
			get
			{
				return new LocalizedString("descInvalidRequest", "ExF11EFC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedServiceConfigurationType
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedServiceConfigurationType", "Ex4E8D4A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorCreateWithRuleId
		{
			get
			{
				return new LocalizedString("RuleErrorCreateWithRuleId", "Ex39EAA8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadExtensionCustomPropertiesFailed
		{
			get
			{
				return new LocalizedString("LoadExtensionCustomPropertiesFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUserNotAllowedByPolicy
		{
			get
			{
				return new LocalizedString("ErrorUserNotAllowedByPolicy", "Ex7CA58E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImGroupLimitReached(int limit)
		{
			return new LocalizedString("ErrorImGroupLimitReached", "", false, false, CoreResources.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString MessageCouldNotGetWeatherData
		{
			get
			{
				return new LocalizedString("MessageCouldNotGetWeatherData", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageMultipleApplicationRolesNotSupported
		{
			get
			{
				return new LocalizedString("MessageMultipleApplicationRolesNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPropertyValidationFailure
		{
			get
			{
				return new LocalizedString("ErrorPropertyValidationFailure", "Ex75C34C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationCalendarViewAssociatedItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationCalendarViewAssociatedItem", "Ex5014BE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidUserPrincipalName
		{
			get
			{
				return new LocalizedString("ErrorInvalidUserPrincipalName", "ExCB3AC6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMissedNotificationEvents
		{
			get
			{
				return new LocalizedString("ErrorMissedNotificationEvents", "Ex1A0D20", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotRemoveAggregatedAccountMailbox
		{
			get
			{
				return new LocalizedString("ErrorCannotRemoveAggregatedAccountMailbox", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarUnableToUpdateMeetingRequest
		{
			get
			{
				return new LocalizedString("MessageCalendarUnableToUpdateMeetingRequest", "Ex6979E3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExecutingUserNotFound(string legacyDn)
		{
			return new LocalizedString("ExecutingUserNotFound", "", false, false, CoreResources.ResourceManager, new object[]
			{
				legacyDn
			});
		}

		public static LocalizedString ErrorInvalidValueForPropertyUserConfigurationPublicFolder
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyUserConfigurationPublicFolder", "Ex9996D2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderSave
		{
			get
			{
				return new LocalizedString("ErrorFolderSave", "Ex9A2CCC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageResolveNamesNotSufficientPermissionsToContactsFolder
		{
			get
			{
				return new LocalizedString("MessageResolveNamesNotSufficientPermissionsToContactsFolder", "Ex352183", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMissingForestConfiguration
		{
			get
			{
				return new LocalizedString("descMissingForestConfiguration", "ExCF96F0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedPathForSortGroup
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedPathForSortGroup", "Ex137B0E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorContainsFilterWrongType
		{
			get
			{
				return new LocalizedString("ErrorContainsFilterWrongType", "ExFD021E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxScopeNotAllowedWithoutQueryString
		{
			get
			{
				return new LocalizedString("ErrorMailboxScopeNotAllowedWithoutQueryString", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessageTrackingPermanentError
		{
			get
			{
				return new LocalizedString("ErrorMessageTrackingPermanentError", "ExB40310", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotDeleteObject
		{
			get
			{
				return new LocalizedString("ErrorCannotDeleteObject", "Ex621F13", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCallerHasNoAdminRoleGranted
		{
			get
			{
				return new LocalizedString("MessageCallerHasNoAdminRoleGranted", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIrmNotSupported
		{
			get
			{
				return new LocalizedString("ErrorIrmNotSupported", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReferenceLinkSharedFrom
		{
			get
			{
				return new LocalizedString("ReferenceLinkSharedFrom", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SentColon
		{
			get
			{
				return new LocalizedString("SentColon", "ExD86B1F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorActingAsUserNotUnique
		{
			get
			{
				return new LocalizedString("ErrorActingAsUserNotUnique", "ExBD39AF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchQueryHasTooManyKeywords
		{
			get
			{
				return new LocalizedString("ErrorSearchQueryHasTooManyKeywords", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderPropertyRequestFailed
		{
			get
			{
				return new LocalizedString("ErrorFolderPropertyRequestFailed", "ExF06095", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMimeContentInvalid
		{
			get
			{
				return new LocalizedString("ErrorMimeContentInvalid", "ExA584F2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSharingSynchronizationFailed
		{
			get
			{
				return new LocalizedString("ErrorSharingSynchronizationFailed", "Ex88386E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPublicFolderSearchNotSupportedOnMultipleFolders
		{
			get
			{
				return new LocalizedString("ErrorPublicFolderSearchNotSupportedOnMultipleFolders", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoFolderClassOverride
		{
			get
			{
				return new LocalizedString("ErrorNoFolderClassOverride", "Ex46F206", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedTypeForConversion
		{
			get
			{
				return new LocalizedString("ErrorUnsupportedTypeForConversion", "Ex083F2A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationDeclineItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationDeclineItem", "Ex8827A8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToSaveCalendarItem
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToSaveCalendarItem", "Ex5A0214", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRightsManagementException
		{
			get
			{
				return new LocalizedString("ErrorRightsManagementException", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOperationNotAllowedWithPublicFolderRoot
		{
			get
			{
				return new LocalizedString("ErrorOperationNotAllowedWithPublicFolderRoot", "ExFE1183", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdReturnedByResolveNames
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdReturnedByResolveNames", "Ex37182D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNoRequestType
		{
			get
			{
				return new LocalizedString("descNoRequestType", "ExCC601F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsOrganizerForTentative
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsOrganizerForTentative", "Ex9E7C5B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidVotingRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidVotingRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidProvisionDeviceType
		{
			get
			{
				return new LocalizedString("ErrorInvalidProvisionDeviceType", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorUnsupportedAddress
		{
			get
			{
				return new LocalizedString("RuleErrorUnsupportedAddress", "Ex269A81", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidCallStatus
		{
			get
			{
				return new LocalizedString("ErrorInvalidCallStatus", "Ex477F32", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSid
		{
			get
			{
				return new LocalizedString("ErrorInvalidSid", "Ex24BA8D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorManagedFoldersRootFailure
		{
			get
			{
				return new LocalizedString("ErrorManagedFoldersRootFailure", "Ex191FE3", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProxiedSubscriptionCallFailure
		{
			get
			{
				return new LocalizedString("ErrorProxiedSubscriptionCallFailure", "Ex4151CF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOccurrenceTimeSpanTooBig
		{
			get
			{
				return new LocalizedString("ErrorOccurrenceTimeSpanTooBig", "Ex444958", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCalendarInsufficientPermissionsToMoveCalendarItem
		{
			get
			{
				return new LocalizedString("MessageCalendarInsufficientPermissionsToMoveCalendarItem", "ExF09A79", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNewEventStreamConnectionOpened
		{
			get
			{
				return new LocalizedString("ErrorNewEventStreamConnectionOpened", "Ex8EDE18", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorArchiveMailboxNotEnabled
		{
			get
			{
				return new LocalizedString("ErrorArchiveMailboxNotEnabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SetGroupMailboxFailed(string name, string error)
		{
			return new LocalizedString("SetGroupMailboxFailed", "", false, false, CoreResources.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString ErrorCalendarCannotUseIdForOccurrenceId
		{
			get
			{
				return new LocalizedString("ErrorCalendarCannotUseIdForOccurrenceId", "ExA9D0B6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAccessDenied
		{
			get
			{
				return new LocalizedString("ErrorAccessDenied", "Ex83A27D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAttachmentSizeLimitExceeded
		{
			get
			{
				return new LocalizedString("ErrorAttachmentSizeLimitExceeded", "ExF8960C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPropertyUpdate
		{
			get
			{
				return new LocalizedString("ErrorPropertyUpdate", "Ex0FE6C0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorInvalidValue
		{
			get
			{
				return new LocalizedString("RuleErrorInvalidValue", "Ex08048C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidManagedFolderQuota
		{
			get
			{
				return new LocalizedString("ErrorInvalidManagedFolderQuota", "Ex37A451", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmRmsErrorMessage(string message)
		{
			return new LocalizedString("IrmRmsErrorMessage", "", false, false, CoreResources.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString ErrorCreateDistinguishedFolder
		{
			get
			{
				return new LocalizedString("ErrorCreateDistinguishedFolder", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ShowDetails
		{
			get
			{
				return new LocalizedString("ShowDetails", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ToColon
		{
			get
			{
				return new LocalizedString("ToColon", "ExC32F38", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCrossMailboxMoveCopy
		{
			get
			{
				return new LocalizedString("ErrorCrossMailboxMoveCopy", "Ex3023B2", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FlagForFollowUp
		{
			get
			{
				return new LocalizedString("FlagForFollowUp", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGetStreamingEventsProxy
		{
			get
			{
				return new LocalizedString("ErrorGetStreamingEventsProxy", "Ex05EBB5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSetCalendarPermissionOnNonCalendarFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotSetCalendarPermissionOnNonCalendarFolder", "Ex0996B8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SaveExtensionCustomPropertiesFailed
		{
			get
			{
				return new LocalizedString("SaveExtensionCustomPropertiesFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorConnectionFailed
		{
			get
			{
				return new LocalizedString("ErrorConnectionFailed", "ExB728B7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotUseLocalAccount
		{
			get
			{
				return new LocalizedString("ErrorCannotUseLocalAccount", "ExFA6E4F", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidOofParameter
		{
			get
			{
				return new LocalizedString("descInvalidOofParameter", "Ex0D2EB7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarSeekToConditionNotSupported(string s)
		{
			return new LocalizedString("ErrorCalendarSeekToConditionNotSupported", "", false, false, CoreResources.ResourceManager, new object[]
			{
				s
			});
		}

		public static LocalizedString ErrorTimeRangeIsTooLarge
		{
			get
			{
				return new LocalizedString("ErrorTimeRangeIsTooLarge", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAffectedTaskOccurrencesRequired
		{
			get
			{
				return new LocalizedString("ErrorAffectedTaskOccurrencesRequired", "Ex837516", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotGetAggregatedAccount
		{
			get
			{
				return new LocalizedString("ErrorCannotGetAggregatedAccount", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AADIdentityCreationFailed
		{
			get
			{
				return new LocalizedString("AADIdentityCreationFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateInputFolderNames
		{
			get
			{
				return new LocalizedString("ErrorDuplicateInputFolderNames", "ExDBA238", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageNonExistentMailboxSmtpAddress
		{
			get
			{
				return new LocalizedString("MessageNonExistentMailboxSmtpAddress", "Ex1798DB", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoveGroupMailboxFailed(string name, string error)
		{
			return new LocalizedString("RemoveGroupMailboxFailed", "", false, false, CoreResources.ResourceManager, new object[]
			{
				name,
				error
			});
		}

		public static LocalizedString ErrorIncorrectUpdatePropertyCount
		{
			get
			{
				return new LocalizedString("ErrorIncorrectUpdatePropertyCount", "ExA6A8AC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSerializedAccessToken
		{
			get
			{
				return new LocalizedString("ErrorInvalidSerializedAccessToken", "ExE9C1D8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRoutingType
		{
			get
			{
				return new LocalizedString("ErrorInvalidRoutingType", "Ex79E48E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSendMeetingInvitationsRequired
		{
			get
			{
				return new LocalizedString("ErrorSendMeetingInvitationsRequired", "Ex4BC018", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidIdNotAnItemAttachmentId
		{
			get
			{
				return new LocalizedString("ErrorInvalidIdNotAnItemAttachmentId", "ExAE23E4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RightsManagementInternalLicensingDisabled
		{
			get
			{
				return new LocalizedString("RightsManagementInternalLicensingDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageCannotUseItemAsRecipient
		{
			get
			{
				return new LocalizedString("MessageCannotUseItemAsRecipient", "Ex043558", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorItemSaveUserConfigurationExists
		{
			get
			{
				return new LocalizedString("ErrorItemSaveUserConfigurationExists", "Ex3673F5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidMailboxMailboxType
		{
			get
			{
				return new LocalizedString("MessageInvalidMailboxMailboxType", "Ex2F3F2E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsCancelledForDecline
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsCancelledForDecline", "ExCEB40C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorClientIntentInvalidStateDefinition
		{
			get
			{
				return new LocalizedString("ErrorClientIntentInvalidStateDefinition", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRetentionTagInvisible
		{
			get
			{
				return new LocalizedString("ErrorInvalidRetentionTagInvisible", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorItemSavePropertyError
		{
			get
			{
				return new LocalizedString("ErrorItemSavePropertyError", "Ex1E7CFA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetScopedTokenFailedWithInvalidScope
		{
			get
			{
				return new LocalizedString("GetScopedTokenFailedWithInvalidScope", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemForOperationRemoveItem
		{
			get
			{
				return new LocalizedString("ErrorInvalidItemForOperationRemoveItem", "Ex38D554", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorMessageClassificationNotFound
		{
			get
			{
				return new LocalizedString("RuleErrorMessageClassificationNotFound", "Ex66EBA5", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageUnableToLoadRBACSettings
		{
			get
			{
				return new LocalizedString("MessageUnableToLoadRBACSettings", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorQueryLanguageNotValid
		{
			get
			{
				return new LocalizedString("ErrorQueryLanguageNotValid", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Purple
		{
			get
			{
				return new LocalizedString("Purple", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMaxItemsToReturn
		{
			get
			{
				return new LocalizedString("InvalidMaxItemsToReturn", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PostedTo
		{
			get
			{
				return new LocalizedString("PostedTo", "Ex346A8E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangeServiceResponseErrorNoResponse
		{
			get
			{
				return new LocalizedString("ExchangeServiceResponseErrorNoResponse", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorPublicFolderOperationFailed
		{
			get
			{
				return new LocalizedString("ErrorPublicFolderOperationFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorBatchProcessingStopped
		{
			get
			{
				return new LocalizedString("ErrorBatchProcessingStopped", "ExECB924", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnifiedMessagingServerNotFound
		{
			get
			{
				return new LocalizedString("ErrorUnifiedMessagingServerNotFound", "ExA051A1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstantSearchNullFolderId
		{
			get
			{
				return new LocalizedString("InstantSearchNullFolderId", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWeatherServiceDisabled
		{
			get
			{
				return new LocalizedString("ErrorWeatherServiceDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNotEnoughPrivileges
		{
			get
			{
				return new LocalizedString("descNotEnoughPrivileges", "Ex44F71C", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarInvalidFirstDayOfWeek
		{
			get
			{
				return new LocalizedString("CalendarInvalidFirstDayOfWeek", "Ex36C37A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Red
		{
			get
			{
				return new LocalizedString("Red", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidExternalSharingSubscriber
		{
			get
			{
				return new LocalizedString("ErrorInvalidExternalSharingSubscriber", "Ex6048B7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotUseFolderIdForItemId
		{
			get
			{
				return new LocalizedString("ErrorCannotUseFolderIdForItemId", "Ex52BA80", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExchange14Required
		{
			get
			{
				return new LocalizedString("ErrorExchange14Required", "Ex0A199B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorProxyCallFailed
		{
			get
			{
				return new LocalizedString("ErrorProxyCallFailed", "Ex4796FE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorOrganizationNotFederated
		{
			get
			{
				return new LocalizedString("ErrorOrganizationNotFederated", "Ex8BFBA4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Blue
		{
			get
			{
				return new LocalizedString("Blue", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotDeleteSubfoldersOfMsgRootFolder
		{
			get
			{
				return new LocalizedString("ErrorCannotDeleteSubfoldersOfMsgRootFolder", "Ex630B38", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUpdatePropertyMismatch
		{
			get
			{
				return new LocalizedString("ErrorUpdatePropertyMismatch", "Ex203920", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIllegalCrossServerConnection
		{
			get
			{
				return new LocalizedString("ErrorIllegalCrossServerConnection", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorImListMigration
		{
			get
			{
				return new LocalizedString("ErrorImListMigration", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorResponseSchemaValidation
		{
			get
			{
				return new LocalizedString("ErrorResponseSchemaValidation", "Ex2527B1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerNotInSite
		{
			get
			{
				return new LocalizedString("ServerNotInSite", "ExA23656", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotAddAggregatedAccountToList
		{
			get
			{
				return new LocalizedString("ErrorCannotAddAggregatedAccountToList", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WhereColon
		{
			get
			{
				return new LocalizedString("WhereColon", "Ex306FE6", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidApprovalRequest
		{
			get
			{
				return new LocalizedString("ErrorInvalidApprovalRequest", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorIncorrectEncodedIdType
		{
			get
			{
				return new LocalizedString("ErrorIncorrectEncodedIdType", "ExD8826E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorGetRemoteArchiveItemFailed
		{
			get
			{
				return new LocalizedString("ErrorGetRemoteArchiveItemFailed", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidImGroupId
		{
			get
			{
				return new LocalizedString("ErrorInvalidImGroupId", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRequestUnknownMethodDebug
		{
			get
			{
				return new LocalizedString("ErrorInvalidRequestUnknownMethodDebug", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorBothViewFilterAndRestrictionNonNull
		{
			get
			{
				return new LocalizedString("ErrorBothViewFilterAndRestrictionNonNull", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotUseItemIdForFolderId
		{
			get
			{
				return new LocalizedString("ErrorCannotUseItemIdForFolderId", "Ex74D79D", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRequestedUser(string user)
		{
			return new LocalizedString("ErrorInvalidRequestedUser", "", false, false, CoreResources.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ErrorCannotDisableMandatoryExtension
		{
			get
			{
				return new LocalizedString("ErrorCannotDisableMandatoryExtension", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSyncStateData
		{
			get
			{
				return new LocalizedString("ErrorInvalidSyncStateData", "ExB49446", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSubmissionQuotaExceeded
		{
			get
			{
				return new LocalizedString("ErrorSubmissionQuotaExceeded", "Ex65324B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMessageDispositionRequired
		{
			get
			{
				return new LocalizedString("ErrorMessageDispositionRequired", "ExEC5273", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchScopeCannotHavePublicFolders
		{
			get
			{
				return new LocalizedString("ErrorSearchScopeCannotHavePublicFolders", "Ex1C5029", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRemoveDelegatesFailed
		{
			get
			{
				return new LocalizedString("ErrorRemoveDelegatesFailed", "ExB2F113", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPagingMaxRows
		{
			get
			{
				return new LocalizedString("ErrorInvalidPagingMaxRows", "ExC20F4A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorMissingParameter
		{
			get
			{
				return new LocalizedString("RuleErrorMissingParameter", "Ex28AE72", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLocationServicesInvalidQuery
		{
			get
			{
				return new LocalizedString("ErrorLocationServicesInvalidQuery", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageOccurrenceNotFound
		{
			get
			{
				return new LocalizedString("MessageOccurrenceNotFound", "ExBD686A", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSearchFolderNotInitialized
		{
			get
			{
				return new LocalizedString("ErrorSearchFolderNotInitialized", "Ex5E01EE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderScopeNotSpecified
		{
			get
			{
				return new LocalizedString("FolderScopeNotSpecified", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSubfilterType
		{
			get
			{
				return new LocalizedString("ErrorInvalidSubfilterType", "Ex7AD9DA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDuplicateUserIdsSpecified
		{
			get
			{
				return new LocalizedString("ErrorDuplicateUserIdsSpecified", "Ex5DC80E", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorDelegateMustBeCalendarEditorToGetMeetingMessages
		{
			get
			{
				return new LocalizedString("ErrorDelegateMustBeCalendarEditorToGetMeetingMessages", "Ex91BB7B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMismatchFolderId
		{
			get
			{
				return new LocalizedString("ErrorMismatchFolderId", "Ex7E49A8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPropertyDelete
		{
			get
			{
				return new LocalizedString("ErrorInvalidPropertyDelete", "Ex345314", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageActingAsMustHaveEmailAddress
		{
			get
			{
				return new LocalizedString("MessageActingAsMustHaveEmailAddress", "ExF281BC", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarIsCancelledForRemove
		{
			get
			{
				return new LocalizedString("ErrorCalendarIsCancelledForRemove", "Ex31BC28", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotResolveODataUrl
		{
			get
			{
				return new LocalizedString("ErrorCannotResolveODataUrl", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarEndDateIsEarlierThanStartDate
		{
			get
			{
				return new LocalizedString("ErrorCalendarEndDateIsEarlierThanStartDate", "ExBDF8D7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPercentCompleteValue
		{
			get
			{
				return new LocalizedString("ErrorInvalidPercentCompleteValue", "Ex04687B", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoApplicableProxyCASServersAvailable
		{
			get
			{
				return new LocalizedString("ErrorNoApplicableProxyCASServersAvailable", "Ex5046B0", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmProtectedVoicemailFeatureDisabled
		{
			get
			{
				return new LocalizedString("IrmProtectedVoicemailFeatureDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmExternalLicensingDisabled
		{
			get
			{
				return new LocalizedString("IrmExternalLicensingDisabled", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExchangeConfigurationException
		{
			get
			{
				return new LocalizedString("ErrorExchangeConfigurationException", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxMoveInProgress
		{
			get
			{
				return new LocalizedString("ErrorMailboxMoveInProgress", "ExC760FF", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidValueForPropertyXmlData
		{
			get
			{
				return new LocalizedString("ErrorInvalidValueForPropertyXmlData", "Ex68ECD8", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleErrorDuplicatedPriority
		{
			get
			{
				return new LocalizedString("RuleErrorDuplicatedPriority", "ExCAE182", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ItemNotExistInPurgesFolder
		{
			get
			{
				return new LocalizedString("ItemNotExistInPurgesFolder", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageMissingUserRolesForMailboxSearchRoleTypeApp
		{
			get
			{
				return new LocalizedString("MessageMissingUserRolesForMailboxSearchRoleTypeApp", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidNameForNameResolution
		{
			get
			{
				return new LocalizedString("ErrorInvalidNameForNameResolution", "Ex152E92", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRecipientSubfilterOrder
		{
			get
			{
				return new LocalizedString("ErrorInvalidRecipientSubfilterOrder", "ExE7B6F1", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorMailboxContainerGuidMismatch
		{
			get
			{
				return new LocalizedString("ErrorMailboxContainerGuidMismatch", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidId
		{
			get
			{
				return new LocalizedString("ErrorInvalidId", "Ex3371DA", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonPrimarySmtpAddress
		{
			get
			{
				return new LocalizedString("ErrorNonPrimarySmtpAddress", "Ex8143F7", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSharedFolderSearchNotSupportedOnMultipleFolders
		{
			get
			{
				return new LocalizedString("ErrorSharedFolderSearchNotSupportedOnMultipleFolders", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarInvalidRecurrence
		{
			get
			{
				return new LocalizedString("ErrorCalendarInvalidRecurrence", "ExF37915", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOperationSaveReplyForwardToPublicFolder
		{
			get
			{
				return new LocalizedString("ErrorInvalidOperationSaveReplyForwardToPublicFolder", "ExBAF833", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidOrderbyThenby
		{
			get
			{
				return new LocalizedString("ErrorInvalidOrderbyThenby", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidRetentionTagTypeMismatch
		{
			get
			{
				return new LocalizedString("ErrorInvalidRetentionTagTypeMismatch", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRequiredPropertyMissing
		{
			get
			{
				return new LocalizedString("ErrorRequiredPropertyMissing", "ExCBB0E4", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorParameterValueEmpty(string parameter)
		{
			return new LocalizedString("ErrorParameterValueEmpty", "", false, false, CoreResources.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString ErrorActiveDirectoryPermanentError
		{
			get
			{
				return new LocalizedString("ErrorActiveDirectoryPermanentError", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IrmRmsError
		{
			get
			{
				return new LocalizedString("IrmRmsError", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoPropertyUpdatesOrAttachmentsSpecified
		{
			get
			{
				return new LocalizedString("ErrorNoPropertyUpdatesOrAttachmentsSpecified", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationActionNeedFlagForFlagAction
		{
			get
			{
				return new LocalizedString("ConversationActionNeedFlagForFlagAction", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorAttachmentNestLevelLimitExceeded
		{
			get
			{
				return new LocalizedString("ErrorAttachmentNestLevelLimitExceeded", "", false, false, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidSmtpAddress
		{
			get
			{
				return new LocalizedString("ErrorInvalidSmtpAddress", "ExE9AFDE", false, true, CoreResources.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(CoreResources.IDs key)
		{
			return new LocalizedString(CoreResources.stringIDs[(uint)key], CoreResources.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(778);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Services.CoreResources", typeof(CoreResources).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			ErrorCannotSaveSentItemInArchiveFolder = 580413482U,
			ErrorMissingUserIdInformation = 844193848U,
			ErrorSearchConfigurationNotFound = 2524429953U,
			ErrorCannotCreateContactInNonContactFolder = 1087525243U,
			IrmFeatureDisabled = 1049269714U,
			EwsProxyResponseTooBig = 1795180790U,
			UpdateFavoritesUnableToDeleteFavoriteEntry = 3858003337U,
			ErrorUpdateDelegatesFailed = 754853510U,
			ErrorNoMailboxSpecifiedForSearchOperation = 1991349599U,
			ErrorCannotApplyHoldOperationOnDG = 1455468349U,
			ErrorInvalidExchangeImpersonationHeaderData = 1944820597U,
			ExOrganizerCannotCallUpdateCalendarItem = 1504384645U,
			IrmViewRightNotGranted = 2498511721U,
			UpdateNonDraftItemInDumpsterNotAllowed = 93006148U,
			ErrorIPGatewayNotFound = 2252936850U,
			ErrorInvalidPropertyForOperation = 2517173182U,
			ErrorNameResolutionNoResults = 574561672U,
			ErrorInvalidItemForOperationCreateItemAttachment = 4225005690U,
			Loading = 3599592070U,
			ErrorItemSave = 2339310738U,
			SubjectColon = 3413891549U,
			ErrorInvalidItemForOperationExpandDL = 2181052460U,
			MessageApplicationHasNoUserApplicationRoleAssigned = 1978429817U,
			ErrorCalendarIsCancelledMessageSent = 3167358706U,
			ErrorInvalidUserInfo = 1633083780U,
			ErrorCalendarViewRangeTooBig = 2945703152U,
			ErrorCalendarIsOrganizerForRemove = 495132450U,
			ErrorInvalidRecipientSubfilterComparison = 244533303U,
			ErrorPassingActingAsForUMConfig = 2476021338U,
			ErrorUserWithoutFederatedProxyAddress = 3060608191U,
			ErrorInvalidSendItemSaveSettings = 3825363766U,
			ErrorWrongServerVersion = 3533302998U,
			ErrorAssociatedTraversalDisallowedWithViewFilter = 3735354645U,
			ErrorMailboxHoldIsNotPermitted = 4045771774U,
			ErrorDuplicateSOAPHeader = 4197444273U,
			ErrorInvalidValueForPropertyUserConfigurationName = 2744667914U,
			ErrorIncorrectSchemaVersion = 3510999536U,
			ErrorImpersonationRequiredForPush = 143544280U,
			ErrorUnifiedMessagingPromptNotFound = 3135900505U,
			ErrorCalendarMeetingRequestIsOutOfDate = 3227656327U,
			MessageExtensionNotAllowedToCreateFAI = 1234709444U,
			ErrorFolderCorrupt = 2966054199U,
			ErrorManagedFolderNotFound = 2306155022U,
			MessageManagementRoleHeaderCannotUseWithOtherHeaders = 1701713067U,
			ErrorQueryFilterTooLong = 2285125742U,
			MessageApplicationUnableActAsUser = 630435929U,
			ErrorInvalidContactEmailIndex = 2886480659U,
			MessageMalformedSoapHeader = 1248433804U,
			ConversationItemQueryFailed = 3629808665U,
			ErrorADOperation = 4038759526U,
			ErrorCalendarIsOrganizerForAccept = 2633097826U,
			ErrorCannotDeleteTaskOccurrence = 3049158008U,
			ErrorTooManyContactsException = 2291046867U,
			ErrorReadEventsFailed = 3577190220U,
			descInvalidEIParameter = 1264061593U,
			ErrorDuplicateLegacyDistinguishedName = 3584287689U,
			MessageActingAsIsNotAValidEmailAddress = 2886782397U,
			MessageInvalidServerVersionForJsonRequest = 1703911099U,
			ErrorCalendarCannotMoveOrCopyOccurrence = 706889665U,
			ErrorPeopleConnectionNotFound = 1747094812U,
			ErrorCalendarMeetingIsOutOfDateResponseNotProcessedMessageSent = 3407017993U,
			ErrorInvalidExcludesRestriction = 2122949970U,
			ErrorMoreThanOneAccessModeSpecified = 789479259U,
			ErrorCreateSubfolderAccessDenied = 4062262029U,
			ErrorInvalidMailboxIdFormat = 1244610207U,
			ErrorCalendarIsCancelledForAccept = 275979752U,
			MessageApplicationRoleShouldPresentWhenUserRolePresent = 1596102169U,
			ErrorInvalidUMSubscriberDataTimeoutValue = 3078968203U,
			ErrorSearchTimeoutExpired = 602009568U,
			descLocalServerConfigurationRetrievalFailed = 3471859246U,
			ErrorInvalidContactEmailAddress = 3156759755U,
			ErrorInvalidValueForPropertyStringArrayDictionaryKey = 1000223261U,
			ErrorChangeKeyRequiredForWriteOperations = 3941855338U,
			ErrorMissingEmailAddress = 4767764U,
			ErrorFullSyncRequiredException = 932372376U,
			ErrorADSessionFilter = 2058899143U,
			ErrorDistinguishedUserNotSupported = 4170132598U,
			ErrorCrossForestCallerNeedsADObject = 3681363043U,
			ErrorSendMeetingInvitationsOrCancellationsRequired = 3422864683U,
			RuleErrorDuplicatedOperationOnTheSameRule = 349902350U,
			ErrorDeletePersonaOnInvalidFolder = 2151362503U,
			ErrorCannotAddAggregatedAccountMailbox = 1407341086U,
			ErrorExceededConnectionCount = 1108442436U,
			ErrorFolderSavePropertyError = 1153968262U,
			ErrorCannotUsePersonalContactsAsRecipientsOrAttendees = 590346139U,
			ErrorInvalidItemForForward = 4004906780U,
			ErrorChangeKeyRequired = 2538925974U,
			ErrorNotAcceptable = 147036963U,
			ErrorMessageTrackingNoSuchDomain = 2055932554U,
			ErrorTraversalNotAllowedWithoutQueryString = 1412463668U,
			ErrorOrganizationAccessBlocked = 3276585407U,
			ErrorInvalidNumberOfMailboxSearch = 3169826345U,
			ErrorCreateManagedFolderPartialCompletion = 727968988U,
			UpdateFavoritesUnableToRenameFavorite = 1486053208U,
			ErrorActiveDirectoryTransientError = 170634829U,
			ErrorInvalidSubscriptionRequestAllFoldersWithFolderIds = 2956059769U,
			ErrorInvalidOperationSendMeetingInvitationCancellationForPublicFolderItem = 2990730164U,
			ErrorIrresolvableConflict = 593146268U,
			ErrorInvalidItemForReplyAll = 479697568U,
			ErrorPhoneNumberNotDialable = 4266358168U,
			ErrorInvalidInternetHeaderChildNodes = 4187771604U,
			ErrorInvalidExpressionTypeForSubFilter = 3459701324U,
			MessageResolveNamesNotSufficientPermissionsToPrivateDLMember = 3931903304U,
			ErrorCannotSetNonCalendarPermissionOnCalendarFolder = 924348366U,
			ErrorParentFolderIdRequired = 2126704764U,
			ErrorEventNotFound = 1854021767U,
			ErrorVoiceMailNotImplemented = 3731723330U,
			ErrorDeleteDistinguishedFolder = 3448951775U,
			ErrorNoPermissionToSearchOrHoldMailbox = 2354781453U,
			ErrorExchangeApplicationNotEnabled = 3213161861U,
			ErrorResolveNamesInvalidFolderType = 1634698783U,
			ErrorExceededFindCountLimit = 2226715912U,
			MessageExtensionAccessActAsMailboxOnly = 2771555298U,
			ErrorPasswordChangeRequired = 3093510304U,
			ErrorInvalidManagedFolderProperty = 254805997U,
			ErrorInvalidIdMalformedEwsLegacyIdFormat = 1565810069U,
			ErrorSchemaViolation = 1538229710U,
			MessageInvalidMailboxContactAddressNotFound = 478602263U,
			ErrorInvalidIndexedPagingParameters = 1293185920U,
			ErrorUnsupportedPathForQuery = 361161677U,
			ErrorInvalidOperationDelegationAssociatedItem = 3721795127U,
			ErrorRemoteUserMailboxMustSpecifyExplicitLocalMailbox = 4256465912U,
			ErrorNoDestinationCASDueToVersionMismatch = 1473115829U,
			ErrorInvalidValueForPropertyBinaryData = 1805735881U,
			ErrorNotDelegate = 2410622290U,
			ErrorCalendarInvalidDayForTimeChangePattern = 1829860367U,
			ErrorInvalidPullSubscriptionId = 1988987848U,
			ErrorCannotCopyPublicFolderRoot = 2571138389U,
			MessageOperationRequiresUserContext = 1967767132U,
			ErrorPromptPublishingOperationFailed = 2217412679U,
			ErrorInvalidFractionalPagingParameters = 2620420056U,
			ErrorPublicFolderMailboxDiscoveryFailed = 4236561690U,
			ErrorUnableToRemoveImContactFromGroup = 3162641137U,
			ErrorSendMeetingCancellationsRequired = 1549704648U,
			MessageRecipientsArrayMustNotBeEmpty = 2011475698U,
			ErrorInvalidItemForOperationTentative = 757111886U,
			ErrorInvalidReferenceItem = 2519519915U,
			IrmReachNotConfigured = 1314141112U,
			ErrorMimeContentInvalidBase64String = 3907819958U,
			ErrorSentTaskRequestUpdate = 364289873U,
			ErrorFoundSyncRequestForNonAggregatedAccount = 1527356366U,
			MessagePropertyIsDeprecatedForThisVersion = 3384523424U,
			ErrorInvalidOperationContactsViewAssociatedItem = 3954262679U,
			ErrorServerBusy = 3655513582U,
			ConversationActionNeedRetentionPolicyTypeForSetRetentionPolicy = 3967405104U,
			ErrorCannotDeletePublicFolderRoot = 2671356913U,
			ErrorImGroupDisplayNameAlreadyExists = 3809605342U,
			NoServer = 384737734U,
			ErrorInvalidImDistributionGroupSmtpAddress = 948947750U,
			ErrorSubscriptionDelegateAccessNotSupported = 3640136739U,
			RuleErrorItemIsNotTemplate = 273046712U,
			ErrorCannotSetPermissionUnknownEntries = 2549623104U,
			MessageIdOrTokenTypeNotFound = 357277919U,
			ErrorLocationServicesDisabled = 2451443999U,
			MessageNotSupportedApplicationRole = 3773912990U,
			ErrorPublicFolderSyncException = 2636256287U,
			ErrorCalendarIsDelegatedForDecline = 1448063240U,
			ErrorUnsupportedODataRequest = 435342351U,
			ErrorDeepTraversalsNotAllowedOnPublicFolders = 4039615479U,
			MessageCouldNotFindWeatherLocationsForSearchString = 2521448946U,
			ErrorInvalidPropertyForSortBy = 2566235088U,
			MessageCalendarUnableToGetAssociatedCalendarItem = 3823874672U,
			ErrorSortByPropertyIsNotFoundOrNotSupported = 2841035169U,
			ErrorNotSupportedSharingMessage = 3991730990U,
			ErrorMissingInformationReferenceItemId = 1492851991U,
			ErrorInvalidSIPUri = 1825729465U,
			ErrorInvalidCompleteDateOutOfRange = 3371984686U,
			ErrorUnifiedMessagingDialPlanNotFound = 226219872U,
			MessageRecipientMustHaveRoutingType = 2688988465U,
			MessageResolveNamesNotSufficientPermissionsToPrivateDL = 3793759002U,
			MessageMissingUserRolesForOrganizationConfigurationRoleTypeApp = 1964352390U,
			ErrorInvalidUserSid = 368663972U,
			ErrorInvalidRecipientSubfilter = 1954916878U,
			ErrorSuffixSearchNotAllowed = 248293106U,
			ErrorUnifiedMessagingReportDataNotFound = 283029019U,
			UpdateFavoritesFolderAlreadyInFavorites = 1502984804U,
			MessageManagementRoleHeaderNotSupportedForOfficeExtension = 2329012714U,
			OneDriveProAttachmentDataProviderName = 2614511650U,
			ErrorCalendarInvalidAttributeValue = 2961161516U,
			MessageInvalidRecurrenceFormat = 3854873845U,
			ErrorInvalidAppApiVersionSupported = 2449079760U,
			ErrorInvalidManagedFolderSize = 4227165423U,
			ErrorTokenSerializationDenied = 3279473776U,
			ErrorInvalidRequest = 3784063568U,
			ErrorSubscriptionUnsubscribed = 2041209694U,
			ErrorInvalidItemForOperationCancelItem = 1426183245U,
			IrmCorruptProtectedMessage = 684230472U,
			ErrorCalendarIsGroupMailboxForAccept = 920557414U,
			ErrorMailboxSearchFailed = 2933656041U,
			ErrorMailboxConfiguration = 1188755898U,
			RuleErrorNotSettable = 382988367U,
			ErrorCopyPublicFolderNotSupported = 4177991609U,
			ErrorInvalidWatermark = 3312780993U,
			ErrorActingAsUserNotFound = 310545492U,
			ErrorDelegateMissingConfiguration = 3438146603U,
			MessageCalendarUnableToUpdateAssociatedCalendarItem = 1562596869U,
			MessageMissingMailboxOwnerEmailAddress = 2555117076U,
			ErrorSentMeetingRequestUpdate = 3080514177U,
			descInvalidTimeZone = 2141183275U,
			ErrorInvalidOperationDisposalTypeAssociatedItem = 1733132070U,
			UpdateFavoritesMoveTypeMustBeSet = 59180037U,
			ConversationActionNeedDeleteTypeForSetDeleteAction = 436889836U,
			ErrorInvalidProxySecurityContext = 3616451054U,
			ErrorInvalidValueForProperty = 570782166U,
			ErrorInvalidRestriction = 1904020973U,
			RuleErrorInvalidAddress = 2909492621U,
			RuleErrorSizeLessThanZero = 599310039U,
			Orange = 3664749912U,
			ErrorRecipientTypeNotSupported = 3611326890U,
			ErrorInvalidIdTooManyAttachmentLevels = 3632066599U,
			ErrorExportRemoteArchiveItemsFailed = 523664899U,
			ErrorCannotSendMessageFromPublicFolder = 1303377787U,
			MessageInsufficientPermissions = 3694049238U,
			MessageCorrelationFailed = 2611688746U,
			ErrorNoMailboxSpecifiedForHoldOperation = 3819492078U,
			ErrorTimeZone = 610144303U,
			ErrorSendAsDenied = 4260694481U,
			MessageSingleOrRecurringCalendarItemExpected = 4292861306U,
			ErrorSearchQueryCannotBeEmpty = 2226875331U,
			ErrorMultipleMailboxesCurrentlyNotSupported = 4136809189U,
			ErrorParentFolderNotFound = 4217637937U,
			ErrorDelegateCannotAddOwner = 143488278U,
			MessageCalendarInsufficientPermissionsToMoveMeetingCancellation = 3869946114U,
			ErrorImpersonateUserDenied = 73255155U,
			ErrorReadReceiptNotPending = 2875907804U,
			ErrorInvalidRetentionTagIdGuid = 1676008137U,
			ErrorCannotCreateTaskInNonTaskFolder = 379663703U,
			MessageNonExistentMailboxNoSmtpAddress = 4074099229U,
			ErrorSchemaValidation = 2523006528U,
			MessageManagementRoleHeaderValueNotApplicable = 3264410200U,
			MessageInvalidRuleVersion = 2540872182U,
			ErrorUnsupportedMimeConversion = 1174046717U,
			ErrorCannotMovePublicFolderItemOnDelete = 463452338U,
			ErrorInvalidItemForOperationArchiveItem = 966537145U,
			ErrorInvalidSearchQuerySyntax = 3021008902U,
			ErrorInvalidValueForCountSystemQueryOption = 4179066588U,
			ErrorFolderSaveFailed = 1067402124U,
			MessageTargetMailboxNotInRoleScope = 2435663882U,
			ErrorInvalidSearchId = 2179607746U,
			ErrorInvalidOperationSyncFolderHierarchyForPublicFolder = 2674546476U,
			ErrorItemCorrupt = 2624402344U,
			ErrorServerTemporaryUnavailable = 3120707856U,
			ErrorCannotArchiveCalendarContactTaskFolderException = 2786380669U,
			ErrorInvalidItemForOperationSendItem = 4123291671U,
			ErrorAggregatedAccountAlreadyExists = 68528320U,
			ErrorInvalidServerVersion = 109614196U,
			ErrorGroupingNonNullWithSuggestionsViewFilter = 1487884331U,
			MessageInvalidMailboxNotPrivateDL = 1958477060U,
			ErrorItemPropertyRequestFailed = 1272021886U,
			ConversationActionNeedDestinationFolderForCopyAction = 1706062739U,
			ErrorLocationServicesRequestFailed = 2653243941U,
			UnrecognizedDistinguishedFolderName = 220777420U,
			ErrorInvalidSubfilterTypeNotRecipientType = 559784827U,
			ErrorInvalidPropertySet = 1701761470U,
			UpdateFavoritesFolderCannotBeNull = 3625531057U,
			ErrorCannotRemoveAggregatedAccountFromList = 1326676491U,
			ErrorProxyTokenExpired = 3699987394U,
			ErrorCannotCreateCalendarItemInNonCalendarFolder = 3564002022U,
			ErrorInvalidOperationGroupByAssociatedItem = 3732945645U,
			MessageCalendarUnableToCreateAssociatedCalendarItem = 2890836210U,
			ErrorMultiLegacyMailboxAccess = 896367800U,
			ErrorUnifiedMailboxAlreadyExists = 3392207806U,
			ErrorInvalidPropertyAppend = 3619206730U,
			ErrorObjectTypeChanged = 4261845811U,
			ErrorSearchableObjectNotFound = 4252616528U,
			ErrorEndTimeMustBeGreaterThanStartTime = 2498507918U,
			ErrorInvalidFederatedOrganizationId = 765833303U,
			MessageExtensionNotAllowedToUpdateFAI = 1583798271U,
			ErrorValueOutOfRange = 1335290147U,
			ErrorNotEnoughMemory = 3719196410U,
			ErrorInvalidExtendedPropertyValue = 3635256568U,
			ErrorMoveCopyFailed = 2524108663U,
			GetClientExtensionTokenFailed = 1985973150U,
			ErrorVirusDetected = 3705244005U,
			ErrorInvalidVotingResponse = 671866695U,
			RuleErrorInboxRulesValidationError = 2296308088U,
			ErrorInvalidIdMonikerTooLong = 1897020671U,
			ErrorMultipleSearchRootsDisallowedWithSearchContext = 111518940U,
			ErrorUserNotUnifiedMessagingEnabled = 4142344047U,
			ErrorCannotMovePublicFolderToPrivateMailbox = 3206878473U,
			ConversationActionAlwaysMoveNoPublicFolder = 751424501U,
			ErrorCallerIsInvalidADAccount = 1834319386U,
			ErrorNoDestinationCASDueToSSLRequirements = 3319799507U,
			ErrorInternalServerTransientError = 3995283118U,
			ErrorInvalidParentFolder = 3659985571U,
			ErrorArchiveFolderPathCreation = 2565659540U,
			MessageCalendarInsufficientPermissionsToMoveItem = 4066246803U,
			ErrorMessagePerFolderCountReceiveQuotaExceeded = 2791864679U,
			ErrorDateTimeNotInUTC = 2643283981U,
			ErrorInvalidAttachmentSubfilter = 2798800298U,
			ErrorUserConfigurationDictionaryNotExist = 2214456911U,
			FromColon = 2918743951U,
			ErrorInvalidSubscriptionRequestNoFolderIds = 2362895530U,
			ErrorCallerIsComputerAccount = 1115854773U,
			ErrorDeleteItemsFailed = 69571280U,
			ErrorNotApplicableOutsideOfDatacenter = 2014273875U,
			RuleErrorOutlookRuleBlobExists = 526527128U,
			descInvalidOofRequestPublicFolder = 2843974690U,
			ErrorMailboxIsNotPartOfAggregatedMailboxes = 21808504U,
			ErrorInvalidRetentionTagNone = 2043815785U,
			MessageInvalidRoleTypeString = 2448725207U,
			MessageInvalidMailboxRecipientNotFoundInActiveDirectory = 2343198056U,
			ErrorNoSyncRequestsMatchedSpecifiedEmailAddress = 402485116U,
			ErrorInvalidDestinationFolderForPostItem = 2999374145U,
			ErrorGetRemoteArchiveFolderFailed = 377617137U,
			RightsManagementMailboxOnlySupport = 3410698111U,
			ErrorMissingItemForCreateItemAttachment = 122085112U,
			ErrorFindRemoteArchiveFolderFailed = 4160418372U,
			ErrorCalendarFolderIsInvalidForCalendarView = 2989650895U,
			ErrorFindConversationNotSupportedForPublicFolders = 3359997542U,
			ErrorUserConfigurationBinaryDataNotExist = 969219158U,
			DefaultHtmlAttachmentHrefText = 1063299331U,
			Green = 3510846499U,
			ErrorItemNotFound = 4005418156U,
			ErrorCannotEmptyFolder = 2838198776U,
			Yellow = 777220966U,
			ErrorInvalidSubscription = 1967895810U,
			ErrorSchemaValidationColon = 4281412187U,
			ErrorDelegateNoUser = 707372475U,
			RuleErrorMissingRangeValue = 107796140U,
			MessageWebMethodUnavailable = 2554577046U,
			ErrorUnsupportedQueryFilter = 395078619U,
			ErrorCannotMovePublicFolderOnDelete = 968334519U,
			ErrorAccessModeSpecified = 3314483401U,
			ErrorInvalidPhotoSize = 654139516U,
			ErrorMultipleMailboxSearchNotSupported = 2656700117U,
			MessageManagementRoleHeaderNotSupportedForPartnerIdentity = 728324719U,
			ConversationActionInvalidFolderType = 1457631314U,
			ErrorUnsupportedSubFilter = 259054457U,
			ErrorInvalidComplianceId = 1408093181U,
			ErrorCalendarCannotUpdateDeletedItem = 3843271914U,
			ErrorInvalidOperationDistinguishedGroupByAssociatedItem = 3782996725U,
			ErrorInvalidDelegatePermission = 3537364541U,
			ErrorInternalServerError = 594155080U,
			ErrorNoPublicFolderServerAvailable = 2356362688U,
			ErrorInvalidPhoneCallId = 3978299680U,
			ErrorInvalidGetSharingFolderRequest = 1793222072U,
			ErrorCannotResolveOrganizationName = 2927988853U,
			ErrorUnsupportedCulture = 809187661U,
			ErrorInvalidChangeKey = 865206910U,
			ErrorMimeContentConversionFailed = 3846347532U,
			ErrorResolveNamesOnlyOneContactsFolderAllowed = 2683464521U,
			ErrorInvalidSchemaVersionForMailboxVersion = 901489999U,
			ErrorInvalidRequestQuotaExceeded = 2012012473U,
			MessageTokenRequestUnauthorized = 768426321U,
			MessageUserRoleNotApplicableForAppOnlyToken = 3910111167U,
			ErrorInvalidValueForPropertyKeyValueConversion = 828992378U,
			ErrorInvalidRetentionTagInheritance = 3769371271U,
			Conversation = 710925581U,
			ErrorCannotCreateUnifiedMailbox = 1338511205U,
			ErrorMailTipsDisabled = 1897429247U,
			ErrorMissingItemIdForCreateItemAttachment = 4222588379U,
			ErrorInvalidMailbox = 1762041369U,
			ErrorDelegateValidationFailed = 4097108255U,
			ErrorUserPromptNeeded = 2715027708U,
			RuleErrorMissingAction = 1898482716U,
			ErrorApplyConversationActionFailed = 706596508U,
			ErrorInsufficientResources = 60783832U,
			ErrorActingAsRequired = 905739673U,
			ErrorCalendarInvalidDayForWeeklyRecurrence = 2681298929U,
			ErrorMissingInformationEmailAddress = 549150802U,
			UpdateFavoritesFavoriteNotFound = 4019544117U,
			ErrorCalendarDurationIsTooLong = 2484699530U,
			ErrorNoRespondingCASInDestinationSite = 4252309617U,
			ErrorInvalidRecipients = 388443881U,
			ErrorAppendBodyTypeMismatch = 269577600U,
			ErrorDistributionListMemberNotExist = 514021796U,
			ErrorRequestTimeout = 3285224352U,
			MessageApplicationHasNoRoleAssginedWhichUserHas = 3607262778U,
			ErrorArchiveMailboxGetConversationFailed = 3668888236U,
			ErrorClientIntentNotFound = 2851949310U,
			ErrorNonExistentMailbox = 2489326695U,
			ErrorVirusMessageDeleted = 1164605313U,
			ErrorCannotFindUnifiedMailbox = 175403818U,
			ErrorUnifiedMailboxSupportedOnlyWithMicrosoftAccount = 1505256501U,
			GroupMailboxCreationFailed = 2833024077U,
			ErrorInvalidSearchQueryLength = 1233823477U,
			ErrorCalendarInvalidPropertyState = 391940363U,
			ErrorAddDelegatesFailed = 1850561764U,
			CcColon = 3496891301U,
			ErrorCrossSiteRequest = 1062691260U,
			ErrorPublicFolderUserMustHaveMailbox = 565625999U,
			ErrorMessageTrackingTransientError = 3399410586U,
			ErrorToFolderNotFound = 1027490726U,
			ErrorDeleteUnifiedMessagingPromptFailed = 1637412134U,
			UpdateFavoritesUnableToMoveFavorite = 3914315493U,
			ErrorPeopleConnectionNoToken = 3141449171U,
			ErrorCannotSpecifySearchFolderAsSourceFolder = 3848937923U,
			ErrorEmailAddressMismatch = 933080956U,
			ErrorUserConfigurationXmlDataNotExist = 2419720676U,
			ErrorUnifiedMessagingRequestFailed = 2346704662U,
			ErrorCreateItemAccessDenied = 1075303082U,
			RuleErrorFolderDoesNotExist = 2887245343U,
			ErrorInvalidImContactId = 3485828594U,
			ErrorNoPropertyTagForCustomProperties = 3969305989U,
			SentTime = 2677919833U,
			MessageNonExistentMailboxGuid = 3279543955U,
			ErrorMaxRequestedUnifiedGroupsSetsExceeded = 216781884U,
			ErrorInvalidAppSchemaVersionSupported = 3555230765U,
			ErrorInvalidLogonType = 3522975510U,
			MessageActAsUserRequiredForSuchApplicationRole = 131857255U,
			ErrorCalendarOutOfRange = 3773356320U,
			ErrorContentIndexingNotEnabled = 3975089319U,
			ErrorContentConversionFailed = 4227151856U,
			ConversationIdNotSupported = 3426540703U,
			ConversationSupportedOnlyForMailboxSession = 730941518U,
			ErrorMoveDistinguishedFolder = 3771523283U,
			ErrorMailboxCannotBeSpecifiedForPublicFolderRoot = 2092164778U,
			IrmPreLicensingFailure = 2805212767U,
			MessageMissingUserRolesForLegalHoldRoleTypeApp = 734136355U,
			ErrorMailboxVersionNotSupported = 430009573U,
			ErrorRestrictionTooComplex = 560492804U,
			RuleErrorRecipientDoesNotExist = 3546363172U,
			ErrorInvalidAggregatedAccountCredentials = 3667869681U,
			descInvalidSecurityContext = 2653688977U,
			MessagePublicFoldersNotSupportedForNonIndexable = 213621866U,
			ErrorInvalidFilterNode = 3943930965U,
			ErrorIrmUserRightNotGranted = 1508237301U,
			descInvalidRequestType = 3956968185U,
			DowaNotProvisioned = 184315686U,
			ErrorRecurrenceEndDateTooBig = 2652436543U,
			ErrorInvalidItemForReply = 629782913U,
			UpdateFavoritesInvalidUpdateFavoriteOperationType = 1342320011U,
			ErrorInvalidManagementRoleHeader = 2674011741U,
			ErrorCannotGetExternalEcpUrl = 200462199U,
			ErrorCannotCreateSearchFolderInPublicFolder = 1645715101U,
			RuleErrorUnsupportedRule = 3288028209U,
			ErrorMissingManagedFolderId = 2518142400U,
			MessageInsufficientPermissionsToSend = 1990145025U,
			ErrorInvalidCompleteDate = 3098927940U,
			ErrorSearchFolderTimeout = 2447591155U,
			ErrorCannotSetAggregatedAccount = 4089853131U,
			ErrorInvalidPushSubscriptionUrl = 1962425675U,
			ErrorCannotAddAggregatedAccount = 1669051638U,
			ErrorCalendarIsGroupMailboxForDecline = 1718538996U,
			ErrorNameResolutionNoMailbox = 761574210U,
			ErrorCannotArchiveItemsInArchiveMailbox = 2225772284U,
			MowaNotProvisioned = 556297389U,
			ErrorInvalidOperationSendAndSaveCopyToPublicFolder = 529689091U,
			ConversationActionNeedDestinationFolderForMoveAction = 1998574567U,
			ErrorViewFilterRequiresSearchContext = 718120058U,
			ErrorDelegateAlreadyExists = 1363870753U,
			ErrorSubmitQueryBasedHoldTaskFailed = 2479091638U,
			ErrorPeopleConnectFailedToReadApplicationConfiguration = 2869245557U,
			ErrorUnsupportedMapiPropertyType = 937093447U,
			ErrorApprovalRequestAlreadyDecided = 1829541172U,
			MessageCouldNotFindWeatherLocations = 472949644U,
			WhenColon = 3770755973U,
			ErrorNoGroupingForQueryString = 70508874U,
			ErrorInvalidIdStoreObjectIdTooLong = 2651121857U,
			ErrorQuotaExceeded = 3654265673U,
			ConversationActionNeedReadStateForSetReadStateAction = 3601113588U,
			ErrorLocationServicesRequestTimedOut = 4226485813U,
			ErrorCalendarInvalidPropertyValue = 3349192959U,
			ErrorManagedFolderAlreadyExists = 978558141U,
			ErrorLocationServicesInvalidSource = 1008089967U,
			OnPremiseSynchorizedDiscoverySearch = 2560374358U,
			ErrorInvalidOperationForAssociatedItems = 3859804741U,
			ErrorCorruptData = 953197733U,
			ErrorCalendarInvalidTimeZone = 39525862U,
			ErrorInvalidOperationMessageDispositionAssociatedItem = 3281131813U,
			ErrorSubscriptionAccessDenied = 2662672540U,
			ErrorCannotReadRequestBody = 2608213760U,
			ErrorNameResolutionMultipleResults = 2070630207U,
			ErrorInvalidExtendedProperty = 866480793U,
			EwsProxyCannotGetCredentials = 3760366944U,
			UpdateFavoritesInvalidMoveFavoriteRequest = 2761096871U,
			ErrorInvalidPermissionSettings = 1359116179U,
			ErrorProxyServiceDiscoveryFailed = 1645280882U,
			ErrorInvalidItemForOperationAcceptItem = 598450895U,
			ErrorInvalidValueForPropertyDuplicateDictionaryKey = 2578390262U,
			ErrorExceededSubscriptionCount = 516980747U,
			ErrorPermissionNotAllowedByPolicy = 739553585U,
			MessageInsufficientPermissionsToSubscribe = 48346381U,
			ErrorInvalidValueForPropertyDate = 1803820018U,
			ErrorUnsupportedRecurrence = 3322365201U,
			ErrorUserADObjectNotFound = 837503410U,
			ErrorCannotAttachSelf = 2020376324U,
			ErrorMissingInformationSharingFolderId = 2938284467U,
			ErrorCannotSetFromOnMeetingResponse = 1762596806U,
			MessageInvalidOperationForPublicFolderItemsAddParticipantByItemId = 3795663900U,
			ErrorInvalidItemForOperationCreateItem = 1439158331U,
			ErrorInvalidPropertyForExists = 3788524313U,
			ErrorCannotSaveSentItemInPublicFolder = 792522617U,
			ErrorRestrictionTooLong = 3143473274U,
			ErrorUnsupportedPropertyDefinition = 789094727U,
			SharePointCreationFailed = 3311760175U,
			ErrorDataSizeLimitExceeded = 2935460503U,
			ErrorFolderExists = 628559436U,
			ErrorUnifiedGroupAlreadyExists = 2930851601U,
			MessageApplicationTokenOnly = 1580647852U,
			ErrorSharingNoExternalEwsAvailable = 4047718788U,
			RuleErrorEmptyValueFound = 1661960732U,
			ErrorOccurrenceCrossingBoundary = 852852329U,
			ErrorArchiveMailboxServiceDiscoveryFailed = 3156121664U,
			ErrorInvalidAttachmentSubfilterTextFilter = 1064353045U,
			ErrorGetSharingMetadataNotSupported = 1966896516U,
			MessageRecipientMustHaveEmailAddress = 1507828071U,
			ErrorInvalidRecipientSubfilterTextFilter = 4094604515U,
			ErrorInvalidPropertyRequest = 3673396595U,
			ErrorCalendarIsNotOrganizer = 1582215140U,
			ErrorInvalidProvisionDeviceID = 3374101509U,
			MessageCouldNotGetWeatherDataForLocation = 4117821571U,
			ErrorTimeProposalMissingStartOrEndTimeError = 2326085984U,
			ErrorInvalidSubfilterTypeNotAttendeeType = 1946206036U,
			PropertyCommandNotSupportSet = 3890629732U,
			ErrorImpersonationFailed = 347738787U,
			ErrorSubscriptionNotFound = 2884324330U,
			MessageCalendarInsufficientPermissionsToMoveMeetingRequest = 2225154662U,
			ErrorInvalidIdMalformed = 3107705007U,
			ErrorCalendarIsGroupMailboxForSuppressReadReceipt = 1035957819U,
			ErrorCannotGetSourceFolderPath = 2940401781U,
			ErrorWildcardAndGroupExpansionNotAllowed = 4083587704U,
			UnsupportedInlineAttachmentContentType = 4077357270U,
			RuleErrorUnexpectedError = 1170272727U,
			MessageCalendarInsufficientPermissionsToDraftsFolder = 1601473907U,
			ErrorADUnavailable = 634294555U,
			ErrorInvalidPhoneNumber = 3260461220U,
			ErrorSoftDeletedTraversalsNotAllowedOnPublicFolders = 547900838U,
			ErrorCalendarIsDelegatedForTentative = 2687301200U,
			ErrorFoldersMustBelongToSameMailbox = 2952942328U,
			ErrorDataSourceOperation = 2697731302U,
			ErrorCalendarMeetingIsOutOfDateResponseNotProcessed = 3573754788U,
			MessageInvalidIdMalformedEwsIdFormat = 1475709851U,
			ErrorPreviousPageNavigationCurrentlyNotSupported = 3699315399U,
			ErrorCannotEmptyPublicFolderToDeletedItems = 2058507107U,
			ErrorInvalidSharingData = 1014449457U,
			MessageCalendarInsufficientPermissionsToMeetingMessageFolder = 1746698887U,
			ErrorInvalidOperationCannotSpecifyItemId = 2503843052U,
			ErrorCalendarIsGroupMailboxForTentative = 3187786876U,
			ErrorMessageSizeExceeded = 2913173341U,
			InvalidDateTimePrecisionValue = 3468080577U,
			ErrorStaleObject = 3943872330U,
			UpdateFavoritesUnableToAddFolderToFavorites = 3119664543U,
			ErrorPasswordExpired = 1282299710U,
			ErrorInvalidOperationCannotPerformOperationOnADRecipients = 3142918589U,
			ErrorTooManyObjectsOpened = 157861094U,
			MessageInvalidMailboxInvalidReferencedItem = 256399585U,
			MessageApplicationHasNoGivenRoleAssigned = 3901728717U,
			MessageRecipientsArrayTooLong = 3113724054U,
			ErrorInvalidIdXml = 3852956793U,
			ErrorCallerWithoutMailboxCannotUseSendOnly = 2271901695U,
			ErrorArchiveMailboxSearchFailed = 2535285679U,
			PostedOn = 2543409328U,
			ErrorInvalidExternalSharingInitiator = 4028591235U,
			ErrorMailboxStoreUnavailable = 1627983613U,
			ErrorInvalidCalendarViewRestrictionOrSort = 2358398289U,
			ErrorSavedItemFolderNotFound = 3610830273U,
			ErrorCalendarOccurrenceIsDeletedFromRecurrence = 3335161738U,
			ErrorMissingRecipients = 2985674644U,
			ErrorTimeProposalInvalidInCreateItemRequest = 3997746891U,
			ErrorCalendarIsDelegatedForRemove = 2990436390U,
			ErrorInvalidLikeRequest = 4151155219U,
			MessageRecurrenceStartDateTooSmall = 271991716U,
			ErrorUnknownTimeZone = 4066404319U,
			ErrorProxyGroupSidLimitExceeded = 1656583547U,
			ErrorCannotRemoveAggregatedAccount = 2834376775U,
			ErrorInvalidShape = 1816334244U,
			ErrorInvalidLicense = 1812149170U,
			ErrorAccountDisabled = 531497785U,
			ErrorHoldIsNotFound = 1949840710U,
			MessageMessageIsNotDraft = 1830098328U,
			ErrorWrongServerVersionDelegate = 3778961523U,
			OnBehalfOf = 2868846894U,
			ErrorInvalidOperationForPublicFolderItems = 1902653190U,
			ErrorCalendarCannotUseIdForRecurringMasterId = 1069471396U,
			ErrorInvalidSubscriptionRequest = 3647226175U,
			ErrorInvalidIdEmpty = 4226852029U,
			ErrorInvalidAttachmentId = 922305341U,
			ErrorBothQueryStringAndRestrictionNonNull = 2329210449U,
			RuleErrorRuleNotFound = 1725658743U,
			ErrorDiscoverySearchesDisabled = 1277300954U,
			ErrorCalendarIsCancelledForTentative = 1147653914U,
			ErrorRecurrenceHasNoOccurrence = 1564162812U,
			MessageNonExistentMailboxLegacyDN = 103255531U,
			ErrorNoDestinationCASDueToKerberosRequirements = 3137087456U,
			ErrorFolderNotFound = 3395659933U,
			ErrorCannotPinGroupIfNotAMember = 2923349632U,
			MessageInsufficientPermissionsToSync = 2118011096U,
			ErrorCalendarIsDelegatedForAccept = 2483737250U,
			ErrorInvalidClientAccessTokenRequest = 2958727324U,
			ErrorCalendarOccurrenceIndexIsOutOfRecurrenceRange = 2006869741U,
			MessageMissingUpdateDelegateRequestInformation = 124559532U,
			ErrorCannotOpenFileAttachment = 492857424U,
			ErrorInvalidFolderId = 234107130U,
			ErrorInvalidPropertyUpdateSentMessage = 2141227684U,
			MessageCalendarInsufficientPermissionsToDefaultCalendarFolder = 1686445652U,
			IrmServerMisConfigured = 784482022U,
			RuleErrorRulesOverQuota = 357046427U,
			ErrorNotAllowedExternalSharingByPolicy = 2890296403U,
			ErrorCannotCreatePostItemInNonMailFolder = 3792171687U,
			ErrorCannotEmptyCalendarOrSearchFolder = 3080652515U,
			ErrorEmptyAggregatedAccountMailboxGuidStoredInSyncRequest = 3504612180U,
			ErrorExpiredSubscription = 3329761676U,
			ErrorODataAccessDisabled = 3795993851U,
			ErrorCannotArchiveItemsInPublicFolders = 3558192788U,
			ErrorAssociatedTraversalDisallowedWithQueryString = 908888675U,
			ErrorCalendarIsOrganizerForDecline = 2980490932U,
			ErrorMissingEmailAddressForManagedFolder = 1228157268U,
			ErrorGetSharingMetadataOnlyForMailbox = 1105778474U,
			MessageActingAsMustHaveRoutingType = 2292082652U,
			ErrorInvalidOperationAddItemToMyCalendar = 473053729U,
			ErrorSyncFolderNotFound = 1581442160U,
			ErrorInvalidSharingMessage = 471235856U,
			descInvalidRequest = 1735870649U,
			ErrorUnsupportedServiceConfigurationType = 3640136612U,
			RuleErrorCreateWithRuleId = 918293667U,
			LoadExtensionCustomPropertiesFailed = 3053756532U,
			ErrorUserNotAllowedByPolicy = 311942179U,
			MessageCouldNotGetWeatherData = 2933471333U,
			MessageMultipleApplicationRolesNotSupported = 2335200077U,
			ErrorPropertyValidationFailure = 3967923828U,
			ErrorInvalidOperationCalendarViewAssociatedItem = 3789879302U,
			ErrorInvalidUserPrincipalName = 266941361U,
			ErrorMissedNotificationEvents = 124305755U,
			ErrorCannotRemoveAggregatedAccountMailbox = 3635708019U,
			MessageCalendarUnableToUpdateMeetingRequest = 2102429258U,
			ErrorInvalidValueForPropertyUserConfigurationPublicFolder = 3014743008U,
			ErrorFolderSave = 3867216855U,
			MessageResolveNamesNotSufficientPermissionsToContactsFolder = 2034497546U,
			descMissingForestConfiguration = 1790760926U,
			ErrorUnsupportedPathForSortGroup = 1103930166U,
			ErrorContainsFilterWrongType = 3836413508U,
			ErrorMailboxScopeNotAllowedWithoutQueryString = 1270269734U,
			ErrorMessageTrackingPermanentError = 2084976894U,
			ErrorCannotDeleteObject = 3912965805U,
			MessageCallerHasNoAdminRoleGranted = 1670145257U,
			ErrorIrmNotSupported = 2692292357U,
			ReferenceLinkSharedFrom = 707914022U,
			SentColon = 295620541U,
			ErrorActingAsUserNotUnique = 1479620947U,
			ErrorSearchQueryHasTooManyKeywords = 3032287327U,
			ErrorFolderPropertyRequestFailed = 2370747299U,
			ErrorMimeContentInvalid = 1795652632U,
			ErrorSharingSynchronizationFailed = 3469371317U,
			ErrorPublicFolderSearchNotSupportedOnMultipleFolders = 2109751382U,
			ErrorNoFolderClassOverride = 3753602229U,
			ErrorUnsupportedTypeForConversion = 836749070U,
			ErrorInvalidItemForOperationDeclineItem = 1535520491U,
			MessageCalendarInsufficientPermissionsToSaveCalendarItem = 3563948173U,
			ErrorRightsManagementException = 237462827U,
			ErrorOperationNotAllowedWithPublicFolderRoot = 2440725179U,
			ErrorInvalidIdReturnedByResolveNames = 189525348U,
			descNoRequestType = 2194994953U,
			ErrorCalendarIsOrganizerForTentative = 3371251772U,
			ErrorInvalidVotingRequest = 2253496121U,
			ErrorInvalidProvisionDeviceType = 832351692U,
			RuleErrorUnsupportedAddress = 2176189925U,
			ErrorInvalidCallStatus = 1093593955U,
			ErrorInvalidSid = 2718542415U,
			ErrorManagedFoldersRootFailure = 2580909644U,
			ErrorProxiedSubscriptionCallFailure = 1569114342U,
			ErrorOccurrenceTimeSpanTooBig = 1214042036U,
			MessageCalendarInsufficientPermissionsToMoveCalendarItem = 1110621977U,
			ErrorNewEventStreamConnectionOpened = 2943900075U,
			ErrorArchiveMailboxNotEnabled = 2054881972U,
			ErrorCalendarCannotUseIdForOccurrenceId = 4180336284U,
			ErrorAccessDenied = 3579904699U,
			ErrorAttachmentSizeLimitExceeded = 1721078306U,
			ErrorPropertyUpdate = 1912743644U,
			RuleErrorInvalidValue = 842243550U,
			ErrorInvalidManagedFolderQuota = 2756368512U,
			ErrorCreateDistinguishedFolder = 304669716U,
			ShowDetails = 3684919469U,
			ToColon = 3465339554U,
			ErrorCrossMailboxMoveCopy = 2832845860U,
			FlagForFollowUp = 4273162695U,
			ErrorGetStreamingEventsProxy = 4264440001U,
			ErrorCannotSetCalendarPermissionOnNonCalendarFolder = 2183377470U,
			SaveExtensionCustomPropertiesFailed = 1686056205U,
			ErrorConnectionFailed = 3500594897U,
			ErrorCannotUseLocalAccount = 2976928908U,
			descInvalidOofParameter = 810356415U,
			ErrorTimeRangeIsTooLarge = 2538042329U,
			ErrorAffectedTaskOccurrencesRequired = 2684918840U,
			ErrorCannotGetAggregatedAccount = 513058151U,
			AADIdentityCreationFailed = 917420962U,
			ErrorDuplicateInputFolderNames = 217482359U,
			MessageNonExistentMailboxSmtpAddress = 4088802584U,
			ErrorIncorrectUpdatePropertyCount = 1439726170U,
			ErrorInvalidSerializedAccessToken = 2485795088U,
			ErrorInvalidRoutingType = 4103342537U,
			ErrorSendMeetingInvitationsRequired = 3383701276U,
			ErrorInvalidIdNotAnItemAttachmentId = 1500443603U,
			RightsManagementInternalLicensingDisabled = 1702622873U,
			MessageCannotUseItemAsRecipient = 247950989U,
			ErrorItemSaveUserConfigurationExists = 815859081U,
			MessageInvalidMailboxMailboxType = 687843280U,
			ErrorCalendarIsCancelledForDecline = 2997278338U,
			ErrorClientIntentInvalidStateDefinition = 3510335548U,
			ErrorInvalidRetentionTagInvisible = 4105318492U,
			ErrorItemSavePropertyError = 1583125739U,
			GetScopedTokenFailedWithInvalidScope = 3308334241U,
			ErrorInvalidItemForOperationRemoveItem = 1518836305U,
			RuleErrorMessageClassificationNotFound = 1858753018U,
			MessageUnableToLoadRBACSettings = 30276810U,
			ErrorQueryLanguageNotValid = 707594371U,
			Purple = 4158023012U,
			InvalidMaxItemsToReturn = 133083912U,
			PostedTo = 4109493280U,
			ExchangeServiceResponseErrorNoResponse = 607223707U,
			ErrorPublicFolderOperationFailed = 1569475421U,
			ErrorBatchProcessingStopped = 444799972U,
			ErrorUnifiedMessagingServerNotFound = 1722439826U,
			InstantSearchNullFolderId = 4077186341U,
			ErrorWeatherServiceDisabled = 4210036349U,
			descNotEnoughPrivileges = 2710201884U,
			CalendarInvalidFirstDayOfWeek = 3284680126U,
			Red = 3021629811U,
			ErrorInvalidExternalSharingSubscriber = 3133201118U,
			ErrorCannotUseFolderIdForItemId = 2770848984U,
			ErrorExchange14Required = 3336001063U,
			ErrorProxyCallFailed = 3032417457U,
			ErrorOrganizationNotFederated = 1788731128U,
			Blue = 2395476974U,
			ErrorCannotDeleteSubfoldersOfMsgRootFolder = 1411102909U,
			ErrorUpdatePropertyMismatch = 1737721488U,
			ErrorIllegalCrossServerConnection = 1160056179U,
			ErrorImListMigration = 822125946U,
			ErrorResponseSchemaValidation = 610335429U,
			ServerNotInSite = 3829975538U,
			ErrorCannotAddAggregatedAccountToList = 3220333293U,
			WhereColon = 1666265192U,
			ErrorInvalidApprovalRequest = 708456719U,
			ErrorIncorrectEncodedIdType = 3010537222U,
			ErrorGetRemoteArchiveItemFailed = 4106572054U,
			ErrorInvalidImGroupId = 827411151U,
			ErrorInvalidRequestUnknownMethodDebug = 1835609958U,
			ErrorBothViewFilterAndRestrictionNonNull = 2675632227U,
			ErrorCannotUseItemIdForFolderId = 2423603834U,
			ErrorCannotDisableMandatoryExtension = 1262244671U,
			ErrorInvalidSyncStateData = 1655535493U,
			ErrorSubmissionQuotaExceeded = 178029729U,
			ErrorMessageDispositionRequired = 2130715693U,
			ErrorSearchScopeCannotHavePublicFolders = 1395824819U,
			ErrorRemoveDelegatesFailed = 3763931121U,
			ErrorInvalidPagingMaxRows = 2467205866U,
			RuleErrorMissingParameter = 3016713339U,
			ErrorLocationServicesInvalidQuery = 1085788054U,
			MessageOccurrenceNotFound = 1319006043U,
			ErrorSearchFolderNotInitialized = 1658627017U,
			FolderScopeNotSpecified = 1064030279U,
			ErrorInvalidSubfilterType = 3880436217U,
			ErrorDuplicateUserIdsSpecified = 4289255106U,
			ErrorDelegateMustBeCalendarEditorToGetMeetingMessages = 1422139444U,
			ErrorMismatchFolderId = 3041888687U,
			ErrorInvalidPropertyDelete = 444235555U,
			MessageActingAsMustHaveEmailAddress = 3538999938U,
			ErrorCalendarIsCancelledForRemove = 4064247940U,
			ErrorCannotResolveODataUrl = 884514945U,
			ErrorCalendarEndDateIsEarlierThanStartDate = 4006585486U,
			ErrorInvalidPercentCompleteValue = 3035123300U,
			ErrorNoApplicableProxyCASServersAvailable = 4164112684U,
			IrmProtectedVoicemailFeatureDisabled = 106943791U,
			IrmExternalLicensingDisabled = 1397740097U,
			ErrorExchangeConfigurationException = 2132997082U,
			ErrorMailboxMoveInProgress = 2069536979U,
			ErrorInvalidValueForPropertyXmlData = 2643780243U,
			RuleErrorDuplicatedPriority = 668221183U,
			ItemNotExistInPurgesFolder = 2622305962U,
			MessageMissingUserRolesForMailboxSearchRoleTypeApp = 3313362701U,
			ErrorInvalidNameForNameResolution = 4279571010U,
			ErrorInvalidRecipientSubfilterOrder = 3776856310U,
			ErrorMailboxContainerGuidMismatch = 3146925354U,
			ErrorInvalidId = 1935400134U,
			ErrorNonPrimarySmtpAddress = 195228379U,
			ErrorSharedFolderSearchNotSupportedOnMultipleFolders = 2280668014U,
			ErrorCalendarInvalidRecurrence = 1532804559U,
			ErrorInvalidOperationSaveReplyForwardToPublicFolder = 4104292452U,
			ErrorInvalidOrderbyThenby = 4072847078U,
			ErrorInvalidRetentionTagTypeMismatch = 531605055U,
			ErrorRequiredPropertyMissing = 608291240U,
			ErrorActiveDirectoryPermanentError = 54420019U,
			IrmRmsError = 360598592U,
			ErrorNoPropertyUpdatesOrAttachmentsSpecified = 3654096821U,
			ConversationActionNeedFlagForFlagAction = 2137439660U,
			ErrorAttachmentNestLevelLimitExceeded = 178421269U,
			ErrorInvalidSmtpAddress = 938097637U
		}

		private enum ParamIDs
		{
			ErrorNoConnectionSettingsAvailableForAggregatedAccount,
			ErrorTimeProposal,
			IrmRmsErrorLocation,
			RuleErrorInvalidSizeRange,
			ExchangeServiceResponseErrorNoResponseForType,
			ErrorAccountNotSupportedForAggregation,
			NewGroupMailboxFailed,
			ErrorInvalidProperty,
			ErrorInvalidUnifiedViewParameter,
			ErrorRightsManagementTemplateNotFound,
			TooManyMailboxQueryObjects,
			ErrorPropertyNotSupportCreate,
			MultifactorRegistrationUnavailable,
			RuleErrorStringValueTooBig,
			ErrorInternalServerErrorFaultInjection,
			ErrorReturnTooManyMailboxesFromDG,
			ActionQueueDeserializationError,
			ErrorMandatoryPropertyMissing,
			GetTenantFailed,
			GetGroupMailboxFailed,
			ErrorInvalidUrlQuery,
			ErrorCannotLinkMoreThanOneO365AccountToAnMsa,
			ErrorDuplicateLegacyDistinguishedNameFound,
			ErrorAggregatedAccountLimitReached,
			ErrorRightsManagementDuplicateTemplateId,
			ErrorSearchTooManyMailboxes,
			IrmRmsErrorCode,
			GetFederatedDirectoryGroupFailed,
			RuleErrorInvalidDateRange,
			ErrorExtensionNotFound,
			ErrorImContactLimitReached,
			ErrorPropertyNotSupportUpdate,
			ErrorPropertyNotSupportFilter,
			GetFederatedDirectoryUserFailed,
			ExchangeServiceResponseErrorWithCode,
			ErrorInvalidDelegateUserId,
			ErrorInvalidPropertyVersionRequest,
			ErrorInvalidParameter,
			ErrorImGroupLimitReached,
			ExecutingUserNotFound,
			SetGroupMailboxFailed,
			IrmRmsErrorMessage,
			ErrorCalendarSeekToConditionNotSupported,
			RemoveGroupMailboxFailed,
			ErrorInvalidRequestedUser,
			ErrorParameterValueEmpty
		}
	}
}
