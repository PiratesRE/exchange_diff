using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.Security.RightsManagement.SOAP.Server;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ServerStrings
	{
		static ServerStrings()
		{
			ServerStrings.stringIDs.Add(2721128427U, "MissingRightsManagementLicense");
			ServerStrings.stringIDs.Add(195662054U, "ServerLocatorServiceTransientFault");
			ServerStrings.stringIDs.Add(824541142U, "EmailAddressMissing");
			ServerStrings.stringIDs.Add(2906390296U, "CannotShareSearchFolder");
			ServerStrings.stringIDs.Add(1390813357U, "EstimateStateStopping");
			ServerStrings.stringIDs.Add(1028519129U, "SpellCheckerDutch");
			ServerStrings.stringIDs.Add(2064592175U, "SpellCheckerNorwegianNynorsk");
			ServerStrings.stringIDs.Add(307085623U, "MigrationFlagsStart");
			ServerStrings.stringIDs.Add(4243709143U, "TeamMailboxMessageSiteAndSiteMailboxDetails");
			ServerStrings.stringIDs.Add(3398846764U, "CannotGetSupportedRoutingTypes");
			ServerStrings.stringIDs.Add(4287963345U, "ClientCulture_0x816");
			ServerStrings.stringIDs.Add(1588035907U, "AsyncOperationTypeMailboxRestore");
			ServerStrings.stringIDs.Add(1805838968U, "MatchContainerClassValidationFailed");
			ServerStrings.stringIDs.Add(3004251640U, "ExCannotCreateSubfolderUnderSearchFolder");
			ServerStrings.stringIDs.Add(161866662U, "InboxRuleImportanceHigh");
			ServerStrings.stringIDs.Add(1003186114U, "MapiCopyFailedProperties");
			ServerStrings.stringIDs.Add(1489962323U, "ClientCulture_0x3C0A");
			ServerStrings.stringIDs.Add(2738718017U, "ErrorTeamMailboxGetUsersNullResponse");
			ServerStrings.stringIDs.Add(2367323544U, "MigrationBatchSupportedActionNone");
			ServerStrings.stringIDs.Add(2198921793U, "ExAuditsUpdateDenied");
			ServerStrings.stringIDs.Add(2134621305U, "ExBadMessageEntryIdSize");
			ServerStrings.stringIDs.Add(2993889924U, "ExAdminAuditLogsUpdateDenied");
			ServerStrings.stringIDs.Add(949829336U, "ExInvalidNumberOfOccurrences");
			ServerStrings.stringIDs.Add(1342791165U, "OleConversionFailed");
			ServerStrings.stringIDs.Add(1551862906U, "ClientCulture_0x3401");
			ServerStrings.stringIDs.Add(346824164U, "ExCannotReadFolderData");
			ServerStrings.stringIDs.Add(1533116792U, "InboxRuleSensitivityNormal");
			ServerStrings.stringIDs.Add(2788778759U, "SpellCheckerCatalan");
			ServerStrings.stringIDs.Add(894291518U, "TeamMailboxMessageHowToGetStarted");
			ServerStrings.stringIDs.Add(2688169016U, "ExInvalidMasterValueAndColumnLength");
			ServerStrings.stringIDs.Add(1002199416U, "MigrationBatchStatusStopping");
			ServerStrings.stringIDs.Add(2264323463U, "ClientCulture_0x440A");
			ServerStrings.stringIDs.Add(3064105318U, "ExFolderAlreadyExistsInClientState");
			ServerStrings.stringIDs.Add(3192255565U, "InvalidPermissionsEntry");
			ServerStrings.stringIDs.Add(753703675U, "ConversionInternalFailure");
			ServerStrings.stringIDs.Add(150715741U, "MigrationTypeExchangeOutlookAnywhere");
			ServerStrings.stringIDs.Add(630982379U, "ClientCulture_0x4809");
			ServerStrings.stringIDs.Add(2605826718U, "MigrationAutodiscoverConfigurationFailure");
			ServerStrings.stringIDs.Add(273958411U, "ExDefaultContactFilename");
			ServerStrings.stringIDs.Add(687935806U, "TeamMailboxMessageReopenClosedSiteMailbox");
			ServerStrings.stringIDs.Add(3547809333U, "MigrationObjectsCountStringPFs");
			ServerStrings.stringIDs.Add(1305341899U, "ExCannotCreateRecurringMeetingWithoutTimeZone");
			ServerStrings.stringIDs.Add(1790240390U, "ExInvalidSaveOnCorrelatedItem");
			ServerStrings.stringIDs.Add(2801555237U, "ErrorTeamMailboxGetListItemChangesNullResponse");
			ServerStrings.stringIDs.Add(3339597578U, "ExCorruptedTimeZone");
			ServerStrings.stringIDs.Add(1540857282U, "MigrationUserStatusSummaryStopped");
			ServerStrings.stringIDs.Add(113768464U, "InboxRuleSensitivityCompanyConfidential");
			ServerStrings.stringIDs.Add(3597498295U, "FailedToAddAttachments");
			ServerStrings.stringIDs.Add(379773582U, "MapiCannotDeliverItem");
			ServerStrings.stringIDs.Add(2576522508U, "MapiCannotGetLocalRepIds");
			ServerStrings.stringIDs.Add(3647547459U, "ClientCulture_0x3C01");
			ServerStrings.stringIDs.Add(209873198U, "FirstDay");
			ServerStrings.stringIDs.Add(4287963599U, "ClientCulture_0x41D");
			ServerStrings.stringIDs.Add(3944203608U, "PersonIsAlreadyLinkedWithGALContact");
			ServerStrings.stringIDs.Add(303788285U, "InboxRuleMessageTypeCalendaring");
			ServerStrings.stringIDs.Add(3653626825U, "Editor");
			ServerStrings.stringIDs.Add(4226852782U, "CannotShareOtherPersonalFolder");
			ServerStrings.stringIDs.Add(540789371U, "TeamMailboxMessageClosedBodyIntroText");
			ServerStrings.stringIDs.Add(2800053981U, "InboxRuleMessageTypeSigned");
			ServerStrings.stringIDs.Add(1894870732U, "MigrationTypePSTImport");
			ServerStrings.stringIDs.Add(179758620U, "DateRangeOneYear");
			ServerStrings.stringIDs.Add(1682339598U, "ExAuditsFolderAccessDenied");
			ServerStrings.stringIDs.Add(1908093332U, "ClientCulture_0x1409");
			ServerStrings.stringIDs.Add(3485169071U, "NoExternalOwaAvailableException");
			ServerStrings.stringIDs.Add(60274512U, "DelegateNotSupportedFolder");
			ServerStrings.stringIDs.Add(3906173474U, "MigrationBatchDirectionLocal");
			ServerStrings.stringIDs.Add(3130219463U, "ErrorFolderDeleted");
			ServerStrings.stringIDs.Add(3911406152U, "BadDateTimeFormatInChangeDate");
			ServerStrings.stringIDs.Add(1489962389U, "ClientCulture_0x1C0A");
			ServerStrings.stringIDs.Add(2011085458U, "TeamMailboxMessageNoActionText");
			ServerStrings.stringIDs.Add(4201517212U, "InvalidMigrationBatchId");
			ServerStrings.stringIDs.Add(3924992590U, "FolderRuleStageOnPromotedMessage");
			ServerStrings.stringIDs.Add(1870702413U, "idUnableToCreateDefaultTaskGroupException");
			ServerStrings.stringIDs.Add(194841891U, "PublicFolderStartSyncFolderHierarchyRpcFailed");
			ServerStrings.stringIDs.Add(149719578U, "SearchStateFailed");
			ServerStrings.stringIDs.Add(1551862809U, "ClientCulture_0x2401");
			ServerStrings.stringIDs.Add(3503392803U, "MapiCannotWritePerUserInformation");
			ServerStrings.stringIDs.Add(2750670341U, "SearchNameCharacterConstraint");
			ServerStrings.stringIDs.Add(4022186804U, "InboxRuleMessageTypeCalendaringResponse");
			ServerStrings.stringIDs.Add(2114183459U, "ErrorTimeProposalInvalidWhenNotAllowedByOrganizer");
			ServerStrings.stringIDs.Add(981561289U, "RequestStateCertExpired");
			ServerStrings.stringIDs.Add(1760294240U, "Thursday");
			ServerStrings.stringIDs.Add(1553527118U, "MapiCannotDeleteAttachment");
			ServerStrings.stringIDs.Add(1512287666U, "ExEventsNotSupportedForDelegateUser");
			ServerStrings.stringIDs.Add(4181674605U, "AsyncOperationTypeImportPST");
			ServerStrings.stringIDs.Add(4056598008U, "MigrationStepDataMigration");
			ServerStrings.stringIDs.Add(2239334052U, "JunkEmailBlockedListXsoTooManyException");
			ServerStrings.stringIDs.Add(2721879535U, "ClientCulture_0x409");
			ServerStrings.stringIDs.Add(3828388569U, "ErrorFolderCreationIsBlocked");
			ServerStrings.stringIDs.Add(3332812272U, "ExInvalidParticipantEntryId");
			ServerStrings.stringIDs.Add(3512980427U, "ExInvalidSpecifierValueError");
			ServerStrings.stringIDs.Add(4064061502U, "TeamMailboxSyncStatusDocumentSyncFailureOnly");
			ServerStrings.stringIDs.Add(2549870927U, "SearchStateInProgress");
			ServerStrings.stringIDs.Add(2171204805U, "JunkEmailInvalidOperationException");
			ServerStrings.stringIDs.Add(1029618557U, "TeamMailboxMessageWhatIsSiteMailbox");
			ServerStrings.stringIDs.Add(2603495179U, "SpellCheckerFrench");
			ServerStrings.stringIDs.Add(192606181U, "ExUnknownRecurrenceBlobRange");
			ServerStrings.stringIDs.Add(1753710770U, "ExAttachmentCannotOpenDueToUnSave");
			ServerStrings.stringIDs.Add(3125164062U, "ClientCulture_0x439");
			ServerStrings.stringIDs.Add(3837865737U, "SpellCheckerEnglishCanada");
			ServerStrings.stringIDs.Add(3771526820U, "MapiCannotUpdateDeferredActionMessages");
			ServerStrings.stringIDs.Add(2014714852U, "MigrationStatisticsCompleteStatus");
			ServerStrings.stringIDs.Add(737023398U, "OleConversionInvalidResultType");
			ServerStrings.stringIDs.Add(3684140834U, "UnableToMakeAutoDiscoveryRequest");
			ServerStrings.stringIDs.Add(4266601947U, "NotificationEmailSubjectImportPst");
			ServerStrings.stringIDs.Add(1338408148U, "SharingMessageAttachmentNotFoundException");
			ServerStrings.stringIDs.Add(2642389795U, "MigrationBatchStatusIncrementalSyncing");
			ServerStrings.stringIDs.Add(4084900412U, "SearchStateStopped");
			ServerStrings.stringIDs.Add(2354834546U, "PublicFolderMailboxNotFound");
			ServerStrings.stringIDs.Add(3021899907U, "MapiCannotGetReceiveFolder");
			ServerStrings.stringIDs.Add(445051701U, "MigrationUserStatusStarting");
			ServerStrings.stringIDs.Add(2210183777U, "AmDbMountNotAllowedDueToLossException");
			ServerStrings.stringIDs.Add(770684443U, "SpellCheckerGermanPreReform");
			ServerStrings.stringIDs.Add(2412724648U, "InvalidKindFormat");
			ServerStrings.stringIDs.Add(905358579U, "OleUnableToReadAttachment");
			ServerStrings.stringIDs.Add(2666546556U, "InvalidModifier");
			ServerStrings.stringIDs.Add(3840447572U, "MigrationUserStatusProvisionUpdating");
			ServerStrings.stringIDs.Add(594875070U, "MigrationMailboxPermissionFullAccess");
			ServerStrings.stringIDs.Add(3928045320U, "WeatherUnitCelsius");
			ServerStrings.stringIDs.Add(2953752128U, "NullDateInChangeDate");
			ServerStrings.stringIDs.Add(4003777722U, "ClientCulture_0x2C09");
			ServerStrings.stringIDs.Add(2084642916U, "MigrationBatchDirectionOffboarding");
			ServerStrings.stringIDs.Add(194112229U, "CalNotifTypeUninteresting");
			ServerStrings.stringIDs.Add(2088104068U, "AmDatabaseNeverMountedException");
			ServerStrings.stringIDs.Add(870651383U, "MapiCannotSetReceiveFolder");
			ServerStrings.stringIDs.Add(3000352360U, "RpcClientWrapperFailedToLoadTopology");
			ServerStrings.stringIDs.Add(3609586398U, "ExCorruptConversationActionItem");
			ServerStrings.stringIDs.Add(2820941203U, "Tuesday");
			ServerStrings.stringIDs.Add(3647326574U, "MapiCannotCreateRestriction");
			ServerStrings.stringIDs.Add(2969695521U, "CorruptJunkMoveStamp");
			ServerStrings.stringIDs.Add(599296811U, "InvalidAttachmentNumber");
			ServerStrings.stringIDs.Add(3125164046U, "ClientCulture_0x83E");
			ServerStrings.stringIDs.Add(4094875965U, "Friday");
			ServerStrings.stringIDs.Add(1586627622U, "NoServerValueAvailable");
			ServerStrings.stringIDs.Add(1690131397U, "DelegateInvalidPermission");
			ServerStrings.stringIDs.Add(1769372998U, "OperationAborted");
			ServerStrings.stringIDs.Add(380587417U, "DiscoveryMailboxNotFound");
			ServerStrings.stringIDs.Add(1559080126U, "ClientCulture_0x422");
			ServerStrings.stringIDs.Add(434577051U, "MigrationUserStatusSummarySynced");
			ServerStrings.stringIDs.Add(656727185U, "FourHours");
			ServerStrings.stringIDs.Add(3033708196U, "MigrationUserStatusCompleted");
			ServerStrings.stringIDs.Add(3797097900U, "ExVotingBlobCorrupt");
			ServerStrings.stringIDs.Add(912807925U, "HexadecimalHtmlColorPatternDescription");
			ServerStrings.stringIDs.Add(3420516204U, "MigrationStepInitialization");
			ServerStrings.stringIDs.Add(3118133575U, "TeamMailboxSyncStatusMembershipSyncFailureOnly");
			ServerStrings.stringIDs.Add(788681379U, "InvalidEncryptedSharedFolderDataException");
			ServerStrings.stringIDs.Add(3198793348U, "ExCorruptRestrictionFilter");
			ServerStrings.stringIDs.Add(3806649575U, "ErrorNotificationAlreadyExists");
			ServerStrings.stringIDs.Add(1743342709U, "ExItemIsOpenedInReadOnlyMode");
			ServerStrings.stringIDs.Add(2479239329U, "UnbalancedParenthesis");
			ServerStrings.stringIDs.Add(3259603877U, "InvalidRpMsgFormat");
			ServerStrings.stringIDs.Add(586169060U, "UserPhotoNotAnImage");
			ServerStrings.stringIDs.Add(1079495974U, "MapiCannotCreateEntryIdFromShortTermId");
			ServerStrings.stringIDs.Add(2721879405U, "ClientCulture_0x807");
			ServerStrings.stringIDs.Add(3407343648U, "MapiCannotCreateBookmark");
			ServerStrings.stringIDs.Add(630534408U, "InvalidateDateRange");
			ServerStrings.stringIDs.Add(1290997994U, "MigrationUserAdminTypeUnknown");
			ServerStrings.stringIDs.Add(2519000101U, "CannotMoveOrCopyBetweenPrivateAndPublicMailbox");
			ServerStrings.stringIDs.Add(2234262993U, "SpellCheckerNorwegianBokmal");
			ServerStrings.stringIDs.Add(3279312390U, "ClientCulture_0x1007");
			ServerStrings.stringIDs.Add(3025564610U, "MigrationBatchSupportedActionStop");
			ServerStrings.stringIDs.Add(1295345518U, "CrossForestNotSupported");
			ServerStrings.stringIDs.Add(2475132438U, "ExCannotAccessSystemFolderId");
			ServerStrings.stringIDs.Add(4287963483U, "ClientCulture_0x410");
			ServerStrings.stringIDs.Add(153308512U, "ExStatefulFilterMustBeSetWhenSetSyncFiltersIsInvokedWithNullFilter");
			ServerStrings.stringIDs.Add(2948566116U, "MapiCannotReadPermissions");
			ServerStrings.stringIDs.Add(3983802523U, "FolderRuleStageOnPublicFolderBefore");
			ServerStrings.stringIDs.Add(3551282863U, "UnbalancedQuote");
			ServerStrings.stringIDs.Add(2092669490U, "FailedToWriteActivityLog");
			ServerStrings.stringIDs.Add(3948653022U, "NoFreeBusyFolder");
			ServerStrings.stringIDs.Add(2721879534U, "ClientCulture_0x408");
			ServerStrings.stringIDs.Add(4050068885U, "SharingConflictException");
			ServerStrings.stringIDs.Add(329227683U, "MapiCannotQueryColumns");
			ServerStrings.stringIDs.Add(914093861U, "ExCaughtMapiExceptionWhileReadingEvents");
			ServerStrings.stringIDs.Add(4287963459U, "ClientCulture_0x81D");
			ServerStrings.stringIDs.Add(1324407795U, "ConversionInvalidSmimeContent");
			ServerStrings.stringIDs.Add(1786112539U, "ExCannotRevertSentMeetingToAppointment");
			ServerStrings.stringIDs.Add(1566523742U, "ExMustSaveFolderToMakeVisibleToOutlook");
			ServerStrings.stringIDs.Add(309976908U, "UnsupportedContentRestriction");
			ServerStrings.stringIDs.Add(1559080244U, "ClientCulture_0x42D");
			ServerStrings.stringIDs.Add(443718431U, "NotificationEmailBodyImportPSTCreated");
			ServerStrings.stringIDs.Add(59886577U, "SearchTargetInSource");
			ServerStrings.stringIDs.Add(809558091U, "ExAddItemAttachmentFailed");
			ServerStrings.stringIDs.Add(2721879541U, "ClientCulture_0x403");
			ServerStrings.stringIDs.Add(1083109339U, "ExCannotMoveOrDeleteDefaultFolders");
			ServerStrings.stringIDs.Add(3923324564U, "ExCannotSeekRow");
			ServerStrings.stringIDs.Add(3620611820U, "MigrationReportBatch");
			ServerStrings.stringIDs.Add(2187234323U, "ExErrorInDetectE15Store");
			ServerStrings.stringIDs.Add(2391045796U, "idDefaultFoldersNotLocalizedException");
			ServerStrings.stringIDs.Add(2969986172U, "MigrationStateCompleted");
			ServerStrings.stringIDs.Add(297411134U, "ErrorMissingMailboxOrPermission");
			ServerStrings.stringIDs.Add(1101524278U, "ClientCulture_0x140C");
			ServerStrings.stringIDs.Add(2192010005U, "MigrationTypeIMAP");
			ServerStrings.stringIDs.Add(630982445U, "ClientCulture_0x2809");
			ServerStrings.stringIDs.Add(1955147499U, "ClientCulture_0x1404");
			ServerStrings.stringIDs.Add(984546473U, "ExCannotRejectDeletes");
			ServerStrings.stringIDs.Add(415920568U, "TeamMailboxSyncStatusMaintenanceSyncFailureOnly");
			ServerStrings.stringIDs.Add(564935456U, "MigrationStateStopped");
			ServerStrings.stringIDs.Add(3735242122U, "MigrationStageDiscovery");
			ServerStrings.stringIDs.Add(2721879655U, "ClientCulture_0x40A");
			ServerStrings.stringIDs.Add(3791574374U, "NotificationEmailBodyCertExpired");
			ServerStrings.stringIDs.Add(1657454002U, "ExCannotRejectSameOperationTwice");
			ServerStrings.stringIDs.Add(3371920351U, "ExCannotGetSearchCriteria");
			ServerStrings.stringIDs.Add(3800790638U, "ExInvalidMaxQueueSize");
			ServerStrings.stringIDs.Add(1437123480U, "ADException");
			ServerStrings.stringIDs.Add(653269905U, "ExNoMailboxOwner");
			ServerStrings.stringIDs.Add(4002989775U, "ExNotConnected");
			ServerStrings.stringIDs.Add(478700673U, "SearchStateStopping");
			ServerStrings.stringIDs.Add(1750841907U, "SpellCheckerKorean");
			ServerStrings.stringIDs.Add(1435478051U, "MigrationTypeExchangeLocalMove");
			ServerStrings.stringIDs.Add(2120544431U, "MapiCannotSubmitMessage");
			ServerStrings.stringIDs.Add(4003777885U, "ClientCulture_0x1C09");
			ServerStrings.stringIDs.Add(1458958954U, "ExInvalidOrder");
			ServerStrings.stringIDs.Add(725138746U, "NoProviderSupportShareFolder");
			ServerStrings.stringIDs.Add(3332361397U, "ExConnectionCacheSizeNotSet");
			ServerStrings.stringIDs.Add(4100720225U, "MigrationFlagsRemove");
			ServerStrings.stringIDs.Add(2084653907U, "ExInvalidRecipient");
			ServerStrings.stringIDs.Add(1532066664U, "ExFoundInvalidRowType");
			ServerStrings.stringIDs.Add(3156797033U, "ExInvalidOffset");
			ServerStrings.stringIDs.Add(2795849522U, "NotEnoughPermissionsToPerformOperation");
			ServerStrings.stringIDs.Add(3321416304U, "MigrationStatisticsPartiallyCompleteStatus");
			ServerStrings.stringIDs.Add(1147224948U, "TeamMailboxSyncStatusNotAvailable");
			ServerStrings.stringIDs.Add(3791300057U, "InvalidOperator");
			ServerStrings.stringIDs.Add(1063299331U, "DefaultHtmlAttachmentHrefText");
			ServerStrings.stringIDs.Add(2883857825U, "ConversionBodyCorrupt");
			ServerStrings.stringIDs.Add(2358432026U, "ClientCulture_0x1407");
			ServerStrings.stringIDs.Add(2091534526U, "MapiCannotSaveChanges");
			ServerStrings.stringIDs.Add(1712595778U, "RejectedSuggestionPersonIdSameAsPersonId");
			ServerStrings.stringIDs.Add(2860711473U, "ErrorInvalidPhoneNumberFormat");
			ServerStrings.stringIDs.Add(1551718751U, "MigrationStateCorrupted");
			ServerStrings.stringIDs.Add(3286105202U, "ProvisioningRequestCsvContainsNeitherPasswordNorFederatedIdentity");
			ServerStrings.stringIDs.Add(3657645519U, "SecurityPrincipalAlreadyDefined");
			ServerStrings.stringIDs.Add(968748330U, "KqlParseException");
			ServerStrings.stringIDs.Add(2414329026U, "ExEventNotFound");
			ServerStrings.stringIDs.Add(1227907325U, "ThreeDays");
			ServerStrings.stringIDs.Add(278097122U, "ExInvalidSortLength");
			ServerStrings.stringIDs.Add(3752161350U, "MapiCannotGetPerUserLongTermIds");
			ServerStrings.stringIDs.Add(2656172439U, "ExFolderWithoutMapiProp");
			ServerStrings.stringIDs.Add(3755862658U, "ExChangeKeyTooLong");
			ServerStrings.stringIDs.Add(2902058685U, "ExUnknownRestrictionType");
			ServerStrings.stringIDs.Add(91197877U, "ExInvalidRowCount");
			ServerStrings.stringIDs.Add(2720991162U, "UnsupportedExistRestriction");
			ServerStrings.stringIDs.Add(4121599705U, "AvailabilityOnly");
			ServerStrings.stringIDs.Add(1399515408U, "MapiCannotExecuteWithInternalAccess");
			ServerStrings.stringIDs.Add(2879753762U, "ExItemNoParentId");
			ServerStrings.stringIDs.Add(1107465087U, "MigrationTypePublicFolder");
			ServerStrings.stringIDs.Add(130796183U, "MapiCannotGetPerUserGuid");
			ServerStrings.stringIDs.Add(2521346493U, "FederationNotEnabled");
			ServerStrings.stringIDs.Add(241258410U, "RequestStateWaitingForFinalization");
			ServerStrings.stringIDs.Add(1176987915U, "RequestStateCompleted");
			ServerStrings.stringIDs.Add(493948276U, "TooManyCultures");
			ServerStrings.stringIDs.Add(3358444168U, "MapiCannotSetCollapseState");
			ServerStrings.stringIDs.Add(1834533699U, "IncompleteUserInformationToAccessGroupMailbox");
			ServerStrings.stringIDs.Add(2116512813U, "ClientCulture_0x2001");
			ServerStrings.stringIDs.Add(1016962785U, "CannotImportMessageChange");
			ServerStrings.stringIDs.Add(3299931409U, "InvalidTimesInTimeSlot");
			ServerStrings.stringIDs.Add(3086195034U, "MigrationReportFinalizationFailure");
			ServerStrings.stringIDs.Add(3050947916U, "StructuredQueryException");
			ServerStrings.stringIDs.Add(628804796U, "ExUnknownResponseType");
			ServerStrings.stringIDs.Add(2214930724U, "RequestStateCreated");
			ServerStrings.stringIDs.Add(2474224463U, "ExInvalidComparisonOperatorInComparisonFilter");
			ServerStrings.stringIDs.Add(3920537899U, "MigrationFolderSettings");
			ServerStrings.stringIDs.Add(2721879419U, "ClientCulture_0x809");
			ServerStrings.stringIDs.Add(3653170115U, "ExUnsupportedSeekReference");
			ServerStrings.stringIDs.Add(3031733457U, "MigrationBatchStatusCompleted");
			ServerStrings.stringIDs.Add(713262539U, "MigrationTestMSAWarning");
			ServerStrings.stringIDs.Add(2595969499U, "InvalidDateTimeRange");
			ServerStrings.stringIDs.Add(2125622397U, "MapiCannotGetMapiTable");
			ServerStrings.stringIDs.Add(1130047631U, "MapiCannotCheckForNotifications");
			ServerStrings.stringIDs.Add(977128087U, "CannotStamplocalFreeBusyId");
			ServerStrings.stringIDs.Add(2828973696U, "ClientCulture_0x100A");
			ServerStrings.stringIDs.Add(1893783426U, "MigrationBatchStatusSynced");
			ServerStrings.stringIDs.Add(2428434059U, "ExchangePrincipalFromMailboxDataError");
			ServerStrings.stringIDs.Add(3615217052U, "MigrationUserStatusIncrementalFailed");
			ServerStrings.stringIDs.Add(3321965778U, "InvalidXml");
			ServerStrings.stringIDs.Add(223559137U, "ExEntryIdToLong");
			ServerStrings.stringIDs.Add(1559080128U, "ClientCulture_0x420");
			ServerStrings.stringIDs.Add(697160562U, "PrincipalFromDifferentSite");
			ServerStrings.stringIDs.Add(3415665237U, "ErrorSavingRules");
			ServerStrings.stringIDs.Add(1802620674U, "PublishedFolderAccessDeniedException");
			ServerStrings.stringIDs.Add(305670746U, "PublicFoldersNotEnabledForEnterprise");
			ServerStrings.stringIDs.Add(3382673089U, "InboxRuleMessageTypeApprovalRequest");
			ServerStrings.stringIDs.Add(4042709143U, "NonUniqueRecipientError");
			ServerStrings.stringIDs.Add(3548482161U, "ExSystemFolderAccessDenied");
			ServerStrings.stringIDs.Add(3379789351U, "MapiCannotRemoveNotification");
			ServerStrings.stringIDs.Add(1699673688U, "ClientCulture_0x180A");
			ServerStrings.stringIDs.Add(3770139034U, "ExCommentFilterPropertiesNotSupported");
			ServerStrings.stringIDs.Add(2450087029U, "ExDictionaryDataCorruptedNullKey");
			ServerStrings.stringIDs.Add(3165677092U, "MigrationBatchStatusStarting");
			ServerStrings.stringIDs.Add(2828973630U, "ClientCulture_0x300A");
			ServerStrings.stringIDs.Add(1987097643U, "ExBadValueForTypeCode0");
			ServerStrings.stringIDs.Add(510893832U, "ErrorTimeProposalInvalidOnRecurringMaster");
			ServerStrings.stringIDs.Add(2114464927U, "SearchStateDeletionInProgress");
			ServerStrings.stringIDs.Add(1600317547U, "ExRuleIdInvalid");
			ServerStrings.stringIDs.Add(3959385997U, "MapiCannotCollapseRow");
			ServerStrings.stringIDs.Add(3606405084U, "SharingUnableToGenerateEncryptedSharedFolderData");
			ServerStrings.stringIDs.Add(3825572190U, "ExConnectionNotCached");
			ServerStrings.stringIDs.Add(931793818U, "CVSPopulationTimedout");
			ServerStrings.stringIDs.Add(300160841U, "BadDateFormatInChangeDate");
			ServerStrings.stringIDs.Add(391772390U, "MigrationBatchStatusCompletedWithErrors");
			ServerStrings.stringIDs.Add(765916263U, "NotReadSubjectPrefix");
			ServerStrings.stringIDs.Add(1428661747U, "MapiCannotFinishSubmit");
			ServerStrings.stringIDs.Add(2721875976U, "ClientCulture_0xC01");
			ServerStrings.stringIDs.Add(2811676186U, "ExItemNotFoundInClientManifest");
			ServerStrings.stringIDs.Add(2612888606U, "ErrorNoStoreObjectId");
			ServerStrings.stringIDs.Add(2194255608U, "CalendarItemCorrelationFailed");
			ServerStrings.stringIDs.Add(260083086U, "ExInvalidOccurrenceId");
			ServerStrings.stringIDs.Add(2814533567U, "DateRangeOneWeek");
			ServerStrings.stringIDs.Add(2512850293U, "EnforceRulesQuota");
			ServerStrings.stringIDs.Add(1837976028U, "ExInvalidMonth");
			ServerStrings.stringIDs.Add(3370782263U, "MigrationUserStatusCompletionSynced");
			ServerStrings.stringIDs.Add(1880367925U, "FirstFullWeek");
			ServerStrings.stringIDs.Add(3254542318U, "MigrationFeatureEndpoints");
			ServerStrings.stringIDs.Add(1291493387U, "ExNoSearchHasBeenInitiated");
			ServerStrings.stringIDs.Add(1916012218U, "MigrationUserStatusIncrementalSyncing");
			ServerStrings.stringIDs.Add(2113494780U, "PublicFolderMailboxesCannotBeCreatedDuringMigration");
			ServerStrings.stringIDs.Add(2841260818U, "MapiCannotCreateFilter");
			ServerStrings.stringIDs.Add(932673369U, "MapiCannotNotifyMessageNewMail");
			ServerStrings.stringIDs.Add(1644908980U, "MigrationUserStatusSyncing");
			ServerStrings.stringIDs.Add(2220521549U, "MigrationBatchFlagForceNewMigration");
			ServerStrings.stringIDs.Add(2553874506U, "CannotGetFinalStateSynchronizerProviderBase");
			ServerStrings.stringIDs.Add(2057374892U, "ServerLocatorClientWCFCallCommunicationError");
			ServerStrings.stringIDs.Add(553140815U, "ExValueCannotBeNull");
			ServerStrings.stringIDs.Add(2472743270U, "ClientCulture_0x3009");
			ServerStrings.stringIDs.Add(2764365677U, "MigrationTypeBulkProvisioning");
			ServerStrings.stringIDs.Add(156083260U, "ErrorFolderIsMailEnabled");
			ServerStrings.stringIDs.Add(2030923649U, "ExCantAccessOccurrenceFromNewItem");
			ServerStrings.stringIDs.Add(1309170924U, "ConversionCorruptContent");
			ServerStrings.stringIDs.Add(2021640990U, "AutoDFailedToGetToken");
			ServerStrings.stringIDs.Add(4205544601U, "ExCorruptPropertyTag");
			ServerStrings.stringIDs.Add(77158404U, "InvalidTimeSlot");
			ServerStrings.stringIDs.Add(3242781799U, "ExCannotOpenMultipleCorrelatedItems");
			ServerStrings.stringIDs.Add(902677431U, "ErrorLanguageIsNull");
			ServerStrings.stringIDs.Add(372387097U, "ExInvalidAcrBaseProfiles");
			ServerStrings.stringIDs.Add(2802946662U, "ExMustSaveFolderToApplySearch");
			ServerStrings.stringIDs.Add(2753071453U, "ExReadTopologyTimeout");
			ServerStrings.stringIDs.Add(3333234210U, "ExUnknownRecurrenceBlobType");
			ServerStrings.stringIDs.Add(4287963476U, "ClientCulture_0x419");
			ServerStrings.stringIDs.Add(113369212U, "SpellCheckerHebrew");
			ServerStrings.stringIDs.Add(2116512976U, "ClientCulture_0x1001");
			ServerStrings.stringIDs.Add(1820555509U, "InvalidAttachmentId");
			ServerStrings.stringIDs.Add(3125164183U, "ClientCulture_0x43F");
			ServerStrings.stringIDs.Add(899245499U, "ExInvalidFolderId");
			ServerStrings.stringIDs.Add(1012814431U, "AmDbMountNotAllowedDueToRegistryConfigurationException");
			ServerStrings.stringIDs.Add(911781703U, "CannotSaveReadOnlyAttachment");
			ServerStrings.stringIDs.Add(3787888884U, "InvalidTnef");
			ServerStrings.stringIDs.Add(392648307U, "MigrationUserStatusIncrementalSynced");
			ServerStrings.stringIDs.Add(2071885718U, "ExAdminAuditLogsDeleteDenied");
			ServerStrings.stringIDs.Add(2114545814U, "ConversionInvalidMessageCodepageCharset");
			ServerStrings.stringIDs.Add(2721879653U, "ClientCulture_0x40C");
			ServerStrings.stringIDs.Add(2379279263U, "DumpsterStatusShutdownException");
			ServerStrings.stringIDs.Add(1345910406U, "CannotDeleteRootFolder");
			ServerStrings.stringIDs.Add(1856386122U, "MapiCannotGetEffectiveRights");
			ServerStrings.stringIDs.Add(3136620084U, "InvalidMechanismToAccessGroupMailbox");
			ServerStrings.stringIDs.Add(3536723681U, "MapiCannotSavePermissions");
			ServerStrings.stringIDs.Add(2876027863U, "ClientCulture_0x1004");
			ServerStrings.stringIDs.Add(3588257392U, "MigrationBatchStatusCreated");
			ServerStrings.stringIDs.Add(2776843979U, "NotAllowedExternalSharingByPolicy");
			ServerStrings.stringIDs.Add(1311410701U, "InboxRuleMessageTypeReadReceipt");
			ServerStrings.stringIDs.Add(1490728717U, "StoreOperationFailed");
			ServerStrings.stringIDs.Add(33074401U, "ErrorExTimeZoneValueNoGmtMatch");
			ServerStrings.stringIDs.Add(3552080970U, "ExStoreObjectValidationError");
			ServerStrings.stringIDs.Add(1010688174U, "MigrationBatchFlagNone");
			ServerStrings.stringIDs.Add(2472743107U, "ClientCulture_0x4009");
			ServerStrings.stringIDs.Add(3188685413U, "TooManyAttachmentsOnProtectedMessage");
			ServerStrings.stringIDs.Add(3397519574U, "PublicFolderOpenFailedOnExistingFolder");
			ServerStrings.stringIDs.Add(1522765994U, "ExInvalidSortOrder");
			ServerStrings.stringIDs.Add(3678175251U, "ReplyRuleNotSupportedOnNonMailPublicFolder");
			ServerStrings.stringIDs.Add(2626172332U, "ExGetPropsFailed");
			ServerStrings.stringIDs.Add(2344409522U, "EstimateStateSucceeded");
			ServerStrings.stringIDs.Add(2621679806U, "MigrationBatchSupportedActionRemove");
			ServerStrings.stringIDs.Add(2545687272U, "MapiCannotSaveMessageStream");
			ServerStrings.stringIDs.Add(3515331893U, "MapiInvalidId");
			ServerStrings.stringIDs.Add(2975245987U, "ContactLinkingMaximumNumberOfContactsPerPersonError");
			ServerStrings.stringIDs.Add(2827742042U, "ConversionUnsupportedContent");
			ServerStrings.stringIDs.Add(4287122418U, "MigrationUserStatusIncrementalStopped");
			ServerStrings.stringIDs.Add(955524647U, "MapiCannotCreateMessage");
			ServerStrings.stringIDs.Add(968419245U, "InvalidSendAddressIdentity");
			ServerStrings.stringIDs.Add(1559080133U, "ClientCulture_0x425");
			ServerStrings.stringIDs.Add(2559314081U, "DisposeOOFHistoryFolder");
			ServerStrings.stringIDs.Add(1115353353U, "ExCantDeleteLastOccurrence");
			ServerStrings.stringIDs.Add(3283674777U, "MigrationReportBatchSuccess");
			ServerStrings.stringIDs.Add(4284023892U, "ErrorAccessingLargeProperty");
			ServerStrings.stringIDs.Add(208132458U, "OperationNotSupportedOnPublicFolderMailbox");
			ServerStrings.stringIDs.Add(872597966U, "ExCannotCreateMeetingCancellation");
			ServerStrings.stringIDs.Add(3533048780U, "MigrationFeaturePAW");
			ServerStrings.stringIDs.Add(2850981798U, "InboxRuleFlagStatusFlagged");
			ServerStrings.stringIDs.Add(135679156U, "JunkEmailBlockedListXsoNullException");
			ServerStrings.stringIDs.Add(3125164186U, "ClientCulture_0x43E");
			ServerStrings.stringIDs.Add(1695521020U, "TeamMailboxMessageGoToYourGroupSite");
			ServerStrings.stringIDs.Add(4287963464U, "ClientCulture_0x81A");
			ServerStrings.stringIDs.Add(2076121062U, "CannotImportMessageMove");
			ServerStrings.stringIDs.Add(1860805300U, "FifteenMinutes");
			ServerStrings.stringIDs.Add(823631819U, "OneDays");
			ServerStrings.stringIDs.Add(1680647297U, "CorruptNaturalLanguageProperty");
			ServerStrings.stringIDs.Add(2312979504U, "DumpsterStatusAlreadyStartedException");
			ServerStrings.stringIDs.Add(4217834195U, "ExCannotSetSearchCriteria");
			ServerStrings.stringIDs.Add(192747935U, "ExBadObjectType");
			ServerStrings.stringIDs.Add(298653586U, "SpellCheckerFinnish");
			ServerStrings.stringIDs.Add(3098387483U, "MigrationBatchStatusWaiting");
			ServerStrings.stringIDs.Add(4038623627U, "UnsupportedKindKeywords");
			ServerStrings.stringIDs.Add(2721879545U, "ClientCulture_0x407");
			ServerStrings.stringIDs.Add(2516633257U, "PropertyChangeMetadataParseError");
			ServerStrings.stringIDs.Add(2642058832U, "SyncFailedToCreateNewItemOrBindToExistingOne");
			ServerStrings.stringIDs.Add(3516269798U, "ConversionFailedInvalidMacBin");
			ServerStrings.stringIDs.Add(435552220U, "SpellCheckerEnglishUnitedStates");
			ServerStrings.stringIDs.Add(1551474375U, "ExContactHasNoId");
			ServerStrings.stringIDs.Add(2860195473U, "ErrorExTimeZoneValueTimeZoneNotFound");
			ServerStrings.stringIDs.Add(1337636428U, "SpellCheckerGermanPostReform");
			ServerStrings.stringIDs.Add(851950942U, "InboxRuleMessageTypePermissionControlled");
			ServerStrings.stringIDs.Add(2721879656U, "ClientCulture_0x40F");
			ServerStrings.stringIDs.Add(1310001827U, "PropertyDefinitionsValuesNotMatch");
			ServerStrings.stringIDs.Add(4287959997U, "ClientCulture_0xC1A");
			ServerStrings.stringIDs.Add(2706950694U, "DateRangeThreeMonths");
			ServerStrings.stringIDs.Add(3038510311U, "ExConnectionAlternate");
			ServerStrings.stringIDs.Add(2795037022U, "MigrationBatchSupportedActionStart");
			ServerStrings.stringIDs.Add(2721879540U, "ClientCulture_0x402");
			ServerStrings.stringIDs.Add(3256433766U, "ExCannotAccessAdminAuditLogsFolderId");
			ServerStrings.stringIDs.Add(1559080132U, "ClientCulture_0x424");
			ServerStrings.stringIDs.Add(132681062U, "MigrationStateWaiting");
			ServerStrings.stringIDs.Add(2224931539U, "MigrationStageProcessing");
			ServerStrings.stringIDs.Add(662607817U, "Database");
			ServerStrings.stringIDs.Add(1899965635U, "MapiCannotGetTransportQueueFolderId");
			ServerStrings.stringIDs.Add(252713015U, "UnsupportedAction");
			ServerStrings.stringIDs.Add(305850519U, "FolderRuleErrorInvalidRecipientEntryId");
			ServerStrings.stringIDs.Add(2536777203U, "TeamMailboxMessageGoToTheSite");
			ServerStrings.stringIDs.Add(834480874U, "TwelveHours");
			ServerStrings.stringIDs.Add(1710892423U, "MigrationStageInjection");
			ServerStrings.stringIDs.Add(3729217742U, "MapiCannotGetContentsTable");
			ServerStrings.stringIDs.Add(3285920218U, "EstimateStateStopped");
			ServerStrings.stringIDs.Add(1888800485U, "NullWorkHours");
			ServerStrings.stringIDs.Add(3088889153U, "MigrationUserStatusCompleting");
			ServerStrings.stringIDs.Add(3726325313U, "FiveMinutes");
			ServerStrings.stringIDs.Add(3449171400U, "InboxRuleMessageTypeVoicemail");
			ServerStrings.stringIDs.Add(3412096701U, "SpellCheckerPortugueseBrasil");
			ServerStrings.stringIDs.Add(561027979U, "GenericFailureRMDecryption");
			ServerStrings.stringIDs.Add(312177309U, "SpellCheckerEnglishAustralia");
			ServerStrings.stringIDs.Add(3160415695U, "NoDeferredActions");
			ServerStrings.stringIDs.Add(3022558012U, "ErrorSetDateTimeFormatWithoutLanguage");
			ServerStrings.stringIDs.Add(987212968U, "ClientCulture_0x1801");
			ServerStrings.stringIDs.Add(2113411584U, "MapiErrorParsingId");
			ServerStrings.stringIDs.Add(563787386U, "MigrationUserAdminTypePartnerTenant");
			ServerStrings.stringIDs.Add(3598152156U, "MigrationUserStatusStopped");
			ServerStrings.stringIDs.Add(909292782U, "MigrationReportBatchFailure");
			ServerStrings.stringIDs.Add(490979673U, "MapiCannotCreateAttachment");
			ServerStrings.stringIDs.Add(1946505183U, "NotificationEmailBodyCertExpiring");
			ServerStrings.stringIDs.Add(2470109498U, "MapiCannotReadPerUserInformation");
			ServerStrings.stringIDs.Add(2189690271U, "ExInvalidSubFilterProperty");
			ServerStrings.stringIDs.Add(1931248018U, "StockReplyTemplate");
			ServerStrings.stringIDs.Add(3362131860U, "CalNotifTypeSummary");
			ServerStrings.stringIDs.Add(633339293U, "JunkEmailInvalidConstructionException");
			ServerStrings.stringIDs.Add(3309334123U, "MapiCannotCreateAssociatedMessage");
			ServerStrings.stringIDs.Add(4287963482U, "ClientCulture_0x413");
			ServerStrings.stringIDs.Add(1706900424U, "MapiCannotSortTable");
			ServerStrings.stringIDs.Add(2822102721U, "MigrationUserStatusStopping");
			ServerStrings.stringIDs.Add(1778962839U, "MapiCannotGetRecipientTable");
			ServerStrings.stringIDs.Add(2932685544U, "ExInvalidCallToTryUpdateCalendarItem");
			ServerStrings.stringIDs.Add(2721879544U, "ClientCulture_0x406");
			ServerStrings.stringIDs.Add(102022121U, "ExCannotAccessAuditsFolderId");
			ServerStrings.stringIDs.Add(18047843U, "ExReadEventsFailed");
			ServerStrings.stringIDs.Add(727433418U, "ExCannotQueryAssociatedTable");
			ServerStrings.stringIDs.Add(1559080121U, "ClientCulture_0x429");
			ServerStrings.stringIDs.Add(2303146172U, "MigrationStepProvisioningUpdate");
			ServerStrings.stringIDs.Add(1311803567U, "TwoWeeks");
			ServerStrings.stringIDs.Add(4021122445U, "MigrationFeatureUpgradeBlock");
			ServerStrings.stringIDs.Add(1530529173U, "ExInvalidServiceType");
			ServerStrings.stringIDs.Add(3873491039U, "NullTimeInChangeDate");
			ServerStrings.stringIDs.Add(1192664468U, "ConversionInvalidSmimeClearSignedContent");
			ServerStrings.stringIDs.Add(2464640173U, "RequestStateSuspended");
			ServerStrings.stringIDs.Add(2393270488U, "MapiIsFromPublicStoreCheckFailed");
			ServerStrings.stringIDs.Add(4172275237U, "ExCannotSendMeetingMessages");
			ServerStrings.stringIDs.Add(2982637405U, "ExAuditsDeleteDenied");
			ServerStrings.stringIDs.Add(4287963485U, "ClientCulture_0x416");
			ServerStrings.stringIDs.Add(4113710984U, "MissingPropertyValue");
			ServerStrings.stringIDs.Add(3202920824U, "FolderNotPublishedException");
			ServerStrings.stringIDs.Add(1807895935U, "ServerLocatorClientEndpointNotFoundException");
			ServerStrings.stringIDs.Add(988424741U, "ExTooComplexGroupSortParameter");
			ServerStrings.stringIDs.Add(220684915U, "MapiCannotLookupEntryId");
			ServerStrings.stringIDs.Add(543772960U, "NotificationEmailBodyExportPSTCreated");
			ServerStrings.stringIDs.Add(1291339327U, "TeamMailboxMessageReactivatedBodyIntroText");
			ServerStrings.stringIDs.Add(1027067688U, "NotSupportedWithMailboxVersionException");
			ServerStrings.stringIDs.Add(1908093266U, "ClientCulture_0x3409");
			ServerStrings.stringIDs.Add(4287963593U, "ClientCulture_0x41B");
			ServerStrings.stringIDs.Add(1892124050U, "CannotAddAttachmentToReadOnlyCollection");
			ServerStrings.stringIDs.Add(1940443510U, "UserPhotoPreviewNotFound");
			ServerStrings.stringIDs.Add(3503580213U, "PublicFolderMailboxesCannotBeMovedDuringMigration");
			ServerStrings.stringIDs.Add(3739954134U, "MigrationBatchDirectionOnboarding");
			ServerStrings.stringIDs.Add(1702371863U, "AsyncOperationTypeMigration");
			ServerStrings.stringIDs.Add(2721879659U, "ClientCulture_0x40E");
			ServerStrings.stringIDs.Add(1959086372U, "OriginatingServer");
			ServerStrings.stringIDs.Add(3553550262U, "EstimateStatePartiallySucceeded");
			ServerStrings.stringIDs.Add(1063322282U, "CannotImportDeletion");
			ServerStrings.stringIDs.Add(668312535U, "MigrationUserStatusSynced");
			ServerStrings.stringIDs.Add(1644403520U, "CannotImportFolderChange");
			ServerStrings.stringIDs.Add(2717353738U, "MigrationUserStatusValidating");
			ServerStrings.stringIDs.Add(2473956989U, "ExConstraintNotSupportedForThisPropertyType");
			ServerStrings.stringIDs.Add(614444523U, "NotificationEmailSubjectCertExpiring");
			ServerStrings.stringIDs.Add(1918350106U, "MigrationUserStatusSummaryCompleted");
			ServerStrings.stringIDs.Add(116529921U, "SpellCheckerArabic");
			ServerStrings.stringIDs.Add(2162216975U, "InternalLicensingDisabledForEnterprise");
			ServerStrings.stringIDs.Add(2264323529U, "ClientCulture_0x240A");
			ServerStrings.stringIDs.Add(1621771988U, "RPCOperationAbortedBecauseOfAnotherRPCThread");
			ServerStrings.stringIDs.Add(2356708790U, "ExInvalidMdbGuid");
			ServerStrings.stringIDs.Add(3718014775U, "SpellCheckerEnglishUnitedKingdom");
			ServerStrings.stringIDs.Add(1886702248U, "ExFilterHierarchyIsTooDeep");
			ServerStrings.stringIDs.Add(176188277U, "MapiCannotSetMessageLockState");
			ServerStrings.stringIDs.Add(2635947676U, "CannotProtectMessageForNonSmtpSender");
			ServerStrings.stringIDs.Add(3669082289U, "ExSearchFolderIsAlreadyVisibleToOutlook");
			ServerStrings.stringIDs.Add(1534555903U, "ExEntryIdFirst4Bytes");
			ServerStrings.stringIDs.Add(4224870879U, "CustomMessageLengthExceeded");
			ServerStrings.stringIDs.Add(469526714U, "ExWrappedStreamFailure");
			ServerStrings.stringIDs.Add(3215248187U, "ErrorExTimeZoneValueWrongGmtFormat");
			ServerStrings.stringIDs.Add(2069696862U, "InternalParserError");
			ServerStrings.stringIDs.Add(2542755219U, "ExInvalidCount");
			ServerStrings.stringIDs.Add(3554710343U, "ADUserNotFound");
			ServerStrings.stringIDs.Add(2441819035U, "InboxRuleFlagStatusNotFlagged");
			ServerStrings.stringIDs.Add(1728333927U, "ConversionMustLoadAllPropeties");
			ServerStrings.stringIDs.Add(4222627801U, "ThreeHours");
			ServerStrings.stringIDs.Add(1816423621U, "MapiCannotGetIDFromNames");
			ServerStrings.stringIDs.Add(3922439094U, "ErrorSigntureTooLarge");
			ServerStrings.stringIDs.Add(2247108930U, "MigrationBatchFlagReportInitial");
			ServerStrings.stringIDs.Add(3391255931U, "ErrorTimeProposalEndTimeBeforeStartTime");
			ServerStrings.stringIDs.Add(3041727278U, "CannotSetMessageFlagStatus");
			ServerStrings.stringIDs.Add(2084296733U, "MigrationFlagsReport");
			ServerStrings.stringIDs.Add(2188504663U, "MigrationStepProvisioning");
			ServerStrings.stringIDs.Add(3392951782U, "FirstFourDayWeek");
			ServerStrings.stringIDs.Add(313625050U, "MapiCannotModifyRecipients");
			ServerStrings.stringIDs.Add(3755684524U, "ConversionCorruptSummaryTnef");
			ServerStrings.stringIDs.Add(1908093169U, "ClientCulture_0x2409");
			ServerStrings.stringIDs.Add(2511211668U, "ExAlreadyConnected");
			ServerStrings.stringIDs.Add(2066272930U, "ExReportMessageCorruptedDueToWrongItemAttachmentType");
			ServerStrings.stringIDs.Add(2828973564U, "ClientCulture_0x500A");
			ServerStrings.stringIDs.Add(4033769924U, "MigrationTypeNone");
			ServerStrings.stringIDs.Add(1112725991U, "CannotImportReadStateChange");
			ServerStrings.stringIDs.Add(2653492529U, "MapiCannotGetAttachmentTable");
			ServerStrings.stringIDs.Add(4054158457U, "MapiCannotOpenAttachment");
			ServerStrings.stringIDs.Add(4130050910U, "ExSuffixTextFilterNotSupported");
			ServerStrings.stringIDs.Add(2641324698U, "ExSeparatorNotFoundOnCompoundValue");
			ServerStrings.stringIDs.Add(4232824632U, "MigrationBatchStatusCorrupted");
			ServerStrings.stringIDs.Add(3135377227U, "MigrationBatchStatusSyncing");
			ServerStrings.stringIDs.Add(4287963488U, "ClientCulture_0x415");
			ServerStrings.stringIDs.Add(3647547362U, "ClientCulture_0x2C01");
			ServerStrings.stringIDs.Add(984518113U, "CannotAccessRemoteMailbox");
			ServerStrings.stringIDs.Add(456720827U, "MapiCannotFindRow");
			ServerStrings.stringIDs.Add(101242371U, "ThirtyMinutes");
			ServerStrings.stringIDs.Add(1022111068U, "MapiCannotSeekRow");
			ServerStrings.stringIDs.Add(2976127978U, "MigrationUserStatusFailed");
			ServerStrings.stringIDs.Add(3086681225U, "ExceptionObjectHasBeenDeleted");
			ServerStrings.stringIDs.Add(2706959952U, "MigrationBatchFlagDisallowExistingUsers");
			ServerStrings.stringIDs.Add(3884678960U, "ClientCulture_0x464");
			ServerStrings.stringIDs.Add(2868399142U, "UnsupportedPropertyRestriction");
			ServerStrings.stringIDs.Add(265623663U, "ServerLocatorClientWCFCallTimeout");
			ServerStrings.stringIDs.Add(3055339528U, "InvalidServiceLocationResponse");
			ServerStrings.stringIDs.Add(722987850U, "MapiCannotDeleteProperties");
			ServerStrings.stringIDs.Add(4146176105U, "NeedFolderIdForPublicFolder");
			ServerStrings.stringIDs.Add(1666174282U, "ClientCulture_0x100C");
			ServerStrings.stringIDs.Add(813145114U, "ManagedByRemoteExchangeOrganization");
			ServerStrings.stringIDs.Add(1511731857U, "AutoDRequestFailed");
			ServerStrings.stringIDs.Add(3680872945U, "DumpsterFolderNotFound");
			ServerStrings.stringIDs.Add(847028569U, "ExFolderNotFoundInClientState");
			ServerStrings.stringIDs.Add(2399298613U, "ImportResultContainedFailure");
			ServerStrings.stringIDs.Add(4287963350U, "ClientCulture_0x813");
			ServerStrings.stringIDs.Add(1419302978U, "ExCannotCreateMeetingResponse");
			ServerStrings.stringIDs.Add(2061760288U, "EightHours");
			ServerStrings.stringIDs.Add(277994955U, "OperationResultFailed");
			ServerStrings.stringIDs.Add(860625858U, "ErrorWorkingHoursEndTimeSmaller");
			ServerStrings.stringIDs.Add(1977152861U, "RoutingTypeRequired");
			ServerStrings.stringIDs.Add(244097521U, "FolderRuleCannotSaveItem");
			ServerStrings.stringIDs.Add(3695851705U, "RequestStateRemoving");
			ServerStrings.stringIDs.Add(1058417226U, "MigrationStateFailed");
			ServerStrings.stringIDs.Add(2568510612U, "MigrationUserStatusCompletionFailed");
			ServerStrings.stringIDs.Add(1709649557U, "MailboxSearchEwsEmptyResponse");
			ServerStrings.stringIDs.Add(2264323692U, "ClientCulture_0x140A");
			ServerStrings.stringIDs.Add(3414312136U, "MigrationUserStatusRemoving");
			ServerStrings.stringIDs.Add(1240718058U, "MigrationFolderCorruptedItems");
			ServerStrings.stringIDs.Add(4287963475U, "ClientCulture_0x418");
			ServerStrings.stringIDs.Add(1953718728U, "SpellCheckerPortuguesePortugal");
			ServerStrings.stringIDs.Add(3480868172U, "TeamMailboxMessageReactivatingText");
			ServerStrings.stringIDs.Add(1241888079U, "SearchLogFileCreateException");
			ServerStrings.stringIDs.Add(2010446754U, "ExCannotGetDeletedItem");
			ServerStrings.stringIDs.Add(496581163U, "MailboxSearchNameTooLong");
			ServerStrings.stringIDs.Add(4287963597U, "ClientCulture_0x41F");
			ServerStrings.stringIDs.Add(1908093103U, "ClientCulture_0x4409");
			ServerStrings.stringIDs.Add(3877064532U, "ExSubmissionQuotaExceeded");
			ServerStrings.stringIDs.Add(1682139678U, "ExCorruptMessageCorrelationBlob");
			ServerStrings.stringIDs.Add(3199235754U, "MigrationFolderDrumTesting");
			ServerStrings.stringIDs.Add(4044636857U, "ExCorruptFolderWebViewInfo");
			ServerStrings.stringIDs.Add(3987708908U, "MigrationBatchFlagDisableOnCopy");
			ServerStrings.stringIDs.Add(1372163092U, "ICSSynchronizationFailed");
			ServerStrings.stringIDs.Add(4036040747U, "OneHours");
			ServerStrings.stringIDs.Add(1163271202U, "InvalidBodyFormat");
			ServerStrings.stringIDs.Add(3938391037U, "PeopleQuickContactsAttributionDisplayName");
			ServerStrings.stringIDs.Add(1437739311U, "TwoHours");
			ServerStrings.stringIDs.Add(1085572452U, "ExPropertyDefinitionInMoreThanOnePropertyProfile");
			ServerStrings.stringIDs.Add(2260241133U, "TeamMailboxSyncStatusMembershipAndMaintenanceSyncFailure");
			ServerStrings.stringIDs.Add(2721876058U, "ClientCulture_0xC0C");
			ServerStrings.stringIDs.Add(3360384478U, "ExUnableToCopyAttachments");
			ServerStrings.stringIDs.Add(4014822501U, "ExCannotUpdateResponses");
			ServerStrings.stringIDs.Add(3909125539U, "ConversationItemHasNoBody");
			ServerStrings.stringIDs.Add(390622925U, "DelegateCollectionInvalidAfterSave");
			ServerStrings.stringIDs.Add(2946586312U, "TeamMailboxSyncStatusDocumentAndMaintenanceSyncFailure");
			ServerStrings.stringIDs.Add(3712347292U, "InvalidAttachmentType");
			ServerStrings.stringIDs.Add(3049568671U, "ExCannotMarkTaskCompletedWhenSuppressCreateOneOff");
			ServerStrings.stringIDs.Add(748440690U, "CannotGetPropertyList");
			ServerStrings.stringIDs.Add(1090294952U, "ErrorInvalidConfigurationXml");
			ServerStrings.stringIDs.Add(3064401295U, "InvalidBase64String");
			ServerStrings.stringIDs.Add(1573110229U, "RequestStateFailed");
			ServerStrings.stringIDs.Add(403751771U, "InboxRuleImportanceNormal");
			ServerStrings.stringIDs.Add(2718297568U, "MigrationLocalhostNotFound");
			ServerStrings.stringIDs.Add(1551862972U, "ClientCulture_0x1401");
			ServerStrings.stringIDs.Add(2594591409U, "TeamMailboxSyncStatusSucceeded");
			ServerStrings.stringIDs.Add(3678601929U, "MigrationErrorAttachmentCorrupted");
			ServerStrings.stringIDs.Add(974121200U, "OleConversionResultFailed");
			ServerStrings.stringIDs.Add(559182252U, "MigrationUserStatusSummaryFailed");
			ServerStrings.stringIDs.Add(2151518657U, "RequestStateCanceled");
			ServerStrings.stringIDs.Add(1326926126U, "ModifyRuleInStore");
			ServerStrings.stringIDs.Add(1178929403U, "ExItemDeletedInRace");
			ServerStrings.stringIDs.Add(2264323626U, "ClientCulture_0x340A");
			ServerStrings.stringIDs.Add(2482422690U, "WeatherUnitFahrenheit");
			ServerStrings.stringIDs.Add(4014230567U, "MessageNotRightsProtected");
			ServerStrings.stringIDs.Add(704719423U, "ConversionMaliciousContent");
			ServerStrings.stringIDs.Add(3550236610U, "NoTemplateMessage");
			ServerStrings.stringIDs.Add(3838981202U, "FolderRuleStageLoading");
			ServerStrings.stringIDs.Add(2310868878U, "LimitedDetails");
			ServerStrings.stringIDs.Add(3851145766U, "AppendOOFHistoryEntry");
			ServerStrings.stringIDs.Add(1180958385U, "ExStoreSessionDisconnected");
			ServerStrings.stringIDs.Add(1719015848U, "NotificationEmailSubjectCertExpired");
			ServerStrings.stringIDs.Add(1951112992U, "MigrationUserAdminTypeDCAdmin");
			ServerStrings.stringIDs.Add(1511957081U, "SpellCheckerSpanish");
			ServerStrings.stringIDs.Add(693971404U, "UserPhotoNotFound");
			ServerStrings.stringIDs.Add(3647547525U, "ClientCulture_0x1C01");
			ServerStrings.stringIDs.Add(959876171U, "MigrationBatchFlagAutoStop");
			ServerStrings.stringIDs.Add(1561549651U, "RemoteArchiveOffline");
			ServerStrings.stringIDs.Add(89761473U, "CannotOpenLocalFreeBusy");
			ServerStrings.stringIDs.Add(3320490165U, "CannotFindExchangePrincipal");
			ServerStrings.stringIDs.Add(2938839179U, "MigrationStageValidation");
			ServerStrings.stringIDs.Add(1865294994U, "CalNotifTypeReminder");
			ServerStrings.stringIDs.Add(4057666506U, "ExFailedToGetUnsearchableItems");
			ServerStrings.stringIDs.Add(3364213626U, "Monday");
			ServerStrings.stringIDs.Add(135933047U, "AsyncOperationTypeUnknown");
			ServerStrings.stringIDs.Add(3737835697U, "MigrationFolderSyncMigration");
			ServerStrings.stringIDs.Add(950284413U, "InboxRuleFlagStatusComplete");
			ServerStrings.stringIDs.Add(838610512U, "NotificationEmailSubjectMoveMailbox");
			ServerStrings.stringIDs.Add(3803104512U, "MessageRpmsgAttachmentIncorrectType");
			ServerStrings.stringIDs.Add(2897062825U, "FullDetails");
			ServerStrings.stringIDs.Add(4157021563U, "JunkEmailObjectDisposedException");
			ServerStrings.stringIDs.Add(4199032562U, "FailedToReadLocalServer");
			ServerStrings.stringIDs.Add(1207296209U, "MapiCannotGetCurrentRow");
			ServerStrings.stringIDs.Add(1074414016U, "MigrationInvalidPassword");
			ServerStrings.stringIDs.Add(2253780921U, "ExCannotDeletePropertiesOnOccurrences");
			ServerStrings.stringIDs.Add(2322466952U, "EstimateStateFailed");
			ServerStrings.stringIDs.Add(1348198184U, "RequestStateCompleting");
			ServerStrings.stringIDs.Add(4287963600U, "ClientCulture_0x41E");
			ServerStrings.stringIDs.Add(3913958124U, "InvalidSharingRecipientsException");
			ServerStrings.stringIDs.Add(3340405076U, "MapiCannotOpenFolder");
			ServerStrings.stringIDs.Add(536874274U, "ClientCulture_0x180C");
			ServerStrings.stringIDs.Add(1558200374U, "ADOperationAbortedBecauseOfAnotherADThread");
			ServerStrings.stringIDs.Add(1606180860U, "UpdateOOFHistoryOperation");
			ServerStrings.stringIDs.Add(3643161838U, "ExAttachmentAlreadyOpen");
			ServerStrings.stringIDs.Add(1606937438U, "NotificationEmailBodyImportPSTCompleted");
			ServerStrings.stringIDs.Add(2205094141U, "ExInvalidAggregate");
			ServerStrings.stringIDs.Add(2046213432U, "NotificationEmailBodyImportPSTFailed");
			ServerStrings.stringIDs.Add(3350951418U, "ExCantAccessOccurrenceFromSingle");
			ServerStrings.stringIDs.Add(3182347319U, "NotificationEmailBodyExportPSTCompleted");
			ServerStrings.stringIDs.Add(2721879411U, "ClientCulture_0x801");
			ServerStrings.stringIDs.Add(2721879543U, "ClientCulture_0x401");
			ServerStrings.stringIDs.Add(1685125049U, "CalNotifTypeNewUpdate");
			ServerStrings.stringIDs.Add(148503943U, "NoExternalEwsAvailableException");
			ServerStrings.stringIDs.Add(3737728227U, "CannotSharePublicFolder");
			ServerStrings.stringIDs.Add(705409536U, "MigrationTypeExchangeRemoteMove");
			ServerStrings.stringIDs.Add(3160872492U, "MigrationUserStatusCompletedWithWarning");
			ServerStrings.stringIDs.Add(898461441U, "FailedToParseUseLicense");
			ServerStrings.stringIDs.Add(2930877107U, "MigrationStateDisabled");
			ServerStrings.stringIDs.Add(3957837829U, "MigrationBatchSupportedActionComplete");
			ServerStrings.stringIDs.Add(3969194465U, "MapiCannotOpenEmbeddedMessage");
			ServerStrings.stringIDs.Add(2205239196U, "InboxRuleImportanceLow");
			ServerStrings.stringIDs.Add(792524531U, "NoMapiPDLs");
			ServerStrings.stringIDs.Add(1929574072U, "RmExceptionGenericMessage");
			ServerStrings.stringIDs.Add(1065277019U, "NotRead");
			ServerStrings.stringIDs.Add(2721879521U, "ClientCulture_0x80C");
			ServerStrings.stringIDs.Add(913845744U, "idUnableToAddDefaultCalendarToDefaultCalendarGroup");
			ServerStrings.stringIDs.Add(987212902U, "ClientCulture_0x3801");
			ServerStrings.stringIDs.Add(1599639819U, "MapiCannotGetAllPerUserLongTermIds");
			ServerStrings.stringIDs.Add(101151487U, "TeamMailboxMessageSiteMailboxEmailAddress");
			ServerStrings.stringIDs.Add(2267978872U, "CalNotifTypeDeletedUpdate");
			ServerStrings.stringIDs.Add(676596483U, "CannotCreateSearchFoldersInPublicStore");
			ServerStrings.stringIDs.Add(3860059626U, "ExDictionaryDataCorruptedNoField");
			ServerStrings.stringIDs.Add(3414452687U, "ExceptionFolderIsRootFolder");
			ServerStrings.stringIDs.Add(2303439835U, "DateRangeOneDay");
			ServerStrings.stringIDs.Add(4287963481U, "ClientCulture_0x412");
			ServerStrings.stringIDs.Add(2937224961U, "AppointmentTombstoneCorrupt");
			ServerStrings.stringIDs.Add(627389694U, "MigrationBatchAutoComplete");
			ServerStrings.stringIDs.Add(3368210174U, "MigrationObjectsCountStringNone");
			ServerStrings.stringIDs.Add(4287963351U, "ClientCulture_0x810");
			ServerStrings.stringIDs.Add(1064525218U, "MapiCannotCopyItem");
			ServerStrings.stringIDs.Add(1405495556U, "ErrorNoStoreObjectIdAndFolderPath");
			ServerStrings.stringIDs.Add(805861954U, "MigrationFolderSyncMigrationReports");
			ServerStrings.stringIDs.Add(2576334771U, "UnsupportedFormsCondition");
			ServerStrings.stringIDs.Add(3719801171U, "ExStartTimeNotSet");
			ServerStrings.stringIDs.Add(2721879406U, "ClientCulture_0x804");
			ServerStrings.stringIDs.Add(718334902U, "ExConversationActionItemNotFound");
			ServerStrings.stringIDs.Add(3108568302U, "MigrationUserStatusProvisioning");
			ServerStrings.stringIDs.Add(951826856U, "InboxRuleMessageTypeNonDeliveryReport");
			ServerStrings.stringIDs.Add(1983194762U, "ExFailedToUnregisterExchangeTopologyNotification");
			ServerStrings.stringIDs.Add(1453422183U, "TeamMailboxMessageLearnMore");
			ServerStrings.stringIDs.Add(1701953962U, "DateRangeThreeDays");
			ServerStrings.stringIDs.Add(2423165282U, "MapiCannotTransportSendMessage");
			ServerStrings.stringIDs.Add(1627212429U, "ExSortNotSupportedInDeepTraversalQuery");
			ServerStrings.stringIDs.Add(1452910403U, "JunkEmailAmbiguousUsernameException");
			ServerStrings.stringIDs.Add(3249771727U, "MigrationBatchStatusSyncedWithErrors");
			ServerStrings.stringIDs.Add(1714411372U, "FailedToFindAvailableHubs");
			ServerStrings.stringIDs.Add(3876212982U, "InternetCalendarName");
			ServerStrings.stringIDs.Add(11761379U, "ExItemNotFound");
			ServerStrings.stringIDs.Add(1042040859U, "ExDelegateNotSupportedRespondToMeetingRequest");
			ServerStrings.stringIDs.Add(4247577850U, "DisposeNonIPMFolder");
			ServerStrings.stringIDs.Add(2585840702U, "InboxRuleSensitivityPrivate");
			ServerStrings.stringIDs.Add(1728869634U, "MigrationFeatureNone");
			ServerStrings.stringIDs.Add(2721879547U, "ClientCulture_0x405");
			ServerStrings.stringIDs.Add(46060694U, "ExStringContainsSurroundingWhiteSpace");
			ServerStrings.stringIDs.Add(1489962160U, "ClientCulture_0x4C0A");
			ServerStrings.stringIDs.Add(630982608U, "ClientCulture_0x1809");
			ServerStrings.stringIDs.Add(2138889051U, "FailedToResealKey");
			ServerStrings.stringIDs.Add(2342985388U, "InvalidParticipantForRules");
			ServerStrings.stringIDs.Add(3831935814U, "SpellCheckerDanish");
			ServerStrings.stringIDs.Add(1280140891U, "MapiCannotGetProperties");
			ServerStrings.stringIDs.Add(1835617035U, "MapiCopyMessagesFailed");
			ServerStrings.stringIDs.Add(2765862542U, "FailedToParseValue");
			ServerStrings.stringIDs.Add(1208307535U, "RuleWriterObjectNotFound");
			ServerStrings.stringIDs.Add(975909957U, "ExInvalidWatermarkString");
			ServerStrings.stringIDs.Add(3000070570U, "ProvisioningRequestCsvContainsBothPasswordAndFederatedIdentity");
			ServerStrings.stringIDs.Add(3348992335U, "OperationResultSucceeded");
			ServerStrings.stringIDs.Add(141464842U, "ServerLocatorServicePermanentFault");
			ServerStrings.stringIDs.Add(1039613051U, "NotMailboxSession");
			ServerStrings.stringIDs.Add(1391852443U, "MigrationStateActive");
			ServerStrings.stringIDs.Add(1743625299U, "Null");
			ServerStrings.stringIDs.Add(1066460788U, "FolderRuleStageOnCreatedMessage");
			ServerStrings.stringIDs.Add(3238359831U, "CannotSetMessageFlags");
			ServerStrings.stringIDs.Add(1511089307U, "ExInvalidAsyncResult");
			ServerStrings.stringIDs.Add(987212805U, "ClientCulture_0x2801");
			ServerStrings.stringIDs.Add(3815733289U, "ExFolderPropertyBagCannotSaveChanges");
			ServerStrings.stringIDs.Add(1665468465U, "MigrationBatchStatusStopped");
			ServerStrings.stringIDs.Add(1852466064U, "KqlParserTimeout");
			ServerStrings.stringIDs.Add(3623312858U, "TenMinutes");
			ServerStrings.stringIDs.Add(2730199010U, "ExMustSetSearchCriteriaToMakeVisibleToOutlook");
			ServerStrings.stringIDs.Add(3452652986U, "Wednesday");
			ServerStrings.stringIDs.Add(2721875974U, "ClientCulture_0xC07");
			ServerStrings.stringIDs.Add(3155538965U, "LegacyMailboxSearchDescription");
			ServerStrings.stringIDs.Add(1494015926U, "MapiCannotFreeBookmark");
			ServerStrings.stringIDs.Add(4041018166U, "CannotChangePermissionsOnFolder");
			ServerStrings.stringIDs.Add(14329882U, "MapiCannotSetProps");
			ServerStrings.stringIDs.Add(1082563208U, "SearchStatePartiallySucceeded");
			ServerStrings.stringIDs.Add(1720321054U, "InboxRuleMessageTypeAutomaticReply");
			ServerStrings.stringIDs.Add(270030022U, "RuleHistoryError");
			ServerStrings.stringIDs.Add(1699673525U, "ClientCulture_0x280A");
			ServerStrings.stringIDs.Add(2116512910U, "ClientCulture_0x3001");
			ServerStrings.stringIDs.Add(422603853U, "SharePoint");
			ServerStrings.stringIDs.Add(3562004828U, "NoDelegateAction");
			ServerStrings.stringIDs.Add(3008535783U, "MigrationFlagsStop");
			ServerStrings.stringIDs.Add(2280589817U, "ExNoOptimizedCodePath");
			ServerStrings.stringIDs.Add(2285862258U, "MigrationBatchFlagUseAdvancedValidation");
			ServerStrings.stringIDs.Add(721699019U, "MapiCannotGetParentEntryId");
			ServerStrings.stringIDs.Add(3915122871U, "ExOnlyMessagesHaveParent");
			ServerStrings.stringIDs.Add(4287963484U, "ClientCulture_0x411");
			ServerStrings.stringIDs.Add(1405345367U, "ExFolderDoesNotMatchFolderId");
			ServerStrings.stringIDs.Add(2992671490U, "MigrationTypeXO1");
			ServerStrings.stringIDs.Add(273956641U, "NotOperator");
			ServerStrings.stringIDs.Add(1699673459U, "ClientCulture_0x480A");
			ServerStrings.stringIDs.Add(3478111469U, "Saturday");
			ServerStrings.stringIDs.Add(4173454943U, "ExFailedToRegisterExchangeTopologyNotification");
			ServerStrings.stringIDs.Add(1732871098U, "MigrationBatchSupportedActionSet");
			ServerStrings.stringIDs.Add(552541799U, "ConversionCannotOpenJournalMessage");
			ServerStrings.stringIDs.Add(2519002207U, "JunkEmailTrustedListXsoEmptyException");
			ServerStrings.stringIDs.Add(2181826069U, "AmFailedToFindSuitableServer");
			ServerStrings.stringIDs.Add(1299125360U, "OperationalError");
			ServerStrings.stringIDs.Add(2689882193U, "ExTooManySortColumns");
			ServerStrings.stringIDs.Add(3810093794U, "LoadRulesFromStore");
			ServerStrings.stringIDs.Add(2721879658U, "ClientCulture_0x40D");
			ServerStrings.stringIDs.Add(1282190256U, "ExCantCopyBadAlienDLMember");
			ServerStrings.stringIDs.Add(3434190160U, "TeamMailboxMessageSendMailToTheSiteMailbox");
			ServerStrings.stringIDs.Add(3610984697U, "InboxRuleSensitivityPersonal");
			ServerStrings.stringIDs.Add(2078392877U, "ExInvalidItemCountAdvisorCondition");
			ServerStrings.stringIDs.Add(3955579167U, "ErrorLoadingRules");
			ServerStrings.stringIDs.Add(4287963487U, "ClientCulture_0x414");
			ServerStrings.stringIDs.Add(1246168046U, "ExEndTimeNotSet");
			ServerStrings.stringIDs.Add(1055197681U, "InboxRuleMessageTypeAutomaticForward");
			ServerStrings.stringIDs.Add(392605258U, "MapiCannotCopyMapiProps");
			ServerStrings.stringIDs.Add(2708396435U, "OneWeeks");
			ServerStrings.stringIDs.Add(494393471U, "TeamMailboxMessageWhatYouCanDoNext");
			ServerStrings.stringIDs.Add(1985781485U, "MapiCannotGetReceiveFolderInfo");
			ServerStrings.stringIDs.Add(2870966119U, "ExInvalidStoreObjectId");
			ServerStrings.stringIDs.Add(1332658223U, "RequestStateQueued");
			ServerStrings.stringIDs.Add(1762000491U, "RecurrenceBlobCorrupted");
			ServerStrings.stringIDs.Add(3526072757U, "CannotFindAttachment");
			ServerStrings.stringIDs.Add(2356618506U, "ExInvalidRecipientBlob");
			ServerStrings.stringIDs.Add(1272751292U, "ExIncompleteBlob");
			ServerStrings.stringIDs.Add(379946208U, "ExPatternNotSet");
			ServerStrings.stringIDs.Add(1944490681U, "ExInvalidDayOfMonth");
			ServerStrings.stringIDs.Add(3835493555U, "ExInvalidGlobalObjectId");
			ServerStrings.stringIDs.Add(2185974115U, "MapiCannotGetHierarchyTable");
			ServerStrings.stringIDs.Add(2721876056U, "ClientCulture_0xC0A");
			ServerStrings.stringIDs.Add(2799724840U, "ExInvalidFullyQualifiedServerName");
			ServerStrings.stringIDs.Add(751268339U, "EstimateStateInProgress");
			ServerStrings.stringIDs.Add(919965275U, "MapiCannotSetReadFlags");
			ServerStrings.stringIDs.Add(2710414285U, "PublicFolderQueryStatusSyncFolderHierarchyRpcFailed");
			ServerStrings.stringIDs.Add(3367423950U, "ExInvalidSearchFolderScope");
			ServerStrings.stringIDs.Add(298783488U, "ActivitySessionIsNull");
			ServerStrings.stringIDs.Add(3325987673U, "SpellCheckerItalian");
			ServerStrings.stringIDs.Add(3248071036U, "FolderRuleStageOnPublicFolderAfter");
			ServerStrings.stringIDs.Add(1964131369U, "NotAllowedAnonymousSharingByPolicy");
			ServerStrings.stringIDs.Add(1214453788U, "MapiCannotGetCollapseState");
			ServerStrings.stringIDs.Add(3651846103U, "ZeroMinutes");
			ServerStrings.stringIDs.Add(3749282747U, "RecipientNotSupportedByAnyProviderException");
			ServerStrings.stringIDs.Add(1970390563U, "MigrationReportFinalizationSuccess");
			ServerStrings.stringIDs.Add(3878608135U, "CalNotifTypeChangedUpdate");
			ServerStrings.stringIDs.Add(3697440372U, "ExSizeFilterPropertyNotSupported");
			ServerStrings.stringIDs.Add(3733109861U, "ExConversationActionInvalidFolderType");
			ServerStrings.stringIDs.Add(2472743173U, "ClientCulture_0x2009");
			ServerStrings.stringIDs.Add(2230033988U, "UserDiscoveryMailboxNotFound");
			ServerStrings.stringIDs.Add(314838903U, "ExCorruptedRecurringCalItem");
			ServerStrings.stringIDs.Add(780978771U, "ExStartDateLaterThanEndDate");
			ServerStrings.stringIDs.Add(2635186097U, "ExInvalidOrganizer");
			ServerStrings.stringIDs.Add(3162466950U, "MaxExclusionReached");
			ServerStrings.stringIDs.Add(291587104U, "JunkEmailBlockedListXsoEmptyException");
			ServerStrings.stringIDs.Add(4224027968U, "SearchStateSucceeded");
			ServerStrings.stringIDs.Add(4078034164U, "ExInvalidJournalReportFormat");
			ServerStrings.stringIDs.Add(1049215562U, "RequestStateCertExpiring");
			ServerStrings.stringIDs.Add(3206936715U, "ConversionBodyConversionFailed");
			ServerStrings.stringIDs.Add(167159909U, "MigrationUserAdminTypeTenantAdmin");
			ServerStrings.stringIDs.Add(2721879652U, "ClientCulture_0x40B");
			ServerStrings.stringIDs.Add(871369479U, "ConversionEmptyAddress");
			ServerStrings.stringIDs.Add(1699673622U, "ClientCulture_0x380A");
			ServerStrings.stringIDs.Add(2430821738U, "PublicFoldersCannotBeAccessedDuringCompletion");
			ServerStrings.stringIDs.Add(845797650U, "MigrationReportUnknown");
			ServerStrings.stringIDs.Add(2220118463U, "FolderRuleResolvingAddressBookEntryId");
			ServerStrings.stringIDs.Add(2430200359U, "SharePointLifecyclePolicy");
			ServerStrings.stringIDs.Add(1296109892U, "MapiCannotAddNotification");
			ServerStrings.stringIDs.Add(2828973533U, "ClientCulture_0x200A");
			ServerStrings.stringIDs.Add(1595290732U, "ExFailedToCreateEventManager");
			ServerStrings.stringIDs.Add(2693957296U, "MaixmumNumberOfMailboxAssociationsForUserReached");
			ServerStrings.stringIDs.Add(4067432976U, "ExInvalidEIT");
			ServerStrings.stringIDs.Add(3176034413U, "ExFilterAndSortNotSupportedInSimpleVirtualPropertyDefinition");
			ServerStrings.stringIDs.Add(2864562615U, "DateRangeOneMonth");
			ServerStrings.stringIDs.Add(1299122915U, "MapiInvalidParam");
			ServerStrings.stringIDs.Add(94201836U, "MapiCannotDeleteUserPhoto");
			ServerStrings.stringIDs.Add(3755817478U, "MigrationUserAdminTypePartner");
			ServerStrings.stringIDs.Add(1504384645U, "ExOrganizerCannotCallUpdateCalendarItem");
			ServerStrings.stringIDs.Add(1680240084U, "DuplicateCondition");
			ServerStrings.stringIDs.Add(1817477041U, "JunkEmailTrustedListXsoTooManyException");
			ServerStrings.stringIDs.Add(1076453993U, "VersionNotInteger");
			ServerStrings.stringIDs.Add(4287963596U, "ClientCulture_0x41A");
			ServerStrings.stringIDs.Add(3423699388U, "MapiCannotGetNamedProperties");
			ServerStrings.stringIDs.Add(543137909U, "ExFailedToDeleteDefaultFolder");
			ServerStrings.stringIDs.Add(1775042376U, "ExDefaultFoldersNotInitialized");
			ServerStrings.stringIDs.Add(2045069482U, "AsyncOperationTypeCertExpiry");
			ServerStrings.stringIDs.Add(3561051015U, "ClosingTagExpectedNoneFound");
			ServerStrings.stringIDs.Add(4098965384U, "FolderRuleStageEvaluation");
			ServerStrings.stringIDs.Add(4287963347U, "ClientCulture_0x814");
			ServerStrings.stringIDs.Add(4024171282U, "TeamMailboxMessageMemberInvitationBodyIntroText");
			ServerStrings.stringIDs.Add(2721875968U, "ClientCulture_0xC09");
			ServerStrings.stringIDs.Add(3683655246U, "UnexpectedToken");
			ServerStrings.stringIDs.Add(3146200960U, "MapiCannotSeekRowBookmark");
			ServerStrings.stringIDs.Add(2294313398U, "PublicFolderSyncFolderRpcFailed");
			ServerStrings.stringIDs.Add(1106682409U, "MigrationFlagsNone");
			ServerStrings.stringIDs.Add(3045858265U, "JunkEmailTrustedListXsoNullException");
			ServerStrings.stringIDs.Add(2855609935U, "MapiCannotQueryRows");
			ServerStrings.stringIDs.Add(147262116U, "MigrationTestMSASuccess");
			ServerStrings.stringIDs.Add(732586480U, "MigrationMailboxPermissionAdmin");
			ServerStrings.stringIDs.Add(1719335392U, "ExInvalidItemId");
			ServerStrings.stringIDs.Add(3009511836U, "MapiCannotSetSpooler");
			ServerStrings.stringIDs.Add(1932453382U, "InvalidSharingTargetRecipientException");
			ServerStrings.stringIDs.Add(1816014188U, "RequestStateInProgress");
			ServerStrings.stringIDs.Add(4127188442U, "UnlockOOFHistory");
			ServerStrings.stringIDs.Add(559598517U, "ExMclCannotBeResolved");
			ServerStrings.stringIDs.Add(3224364440U, "FailedToReadConfiguration");
			ServerStrings.stringIDs.Add(749132645U, "ADUserNoMailbox");
			ServerStrings.stringIDs.Add(1136597198U, "ExReadExchangeTopologyFailed");
			ServerStrings.stringIDs.Add(152248145U, "MigrationUserStatusCorrupted");
			ServerStrings.stringIDs.Add(4055176570U, "ExMatchShouldHaveBeenCalled");
			ServerStrings.stringIDs.Add(3282126921U, "ExCannotModifyRemovedRecipient");
			ServerStrings.stringIDs.Add(3306331751U, "InvalidDateTimeFormat");
			ServerStrings.stringIDs.Add(3270231634U, "OleConversionPrepareFailed");
			ServerStrings.stringIDs.Add(2098990361U, "NotificationEmailBodyExportPSTFailed");
			ServerStrings.stringIDs.Add(1342934277U, "ExRangeNotSet");
			ServerStrings.stringIDs.Add(2096844093U, "OrganizationNotFederatedException");
			ServerStrings.stringIDs.Add(1559080129U, "ClientCulture_0x421");
			ServerStrings.stringIDs.Add(1453305798U, "ACLTooBig");
			ServerStrings.stringIDs.Add(1949750262U, "TeamMailboxMessageNotConnectedToSite");
			ServerStrings.stringIDs.Add(77059561U, "TeamMailboxSyncStatusFailed");
			ServerStrings.stringIDs.Add(2721879523U, "ClientCulture_0x80A");
			ServerStrings.stringIDs.Add(713774522U, "FailedToBindToUseLicense");
			ServerStrings.stringIDs.Add(3349952875U, "MapiCannotGetParentId");
			ServerStrings.stringIDs.Add(3964486357U, "ExEndDateEarlierThanStartDate");
			ServerStrings.stringIDs.Add(2183577153U, "ExInvalidCustomSerializationData");
			ServerStrings.stringIDs.Add(1559063653U, "ConversionLimitsExceeded");
			ServerStrings.stringIDs.Add(1403049999U, "LargeMultivaluedPropertiesNotSupportedInTNEF");
			ServerStrings.stringIDs.Add(3892201190U, "MigrationSkippableStepSettingTargetAddress");
			ServerStrings.stringIDs.Add(3041066937U, "InvalidPropertyKey");
			ServerStrings.stringIDs.Add(1512671255U, "TwoDays");
			ServerStrings.stringIDs.Add(1027306046U, "ExOutlookSearchFolderDoesNotHaveMailboxSession");
			ServerStrings.stringIDs.Add(1559080130U, "ClientCulture_0x426");
			ServerStrings.stringIDs.Add(1572145744U, "MalformedAdrEntry");
			ServerStrings.stringIDs.Add(1033019572U, "FailedToFindIssuanceLicenseAndURI");
			ServerStrings.stringIDs.Add(4010739301U, "InboxRuleMessageTypeEncrypted");
			ServerStrings.stringIDs.Add(927529551U, "SuffixMatchNotSupported");
			ServerStrings.stringIDs.Add(1799732388U, "idClientSessionInfoParseException");
			ServerStrings.stringIDs.Add(2472743336U, "ClientCulture_0x1009");
			ServerStrings.stringIDs.Add(2107406877U, "OperationResultPartiallySucceeded");
			ServerStrings.stringIDs.Add(628765410U, "UceContentFilterLoadFailure");
			ServerStrings.stringIDs.Add(1228234464U, "MapiRulesError");
			ServerStrings.stringIDs.Add(2423017895U, "MigrationBatchStatusRemoving");
			ServerStrings.stringIDs.Add(2503716610U, "ExBadFolderEntryIdSize");
			ServerStrings.stringIDs.Add(3097513127U, "NotSupportedSharingMessageException");
			ServerStrings.stringIDs.Add(885639843U, "IncompleteExchangePrincipal");
			ServerStrings.stringIDs.Add(2444446273U, "MigrationFeatureMultiBatch");
			ServerStrings.stringIDs.Add(2889367749U, "MapiCannotSetTableColumns");
			ServerStrings.stringIDs.Add(3398202471U, "FailedToReadActivityLog");
			ServerStrings.stringIDs.Add(2045304318U, "SearchOperationFailed");
			ServerStrings.stringIDs.Add(3997956246U, "ExDefaultJournalFilename");
			ServerStrings.stringIDs.Add(1407904664U, "MapiCannotExpandRow");
			ServerStrings.stringIDs.Add(3375709410U, "ExCannotStartDeadSessionChecking");
			ServerStrings.stringIDs.Add(2953075549U, "SearchStateNotStarted");
			ServerStrings.stringIDs.Add(2989293604U, "SpellCheckerSwedish");
			ServerStrings.stringIDs.Add(3084838222U, "NotificationEmailSubjectExportPst");
			ServerStrings.stringIDs.Add(2174906965U, "ContentRestrictionOnSearchKey");
			ServerStrings.stringIDs.Add(671447089U, "ExUnknownFilterType");
			ServerStrings.stringIDs.Add(2828973467U, "ClientCulture_0x400A");
			ServerStrings.stringIDs.Add(3647529948U, "UnableToLoadDrmMessage");
			ServerStrings.stringIDs.Add(4048687496U, "ExCannotParseValue");
			ServerStrings.stringIDs.Add(2116512747U, "ClientCulture_0x4001");
			ServerStrings.stringIDs.Add(1918332060U, "TeamMailboxMessageToLearnMore");
			ServerStrings.stringIDs.Add(1272465216U, "MigrationBatchStatusCompleting");
			ServerStrings.stringIDs.Add(2453169079U, "MapiCannotGetRowCount");
			ServerStrings.stringIDs.Add(3849632232U, "FolderRuleStageOnDeliveredMessage");
			ServerStrings.stringIDs.Add(2614560510U, "MigrationBatchSupportedActionAppend");
			ServerStrings.stringIDs.Add(1858795727U, "ExCannotOpenRejectedItem");
			ServerStrings.stringIDs.Add(82143339U, "MigrationBatchStatusFailed");
			ServerStrings.stringIDs.Add(597150166U, "VotingDataCorrupt");
			ServerStrings.stringIDs.Add(3941673604U, "MigrationTestMSAFailed");
			ServerStrings.stringIDs.Add(656484203U, "TeamMailboxSyncStatusDocumentAndMembershipSyncFailure");
			ServerStrings.stringIDs.Add(460612780U, "PublicFoldersCannotBeMovedDuringMigration");
			ServerStrings.stringIDs.Add(480886895U, "ConversionNoReplayContent");
			ServerStrings.stringIDs.Add(2721879546U, "ClientCulture_0x404");
			ServerStrings.stringIDs.Add(495633109U, "ExCantSubmitWithoutRecipients");
			ServerStrings.stringIDs.Add(1614900577U, "LastErrorMessage");
			ServerStrings.stringIDs.Add(2849660304U, "JunkEmailFolderNotFoundException");
			ServerStrings.stringIDs.Add(1559080241U, "ClientCulture_0x42A");
			ServerStrings.stringIDs.Add(3133024367U, "AmMoveNotApplicableForDbException");
			ServerStrings.stringIDs.Add(3710501592U, "ExCantUndeleteOccurrence");
			ServerStrings.stringIDs.Add(2418303566U, "MigrationUserStatusQueued");
			ServerStrings.stringIDs.Add(4210093826U, "DateRangeSixMonths");
			ServerStrings.stringIDs.Add(1365634215U, "MigrationSkippableStepNone");
			ServerStrings.stringIDs.Add(3746574982U, "MalformedCommentRestriction");
			ServerStrings.stringIDs.Add(1559080131U, "ClientCulture_0x427");
			ServerStrings.stringIDs.Add(2702427225U, "MigrationUserStatusSummaryActive");
			ServerStrings.stringIDs.Add(2343856628U, "ErrorEmptyFolderNotSupported");
			ServerStrings.stringIDs.Add(2264323560U, "ClientCulture_0x540A");
			ServerStrings.stringIDs.Add(1489962226U, "ClientCulture_0x2C0A");
			ServerStrings.stringIDs.Add(3302221775U, "MissingOperand");
			ServerStrings.stringIDs.Add(1949879021U, "DuplicateAction");
			ServerStrings.stringIDs.Add(2882103486U, "SearchStateQueued");
			ServerStrings.stringIDs.Add(3607950093U, "ExQueryPropertyBagRowNotSet");
			ServerStrings.stringIDs.Add(1073167130U, "Sunday");
			ServerStrings.stringIDs.Add(3059751105U, "WeatherUnitDefault");
			ServerStrings.stringIDs.Add(3120595407U, "RequestSecurityTokenException");
			ServerStrings.stringIDs.Add(1768187831U, "ExNoOccurrencesInRecurrence");
			ServerStrings.stringIDs.Add(276036243U, "ExInvalidMclXml");
			ServerStrings.stringIDs.Add(4168806856U, "ExInvalidIdFormat");
			ServerStrings.stringIDs.Add(2161223297U, "ExAdminAuditLogsFolderAccessDenied");
			ServerStrings.stringIDs.Add(2721875973U, "ClientCulture_0xC04");
			ServerStrings.stringIDs.Add(4170247214U, "ExInvalidComparisionOperatorInPropertyComparisionFilter");
			ServerStrings.stringIDs.Add(1937417240U, "AsyncOperationTypeExportPST");
		}

		public static LocalizedString InvalidReceiveMeetingMessageCopiesException(string delegateUser)
		{
			return new LocalizedString("InvalidReceiveMeetingMessageCopiesException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				delegateUser
			});
		}

		public static LocalizedString MissingRightsManagementLicense
		{
			get
			{
				return new LocalizedString("MissingRightsManagementLicense", "ExD831F7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerLocatorServiceTransientFault
		{
			get
			{
				return new LocalizedString("ServerLocatorServiceTransientFault", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDatabaseADException(string dbName, string error)
		{
			return new LocalizedString("AmDatabaseADException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dbName,
				error
			});
		}

		public static LocalizedString EmailAddressMissing
		{
			get
			{
				return new LocalizedString("EmailAddressMissing", "ExE5B1C8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotShareSearchFolder
		{
			get
			{
				return new LocalizedString("CannotShareSearchFolder", "Ex5B8087", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EstimateStateStopping
		{
			get
			{
				return new LocalizedString("EstimateStateStopping", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerDutch
		{
			get
			{
				return new LocalizedString("SpellCheckerDutch", "Ex75A03E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerNorwegianNynorsk
		{
			get
			{
				return new LocalizedString("SpellCheckerNorwegianNynorsk", "ExF0B996", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressAndOriginMismatch(object origin)
		{
			return new LocalizedString("AddressAndOriginMismatch", "ExA7983D", false, true, ServerStrings.ResourceManager, new object[]
			{
				origin
			});
		}

		public static LocalizedString RepairingIsNotApplicableForCurrentMonitorState(string monitorName, string targetResource, string alertState)
		{
			return new LocalizedString("RepairingIsNotApplicableForCurrentMonitorState", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				monitorName,
				targetResource,
				alertState
			});
		}

		public static LocalizedString MigrationInvalidTargetAddress(string email)
		{
			return new LocalizedString("MigrationInvalidTargetAddress", "ExE23BF5", false, true, ServerStrings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString MigrationFlagsStart
		{
			get
			{
				return new LocalizedString("MigrationFlagsStart", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListXsoFormatException(string value)
		{
			return new LocalizedString("JunkEmailTrustedListXsoFormatException", "Ex1B39F7", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString TeamMailboxMessageSiteAndSiteMailboxDetails
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageSiteAndSiteMailboxDetails", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetSupportedRoutingTypes
		{
			get
			{
				return new LocalizedString("CannotGetSupportedRoutingTypes", "Ex7EB29B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSyncStateAlreadyExists(string syncStateName)
		{
			return new LocalizedString("ExSyncStateAlreadyExists", "Ex835B84", false, true, ServerStrings.ResourceManager, new object[]
			{
				syncStateName
			});
		}

		public static LocalizedString FailedToAcquireFederationRac(string tenantId, Uri uri)
		{
			return new LocalizedString("FailedToAcquireFederationRac", "Ex8251B9", false, true, ServerStrings.ResourceManager, new object[]
			{
				tenantId,
				uri
			});
		}

		public static LocalizedString SaveFailureAfterPromotion(string uid)
		{
			return new LocalizedString("SaveFailureAfterPromotion", "Ex9EBF5C", false, true, ServerStrings.ResourceManager, new object[]
			{
				uid
			});
		}

		public static LocalizedString ClientCulture_0x816
		{
			get
			{
				return new LocalizedString("ClientCulture_0x816", "ExDC0F9D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCannotSyncHoldObjectManagedByOtherOrg(string name, string currentOrg, string objOrg)
		{
			return new LocalizedString("ErrorCannotSyncHoldObjectManagedByOtherOrg", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name,
				currentOrg,
				objOrg
			});
		}

		public static LocalizedString TaskOperationFailedWithEcException(int ec)
		{
			return new LocalizedString("TaskOperationFailedWithEcException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString AsyncOperationTypeMailboxRestore
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeMailboxRestore", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MatchContainerClassValidationFailed
		{
			get
			{
				return new LocalizedString("MatchContainerClassValidationFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotCreateSubfolderUnderSearchFolder
		{
			get
			{
				return new LocalizedString("ExCannotCreateSubfolderUnderSearchFolder", "Ex27B20C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleImportanceHigh
		{
			get
			{
				return new LocalizedString("InboxRuleImportanceHigh", "Ex2DEA34", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCopyFailedProperties
		{
			get
			{
				return new LocalizedString("MapiCopyFailedProperties", "ExC43ADE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x3C0A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x3C0A", "Ex81C073", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTeamMailboxGetUsersNullResponse
		{
			get
			{
				return new LocalizedString("ErrorTeamMailboxGetUsersNullResponse", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchSupportedActionNone
		{
			get
			{
				return new LocalizedString("MigrationBatchSupportedActionNone", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoFileTooLarge(int limit)
		{
			return new LocalizedString("UserPhotoFileTooLarge", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString ExternalIdentityInconsistentSid(string mailbox, string entryId, string currentSid, string stringNewSid)
		{
			return new LocalizedString("ExternalIdentityInconsistentSid", "Ex1995BD", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox,
				entryId,
				currentSid,
				stringNewSid
			});
		}

		public static LocalizedString ExAuditsUpdateDenied
		{
			get
			{
				return new LocalizedString("ExAuditsUpdateDenied", "ExC1463B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintFlushNotSatisfied(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid, object utcCommitTime, object replicationTime)
		{
			return new LocalizedString("DataMoveReplicationConstraintFlushNotSatisfied", "Ex09A030", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid,
				utcCommitTime,
				replicationTime
			});
		}

		public static LocalizedString ExBadMessageEntryIdSize
		{
			get
			{
				return new LocalizedString("ExBadMessageEntryIdSize", "Ex54225F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFoldersNotEnabledForTenant(string org)
		{
			return new LocalizedString("PublicFoldersNotEnabledForTenant", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				org
			});
		}

		public static LocalizedString ExAdminAuditLogsUpdateDenied
		{
			get
			{
				return new LocalizedString("ExAdminAuditLogsUpdateDenied", "Ex7B50FB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidNumberOfOccurrences
		{
			get
			{
				return new LocalizedString("ExInvalidNumberOfOccurrences", "Ex99D03F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobConnectionSettingsIncomplete(string fieldName)
		{
			return new LocalizedString("MigrationJobConnectionSettingsIncomplete", "Ex4889F7", false, true, ServerStrings.ResourceManager, new object[]
			{
				fieldName
			});
		}

		public static LocalizedString JunkEmailBlockedListOwnersEmailAddressException(string value)
		{
			return new LocalizedString("JunkEmailBlockedListOwnersEmailAddressException", "Ex93027C", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ExServerNotInSite(string serverName)
		{
			return new LocalizedString("ExServerNotInSite", "ExC143F9", false, true, ServerStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString OleConversionFailed
		{
			get
			{
				return new LocalizedString("OleConversionFailed", "Ex438E33", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x3401
		{
			get
			{
				return new LocalizedString("ClientCulture_0x3401", "Ex547263", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotReadFolderData
		{
			get
			{
				return new LocalizedString("ExCannotReadFolderData", "ExB2C80E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleSensitivityNormal
		{
			get
			{
				return new LocalizedString("InboxRuleSensitivityNormal", "ExC3B29E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerCatalan
		{
			get
			{
				return new LocalizedString("SpellCheckerCatalan", "ExC9BC78", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageHowToGetStarted
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageHowToGetStarted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMasterValueAndColumnLength
		{
			get
			{
				return new LocalizedString("ExInvalidMasterValueAndColumnLength", "Ex0EA1D6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSharingMessageException(string property)
		{
			return new LocalizedString("InvalidSharingMessageException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString MigrationBatchStatusStopping
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusStopping", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x440A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x440A", "Ex9C0E65", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFolderAlreadyExistsInClientState
		{
			get
			{
				return new LocalizedString("ExFolderAlreadyExistsInClientState", "Ex2386C6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPermissionsEntry
		{
			get
			{
				return new LocalizedString("InvalidPermissionsEntry", "ExF7C3B8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionInternalFailure
		{
			get
			{
				return new LocalizedString("ConversionInternalFailure", "ExD5AF06", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotObtainSynchronizationUploadState(Type mapiCollectorType)
		{
			return new LocalizedString("CannotObtainSynchronizationUploadState", "ExD6FAB7", false, true, ServerStrings.ResourceManager, new object[]
			{
				mapiCollectorType
			});
		}

		public static LocalizedString ExFolderDeletePropsFailed(string exceptionMessage)
		{
			return new LocalizedString("ExFolderDeletePropsFailed", "Ex7AB4F1", false, true, ServerStrings.ResourceManager, new object[]
			{
				exceptionMessage
			});
		}

		public static LocalizedString MigrationTypeExchangeOutlookAnywhere
		{
			get
			{
				return new LocalizedString("MigrationTypeExchangeOutlookAnywhere", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConfigTypeNotMatched(string definedType, string usedType)
		{
			return new LocalizedString("ExConfigTypeNotMatched", "Ex7FF538", false, true, ServerStrings.ResourceManager, new object[]
			{
				definedType,
				usedType
			});
		}

		public static LocalizedString UserCannotBeFoundFromContext(int errorCode)
		{
			return new LocalizedString("UserCannotBeFoundFromContext", "Ex485AC1", false, true, ServerStrings.ResourceManager, new object[]
			{
				errorCode
			});
		}

		public static LocalizedString ClientCulture_0x4809
		{
			get
			{
				return new LocalizedString("ClientCulture_0x4809", "Ex7E27E4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationAutodiscoverConfigurationFailure
		{
			get
			{
				return new LocalizedString("MigrationAutodiscoverConfigurationFailure", "ExA6C5F6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDefaultContactFilename
		{
			get
			{
				return new LocalizedString("ExDefaultContactFilename", "Ex6280F9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListXsoGenericException(string value)
		{
			return new LocalizedString("JunkEmailBlockedListXsoGenericException", "Ex56BA32", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString TeamMailboxMessageReopenClosedSiteMailbox
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageReopenClosedSiteMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCreateFolder(string name)
		{
			return new LocalizedString("MapiCannotCreateFolder", "ExD0BCE9", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MigrationObjectsCountStringPFs
		{
			get
			{
				return new LocalizedString("MigrationObjectsCountStringPFs", "Ex190242", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotCreateRecurringMeetingWithoutTimeZone
		{
			get
			{
				return new LocalizedString("ExCannotCreateRecurringMeetingWithoutTimeZone", "Ex890DB1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidSaveOnCorrelatedItem
		{
			get
			{
				return new LocalizedString("ExInvalidSaveOnCorrelatedItem", "Ex0B95C5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCantDeleteOpenedOccurrence(object occId)
		{
			return new LocalizedString("ExCantDeleteOpenedOccurrence", "ExFB5989", false, true, ServerStrings.ResourceManager, new object[]
			{
				occId
			});
		}

		public static LocalizedString ErrorTeamMailboxGetListItemChangesNullResponse
		{
			get
			{
				return new LocalizedString("ErrorTeamMailboxGetListItemChangesNullResponse", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCorruptedTimeZone
		{
			get
			{
				return new LocalizedString("ExCorruptedTimeZone", "ExB67D00", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BindToWrongObjectType(string objectClass, string intendedType)
		{
			return new LocalizedString("BindToWrongObjectType", "Ex5CD494", false, true, ServerStrings.ResourceManager, new object[]
			{
				objectClass,
				intendedType
			});
		}

		public static LocalizedString MigrationUserStatusSummaryStopped
		{
			get
			{
				return new LocalizedString("MigrationUserStatusSummaryStopped", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idMailboxInfoStaleException(string mailboxId)
		{
			return new LocalizedString("idMailboxInfoStaleException", "Ex6160E3", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ExCannotSaveInvalidObject(object firstError)
		{
			return new LocalizedString("ExCannotSaveInvalidObject", "Ex31080A", false, true, ServerStrings.ResourceManager, new object[]
			{
				firstError
			});
		}

		public static LocalizedString InboxRuleSensitivityCompanyConfidential
		{
			get
			{
				return new LocalizedString("InboxRuleSensitivityCompanyConfidential", "Ex24A804", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmServerTransientException(string errorMessage)
		{
			return new LocalizedString("AmServerTransientException", "Ex0BB11D", false, true, ServerStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString FailedToAddAttachments
		{
			get
			{
				return new LocalizedString("FailedToAddAttachments", "Ex709496", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotDeliverItem
		{
			get
			{
				return new LocalizedString("MapiCannotDeliverItem", "ExEE6234", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCalendarTypeNotSupported(object calendarType)
		{
			return new LocalizedString("ExCalendarTypeNotSupported", "ExB93B8B", false, true, ServerStrings.ResourceManager, new object[]
			{
				calendarType
			});
		}

		public static LocalizedString MapiCannotGetLocalRepIds
		{
			get
			{
				return new LocalizedString("MapiCannotGetLocalRepIds", "Ex623AC5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x3C01
		{
			get
			{
				return new LocalizedString("ClientCulture_0x3C01", "ExA55642", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAcquireUseLicense(Uri url)
		{
			return new LocalizedString("FailedToAcquireUseLicense", "Ex2DF607", false, true, ServerStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString FirstDay
		{
			get
			{
				return new LocalizedString("FirstDay", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x41D
		{
			get
			{
				return new LocalizedString("ClientCulture_0x41D", "Ex3E7039", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PersonIsAlreadyLinkedWithGALContact
		{
			get
			{
				return new LocalizedString("PersonIsAlreadyLinkedWithGALContact", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmReplayServiceDownException(string serverName, string rpcErrorMessage)
		{
			return new LocalizedString("AmReplayServiceDownException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				serverName,
				rpcErrorMessage
			});
		}

		public static LocalizedString InboxRuleMessageTypeCalendaring
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeCalendaring", "Ex2E56BF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Editor
		{
			get
			{
				return new LocalizedString("Editor", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotShareOtherPersonalFolder
		{
			get
			{
				return new LocalizedString("CannotShareOtherPersonalFolder", "ExB54D40", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageClosedBodyIntroText
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageClosedBodyIntroText", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeSigned
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeSigned", "ExDFBD7D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDefaultFolderNotFound(object folder)
		{
			return new LocalizedString("ExDefaultFolderNotFound", "Ex0E920E", false, true, ServerStrings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString MigrationTypePSTImport
		{
			get
			{
				return new LocalizedString("MigrationTypePSTImport", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateRangeOneYear
		{
			get
			{
				return new LocalizedString("DateRangeOneYear", "ExE6B1F6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAuditsFolderAccessDenied
		{
			get
			{
				return new LocalizedString("ExAuditsFolderAccessDenied", "Ex36C1CC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x1409
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1409", "ExB851E5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoExternalOwaAvailableException
		{
			get
			{
				return new LocalizedString("NoExternalOwaAvailableException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegateNotSupportedFolder
		{
			get
			{
				return new LocalizedString("DelegateNotSupportedFolder", "Ex9EEEAD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListInternalToOrganizationException(string value)
		{
			return new LocalizedString("JunkEmailBlockedListInternalToOrganizationException", "ExBB3B55", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString FailedToAcquireUseLicenses(string orgId)
		{
			return new LocalizedString("FailedToAcquireUseLicenses", "ExB83176", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString AmDbNotMountedNoViableServersException(string dbName)
		{
			return new LocalizedString("AmDbNotMountedNoViableServersException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString ExNumberOfRowsToFetchInvalid(string numRows)
		{
			return new LocalizedString("ExNumberOfRowsToFetchInvalid", "Ex92A046", false, true, ServerStrings.ResourceManager, new object[]
			{
				numRows
			});
		}

		public static LocalizedString PropertyIsReadOnly(string property)
		{
			return new LocalizedString("PropertyIsReadOnly", "Ex776C3D", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString MigrationBatchDirectionLocal
		{
			get
			{
				return new LocalizedString("MigrationBatchDirectionLocal", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderDeleted
		{
			get
			{
				return new LocalizedString("ErrorFolderDeleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveMonitoringServerException(string errorMessage)
		{
			return new LocalizedString("ActiveMonitoringServerException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString AmServerDagNotFound(string serverName)
		{
			return new LocalizedString("AmServerDagNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString MigrationObjectsCountStringMailboxes(string mailboxes)
		{
			return new LocalizedString("MigrationObjectsCountStringMailboxes", "Ex6AF053", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxes
			});
		}

		public static LocalizedString AddressAndRoutingTypeMismatch(string routingType)
		{
			return new LocalizedString("AddressAndRoutingTypeMismatch", "Ex6E092B", false, true, ServerStrings.ResourceManager, new object[]
			{
				routingType
			});
		}

		public static LocalizedString BadDateTimeFormatInChangeDate
		{
			get
			{
				return new LocalizedString("BadDateTimeFormatInChangeDate", "ExB87CF2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientAddressInvalidForExternalLicensing(string address, Uri uri, string tenantId)
		{
			return new LocalizedString("RecipientAddressInvalidForExternalLicensing", "Ex951F0D", false, true, ServerStrings.ResourceManager, new object[]
			{
				address,
				uri,
				tenantId
			});
		}

		public static LocalizedString ClientCulture_0x1C0A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1C0A", "Ex02ED11", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageNoActionText
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageNoActionText", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMigrationBatchId
		{
			get
			{
				return new LocalizedString("InvalidMigrationBatchId", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchNotFoundError(string batchName)
		{
			return new LocalizedString("MigrationBatchNotFoundError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				batchName
			});
		}

		public static LocalizedString FolderRuleStageOnPromotedMessage
		{
			get
			{
				return new LocalizedString("FolderRuleStageOnPromotedMessage", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idUnableToCreateDefaultTaskGroupException
		{
			get
			{
				return new LocalizedString("idUnableToCreateDefaultTaskGroupException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderStartSyncFolderHierarchyRpcFailed
		{
			get
			{
				return new LocalizedString("PublicFolderStartSyncFolderHierarchyRpcFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStateFailed
		{
			get
			{
				return new LocalizedString("SearchStateFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x2401
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2401", "ExC9AB56", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMonthNthDayMask(object dayMask)
		{
			return new LocalizedString("ExInvalidMonthNthDayMask", "ExB66492", false, true, ServerStrings.ResourceManager, new object[]
			{
				dayMask
			});
		}

		public static LocalizedString PublicFolderPerServerThreadLimitExceeded(string server, int limit, int totalActiveServers)
		{
			return new LocalizedString("PublicFolderPerServerThreadLimitExceeded", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				server,
				limit,
				totalActiveServers
			});
		}

		public static LocalizedString MapiCannotWritePerUserInformation
		{
			get
			{
				return new LocalizedString("MapiCannotWritePerUserInformation", "Ex374E09", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchNameCharacterConstraint
		{
			get
			{
				return new LocalizedString("SearchNameCharacterConstraint", "Ex23EF52", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeCalendaringResponse
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeCalendaringResponse", "Ex1AC335", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintNotSatisfiedForNonReplicatedDatabase(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintNotSatisfiedForNonReplicatedDatabase", "Ex6588C5", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid
			});
		}

		public static LocalizedString ErrorTimeProposalInvalidWhenNotAllowedByOrganizer
		{
			get
			{
				return new LocalizedString("ErrorTimeProposalInvalidWhenNotAllowedByOrganizer", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateCertExpired
		{
			get
			{
				return new LocalizedString("RequestStateCertExpired", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToCreateLicenseGenerator(string orgId, string type)
		{
			return new LocalizedString("FailedToCreateLicenseGenerator", "Ex105715", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId,
				type
			});
		}

		public static LocalizedString OscFolderForProviderNotFound(string provider)
		{
			return new LocalizedString("OscFolderForProviderNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				provider
			});
		}

		public static LocalizedString Thursday
		{
			get
			{
				return new LocalizedString("Thursday", "Ex245CEF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotDeleteAttachment
		{
			get
			{
				return new LocalizedString("MapiCannotDeleteAttachment", "ExD20458", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExEventsNotSupportedForDelegateUser
		{
			get
			{
				return new LocalizedString("ExEventsNotSupportedForDelegateUser", "Ex03D0FB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeImportPST
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeImportPST", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ImportItemThrewGrayException(string exception)
		{
			return new LocalizedString("ImportItemThrewGrayException", "ExCE7517", false, true, ServerStrings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString MigrationStepDataMigration
		{
			get
			{
				return new LocalizedString("MigrationStepDataMigration", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListXsoTooManyException
		{
			get
			{
				return new LocalizedString("JunkEmailBlockedListXsoTooManyException", "Ex99A43E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x409
		{
			get
			{
				return new LocalizedString("ClientCulture_0x409", "ExA7BCC8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonCanonicalACL(string errorInformation)
		{
			return new LocalizedString("NonCanonicalACL", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errorInformation
			});
		}

		public static LocalizedString ErrorFolderCreationIsBlocked
		{
			get
			{
				return new LocalizedString("ErrorFolderCreationIsBlocked", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidParticipantEntryId
		{
			get
			{
				return new LocalizedString("ExInvalidParticipantEntryId", "Ex9B1835", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidSpecifierValueError
		{
			get
			{
				return new LocalizedString("ExInvalidSpecifierValueError", "ExF5B629", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusDocumentSyncFailureOnly
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusDocumentSyncFailureOnly", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStateInProgress
		{
			get
			{
				return new LocalizedString("SearchStateInProgress", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailInvalidOperationException
		{
			get
			{
				return new LocalizedString("JunkEmailInvalidOperationException", "ExB16774", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoFileTooSmall(int min)
		{
			return new LocalizedString("UserPhotoFileTooSmall", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				min
			});
		}

		public static LocalizedString InvalidWorkingPeriod(string parameter, int rangeStart, int rangeEnd)
		{
			return new LocalizedString("InvalidWorkingPeriod", "Ex593C43", false, true, ServerStrings.ResourceManager, new object[]
			{
				parameter,
				rangeStart,
				rangeEnd
			});
		}

		public static LocalizedString TeamMailboxMessageWhatIsSiteMailbox
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageWhatIsSiteMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerFrench
		{
			get
			{
				return new LocalizedString("SpellCheckerFrench", "Ex4AEFA2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExUnknownRecurrenceBlobRange
		{
			get
			{
				return new LocalizedString("ExUnknownRecurrenceBlobRange", "Ex9988EF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmFailedToDeterminePAM(string dagName)
		{
			return new LocalizedString("AmFailedToDeterminePAM", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dagName
			});
		}

		public static LocalizedString ExInvalidMAPIType(uint type)
		{
			return new LocalizedString("ExInvalidMAPIType", "Ex8A2E1E", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString MapiCannotOpenAttachmentId(object id)
		{
			return new LocalizedString("MapiCannotOpenAttachmentId", "ExF29E6F", false, true, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExUnableToOpenOrCreateDefaultFolder(string defaultFolderName)
		{
			return new LocalizedString("ExUnableToOpenOrCreateDefaultFolder", "ExAD93AB", false, true, ServerStrings.ResourceManager, new object[]
			{
				defaultFolderName
			});
		}

		public static LocalizedString TeamMailboxMessageWelcomeSubject(string tmName)
		{
			return new LocalizedString("TeamMailboxMessageWelcomeSubject", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				tmName
			});
		}

		public static LocalizedString ExAttachmentCannotOpenDueToUnSave
		{
			get
			{
				return new LocalizedString("ExAttachmentCannotOpenDueToUnSave", "ExBD810D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x439
		{
			get
			{
				return new LocalizedString("ClientCulture_0x439", "Ex77B11F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionCorruptTnef(int complianceStatus)
		{
			return new LocalizedString("ConversionCorruptTnef", "Ex1A97B8", false, true, ServerStrings.ResourceManager, new object[]
			{
				complianceStatus
			});
		}

		public static LocalizedString SpellCheckerEnglishCanada
		{
			get
			{
				return new LocalizedString("SpellCheckerEnglishCanada", "ExC86394", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotUpdateDeferredActionMessages
		{
			get
			{
				return new LocalizedString("MapiCannotUpdateDeferredActionMessages", "Ex4B27D9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStatisticsCompleteStatus
		{
			get
			{
				return new LocalizedString("MigrationStatisticsCompleteStatus", "Ex35A837", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OleConversionInvalidResultType
		{
			get
			{
				return new LocalizedString("OleConversionInvalidResultType", "ExEC0E54", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToMakeAutoDiscoveryRequest
		{
			get
			{
				return new LocalizedString("UnableToMakeAutoDiscoveryRequest", "ExF7FF53", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailSubjectImportPst
		{
			get
			{
				return new LocalizedString("NotificationEmailSubjectImportPst", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingMessageAttachmentNotFoundException
		{
			get
			{
				return new LocalizedString("SharingMessageAttachmentNotFoundException", "Ex2B46C8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNullSortOrderParameter(int index)
		{
			return new LocalizedString("ExNullSortOrderParameter", "Ex2308AA", false, true, ServerStrings.ResourceManager, new object[]
			{
				index
			});
		}

		public static LocalizedString MigrationBatchStatusIncrementalSyncing
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusIncrementalSyncing", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStateStopped
		{
			get
			{
				return new LocalizedString("SearchStateStopped", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderMailboxNotFound
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxNotFound", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetReceiveFolder
		{
			get
			{
				return new LocalizedString("MapiCannotGetReceiveFolder", "ExA9569C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusStarting
		{
			get
			{
				return new LocalizedString("MigrationUserStatusStarting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserSidNotFound(string userLegDn)
		{
			return new LocalizedString("UserSidNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				userLegDn
			});
		}

		public static LocalizedString PublicFolderSyncFolderHierarchyFailed(string innerMessage)
		{
			return new LocalizedString("PublicFolderSyncFolderHierarchyFailed", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				innerMessage
			});
		}

		public static LocalizedString AmDbMountNotAllowedDueToLossException
		{
			get
			{
				return new LocalizedString("AmDbMountNotAllowedDueToLossException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerGermanPreReform
		{
			get
			{
				return new LocalizedString("SpellCheckerGermanPreReform", "ExF478AF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidKindFormat
		{
			get
			{
				return new LocalizedString("InvalidKindFormat", "Ex93B836", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OleUnableToReadAttachment
		{
			get
			{
				return new LocalizedString("OleUnableToReadAttachment", "Ex60F765", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidModifier
		{
			get
			{
				return new LocalizedString("InvalidModifier", "Ex61E271", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusProvisionUpdating
		{
			get
			{
				return new LocalizedString("MigrationUserStatusProvisionUpdating", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationMailboxPermissionFullAccess
		{
			get
			{
				return new LocalizedString("MigrationMailboxPermissionFullAccess", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxNotFoundByAdObjectId(string adObjectId)
		{
			return new LocalizedString("MailboxNotFoundByAdObjectId", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				adObjectId
			});
		}

		public static LocalizedString idResourceQuarantinedDueToBlackHole(Guid resourceIdentity)
		{
			return new LocalizedString("idResourceQuarantinedDueToBlackHole", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				resourceIdentity
			});
		}

		public static LocalizedString WeatherUnitCelsius
		{
			get
			{
				return new LocalizedString("WeatherUnitCelsius", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullDateInChangeDate
		{
			get
			{
				return new LocalizedString("NullDateInChangeDate", "ExF7EDF1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x2C09
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2C09", "ExE1BF2F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchDirectionOffboarding
		{
			get
			{
				return new LocalizedString("MigrationBatchDirectionOffboarding", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidParameter(string argumentName, int argumentNumber)
		{
			return new LocalizedString("ExInvalidParameter", "Ex234101", false, true, ServerStrings.ResourceManager, new object[]
			{
				argumentName,
				argumentNumber
			});
		}

		public static LocalizedString CalNotifTypeUninteresting
		{
			get
			{
				return new LocalizedString("CalNotifTypeUninteresting", "Ex8096C6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDatabaseNeverMountedException
		{
			get
			{
				return new LocalizedString("AmDatabaseNeverMountedException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSetReceiveFolder
		{
			get
			{
				return new LocalizedString("MapiCannotSetReceiveFolder", "Ex199343", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcClientWrapperFailedToLoadTopology
		{
			get
			{
				return new LocalizedString("RpcClientWrapperFailedToLoadTopology", "ExF24F3F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCorruptConversationActionItem
		{
			get
			{
				return new LocalizedString("ExCorruptConversationActionItem", "Ex663F93", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToUnprotectAttachment(string filename)
		{
			return new LocalizedString("FailedToUnprotectAttachment", "Ex59A6ED", false, true, ServerStrings.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString SearchMandatoryParameter(string p1)
		{
			return new LocalizedString("SearchMandatoryParameter", "Ex1C995B", false, true, ServerStrings.ResourceManager, new object[]
			{
				p1
			});
		}

		public static LocalizedString InvalidDuration(int duration)
		{
			return new LocalizedString("InvalidDuration", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				duration
			});
		}

		public static LocalizedString Tuesday
		{
			get
			{
				return new LocalizedString("Tuesday", "Ex180ABD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCreateRestriction
		{
			get
			{
				return new LocalizedString("MapiCannotCreateRestriction", "Ex12B44B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptJunkMoveStamp
		{
			get
			{
				return new LocalizedString("CorruptJunkMoveStamp", "Ex2FEEB0", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttachmentNumber
		{
			get
			{
				return new LocalizedString("InvalidAttachmentNumber", "Ex2371CF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x83E
		{
			get
			{
				return new LocalizedString("ClientCulture_0x83E", "ExD610E5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailSubjectFailed(string notificationType)
		{
			return new LocalizedString("NotificationEmailSubjectFailed", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				notificationType
			});
		}

		public static LocalizedString Friday
		{
			get
			{
				return new LocalizedString("Friday", "Ex5D0CA6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoServerValueAvailable
		{
			get
			{
				return new LocalizedString("NoServerValueAvailable", "ExA26E67", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegateInvalidPermission
		{
			get
			{
				return new LocalizedString("DelegateInvalidPermission", "ExFC228F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationAborted
		{
			get
			{
				return new LocalizedString("OperationAborted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscoveryMailboxNotFound
		{
			get
			{
				return new LocalizedString("DiscoveryMailboxNotFound", "ExAB49AD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x422
		{
			get
			{
				return new LocalizedString("ClientCulture_0x422", "Ex496440", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusSummarySynced
		{
			get
			{
				return new LocalizedString("MigrationUserStatusSummarySynced", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FourHours
		{
			get
			{
				return new LocalizedString("FourHours", "Ex21670B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusCompleted
		{
			get
			{
				return new LocalizedString("MigrationUserStatusCompleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExVotingBlobCorrupt
		{
			get
			{
				return new LocalizedString("ExVotingBlobCorrupt", "ExBEF25D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalLicensingDisabledForTenant(OrganizationId orgId)
		{
			return new LocalizedString("InternalLicensingDisabledForTenant", "ExA60B3F", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString HexadecimalHtmlColorPatternDescription
		{
			get
			{
				return new LocalizedString("HexadecimalHtmlColorPatternDescription", "Ex7F3109", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExBatchBuilderADLookupFailed(ProxyAddress proxyAddress, object error)
		{
			return new LocalizedString("ExBatchBuilderADLookupFailed", "Ex65A385", false, true, ServerStrings.ResourceManager, new object[]
			{
				proxyAddress,
				error
			});
		}

		public static LocalizedString MigrationStepInitialization
		{
			get
			{
				return new LocalizedString("MigrationStepInitialization", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusMembershipSyncFailureOnly
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusMembershipSyncFailureOnly", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidEncryptedSharedFolderDataException
		{
			get
			{
				return new LocalizedString("InvalidEncryptedSharedFolderDataException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveMonitoringServiceDown(string serverName, string rpcErrorMessage)
		{
			return new LocalizedString("ActiveMonitoringServiceDown", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				serverName,
				rpcErrorMessage
			});
		}

		public static LocalizedString ExCorruptRestrictionFilter
		{
			get
			{
				return new LocalizedString("ExCorruptRestrictionFilter", "ExA40FB8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorStatisticsStartIndexIsOutOfBound(int startIndex, int totalKeywords)
		{
			return new LocalizedString("ErrorStatisticsStartIndexIsOutOfBound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				startIndex,
				totalKeywords
			});
		}

		public static LocalizedString ExServerNotFound(string serverName)
		{
			return new LocalizedString("ExServerNotFound", "Ex2775B7", false, true, ServerStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ErrorNotificationAlreadyExists
		{
			get
			{
				return new LocalizedString("ErrorNotificationAlreadyExists", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CanUseApiOnlyWhenTimeZoneIsNull(string api)
		{
			return new LocalizedString("CanUseApiOnlyWhenTimeZoneIsNull", "ExC62EB7", false, true, ServerStrings.ResourceManager, new object[]
			{
				api
			});
		}

		public static LocalizedString ExItemIsOpenedInReadOnlyMode
		{
			get
			{
				return new LocalizedString("ExItemIsOpenedInReadOnlyMode", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnbalancedParenthesis
		{
			get
			{
				return new LocalizedString("UnbalancedParenthesis", "Ex4F2B12", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExUnsupportedMapiType(Type type)
		{
			return new LocalizedString("ExUnsupportedMapiType", "ExD122BE", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString DataMoveReplicationConstraintNotSatisfiedNoHealthyCopies(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintNotSatisfiedNoHealthyCopies", "Ex998A23", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid
			});
		}

		public static LocalizedString InvalidRpMsgFormat
		{
			get
			{
				return new LocalizedString("InvalidRpMsgFormat", "ExB640A2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagNetworkManagementError(string err)
		{
			return new LocalizedString("DagNetworkManagementError", "ExE6256F", false, true, ServerStrings.ResourceManager, new object[]
			{
				err
			});
		}

		public static LocalizedString UserPhotoNotAnImage
		{
			get
			{
				return new LocalizedString("UserPhotoNotAnImage", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCreateEntryIdFromShortTermId
		{
			get
			{
				return new LocalizedString("MapiCannotCreateEntryIdFromShortTermId", "Ex1B40B7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x807
		{
			get
			{
				return new LocalizedString("ClientCulture_0x807", "ExFB27B1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveMonitoringServerTransientException(string errorMessage)
		{
			return new LocalizedString("ActiveMonitoringServerTransientException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString MapiCannotCreateBookmark
		{
			get
			{
				return new LocalizedString("MapiCannotCreateBookmark", "ExFF98D4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidateDateRange
		{
			get
			{
				return new LocalizedString("InvalidateDateRange", "Ex0F12D9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserAdminTypeUnknown
		{
			get
			{
				return new LocalizedString("MigrationUserAdminTypeUnknown", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotMoveOrCopyBetweenPrivateAndPublicMailbox
		{
			get
			{
				return new LocalizedString("CannotMoveOrCopyBetweenPrivateAndPublicMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageClosedSubject(string tmName)
		{
			return new LocalizedString("TeamMailboxMessageClosedSubject", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				tmName
			});
		}

		public static LocalizedString ErrorSavingSearchObject(string id)
		{
			return new LocalizedString("ErrorSavingSearchObject", "ExCBA4AC", false, true, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString SpellCheckerNorwegianBokmal
		{
			get
			{
				return new LocalizedString("SpellCheckerNorwegianBokmal", "ExF3957A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderSaveOperationResult(LocalizedString operationResult, int errorCount, string propertyErrors)
		{
			return new LocalizedString("FolderSaveOperationResult", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				operationResult,
				errorCount,
				propertyErrors
			});
		}

		public static LocalizedString ClientCulture_0x1007
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1007", "Ex562DC5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchSupportedActionStop
		{
			get
			{
				return new LocalizedString("MigrationBatchSupportedActionStop", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CrossForestNotSupported
		{
			get
			{
				return new LocalizedString("CrossForestNotSupported", "ExCA5E73", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotAccessSystemFolderId
		{
			get
			{
				return new LocalizedString("ExCannotAccessSystemFolderId", "Ex29F424", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x410
		{
			get
			{
				return new LocalizedString("ClientCulture_0x410", "Ex0E6E86", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToLocateTPDConfig(string orgId)
		{
			return new LocalizedString("FailedToLocateTPDConfig", "Ex054C4E", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString ExStatefulFilterMustBeSetWhenSetSyncFiltersIsInvokedWithNullFilter
		{
			get
			{
				return new LocalizedString("ExStatefulFilterMustBeSetWhenSetSyncFiltersIsInvokedWithNullFilter", "Ex678A08", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotReadPermissions
		{
			get
			{
				return new LocalizedString("MapiCannotReadPermissions", "Ex043A0E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderRuleStageOnPublicFolderBefore
		{
			get
			{
				return new LocalizedString("FolderRuleStageOnPublicFolderBefore", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnbalancedQuote
		{
			get
			{
				return new LocalizedString("UnbalancedQuote", "Ex107021", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToWriteActivityLog
		{
			get
			{
				return new LocalizedString("FailedToWriteActivityLog", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoFreeBusyFolder
		{
			get
			{
				return new LocalizedString("NoFreeBusyFolder", "Ex44578F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x408
		{
			get
			{
				return new LocalizedString("ClientCulture_0x408", "Ex6C6C30", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidExceptionListWithDoubleEntry(object date)
		{
			return new LocalizedString("ExInvalidExceptionListWithDoubleEntry", "ExC81AA6", false, true, ServerStrings.ResourceManager, new object[]
			{
				date
			});
		}

		public static LocalizedString SharingConflictException
		{
			get
			{
				return new LocalizedString("SharingConflictException", "ExF63A28", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotQueryColumns
		{
			get
			{
				return new LocalizedString("MapiCannotQueryColumns", "ExFBC1B3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCaughtMapiExceptionWhileReadingEvents
		{
			get
			{
				return new LocalizedString("ExCaughtMapiExceptionWhileReadingEvents", "Ex2F746B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x81D
		{
			get
			{
				return new LocalizedString("ClientCulture_0x81D", "ExB8F2B4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotCreateFolder(object saveResult)
		{
			return new LocalizedString("ExCannotCreateFolder", "Ex13C206", false, true, ServerStrings.ResourceManager, new object[]
			{
				saveResult
			});
		}

		public static LocalizedString ConversionInvalidSmimeContent
		{
			get
			{
				return new LocalizedString("ConversionInvalidSmimeContent", "Ex4BD2C4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotRevertSentMeetingToAppointment
		{
			get
			{
				return new LocalizedString("ExCannotRevertSentMeetingToAppointment", "Ex5B5AC4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidDateTimeRange(object startTime, object endTime)
		{
			return new LocalizedString("ExInvalidDateTimeRange", "ExFD3ECD", false, true, ServerStrings.ResourceManager, new object[]
			{
				startTime,
				endTime
			});
		}

		public static LocalizedString AmServerNotFoundException(string server)
		{
			return new LocalizedString("AmServerNotFoundException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString UserHasNoEventPermisson(string folderId)
		{
			return new LocalizedString("UserHasNoEventPermisson", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				folderId
			});
		}

		public static LocalizedString RpcServerRequestAlreadyPending(string mailboxGuid, string pendingClient, string pendingSince)
		{
			return new LocalizedString("RpcServerRequestAlreadyPending", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				pendingClient,
				pendingSince
			});
		}

		public static LocalizedString ExUnsupportedABProvider(string provider, string version)
		{
			return new LocalizedString("ExUnsupportedABProvider", "ExC220F9", false, true, ServerStrings.ResourceManager, new object[]
			{
				provider,
				version
			});
		}

		public static LocalizedString ExMustSaveFolderToMakeVisibleToOutlook
		{
			get
			{
				return new LocalizedString("ExMustSaveFolderToMakeVisibleToOutlook", "ExB803F3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedContentRestriction
		{
			get
			{
				return new LocalizedString("UnsupportedContentRestriction", "ExAC8684", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationItemStatisticsString(string emailAddress, long itemsSynced, long itemsSkipped, string startTime)
		{
			return new LocalizedString("MigrationItemStatisticsString", "Ex5D8F20", false, true, ServerStrings.ResourceManager, new object[]
			{
				emailAddress,
				itemsSynced,
				itemsSkipped,
				startTime
			});
		}

		public static LocalizedString ClientCulture_0x42D
		{
			get
			{
				return new LocalizedString("ClientCulture_0x42D", "Ex5BB852", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValidationFailureAfterPromotion(string uid)
		{
			return new LocalizedString("ValidationFailureAfterPromotion", "Ex2465C2", false, true, ServerStrings.ResourceManager, new object[]
			{
				uid
			});
		}

		public static LocalizedString ExCurrentServerNotInSite(string serverName)
		{
			return new LocalizedString("ExCurrentServerNotInSite", "Ex6F891D", false, true, ServerStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString NotificationEmailBodyImportPSTCreated
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyImportPSTCreated", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchTargetInSource
		{
			get
			{
				return new LocalizedString("SearchTargetInSource", "Ex615213", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAddItemAttachmentFailed
		{
			get
			{
				return new LocalizedString("ExAddItemAttachmentFailed", "ExE2BCCE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotResolvePropertyException(string propertySchema)
		{
			return new LocalizedString("CannotResolvePropertyException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				propertySchema
			});
		}

		public static LocalizedString FailedToVerifyDRMPropsSignatureADError(string sid)
		{
			return new LocalizedString("FailedToVerifyDRMPropsSignatureADError", "ExEF0CC5", false, true, ServerStrings.ResourceManager, new object[]
			{
				sid
			});
		}

		public static LocalizedString RpcServerIgnorePendingDeleteTeamMailbox(string mailboxGuid)
		{
			return new LocalizedString("RpcServerIgnorePendingDeleteTeamMailbox", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString ClientCulture_0x403
		{
			get
			{
				return new LocalizedString("ClientCulture_0x403", "ExF12F46", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotMoveOrDeleteDefaultFolders
		{
			get
			{
				return new LocalizedString("ExCannotMoveOrDeleteDefaultFolders", "Ex5E676E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationObjectsCountStringGroups(string groups)
		{
			return new LocalizedString("MigrationObjectsCountStringGroups", "ExCE86CB", false, true, ServerStrings.ResourceManager, new object[]
			{
				groups
			});
		}

		public static LocalizedString ExCannotSeekRow
		{
			get
			{
				return new LocalizedString("ExCannotSeekRow", "Ex143A07", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMonthlyDayOfMonth(int dayOfMonth)
		{
			return new LocalizedString("ExInvalidMonthlyDayOfMonth", "ExA6D522", false, true, ServerStrings.ResourceManager, new object[]
			{
				dayOfMonth
			});
		}

		public static LocalizedString ErrorCalendarReminderNotMinutes(string value)
		{
			return new LocalizedString("ErrorCalendarReminderNotMinutes", "Ex6D6701", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MigrationReportBatch
		{
			get
			{
				return new LocalizedString("MigrationReportBatch", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExErrorInDetectE15Store
		{
			get
			{
				return new LocalizedString("ExErrorInDetectE15Store", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idDefaultFoldersNotLocalizedException
		{
			get
			{
				return new LocalizedString("idDefaultFoldersNotLocalizedException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStateCompleted
		{
			get
			{
				return new LocalizedString("MigrationStateCompleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCorrelationFailedForOccurrence(string subject)
		{
			return new LocalizedString("ExCorrelationFailedForOccurrence", "Ex36138C", false, true, ServerStrings.ResourceManager, new object[]
			{
				subject
			});
		}

		public static LocalizedString ErrorMissingMailboxOrPermission
		{
			get
			{
				return new LocalizedString("ErrorMissingMailboxOrPermission", "ExBAA10A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeProposalInvalidDuration(int days)
		{
			return new LocalizedString("ErrorTimeProposalInvalidDuration", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				days
			});
		}

		public static LocalizedString MigrationFolderNotFound(string mailboxName)
		{
			return new LocalizedString("MigrationFolderNotFound", "Ex9B7920", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString ClientCulture_0x140C
		{
			get
			{
				return new LocalizedString("ClientCulture_0x140C", "ExA20802", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTypeIMAP
		{
			get
			{
				return new LocalizedString("MigrationTypeIMAP", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x2809
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2809", "ExF70A3B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x1404
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1404", "ExF8CE5F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotRejectDeletes
		{
			get
			{
				return new LocalizedString("ExCannotRejectDeletes", "ExB96859", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusMaintenanceSyncFailureOnly
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusMaintenanceSyncFailureOnly", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStateStopped
		{
			get
			{
				return new LocalizedString("MigrationStateStopped", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStageDiscovery
		{
			get
			{
				return new LocalizedString("MigrationStageDiscovery", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationMailboxDatabaseInfoNotAvailable(string mbxid)
		{
			return new LocalizedString("MigrationMailboxDatabaseInfoNotAvailable", "Ex7F3CA2", false, true, ServerStrings.ResourceManager, new object[]
			{
				mbxid
			});
		}

		public static LocalizedString ClientCulture_0x40A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x40A", "Ex901F0E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailBodyCertExpired
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyCertExpired", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleSystemMigrationMailboxes(string mailboxName)
		{
			return new LocalizedString("MultipleSystemMigrationMailboxes", "ExFD4BE3", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString ExCannotRejectSameOperationTwice
		{
			get
			{
				return new LocalizedString("ExCannotRejectSameOperationTwice", "Ex272E05", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotGetSearchCriteria
		{
			get
			{
				return new LocalizedString("ExCannotGetSearchCriteria", "Ex7842CB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMaxQueueSize
		{
			get
			{
				return new LocalizedString("ExInvalidMaxQueueSize", "ExDC03B5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADException
		{
			get
			{
				return new LocalizedString("ADException", "Ex4589A3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNoMailboxOwner
		{
			get
			{
				return new LocalizedString("ExNoMailboxOwner", "Ex007029", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNotConnected
		{
			get
			{
				return new LocalizedString("ExNotConnected", "ExC4FA49", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStateStopping
		{
			get
			{
				return new LocalizedString("SearchStateStopping", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerKorean
		{
			get
			{
				return new LocalizedString("SpellCheckerKorean", "ExF7EA11", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidNamedProperty(string property)
		{
			return new LocalizedString("ExInvalidNamedProperty", "Ex48473F", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString DagNetworkRenameDupName(string oldName, string newName)
		{
			return new LocalizedString("DagNetworkRenameDupName", "ExCE24CA", false, true, ServerStrings.ResourceManager, new object[]
			{
				oldName,
				newName
			});
		}

		public static LocalizedString ErrorInvalidQueryLanguage(string name, string language)
		{
			return new LocalizedString("ErrorInvalidQueryLanguage", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name,
				language
			});
		}

		public static LocalizedString ExValueOfWrongType(object value, Type type)
		{
			return new LocalizedString("ExValueOfWrongType", "Ex5CBBF4", false, true, ServerStrings.ResourceManager, new object[]
			{
				value,
				type
			});
		}

		public static LocalizedString InvalidReminderTime(string reminderTime, string referenceTime)
		{
			return new LocalizedString("InvalidReminderTime", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				reminderTime,
				referenceTime
			});
		}

		public static LocalizedString MigrationTypeExchangeLocalMove
		{
			get
			{
				return new LocalizedString("MigrationTypeExchangeLocalMove", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExternalDirectoryObjectIdError(string externalDirectoryObjectId)
		{
			return new LocalizedString("InvalidExternalDirectoryObjectIdError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				externalDirectoryObjectId
			});
		}

		public static LocalizedString MapiCannotSubmitMessage
		{
			get
			{
				return new LocalizedString("MapiCannotSubmitMessage", "ExD4E03D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x1C09
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1C09", "Ex398E95", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idUnableToCreateDefaultCalendarGroupException(string calendarType)
		{
			return new LocalizedString("idUnableToCreateDefaultCalendarGroupException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				calendarType
			});
		}

		public static LocalizedString ExInvalidWeeklyDayMask(object dayMask)
		{
			return new LocalizedString("ExInvalidWeeklyDayMask", "Ex26FC14", false, true, ServerStrings.ResourceManager, new object[]
			{
				dayMask
			});
		}

		public static LocalizedString ExInvalidOrder
		{
			get
			{
				return new LocalizedString("ExInvalidOrder", "Ex8F7E7A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncorrectEntriesInServiceLocationResponse(int numResponses, int expected)
		{
			return new LocalizedString("IncorrectEntriesInServiceLocationResponse", "ExFCEB21", false, true, ServerStrings.ResourceManager, new object[]
			{
				numResponses,
				expected
			});
		}

		public static LocalizedString InconsistentCalendarMethod(string method, string id)
		{
			return new LocalizedString("InconsistentCalendarMethod", "Ex408F1F", false, true, ServerStrings.ResourceManager, new object[]
			{
				method,
				id
			});
		}

		public static LocalizedString NoProviderSupportShareFolder
		{
			get
			{
				return new LocalizedString("NoProviderSupportShareFolder", "Ex0497E6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConnectionCacheSizeNotSet
		{
			get
			{
				return new LocalizedString("ExConnectionCacheSizeNotSet", "Ex554DB9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFlagsRemove
		{
			get
			{
				return new LocalizedString("MigrationFlagsRemove", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCreateSynchronizerEx(Type xsoManifestType)
		{
			return new LocalizedString("CannotCreateSynchronizerEx", "ExE5BAE6", false, true, ServerStrings.ResourceManager, new object[]
			{
				xsoManifestType
			});
		}

		public static LocalizedString ExInvalidRecipient
		{
			get
			{
				return new LocalizedString("ExInvalidRecipient", "ExE8E5B3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFoundInvalidRowType
		{
			get
			{
				return new LocalizedString("ExFoundInvalidRowType", "Ex55BFE3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidOffset
		{
			get
			{
				return new LocalizedString("ExInvalidOffset", "ExB3B3DA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotEnoughPermissionsToPerformOperation
		{
			get
			{
				return new LocalizedString("NotEnoughPermissionsToPerformOperation", "Ex1B6032", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidExceptionCount(int exceptionListCount, int exceptionInfoCount)
		{
			return new LocalizedString("ExInvalidExceptionCount", "Ex5994AD", false, true, ServerStrings.ResourceManager, new object[]
			{
				exceptionListCount,
				exceptionInfoCount
			});
		}

		public static LocalizedString RemoteAccountPolicyNotFound(string policy)
		{
			return new LocalizedString("RemoteAccountPolicyNotFound", "Ex226A6A", false, true, ServerStrings.ResourceManager, new object[]
			{
				policy
			});
		}

		public static LocalizedString MigrationStatisticsPartiallyCompleteStatus
		{
			get
			{
				return new LocalizedString("MigrationStatisticsPartiallyCompleteStatus", "Ex7F1C00", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusNotAvailable
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusNotAvailable", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOperator
		{
			get
			{
				return new LocalizedString("InvalidOperator", "Ex138281", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultHtmlAttachmentHrefText
		{
			get
			{
				return new LocalizedString("DefaultHtmlAttachmentHrefText", "Ex2D9CFF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncorrectServerError(SmtpAddress targetMailboxSmtpAddress, string expectedServerFqdn)
		{
			return new LocalizedString("IncorrectServerError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				targetMailboxSmtpAddress,
				expectedServerFqdn
			});
		}

		public static LocalizedString ExInvalidYearlyRecurrencePeriod(int period)
		{
			return new LocalizedString("ExInvalidYearlyRecurrencePeriod", "Ex45CE8D", false, true, ServerStrings.ResourceManager, new object[]
			{
				period
			});
		}

		public static LocalizedString ConversionBodyCorrupt
		{
			get
			{
				return new LocalizedString("ConversionBodyCorrupt", "ExE250BB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcServerDirectoryError(string mailboxGuid, string error)
		{
			return new LocalizedString("RpcServerDirectoryError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				error
			});
		}

		public static LocalizedString ClientCulture_0x1407
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1407", "ExE4EC57", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadUserConfig(string user)
		{
			return new LocalizedString("FailedToReadUserConfig", "ExBB9B9B", false, true, ServerStrings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString InvalidDueDate1(string dueDate, string referenceTime)
		{
			return new LocalizedString("InvalidDueDate1", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dueDate,
				referenceTime
			});
		}

		public static LocalizedString ExUnsupportedCodepage(int codepage)
		{
			return new LocalizedString("ExUnsupportedCodepage", "Ex3D3B19", false, true, ServerStrings.ResourceManager, new object[]
			{
				codepage
			});
		}

		public static LocalizedString MapiCannotSaveChanges
		{
			get
			{
				return new LocalizedString("MapiCannotSaveChanges", "ExF1AEC3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RejectedSuggestionPersonIdSameAsPersonId
		{
			get
			{
				return new LocalizedString("RejectedSuggestionPersonIdSameAsPersonId", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidPhoneNumberFormat
		{
			get
			{
				return new LocalizedString("ErrorInvalidPhoneNumberFormat", "Ex4ACC2A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStateCorrupted
		{
			get
			{
				return new LocalizedString("MigrationStateCorrupted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningRequestCsvContainsNeitherPasswordNorFederatedIdentity
		{
			get
			{
				return new LocalizedString("ProvisioningRequestCsvContainsNeitherPasswordNorFederatedIdentity", "Ex2732BB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SecurityPrincipalAlreadyDefined
		{
			get
			{
				return new LocalizedString("SecurityPrincipalAlreadyDefined", "Ex94DA23", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmailFormatNotSupported(object addressType, string routingType)
		{
			return new LocalizedString("EmailFormatNotSupported", "Ex04CF93", false, true, ServerStrings.ResourceManager, new object[]
			{
				addressType,
				routingType
			});
		}

		public static LocalizedString ExInvalidTypeGroupCombination(object type, object group)
		{
			return new LocalizedString("ExInvalidTypeGroupCombination", "ExFDC860", false, true, ServerStrings.ResourceManager, new object[]
			{
				type,
				group
			});
		}

		public static LocalizedString KqlParseException
		{
			get
			{
				return new LocalizedString("KqlParseException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchErrorString(int rowIndex, string emailAddress, string localizedMessage)
		{
			return new LocalizedString("MigrationBatchErrorString", "Ex817ADC", false, true, ServerStrings.ResourceManager, new object[]
			{
				rowIndex,
				emailAddress,
				localizedMessage
			});
		}

		public static LocalizedString ExEventNotFound
		{
			get
			{
				return new LocalizedString("ExEventNotFound", "ExED913E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ThreeDays
		{
			get
			{
				return new LocalizedString("ThreeDays", "Ex4D4EB8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClosingTagExpected(string found)
		{
			return new LocalizedString("ClosingTagExpected", "Ex494BA1", false, true, ServerStrings.ResourceManager, new object[]
			{
				found
			});
		}

		public static LocalizedString AmServiceDownException(string serverName)
		{
			return new LocalizedString("AmServiceDownException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString ExInvalidSortLength
		{
			get
			{
				return new LocalizedString("ExInvalidSortLength", "Ex0FA70B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidAttendeeType(object attendeeType)
		{
			return new LocalizedString("ExInvalidAttendeeType", "ExF16A21", false, true, ServerStrings.ResourceManager, new object[]
			{
				attendeeType
			});
		}

		public static LocalizedString AmDatabaseMasterIsInvalid(string dbName)
		{
			return new LocalizedString("AmDatabaseMasterIsInvalid", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dbName
			});
		}

		public static LocalizedString MapiCannotGetPerUserLongTermIds
		{
			get
			{
				return new LocalizedString("MapiCannotGetPerUserLongTermIds", "ExBBF4BC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFolderWithoutMapiProp
		{
			get
			{
				return new LocalizedString("ExFolderWithoutMapiProp", "Ex618E8F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExChangeKeyTooLong
		{
			get
			{
				return new LocalizedString("ExChangeKeyTooLong", "Ex0E36B5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationGroupMembersAttachmentCorrupted(string groupSmtpAddress)
		{
			return new LocalizedString("MigrationGroupMembersAttachmentCorrupted", "Ex9A0446", false, true, ServerStrings.ResourceManager, new object[]
			{
				groupSmtpAddress
			});
		}

		public static LocalizedString ExUnknownRestrictionType
		{
			get
			{
				return new LocalizedString("ExUnknownRestrictionType", "Ex024C18", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidRowCount
		{
			get
			{
				return new LocalizedString("ExInvalidRowCount", "ExD3C2FA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedExistRestriction
		{
			get
			{
				return new LocalizedString("UnsupportedExistRestriction", "Ex5AAE45", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UseMethodInstead(string method)
		{
			return new LocalizedString("UseMethodInstead", "Ex6527F6", false, true, ServerStrings.ResourceManager, new object[]
			{
				method
			});
		}

		public static LocalizedString AvailabilityOnly
		{
			get
			{
				return new LocalizedString("AvailabilityOnly", "Ex6FD924", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotExecuteWithInternalAccess
		{
			get
			{
				return new LocalizedString("MapiCannotExecuteWithInternalAccess", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidReminderOffset(int offset, int min, int max)
		{
			return new LocalizedString("InvalidReminderOffset", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				offset,
				min,
				max
			});
		}

		public static LocalizedString ExSearchFolderNoAssociatedItem(object Guid)
		{
			return new LocalizedString("ExSearchFolderNoAssociatedItem", "ExA72C7E", false, true, ServerStrings.ResourceManager, new object[]
			{
				Guid
			});
		}

		public static LocalizedString ExItemNoParentId
		{
			get
			{
				return new LocalizedString("ExItemNoParentId", "Ex6217D1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTypePublicFolder
		{
			get
			{
				return new LocalizedString("MigrationTypePublicFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetPerUserGuid
		{
			get
			{
				return new LocalizedString("MapiCannotGetPerUserGuid", "Ex4E15E8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseLocationNotAvailable(Guid databaseGuid)
		{
			return new LocalizedString("DatabaseLocationNotAvailable", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				databaseGuid
			});
		}

		public static LocalizedString FederationNotEnabled
		{
			get
			{
				return new LocalizedString("FederationNotEnabled", "Ex8154EB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateWaitingForFinalization
		{
			get
			{
				return new LocalizedString("RequestStateWaitingForFinalization", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobConnectionSettingsInvalid(string fieldName, string value)
		{
			return new LocalizedString("MigrationJobConnectionSettingsInvalid", "ExA76022", false, true, ServerStrings.ResourceManager, new object[]
			{
				fieldName,
				value
			});
		}

		public static LocalizedString RequestStateCompleted
		{
			get
			{
				return new LocalizedString("RequestStateCompleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxSearchObjectNotExist(string id)
		{
			return new LocalizedString("MailboxSearchObjectNotExist", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString TooManyCultures
		{
			get
			{
				return new LocalizedString("TooManyCultures", "Ex874281", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSetCollapseState
		{
			get
			{
				return new LocalizedString("MapiCannotSetCollapseState", "Ex3FD53C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IncompleteUserInformationToAccessGroupMailbox
		{
			get
			{
				return new LocalizedString("IncompleteUserInformationToAccessGroupMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x2001
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2001", "ExDA8A26", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidValueTypeForCalculatedProperty(object value, Type property)
		{
			return new LocalizedString("ExInvalidValueTypeForCalculatedProperty", "Ex77AB27", false, true, ServerStrings.ResourceManager, new object[]
			{
				value,
				property
			});
		}

		public static LocalizedString CannotImportMessageChange
		{
			get
			{
				return new LocalizedString("CannotImportMessageChange", "Ex5634B0", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTimesInTimeSlot
		{
			get
			{
				return new LocalizedString("InvalidTimesInTimeSlot", "Ex7EB919", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportFinalizationFailure
		{
			get
			{
				return new LocalizedString("MigrationReportFinalizationFailure", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StructuredQueryException
		{
			get
			{
				return new LocalizedString("StructuredQueryException", "Ex7CCE6A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrincipalNotAllowedByPolicy(string principal)
		{
			return new LocalizedString("PrincipalNotAllowedByPolicy", "Ex52DBF9", false, true, ServerStrings.ResourceManager, new object[]
			{
				principal
			});
		}

		public static LocalizedString ExUnknownResponseType
		{
			get
			{
				return new LocalizedString("ExUnknownResponseType", "ExE9BF33", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateCreated
		{
			get
			{
				return new LocalizedString("RequestStateCreated", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCorruptDataRecoverError(string folder)
		{
			return new LocalizedString("ExCorruptDataRecoverError", "ExEA04E5", false, true, ServerStrings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString ExInvalidComparisonOperatorInComparisonFilter
		{
			get
			{
				return new LocalizedString("ExInvalidComparisonOperatorInComparisonFilter", "ExCDE221", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFolderSettings
		{
			get
			{
				return new LocalizedString("MigrationFolderSettings", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x809
		{
			get
			{
				return new LocalizedString("ClientCulture_0x809", "Ex36E366", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveMonitoringUnknownGenericRpcCommand(int requestedServerVersion, int replyingServerVersion, int commandId)
		{
			return new LocalizedString("ActiveMonitoringUnknownGenericRpcCommand", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				requestedServerVersion,
				replyingServerVersion,
				commandId
			});
		}

		public static LocalizedString ExUnsupportedSeekReference
		{
			get
			{
				return new LocalizedString("ExUnsupportedSeekReference", "Ex68EB89", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusCompleted
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusCompleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExTooManySubscriptionsOnPublicStore(string serverFqdn)
		{
			return new LocalizedString("ExTooManySubscriptionsOnPublicStore", "ExEF6549", false, true, ServerStrings.ResourceManager, new object[]
			{
				serverFqdn
			});
		}

		public static LocalizedString MigrationTestMSAWarning
		{
			get
			{
				return new LocalizedString("MigrationTestMSAWarning", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDateTimeRange
		{
			get
			{
				return new LocalizedString("InvalidDateTimeRange", "Ex2E5F2C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCalculatedPropertyStreamAccessNotSupported(string property)
		{
			return new LocalizedString("ExCalculatedPropertyStreamAccessNotSupported", "Ex3A51CF", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString InvalidRmsUrl(ServiceType type, string response)
		{
			return new LocalizedString("InvalidRmsUrl", "Ex298347", false, true, ServerStrings.ResourceManager, new object[]
			{
				type,
				response
			});
		}

		public static LocalizedString AmRpcOperationNotImplemented(string operationHint, string serverName)
		{
			return new LocalizedString("AmRpcOperationNotImplemented", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				operationHint,
				serverName
			});
		}

		public static LocalizedString SharingFolderName(string folderName, string sharer)
		{
			return new LocalizedString("SharingFolderName", "Ex46F8A3", false, true, ServerStrings.ResourceManager, new object[]
			{
				folderName,
				sharer
			});
		}

		public static LocalizedString MapiCannotGetMapiTable
		{
			get
			{
				return new LocalizedString("MapiCannotGetMapiTable", "ExF264D9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCheckForNotifications
		{
			get
			{
				return new LocalizedString("MapiCannotCheckForNotifications", "ExD6EAF7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationNotAllowed(string operation, string type)
		{
			return new LocalizedString("OperationNotAllowed", "Ex314361", false, true, ServerStrings.ResourceManager, new object[]
			{
				operation,
				type
			});
		}

		public static LocalizedString CannotStamplocalFreeBusyId
		{
			get
			{
				return new LocalizedString("CannotStamplocalFreeBusyId", "Ex72C521", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingPolicyNotFound(string policy)
		{
			return new LocalizedString("SharingPolicyNotFound", "Ex32A5DA", false, true, ServerStrings.ResourceManager, new object[]
			{
				policy
			});
		}

		public static LocalizedString ClientCulture_0x100A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x100A", "ExCD7310", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusSynced
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusSynced", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExchangePrincipalFromMailboxDataError
		{
			get
			{
				return new LocalizedString("ExchangePrincipalFromMailboxDataError", "ExEF79A4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusIncrementalFailed
		{
			get
			{
				return new LocalizedString("MigrationUserStatusIncrementalFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidXml
		{
			get
			{
				return new LocalizedString("InvalidXml", "Ex6F8448", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidExternalSharingInitiatorException(string initiator, string sender)
		{
			return new LocalizedString("InvalidExternalSharingInitiatorException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				initiator,
				sender
			});
		}

		public static LocalizedString ExEntryIdToLong
		{
			get
			{
				return new LocalizedString("ExEntryIdToLong", "Ex2718EE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSortGroupNotSupportedForProperty(string propertyName)
		{
			return new LocalizedString("ExSortGroupNotSupportedForProperty", "ExEC90F1", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString ClientCulture_0x420
		{
			get
			{
				return new LocalizedString("ClientCulture_0x420", "ExCD0590", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationContainsDuplicateMids(string legacyDN, long mid)
		{
			return new LocalizedString("ConversationContainsDuplicateMids", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				legacyDN,
				mid
			});
		}

		public static LocalizedString PrincipalFromDifferentSite
		{
			get
			{
				return new LocalizedString("PrincipalFromDifferentSite", "Ex669A85", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSavingRules
		{
			get
			{
				return new LocalizedString("ErrorSavingRules", "Ex2BFE42", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublishedFolderAccessDeniedException
		{
			get
			{
				return new LocalizedString("PublishedFolderAccessDeniedException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFoldersNotEnabledForEnterprise
		{
			get
			{
				return new LocalizedString("PublicFoldersNotEnabledForEnterprise", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeApprovalRequest
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeApprovalRequest", "ExB4DDBD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonUniqueRecipientError
		{
			get
			{
				return new LocalizedString("NonUniqueRecipientError", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParticipantPropertyTooBig(string property)
		{
			return new LocalizedString("ParticipantPropertyTooBig", "Ex7AD3E4", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString CopyToDumpsterFailure(string error)
		{
			return new LocalizedString("CopyToDumpsterFailure", "Ex0AAB56", false, true, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ExSystemFolderAccessDenied
		{
			get
			{
				return new LocalizedString("ExSystemFolderAccessDenied", "ExB7E350", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNotSupportedConfigurationSearchFlags(string flags)
		{
			return new LocalizedString("ExNotSupportedConfigurationSearchFlags", "Ex5EBE6D", false, true, ServerStrings.ResourceManager, new object[]
			{
				flags
			});
		}

		public static LocalizedString MapiCannotRemoveNotification
		{
			get
			{
				return new LocalizedString("MapiCannotRemoveNotification", "Ex8848E7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidParticipant(string explanation, object status)
		{
			return new LocalizedString("InvalidParticipant", "ExA8F2DB", false, true, ServerStrings.ResourceManager, new object[]
			{
				explanation,
				status
			});
		}

		public static LocalizedString ClientCulture_0x180A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x180A", "ExB8A2FF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidEventWatermarkBadOrigin(Guid watermarkOrigin, Guid eventSourceOrigin)
		{
			return new LocalizedString("ExInvalidEventWatermarkBadOrigin", "ExF64E65", false, true, ServerStrings.ResourceManager, new object[]
			{
				watermarkOrigin,
				eventSourceOrigin
			});
		}

		public static LocalizedString ExCommentFilterPropertiesNotSupported
		{
			get
			{
				return new LocalizedString("ExCommentFilterPropertiesNotSupported", "Ex34268D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExPropertyNotValidOnOccurrence(string property)
		{
			return new LocalizedString("ExPropertyNotValidOnOccurrence", "ExC92BEB", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString DefaultFolderAccessDenied(string folder)
		{
			return new LocalizedString("DefaultFolderAccessDenied", "Ex68AB5D", false, true, ServerStrings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString ExDictionaryDataCorruptedNullKey
		{
			get
			{
				return new LocalizedString("ExDictionaryDataCorruptedNullKey", "Ex6CD17C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString COWMailboxInfoCacheTimeout(double seconds, int count)
		{
			return new LocalizedString("COWMailboxInfoCacheTimeout", "Ex48E4DC", false, true, ServerStrings.ResourceManager, new object[]
			{
				seconds,
				count
			});
		}

		public static LocalizedString MigrationBatchStatusStarting
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusStarting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFinalEventFound(string finalEvent)
		{
			return new LocalizedString("ExFinalEventFound", "Ex6CAAE4", false, true, ServerStrings.ResourceManager, new object[]
			{
				finalEvent
			});
		}

		public static LocalizedString ClientCulture_0x300A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x300A", "ExFAADEB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToFindLicenseUri(string tenantId)
		{
			return new LocalizedString("FailedToFindLicenseUri", "Ex4FC2FB", false, true, ServerStrings.ResourceManager, new object[]
			{
				tenantId
			});
		}

		public static LocalizedString ExBadValueForTypeCode0
		{
			get
			{
				return new LocalizedString("ExBadValueForTypeCode0", "Ex8DDAFC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeProposalInvalidOnRecurringMaster
		{
			get
			{
				return new LocalizedString("ErrorTimeProposalInvalidOnRecurringMaster", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadEnumValue(Type enumType)
		{
			return new LocalizedString("BadEnumValue", "ExFCB7A2", false, true, ServerStrings.ResourceManager, new object[]
			{
				enumType
			});
		}

		public static LocalizedString SearchStateDeletionInProgress
		{
			get
			{
				return new LocalizedString("SearchStateDeletionInProgress", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExRuleIdInvalid
		{
			get
			{
				return new LocalizedString("ExRuleIdInvalid", "ExE0354D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotStreamablePropertyValue(Type type)
		{
			return new LocalizedString("NotStreamablePropertyValue", "Ex46AB03", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ObjectMustBeOfType(string type)
		{
			return new LocalizedString("ObjectMustBeOfType", "Ex6C734F", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString MapiCannotCollapseRow
		{
			get
			{
				return new LocalizedString("MapiCannotCollapseRow", "Ex0B1A60", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingUnableToGenerateEncryptedSharedFolderData
		{
			get
			{
				return new LocalizedString("SharingUnableToGenerateEncryptedSharedFolderData", "ExEA5D23", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConnectionNotCached
		{
			get
			{
				return new LocalizedString("ExConnectionNotCached", "ExC60AD3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCorruptedData(string name)
		{
			return new LocalizedString("ErrorCorruptedData", "Ex55B78A", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CVSPopulationTimedout
		{
			get
			{
				return new LocalizedString("CVSPopulationTimedout", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadDateFormatInChangeDate
		{
			get
			{
				return new LocalizedString("BadDateFormatInChangeDate", "Ex73BFD1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusCompletedWithErrors
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusCompletedWithErrors", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueryUsageRightsPrelicenseResponseHasFailure(string uri, RightsManagementFailureCode failure)
		{
			return new LocalizedString("QueryUsageRightsPrelicenseResponseHasFailure", "Ex49BEFC", false, true, ServerStrings.ResourceManager, new object[]
			{
				uri,
				failure
			});
		}

		public static LocalizedString NotReadSubjectPrefix
		{
			get
			{
				return new LocalizedString("NotReadSubjectPrefix", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotFinishSubmit
		{
			get
			{
				return new LocalizedString("MapiCannotFinishSubmit", "ExCBFB85", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0xC01
		{
			get
			{
				return new LocalizedString("ClientCulture_0xC01", "ExEED192", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExItemNotFoundInClientManifest
		{
			get
			{
				return new LocalizedString("ExItemNotFoundInClientManifest", "Ex033D29", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNoStoreObjectId
		{
			get
			{
				return new LocalizedString("ErrorNoStoreObjectId", "ExFB36A8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalendarItemCorrelationFailed
		{
			get
			{
				return new LocalizedString("CalendarItemCorrelationFailed", "ExB2C2AD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidOccurrenceId
		{
			get
			{
				return new LocalizedString("ExInvalidOccurrenceId", "Ex765023", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateRangeOneWeek
		{
			get
			{
				return new LocalizedString("DateRangeOneWeek", "Ex10D3FA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EnforceRulesQuota
		{
			get
			{
				return new LocalizedString("EnforceRulesQuota", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationMailboxNotFoundOnServerError(string mailboxName, string expected, string found)
		{
			return new LocalizedString("MigrationMailboxNotFoundOnServerError", "ExD46017", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxName,
				expected,
				found
			});
		}

		public static LocalizedString ExInvalidMonth
		{
			get
			{
				return new LocalizedString("ExInvalidMonth", "ExEB368D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusCompletionSynced
		{
			get
			{
				return new LocalizedString("MigrationUserStatusCompletionSynced", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstFullWeek
		{
			get
			{
				return new LocalizedString("FirstFullWeek", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StreamPropertyNotFound(string property)
		{
			return new LocalizedString("StreamPropertyNotFound", "Ex78704B", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString idOscContactSourcesForContactParseError(string errMessage)
		{
			return new LocalizedString("idOscContactSourcesForContactParseError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString MigrationFeatureEndpoints
		{
			get
			{
				return new LocalizedString("MigrationFeatureEndpoints", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNoSearchHasBeenInitiated
		{
			get
			{
				return new LocalizedString("ExNoSearchHasBeenInitiated", "Ex69E186", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusIncrementalSyncing
		{
			get
			{
				return new LocalizedString("MigrationUserStatusIncrementalSyncing", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExIncorrectOriginalTimeInExtendedExceptionInfo(object originalTime, object invalidTime)
		{
			return new LocalizedString("ExIncorrectOriginalTimeInExtendedExceptionInfo", "Ex5438A5", false, true, ServerStrings.ResourceManager, new object[]
			{
				originalTime,
				invalidTime
			});
		}

		public static LocalizedString PublicFolderMailboxesCannotBeCreatedDuringMigration
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxesCannotBeCreatedDuringMigration", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DeleteFromDumpsterFailure(string mailbox)
		{
			return new LocalizedString("DeleteFromDumpsterFailure", "Ex8F6768", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString DetailLevelNotAllowedByPolicy(string detailLevel)
		{
			return new LocalizedString("DetailLevelNotAllowedByPolicy", "Ex3E3DE0", false, true, ServerStrings.ResourceManager, new object[]
			{
				detailLevel
			});
		}

		public static LocalizedString MapiCannotCreateFilter
		{
			get
			{
				return new LocalizedString("MapiCannotCreateFilter", "ExD9C071", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotNotifyMessageNewMail
		{
			get
			{
				return new LocalizedString("MapiCannotNotifyMessageNewMail", "ExCA8F9B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCalendarReminderNegative(string value)
		{
			return new LocalizedString("ErrorCalendarReminderNegative", "ExAA98E7", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString GroupMailboxAccessSidConstructionFailed(Guid groupMailboxGuid, string userType)
		{
			return new LocalizedString("GroupMailboxAccessSidConstructionFailed", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				groupMailboxGuid,
				userType
			});
		}

		public static LocalizedString MigrationUserStatusSyncing
		{
			get
			{
				return new LocalizedString("MigrationUserStatusSyncing", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchFlagForceNewMigration
		{
			get
			{
				return new LocalizedString("MigrationBatchFlagForceNewMigration", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetFinalStateSynchronizerProviderBase
		{
			get
			{
				return new LocalizedString("CannotGetFinalStateSynchronizerProviderBase", "Ex283160", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerLocatorClientWCFCallCommunicationError
		{
			get
			{
				return new LocalizedString("ServerLocatorClientWCFCallCommunicationError", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationTimedOut(string timeout)
		{
			return new LocalizedString("OperationTimedOut", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				timeout
			});
		}

		public static LocalizedString MailboxVersionTooLow(string mailbox, string expectedVersion, string actualVersion)
		{
			return new LocalizedString("MailboxVersionTooLow", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailbox,
				expectedVersion,
				actualVersion
			});
		}

		public static LocalizedString ExValueCannotBeNull
		{
			get
			{
				return new LocalizedString("ExValueCannotBeNull", "ExEDF149", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x3009
		{
			get
			{
				return new LocalizedString("ClientCulture_0x3009", "Ex626C90", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotSaveSyncStateFolder(string folderName, string saveResult)
		{
			return new LocalizedString("ExCannotSaveSyncStateFolder", "Ex3C7A8F", false, true, ServerStrings.ResourceManager, new object[]
			{
				folderName,
				saveResult
			});
		}

		public static LocalizedString MigrationTypeBulkProvisioning
		{
			get
			{
				return new LocalizedString("MigrationTypeBulkProvisioning", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderIsMailEnabled
		{
			get
			{
				return new LocalizedString("ErrorFolderIsMailEnabled", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCantAccessOccurrenceFromNewItem
		{
			get
			{
				return new LocalizedString("ExCantAccessOccurrenceFromNewItem", "Ex4954C0", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNotSupportedNotificationType(uint notificationType)
		{
			return new LocalizedString("ExNotSupportedNotificationType", "Ex5A45EF", false, true, ServerStrings.ResourceManager, new object[]
			{
				notificationType
			});
		}

		public static LocalizedString ConversionInvalidItemType(string type)
		{
			return new LocalizedString("ConversionInvalidItemType", "ExBF3BED", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString ConversionCorruptContent
		{
			get
			{
				return new LocalizedString("ConversionCorruptContent", "ExADAC25", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoDFailedToGetToken
		{
			get
			{
				return new LocalizedString("AutoDFailedToGetToken", "Ex62564D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCorruptPropertyTag
		{
			get
			{
				return new LocalizedString("ExCorruptPropertyTag", "ExC0FBE1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTimeSlot
		{
			get
			{
				return new LocalizedString("InvalidTimeSlot", "ExD7205E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotOpenMultipleCorrelatedItems
		{
			get
			{
				return new LocalizedString("ExCannotOpenMultipleCorrelatedItems", "ExD91EEA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindRequestIndexEntry(Guid requestGuid)
		{
			return new LocalizedString("CannotFindRequestIndexEntry", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				requestGuid
			});
		}

		public static LocalizedString ErrorLanguageIsNull
		{
			get
			{
				return new LocalizedString("ErrorLanguageIsNull", "Ex7E96B9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExBodyFormatConversionNotSupported(string format)
		{
			return new LocalizedString("ExBodyFormatConversionNotSupported", "Ex6F3C48", false, true, ServerStrings.ResourceManager, new object[]
			{
				format
			});
		}

		public static LocalizedString ExConfigExisted(string configName)
		{
			return new LocalizedString("ExConfigExisted", "ExF3EA79", false, true, ServerStrings.ResourceManager, new object[]
			{
				configName
			});
		}

		public static LocalizedString ExInvalidAcrBaseProfiles
		{
			get
			{
				return new LocalizedString("ExInvalidAcrBaseProfiles", "ExBDE4F3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExMustSaveFolderToApplySearch
		{
			get
			{
				return new LocalizedString("ExMustSaveFolderToApplySearch", "ExD00A99", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExReadTopologyTimeout
		{
			get
			{
				return new LocalizedString("ExReadTopologyTimeout", "ExA11951", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExUnknownRecurrenceBlobType
		{
			get
			{
				return new LocalizedString("ExUnknownRecurrenceBlobType", "ExB6B567", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x419
		{
			get
			{
				return new LocalizedString("ClientCulture_0x419", "ExA616B9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetSynchronizeBuffers(Type mapiManifestType)
		{
			return new LocalizedString("CannotGetSynchronizeBuffers", "Ex288C19", false, true, ServerStrings.ResourceManager, new object[]
			{
				mapiManifestType
			});
		}

		public static LocalizedString SpellCheckerHebrew
		{
			get
			{
				return new LocalizedString("SpellCheckerHebrew", "ExA0672B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TenantLicensesDistributionPointMismatch(string tenantId, Uri serviceLocation, Uri publishLocation)
		{
			return new LocalizedString("TenantLicensesDistributionPointMismatch", "Ex549A2F", false, true, ServerStrings.ResourceManager, new object[]
			{
				tenantId,
				serviceLocation,
				publishLocation
			});
		}

		public static LocalizedString ClientCulture_0x1001
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1001", "ExA7B6EC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttachmentId
		{
			get
			{
				return new LocalizedString("InvalidAttachmentId", "ExF4240C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x43F
		{
			get
			{
				return new LocalizedString("ClientCulture_0x43F", "Ex5A6B54", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidFolderId
		{
			get
			{
				return new LocalizedString("ExInvalidFolderId", "Ex507E00", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExUrlNotFound(Uri url)
		{
			return new LocalizedString("ExUrlNotFound", "Ex835A9C", false, true, ServerStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString AmDbMountNotAllowedDueToRegistryConfigurationException
		{
			get
			{
				return new LocalizedString("AmDbMountNotAllowedDueToRegistryConfigurationException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSaveReadOnlyAttachment
		{
			get
			{
				return new LocalizedString("CannotSaveReadOnlyAttachment", "Ex55ACB2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidTnef
		{
			get
			{
				return new LocalizedString("InvalidTnef", "ExED4FFA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusIncrementalSynced
		{
			get
			{
				return new LocalizedString("MigrationUserStatusIncrementalSynced", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidICalElement(string name)
		{
			return new LocalizedString("InvalidICalElement", "Ex778037", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExAdminAuditLogsDeleteDenied
		{
			get
			{
				return new LocalizedString("ExAdminAuditLogsDeleteDenied", "ExC3F166", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToRpcAcquireUseLicenses(string orgId, string status, string server)
		{
			return new LocalizedString("FailedToRpcAcquireUseLicenses", "ExCAAB7A", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId,
				status,
				server
			});
		}

		public static LocalizedString ExUnknownPattern(object pattern)
		{
			return new LocalizedString("ExUnknownPattern", "Ex2AEAB5", false, true, ServerStrings.ResourceManager, new object[]
			{
				pattern
			});
		}

		public static LocalizedString ConversionInvalidMessageCodepageCharset
		{
			get
			{
				return new LocalizedString("ConversionInvalidMessageCodepageCharset", "ExCFF2A5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFilterNotSupportedForProperty(string filterType, string propertyName)
		{
			return new LocalizedString("ExFilterNotSupportedForProperty", "Ex876A64", false, true, ServerStrings.ResourceManager, new object[]
			{
				filterType,
				propertyName
			});
		}

		public static LocalizedString SearchObjectIsInvalid(string id)
		{
			return new LocalizedString("SearchObjectIsInvalid", "ExA4E124", false, true, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ClientCulture_0x40C
		{
			get
			{
				return new LocalizedString("ClientCulture_0x40C", "ExF37899", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DumpsterStatusShutdownException
		{
			get
			{
				return new LocalizedString("DumpsterStatusShutdownException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToDownloadServerLicensingMExData(Uri url)
		{
			return new LocalizedString("FailedToDownloadServerLicensingMExData", "Ex1C7BFF", false, true, ServerStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString ActiveMonitoringOperationFailedException(string errMessage)
		{
			return new LocalizedString("ActiveMonitoringOperationFailedException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString CannotDeleteRootFolder
		{
			get
			{
				return new LocalizedString("CannotDeleteRootFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetEffectiveRights
		{
			get
			{
				return new LocalizedString("MapiCannotGetEffectiveRights", "Ex930822", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMechanismToAccessGroupMailbox
		{
			get
			{
				return new LocalizedString("InvalidMechanismToAccessGroupMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedClientOperation(string client)
		{
			return new LocalizedString("UnsupportedClientOperation", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				client
			});
		}

		public static LocalizedString ExternalLicensingDisabledForEnterprise(Uri uri)
		{
			return new LocalizedString("ExternalLicensingDisabledForEnterprise", "Ex3D3E94", false, true, ServerStrings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString MapiCannotSavePermissions
		{
			get
			{
				return new LocalizedString("MapiCannotSavePermissions", "Ex75B8FE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x1004
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1004", "ExF84280", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusCreated
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusCreated", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListXsoTooBigException(string value)
		{
			return new LocalizedString("JunkEmailBlockedListXsoTooBigException", "ExD65092", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString AmDatabaseCopyNotFoundException(string dbName, string serverName)
		{
			return new LocalizedString("AmDatabaseCopyNotFoundException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dbName,
				serverName
			});
		}

		public static LocalizedString NotAllowedExternalSharingByPolicy
		{
			get
			{
				return new LocalizedString("NotAllowedExternalSharingByPolicy", "Ex36DB97", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeReadReceipt
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeReadReceipt", "ExDEB1AE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreOperationFailed
		{
			get
			{
				return new LocalizedString("StoreOperationFailed", "Ex544968", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExTimeZoneValueNoGmtMatch
		{
			get
			{
				return new LocalizedString("ErrorExTimeZoneValueNoGmtMatch", "Ex97A326", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ValueNotRecognizedForAttribute(string name)
		{
			return new LocalizedString("ValueNotRecognizedForAttribute", "ExE3054F", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString AmOperationFailedWithEcException(int ec)
		{
			return new LocalizedString("AmOperationFailedWithEcException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString ExStoreObjectValidationError
		{
			get
			{
				return new LocalizedString("ExStoreObjectValidationError", "Ex44DC6D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchFlagNone
		{
			get
			{
				return new LocalizedString("MigrationBatchFlagNone", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x4009
		{
			get
			{
				return new LocalizedString("ClientCulture_0x4009", "Ex79B0A0", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TooManyAttachmentsOnProtectedMessage
		{
			get
			{
				return new LocalizedString("TooManyAttachmentsOnProtectedMessage", "ExB4408A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RuleParseError(string parameter)
		{
			return new LocalizedString("RuleParseError", "Ex591050", false, true, ServerStrings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString PublicFolderOpenFailedOnExistingFolder
		{
			get
			{
				return new LocalizedString("PublicFolderOpenFailedOnExistingFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidSortOrder
		{
			get
			{
				return new LocalizedString("ExInvalidSortOrder", "ExF0900A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ReplyRuleNotSupportedOnNonMailPublicFolder
		{
			get
			{
				return new LocalizedString("ReplyRuleNotSupportedOnNonMailPublicFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExGetPropsFailed
		{
			get
			{
				return new LocalizedString("ExGetPropsFailed", "Ex4AF038", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EstimateStateSucceeded
		{
			get
			{
				return new LocalizedString("EstimateStateSucceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchSupportedActionRemove
		{
			get
			{
				return new LocalizedString("MigrationBatchSupportedActionRemove", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSaveMessageStream
		{
			get
			{
				return new LocalizedString("MapiCannotSaveMessageStream", "Ex40EBA3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderRuleErrorRecordForSpecificRule(string id, string recipient, string stage, string exceptionType, string exceptionMessage)
		{
			return new LocalizedString("FolderRuleErrorRecordForSpecificRule", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				id,
				recipient,
				stage,
				exceptionType,
				exceptionMessage
			});
		}

		public static LocalizedString MapiInvalidId
		{
			get
			{
				return new LocalizedString("MapiInvalidId", "Ex00D65B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContactLinkingMaximumNumberOfContactsPerPersonError
		{
			get
			{
				return new LocalizedString("ContactLinkingMaximumNumberOfContactsPerPersonError", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintUnknown(Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintUnknown", "ExFCEC68", false, true, ServerStrings.ResourceManager, new object[]
			{
				databaseGuid
			});
		}

		public static LocalizedString ConversionUnsupportedContent
		{
			get
			{
				return new LocalizedString("ConversionUnsupportedContent", "Ex274731", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusIncrementalStopped
		{
			get
			{
				return new LocalizedString("MigrationUserStatusIncrementalStopped", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCreateMessage
		{
			get
			{
				return new LocalizedString("MapiCannotCreateMessage", "ExDCC5F1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExPropertyValidationFailed(string errorDescription, object property)
		{
			return new LocalizedString("ExPropertyValidationFailed", "Ex60EC60", false, true, ServerStrings.ResourceManager, new object[]
			{
				errorDescription,
				property
			});
		}

		public static LocalizedString InvalidSendAddressIdentity
		{
			get
			{
				return new LocalizedString("InvalidSendAddressIdentity", "Ex30307E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x425
		{
			get
			{
				return new LocalizedString("ClientCulture_0x425", "Ex64157B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString BadTimeFormatInChangeDate(string time)
		{
			return new LocalizedString("BadTimeFormatInChangeDate", "ExEE9D6C", false, true, ServerStrings.ResourceManager, new object[]
			{
				time
			});
		}

		public static LocalizedString DisposeOOFHistoryFolder
		{
			get
			{
				return new LocalizedString("DisposeOOFHistoryFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCantDeleteLastOccurrence
		{
			get
			{
				return new LocalizedString("ExCantDeleteLastOccurrence", "ExA05995", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDeleteNotSupportedForCalculatedProperty(object proptertyID)
		{
			return new LocalizedString("ExDeleteNotSupportedForCalculatedProperty", "Ex7EB285", false, true, ServerStrings.ResourceManager, new object[]
			{
				proptertyID
			});
		}

		public static LocalizedString MigrationReportBatchSuccess
		{
			get
			{
				return new LocalizedString("MigrationReportBatchSuccess", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DagNetworkCreateDupName(string name)
		{
			return new LocalizedString("DagNetworkCreateDupName", "ExE45F06", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ErrorAccessingLargeProperty
		{
			get
			{
				return new LocalizedString("ErrorAccessingLargeProperty", "ExAF00CF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationNotSupportedOnPublicFolderMailbox
		{
			get
			{
				return new LocalizedString("OperationNotSupportedOnPublicFolderMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotCreateMeetingCancellation
		{
			get
			{
				return new LocalizedString("ExCannotCreateMeetingCancellation", "ExB794C5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFeaturePAW
		{
			get
			{
				return new LocalizedString("MigrationFeaturePAW", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintNotSatisfiedInvalidConstraint(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintNotSatisfiedInvalidConstraint", "ExD92778", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid
			});
		}

		public static LocalizedString AmOperationFailedException(string errMessage)
		{
			return new LocalizedString("AmOperationFailedException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString InboxRuleFlagStatusFlagged
		{
			get
			{
				return new LocalizedString("InboxRuleFlagStatusFlagged", "ExF925D7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExMismatchedSyncStateDataType(string expectedType, string actualType)
		{
			return new LocalizedString("ExMismatchedSyncStateDataType", "Ex311F7C", false, true, ServerStrings.ResourceManager, new object[]
			{
				expectedType,
				actualType
			});
		}

		public static LocalizedString JunkEmailBlockedListXsoNullException
		{
			get
			{
				return new LocalizedString("JunkEmailBlockedListXsoNullException", "ExC8C78D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcServerIgnoreNotFoundTeamMailbox(string mailboxGuid)
		{
			return new LocalizedString("RpcServerIgnoreNotFoundTeamMailbox", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString ClientCulture_0x43E
		{
			get
			{
				return new LocalizedString("ClientCulture_0x43E", "Ex339C0C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AnchorDatabaseNotFound(string mdbGuid)
		{
			return new LocalizedString("AnchorDatabaseNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString TeamMailboxMessageGoToYourGroupSite
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageGoToYourGroupSite", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x81A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x81A", "Ex36BF9E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNoMatchingStorePropertyDefinition(string property)
		{
			return new LocalizedString("ExNoMatchingStorePropertyDefinition", "Ex30106A", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString CannotImportMessageMove
		{
			get
			{
				return new LocalizedString("CannotImportMessageMove", "Ex9F8509", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FifteenMinutes
		{
			get
			{
				return new LocalizedString("FifteenMinutes", "ExD37565", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OneDays
		{
			get
			{
				return new LocalizedString("OneDays", "Ex553547", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptNaturalLanguageProperty
		{
			get
			{
				return new LocalizedString("CorruptNaturalLanguageProperty", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToRetrieveUserLicense(string userName, int errorCode)
		{
			return new LocalizedString("FailedToRetrieveUserLicense", "ExEBF2BA", false, true, ServerStrings.ResourceManager, new object[]
			{
				userName,
				errorCode
			});
		}

		public static LocalizedString DumpsterStatusAlreadyStartedException
		{
			get
			{
				return new LocalizedString("DumpsterStatusAlreadyStartedException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotSetSearchCriteria
		{
			get
			{
				return new LocalizedString("ExCannotSetSearchCriteria", "ExC1AAF5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMailboxType(string externalDirectoryObjectId, string type)
		{
			return new LocalizedString("InvalidMailboxType", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				externalDirectoryObjectId,
				type
			});
		}

		public static LocalizedString ExBadObjectType
		{
			get
			{
				return new LocalizedString("ExBadObjectType", "Ex7CA0F8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerFinnish
		{
			get
			{
				return new LocalizedString("SpellCheckerFinnish", "Ex43DA68", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusWaiting
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusWaiting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToFindServerInfo(Uri url)
		{
			return new LocalizedString("FailedToFindServerInfo", "Ex3DBB29", false, true, ServerStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString UnsupportedKindKeywords
		{
			get
			{
				return new LocalizedString("UnsupportedKindKeywords", "ExC651A7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x407
		{
			get
			{
				return new LocalizedString("ClientCulture_0x407", "ExF9CD83", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyChangeMetadataParseError
		{
			get
			{
				return new LocalizedString("PropertyChangeMetadataParseError", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadSharedServerBoxRacIdentityFromIRMConfig(string orgId)
		{
			return new LocalizedString("FailedToReadSharedServerBoxRacIdentityFromIRMConfig", "Ex9FD150", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString ExNonCalendarItemReturned(string msgClass)
		{
			return new LocalizedString("ExNonCalendarItemReturned", "Ex8CE0C4", false, true, ServerStrings.ResourceManager, new object[]
			{
				msgClass
			});
		}

		public static LocalizedString SyncFailedToCreateNewItemOrBindToExistingOne
		{
			get
			{
				return new LocalizedString("SyncFailedToCreateNewItemOrBindToExistingOne", "Ex6A0319", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegateValidationFailed(string name)
		{
			return new LocalizedString("DelegateValidationFailed", "Ex5C0C76", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ConversionFailedInvalidMacBin
		{
			get
			{
				return new LocalizedString("ConversionFailedInvalidMacBin", "ExAB4FF2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerEnglishUnitedStates
		{
			get
			{
				return new LocalizedString("SpellCheckerEnglishUnitedStates", "Ex947099", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExResponseTypeNoSubjectPrefix(string responseType)
		{
			return new LocalizedString("ExResponseTypeNoSubjectPrefix", "Ex85E45A", false, true, ServerStrings.ResourceManager, new object[]
			{
				responseType
			});
		}

		public static LocalizedString ExContactHasNoId
		{
			get
			{
				return new LocalizedString("ExContactHasNoId", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationNSPINoUsersFound(string exchangeServer)
		{
			return new LocalizedString("MigrationNSPINoUsersFound", "ExB6381B", false, true, ServerStrings.ResourceManager, new object[]
			{
				exchangeServer
			});
		}

		public static LocalizedString ErrorExTimeZoneValueTimeZoneNotFound
		{
			get
			{
				return new LocalizedString("ErrorExTimeZoneValueTimeZoneNotFound", "ExECE8E5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerGermanPostReform
		{
			get
			{
				return new LocalizedString("SpellCheckerGermanPostReform", "ExADA197", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypePermissionControlled
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypePermissionControlled", "Ex3F80B1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x40F
		{
			get
			{
				return new LocalizedString("ClientCulture_0x40F", "ExD2A74A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyDefinitionsValuesNotMatch
		{
			get
			{
				return new LocalizedString("PropertyDefinitionsValuesNotMatch", "Ex9D6AF5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListXsoGenericException(string value)
		{
			return new LocalizedString("JunkEmailTrustedListXsoGenericException", "Ex06F12D", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ClientCulture_0xC1A
		{
			get
			{
				return new LocalizedString("ClientCulture_0xC1A", "Ex2B5F79", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateRangeThreeMonths
		{
			get
			{
				return new LocalizedString("DateRangeThreeMonths", "Ex8F5EE5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConnectionAlternate
		{
			get
			{
				return new LocalizedString("ExConnectionAlternate", "ExA872D8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchSupportedActionStart
		{
			get
			{
				return new LocalizedString("MigrationBatchSupportedActionStart", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x402
		{
			get
			{
				return new LocalizedString("ClientCulture_0x402", "ExA5CF99", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AttemptingSessionCreationAgainstWrongGroupMailbox(string current, string original)
		{
			return new LocalizedString("AttemptingSessionCreationAgainstWrongGroupMailbox", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				current,
				original
			});
		}

		public static LocalizedString ExCannotAccessAdminAuditLogsFolderId
		{
			get
			{
				return new LocalizedString("ExCannotAccessAdminAuditLogsFolderId", "Ex0F6EAC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x424
		{
			get
			{
				return new LocalizedString("ClientCulture_0x424", "Ex64E988", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStateWaiting
		{
			get
			{
				return new LocalizedString("MigrationStateWaiting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStageProcessing
		{
			get
			{
				return new LocalizedString("MigrationStageProcessing", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Database
		{
			get
			{
				return new LocalizedString("Database", "Ex94C5DB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidTypeInBookingPolicyConfig(string type, string parameter)
		{
			return new LocalizedString("descInvalidTypeInBookingPolicyConfig", "Ex544FD6", false, true, ServerStrings.ResourceManager, new object[]
			{
				type,
				parameter
			});
		}

		public static LocalizedString MapiCannotGetTransportQueueFolderId
		{
			get
			{
				return new LocalizedString("MapiCannotGetTransportQueueFolderId", "Ex5A898B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSupportException(LocalizedString exceptionMessage)
		{
			return new LocalizedString("NoSupportException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				exceptionMessage
			});
		}

		public static LocalizedString MalformedWorkingHours(string mailbox, string exceptionInfo)
		{
			return new LocalizedString("MalformedWorkingHours", "ExF3C4B1", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox,
				exceptionInfo
			});
		}

		public static LocalizedString DataMoveReplicationConstraintNotSatisfiedThrottled(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintNotSatisfiedThrottled", "Ex073F5D", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid
			});
		}

		public static LocalizedString UnsupportedAction
		{
			get
			{
				return new LocalizedString("UnsupportedAction", "Ex652C41", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderOperationDenied(string objectType, string operation)
		{
			return new LocalizedString("PublicFolderOperationDenied", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				objectType,
				operation
			});
		}

		public static LocalizedString ExInvalidO12BytesToSkip(int bytesToSkip)
		{
			return new LocalizedString("ExInvalidO12BytesToSkip", "ExBBDAA2", false, true, ServerStrings.ResourceManager, new object[]
			{
				bytesToSkip
			});
		}

		public static LocalizedString ColumnError(string columnName, LocalizedString description)
		{
			return new LocalizedString("ColumnError", "ExE345F2", false, true, ServerStrings.ResourceManager, new object[]
			{
				columnName,
				description
			});
		}

		public static LocalizedString FolderRuleErrorInvalidRecipientEntryId
		{
			get
			{
				return new LocalizedString("FolderRuleErrorInvalidRecipientEntryId", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageGoToTheSite
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageGoToTheSite", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TwelveHours
		{
			get
			{
				return new LocalizedString("TwelveHours", "Ex763518", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStageInjection
		{
			get
			{
				return new LocalizedString("MigrationStageInjection", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetContentsTable
		{
			get
			{
				return new LocalizedString("MapiCannotGetContentsTable", "ExFCDAC3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EstimateStateStopped
		{
			get
			{
				return new LocalizedString("EstimateStateStopped", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullWorkHours
		{
			get
			{
				return new LocalizedString("NullWorkHours", "Ex081036", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusCompleting
		{
			get
			{
				return new LocalizedString("MigrationUserStatusCompleting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorValidateXsoDriverProperty(string propertyName, string description)
		{
			return new LocalizedString("ErrorValidateXsoDriverProperty", "ExBB58F9", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyName,
				description
			});
		}

		public static LocalizedString FiveMinutes
		{
			get
			{
				return new LocalizedString("FiveMinutes", "Ex12C5DD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmInvalidActionCodeException(int actionCode, string member, string value)
		{
			return new LocalizedString("AmInvalidActionCodeException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				actionCode,
				member,
				value
			});
		}

		public static LocalizedString InboxRuleMessageTypeVoicemail
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeVoicemail", "Ex39910F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadIRMConfig(string orgId)
		{
			return new LocalizedString("FailedToReadIRMConfig", "ExABD478", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId
			});
		}

		public static LocalizedString SpellCheckerPortugueseBrasil
		{
			get
			{
				return new LocalizedString("SpellCheckerPortugueseBrasil", "ExF8A303", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CultureMismatchAfterConnect(string setCulture, string storeCulture)
		{
			return new LocalizedString("CultureMismatchAfterConnect", "ExEC3FAB", false, true, ServerStrings.ResourceManager, new object[]
			{
				setCulture,
				storeCulture
			});
		}

		public static LocalizedString GenericFailureRMDecryption
		{
			get
			{
				return new LocalizedString("GenericFailureRMDecryption", "Ex713D35", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalIdentityInvalidSid(string mailbox, string entryId, string sid)
		{
			return new LocalizedString("ExternalIdentityInvalidSid", "ExE7931F", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox,
				entryId,
				sid
			});
		}

		public static LocalizedString MaxRemindersExceeded(int count, int max)
		{
			return new LocalizedString("MaxRemindersExceeded", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				count,
				max
			});
		}

		public static LocalizedString SpellCheckerEnglishAustralia
		{
			get
			{
				return new LocalizedString("SpellCheckerEnglishAustralia", "ExE6AA20", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonUniqueAssociationError(string idProperty, string values)
		{
			return new LocalizedString("NonUniqueAssociationError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				idProperty,
				values
			});
		}

		public static LocalizedString NoDeferredActions
		{
			get
			{
				return new LocalizedString("NoDeferredActions", "Ex8053A8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorSetDateTimeFormatWithoutLanguage
		{
			get
			{
				return new LocalizedString("ErrorSetDateTimeFormatWithoutLanguage", "Ex477B73", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x1801
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1801", "Ex2C8945", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiErrorParsingId
		{
			get
			{
				return new LocalizedString("MapiErrorParsingId", "ExFF857F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserAdminTypePartnerTenant
		{
			get
			{
				return new LocalizedString("MigrationUserAdminTypePartnerTenant", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusStopped
		{
			get
			{
				return new LocalizedString("MigrationUserStatusStopped", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConfigSerializeDictionaryFailed(Exception e)
		{
			return new LocalizedString("ExConfigSerializeDictionaryFailed", "Ex8050F4", false, true, ServerStrings.ResourceManager, new object[]
			{
				e
			});
		}

		public static LocalizedString PublicFolderSyncFolderFailed(string innerMessage)
		{
			return new LocalizedString("PublicFolderSyncFolderFailed", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				innerMessage
			});
		}

		public static LocalizedString MigrationReportBatchFailure
		{
			get
			{
				return new LocalizedString("MigrationReportBatchFailure", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCreateAttachment
		{
			get
			{
				return new LocalizedString("MapiCannotCreateAttachment", "Ex03A335", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorUnsupportedConfigurationXmlCategory(string category)
		{
			return new LocalizedString("ErrorUnsupportedConfigurationXmlCategory", "Ex14D62D", false, true, ServerStrings.ResourceManager, new object[]
			{
				category
			});
		}

		public static LocalizedString FailedToRetrieveServerLicense(int errorCode)
		{
			return new LocalizedString("FailedToRetrieveServerLicense", "Ex6C68CC", false, true, ServerStrings.ResourceManager, new object[]
			{
				errorCode
			});
		}

		public static LocalizedString NotificationEmailBodyCertExpiring
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyCertExpiring", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotReadPerUserInformation
		{
			get
			{
				return new LocalizedString("MapiCannotReadPerUserInformation", "Ex948053", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidSubFilterProperty
		{
			get
			{
				return new LocalizedString("ExInvalidSubFilterProperty", "ExD17538", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StockReplyTemplate
		{
			get
			{
				return new LocalizedString("StockReplyTemplate", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalNotifTypeSummary
		{
			get
			{
				return new LocalizedString("CalNotifTypeSummary", "Ex57AA7C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailInvalidConstructionException
		{
			get
			{
				return new LocalizedString("JunkEmailInvalidConstructionException", "Ex1F7B70", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCreateAssociatedMessage
		{
			get
			{
				return new LocalizedString("MapiCannotCreateAssociatedMessage", "Ex924471", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x413
		{
			get
			{
				return new LocalizedString("ClientCulture_0x413", "Ex794C6C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSortTable
		{
			get
			{
				return new LocalizedString("MapiCannotSortTable", "Ex03E6C9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusStopping
		{
			get
			{
				return new LocalizedString("MigrationUserStatusStopping", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExPropertyRequiresStreaming(PropertyDefinition propertyDefinition)
		{
			return new LocalizedString("ExPropertyRequiresStreaming", "Ex9DA8F2", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyDefinition
			});
		}

		public static LocalizedString MapiCannotGetRecipientTable
		{
			get
			{
				return new LocalizedString("MapiCannotGetRecipientTable", "Ex8164D2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidCallToTryUpdateCalendarItem
		{
			get
			{
				return new LocalizedString("ExInvalidCallToTryUpdateCalendarItem", "Ex6F6133", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x406
		{
			get
			{
				return new LocalizedString("ClientCulture_0x406", "ExF0511D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationWithoutRootNode(string conversationId)
		{
			return new LocalizedString("ConversationWithoutRootNode", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				conversationId
			});
		}

		public static LocalizedString ExCannotAccessAuditsFolderId
		{
			get
			{
				return new LocalizedString("ExCannotAccessAuditsFolderId", "Ex9362A8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExReadEventsFailed
		{
			get
			{
				return new LocalizedString("ExReadEventsFailed", "ExA87AB7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotQueryAssociatedTable
		{
			get
			{
				return new LocalizedString("ExCannotQueryAssociatedTable", "Ex6D5BB3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idClientSessionInfoTypeParseException(string typeName, string assemblyName)
		{
			return new LocalizedString("idClientSessionInfoTypeParseException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				typeName,
				assemblyName
			});
		}

		public static LocalizedString ClientCulture_0x429
		{
			get
			{
				return new LocalizedString("ClientCulture_0x429", "Ex41D50D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStepProvisioningUpdate
		{
			get
			{
				return new LocalizedString("MigrationStepProvisioningUpdate", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExBatchBuilderError(object batchBuilder, string message)
		{
			return new LocalizedString("ExBatchBuilderError", "Ex6851DB", false, true, ServerStrings.ResourceManager, new object[]
			{
				batchBuilder,
				message
			});
		}

		public static LocalizedString ActiveManagerGenericRpcVersionNotSupported(int requestServerVersion, int requestCommandId, int requestCommandMajorVersion, int requestCommandMinorVersion, int actualServerVersion, int actualMajorVersion, int actualMinorVersion)
		{
			return new LocalizedString("ActiveManagerGenericRpcVersionNotSupported", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				requestServerVersion,
				requestCommandId,
				requestCommandMajorVersion,
				requestCommandMinorVersion,
				actualServerVersion,
				actualMajorVersion,
				actualMinorVersion
			});
		}

		public static LocalizedString ErrorTeamMailboxUserVersionUnqualified(string users)
		{
			return new LocalizedString("ErrorTeamMailboxUserVersionUnqualified", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				users
			});
		}

		public static LocalizedString TwoWeeks
		{
			get
			{
				return new LocalizedString("TwoWeeks", "Ex93CD82", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMonthNthOccurence(int nthOccurrence)
		{
			return new LocalizedString("ExInvalidMonthNthOccurence", "Ex7CAC44", false, true, ServerStrings.ResourceManager, new object[]
			{
				nthOccurrence
			});
		}

		public static LocalizedString MigrationFeatureUpgradeBlock
		{
			get
			{
				return new LocalizedString("MigrationFeatureUpgradeBlock", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidServiceType
		{
			get
			{
				return new LocalizedString("ExInvalidServiceType", "ExC2CDB3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NullTimeInChangeDate
		{
			get
			{
				return new LocalizedString("NullTimeInChangeDate", "ExE180A8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionInvalidSmimeClearSignedContent
		{
			get
			{
				return new LocalizedString("ConversionInvalidSmimeClearSignedContent", "Ex24059F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateSuspended
		{
			get
			{
				return new LocalizedString("RequestStateSuspended", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiIsFromPublicStoreCheckFailed
		{
			get
			{
				return new LocalizedString("MapiIsFromPublicStoreCheckFailed", "Ex55032A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidEventWatermarkBadEventCounter(long eventCounter, long lastObservedEventCounter)
		{
			return new LocalizedString("ExInvalidEventWatermarkBadEventCounter", "Ex810694", false, true, ServerStrings.ResourceManager, new object[]
			{
				eventCounter,
				lastObservedEventCounter
			});
		}

		public static LocalizedString ErrorInvalidDateFormat(string dateFormat, string lang, string validFormats)
		{
			return new LocalizedString("ErrorInvalidDateFormat", "Ex328A48", false, true, ServerStrings.ResourceManager, new object[]
			{
				dateFormat,
				lang,
				validFormats
			});
		}

		public static LocalizedString TaskRecurrenceNotSupported(string pattern, string range)
		{
			return new LocalizedString("TaskRecurrenceNotSupported", "Ex154990", false, true, ServerStrings.ResourceManager, new object[]
			{
				pattern,
				range
			});
		}

		public static LocalizedString ExCannotSendMeetingMessages
		{
			get
			{
				return new LocalizedString("ExCannotSendMeetingMessages", "Ex4C4CD7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAuditsDeleteDenied
		{
			get
			{
				return new LocalizedString("ExAuditsDeleteDenied", "ExB55749", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x416
		{
			get
			{
				return new LocalizedString("ClientCulture_0x416", "Ex002C22", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCreateCollector(Type xsoManifestType)
		{
			return new LocalizedString("CannotCreateCollector", "Ex4D427A", false, true, ServerStrings.ResourceManager, new object[]
			{
				xsoManifestType
			});
		}

		public static LocalizedString MissingPropertyValue
		{
			get
			{
				return new LocalizedString("MissingPropertyValue", "Ex377209", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderNotPublishedException
		{
			get
			{
				return new LocalizedString("FolderNotPublishedException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerLocatorClientEndpointNotFoundException
		{
			get
			{
				return new LocalizedString("ServerLocatorClientEndpointNotFoundException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataBaseNotFoundError(Guid databaseGuid)
		{
			return new LocalizedString("DataBaseNotFoundError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				databaseGuid
			});
		}

		public static LocalizedString ExTooComplexGroupSortParameter
		{
			get
			{
				return new LocalizedString("ExTooComplexGroupSortParameter", "Ex192DC8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedReportType(string className)
		{
			return new LocalizedString("UnsupportedReportType", "Ex3E9262", false, true, ServerStrings.ResourceManager, new object[]
			{
				className
			});
		}

		public static LocalizedString ExceptionIsNotPublicFolder(string name)
		{
			return new LocalizedString("ExceptionIsNotPublicFolder", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MapiCannotLookupEntryId
		{
			get
			{
				return new LocalizedString("MapiCannotLookupEntryId", "Ex52F50C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailBodyExportPSTCreated
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyExportPSTCreated", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageReactivatedBodyIntroText
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageReactivatedBodyIntroText", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotSupportedWithMailboxVersionException
		{
			get
			{
				return new LocalizedString("NotSupportedWithMailboxVersionException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x3409
		{
			get
			{
				return new LocalizedString("ClientCulture_0x3409", "Ex9BC8BD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExTooManySubscriptions(string mailbox, string server)
		{
			return new LocalizedString("ExTooManySubscriptions", "Ex40965C", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox,
				server
			});
		}

		public static LocalizedString ActiveMonitoringRpcVersionNotSupported(int requestServerVersion, int requestCommandId, int requestCommandMajorVersion, int requestCommandMinorVersion, int actualServerVersion, int actualMajorVersion, int actualMinorVersion)
		{
			return new LocalizedString("ActiveMonitoringRpcVersionNotSupported", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				requestServerVersion,
				requestCommandId,
				requestCommandMajorVersion,
				requestCommandMinorVersion,
				actualServerVersion,
				actualMajorVersion,
				actualMinorVersion
			});
		}

		public static LocalizedString ClientCulture_0x41B
		{
			get
			{
				return new LocalizedString("ClientCulture_0x41B", "Ex335AED", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotAddAttachmentToReadOnlyCollection
		{
			get
			{
				return new LocalizedString("CannotAddAttachmentToReadOnlyCollection", "ExBE25CD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExReplyToTooManyRecipients(int maxRecipientsAllowed)
		{
			return new LocalizedString("ExReplyToTooManyRecipients", "Ex874145", false, true, ServerStrings.ResourceManager, new object[]
			{
				maxRecipientsAllowed
			});
		}

		public static LocalizedString UserPhotoPreviewNotFound
		{
			get
			{
				return new LocalizedString("UserPhotoPreviewNotFound", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OscSyncLockNotFound(string provider, string userId, string networkId)
		{
			return new LocalizedString("OscSyncLockNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				provider,
				userId,
				networkId
			});
		}

		public static LocalizedString ErrorFailedToDeletePublicFolder(string identity, string exceptionMessage)
		{
			return new LocalizedString("ErrorFailedToDeletePublicFolder", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				identity,
				exceptionMessage
			});
		}

		public static LocalizedString PublicFolderMailboxesCannotBeMovedDuringMigration
		{
			get
			{
				return new LocalizedString("PublicFolderMailboxesCannotBeMovedDuringMigration", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcServerIgnoreClosedTeamMailbox(string mailboxGuid)
		{
			return new LocalizedString("RpcServerIgnoreClosedTeamMailbox", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString FailedToVerifyDRMPropsSignature(string userIdentity, int errorCode)
		{
			return new LocalizedString("FailedToVerifyDRMPropsSignature", "Ex93C211", false, true, ServerStrings.ResourceManager, new object[]
			{
				userIdentity,
				errorCode
			});
		}

		public static LocalizedString MigrationBatchDirectionOnboarding
		{
			get
			{
				return new LocalizedString("MigrationBatchDirectionOnboarding", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationSystemMailboxNotFound(string mailboxName)
		{
			return new LocalizedString("MigrationSystemMailboxNotFound", "Ex1F974A", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString InvalidSmtpAddress(string address)
		{
			return new LocalizedString("InvalidSmtpAddress", "Ex0FD11F", false, true, ServerStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString AsyncOperationTypeMigration
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeMigration", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvokingMethodNotSupported(string type, string method)
		{
			return new LocalizedString("InvokingMethodNotSupported", "ExEFC7D9", false, true, ServerStrings.ResourceManager, new object[]
			{
				type,
				method
			});
		}

		public static LocalizedString ClientCulture_0x40E
		{
			get
			{
				return new LocalizedString("ClientCulture_0x40E", "ExEB773A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OriginatingServer
		{
			get
			{
				return new LocalizedString("OriginatingServer", "Ex3D197B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EstimateStatePartiallySucceeded
		{
			get
			{
				return new LocalizedString("EstimateStatePartiallySucceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotImportDeletion
		{
			get
			{
				return new LocalizedString("CannotImportDeletion", "Ex965937", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusSynced
		{
			get
			{
				return new LocalizedString("MigrationUserStatusSynced", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotImportFolderChange
		{
			get
			{
				return new LocalizedString("CannotImportFolderChange", "Ex4EB3C1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusValidating
		{
			get
			{
				return new LocalizedString("MigrationUserStatusValidating", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetOrgContainer(Guid externalDirectoryOrgId)
		{
			return new LocalizedString("FailedToGetOrgContainer", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				externalDirectoryOrgId
			});
		}

		public static LocalizedString ValidationForServiceLocationResponseFailed(Uri url, string str)
		{
			return new LocalizedString("ValidationForServiceLocationResponseFailed", "Ex2DE658", false, true, ServerStrings.ResourceManager, new object[]
			{
				url,
				str
			});
		}

		public static LocalizedString MigrationObjectsCountStringContacts(string contacts)
		{
			return new LocalizedString("MigrationObjectsCountStringContacts", "Ex390820", false, true, ServerStrings.ResourceManager, new object[]
			{
				contacts
			});
		}

		public static LocalizedString ExConstraintNotSupportedForThisPropertyType
		{
			get
			{
				return new LocalizedString("ExConstraintNotSupportedForThisPropertyType", "Ex86612B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailSubjectCertExpiring
		{
			get
			{
				return new LocalizedString("NotificationEmailSubjectCertExpiring", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxSearchEwsFailedExceptionWithError(string error)
		{
			return new LocalizedString("MailboxSearchEwsFailedExceptionWithError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString MigrationUserStatusSummaryCompleted
		{
			get
			{
				return new LocalizedString("MigrationUserStatusSummaryCompleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerArabic
		{
			get
			{
				return new LocalizedString("SpellCheckerArabic", "ExAF5B2C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalLicensingDisabledForEnterprise
		{
			get
			{
				return new LocalizedString("InternalLicensingDisabledForEnterprise", "Ex175F62", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x240A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x240A", "Ex1B501D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ParsingErrorAt(int position)
		{
			return new LocalizedString("ParsingErrorAt", "ExFA340E", false, true, ServerStrings.ResourceManager, new object[]
			{
				position
			});
		}

		public static LocalizedString RPCOperationAbortedBecauseOfAnotherRPCThread
		{
			get
			{
				return new LocalizedString("RPCOperationAbortedBecauseOfAnotherRPCThread", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMdbGuid
		{
			get
			{
				return new LocalizedString("ExInvalidMdbGuid", "Ex095551", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerEnglishUnitedKingdom
		{
			get
			{
				return new LocalizedString("SpellCheckerEnglishUnitedKingdom", "Ex4591C9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFilterHierarchyIsTooDeep
		{
			get
			{
				return new LocalizedString("ExFilterHierarchyIsTooDeep", "ExFA73AD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSetMessageLockState
		{
			get
			{
				return new LocalizedString("MapiCannotSetMessageLockState", "ExB84C64", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotProtectMessageForNonSmtpSender
		{
			get
			{
				return new LocalizedString("CannotProtectMessageForNonSmtpSender", "ExC5B140", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSearchFolderIsAlreadyVisibleToOutlook
		{
			get
			{
				return new LocalizedString("ExSearchFolderIsAlreadyVisibleToOutlook", "Ex4B7F3A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExEntryIdFirst4Bytes
		{
			get
			{
				return new LocalizedString("ExEntryIdFirst4Bytes", "Ex511256", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CustomMessageLengthExceeded
		{
			get
			{
				return new LocalizedString("CustomMessageLengthExceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDictionaryDataCorruptedDuplicateKey(string key)
		{
			return new LocalizedString("ExDictionaryDataCorruptedDuplicateKey", "Ex5D6215", false, true, ServerStrings.ResourceManager, new object[]
			{
				key
			});
		}

		public static LocalizedString ExWrappedStreamFailure
		{
			get
			{
				return new LocalizedString("ExWrappedStreamFailure", "ExC79E85", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExTimeZoneValueWrongGmtFormat
		{
			get
			{
				return new LocalizedString("ErrorExTimeZoneValueWrongGmtFormat", "ExA84E98", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InternalParserError
		{
			get
			{
				return new LocalizedString("InternalParserError", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationNSPIMissingRequiredField(PropTag proptag)
		{
			return new LocalizedString("MigrationNSPIMissingRequiredField", "Ex097DA8", false, true, ServerStrings.ResourceManager, new object[]
			{
				proptag
			});
		}

		public static LocalizedString ExInvalidCount
		{
			get
			{
				return new LocalizedString("ExInvalidCount", "ExC8BEE8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTransientError(string messageDetail)
		{
			return new LocalizedString("MigrationTransientError", "ExA25383", false, true, ServerStrings.ResourceManager, new object[]
			{
				messageDetail
			});
		}

		public static LocalizedString ADUserNotFound
		{
			get
			{
				return new LocalizedString("ADUserNotFound", "ExFA5BEC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFolderSetPropsFailed(string exceptionMessage)
		{
			return new LocalizedString("ExFolderSetPropsFailed", "Ex30921A", false, true, ServerStrings.ResourceManager, new object[]
			{
				exceptionMessage
			});
		}

		public static LocalizedString InboxRuleFlagStatusNotFlagged
		{
			get
			{
				return new LocalizedString("InboxRuleFlagStatusNotFlagged", "Ex75D0E8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmReferralException(string referredServer)
		{
			return new LocalizedString("AmReferralException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				referredServer
			});
		}

		public static LocalizedString TaskServerTransientException(string errorMessage)
		{
			return new LocalizedString("TaskServerTransientException", "Ex2EAFDE", false, true, ServerStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ConversionMustLoadAllPropeties
		{
			get
			{
				return new LocalizedString("ConversionMustLoadAllPropeties", "ExEB2F6D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ThreeHours
		{
			get
			{
				return new LocalizedString("ThreeHours", "ExE63F5A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmOperationNotValidOnCurrentRole(string error)
		{
			return new LocalizedString("AmOperationNotValidOnCurrentRole", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString MapiCannotGetIDFromNames
		{
			get
			{
				return new LocalizedString("MapiCannotGetIDFromNames", "ExD4B997", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RangedParameter(string parameter, int max)
		{
			return new LocalizedString("RangedParameter", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				parameter,
				max
			});
		}

		public static LocalizedString ErrorSigntureTooLarge
		{
			get
			{
				return new LocalizedString("ErrorSigntureTooLarge", "Ex68357B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchFlagReportInitial
		{
			get
			{
				return new LocalizedString("MigrationBatchFlagReportInitial", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeProposalEndTimeBeforeStartTime
		{
			get
			{
				return new LocalizedString("ErrorTimeProposalEndTimeBeforeStartTime", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSetMessageFlagStatus
		{
			get
			{
				return new LocalizedString("CannotSetMessageFlagStatus", "ExC5EDAE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFlagsReport
		{
			get
			{
				return new LocalizedString("MigrationFlagsReport", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString IsNotMailboxOwner(object userSid, object mailboxOwnerSid)
		{
			return new LocalizedString("IsNotMailboxOwner", "ExBB3EF9", false, true, ServerStrings.ResourceManager, new object[]
			{
				userSid,
				mailboxOwnerSid
			});
		}

		public static LocalizedString JunkEmailBlockedListXsoDuplicateException(string value)
		{
			return new LocalizedString("JunkEmailBlockedListXsoDuplicateException", "ExBDDD0D", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MigrationStepProvisioning
		{
			get
			{
				return new LocalizedString("MigrationStepProvisioning", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FirstFourDayWeek
		{
			get
			{
				return new LocalizedString("FirstFourDayWeek", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotModifyRecipients
		{
			get
			{
				return new LocalizedString("MapiCannotModifyRecipients", "ExA3B4C2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionCorruptSummaryTnef
		{
			get
			{
				return new LocalizedString("ConversionCorruptSummaryTnef", "Ex1DB7D1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExTooManyDuplicateDataColumns(int maxDuplicateDataColumns)
		{
			return new LocalizedString("ExTooManyDuplicateDataColumns", "Ex0117E9", false, true, ServerStrings.ResourceManager, new object[]
			{
				maxDuplicateDataColumns
			});
		}

		public static LocalizedString ClientCulture_0x2409
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2409", "Ex49949E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAlreadyConnected
		{
			get
			{
				return new LocalizedString("ExAlreadyConnected", "Ex9A2928", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationErrorString(string emailAddress, string localizedMessage)
		{
			return new LocalizedString("MigrationErrorString", "ExFFEE96", false, true, ServerStrings.ResourceManager, new object[]
			{
				emailAddress,
				localizedMessage
			});
		}

		public static LocalizedString ExReportMessageCorruptedDueToWrongItemAttachmentType
		{
			get
			{
				return new LocalizedString("ExReportMessageCorruptedDueToWrongItemAttachmentType", "Ex6849D8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationGroupMembersNotAvailable(string groupSmtpAddress)
		{
			return new LocalizedString("MigrationGroupMembersNotAvailable", "ExE2FEE0", false, true, ServerStrings.ResourceManager, new object[]
			{
				groupSmtpAddress
			});
		}

		public static LocalizedString RpcServerIgnoreUnlinkedTeamMailbox(string mailboxGuid)
		{
			return new LocalizedString("RpcServerIgnoreUnlinkedTeamMailbox", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString ClientCulture_0x500A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x500A", "ExFDB090", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTypeNone
		{
			get
			{
				return new LocalizedString("MigrationTypeNone", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmServerException(string errorMessage)
		{
			return new LocalizedString("AmServerException", "Ex92D94B", false, true, ServerStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString CannotImportReadStateChange
		{
			get
			{
				return new LocalizedString("CannotImportReadStateChange", "Ex8FC2C6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConversationNotFound(string conversationId, string conversationFamilyId)
		{
			return new LocalizedString("ExConversationNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				conversationId,
				conversationFamilyId
			});
		}

		public static LocalizedString ErrorNotSupportedLanguageWithInstalledLanguagePack(string lang)
		{
			return new LocalizedString("ErrorNotSupportedLanguageWithInstalledLanguagePack", "Ex28B323", false, true, ServerStrings.ResourceManager, new object[]
			{
				lang
			});
		}

		public static LocalizedString MapiCannotGetAttachmentTable
		{
			get
			{
				return new LocalizedString("MapiCannotGetAttachmentTable", "Ex76BFC6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInPlaceHoldIdentityChanged(string name)
		{
			return new LocalizedString("ErrorInPlaceHoldIdentityChanged", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MapiCannotOpenAttachment
		{
			get
			{
				return new LocalizedString("MapiCannotOpenAttachment", "Ex18E168", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSuffixTextFilterNotSupported
		{
			get
			{
				return new LocalizedString("ExSuffixTextFilterNotSupported", "Ex27263F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSeparatorNotFoundOnCompoundValue
		{
			get
			{
				return new LocalizedString("ExSeparatorNotFoundOnCompoundValue", "Ex07AA84", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusCorrupted
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusCorrupted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusSyncing
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusSyncing", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSharingDataException(string name, string value)
		{
			return new LocalizedString("InvalidSharingDataException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name,
				value
			});
		}

		public static LocalizedString ClientCulture_0x415
		{
			get
			{
				return new LocalizedString("ClientCulture_0x415", "Ex664B26", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationOrganizationNotFound(string mailboxName)
		{
			return new LocalizedString("MigrationOrganizationNotFound", "ExA992C3", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString AmDbMountNotAllowedDueToAcllErrorException(string errMessage, long numLogsLost)
		{
			return new LocalizedString("AmDbMountNotAllowedDueToAcllErrorException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errMessage,
				numLogsLost
			});
		}

		public static LocalizedString ClientCulture_0x2C01
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2C01", "Ex25C35D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderSyncFolderHierarchyFailedAfterMultipleAttempts(int attempts, string innerMessage)
		{
			return new LocalizedString("PublicFolderSyncFolderHierarchyFailedAfterMultipleAttempts", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				attempts,
				innerMessage
			});
		}

		public static LocalizedString CannotAccessRemoteMailbox
		{
			get
			{
				return new LocalizedString("CannotAccessRemoteMailbox", "Ex0052F2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotFindRow
		{
			get
			{
				return new LocalizedString("MapiCannotFindRow", "ExBD9DCF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoInternalEwsAvailableException(string mailbox)
		{
			return new LocalizedString("NoInternalEwsAvailableException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString AmInvalidConfiguration(string error)
		{
			return new LocalizedString("AmInvalidConfiguration", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ThirtyMinutes
		{
			get
			{
				return new LocalizedString("ThirtyMinutes", "Ex1AC83C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSeekRow
		{
			get
			{
				return new LocalizedString("MapiCannotSeekRow", "ExD55968", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusFailed
		{
			get
			{
				return new LocalizedString("MigrationUserStatusFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExceptionObjectHasBeenDeleted
		{
			get
			{
				return new LocalizedString("ExceptionObjectHasBeenDeleted", "ExBF3124", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchFlagDisallowExistingUsers
		{
			get
			{
				return new LocalizedString("MigrationBatchFlagDisallowExistingUsers", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x464
		{
			get
			{
				return new LocalizedString("ClientCulture_0x464", "Ex737240", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidLocalDirectorySecurityIdentifier(string sid)
		{
			return new LocalizedString("InvalidLocalDirectorySecurityIdentifier", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				sid
			});
		}

		public static LocalizedString CalendarOriginatorIdCorrupt(string calendarOriginatorId)
		{
			return new LocalizedString("CalendarOriginatorIdCorrupt", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				calendarOriginatorId
			});
		}

		public static LocalizedString UnsupportedPropertyRestriction
		{
			get
			{
				return new LocalizedString("UnsupportedPropertyRestriction", "ExFDBD2B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AppointmentActionNotSupported(string action)
		{
			return new LocalizedString("AppointmentActionNotSupported", "Ex3BD3B7", false, true, ServerStrings.ResourceManager, new object[]
			{
				action
			});
		}

		public static LocalizedString TooManyActiveManagerClientRPCs(int maximum)
		{
			return new LocalizedString("TooManyActiveManagerClientRPCs", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				maximum
			});
		}

		public static LocalizedString RightsNotAllowedByPolicy(string storeObjectType, string folderName)
		{
			return new LocalizedString("RightsNotAllowedByPolicy", "ExAD7A50", false, true, ServerStrings.ResourceManager, new object[]
			{
				storeObjectType,
				folderName
			});
		}

		public static LocalizedString ServerLocatorClientWCFCallTimeout
		{
			get
			{
				return new LocalizedString("ServerLocatorClientWCFCallTimeout", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidServiceLocationResponse
		{
			get
			{
				return new LocalizedString("InvalidServiceLocationResponse", "Ex54D399", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotDeleteProperties
		{
			get
			{
				return new LocalizedString("MapiCannotDeleteProperties", "Ex108358", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NeedFolderIdForPublicFolder
		{
			get
			{
				return new LocalizedString("NeedFolderIdForPublicFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmServerNotFoundToVerifyRpcVersion(string serverName)
		{
			return new LocalizedString("AmServerNotFoundToVerifyRpcVersion", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				serverName
			});
		}

		public static LocalizedString TaskOperationFailedException(string errMessage)
		{
			return new LocalizedString("TaskOperationFailedException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString ClientCulture_0x100C
		{
			get
			{
				return new LocalizedString("ClientCulture_0x100C", "Ex91D09B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionMaxRecipientExceeded(int maxrecipients)
		{
			return new LocalizedString("ConversionMaxRecipientExceeded", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				maxrecipients
			});
		}

		public static LocalizedString ManagedByRemoteExchangeOrganization
		{
			get
			{
				return new LocalizedString("ManagedByRemoteExchangeOrganization", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AutoDRequestFailed
		{
			get
			{
				return new LocalizedString("AutoDRequestFailed", "ExAFA011", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoSharingHandlerFoundException(string recipient)
		{
			return new LocalizedString("NoSharingHandlerFoundException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString DumpsterFolderNotFound
		{
			get
			{
				return new LocalizedString("DumpsterFolderNotFound", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFolderNotFoundInClientState
		{
			get
			{
				return new LocalizedString("ExFolderNotFoundInClientState", "ExE84B6E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalUserNotFound(string sid)
		{
			return new LocalizedString("ExternalUserNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				sid
			});
		}

		public static LocalizedString ExTenantAccessBlocked(string organizationId)
		{
			return new LocalizedString("ExTenantAccessBlocked", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				organizationId
			});
		}

		public static LocalizedString ImportResultContainedFailure
		{
			get
			{
				return new LocalizedString("ImportResultContainedFailure", "Ex08D54F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x813
		{
			get
			{
				return new LocalizedString("ClientCulture_0x813", "ExAF5FAA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotCreateMeetingResponse
		{
			get
			{
				return new LocalizedString("ExCannotCreateMeetingResponse", "ExC172FE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EightHours
		{
			get
			{
				return new LocalizedString("EightHours", "Ex4ECD5D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExUnresolvedRecipient(string recipientDisplayName)
		{
			return new LocalizedString("ExUnresolvedRecipient", "Ex72B03F", false, true, ServerStrings.ResourceManager, new object[]
			{
				recipientDisplayName
			});
		}

		public static LocalizedString ExInvalidAttachmentId(string idbytes)
		{
			return new LocalizedString("ExInvalidAttachmentId", "Ex64971C", false, true, ServerStrings.ResourceManager, new object[]
			{
				idbytes
			});
		}

		public static LocalizedString OperationResultFailed
		{
			get
			{
				return new LocalizedString("OperationResultFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorWorkingHoursEndTimeSmaller
		{
			get
			{
				return new LocalizedString("ErrorWorkingHoursEndTimeSmaller", "Ex212A1E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorFolderAlreadyExists(string name)
		{
			return new LocalizedString("ErrorFolderAlreadyExists", "Ex4C0E4F", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString RoutingTypeRequired
		{
			get
			{
				return new LocalizedString("RoutingTypeRequired", "Ex0879F8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAddressFormat(string routingType, string address)
		{
			return new LocalizedString("InvalidAddressFormat", "Ex46618E", false, true, ServerStrings.ResourceManager, new object[]
			{
				routingType,
				address
			});
		}

		public static LocalizedString ExOccurrenceNotPresent(object occId)
		{
			return new LocalizedString("ExOccurrenceNotPresent", "Ex593F3C", false, true, ServerStrings.ResourceManager, new object[]
			{
				occId
			});
		}

		public static LocalizedString FolderRuleCannotSaveItem
		{
			get
			{
				return new LocalizedString("FolderRuleCannotSaveItem", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateRemoving
		{
			get
			{
				return new LocalizedString("RequestStateRemoving", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStateFailed
		{
			get
			{
				return new LocalizedString("MigrationStateFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusCompletionFailed
		{
			get
			{
				return new LocalizedString("MigrationUserStatusCompletionFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxSearchEwsEmptyResponse
		{
			get
			{
				return new LocalizedString("MailboxSearchEwsEmptyResponse", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExComparisonOperatorNotSupported(object relOp)
		{
			return new LocalizedString("ExComparisonOperatorNotSupported", "ExAF2A42", false, true, ServerStrings.ResourceManager, new object[]
			{
				relOp
			});
		}

		public static LocalizedString ClientCulture_0x140A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x140A", "Ex2EA3A8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusRemoving
		{
			get
			{
				return new LocalizedString("MigrationUserStatusRemoving", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFolderCorruptedItems
		{
			get
			{
				return new LocalizedString("MigrationFolderCorruptedItems", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x418
		{
			get
			{
				return new LocalizedString("ClientCulture_0x418", "Ex22C242", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerPortuguesePortugal
		{
			get
			{
				return new LocalizedString("SpellCheckerPortuguesePortugal", "Ex6F826C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageReactivatingText
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageReactivatingText", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchLogFileCreateException
		{
			get
			{
				return new LocalizedString("SearchLogFileCreateException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotGetDeletedItem
		{
			get
			{
				return new LocalizedString("ExCannotGetDeletedItem", "Ex025082", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxSearchNameTooLong
		{
			get
			{
				return new LocalizedString("MailboxSearchNameTooLong", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExternalLicensingDisabledForTenant(Uri uri, OrganizationId orgId)
		{
			return new LocalizedString("ExternalLicensingDisabledForTenant", "Ex6AA711", false, true, ServerStrings.ResourceManager, new object[]
			{
				uri,
				orgId
			});
		}

		public static LocalizedString ClientCulture_0x41F
		{
			get
			{
				return new LocalizedString("ClientCulture_0x41F", "Ex3A6328", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x4409
		{
			get
			{
				return new LocalizedString("ClientCulture_0x4409", "Ex045C19", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSubmissionQuotaExceeded
		{
			get
			{
				return new LocalizedString("ExSubmissionQuotaExceeded", "Ex6D2B2E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCorruptMessageCorrelationBlob
		{
			get
			{
				return new LocalizedString("ExCorruptMessageCorrelationBlob", "Ex4AB049", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFolderDrumTesting
		{
			get
			{
				return new LocalizedString("MigrationFolderDrumTesting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CantParseParticipant(string inputString)
		{
			return new LocalizedString("CantParseParticipant", "ExEEB2A9", false, true, ServerStrings.ResourceManager, new object[]
			{
				inputString
			});
		}

		public static LocalizedString ErrorFolderSave(string id, string details)
		{
			return new LocalizedString("ErrorFolderSave", "Ex2CCC6C", false, true, ServerStrings.ResourceManager, new object[]
			{
				id,
				details
			});
		}

		public static LocalizedString ExCorruptFolderWebViewInfo
		{
			get
			{
				return new LocalizedString("ExCorruptFolderWebViewInfo", "Ex799C65", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchFlagDisableOnCopy
		{
			get
			{
				return new LocalizedString("MigrationBatchFlagDisableOnCopy", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExMailboxAccessDenied(string owner, string accessingUser)
		{
			return new LocalizedString("ExMailboxAccessDenied", "Ex711EFB", false, true, ServerStrings.ResourceManager, new object[]
			{
				owner,
				accessingUser
			});
		}

		public static LocalizedString ICSSynchronizationFailed
		{
			get
			{
				return new LocalizedString("ICSSynchronizationFailed", "Ex69CDC7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidTypeInBlob(object blobType)
		{
			return new LocalizedString("ExInvalidTypeInBlob", "ExACB26B", false, true, ServerStrings.ResourceManager, new object[]
			{
				blobType
			});
		}

		public static LocalizedString OneHours
		{
			get
			{
				return new LocalizedString("OneHours", "ExEFAC6F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PromoteVeventFailure(string uid)
		{
			return new LocalizedString("PromoteVeventFailure", "ExBDC768", false, true, ServerStrings.ResourceManager, new object[]
			{
				uid
			});
		}

		public static LocalizedString InvalidBodyFormat
		{
			get
			{
				return new LocalizedString("InvalidBodyFormat", "Ex9E084B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderRuleErrorRecord(string recipient, string stage, string exceptionType, string exceptionMessage)
		{
			return new LocalizedString("FolderRuleErrorRecord", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				recipient,
				stage,
				exceptionType,
				exceptionMessage
			});
		}

		public static LocalizedString PeopleQuickContactsAttributionDisplayName
		{
			get
			{
				return new LocalizedString("PeopleQuickContactsAttributionDisplayName", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TwoHours
		{
			get
			{
				return new LocalizedString("TwoHours", "Ex72977C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExPropertyDefinitionInMoreThanOnePropertyProfile
		{
			get
			{
				return new LocalizedString("ExPropertyDefinitionInMoreThanOnePropertyProfile", "Ex01B729", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusMembershipAndMaintenanceSyncFailure
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusMembershipAndMaintenanceSyncFailure", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0xC0C
		{
			get
			{
				return new LocalizedString("ClientCulture_0xC0C", "Ex5441E2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExUnableToCopyAttachments
		{
			get
			{
				return new LocalizedString("ExUnableToCopyAttachments", "ExB42355", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotUpdateResponses
		{
			get
			{
				return new LocalizedString("ExCannotUpdateResponses", "Ex9D4D4C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationItemHasNoBody
		{
			get
			{
				return new LocalizedString("ConversationItemHasNoBody", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DelegateCollectionInvalidAfterSave
		{
			get
			{
				return new LocalizedString("DelegateCollectionInvalidAfterSave", "Ex6D9AC2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusDocumentAndMaintenanceSyncFailure
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusDocumentAndMaintenanceSyncFailure", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAttachmentType
		{
			get
			{
				return new LocalizedString("InvalidAttachmentType", "Ex0ABEE6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotMarkTaskCompletedWhenSuppressCreateOneOff
		{
			get
			{
				return new LocalizedString("ExCannotMarkTaskCompletedWhenSuppressCreateOneOff", "Ex4A8040", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetPropertyList
		{
			get
			{
				return new LocalizedString("CannotGetPropertyList", "Ex4CCA0F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidConfigurationXml
		{
			get
			{
				return new LocalizedString("ErrorInvalidConfigurationXml", "ExBF50D3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MalformedTimeZoneWorkingHours(string mailbox, string exceptionInfo)
		{
			return new LocalizedString("MalformedTimeZoneWorkingHours", "Ex90FBB2", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox,
				exceptionInfo
			});
		}

		public static LocalizedString InvalidBase64String
		{
			get
			{
				return new LocalizedString("InvalidBase64String", "ExFE7FAA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateFailed
		{
			get
			{
				return new LocalizedString("RequestStateFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidFileTimeInRecurrenceBlob(int fileTime)
		{
			return new LocalizedString("ExInvalidFileTimeInRecurrenceBlob", "Ex70AF44", false, true, ServerStrings.ResourceManager, new object[]
			{
				fileTime
			});
		}

		public static LocalizedString InboxRuleImportanceNormal
		{
			get
			{
				return new LocalizedString("InboxRuleImportanceNormal", "Ex11ADED", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationLocalhostNotFound
		{
			get
			{
				return new LocalizedString("MigrationLocalhostNotFound", "Ex822317", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x1401
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1401", "Ex9DAD09", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidInPlaceHoldIdentity(string name)
		{
			return new LocalizedString("ErrorInvalidInPlaceHoldIdentity", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExBodyConversionNotSupportedType(string format)
		{
			return new LocalizedString("ExBodyConversionNotSupportedType", "Ex8314F4", false, true, ServerStrings.ResourceManager, new object[]
			{
				format
			});
		}

		public static LocalizedString TeamMailboxSyncStatusSucceeded
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusSucceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationErrorAttachmentCorrupted
		{
			get
			{
				return new LocalizedString("MigrationErrorAttachmentCorrupted", "Ex43FBE7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OleConversionResultFailed
		{
			get
			{
				return new LocalizedString("OleConversionResultFailed", "Ex79242F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusSummaryFailed
		{
			get
			{
				return new LocalizedString("MigrationUserStatusSummaryFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateCanceled
		{
			get
			{
				return new LocalizedString("RequestStateCanceled", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ModifyRuleInStore
		{
			get
			{
				return new LocalizedString("ModifyRuleInStore", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExItemDeletedInRace
		{
			get
			{
				return new LocalizedString("ExItemDeletedInRace", "Ex52D036", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x340A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x340A", "ExDE1E5F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WeatherUnitFahrenheit
		{
			get
			{
				return new LocalizedString("WeatherUnitFahrenheit", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageNotRightsProtected
		{
			get
			{
				return new LocalizedString("MessageNotRightsProtected", "ExA28CD3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SaveConfigurationItem(string mailbox, string exceptionInfo)
		{
			return new LocalizedString("SaveConfigurationItem", "Ex0280B6", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox,
				exceptionInfo
			});
		}

		public static LocalizedString ConversionMaliciousContent
		{
			get
			{
				return new LocalizedString("ConversionMaliciousContent", "Ex9CCF09", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcServerUnhandledException(string error)
		{
			return new LocalizedString("RpcServerUnhandledException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString TaskInvalidFlagStatus(int flagStatus, int taskStatus)
		{
			return new LocalizedString("TaskInvalidFlagStatus", "Ex8839D8", false, true, ServerStrings.ResourceManager, new object[]
			{
				flagStatus,
				taskStatus
			});
		}

		public static LocalizedString RpcServerParameterSerializationError(string error)
		{
			return new LocalizedString("RpcServerParameterSerializationError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString NoTemplateMessage
		{
			get
			{
				return new LocalizedString("NoTemplateMessage", "ExA6872D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExTimeInExtendedInfoNotSameAsExceptionInfo(string timeType, object extendedInfoTime, object exceptionInfoTime)
		{
			return new LocalizedString("ExTimeInExtendedInfoNotSameAsExceptionInfo", "Ex2F385A", false, true, ServerStrings.ResourceManager, new object[]
			{
				timeType,
				extendedInfoTime,
				exceptionInfoTime
			});
		}

		public static LocalizedString FailedToGetRmsTemplates(string tenantId)
		{
			return new LocalizedString("FailedToGetRmsTemplates", "Ex36B628", false, true, ServerStrings.ResourceManager, new object[]
			{
				tenantId
			});
		}

		public static LocalizedString FolderRuleStageLoading
		{
			get
			{
				return new LocalizedString("FolderRuleStageLoading", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidFolderType(int type)
		{
			return new LocalizedString("ExInvalidFolderType", "Ex0C8624", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString LimitedDetails
		{
			get
			{
				return new LocalizedString("LimitedDetails", "Ex1F2397", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AppendOOFHistoryEntry
		{
			get
			{
				return new LocalizedString("AppendOOFHistoryEntry", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExItemNotFoundInConversation(object itemId, object conversationId)
		{
			return new LocalizedString("ExItemNotFoundInConversation", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				itemId,
				conversationId
			});
		}

		public static LocalizedString ExInvalidMinutePeriod(int period)
		{
			return new LocalizedString("ExInvalidMinutePeriod", "Ex51B4FA", false, true, ServerStrings.ResourceManager, new object[]
			{
				period
			});
		}

		public static LocalizedString ExStoreSessionDisconnected
		{
			get
			{
				return new LocalizedString("ExStoreSessionDisconnected", "Ex8978DF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionMaxRecipientSizeExceeded(int maxsize, string truncatedPart)
		{
			return new LocalizedString("ConversionMaxRecipientSizeExceeded", "ExE93799", false, true, ServerStrings.ResourceManager, new object[]
			{
				maxsize,
				truncatedPart
			});
		}

		public static LocalizedString ErrorInvalidStatisticsStartIndex(string name)
		{
			return new LocalizedString("ErrorInvalidStatisticsStartIndex", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString NotificationEmailSubjectCertExpired
		{
			get
			{
				return new LocalizedString("NotificationEmailSubjectCertExpired", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserAdminTypeDCAdmin
		{
			get
			{
				return new LocalizedString("MigrationUserAdminTypeDCAdmin", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerSpanish
		{
			get
			{
				return new LocalizedString("SpellCheckerSpanish", "ExDA06B6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoNotFound
		{
			get
			{
				return new LocalizedString("UserPhotoNotFound", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationInvalidStatus(string statusType, string status)
		{
			return new LocalizedString("MigrationInvalidStatus", "Ex5AC1ED", false, true, ServerStrings.ResourceManager, new object[]
			{
				statusType,
				status
			});
		}

		public static LocalizedString QueryUsageRightsNoPrelicenseResponse(string uri)
		{
			return new LocalizedString("QueryUsageRightsNoPrelicenseResponse", "ExE9FCED", false, true, ServerStrings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString ClientCulture_0x1C01
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1C01", "ExCB8B3D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchFlagAutoStop
		{
			get
			{
				return new LocalizedString("MigrationBatchFlagAutoStop", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RemoteArchiveOffline
		{
			get
			{
				return new LocalizedString("RemoteArchiveOffline", "Ex21F3BA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotOpenLocalFreeBusy
		{
			get
			{
				return new LocalizedString("CannotOpenLocalFreeBusy", "ExBEB5DD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExBatchBuilderNeedsPropertyToConvertRT(PropertyDefinition propertyDefinition, string currentRoutingType, string destinationRoutingType, string participant)
		{
			return new LocalizedString("ExBatchBuilderNeedsPropertyToConvertRT", "Ex95821B", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyDefinition,
				currentRoutingType,
				destinationRoutingType,
				participant
			});
		}

		public static LocalizedString CannotFindExchangePrincipal
		{
			get
			{
				return new LocalizedString("CannotFindExchangePrincipal", "Ex47964E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonUniqueRecipientByExternalIdError(string externalDirectoryObjectId)
		{
			return new LocalizedString("NonUniqueRecipientByExternalIdError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				externalDirectoryObjectId
			});
		}

		public static LocalizedString MigrationStageValidation
		{
			get
			{
				return new LocalizedString("MigrationStageValidation", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalNotifTypeReminder
		{
			get
			{
				return new LocalizedString("CalNotifTypeReminder", "ExA22CD2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFailedToGetUnsearchableItems
		{
			get
			{
				return new LocalizedString("ExFailedToGetUnsearchableItems", "Ex8CCF32", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Monday
		{
			get
			{
				return new LocalizedString("Monday", "ExF95A69", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeUnknown
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeUnknown", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFolderSyncMigration
		{
			get
			{
				return new LocalizedString("MigrationFolderSyncMigration", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleFlagStatusComplete
		{
			get
			{
				return new LocalizedString("InboxRuleFlagStatusComplete", "Ex55D3AD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailSubjectMoveMailbox
		{
			get
			{
				return new LocalizedString("NotificationEmailSubjectMoveMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageRpmsgAttachmentIncorrectType
		{
			get
			{
				return new LocalizedString("MessageRpmsgAttachmentIncorrectType", "ExC052B6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MultipleProvisioningMailboxes(string mailboxId)
		{
			return new LocalizedString("MultipleProvisioningMailboxes", "ExBB7B8C", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ExMailboxMustBeAccessedAsOwner(string owner, string accessingUser)
		{
			return new LocalizedString("ExMailboxMustBeAccessedAsOwner", "Ex3A00A8", false, true, ServerStrings.ResourceManager, new object[]
			{
				owner,
				accessingUser
			});
		}

		public static LocalizedString CannotCreateOscFolderBecauseOfConflict(string provider)
		{
			return new LocalizedString("CannotCreateOscFolderBecauseOfConflict", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				provider
			});
		}

		public static LocalizedString FullDetails
		{
			get
			{
				return new LocalizedString("FullDetails", "Ex469BDC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailObjectDisposedException
		{
			get
			{
				return new LocalizedString("JunkEmailObjectDisposedException", "ExAFF31F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadLocalServer
		{
			get
			{
				return new LocalizedString("FailedToReadLocalServer", "Ex5685A2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetCurrentRow
		{
			get
			{
				return new LocalizedString("MapiCannotGetCurrentRow", "Ex440EB2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationInvalidPassword
		{
			get
			{
				return new LocalizedString("MigrationInvalidPassword", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotDeletePropertiesOnOccurrences
		{
			get
			{
				return new LocalizedString("ExCannotDeletePropertiesOnOccurrences", "Ex48E90E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LicenseUriInvalidForTenant(string uri, string tenantId)
		{
			return new LocalizedString("LicenseUriInvalidForTenant", "ExF3BEA7", false, true, ServerStrings.ResourceManager, new object[]
			{
				uri,
				tenantId
			});
		}

		public static LocalizedString ExConfigNameInvalid(string name)
		{
			return new LocalizedString("ExConfigNameInvalid", "Ex73E46E", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString EstimateStateFailed
		{
			get
			{
				return new LocalizedString("EstimateStateFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMultivaluePropertyFilter(string filterType)
		{
			return new LocalizedString("ExInvalidMultivaluePropertyFilter", "ExE1D1F9", false, true, ServerStrings.ResourceManager, new object[]
			{
				filterType
			});
		}

		public static LocalizedString ExInvalidResponseType(object responseType)
		{
			return new LocalizedString("ExInvalidResponseType", "Ex522748", false, true, ServerStrings.ResourceManager, new object[]
			{
				responseType
			});
		}

		public static LocalizedString RequestStateCompleting
		{
			get
			{
				return new LocalizedString("RequestStateCompleting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x41E
		{
			get
			{
				return new LocalizedString("ClientCulture_0x41E", "Ex88BB62", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSharingRecipientsException
		{
			get
			{
				return new LocalizedString("InvalidSharingRecipientsException", "ExE7475E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotOpenFolder
		{
			get
			{
				return new LocalizedString("MapiCannotOpenFolder", "Ex61964B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x180C
		{
			get
			{
				return new LocalizedString("ClientCulture_0x180C", "Ex68FB65", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTeamMailboxSendingNotifications(string ex)
		{
			return new LocalizedString("ErrorTeamMailboxSendingNotifications", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				ex
			});
		}

		public static LocalizedString AnonymousPublishingUrlValidationException(string url)
		{
			return new LocalizedString("AnonymousPublishingUrlValidationException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString MapiCannotModifyPropertyTable(string tableName)
		{
			return new LocalizedString("MapiCannotModifyPropertyTable", "Ex4C513F", false, true, ServerStrings.ResourceManager, new object[]
			{
				tableName
			});
		}

		public static LocalizedString ADOperationAbortedBecauseOfAnotherADThread
		{
			get
			{
				return new LocalizedString("ADOperationAbortedBecauseOfAnotherADThread", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Ex12NotSupportedDeleteItemFlags(int flags)
		{
			return new LocalizedString("Ex12NotSupportedDeleteItemFlags", "Ex4BC6FC", false, true, ServerStrings.ResourceManager, new object[]
			{
				flags
			});
		}

		public static LocalizedString UpdateOOFHistoryOperation
		{
			get
			{
				return new LocalizedString("UpdateOOFHistoryOperation", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisplayNameRequiredForRoutingType(string routingType)
		{
			return new LocalizedString("DisplayNameRequiredForRoutingType", "Ex1AFB71", false, true, ServerStrings.ResourceManager, new object[]
			{
				routingType
			});
		}

		public static LocalizedString ExAttachmentAlreadyOpen
		{
			get
			{
				return new LocalizedString("ExAttachmentAlreadyOpen", "ExE5CFF4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserPhotoImageTooSmall(int min)
		{
			return new LocalizedString("UserPhotoImageTooSmall", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				min
			});
		}

		public static LocalizedString NotificationEmailBodyImportPSTCompleted
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyImportPSTCompleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidAggregate
		{
			get
			{
				return new LocalizedString("ExInvalidAggregate", "Ex180D68", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailBodyImportPSTFailed
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyImportPSTFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCantAccessOccurrenceFromSingle
		{
			get
			{
				return new LocalizedString("ExCantAccessOccurrenceFromSingle", "Ex77AC41", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProgramError(string reason)
		{
			return new LocalizedString("ProgramError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString ErrorInvalidQuery(string name, string errorMessage)
		{
			return new LocalizedString("ErrorInvalidQuery", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name,
				errorMessage
			});
		}

		public static LocalizedString NotificationEmailBodyExportPSTCompleted
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyExportPSTCompleted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x801
		{
			get
			{
				return new LocalizedString("ClientCulture_0x801", "Ex42ADC6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x401
		{
			get
			{
				return new LocalizedString("ClientCulture_0x401", "Ex345771", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalNotifTypeNewUpdate
		{
			get
			{
				return new LocalizedString("CalNotifTypeNewUpdate", "Ex071F3E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoExternalEwsAvailableException
		{
			get
			{
				return new LocalizedString("NoExternalEwsAvailableException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSharePublicFolder
		{
			get
			{
				return new LocalizedString("CannotSharePublicFolder", "ExE3612F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTypeExchangeRemoteMove
		{
			get
			{
				return new LocalizedString("MigrationTypeExchangeRemoteMove", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusCompletedWithWarning
		{
			get
			{
				return new LocalizedString("MigrationUserStatusCompletedWithWarning", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EulNotFoundInContainerItem(string filename)
		{
			return new LocalizedString("EulNotFoundInContainerItem", "Ex38EC88", false, true, ServerStrings.ResourceManager, new object[]
			{
				filename
			});
		}

		public static LocalizedString FailedToAquirePublishLicense(string sender)
		{
			return new LocalizedString("FailedToAquirePublishLicense", "ExCAE64F", false, true, ServerStrings.ResourceManager, new object[]
			{
				sender
			});
		}

		public static LocalizedString FailedToParseUseLicense
		{
			get
			{
				return new LocalizedString("FailedToParseUseLicense", "ExFC15EE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStateDisabled
		{
			get
			{
				return new LocalizedString("MigrationStateDisabled", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchSupportedActionComplete
		{
			get
			{
				return new LocalizedString("MigrationBatchSupportedActionComplete", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFolderId(string id)
		{
			return new LocalizedString("InvalidFolderId", "Ex638F1C", false, true, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString MapiCannotOpenEmbeddedMessage
		{
			get
			{
				return new LocalizedString("MapiCannotOpenEmbeddedMessage", "Ex1F388A", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleImportanceLow
		{
			get
			{
				return new LocalizedString("InboxRuleImportanceLow", "ExBEB271", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoMapiPDLs
		{
			get
			{
				return new LocalizedString("NoMapiPDLs", "ExEABE18", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RmExceptionGenericMessage
		{
			get
			{
				return new LocalizedString("RmExceptionGenericMessage", "ExAEE0C0", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PositiveParameter(string parameter)
		{
			return new LocalizedString("PositiveParameter", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString RecipientAddressNotSpecifiedForExternalLicensing(Uri uri, string tenantId)
		{
			return new LocalizedString("RecipientAddressNotSpecifiedForExternalLicensing", "Ex41216E", false, true, ServerStrings.ResourceManager, new object[]
			{
				uri,
				tenantId
			});
		}

		public static LocalizedString NotRead
		{
			get
			{
				return new LocalizedString("NotRead", "Ex1070BB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcServerRequestSuccess(string server)
		{
			return new LocalizedString("RpcServerRequestSuccess", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString AdUserNotFoundException(string errMessage)
		{
			return new LocalizedString("AdUserNotFoundException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				errMessage
			});
		}

		public static LocalizedString ClientCulture_0x80C
		{
			get
			{
				return new LocalizedString("ClientCulture_0x80C", "Ex3EB34F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idUnableToAddDefaultCalendarToDefaultCalendarGroup
		{
			get
			{
				return new LocalizedString("idUnableToAddDefaultCalendarToDefaultCalendarGroup", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x3801
		{
			get
			{
				return new LocalizedString("ClientCulture_0x3801", "Ex2CE6DA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetAllPerUserLongTermIds
		{
			get
			{
				return new LocalizedString("MapiCannotGetAllPerUserLongTermIds", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageSiteMailboxEmailAddress
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageSiteMailboxEmailAddress", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalNotifTypeDeletedUpdate
		{
			get
			{
				return new LocalizedString("CalNotifTypeDeletedUpdate", "ExAC6027", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCreateSearchFoldersInPublicStore
		{
			get
			{
				return new LocalizedString("CannotCreateSearchFoldersInPublicStore", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDictionaryDataCorruptedNoField
		{
			get
			{
				return new LocalizedString("ExDictionaryDataCorruptedNoField", "ExAA8571", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idNotSupportedWithServerVersionException(string mailboxId, int mailboxVersion, int serverVersion)
		{
			return new LocalizedString("idNotSupportedWithServerVersionException", "ExA79EBB", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxId,
				mailboxVersion,
				serverVersion
			});
		}

		public static LocalizedString ExceptionFolderIsRootFolder
		{
			get
			{
				return new LocalizedString("ExceptionFolderIsRootFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateRangeOneDay
		{
			get
			{
				return new LocalizedString("DateRangeOneDay", "ExEEDB71", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcServerStorageError(string mailboxGuid, string error)
		{
			return new LocalizedString("RpcServerStorageError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				error
			});
		}

		public static LocalizedString ClientCulture_0x412
		{
			get
			{
				return new LocalizedString("ClientCulture_0x412", "ExBA635D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AppointmentTombstoneCorrupt
		{
			get
			{
				return new LocalizedString("AppointmentTombstoneCorrupt", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString QueryUsageRightsPrelicenseResponseFailedToExtractRights(string uri)
		{
			return new LocalizedString("QueryUsageRightsPrelicenseResponseFailedToExtractRights", "Ex7733F1", false, true, ServerStrings.ResourceManager, new object[]
			{
				uri
			});
		}

		public static LocalizedString MigrationBatchAutoComplete
		{
			get
			{
				return new LocalizedString("MigrationBatchAutoComplete", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedTag(string name)
		{
			return new LocalizedString("UnexpectedTag", "ExE31BC4", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MigrationPermanentError(string messageDetail)
		{
			return new LocalizedString("MigrationPermanentError", "Ex383EF2", false, true, ServerStrings.ResourceManager, new object[]
			{
				messageDetail
			});
		}

		public static LocalizedString CannotShareFolderException(string reason)
		{
			return new LocalizedString("CannotShareFolderException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				reason
			});
		}

		public static LocalizedString MigrationObjectsCountStringNone
		{
			get
			{
				return new LocalizedString("MigrationObjectsCountStringNone", "Ex31A129", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x810
		{
			get
			{
				return new LocalizedString("ClientCulture_0x810", "ExB1C12B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorADUserFoundByReadOnlyButNotWrite(string legacyDn)
		{
			return new LocalizedString("ErrorADUserFoundByReadOnlyButNotWrite", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				legacyDn
			});
		}

		public static LocalizedString MapiCannotCopyItem
		{
			get
			{
				return new LocalizedString("MapiCannotCopyItem", "Ex0605B8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionMaxEmbeddedDepthExceeded(int maxdepth)
		{
			return new LocalizedString("ConversionMaxEmbeddedDepthExceeded", "ExD55B41", false, true, ServerStrings.ResourceManager, new object[]
			{
				maxdepth
			});
		}

		public static LocalizedString RpcServerWrongRequestServer(string mailboxGuid, string error)
		{
			return new LocalizedString("RpcServerWrongRequestServer", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				error
			});
		}

		public static LocalizedString ErrorNoStoreObjectIdAndFolderPath
		{
			get
			{
				return new LocalizedString("ErrorNoStoreObjectIdAndFolderPath", "Ex0F85B2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFolderSyncMigrationReports
		{
			get
			{
				return new LocalizedString("MigrationFolderSyncMigrationReports", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnsupportedFormsCondition
		{
			get
			{
				return new LocalizedString("UnsupportedFormsCondition", "ExB6B9B6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExStartTimeNotSet
		{
			get
			{
				return new LocalizedString("ExStartTimeNotSet", "ExB85FF2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x804
		{
			get
			{
				return new LocalizedString("ClientCulture_0x804", "ExA5B0A9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConversationActionItemNotFound
		{
			get
			{
				return new LocalizedString("ExConversationActionItemNotFound", "Ex4DA5D6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusProvisioning
		{
			get
			{
				return new LocalizedString("MigrationUserStatusProvisioning", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeNonDeliveryReport
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeNonDeliveryReport", "ExEE9434", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFailedToUnregisterExchangeTopologyNotification
		{
			get
			{
				return new LocalizedString("ExFailedToUnregisterExchangeTopologyNotification", "ExB79E36", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageLearnMore
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageLearnMore", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DateRangeThreeDays
		{
			get
			{
				return new LocalizedString("DateRangeThreeDays", "Ex6B2F97", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CantFindCalendarFolderException(object identity)
		{
			return new LocalizedString("CantFindCalendarFolderException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString MapiCannotTransportSendMessage
		{
			get
			{
				return new LocalizedString("MapiCannotTransportSendMessage", "ExE286EB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSortNotSupportedInDeepTraversalQuery
		{
			get
			{
				return new LocalizedString("ExSortNotSupportedInDeepTraversalQuery", "Ex4C3A26", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailAmbiguousUsernameException
		{
			get
			{
				return new LocalizedString("JunkEmailAmbiguousUsernameException", "Ex7D436F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusSyncedWithErrors
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusSyncedWithErrors", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotCreateEmbeddedItem(string type)
		{
			return new LocalizedString("CannotCreateEmbeddedItem", "Ex6F46E3", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString FailedToFindAvailableHubs
		{
			get
			{
				return new LocalizedString("FailedToFindAvailableHubs", "Ex269861", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTeamMailboxGetUserMailboxDatabaseFailed(string user, string ex)
		{
			return new LocalizedString("ErrorTeamMailboxGetUserMailboxDatabaseFailed", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				user,
				ex
			});
		}

		public static LocalizedString SystemAPIFailed(string method, int retVal)
		{
			return new LocalizedString("SystemAPIFailed", "ExB42018", false, true, ServerStrings.ResourceManager, new object[]
			{
				method,
				retVal
			});
		}

		public static LocalizedString OleConversionInitError(string processName, int expectedId, int trueId)
		{
			return new LocalizedString("OleConversionInitError", "ExE8D18C", false, true, ServerStrings.ResourceManager, new object[]
			{
				processName,
				expectedId,
				trueId
			});
		}

		public static LocalizedString InternetCalendarName
		{
			get
			{
				return new LocalizedString("InternetCalendarName", "ExB2264B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExUnableToGetStreamProperty(string propertyName)
		{
			return new LocalizedString("ExUnableToGetStreamProperty", "ExC7FE77", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString ExMeetingCantCrossOtherOccurrences(TimeSpan endOffset, TimeSpan minimumTimeBetweenOccurrences)
		{
			return new LocalizedString("ExMeetingCantCrossOtherOccurrences", "Ex3FC250", false, true, ServerStrings.ResourceManager, new object[]
			{
				endOffset,
				minimumTimeBetweenOccurrences
			});
		}

		public static LocalizedString ExItemNotFound
		{
			get
			{
				return new LocalizedString("ExItemNotFound", "Ex5F9FEA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDelegateNotSupportedRespondToMeetingRequest
		{
			get
			{
				return new LocalizedString("ExDelegateNotSupportedRespondToMeetingRequest", "Ex1502CA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DisposeNonIPMFolder
		{
			get
			{
				return new LocalizedString("DisposeNonIPMFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConfigurationNotFound(string name)
		{
			return new LocalizedString("ExConfigurationNotFound", "ExB99D32", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InboxRuleSensitivityPrivate
		{
			get
			{
				return new LocalizedString("InboxRuleSensitivityPrivate", "Ex1D9027", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFeatureNone
		{
			get
			{
				return new LocalizedString("MigrationFeatureNone", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x405
		{
			get
			{
				return new LocalizedString("ClientCulture_0x405", "Ex28DE6F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNotSupportedCreateMode(int createMode)
		{
			return new LocalizedString("ExNotSupportedCreateMode", "Ex6C2042", false, true, ServerStrings.ResourceManager, new object[]
			{
				createMode
			});
		}

		public static LocalizedString ExStringContainsSurroundingWhiteSpace
		{
			get
			{
				return new LocalizedString("ExStringContainsSurroundingWhiteSpace", "ExC4366D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x4C0A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x4C0A", "ExBAF5FE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmDbAcllErrorNoReplicaInstance(string database, string server)
		{
			return new LocalizedString("AmDbAcllErrorNoReplicaInstance", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				database,
				server
			});
		}

		public static LocalizedString ClientCulture_0x1809
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1809", "Ex108688", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToResealKey
		{
			get
			{
				return new LocalizedString("FailedToResealKey", "Ex2A861E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidParticipantForRules
		{
			get
			{
				return new LocalizedString("InvalidParticipantForRules", "Ex7B88E5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SyncStateCollision(string name)
		{
			return new LocalizedString("SyncStateCollision", "Ex3C9243", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SpellCheckerDanish
		{
			get
			{
				return new LocalizedString("SpellCheckerDanish", "Ex79E420", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetProperties
		{
			get
			{
				return new LocalizedString("MapiCannotGetProperties", "ExB22655", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCopyMessagesFailed
		{
			get
			{
				return new LocalizedString("MapiCopyMessagesFailed", "Ex0151D5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToParseValue
		{
			get
			{
				return new LocalizedString("FailedToParseValue", "Ex91EA1B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RmLicenseRetrieveFailed(int code)
		{
			return new LocalizedString("RmLicenseRetrieveFailed", "ExDE16CC", false, true, ServerStrings.ResourceManager, new object[]
			{
				code
			});
		}

		public static LocalizedString RuleWriterObjectNotFound
		{
			get
			{
				return new LocalizedString("RuleWriterObjectNotFound", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscoveryMailboxCannotBeSourceOrTarget(string mailbox)
		{
			return new LocalizedString("DiscoveryMailboxCannotBeSourceOrTarget", "Ex033784", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString ExInvalidWatermarkString
		{
			get
			{
				return new LocalizedString("ExInvalidWatermarkString", "ExB85475", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningRequestCsvContainsBothPasswordAndFederatedIdentity
		{
			get
			{
				return new LocalizedString("ProvisioningRequestCsvContainsBothPasswordAndFederatedIdentity", "ExDB943B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationResultSucceeded
		{
			get
			{
				return new LocalizedString("OperationResultSucceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerLocatorServicePermanentFault
		{
			get
			{
				return new LocalizedString("ServerLocatorServicePermanentFault", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotMailboxSession
		{
			get
			{
				return new LocalizedString("NotMailboxSession", "Ex14E4F8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationStateActive
		{
			get
			{
				return new LocalizedString("MigrationStateActive", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Null
		{
			get
			{
				return new LocalizedString("Null", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorNonUniqueLegacyDN(string legacyDN)
		{
			return new LocalizedString("ErrorNonUniqueLegacyDN", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				legacyDN
			});
		}

		public static LocalizedString ExMclIsTooBig(long actualSize, long maxAllowedSize)
		{
			return new LocalizedString("ExMclIsTooBig", "ExEBD215", false, true, ServerStrings.ResourceManager, new object[]
			{
				actualSize,
				maxAllowedSize
			});
		}

		public static LocalizedString FolderRuleStageOnCreatedMessage
		{
			get
			{
				return new LocalizedString("FolderRuleStageOnCreatedMessage", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSetMessageFlags
		{
			get
			{
				return new LocalizedString("CannotSetMessageFlags", "ExFAB42B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidAsyncResult
		{
			get
			{
				return new LocalizedString("ExInvalidAsyncResult", "ExF7EEFE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidPropertyType(string propertyKey, string type)
		{
			return new LocalizedString("ExInvalidPropertyType", "Ex8CEAA8", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyKey,
				type
			});
		}

		public static LocalizedString ClientCulture_0x2801
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2801", "ExAE73CB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFolderPropertyBagCannotSaveChanges
		{
			get
			{
				return new LocalizedString("ExFolderPropertyBagCannotSaveChanges", "ExD835DE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusStopped
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusStopped", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxDatabaseRequired(Guid mailboxGuid)
		{
			return new LocalizedString("MailboxDatabaseRequired", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString KqlParserTimeout
		{
			get
			{
				return new LocalizedString("KqlParserTimeout", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListXsoTooBigException(string value)
		{
			return new LocalizedString("JunkEmailTrustedListXsoTooBigException", "Ex4E399A", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString TenMinutes
		{
			get
			{
				return new LocalizedString("TenMinutes", "Ex16843E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExOperationNotSupportedForRoutingType(string operation, string routingType)
		{
			return new LocalizedString("ExOperationNotSupportedForRoutingType", "Ex068DC8", false, true, ServerStrings.ResourceManager, new object[]
			{
				operation,
				routingType
			});
		}

		public static LocalizedString DiscoveryMailboxIsNotUnique(string id1, string id2)
		{
			return new LocalizedString("DiscoveryMailboxIsNotUnique", "Ex7673AA", false, true, ServerStrings.ResourceManager, new object[]
			{
				id1,
				id2
			});
		}

		public static LocalizedString ExMustSetSearchCriteriaToMakeVisibleToOutlook
		{
			get
			{
				return new LocalizedString("ExMustSetSearchCriteriaToMakeVisibleToOutlook", "ExE891AD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConflictingObjectType(int expected, int actual)
		{
			return new LocalizedString("ConflictingObjectType", "Ex04F689", false, true, ServerStrings.ResourceManager, new object[]
			{
				expected,
				actual
			});
		}

		public static LocalizedString Wednesday
		{
			get
			{
				return new LocalizedString("Wednesday", "ExD565E3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0xC07
		{
			get
			{
				return new LocalizedString("ClientCulture_0xC07", "Ex3D8E96", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotOpenMailbox(string mailbox)
		{
			return new LocalizedString("MapiCannotOpenMailbox", "Ex2B87C8", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString LegacyMailboxSearchDescription
		{
			get
			{
				return new LocalizedString("LegacyMailboxSearchDescription", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSyncStateCorrupted(string syncstate)
		{
			return new LocalizedString("ExSyncStateCorrupted", "Ex522C7C", false, true, ServerStrings.ResourceManager, new object[]
			{
				syncstate
			});
		}

		public static LocalizedString ExInvalidHexString(string hexString)
		{
			return new LocalizedString("ExInvalidHexString", "Ex4ACF3A", false, true, ServerStrings.ResourceManager, new object[]
			{
				hexString
			});
		}

		public static LocalizedString MigrationUserSkippedItemString(string folder, string subject)
		{
			return new LocalizedString("MigrationUserSkippedItemString", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				folder,
				subject
			});
		}

		public static LocalizedString FolderRuleErrorInvalidRecipient(string recipient)
		{
			return new LocalizedString("FolderRuleErrorInvalidRecipient", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString MapiCannotMatchAttachmentIds(object actualId, object expectedId)
		{
			return new LocalizedString("MapiCannotMatchAttachmentIds", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				actualId,
				expectedId
			});
		}

		public static LocalizedString MapiCannotFreeBookmark
		{
			get
			{
				return new LocalizedString("MapiCannotFreeBookmark", "ExDF15FD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToAcquireRacAndClc(string origId, string id)
		{
			return new LocalizedString("FailedToAcquireRacAndClc", "Ex2533BB", false, true, ServerStrings.ResourceManager, new object[]
			{
				origId,
				id
			});
		}

		public static LocalizedString CannotChangePermissionsOnFolder
		{
			get
			{
				return new LocalizedString("CannotChangePermissionsOnFolder", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSetProps
		{
			get
			{
				return new LocalizedString("MapiCannotSetProps", "Ex9054B2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStatePartiallySucceeded
		{
			get
			{
				return new LocalizedString("SearchStatePartiallySucceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailSubjectCreated(string notificationType)
		{
			return new LocalizedString("NotificationEmailSubjectCreated", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				notificationType
			});
		}

		public static LocalizedString SyncStateMissing(string name)
		{
			return new LocalizedString("SyncStateMissing", "ExF4DEF6", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InboxRuleMessageTypeAutomaticReply
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeAutomaticReply", "Ex82D866", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToFindTargetUriFromMExData(Uri url)
		{
			return new LocalizedString("FailedToFindTargetUriFromMExData", "Ex103BC8", false, true, ServerStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString AmDatabaseNotFoundException(Guid dbGuid)
		{
			return new LocalizedString("AmDatabaseNotFoundException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dbGuid
			});
		}

		public static LocalizedString ExInvalidHexCharacter(char hexCharacter)
		{
			return new LocalizedString("ExInvalidHexCharacter", "Ex115A50", false, true, ServerStrings.ResourceManager, new object[]
			{
				hexCharacter
			});
		}

		public static LocalizedString RuleHistoryError
		{
			get
			{
				return new LocalizedString("RuleHistoryError", "Ex834D15", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x280A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x280A", "ExB16BD5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerForDatabaseNotFound(string dbName, string databaseGuid)
		{
			return new LocalizedString("ServerForDatabaseNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dbName,
				databaseGuid
			});
		}

		public static LocalizedString ClientCulture_0x3001
		{
			get
			{
				return new LocalizedString("ClientCulture_0x3001", "Ex690E17", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RpcClientException(string method, string server)
		{
			return new LocalizedString("RpcClientException", "Ex1D3DE9", false, true, ServerStrings.ResourceManager, new object[]
			{
				method,
				server
			});
		}

		public static LocalizedString SharePoint
		{
			get
			{
				return new LocalizedString("SharePoint", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorCouldNotUpdateMasterIdentityProperty(string name)
		{
			return new LocalizedString("ErrorCouldNotUpdateMasterIdentityProperty", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString DataMoveReplicationConstraintSatisfied(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintSatisfied", "ExE281C0", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid
			});
		}

		public static LocalizedString NoDelegateAction
		{
			get
			{
				return new LocalizedString("NoDelegateAction", "Ex953F36", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFlagsStop
		{
			get
			{
				return new LocalizedString("MigrationFlagsStop", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNoOptimizedCodePath
		{
			get
			{
				return new LocalizedString("ExNoOptimizedCodePath", "ExECB030", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownOscProvider(string provider)
		{
			return new LocalizedString("UnknownOscProvider", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				provider
			});
		}

		public static LocalizedString ErrorTeamMailboxUserNotResolved(string users)
		{
			return new LocalizedString("ErrorTeamMailboxUserNotResolved", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				users
			});
		}

		public static LocalizedString MigrationBatchFlagUseAdvancedValidation
		{
			get
			{
				return new LocalizedString("MigrationBatchFlagUseAdvancedValidation", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetParentEntryId
		{
			get
			{
				return new LocalizedString("MapiCannotGetParentEntryId", "Ex8B3B77", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExOnlyMessagesHaveParent
		{
			get
			{
				return new LocalizedString("ExOnlyMessagesHaveParent", "ExE7329B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderRuleErrorGroupDoesNotResolve(string address)
		{
			return new LocalizedString("FolderRuleErrorGroupDoesNotResolve", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString ClientCulture_0x411
		{
			get
			{
				return new LocalizedString("ClientCulture_0x411", "Ex3F88D9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFolderDoesNotMatchFolderId
		{
			get
			{
				return new LocalizedString("ExFolderDoesNotMatchFolderId", "Ex800E7B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationTypeXO1
		{
			get
			{
				return new LocalizedString("MigrationTypeXO1", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DatabaseNotFound(string databaseId)
		{
			return new LocalizedString("DatabaseNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				databaseId
			});
		}

		public static LocalizedString NotOperator
		{
			get
			{
				return new LocalizedString("NotOperator", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x480A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x480A", "ExAA2487", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Saturday
		{
			get
			{
				return new LocalizedString("Saturday", "ExA50D93", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFailedToRegisterExchangeTopologyNotification
		{
			get
			{
				return new LocalizedString("ExFailedToRegisterExchangeTopologyNotification", "ExE1EBCE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchSupportedActionSet
		{
			get
			{
				return new LocalizedString("MigrationBatchSupportedActionSet", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSearchFolderCorruptOutlookBlob(string param)
		{
			return new LocalizedString("ExSearchFolderCorruptOutlookBlob", "Ex852FE1", false, true, ServerStrings.ResourceManager, new object[]
			{
				param
			});
		}

		public static LocalizedString ConversionCannotOpenJournalMessage
		{
			get
			{
				return new LocalizedString("ConversionCannotOpenJournalMessage", "Ex0E26E5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotMoveOrCopyOccurrenceItem(object itemId)
		{
			return new LocalizedString("ExCannotMoveOrCopyOccurrenceItem", "Ex3D161C", false, true, ServerStrings.ResourceManager, new object[]
			{
				itemId
			});
		}

		public static LocalizedString JunkEmailTrustedListXsoEmptyException
		{
			get
			{
				return new LocalizedString("JunkEmailTrustedListXsoEmptyException", "Ex29DBA4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AmFailedToFindSuitableServer
		{
			get
			{
				return new LocalizedString("AmFailedToFindSuitableServer", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationalError
		{
			get
			{
				return new LocalizedString("OperationalError", "Ex19E8E8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExTooManySortColumns
		{
			get
			{
				return new LocalizedString("ExTooManySortColumns", "Ex894B55", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LoadRulesFromStore
		{
			get
			{
				return new LocalizedString("LoadRulesFromStore", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x40D
		{
			get
			{
				return new LocalizedString("ClientCulture_0x40D", "Ex65A67B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCantCopyBadAlienDLMember
		{
			get
			{
				return new LocalizedString("ExCantCopyBadAlienDLMember", "Ex948C20", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageSendMailToTheSiteMailbox
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageSendMailToTheSiteMailbox", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListXsoFormatException(string value)
		{
			return new LocalizedString("JunkEmailBlockedListXsoFormatException", "ExA9C73B", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvalidDueDate2(string dueDate, string reminderTime)
		{
			return new LocalizedString("InvalidDueDate2", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				dueDate,
				reminderTime
			});
		}

		public static LocalizedString InboxRuleSensitivityPersonal
		{
			get
			{
				return new LocalizedString("InboxRuleSensitivityPersonal", "Ex74F127", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidItemCountAdvisorCondition
		{
			get
			{
				return new LocalizedString("ExInvalidItemCountAdvisorCondition", "Ex92E02C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorLoadingRules
		{
			get
			{
				return new LocalizedString("ErrorLoadingRules", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x414
		{
			get
			{
				return new LocalizedString("ClientCulture_0x414", "Ex3053DD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExEndTimeNotSet
		{
			get
			{
				return new LocalizedString("ExEndTimeNotSet", "Ex568ACE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeAutomaticForward
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeAutomaticForward", "Ex1D44E3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotCopyMapiProps
		{
			get
			{
				return new LocalizedString("MapiCannotCopyMapiProps", "ExF39EC5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OneWeeks
		{
			get
			{
				return new LocalizedString("OneWeeks", "Ex07309C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageWhatYouCanDoNext
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageWhatYouCanDoNext", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetReceiveFolderInfo
		{
			get
			{
				return new LocalizedString("MapiCannotGetReceiveFolderInfo", "Ex270031", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationRunspaceError(string commandName, string msg)
		{
			return new LocalizedString("MigrationRunspaceError", "Ex650C08", false, true, ServerStrings.ResourceManager, new object[]
			{
				commandName,
				msg
			});
		}

		public static LocalizedString ExInvalidStoreObjectId
		{
			get
			{
				return new LocalizedString("ExInvalidStoreObjectId", "ExC007AC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestStateQueued
		{
			get
			{
				return new LocalizedString("RequestStateQueued", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecurrenceBlobCorrupted
		{
			get
			{
				return new LocalizedString("RecurrenceBlobCorrupted", "Ex89ED2D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotFindAttachment
		{
			get
			{
				return new LocalizedString("CannotFindAttachment", "Ex5266E8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidRecipientBlob
		{
			get
			{
				return new LocalizedString("ExInvalidRecipientBlob", "Ex5962A3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageReactivatedSubject(string tmName)
		{
			return new LocalizedString("TeamMailboxMessageReactivatedSubject", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				tmName
			});
		}

		public static LocalizedString ExIncompleteBlob
		{
			get
			{
				return new LocalizedString("ExIncompleteBlob", "Ex3A7A3D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExPatternNotSet
		{
			get
			{
				return new LocalizedString("ExPatternNotSet", "Ex16EFD9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CompositeError(LocalizedString error1, LocalizedString error2)
		{
			return new LocalizedString("CompositeError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error1,
				error2
			});
		}

		public static LocalizedString ExInvalidDayOfMonth
		{
			get
			{
				return new LocalizedString("ExInvalidDayOfMonth", "Ex98518B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidGlobalObjectId
		{
			get
			{
				return new LocalizedString("ExInvalidGlobalObjectId", "Ex453FE1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetHierarchyTable
		{
			get
			{
				return new LocalizedString("MapiCannotGetHierarchyTable", "ExA6C898", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNullItemIdParameter(int index)
		{
			return new LocalizedString("ExNullItemIdParameter", "Ex566E8A", false, true, ServerStrings.ResourceManager, new object[]
			{
				index
			});
		}

		public static LocalizedString ExItemDoesNotBelongToCurrentFolder(object itemId)
		{
			return new LocalizedString("ExItemDoesNotBelongToCurrentFolder", "Ex2E8F86", false, true, ServerStrings.ResourceManager, new object[]
			{
				itemId
			});
		}

		public static LocalizedString ClientCulture_0xC0A
		{
			get
			{
				return new LocalizedString("ClientCulture_0xC0A", "Ex89D7D7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidFullyQualifiedServerName
		{
			get
			{
				return new LocalizedString("ExInvalidFullyQualifiedServerName", "Ex482CF4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EstimateStateInProgress
		{
			get
			{
				return new LocalizedString("EstimateStateInProgress", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSetReadFlags
		{
			get
			{
				return new LocalizedString("MapiCannotSetReadFlags", "Ex34D7CF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderQueryStatusSyncFolderHierarchyRpcFailed
		{
			get
			{
				return new LocalizedString("PublicFolderQueryStatusSyncFolderHierarchyRpcFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AddressRequiredForRoutingType(string routingType)
		{
			return new LocalizedString("AddressRequiredForRoutingType", "Ex522D18", false, true, ServerStrings.ResourceManager, new object[]
			{
				routingType
			});
		}

		public static LocalizedString ExInvalidSearchFolderScope
		{
			get
			{
				return new LocalizedString("ExInvalidSearchFolderScope", "Ex82F751", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActivitySessionIsNull
		{
			get
			{
				return new LocalizedString("ActivitySessionIsNull", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerItalian
		{
			get
			{
				return new LocalizedString("SpellCheckerItalian", "ExA50EDC", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InconsistentCalendarType(string type, string id)
		{
			return new LocalizedString("InconsistentCalendarType", "ExA69809", false, true, ServerStrings.ResourceManager, new object[]
			{
				type,
				id
			});
		}

		public static LocalizedString FolderRuleStageOnPublicFolderAfter
		{
			get
			{
				return new LocalizedString("FolderRuleStageOnPublicFolderAfter", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotAllowedAnonymousSharingByPolicy
		{
			get
			{
				return new LocalizedString("NotAllowedAnonymousSharingByPolicy", "Ex846A49", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PropertyErrorString(string name, PropertyErrorCode code, string descr)
		{
			return new LocalizedString("PropertyErrorString", "ExD955C2", false, true, ServerStrings.ResourceManager, new object[]
			{
				name,
				code,
				descr
			});
		}

		public static LocalizedString ExStartDateCantBeGreaterThanMaximum(object startDate, object maximumStartDate)
		{
			return new LocalizedString("ExStartDateCantBeGreaterThanMaximum", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				startDate,
				maximumStartDate
			});
		}

		public static LocalizedString ExEndDateCantExceedMaxDate(object endTime, object maxTime)
		{
			return new LocalizedString("ExEndDateCantExceedMaxDate", "Ex462507", false, true, ServerStrings.ResourceManager, new object[]
			{
				endTime,
				maxTime
			});
		}

		public static LocalizedString CannotCreateManifestEx(Type xsoManifestType)
		{
			return new LocalizedString("CannotCreateManifestEx", "Ex456737", false, true, ServerStrings.ResourceManager, new object[]
			{
				xsoManifestType
			});
		}

		public static LocalizedString MapiCannotGetCollapseState
		{
			get
			{
				return new LocalizedString("MapiCannotGetCollapseState", "Ex91A6BE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ZeroMinutes
		{
			get
			{
				return new LocalizedString("ZeroMinutes", "Ex42F6F9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientNotSupportedByAnyProviderException
		{
			get
			{
				return new LocalizedString("RecipientNotSupportedByAnyProviderException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationReportFinalizationSuccess
		{
			get
			{
				return new LocalizedString("MigrationReportFinalizationSuccess", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CalNotifTypeChangedUpdate
		{
			get
			{
				return new LocalizedString("CalNotifTypeChangedUpdate", "ExE24103", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExNewerVersionedSyncState(string syncstate, int savedVersion, int casVersion)
		{
			return new LocalizedString("ExNewerVersionedSyncState", "Ex680792", false, true, ServerStrings.ResourceManager, new object[]
			{
				syncstate,
				savedVersion,
				casVersion
			});
		}

		public static LocalizedString ExSizeFilterPropertyNotSupported
		{
			get
			{
				return new LocalizedString("ExSizeFilterPropertyNotSupported", "ExA2611E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidTimeFormat(string timeFormat, string lang, string validFormats)
		{
			return new LocalizedString("ErrorInvalidTimeFormat", "Ex402B93", false, true, ServerStrings.ResourceManager, new object[]
			{
				timeFormat,
				lang,
				validFormats
			});
		}

		public static LocalizedString InvalidTagName(string expected, string got)
		{
			return new LocalizedString("InvalidTagName", "ExDF59B1", false, true, ServerStrings.ResourceManager, new object[]
			{
				expected,
				got
			});
		}

		public static LocalizedString ExConversationActionInvalidFolderType
		{
			get
			{
				return new LocalizedString("ExConversationActionInvalidFolderType", "ExD19EDB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x2009
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2009", "Ex8B0E51", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UserDiscoveryMailboxNotFound
		{
			get
			{
				return new LocalizedString("UserDiscoveryMailboxNotFound", "Ex80BE98", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCorruptedRecurringCalItem
		{
			get
			{
				return new LocalizedString("ExCorruptedRecurringCalItem", "ExF732FD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationJobItemRecipientTypeMismatch(string smtpAddress, string newType, string oldType)
		{
			return new LocalizedString("MigrationJobItemRecipientTypeMismatch", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				smtpAddress,
				newType,
				oldType
			});
		}

		public static LocalizedString ExStartDateLaterThanEndDate
		{
			get
			{
				return new LocalizedString("ExStartDateLaterThanEndDate", "Ex406EED", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExModifiedOccurrenceCrossingAdjacentOccurrenceBoundary(object startTime, object endTime, object adjacentOccStartTime, object adjacentOccEndTime)
		{
			return new LocalizedString("ExModifiedOccurrenceCrossingAdjacentOccurrenceBoundary", "Ex30ED0F", false, true, ServerStrings.ResourceManager, new object[]
			{
				startTime,
				endTime,
				adjacentOccStartTime,
				adjacentOccEndTime
			});
		}

		public static LocalizedString ExInvalidOrganizer
		{
			get
			{
				return new LocalizedString("ExInvalidOrganizer", "Ex656263", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxExclusionReached
		{
			get
			{
				return new LocalizedString("MaxExclusionReached", "Ex3A695F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailBlockedListXsoEmptyException
		{
			get
			{
				return new LocalizedString("JunkEmailBlockedListXsoEmptyException", "ExA36FE0", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStateSucceeded
		{
			get
			{
				return new LocalizedString("SearchStateSucceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExComparisonOperatorNotSupportedForProperty(string comparisonOperator, string propertyName)
		{
			return new LocalizedString("ExComparisonOperatorNotSupportedForProperty", "Ex0008CC", false, true, ServerStrings.ResourceManager, new object[]
			{
				comparisonOperator,
				propertyName
			});
		}

		public static LocalizedString ExInvalidJournalReportFormat
		{
			get
			{
				return new LocalizedString("ExInvalidJournalReportFormat", "Ex071254", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotAuthorizedtoAccessGroupMailbox(string user, string group)
		{
			return new LocalizedString("NotAuthorizedtoAccessGroupMailbox", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				user,
				group
			});
		}

		public static LocalizedString RequestStateCertExpiring
		{
			get
			{
				return new LocalizedString("RequestStateCertExpiring", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TaskServerException(string errorMessage)
		{
			return new LocalizedString("TaskServerException", "ExB2F450", false, true, ServerStrings.ResourceManager, new object[]
			{
				errorMessage
			});
		}

		public static LocalizedString ConversionBodyConversionFailed
		{
			get
			{
				return new LocalizedString("ConversionBodyConversionFailed", "ExEC2A9C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidQueryTooLong(string name)
		{
			return new LocalizedString("ErrorInvalidQueryTooLong", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString MigrationUserAdminTypeTenantAdmin
		{
			get
			{
				return new LocalizedString("MigrationUserAdminTypeTenantAdmin", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x40B
		{
			get
			{
				return new LocalizedString("ClientCulture_0x40B", "Ex8C1297", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConfigDataCorrupted(string field)
		{
			return new LocalizedString("ExConfigDataCorrupted", "ExB43055", false, true, ServerStrings.ResourceManager, new object[]
			{
				field
			});
		}

		public static LocalizedString ConversionEmptyAddress
		{
			get
			{
				return new LocalizedString("ConversionEmptyAddress", "ExD90BD9", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x380A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x380A", "ExAF4C31", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFoldersCannotBeAccessedDuringCompletion
		{
			get
			{
				return new LocalizedString("PublicFoldersCannotBeAccessedDuringCompletion", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LicenseExpired(string expiryTime)
		{
			return new LocalizedString("LicenseExpired", "Ex9E8A95", false, true, ServerStrings.ResourceManager, new object[]
			{
				expiryTime
			});
		}

		public static LocalizedString ElementHasUnsupportedValue(string name)
		{
			return new LocalizedString("ElementHasUnsupportedValue", "Ex81A452", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString CannotResolvePropertyTagsToPropertyDefinitions(uint propertyTag)
		{
			return new LocalizedString("CannotResolvePropertyTagsToPropertyDefinitions", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				propertyTag
			});
		}

		public static LocalizedString MigrationReportUnknown
		{
			get
			{
				return new LocalizedString("MigrationReportUnknown", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderRuleResolvingAddressBookEntryId
		{
			get
			{
				return new LocalizedString("FolderRuleResolvingAddressBookEntryId", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharePointLifecyclePolicy
		{
			get
			{
				return new LocalizedString("SharePointLifecyclePolicy", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotAddNotification
		{
			get
			{
				return new LocalizedString("MapiCannotAddNotification", "ExB8C380", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonNegativeParameter(string parameter)
		{
			return new LocalizedString("NonNegativeParameter", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				parameter
			});
		}

		public static LocalizedString ClientCulture_0x200A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x200A", "Ex549083", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFailedToCreateEventManager
		{
			get
			{
				return new LocalizedString("ExFailedToCreateEventManager", "ExD6A304", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaixmumNumberOfMailboxAssociationsForUserReached
		{
			get
			{
				return new LocalizedString("MaixmumNumberOfMailboxAssociationsForUserReached", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidEIT
		{
			get
			{
				return new LocalizedString("ExInvalidEIT", "Ex3FF77B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFilterAndSortNotSupportedInSimpleVirtualPropertyDefinition
		{
			get
			{
				return new LocalizedString("ExFilterAndSortNotSupportedInSimpleVirtualPropertyDefinition", "Ex61CE5C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CreateConfigurationItem(string mailbox)
		{
			return new LocalizedString("CreateConfigurationItem", "Ex6F2B5B", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString DateRangeOneMonth
		{
			get
			{
				return new LocalizedString("DateRangeOneMonth", "ExE73099", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiInvalidParam
		{
			get
			{
				return new LocalizedString("MapiInvalidParam", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotDeleteUserPhoto
		{
			get
			{
				return new LocalizedString("MapiCannotDeleteUserPhoto", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserAdminTypePartner
		{
			get
			{
				return new LocalizedString("MigrationUserAdminTypePartner", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveMonitoringOperationFailedWithEcException(int ec)
		{
			return new LocalizedString("ActiveMonitoringOperationFailedWithEcException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				ec
			});
		}

		public static LocalizedString ExOrganizerCannotCallUpdateCalendarItem
		{
			get
			{
				return new LocalizedString("ExOrganizerCannotCallUpdateCalendarItem", "ExCA94E2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateCondition
		{
			get
			{
				return new LocalizedString("DuplicateCondition", "Ex092B6F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListXsoTooManyException
		{
			get
			{
				return new LocalizedString("JunkEmailTrustedListXsoTooManyException", "ExA0A929", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VersionNotInteger
		{
			get
			{
				return new LocalizedString("VersionNotInteger", "ExDE59FB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x41A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x41A", "ExB72BAF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidRecurrenceInterval(int recurrenceInterval)
		{
			return new LocalizedString("ExInvalidRecurrenceInterval", "Ex3E43DC", false, true, ServerStrings.ResourceManager, new object[]
			{
				recurrenceInterval
			});
		}

		public static LocalizedString FederatedMailboxNotSet(string tenantId)
		{
			return new LocalizedString("FederatedMailboxNotSet", "ExF831B4", false, true, ServerStrings.ResourceManager, new object[]
			{
				tenantId
			});
		}

		public static LocalizedString JunkEmailValidationError(string value)
		{
			return new LocalizedString("JunkEmailValidationError", "Ex4032C2", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MapiCannotGetNamedProperties
		{
			get
			{
				return new LocalizedString("MapiCannotGetNamedProperties", "Ex4FFDEF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFailedToDeleteDefaultFolder
		{
			get
			{
				return new LocalizedString("ExFailedToDeleteDefaultFolder", "Ex5F0402", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExDefaultFoldersNotInitialized
		{
			get
			{
				return new LocalizedString("ExDefaultFoldersNotInitialized", "ExE6AFED", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeCertExpiry
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeCertExpiry", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUnexpectedExchangePrincipalFound(string smtpAddress)
		{
			return new LocalizedString("MigrationUnexpectedExchangePrincipalFound", "ExE22EA7", false, true, ServerStrings.ResourceManager, new object[]
			{
				smtpAddress
			});
		}

		public static LocalizedString RpcServerRequestThrottled(string mailboxGuid, string nextAllowedTime)
		{
			return new LocalizedString("RpcServerRequestThrottled", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mailboxGuid,
				nextAllowedTime
			});
		}

		public static LocalizedString ClosingTagExpectedNoneFound
		{
			get
			{
				return new LocalizedString("ClosingTagExpectedNoneFound", "Ex681320", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DefaultFolderNotFoundInPublicFolderMailbox(object id)
		{
			return new LocalizedString("DefaultFolderNotFoundInPublicFolderMailbox", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString JunkEmailTrustedListInternalToOrganizationException(string value)
		{
			return new LocalizedString("JunkEmailTrustedListInternalToOrganizationException", "Ex5BD8BA", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString FolderRuleStageEvaluation
		{
			get
			{
				return new LocalizedString("FolderRuleStageEvaluation", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExStartDateCantBeLessThanMinimum(object startDate, object minimumStartDate)
		{
			return new LocalizedString("ExStartDateCantBeLessThanMinimum", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				startDate,
				minimumStartDate
			});
		}

		public static LocalizedString ExPDLCorruptOutlookBlob(string param)
		{
			return new LocalizedString("ExPDLCorruptOutlookBlob", "Ex9B9D77", false, true, ServerStrings.ResourceManager, new object[]
			{
				param
			});
		}

		public static LocalizedString ClientCulture_0x814
		{
			get
			{
				return new LocalizedString("ClientCulture_0x814", "Ex7687AE", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageMemberInvitationBodyIntroText
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageMemberInvitationBodyIntroText", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0xC09
		{
			get
			{
				return new LocalizedString("ClientCulture_0xC09", "Ex712706", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedToken
		{
			get
			{
				return new LocalizedString("UnexpectedToken", "Ex19401D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSeekRowBookmark
		{
			get
			{
				return new LocalizedString("MapiCannotSeekRowBookmark", "Ex714CD8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderSyncFolderRpcFailed
		{
			get
			{
				return new LocalizedString("PublicFolderSyncFolderRpcFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DataMoveReplicationConstraintNotSatisfied(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintNotSatisfied", "Ex46A4F7", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid
			});
		}

		public static LocalizedString MigrationFlagsNone
		{
			get
			{
				return new LocalizedString("MigrationFlagsNone", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListXsoNullException
		{
			get
			{
				return new LocalizedString("JunkEmailTrustedListXsoNullException", "Ex3B51D7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotQueryRows
		{
			get
			{
				return new LocalizedString("MapiCannotQueryRows", "ExF149A5", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidLicense(string mailbox)
		{
			return new LocalizedString("ExInvalidLicense", "Ex78C8F8", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString ErrorCalendarReminderTooLarge(string value)
		{
			return new LocalizedString("ErrorCalendarReminderTooLarge", "Ex9C2D25", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString MigrationTestMSASuccess
		{
			get
			{
				return new LocalizedString("MigrationTestMSASuccess", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSetNotSupportedForCalculatedProperty(object proptertyID)
		{
			return new LocalizedString("ExSetNotSupportedForCalculatedProperty", "ExA65ABB", false, true, ServerStrings.ResourceManager, new object[]
			{
				proptertyID
			});
		}

		public static LocalizedString MigrationMailboxPermissionAdmin
		{
			get
			{
				return new LocalizedString("MigrationMailboxPermissionAdmin", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidItemId
		{
			get
			{
				return new LocalizedString("ExInvalidItemId", "Ex967D22", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotSetSpooler
		{
			get
			{
				return new LocalizedString("MapiCannotSetSpooler", "Ex503D52", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExFilterNotSupported(object restriction)
		{
			return new LocalizedString("ExFilterNotSupported", "ExD66D33", false, true, ServerStrings.ResourceManager, new object[]
			{
				restriction
			});
		}

		public static LocalizedString JunkEmailTrustedListOwnersEmailAddressException(string value)
		{
			return new LocalizedString("JunkEmailTrustedListOwnersEmailAddressException", "ExF3B776", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorUnsupportedConfigurationXmlVersion(string category, string version)
		{
			return new LocalizedString("ErrorUnsupportedConfigurationXmlVersion", "Ex438911", false, true, ServerStrings.ResourceManager, new object[]
			{
				category,
				version
			});
		}

		public static LocalizedString InvalidSharingTargetRecipientException
		{
			get
			{
				return new LocalizedString("InvalidSharingTargetRecipientException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMultivalueElement(int elementIndex)
		{
			return new LocalizedString("ExInvalidMultivalueElement", "ExA9B69F", false, true, ServerStrings.ResourceManager, new object[]
			{
				elementIndex
			});
		}

		public static LocalizedString RequestStateInProgress
		{
			get
			{
				return new LocalizedString("RequestStateInProgress", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnlockOOFHistory
		{
			get
			{
				return new LocalizedString("UnlockOOFHistory", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderHierarchySessionNull(string user)
		{
			return new LocalizedString("PublicFolderHierarchySessionNull", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ExMclCannotBeResolved
		{
			get
			{
				return new LocalizedString("ExMclCannotBeResolved", "Ex711F76", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadConfiguration
		{
			get
			{
				return new LocalizedString("FailedToReadConfiguration", "Ex145E0E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADUserNoMailbox
		{
			get
			{
				return new LocalizedString("ADUserNoMailbox", "Ex7DC61E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExReadExchangeTopologyFailed
		{
			get
			{
				return new LocalizedString("ExReadExchangeTopologyFailed", "ExA104A7", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailSubjectCompleted(string notificationType)
		{
			return new LocalizedString("NotificationEmailSubjectCompleted", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				notificationType
			});
		}

		public static LocalizedString ErrorTeamMailboxUserTypeUnqualified(string users)
		{
			return new LocalizedString("ErrorTeamMailboxUserTypeUnqualified", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				users
			});
		}

		public static LocalizedString MigrationUserStatusCorrupted
		{
			get
			{
				return new LocalizedString("MigrationUserStatusCorrupted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExMatchShouldHaveBeenCalled
		{
			get
			{
				return new LocalizedString("ExMatchShouldHaveBeenCalled", "ExE03785", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotModifyRemovedRecipient
		{
			get
			{
				return new LocalizedString("ExCannotModifyRemovedRecipient", "Ex43F3A2", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidDateTimeFormat
		{
			get
			{
				return new LocalizedString("InvalidDateTimeFormat", "ExC829C4", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OleConversionPrepareFailed
		{
			get
			{
				return new LocalizedString("OleConversionPrepareFailed", "ExD62870", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailBodyExportPSTFailed
		{
			get
			{
				return new LocalizedString("NotificationEmailBodyExportPSTFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExRangeNotSet
		{
			get
			{
				return new LocalizedString("ExRangeNotSet", "Ex22F0CB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversationCreatorSidNotSet(string conversationId, string rootDeliveredTime)
		{
			return new LocalizedString("ConversationCreatorSidNotSet", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				conversationId,
				rootDeliveredTime
			});
		}

		public static LocalizedString ExNullParameter(string argName, int argNumber)
		{
			return new LocalizedString("ExNullParameter", "Ex551FFA", false, true, ServerStrings.ResourceManager, new object[]
			{
				argName,
				argNumber
			});
		}

		public static LocalizedString ExEmptyCollection(string argument)
		{
			return new LocalizedString("ExEmptyCollection", "ExE9E1AE", false, true, ServerStrings.ResourceManager, new object[]
			{
				argument
			});
		}

		public static LocalizedString OrganizationNotFederatedException
		{
			get
			{
				return new LocalizedString("OrganizationNotFederatedException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x421
		{
			get
			{
				return new LocalizedString("ClientCulture_0x421", "Ex5A41A3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ACLTooBig
		{
			get
			{
				return new LocalizedString("ACLTooBig", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationAttachmentNotFound(string attachment)
		{
			return new LocalizedString("MigrationAttachmentNotFound", "ExC93802", false, true, ServerStrings.ResourceManager, new object[]
			{
				attachment
			});
		}

		public static LocalizedString TeamMailboxMessageNotConnectedToSite
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageNotConnectedToSite", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ActiveManagerUnknownGenericRpcCommand(int requestedServerVersion, int replyingServerVersion, int commandId)
		{
			return new LocalizedString("ActiveManagerUnknownGenericRpcCommand", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				requestedServerVersion,
				replyingServerVersion,
				commandId
			});
		}

		public static LocalizedString TeamMailboxSyncStatusFailed
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x80A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x80A", "Ex13DC11", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RecipientAddressInvalid(string user)
		{
			return new LocalizedString("RecipientAddressInvalid", "Ex76B8AB", false, true, ServerStrings.ResourceManager, new object[]
			{
				user
			});
		}

		public static LocalizedString ExInvalidExceptionInfoSubstringLength(int lengthWithNull, int lengthWithoutNull)
		{
			return new LocalizedString("ExInvalidExceptionInfoSubstringLength", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				lengthWithNull,
				lengthWithoutNull
			});
		}

		public static LocalizedString DagNetworkCannotRemoveActiveSubnet(string name)
		{
			return new LocalizedString("DagNetworkCannotRemoveActiveSubnet", "Ex753A61", false, true, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString FailedToBindToUseLicense
		{
			get
			{
				return new LocalizedString("FailedToBindToUseLicense", "Ex435C84", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetParentId
		{
			get
			{
				return new LocalizedString("MapiCannotGetParentId", "Ex8339EA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExEndDateEarlierThanStartDate
		{
			get
			{
				return new LocalizedString("ExEndDateEarlierThanStartDate", "ExF31D58", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidNullParameterForChangeTypes(string parameterName, string changeTypes)
		{
			return new LocalizedString("ExInvalidNullParameterForChangeTypes", "Ex297F8D", false, true, ServerStrings.ResourceManager, new object[]
			{
				parameterName,
				changeTypes
			});
		}

		public static LocalizedString ExInvalidCustomSerializationData
		{
			get
			{
				return new LocalizedString("ExInvalidCustomSerializationData", "Ex166206", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionMaxBodyPartsExceeded(int maxbodyparts)
		{
			return new LocalizedString("ConversionMaxBodyPartsExceeded", "Ex3586B5", false, true, ServerStrings.ResourceManager, new object[]
			{
				maxbodyparts
			});
		}

		public static LocalizedString ConversionLimitsExceeded
		{
			get
			{
				return new LocalizedString("ConversionLimitsExceeded", "Ex993D9E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LargeMultivaluedPropertiesNotSupportedInTNEF
		{
			get
			{
				return new LocalizedString("LargeMultivaluedPropertiesNotSupportedInTNEF", "Ex331CDB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationSkippableStepSettingTargetAddress
		{
			get
			{
				return new LocalizedString("MigrationSkippableStepSettingTargetAddress", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString JunkEmailTrustedListXsoDuplicateException(string value)
		{
			return new LocalizedString("JunkEmailTrustedListXsoDuplicateException", "Ex1338C2", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString InvalidPropertyKey
		{
			get
			{
				return new LocalizedString("InvalidPropertyKey", "Ex29EB20", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TwoDays
		{
			get
			{
				return new LocalizedString("TwoDays", "Ex185917", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExOutlookSearchFolderDoesNotHaveMailboxSession
		{
			get
			{
				return new LocalizedString("ExOutlookSearchFolderDoesNotHaveMailboxSession", "ExEFC21C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotAuthenticateUserByTheClientSecurityContext(int error)
		{
			return new LocalizedString("CannotAuthenticateUserByTheClientSecurityContext", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ClientCulture_0x426
		{
			get
			{
				return new LocalizedString("ClientCulture_0x426", "Ex8ADF47", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MalformedAdrEntry
		{
			get
			{
				return new LocalizedString("MalformedAdrEntry", "Ex695734", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToFindIssuanceLicenseAndURI
		{
			get
			{
				return new LocalizedString("FailedToFindIssuanceLicenseAndURI", "ExB064E6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InboxRuleMessageTypeEncrypted
		{
			get
			{
				return new LocalizedString("InboxRuleMessageTypeEncrypted", "Ex5D23CD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SuffixMatchNotSupported
		{
			get
			{
				return new LocalizedString("SuffixMatchNotSupported", "ExE557D1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString idClientSessionInfoParseException
		{
			get
			{
				return new LocalizedString("idClientSessionInfoParseException", "Ex560AA8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExComparisonFilterPropertiesNotSupported(string property1, string property2)
		{
			return new LocalizedString("ExComparisonFilterPropertiesNotSupported", "ExC22DC7", false, true, ServerStrings.ResourceManager, new object[]
			{
				property1,
				property2
			});
		}

		public static LocalizedString CannotGetLongTermIdFromId(long id)
		{
			return new LocalizedString("CannotGetLongTermIdFromId", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString RecoverableItemsAccessDeniedException(string folder)
		{
			return new LocalizedString("RecoverableItemsAccessDeniedException", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				folder
			});
		}

		public static LocalizedString ClientCulture_0x1009
		{
			get
			{
				return new LocalizedString("ClientCulture_0x1009", "Ex1CD3B8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString OperationResultPartiallySucceeded
		{
			get
			{
				return new LocalizedString("OperationResultPartiallySucceeded", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UceContentFilterLoadFailure
		{
			get
			{
				return new LocalizedString("UceContentFilterLoadFailure", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiRulesError
		{
			get
			{
				return new LocalizedString("MapiRulesError", "Ex5FDC87", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorExTimeZoneValueMultipleGmtMatches(string matches)
		{
			return new LocalizedString("ErrorExTimeZoneValueMultipleGmtMatches", "ExACE841", false, true, ServerStrings.ResourceManager, new object[]
			{
				matches
			});
		}

		public static LocalizedString ExUnsupportedCharset(string charset)
		{
			return new LocalizedString("ExUnsupportedCharset", "Ex392060", false, true, ServerStrings.ResourceManager, new object[]
			{
				charset
			});
		}

		public static LocalizedString FolderRuleErrorInvalidGroup(string group)
		{
			return new LocalizedString("FolderRuleErrorInvalidGroup", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				group
			});
		}

		public static LocalizedString MigrationBatchStatusRemoving
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusRemoving", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExBadFolderEntryIdSize
		{
			get
			{
				return new LocalizedString("ExBadFolderEntryIdSize", "Ex240D07", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotSupportedSharingMessageException
		{
			get
			{
				return new LocalizedString("NotSupportedSharingMessageException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotGetIdFromLongTermId(string longTermId)
		{
			return new LocalizedString("CannotGetIdFromLongTermId", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				longTermId
			});
		}

		public static LocalizedString IncompleteExchangePrincipal
		{
			get
			{
				return new LocalizedString("IncompleteExchangePrincipal", "Ex1168AF", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationFeatureMultiBatch
		{
			get
			{
				return new LocalizedString("MigrationFeatureMultiBatch", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorXsoObjectPropertyValidationError(string name, string details)
		{
			return new LocalizedString("ErrorXsoObjectPropertyValidationError", "ExF0028C", false, true, ServerStrings.ResourceManager, new object[]
			{
				name,
				details
			});
		}

		public static LocalizedString MapiCannotSetTableColumns
		{
			get
			{
				return new LocalizedString("MapiCannotSetTableColumns", "ExB76470", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToReadActivityLog
		{
			get
			{
				return new LocalizedString("FailedToReadActivityLog", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorInvalidItemHoldPeriod(string name)
		{
			return new LocalizedString("ErrorInvalidItemHoldPeriod", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SearchOperationFailed
		{
			get
			{
				return new LocalizedString("SearchOperationFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProvisioningMailboxNotFound(string mailboxId)
		{
			return new LocalizedString("ProvisioningMailboxNotFound", "ExD455D2", false, true, ServerStrings.ResourceManager, new object[]
			{
				mailboxId
			});
		}

		public static LocalizedString ExDefaultJournalFilename
		{
			get
			{
				return new LocalizedString("ExDefaultJournalFilename", "ExC29219", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotExpandRow
		{
			get
			{
				return new LocalizedString("MapiCannotExpandRow", "Ex3FE48C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidBase64StringFormat(string base64String)
		{
			return new LocalizedString("ExInvalidBase64StringFormat", "Ex15A662", false, true, ServerStrings.ResourceManager, new object[]
			{
				base64String
			});
		}

		public static LocalizedString ExInvalidWABObjectType(object objectType)
		{
			return new LocalizedString("ExInvalidWABObjectType", "ExBD7ABF", false, true, ServerStrings.ResourceManager, new object[]
			{
				objectType
			});
		}

		public static LocalizedString ExCannotStartDeadSessionChecking
		{
			get
			{
				return new LocalizedString("ExCannotStartDeadSessionChecking", "ExA27A81", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchStateNotStarted
		{
			get
			{
				return new LocalizedString("SearchStateNotStarted", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SpellCheckerSwedish
		{
			get
			{
				return new LocalizedString("SpellCheckerSwedish", "ExC6A1C8", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotificationEmailSubjectExportPst
		{
			get
			{
				return new LocalizedString("NotificationEmailSubjectExportPst", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ContentRestrictionOnSearchKey
		{
			get
			{
				return new LocalizedString("ContentRestrictionOnSearchKey", "Ex8C4D5C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AggregatedMailboxNotFound(string guid)
		{
			return new LocalizedString("AggregatedMailboxNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				guid
			});
		}

		public static LocalizedString AnchorServerNotFound(string mdbGuid)
		{
			return new LocalizedString("AnchorServerNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				mdbGuid
			});
		}

		public static LocalizedString ReminderPropertyNotSupported(string itemTypeName, string propertyName)
		{
			return new LocalizedString("ReminderPropertyNotSupported", "ExF73C00", false, true, ServerStrings.ResourceManager, new object[]
			{
				itemTypeName,
				propertyName
			});
		}

		public static LocalizedString RpcServerParameterInvalidError(string error)
		{
			return new LocalizedString("RpcServerParameterInvalidError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				error
			});
		}

		public static LocalizedString ExUnknownFilterType
		{
			get
			{
				return new LocalizedString("ExUnknownFilterType", "ExF16ACD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x400A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x400A", "Ex6929EA", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSearchFolderAlreadyExists(object Guid)
		{
			return new LocalizedString("ExSearchFolderAlreadyExists", "Ex92E0FE", false, true, ServerStrings.ResourceManager, new object[]
			{
				Guid
			});
		}

		public static LocalizedString CannotVerifyDRMPropsSignatureUserNotFound(string sid)
		{
			return new LocalizedString("CannotVerifyDRMPropsSignatureUserNotFound", "Ex484EF1", false, true, ServerStrings.ResourceManager, new object[]
			{
				sid
			});
		}

		public static LocalizedString DataMoveReplicationConstraintSatisfiedForNonReplicatedDatabase(DataMoveReplicationConstraintParameter constraint, Guid databaseGuid)
		{
			return new LocalizedString("DataMoveReplicationConstraintSatisfiedForNonReplicatedDatabase", "Ex2A91E8", false, true, ServerStrings.ResourceManager, new object[]
			{
				constraint,
				databaseGuid
			});
		}

		public static LocalizedString ExCannotStampDefaultFolderId(string saveResult)
		{
			return new LocalizedString("ExCannotStampDefaultFolderId", "Ex206127", false, true, ServerStrings.ResourceManager, new object[]
			{
				saveResult
			});
		}

		public static LocalizedString UnableToLoadDrmMessage
		{
			get
			{
				return new LocalizedString("UnableToLoadDrmMessage", "Ex71096F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotParseValue
		{
			get
			{
				return new LocalizedString("ExCannotParseValue", "Ex2A507B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToRpcAcquireRacAndClc(string orgId, string status, string server)
		{
			return new LocalizedString("FailedToRpcAcquireRacAndClc", "Ex884C3B", false, true, ServerStrings.ResourceManager, new object[]
			{
				orgId,
				status,
				server
			});
		}

		public static LocalizedString ClientCulture_0x4001
		{
			get
			{
				return new LocalizedString("ClientCulture_0x4001", "Ex50E7A1", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidCacheEntryId(int id)
		{
			return new LocalizedString("InvalidCacheEntryId", "ExBE4A26", false, true, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString TeamMailboxMessageToLearnMore
		{
			get
			{
				return new LocalizedString("TeamMailboxMessageToLearnMore", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusCompleting
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusCompleting", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MapiCannotGetRowCount
		{
			get
			{
				return new LocalizedString("MapiCannotGetRowCount", "ExE2D314", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FolderRuleStageOnDeliveredMessage
		{
			get
			{
				return new LocalizedString("FolderRuleStageOnDeliveredMessage", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToDownloadCertificationMExData(Uri url)
		{
			return new LocalizedString("FailedToDownloadCertificationMExData", "Ex574FE8", false, true, ServerStrings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString MigrationGroupMembersAlreadyAvailable(string groupSmtpAddress)
		{
			return new LocalizedString("MigrationGroupMembersAlreadyAvailable", "ExDCCBA7", false, true, ServerStrings.ResourceManager, new object[]
			{
				groupSmtpAddress
			});
		}

		public static LocalizedString TimeZoneReferenceWithNullTimeZone(string timeZoneId)
		{
			return new LocalizedString("TimeZoneReferenceWithNullTimeZone", "ExDC929F", false, true, ServerStrings.ResourceManager, new object[]
			{
				timeZoneId
			});
		}

		public static LocalizedString ExInvalidBodyFormat(string format)
		{
			return new LocalizedString("ExInvalidBodyFormat", "ExE5F978", false, true, ServerStrings.ResourceManager, new object[]
			{
				format
			});
		}

		public static LocalizedString idUnableToAddDefaultTaskFolderToDefaultTaskGroup(string folderType)
		{
			return new LocalizedString("idUnableToAddDefaultTaskFolderToDefaultTaskGroup", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				folderType
			});
		}

		public static LocalizedString ReplayServiceDown(string serverName, string rpcErrorMessage)
		{
			return new LocalizedString("ReplayServiceDown", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				serverName,
				rpcErrorMessage
			});
		}

		public static LocalizedString FailedToAcquireTenantLicenses(string tenantId, string uri)
		{
			return new LocalizedString("FailedToAcquireTenantLicenses", "ExC3BC0A", false, true, ServerStrings.ResourceManager, new object[]
			{
				tenantId,
				uri
			});
		}

		public static LocalizedString ExModifiedOccurrenceCantHaveStartDateAsAdjacentOccurrence(object startDate, object adjacentStartDate)
		{
			return new LocalizedString("ExModifiedOccurrenceCantHaveStartDateAsAdjacentOccurrence", "ExE8F0D6", false, true, ServerStrings.ResourceManager, new object[]
			{
				startDate,
				adjacentStartDate
			});
		}

		public static LocalizedString MigrationBatchSupportedActionAppend
		{
			get
			{
				return new LocalizedString("MigrationBatchSupportedActionAppend", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExCannotOpenRejectedItem
		{
			get
			{
				return new LocalizedString("ExCannotOpenRejectedItem", "Ex92B024", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationBatchStatusFailed
		{
			get
			{
				return new LocalizedString("MigrationBatchStatusFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString VotingDataCorrupt
		{
			get
			{
				return new LocalizedString("VotingDataCorrupt", "Ex8B1234", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WrongTimeZoneReference(string timeZoneId)
		{
			return new LocalizedString("WrongTimeZoneReference", "ExCC0652", false, true, ServerStrings.ResourceManager, new object[]
			{
				timeZoneId
			});
		}

		public static LocalizedString MigrationTestMSAFailed
		{
			get
			{
				return new LocalizedString("MigrationTestMSAFailed", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxSyncStatusDocumentAndMembershipSyncFailure
		{
			get
			{
				return new LocalizedString("TeamMailboxSyncStatusDocumentAndMembershipSyncFailure", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExConstraintViolationByteArrayLengthTooLong(string propertyName, long maxLength, long actualLength)
		{
			return new LocalizedString("ExConstraintViolationByteArrayLengthTooLong", "Ex1A6202", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyName,
				maxLength,
				actualLength
			});
		}

		public static LocalizedString ExPropertyError(string propertyName, string propertyErrorCode, string providerDescription)
		{
			return new LocalizedString("ExPropertyError", "ExC0E5EC", false, true, ServerStrings.ResourceManager, new object[]
			{
				propertyName,
				propertyErrorCode,
				providerDescription
			});
		}

		public static LocalizedString ExInvalidChangeType(string changeType)
		{
			return new LocalizedString("ExInvalidChangeType", "ExF23224", false, true, ServerStrings.ResourceManager, new object[]
			{
				changeType
			});
		}

		public static LocalizedString PublicFoldersCannotBeMovedDuringMigration
		{
			get
			{
				return new LocalizedString("PublicFoldersCannotBeMovedDuringMigration", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ConversionNoReplayContent
		{
			get
			{
				return new LocalizedString("ConversionNoReplayContent", "Ex2F3847", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x404
		{
			get
			{
				return new LocalizedString("ClientCulture_0x404", "ExF292C6", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExSaveFailedBecauseOfConflicts(object itemId)
		{
			return new LocalizedString("ExSaveFailedBecauseOfConflicts", "ExE0CF58", false, true, ServerStrings.ResourceManager, new object[]
			{
				itemId
			});
		}

		public static LocalizedString SharingFolderNameWithSuffix(string folderName, int suffix)
		{
			return new LocalizedString("SharingFolderNameWithSuffix", "Ex9CF231", false, true, ServerStrings.ResourceManager, new object[]
			{
				folderName,
				suffix
			});
		}

		public static LocalizedString ExPropertyNotStreamable(string property)
		{
			return new LocalizedString("ExPropertyNotStreamable", "Ex170CD7", false, true, ServerStrings.ResourceManager, new object[]
			{
				property
			});
		}

		public static LocalizedString MigrationInvalidTargetProxyAddress(string email)
		{
			return new LocalizedString("MigrationInvalidTargetProxyAddress", "Ex61F981", false, true, ServerStrings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString ExCantSubmitWithoutRecipients
		{
			get
			{
				return new LocalizedString("ExCantSubmitWithoutRecipients", "Ex11767B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LastErrorMessage
		{
			get
			{
				return new LocalizedString("LastErrorMessage", "ExC64014", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RepairingIsNotSetSinceMonitorEntryIsNotFound(string monitorName, string targetResource)
		{
			return new LocalizedString("RepairingIsNotSetSinceMonitorEntryIsNotFound", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				monitorName,
				targetResource
			});
		}

		public static LocalizedString JunkEmailFolderNotFoundException
		{
			get
			{
				return new LocalizedString("JunkEmailFolderNotFoundException", "Ex954A7C", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x42A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x42A", "Ex2E1237", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PublicFolderConnectionThreadLimitExceeded(int limit)
		{
			return new LocalizedString("PublicFolderConnectionThreadLimitExceeded", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				limit
			});
		}

		public static LocalizedString AmMoveNotApplicableForDbException
		{
			get
			{
				return new LocalizedString("AmMoveNotApplicableForDbException", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TeamMailboxMessageMemberInvitationSubject(string tmName)
		{
			return new LocalizedString("TeamMailboxMessageMemberInvitationSubject", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				tmName
			});
		}

		public static LocalizedString ExInvalidValueForFlagsCalculatedProperty(int flag)
		{
			return new LocalizedString("ExInvalidValueForFlagsCalculatedProperty", "ExC2422A", false, true, ServerStrings.ResourceManager, new object[]
			{
				flag
			});
		}

		public static LocalizedString ADUserNotFoundId(object id)
		{
			return new LocalizedString("ADUserNotFoundId", "Ex62D59A", false, true, ServerStrings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString ExCantUndeleteOccurrence
		{
			get
			{
				return new LocalizedString("ExCantUndeleteOccurrence", "Ex4D7D4E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusQueued
		{
			get
			{
				return new LocalizedString("MigrationUserStatusQueued", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUrlScheme(ServiceType type, Uri url)
		{
			return new LocalizedString("InvalidUrlScheme", "ExBC6F98", false, true, ServerStrings.ResourceManager, new object[]
			{
				type,
				url
			});
		}

		public static LocalizedString ExTypeSerializationNotSupported(Type type)
		{
			return new LocalizedString("ExTypeSerializationNotSupported", "Ex17D281", false, true, ServerStrings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString DateRangeSixMonths
		{
			get
			{
				return new LocalizedString("DateRangeSixMonths", "Ex446F58", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationMRSJobMissing(string identity)
		{
			return new LocalizedString("MigrationMRSJobMissing", "Ex72DB06", false, true, ServerStrings.ResourceManager, new object[]
			{
				identity
			});
		}

		public static LocalizedString FailedToCheckPublishLicenseOwnership(string organizationId)
		{
			return new LocalizedString("FailedToCheckPublishLicenseOwnership", "Ex30E3D1", false, true, ServerStrings.ResourceManager, new object[]
			{
				organizationId
			});
		}

		public static LocalizedString MigrationSkippableStepNone
		{
			get
			{
				return new LocalizedString("MigrationSkippableStepNone", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CannotSynchronizeManifestEx(Type mapiManifestType, object clientPhase, object manifestPhase)
		{
			return new LocalizedString("CannotSynchronizeManifestEx", "Ex0673D0", false, true, ServerStrings.ResourceManager, new object[]
			{
				mapiManifestType,
				clientPhase,
				manifestPhase
			});
		}

		public static LocalizedString MalformedCommentRestriction
		{
			get
			{
				return new LocalizedString("MalformedCommentRestriction", "ExEF5D98", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x427
		{
			get
			{
				return new LocalizedString("ClientCulture_0x427", "Ex2F7231", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MigrationUserStatusSummaryActive
		{
			get
			{
				return new LocalizedString("MigrationUserStatusSummaryActive", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString StoreDataInvalid(string value)
		{
			return new LocalizedString("StoreDataInvalid", "Ex981545", false, true, ServerStrings.ResourceManager, new object[]
			{
				value
			});
		}

		public static LocalizedString ErrorEmptyFolderNotSupported
		{
			get
			{
				return new LocalizedString("ErrorEmptyFolderNotSupported", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x540A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x540A", "Ex13031F", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0x2C0A
		{
			get
			{
				return new LocalizedString("ClientCulture_0x2C0A", "Ex774EC0", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MissingOperand
		{
			get
			{
				return new LocalizedString("MissingOperand", "ExCCBD0D", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DuplicateAction
		{
			get
			{
				return new LocalizedString("DuplicateAction", "Ex69B50E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExTooManyInstancesOnSeries(uint maxNumberOfInstances)
		{
			return new LocalizedString("ExTooManyInstancesOnSeries", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				maxNumberOfInstances
			});
		}

		public static LocalizedString SearchStateQueued
		{
			get
			{
				return new LocalizedString("SearchStateQueued", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExQueryPropertyBagRowNotSet
		{
			get
			{
				return new LocalizedString("ExQueryPropertyBagRowNotSet", "ExE6FCB3", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString Sunday
		{
			get
			{
				return new LocalizedString("Sunday", "Ex74DD10", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString WeatherUnitDefault
		{
			get
			{
				return new LocalizedString("WeatherUnitDefault", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RequestSecurityTokenException
		{
			get
			{
				return new LocalizedString("RequestSecurityTokenException", "Ex878A42", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidAddressError(string legacyDn)
		{
			return new LocalizedString("InvalidAddressError", "", false, false, ServerStrings.ResourceManager, new object[]
			{
				legacyDn
			});
		}

		public static LocalizedString ExGetNotSupportedForCalculatedProperty(object proptertyID)
		{
			return new LocalizedString("ExGetNotSupportedForCalculatedProperty", "ExE56D7A", false, true, ServerStrings.ResourceManager, new object[]
			{
				proptertyID
			});
		}

		public static LocalizedString ExNoOccurrencesInRecurrence
		{
			get
			{
				return new LocalizedString("ExNoOccurrencesInRecurrence", "Ex318F74", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidMclXml
		{
			get
			{
				return new LocalizedString("ExInvalidMclXml", "ExFA882B", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExInvalidIdFormat
		{
			get
			{
				return new LocalizedString("ExInvalidIdFormat", "ExE0A13E", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExAdminAuditLogsFolderAccessDenied
		{
			get
			{
				return new LocalizedString("ExAdminAuditLogsFolderAccessDenied", "ExAFE4DD", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ClientCulture_0xC04
		{
			get
			{
				return new LocalizedString("ClientCulture_0xC04", "ExD84C19", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ExTooManyObjects(string objectName, int count, int limits)
		{
			return new LocalizedString("ExTooManyObjects", "Ex47A925", false, true, ServerStrings.ResourceManager, new object[]
			{
				objectName,
				count,
				limits
			});
		}

		public static LocalizedString ExInvalidComparisionOperatorInPropertyComparisionFilter
		{
			get
			{
				return new LocalizedString("ExInvalidComparisionOperatorInPropertyComparisionFilter", "ExBCC9DB", false, true, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AsyncOperationTypeExportPST
		{
			get
			{
				return new LocalizedString("AsyncOperationTypeExportPST", "", false, false, ServerStrings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(ServerStrings.IDs key)
		{
			return new LocalizedString(ServerStrings.stringIDs[(uint)key], ServerStrings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(971);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.Data.Storage.ServerStrings", typeof(ServerStrings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			MissingRightsManagementLicense = 2721128427U,
			ServerLocatorServiceTransientFault = 195662054U,
			EmailAddressMissing = 824541142U,
			CannotShareSearchFolder = 2906390296U,
			EstimateStateStopping = 1390813357U,
			SpellCheckerDutch = 1028519129U,
			SpellCheckerNorwegianNynorsk = 2064592175U,
			MigrationFlagsStart = 307085623U,
			TeamMailboxMessageSiteAndSiteMailboxDetails = 4243709143U,
			CannotGetSupportedRoutingTypes = 3398846764U,
			ClientCulture_0x816 = 4287963345U,
			AsyncOperationTypeMailboxRestore = 1588035907U,
			MatchContainerClassValidationFailed = 1805838968U,
			ExCannotCreateSubfolderUnderSearchFolder = 3004251640U,
			InboxRuleImportanceHigh = 161866662U,
			MapiCopyFailedProperties = 1003186114U,
			ClientCulture_0x3C0A = 1489962323U,
			ErrorTeamMailboxGetUsersNullResponse = 2738718017U,
			MigrationBatchSupportedActionNone = 2367323544U,
			ExAuditsUpdateDenied = 2198921793U,
			ExBadMessageEntryIdSize = 2134621305U,
			ExAdminAuditLogsUpdateDenied = 2993889924U,
			ExInvalidNumberOfOccurrences = 949829336U,
			OleConversionFailed = 1342791165U,
			ClientCulture_0x3401 = 1551862906U,
			ExCannotReadFolderData = 346824164U,
			InboxRuleSensitivityNormal = 1533116792U,
			SpellCheckerCatalan = 2788778759U,
			TeamMailboxMessageHowToGetStarted = 894291518U,
			ExInvalidMasterValueAndColumnLength = 2688169016U,
			MigrationBatchStatusStopping = 1002199416U,
			ClientCulture_0x440A = 2264323463U,
			ExFolderAlreadyExistsInClientState = 3064105318U,
			InvalidPermissionsEntry = 3192255565U,
			ConversionInternalFailure = 753703675U,
			MigrationTypeExchangeOutlookAnywhere = 150715741U,
			ClientCulture_0x4809 = 630982379U,
			MigrationAutodiscoverConfigurationFailure = 2605826718U,
			ExDefaultContactFilename = 273958411U,
			TeamMailboxMessageReopenClosedSiteMailbox = 687935806U,
			MigrationObjectsCountStringPFs = 3547809333U,
			ExCannotCreateRecurringMeetingWithoutTimeZone = 1305341899U,
			ExInvalidSaveOnCorrelatedItem = 1790240390U,
			ErrorTeamMailboxGetListItemChangesNullResponse = 2801555237U,
			ExCorruptedTimeZone = 3339597578U,
			MigrationUserStatusSummaryStopped = 1540857282U,
			InboxRuleSensitivityCompanyConfidential = 113768464U,
			FailedToAddAttachments = 3597498295U,
			MapiCannotDeliverItem = 379773582U,
			MapiCannotGetLocalRepIds = 2576522508U,
			ClientCulture_0x3C01 = 3647547459U,
			FirstDay = 209873198U,
			ClientCulture_0x41D = 4287963599U,
			PersonIsAlreadyLinkedWithGALContact = 3944203608U,
			InboxRuleMessageTypeCalendaring = 303788285U,
			Editor = 3653626825U,
			CannotShareOtherPersonalFolder = 4226852782U,
			TeamMailboxMessageClosedBodyIntroText = 540789371U,
			InboxRuleMessageTypeSigned = 2800053981U,
			MigrationTypePSTImport = 1894870732U,
			DateRangeOneYear = 179758620U,
			ExAuditsFolderAccessDenied = 1682339598U,
			ClientCulture_0x1409 = 1908093332U,
			NoExternalOwaAvailableException = 3485169071U,
			DelegateNotSupportedFolder = 60274512U,
			MigrationBatchDirectionLocal = 3906173474U,
			ErrorFolderDeleted = 3130219463U,
			BadDateTimeFormatInChangeDate = 3911406152U,
			ClientCulture_0x1C0A = 1489962389U,
			TeamMailboxMessageNoActionText = 2011085458U,
			InvalidMigrationBatchId = 4201517212U,
			FolderRuleStageOnPromotedMessage = 3924992590U,
			idUnableToCreateDefaultTaskGroupException = 1870702413U,
			PublicFolderStartSyncFolderHierarchyRpcFailed = 194841891U,
			SearchStateFailed = 149719578U,
			ClientCulture_0x2401 = 1551862809U,
			MapiCannotWritePerUserInformation = 3503392803U,
			SearchNameCharacterConstraint = 2750670341U,
			InboxRuleMessageTypeCalendaringResponse = 4022186804U,
			ErrorTimeProposalInvalidWhenNotAllowedByOrganizer = 2114183459U,
			RequestStateCertExpired = 981561289U,
			Thursday = 1760294240U,
			MapiCannotDeleteAttachment = 1553527118U,
			ExEventsNotSupportedForDelegateUser = 1512287666U,
			AsyncOperationTypeImportPST = 4181674605U,
			MigrationStepDataMigration = 4056598008U,
			JunkEmailBlockedListXsoTooManyException = 2239334052U,
			ClientCulture_0x409 = 2721879535U,
			ErrorFolderCreationIsBlocked = 3828388569U,
			ExInvalidParticipantEntryId = 3332812272U,
			ExInvalidSpecifierValueError = 3512980427U,
			TeamMailboxSyncStatusDocumentSyncFailureOnly = 4064061502U,
			SearchStateInProgress = 2549870927U,
			JunkEmailInvalidOperationException = 2171204805U,
			TeamMailboxMessageWhatIsSiteMailbox = 1029618557U,
			SpellCheckerFrench = 2603495179U,
			ExUnknownRecurrenceBlobRange = 192606181U,
			ExAttachmentCannotOpenDueToUnSave = 1753710770U,
			ClientCulture_0x439 = 3125164062U,
			SpellCheckerEnglishCanada = 3837865737U,
			MapiCannotUpdateDeferredActionMessages = 3771526820U,
			MigrationStatisticsCompleteStatus = 2014714852U,
			OleConversionInvalidResultType = 737023398U,
			UnableToMakeAutoDiscoveryRequest = 3684140834U,
			NotificationEmailSubjectImportPst = 4266601947U,
			SharingMessageAttachmentNotFoundException = 1338408148U,
			MigrationBatchStatusIncrementalSyncing = 2642389795U,
			SearchStateStopped = 4084900412U,
			PublicFolderMailboxNotFound = 2354834546U,
			MapiCannotGetReceiveFolder = 3021899907U,
			MigrationUserStatusStarting = 445051701U,
			AmDbMountNotAllowedDueToLossException = 2210183777U,
			SpellCheckerGermanPreReform = 770684443U,
			InvalidKindFormat = 2412724648U,
			OleUnableToReadAttachment = 905358579U,
			InvalidModifier = 2666546556U,
			MigrationUserStatusProvisionUpdating = 3840447572U,
			MigrationMailboxPermissionFullAccess = 594875070U,
			WeatherUnitCelsius = 3928045320U,
			NullDateInChangeDate = 2953752128U,
			ClientCulture_0x2C09 = 4003777722U,
			MigrationBatchDirectionOffboarding = 2084642916U,
			CalNotifTypeUninteresting = 194112229U,
			AmDatabaseNeverMountedException = 2088104068U,
			MapiCannotSetReceiveFolder = 870651383U,
			RpcClientWrapperFailedToLoadTopology = 3000352360U,
			ExCorruptConversationActionItem = 3609586398U,
			Tuesday = 2820941203U,
			MapiCannotCreateRestriction = 3647326574U,
			CorruptJunkMoveStamp = 2969695521U,
			InvalidAttachmentNumber = 599296811U,
			ClientCulture_0x83E = 3125164046U,
			Friday = 4094875965U,
			NoServerValueAvailable = 1586627622U,
			DelegateInvalidPermission = 1690131397U,
			OperationAborted = 1769372998U,
			DiscoveryMailboxNotFound = 380587417U,
			ClientCulture_0x422 = 1559080126U,
			MigrationUserStatusSummarySynced = 434577051U,
			FourHours = 656727185U,
			MigrationUserStatusCompleted = 3033708196U,
			ExVotingBlobCorrupt = 3797097900U,
			HexadecimalHtmlColorPatternDescription = 912807925U,
			MigrationStepInitialization = 3420516204U,
			TeamMailboxSyncStatusMembershipSyncFailureOnly = 3118133575U,
			InvalidEncryptedSharedFolderDataException = 788681379U,
			ExCorruptRestrictionFilter = 3198793348U,
			ErrorNotificationAlreadyExists = 3806649575U,
			ExItemIsOpenedInReadOnlyMode = 1743342709U,
			UnbalancedParenthesis = 2479239329U,
			InvalidRpMsgFormat = 3259603877U,
			UserPhotoNotAnImage = 586169060U,
			MapiCannotCreateEntryIdFromShortTermId = 1079495974U,
			ClientCulture_0x807 = 2721879405U,
			MapiCannotCreateBookmark = 3407343648U,
			InvalidateDateRange = 630534408U,
			MigrationUserAdminTypeUnknown = 1290997994U,
			CannotMoveOrCopyBetweenPrivateAndPublicMailbox = 2519000101U,
			SpellCheckerNorwegianBokmal = 2234262993U,
			ClientCulture_0x1007 = 3279312390U,
			MigrationBatchSupportedActionStop = 3025564610U,
			CrossForestNotSupported = 1295345518U,
			ExCannotAccessSystemFolderId = 2475132438U,
			ClientCulture_0x410 = 4287963483U,
			ExStatefulFilterMustBeSetWhenSetSyncFiltersIsInvokedWithNullFilter = 153308512U,
			MapiCannotReadPermissions = 2948566116U,
			FolderRuleStageOnPublicFolderBefore = 3983802523U,
			UnbalancedQuote = 3551282863U,
			FailedToWriteActivityLog = 2092669490U,
			NoFreeBusyFolder = 3948653022U,
			ClientCulture_0x408 = 2721879534U,
			SharingConflictException = 4050068885U,
			MapiCannotQueryColumns = 329227683U,
			ExCaughtMapiExceptionWhileReadingEvents = 914093861U,
			ClientCulture_0x81D = 4287963459U,
			ConversionInvalidSmimeContent = 1324407795U,
			ExCannotRevertSentMeetingToAppointment = 1786112539U,
			ExMustSaveFolderToMakeVisibleToOutlook = 1566523742U,
			UnsupportedContentRestriction = 309976908U,
			ClientCulture_0x42D = 1559080244U,
			NotificationEmailBodyImportPSTCreated = 443718431U,
			SearchTargetInSource = 59886577U,
			ExAddItemAttachmentFailed = 809558091U,
			ClientCulture_0x403 = 2721879541U,
			ExCannotMoveOrDeleteDefaultFolders = 1083109339U,
			ExCannotSeekRow = 3923324564U,
			MigrationReportBatch = 3620611820U,
			ExErrorInDetectE15Store = 2187234323U,
			idDefaultFoldersNotLocalizedException = 2391045796U,
			MigrationStateCompleted = 2969986172U,
			ErrorMissingMailboxOrPermission = 297411134U,
			ClientCulture_0x140C = 1101524278U,
			MigrationTypeIMAP = 2192010005U,
			ClientCulture_0x2809 = 630982445U,
			ClientCulture_0x1404 = 1955147499U,
			ExCannotRejectDeletes = 984546473U,
			TeamMailboxSyncStatusMaintenanceSyncFailureOnly = 415920568U,
			MigrationStateStopped = 564935456U,
			MigrationStageDiscovery = 3735242122U,
			ClientCulture_0x40A = 2721879655U,
			NotificationEmailBodyCertExpired = 3791574374U,
			ExCannotRejectSameOperationTwice = 1657454002U,
			ExCannotGetSearchCriteria = 3371920351U,
			ExInvalidMaxQueueSize = 3800790638U,
			ADException = 1437123480U,
			ExNoMailboxOwner = 653269905U,
			ExNotConnected = 4002989775U,
			SearchStateStopping = 478700673U,
			SpellCheckerKorean = 1750841907U,
			MigrationTypeExchangeLocalMove = 1435478051U,
			MapiCannotSubmitMessage = 2120544431U,
			ClientCulture_0x1C09 = 4003777885U,
			ExInvalidOrder = 1458958954U,
			NoProviderSupportShareFolder = 725138746U,
			ExConnectionCacheSizeNotSet = 3332361397U,
			MigrationFlagsRemove = 4100720225U,
			ExInvalidRecipient = 2084653907U,
			ExFoundInvalidRowType = 1532066664U,
			ExInvalidOffset = 3156797033U,
			NotEnoughPermissionsToPerformOperation = 2795849522U,
			MigrationStatisticsPartiallyCompleteStatus = 3321416304U,
			TeamMailboxSyncStatusNotAvailable = 1147224948U,
			InvalidOperator = 3791300057U,
			DefaultHtmlAttachmentHrefText = 1063299331U,
			ConversionBodyCorrupt = 2883857825U,
			ClientCulture_0x1407 = 2358432026U,
			MapiCannotSaveChanges = 2091534526U,
			RejectedSuggestionPersonIdSameAsPersonId = 1712595778U,
			ErrorInvalidPhoneNumberFormat = 2860711473U,
			MigrationStateCorrupted = 1551718751U,
			ProvisioningRequestCsvContainsNeitherPasswordNorFederatedIdentity = 3286105202U,
			SecurityPrincipalAlreadyDefined = 3657645519U,
			KqlParseException = 968748330U,
			ExEventNotFound = 2414329026U,
			ThreeDays = 1227907325U,
			ExInvalidSortLength = 278097122U,
			MapiCannotGetPerUserLongTermIds = 3752161350U,
			ExFolderWithoutMapiProp = 2656172439U,
			ExChangeKeyTooLong = 3755862658U,
			ExUnknownRestrictionType = 2902058685U,
			ExInvalidRowCount = 91197877U,
			UnsupportedExistRestriction = 2720991162U,
			AvailabilityOnly = 4121599705U,
			MapiCannotExecuteWithInternalAccess = 1399515408U,
			ExItemNoParentId = 2879753762U,
			MigrationTypePublicFolder = 1107465087U,
			MapiCannotGetPerUserGuid = 130796183U,
			FederationNotEnabled = 2521346493U,
			RequestStateWaitingForFinalization = 241258410U,
			RequestStateCompleted = 1176987915U,
			TooManyCultures = 493948276U,
			MapiCannotSetCollapseState = 3358444168U,
			IncompleteUserInformationToAccessGroupMailbox = 1834533699U,
			ClientCulture_0x2001 = 2116512813U,
			CannotImportMessageChange = 1016962785U,
			InvalidTimesInTimeSlot = 3299931409U,
			MigrationReportFinalizationFailure = 3086195034U,
			StructuredQueryException = 3050947916U,
			ExUnknownResponseType = 628804796U,
			RequestStateCreated = 2214930724U,
			ExInvalidComparisonOperatorInComparisonFilter = 2474224463U,
			MigrationFolderSettings = 3920537899U,
			ClientCulture_0x809 = 2721879419U,
			ExUnsupportedSeekReference = 3653170115U,
			MigrationBatchStatusCompleted = 3031733457U,
			MigrationTestMSAWarning = 713262539U,
			InvalidDateTimeRange = 2595969499U,
			MapiCannotGetMapiTable = 2125622397U,
			MapiCannotCheckForNotifications = 1130047631U,
			CannotStamplocalFreeBusyId = 977128087U,
			ClientCulture_0x100A = 2828973696U,
			MigrationBatchStatusSynced = 1893783426U,
			ExchangePrincipalFromMailboxDataError = 2428434059U,
			MigrationUserStatusIncrementalFailed = 3615217052U,
			InvalidXml = 3321965778U,
			ExEntryIdToLong = 223559137U,
			ClientCulture_0x420 = 1559080128U,
			PrincipalFromDifferentSite = 697160562U,
			ErrorSavingRules = 3415665237U,
			PublishedFolderAccessDeniedException = 1802620674U,
			PublicFoldersNotEnabledForEnterprise = 305670746U,
			InboxRuleMessageTypeApprovalRequest = 3382673089U,
			NonUniqueRecipientError = 4042709143U,
			ExSystemFolderAccessDenied = 3548482161U,
			MapiCannotRemoveNotification = 3379789351U,
			ClientCulture_0x180A = 1699673688U,
			ExCommentFilterPropertiesNotSupported = 3770139034U,
			ExDictionaryDataCorruptedNullKey = 2450087029U,
			MigrationBatchStatusStarting = 3165677092U,
			ClientCulture_0x300A = 2828973630U,
			ExBadValueForTypeCode0 = 1987097643U,
			ErrorTimeProposalInvalidOnRecurringMaster = 510893832U,
			SearchStateDeletionInProgress = 2114464927U,
			ExRuleIdInvalid = 1600317547U,
			MapiCannotCollapseRow = 3959385997U,
			SharingUnableToGenerateEncryptedSharedFolderData = 3606405084U,
			ExConnectionNotCached = 3825572190U,
			CVSPopulationTimedout = 931793818U,
			BadDateFormatInChangeDate = 300160841U,
			MigrationBatchStatusCompletedWithErrors = 391772390U,
			NotReadSubjectPrefix = 765916263U,
			MapiCannotFinishSubmit = 1428661747U,
			ClientCulture_0xC01 = 2721875976U,
			ExItemNotFoundInClientManifest = 2811676186U,
			ErrorNoStoreObjectId = 2612888606U,
			CalendarItemCorrelationFailed = 2194255608U,
			ExInvalidOccurrenceId = 260083086U,
			DateRangeOneWeek = 2814533567U,
			EnforceRulesQuota = 2512850293U,
			ExInvalidMonth = 1837976028U,
			MigrationUserStatusCompletionSynced = 3370782263U,
			FirstFullWeek = 1880367925U,
			MigrationFeatureEndpoints = 3254542318U,
			ExNoSearchHasBeenInitiated = 1291493387U,
			MigrationUserStatusIncrementalSyncing = 1916012218U,
			PublicFolderMailboxesCannotBeCreatedDuringMigration = 2113494780U,
			MapiCannotCreateFilter = 2841260818U,
			MapiCannotNotifyMessageNewMail = 932673369U,
			MigrationUserStatusSyncing = 1644908980U,
			MigrationBatchFlagForceNewMigration = 2220521549U,
			CannotGetFinalStateSynchronizerProviderBase = 2553874506U,
			ServerLocatorClientWCFCallCommunicationError = 2057374892U,
			ExValueCannotBeNull = 553140815U,
			ClientCulture_0x3009 = 2472743270U,
			MigrationTypeBulkProvisioning = 2764365677U,
			ErrorFolderIsMailEnabled = 156083260U,
			ExCantAccessOccurrenceFromNewItem = 2030923649U,
			ConversionCorruptContent = 1309170924U,
			AutoDFailedToGetToken = 2021640990U,
			ExCorruptPropertyTag = 4205544601U,
			InvalidTimeSlot = 77158404U,
			ExCannotOpenMultipleCorrelatedItems = 3242781799U,
			ErrorLanguageIsNull = 902677431U,
			ExInvalidAcrBaseProfiles = 372387097U,
			ExMustSaveFolderToApplySearch = 2802946662U,
			ExReadTopologyTimeout = 2753071453U,
			ExUnknownRecurrenceBlobType = 3333234210U,
			ClientCulture_0x419 = 4287963476U,
			SpellCheckerHebrew = 113369212U,
			ClientCulture_0x1001 = 2116512976U,
			InvalidAttachmentId = 1820555509U,
			ClientCulture_0x43F = 3125164183U,
			ExInvalidFolderId = 899245499U,
			AmDbMountNotAllowedDueToRegistryConfigurationException = 1012814431U,
			CannotSaveReadOnlyAttachment = 911781703U,
			InvalidTnef = 3787888884U,
			MigrationUserStatusIncrementalSynced = 392648307U,
			ExAdminAuditLogsDeleteDenied = 2071885718U,
			ConversionInvalidMessageCodepageCharset = 2114545814U,
			ClientCulture_0x40C = 2721879653U,
			DumpsterStatusShutdownException = 2379279263U,
			CannotDeleteRootFolder = 1345910406U,
			MapiCannotGetEffectiveRights = 1856386122U,
			InvalidMechanismToAccessGroupMailbox = 3136620084U,
			MapiCannotSavePermissions = 3536723681U,
			ClientCulture_0x1004 = 2876027863U,
			MigrationBatchStatusCreated = 3588257392U,
			NotAllowedExternalSharingByPolicy = 2776843979U,
			InboxRuleMessageTypeReadReceipt = 1311410701U,
			StoreOperationFailed = 1490728717U,
			ErrorExTimeZoneValueNoGmtMatch = 33074401U,
			ExStoreObjectValidationError = 3552080970U,
			MigrationBatchFlagNone = 1010688174U,
			ClientCulture_0x4009 = 2472743107U,
			TooManyAttachmentsOnProtectedMessage = 3188685413U,
			PublicFolderOpenFailedOnExistingFolder = 3397519574U,
			ExInvalidSortOrder = 1522765994U,
			ReplyRuleNotSupportedOnNonMailPublicFolder = 3678175251U,
			ExGetPropsFailed = 2626172332U,
			EstimateStateSucceeded = 2344409522U,
			MigrationBatchSupportedActionRemove = 2621679806U,
			MapiCannotSaveMessageStream = 2545687272U,
			MapiInvalidId = 3515331893U,
			ContactLinkingMaximumNumberOfContactsPerPersonError = 2975245987U,
			ConversionUnsupportedContent = 2827742042U,
			MigrationUserStatusIncrementalStopped = 4287122418U,
			MapiCannotCreateMessage = 955524647U,
			InvalidSendAddressIdentity = 968419245U,
			ClientCulture_0x425 = 1559080133U,
			DisposeOOFHistoryFolder = 2559314081U,
			ExCantDeleteLastOccurrence = 1115353353U,
			MigrationReportBatchSuccess = 3283674777U,
			ErrorAccessingLargeProperty = 4284023892U,
			OperationNotSupportedOnPublicFolderMailbox = 208132458U,
			ExCannotCreateMeetingCancellation = 872597966U,
			MigrationFeaturePAW = 3533048780U,
			InboxRuleFlagStatusFlagged = 2850981798U,
			JunkEmailBlockedListXsoNullException = 135679156U,
			ClientCulture_0x43E = 3125164186U,
			TeamMailboxMessageGoToYourGroupSite = 1695521020U,
			ClientCulture_0x81A = 4287963464U,
			CannotImportMessageMove = 2076121062U,
			FifteenMinutes = 1860805300U,
			OneDays = 823631819U,
			CorruptNaturalLanguageProperty = 1680647297U,
			DumpsterStatusAlreadyStartedException = 2312979504U,
			ExCannotSetSearchCriteria = 4217834195U,
			ExBadObjectType = 192747935U,
			SpellCheckerFinnish = 298653586U,
			MigrationBatchStatusWaiting = 3098387483U,
			UnsupportedKindKeywords = 4038623627U,
			ClientCulture_0x407 = 2721879545U,
			PropertyChangeMetadataParseError = 2516633257U,
			SyncFailedToCreateNewItemOrBindToExistingOne = 2642058832U,
			ConversionFailedInvalidMacBin = 3516269798U,
			SpellCheckerEnglishUnitedStates = 435552220U,
			ExContactHasNoId = 1551474375U,
			ErrorExTimeZoneValueTimeZoneNotFound = 2860195473U,
			SpellCheckerGermanPostReform = 1337636428U,
			InboxRuleMessageTypePermissionControlled = 851950942U,
			ClientCulture_0x40F = 2721879656U,
			PropertyDefinitionsValuesNotMatch = 1310001827U,
			ClientCulture_0xC1A = 4287959997U,
			DateRangeThreeMonths = 2706950694U,
			ExConnectionAlternate = 3038510311U,
			MigrationBatchSupportedActionStart = 2795037022U,
			ClientCulture_0x402 = 2721879540U,
			ExCannotAccessAdminAuditLogsFolderId = 3256433766U,
			ClientCulture_0x424 = 1559080132U,
			MigrationStateWaiting = 132681062U,
			MigrationStageProcessing = 2224931539U,
			Database = 662607817U,
			MapiCannotGetTransportQueueFolderId = 1899965635U,
			UnsupportedAction = 252713015U,
			FolderRuleErrorInvalidRecipientEntryId = 305850519U,
			TeamMailboxMessageGoToTheSite = 2536777203U,
			TwelveHours = 834480874U,
			MigrationStageInjection = 1710892423U,
			MapiCannotGetContentsTable = 3729217742U,
			EstimateStateStopped = 3285920218U,
			NullWorkHours = 1888800485U,
			MigrationUserStatusCompleting = 3088889153U,
			FiveMinutes = 3726325313U,
			InboxRuleMessageTypeVoicemail = 3449171400U,
			SpellCheckerPortugueseBrasil = 3412096701U,
			GenericFailureRMDecryption = 561027979U,
			SpellCheckerEnglishAustralia = 312177309U,
			NoDeferredActions = 3160415695U,
			ErrorSetDateTimeFormatWithoutLanguage = 3022558012U,
			ClientCulture_0x1801 = 987212968U,
			MapiErrorParsingId = 2113411584U,
			MigrationUserAdminTypePartnerTenant = 563787386U,
			MigrationUserStatusStopped = 3598152156U,
			MigrationReportBatchFailure = 909292782U,
			MapiCannotCreateAttachment = 490979673U,
			NotificationEmailBodyCertExpiring = 1946505183U,
			MapiCannotReadPerUserInformation = 2470109498U,
			ExInvalidSubFilterProperty = 2189690271U,
			StockReplyTemplate = 1931248018U,
			CalNotifTypeSummary = 3362131860U,
			JunkEmailInvalidConstructionException = 633339293U,
			MapiCannotCreateAssociatedMessage = 3309334123U,
			ClientCulture_0x413 = 4287963482U,
			MapiCannotSortTable = 1706900424U,
			MigrationUserStatusStopping = 2822102721U,
			MapiCannotGetRecipientTable = 1778962839U,
			ExInvalidCallToTryUpdateCalendarItem = 2932685544U,
			ClientCulture_0x406 = 2721879544U,
			ExCannotAccessAuditsFolderId = 102022121U,
			ExReadEventsFailed = 18047843U,
			ExCannotQueryAssociatedTable = 727433418U,
			ClientCulture_0x429 = 1559080121U,
			MigrationStepProvisioningUpdate = 2303146172U,
			TwoWeeks = 1311803567U,
			MigrationFeatureUpgradeBlock = 4021122445U,
			ExInvalidServiceType = 1530529173U,
			NullTimeInChangeDate = 3873491039U,
			ConversionInvalidSmimeClearSignedContent = 1192664468U,
			RequestStateSuspended = 2464640173U,
			MapiIsFromPublicStoreCheckFailed = 2393270488U,
			ExCannotSendMeetingMessages = 4172275237U,
			ExAuditsDeleteDenied = 2982637405U,
			ClientCulture_0x416 = 4287963485U,
			MissingPropertyValue = 4113710984U,
			FolderNotPublishedException = 3202920824U,
			ServerLocatorClientEndpointNotFoundException = 1807895935U,
			ExTooComplexGroupSortParameter = 988424741U,
			MapiCannotLookupEntryId = 220684915U,
			NotificationEmailBodyExportPSTCreated = 543772960U,
			TeamMailboxMessageReactivatedBodyIntroText = 1291339327U,
			NotSupportedWithMailboxVersionException = 1027067688U,
			ClientCulture_0x3409 = 1908093266U,
			ClientCulture_0x41B = 4287963593U,
			CannotAddAttachmentToReadOnlyCollection = 1892124050U,
			UserPhotoPreviewNotFound = 1940443510U,
			PublicFolderMailboxesCannotBeMovedDuringMigration = 3503580213U,
			MigrationBatchDirectionOnboarding = 3739954134U,
			AsyncOperationTypeMigration = 1702371863U,
			ClientCulture_0x40E = 2721879659U,
			OriginatingServer = 1959086372U,
			EstimateStatePartiallySucceeded = 3553550262U,
			CannotImportDeletion = 1063322282U,
			MigrationUserStatusSynced = 668312535U,
			CannotImportFolderChange = 1644403520U,
			MigrationUserStatusValidating = 2717353738U,
			ExConstraintNotSupportedForThisPropertyType = 2473956989U,
			NotificationEmailSubjectCertExpiring = 614444523U,
			MigrationUserStatusSummaryCompleted = 1918350106U,
			SpellCheckerArabic = 116529921U,
			InternalLicensingDisabledForEnterprise = 2162216975U,
			ClientCulture_0x240A = 2264323529U,
			RPCOperationAbortedBecauseOfAnotherRPCThread = 1621771988U,
			ExInvalidMdbGuid = 2356708790U,
			SpellCheckerEnglishUnitedKingdom = 3718014775U,
			ExFilterHierarchyIsTooDeep = 1886702248U,
			MapiCannotSetMessageLockState = 176188277U,
			CannotProtectMessageForNonSmtpSender = 2635947676U,
			ExSearchFolderIsAlreadyVisibleToOutlook = 3669082289U,
			ExEntryIdFirst4Bytes = 1534555903U,
			CustomMessageLengthExceeded = 4224870879U,
			ExWrappedStreamFailure = 469526714U,
			ErrorExTimeZoneValueWrongGmtFormat = 3215248187U,
			InternalParserError = 2069696862U,
			ExInvalidCount = 2542755219U,
			ADUserNotFound = 3554710343U,
			InboxRuleFlagStatusNotFlagged = 2441819035U,
			ConversionMustLoadAllPropeties = 1728333927U,
			ThreeHours = 4222627801U,
			MapiCannotGetIDFromNames = 1816423621U,
			ErrorSigntureTooLarge = 3922439094U,
			MigrationBatchFlagReportInitial = 2247108930U,
			ErrorTimeProposalEndTimeBeforeStartTime = 3391255931U,
			CannotSetMessageFlagStatus = 3041727278U,
			MigrationFlagsReport = 2084296733U,
			MigrationStepProvisioning = 2188504663U,
			FirstFourDayWeek = 3392951782U,
			MapiCannotModifyRecipients = 313625050U,
			ConversionCorruptSummaryTnef = 3755684524U,
			ClientCulture_0x2409 = 1908093169U,
			ExAlreadyConnected = 2511211668U,
			ExReportMessageCorruptedDueToWrongItemAttachmentType = 2066272930U,
			ClientCulture_0x500A = 2828973564U,
			MigrationTypeNone = 4033769924U,
			CannotImportReadStateChange = 1112725991U,
			MapiCannotGetAttachmentTable = 2653492529U,
			MapiCannotOpenAttachment = 4054158457U,
			ExSuffixTextFilterNotSupported = 4130050910U,
			ExSeparatorNotFoundOnCompoundValue = 2641324698U,
			MigrationBatchStatusCorrupted = 4232824632U,
			MigrationBatchStatusSyncing = 3135377227U,
			ClientCulture_0x415 = 4287963488U,
			ClientCulture_0x2C01 = 3647547362U,
			CannotAccessRemoteMailbox = 984518113U,
			MapiCannotFindRow = 456720827U,
			ThirtyMinutes = 101242371U,
			MapiCannotSeekRow = 1022111068U,
			MigrationUserStatusFailed = 2976127978U,
			ExceptionObjectHasBeenDeleted = 3086681225U,
			MigrationBatchFlagDisallowExistingUsers = 2706959952U,
			ClientCulture_0x464 = 3884678960U,
			UnsupportedPropertyRestriction = 2868399142U,
			ServerLocatorClientWCFCallTimeout = 265623663U,
			InvalidServiceLocationResponse = 3055339528U,
			MapiCannotDeleteProperties = 722987850U,
			NeedFolderIdForPublicFolder = 4146176105U,
			ClientCulture_0x100C = 1666174282U,
			ManagedByRemoteExchangeOrganization = 813145114U,
			AutoDRequestFailed = 1511731857U,
			DumpsterFolderNotFound = 3680872945U,
			ExFolderNotFoundInClientState = 847028569U,
			ImportResultContainedFailure = 2399298613U,
			ClientCulture_0x813 = 4287963350U,
			ExCannotCreateMeetingResponse = 1419302978U,
			EightHours = 2061760288U,
			OperationResultFailed = 277994955U,
			ErrorWorkingHoursEndTimeSmaller = 860625858U,
			RoutingTypeRequired = 1977152861U,
			FolderRuleCannotSaveItem = 244097521U,
			RequestStateRemoving = 3695851705U,
			MigrationStateFailed = 1058417226U,
			MigrationUserStatusCompletionFailed = 2568510612U,
			MailboxSearchEwsEmptyResponse = 1709649557U,
			ClientCulture_0x140A = 2264323692U,
			MigrationUserStatusRemoving = 3414312136U,
			MigrationFolderCorruptedItems = 1240718058U,
			ClientCulture_0x418 = 4287963475U,
			SpellCheckerPortuguesePortugal = 1953718728U,
			TeamMailboxMessageReactivatingText = 3480868172U,
			SearchLogFileCreateException = 1241888079U,
			ExCannotGetDeletedItem = 2010446754U,
			MailboxSearchNameTooLong = 496581163U,
			ClientCulture_0x41F = 4287963597U,
			ClientCulture_0x4409 = 1908093103U,
			ExSubmissionQuotaExceeded = 3877064532U,
			ExCorruptMessageCorrelationBlob = 1682139678U,
			MigrationFolderDrumTesting = 3199235754U,
			ExCorruptFolderWebViewInfo = 4044636857U,
			MigrationBatchFlagDisableOnCopy = 3987708908U,
			ICSSynchronizationFailed = 1372163092U,
			OneHours = 4036040747U,
			InvalidBodyFormat = 1163271202U,
			PeopleQuickContactsAttributionDisplayName = 3938391037U,
			TwoHours = 1437739311U,
			ExPropertyDefinitionInMoreThanOnePropertyProfile = 1085572452U,
			TeamMailboxSyncStatusMembershipAndMaintenanceSyncFailure = 2260241133U,
			ClientCulture_0xC0C = 2721876058U,
			ExUnableToCopyAttachments = 3360384478U,
			ExCannotUpdateResponses = 4014822501U,
			ConversationItemHasNoBody = 3909125539U,
			DelegateCollectionInvalidAfterSave = 390622925U,
			TeamMailboxSyncStatusDocumentAndMaintenanceSyncFailure = 2946586312U,
			InvalidAttachmentType = 3712347292U,
			ExCannotMarkTaskCompletedWhenSuppressCreateOneOff = 3049568671U,
			CannotGetPropertyList = 748440690U,
			ErrorInvalidConfigurationXml = 1090294952U,
			InvalidBase64String = 3064401295U,
			RequestStateFailed = 1573110229U,
			InboxRuleImportanceNormal = 403751771U,
			MigrationLocalhostNotFound = 2718297568U,
			ClientCulture_0x1401 = 1551862972U,
			TeamMailboxSyncStatusSucceeded = 2594591409U,
			MigrationErrorAttachmentCorrupted = 3678601929U,
			OleConversionResultFailed = 974121200U,
			MigrationUserStatusSummaryFailed = 559182252U,
			RequestStateCanceled = 2151518657U,
			ModifyRuleInStore = 1326926126U,
			ExItemDeletedInRace = 1178929403U,
			ClientCulture_0x340A = 2264323626U,
			WeatherUnitFahrenheit = 2482422690U,
			MessageNotRightsProtected = 4014230567U,
			ConversionMaliciousContent = 704719423U,
			NoTemplateMessage = 3550236610U,
			FolderRuleStageLoading = 3838981202U,
			LimitedDetails = 2310868878U,
			AppendOOFHistoryEntry = 3851145766U,
			ExStoreSessionDisconnected = 1180958385U,
			NotificationEmailSubjectCertExpired = 1719015848U,
			MigrationUserAdminTypeDCAdmin = 1951112992U,
			SpellCheckerSpanish = 1511957081U,
			UserPhotoNotFound = 693971404U,
			ClientCulture_0x1C01 = 3647547525U,
			MigrationBatchFlagAutoStop = 959876171U,
			RemoteArchiveOffline = 1561549651U,
			CannotOpenLocalFreeBusy = 89761473U,
			CannotFindExchangePrincipal = 3320490165U,
			MigrationStageValidation = 2938839179U,
			CalNotifTypeReminder = 1865294994U,
			ExFailedToGetUnsearchableItems = 4057666506U,
			Monday = 3364213626U,
			AsyncOperationTypeUnknown = 135933047U,
			MigrationFolderSyncMigration = 3737835697U,
			InboxRuleFlagStatusComplete = 950284413U,
			NotificationEmailSubjectMoveMailbox = 838610512U,
			MessageRpmsgAttachmentIncorrectType = 3803104512U,
			FullDetails = 2897062825U,
			JunkEmailObjectDisposedException = 4157021563U,
			FailedToReadLocalServer = 4199032562U,
			MapiCannotGetCurrentRow = 1207296209U,
			MigrationInvalidPassword = 1074414016U,
			ExCannotDeletePropertiesOnOccurrences = 2253780921U,
			EstimateStateFailed = 2322466952U,
			RequestStateCompleting = 1348198184U,
			ClientCulture_0x41E = 4287963600U,
			InvalidSharingRecipientsException = 3913958124U,
			MapiCannotOpenFolder = 3340405076U,
			ClientCulture_0x180C = 536874274U,
			ADOperationAbortedBecauseOfAnotherADThread = 1558200374U,
			UpdateOOFHistoryOperation = 1606180860U,
			ExAttachmentAlreadyOpen = 3643161838U,
			NotificationEmailBodyImportPSTCompleted = 1606937438U,
			ExInvalidAggregate = 2205094141U,
			NotificationEmailBodyImportPSTFailed = 2046213432U,
			ExCantAccessOccurrenceFromSingle = 3350951418U,
			NotificationEmailBodyExportPSTCompleted = 3182347319U,
			ClientCulture_0x801 = 2721879411U,
			ClientCulture_0x401 = 2721879543U,
			CalNotifTypeNewUpdate = 1685125049U,
			NoExternalEwsAvailableException = 148503943U,
			CannotSharePublicFolder = 3737728227U,
			MigrationTypeExchangeRemoteMove = 705409536U,
			MigrationUserStatusCompletedWithWarning = 3160872492U,
			FailedToParseUseLicense = 898461441U,
			MigrationStateDisabled = 2930877107U,
			MigrationBatchSupportedActionComplete = 3957837829U,
			MapiCannotOpenEmbeddedMessage = 3969194465U,
			InboxRuleImportanceLow = 2205239196U,
			NoMapiPDLs = 792524531U,
			RmExceptionGenericMessage = 1929574072U,
			NotRead = 1065277019U,
			ClientCulture_0x80C = 2721879521U,
			idUnableToAddDefaultCalendarToDefaultCalendarGroup = 913845744U,
			ClientCulture_0x3801 = 987212902U,
			MapiCannotGetAllPerUserLongTermIds = 1599639819U,
			TeamMailboxMessageSiteMailboxEmailAddress = 101151487U,
			CalNotifTypeDeletedUpdate = 2267978872U,
			CannotCreateSearchFoldersInPublicStore = 676596483U,
			ExDictionaryDataCorruptedNoField = 3860059626U,
			ExceptionFolderIsRootFolder = 3414452687U,
			DateRangeOneDay = 2303439835U,
			ClientCulture_0x412 = 4287963481U,
			AppointmentTombstoneCorrupt = 2937224961U,
			MigrationBatchAutoComplete = 627389694U,
			MigrationObjectsCountStringNone = 3368210174U,
			ClientCulture_0x810 = 4287963351U,
			MapiCannotCopyItem = 1064525218U,
			ErrorNoStoreObjectIdAndFolderPath = 1405495556U,
			MigrationFolderSyncMigrationReports = 805861954U,
			UnsupportedFormsCondition = 2576334771U,
			ExStartTimeNotSet = 3719801171U,
			ClientCulture_0x804 = 2721879406U,
			ExConversationActionItemNotFound = 718334902U,
			MigrationUserStatusProvisioning = 3108568302U,
			InboxRuleMessageTypeNonDeliveryReport = 951826856U,
			ExFailedToUnregisterExchangeTopologyNotification = 1983194762U,
			TeamMailboxMessageLearnMore = 1453422183U,
			DateRangeThreeDays = 1701953962U,
			MapiCannotTransportSendMessage = 2423165282U,
			ExSortNotSupportedInDeepTraversalQuery = 1627212429U,
			JunkEmailAmbiguousUsernameException = 1452910403U,
			MigrationBatchStatusSyncedWithErrors = 3249771727U,
			FailedToFindAvailableHubs = 1714411372U,
			InternetCalendarName = 3876212982U,
			ExItemNotFound = 11761379U,
			ExDelegateNotSupportedRespondToMeetingRequest = 1042040859U,
			DisposeNonIPMFolder = 4247577850U,
			InboxRuleSensitivityPrivate = 2585840702U,
			MigrationFeatureNone = 1728869634U,
			ClientCulture_0x405 = 2721879547U,
			ExStringContainsSurroundingWhiteSpace = 46060694U,
			ClientCulture_0x4C0A = 1489962160U,
			ClientCulture_0x1809 = 630982608U,
			FailedToResealKey = 2138889051U,
			InvalidParticipantForRules = 2342985388U,
			SpellCheckerDanish = 3831935814U,
			MapiCannotGetProperties = 1280140891U,
			MapiCopyMessagesFailed = 1835617035U,
			FailedToParseValue = 2765862542U,
			RuleWriterObjectNotFound = 1208307535U,
			ExInvalidWatermarkString = 975909957U,
			ProvisioningRequestCsvContainsBothPasswordAndFederatedIdentity = 3000070570U,
			OperationResultSucceeded = 3348992335U,
			ServerLocatorServicePermanentFault = 141464842U,
			NotMailboxSession = 1039613051U,
			MigrationStateActive = 1391852443U,
			Null = 1743625299U,
			FolderRuleStageOnCreatedMessage = 1066460788U,
			CannotSetMessageFlags = 3238359831U,
			ExInvalidAsyncResult = 1511089307U,
			ClientCulture_0x2801 = 987212805U,
			ExFolderPropertyBagCannotSaveChanges = 3815733289U,
			MigrationBatchStatusStopped = 1665468465U,
			KqlParserTimeout = 1852466064U,
			TenMinutes = 3623312858U,
			ExMustSetSearchCriteriaToMakeVisibleToOutlook = 2730199010U,
			Wednesday = 3452652986U,
			ClientCulture_0xC07 = 2721875974U,
			LegacyMailboxSearchDescription = 3155538965U,
			MapiCannotFreeBookmark = 1494015926U,
			CannotChangePermissionsOnFolder = 4041018166U,
			MapiCannotSetProps = 14329882U,
			SearchStatePartiallySucceeded = 1082563208U,
			InboxRuleMessageTypeAutomaticReply = 1720321054U,
			RuleHistoryError = 270030022U,
			ClientCulture_0x280A = 1699673525U,
			ClientCulture_0x3001 = 2116512910U,
			SharePoint = 422603853U,
			NoDelegateAction = 3562004828U,
			MigrationFlagsStop = 3008535783U,
			ExNoOptimizedCodePath = 2280589817U,
			MigrationBatchFlagUseAdvancedValidation = 2285862258U,
			MapiCannotGetParentEntryId = 721699019U,
			ExOnlyMessagesHaveParent = 3915122871U,
			ClientCulture_0x411 = 4287963484U,
			ExFolderDoesNotMatchFolderId = 1405345367U,
			MigrationTypeXO1 = 2992671490U,
			NotOperator = 273956641U,
			ClientCulture_0x480A = 1699673459U,
			Saturday = 3478111469U,
			ExFailedToRegisterExchangeTopologyNotification = 4173454943U,
			MigrationBatchSupportedActionSet = 1732871098U,
			ConversionCannotOpenJournalMessage = 552541799U,
			JunkEmailTrustedListXsoEmptyException = 2519002207U,
			AmFailedToFindSuitableServer = 2181826069U,
			OperationalError = 1299125360U,
			ExTooManySortColumns = 2689882193U,
			LoadRulesFromStore = 3810093794U,
			ClientCulture_0x40D = 2721879658U,
			ExCantCopyBadAlienDLMember = 1282190256U,
			TeamMailboxMessageSendMailToTheSiteMailbox = 3434190160U,
			InboxRuleSensitivityPersonal = 3610984697U,
			ExInvalidItemCountAdvisorCondition = 2078392877U,
			ErrorLoadingRules = 3955579167U,
			ClientCulture_0x414 = 4287963487U,
			ExEndTimeNotSet = 1246168046U,
			InboxRuleMessageTypeAutomaticForward = 1055197681U,
			MapiCannotCopyMapiProps = 392605258U,
			OneWeeks = 2708396435U,
			TeamMailboxMessageWhatYouCanDoNext = 494393471U,
			MapiCannotGetReceiveFolderInfo = 1985781485U,
			ExInvalidStoreObjectId = 2870966119U,
			RequestStateQueued = 1332658223U,
			RecurrenceBlobCorrupted = 1762000491U,
			CannotFindAttachment = 3526072757U,
			ExInvalidRecipientBlob = 2356618506U,
			ExIncompleteBlob = 1272751292U,
			ExPatternNotSet = 379946208U,
			ExInvalidDayOfMonth = 1944490681U,
			ExInvalidGlobalObjectId = 3835493555U,
			MapiCannotGetHierarchyTable = 2185974115U,
			ClientCulture_0xC0A = 2721876056U,
			ExInvalidFullyQualifiedServerName = 2799724840U,
			EstimateStateInProgress = 751268339U,
			MapiCannotSetReadFlags = 919965275U,
			PublicFolderQueryStatusSyncFolderHierarchyRpcFailed = 2710414285U,
			ExInvalidSearchFolderScope = 3367423950U,
			ActivitySessionIsNull = 298783488U,
			SpellCheckerItalian = 3325987673U,
			FolderRuleStageOnPublicFolderAfter = 3248071036U,
			NotAllowedAnonymousSharingByPolicy = 1964131369U,
			MapiCannotGetCollapseState = 1214453788U,
			ZeroMinutes = 3651846103U,
			RecipientNotSupportedByAnyProviderException = 3749282747U,
			MigrationReportFinalizationSuccess = 1970390563U,
			CalNotifTypeChangedUpdate = 3878608135U,
			ExSizeFilterPropertyNotSupported = 3697440372U,
			ExConversationActionInvalidFolderType = 3733109861U,
			ClientCulture_0x2009 = 2472743173U,
			UserDiscoveryMailboxNotFound = 2230033988U,
			ExCorruptedRecurringCalItem = 314838903U,
			ExStartDateLaterThanEndDate = 780978771U,
			ExInvalidOrganizer = 2635186097U,
			MaxExclusionReached = 3162466950U,
			JunkEmailBlockedListXsoEmptyException = 291587104U,
			SearchStateSucceeded = 4224027968U,
			ExInvalidJournalReportFormat = 4078034164U,
			RequestStateCertExpiring = 1049215562U,
			ConversionBodyConversionFailed = 3206936715U,
			MigrationUserAdminTypeTenantAdmin = 167159909U,
			ClientCulture_0x40B = 2721879652U,
			ConversionEmptyAddress = 871369479U,
			ClientCulture_0x380A = 1699673622U,
			PublicFoldersCannotBeAccessedDuringCompletion = 2430821738U,
			MigrationReportUnknown = 845797650U,
			FolderRuleResolvingAddressBookEntryId = 2220118463U,
			SharePointLifecyclePolicy = 2430200359U,
			MapiCannotAddNotification = 1296109892U,
			ClientCulture_0x200A = 2828973533U,
			ExFailedToCreateEventManager = 1595290732U,
			MaixmumNumberOfMailboxAssociationsForUserReached = 2693957296U,
			ExInvalidEIT = 4067432976U,
			ExFilterAndSortNotSupportedInSimpleVirtualPropertyDefinition = 3176034413U,
			DateRangeOneMonth = 2864562615U,
			MapiInvalidParam = 1299122915U,
			MapiCannotDeleteUserPhoto = 94201836U,
			MigrationUserAdminTypePartner = 3755817478U,
			ExOrganizerCannotCallUpdateCalendarItem = 1504384645U,
			DuplicateCondition = 1680240084U,
			JunkEmailTrustedListXsoTooManyException = 1817477041U,
			VersionNotInteger = 1076453993U,
			ClientCulture_0x41A = 4287963596U,
			MapiCannotGetNamedProperties = 3423699388U,
			ExFailedToDeleteDefaultFolder = 543137909U,
			ExDefaultFoldersNotInitialized = 1775042376U,
			AsyncOperationTypeCertExpiry = 2045069482U,
			ClosingTagExpectedNoneFound = 3561051015U,
			FolderRuleStageEvaluation = 4098965384U,
			ClientCulture_0x814 = 4287963347U,
			TeamMailboxMessageMemberInvitationBodyIntroText = 4024171282U,
			ClientCulture_0xC09 = 2721875968U,
			UnexpectedToken = 3683655246U,
			MapiCannotSeekRowBookmark = 3146200960U,
			PublicFolderSyncFolderRpcFailed = 2294313398U,
			MigrationFlagsNone = 1106682409U,
			JunkEmailTrustedListXsoNullException = 3045858265U,
			MapiCannotQueryRows = 2855609935U,
			MigrationTestMSASuccess = 147262116U,
			MigrationMailboxPermissionAdmin = 732586480U,
			ExInvalidItemId = 1719335392U,
			MapiCannotSetSpooler = 3009511836U,
			InvalidSharingTargetRecipientException = 1932453382U,
			RequestStateInProgress = 1816014188U,
			UnlockOOFHistory = 4127188442U,
			ExMclCannotBeResolved = 559598517U,
			FailedToReadConfiguration = 3224364440U,
			ADUserNoMailbox = 749132645U,
			ExReadExchangeTopologyFailed = 1136597198U,
			MigrationUserStatusCorrupted = 152248145U,
			ExMatchShouldHaveBeenCalled = 4055176570U,
			ExCannotModifyRemovedRecipient = 3282126921U,
			InvalidDateTimeFormat = 3306331751U,
			OleConversionPrepareFailed = 3270231634U,
			NotificationEmailBodyExportPSTFailed = 2098990361U,
			ExRangeNotSet = 1342934277U,
			OrganizationNotFederatedException = 2096844093U,
			ClientCulture_0x421 = 1559080129U,
			ACLTooBig = 1453305798U,
			TeamMailboxMessageNotConnectedToSite = 1949750262U,
			TeamMailboxSyncStatusFailed = 77059561U,
			ClientCulture_0x80A = 2721879523U,
			FailedToBindToUseLicense = 713774522U,
			MapiCannotGetParentId = 3349952875U,
			ExEndDateEarlierThanStartDate = 3964486357U,
			ExInvalidCustomSerializationData = 2183577153U,
			ConversionLimitsExceeded = 1559063653U,
			LargeMultivaluedPropertiesNotSupportedInTNEF = 1403049999U,
			MigrationSkippableStepSettingTargetAddress = 3892201190U,
			InvalidPropertyKey = 3041066937U,
			TwoDays = 1512671255U,
			ExOutlookSearchFolderDoesNotHaveMailboxSession = 1027306046U,
			ClientCulture_0x426 = 1559080130U,
			MalformedAdrEntry = 1572145744U,
			FailedToFindIssuanceLicenseAndURI = 1033019572U,
			InboxRuleMessageTypeEncrypted = 4010739301U,
			SuffixMatchNotSupported = 927529551U,
			idClientSessionInfoParseException = 1799732388U,
			ClientCulture_0x1009 = 2472743336U,
			OperationResultPartiallySucceeded = 2107406877U,
			UceContentFilterLoadFailure = 628765410U,
			MapiRulesError = 1228234464U,
			MigrationBatchStatusRemoving = 2423017895U,
			ExBadFolderEntryIdSize = 2503716610U,
			NotSupportedSharingMessageException = 3097513127U,
			IncompleteExchangePrincipal = 885639843U,
			MigrationFeatureMultiBatch = 2444446273U,
			MapiCannotSetTableColumns = 2889367749U,
			FailedToReadActivityLog = 3398202471U,
			SearchOperationFailed = 2045304318U,
			ExDefaultJournalFilename = 3997956246U,
			MapiCannotExpandRow = 1407904664U,
			ExCannotStartDeadSessionChecking = 3375709410U,
			SearchStateNotStarted = 2953075549U,
			SpellCheckerSwedish = 2989293604U,
			NotificationEmailSubjectExportPst = 3084838222U,
			ContentRestrictionOnSearchKey = 2174906965U,
			ExUnknownFilterType = 671447089U,
			ClientCulture_0x400A = 2828973467U,
			UnableToLoadDrmMessage = 3647529948U,
			ExCannotParseValue = 4048687496U,
			ClientCulture_0x4001 = 2116512747U,
			TeamMailboxMessageToLearnMore = 1918332060U,
			MigrationBatchStatusCompleting = 1272465216U,
			MapiCannotGetRowCount = 2453169079U,
			FolderRuleStageOnDeliveredMessage = 3849632232U,
			MigrationBatchSupportedActionAppend = 2614560510U,
			ExCannotOpenRejectedItem = 1858795727U,
			MigrationBatchStatusFailed = 82143339U,
			VotingDataCorrupt = 597150166U,
			MigrationTestMSAFailed = 3941673604U,
			TeamMailboxSyncStatusDocumentAndMembershipSyncFailure = 656484203U,
			PublicFoldersCannotBeMovedDuringMigration = 460612780U,
			ConversionNoReplayContent = 480886895U,
			ClientCulture_0x404 = 2721879546U,
			ExCantSubmitWithoutRecipients = 495633109U,
			LastErrorMessage = 1614900577U,
			JunkEmailFolderNotFoundException = 2849660304U,
			ClientCulture_0x42A = 1559080241U,
			AmMoveNotApplicableForDbException = 3133024367U,
			ExCantUndeleteOccurrence = 3710501592U,
			MigrationUserStatusQueued = 2418303566U,
			DateRangeSixMonths = 4210093826U,
			MigrationSkippableStepNone = 1365634215U,
			MalformedCommentRestriction = 3746574982U,
			ClientCulture_0x427 = 1559080131U,
			MigrationUserStatusSummaryActive = 2702427225U,
			ErrorEmptyFolderNotSupported = 2343856628U,
			ClientCulture_0x540A = 2264323560U,
			ClientCulture_0x2C0A = 1489962226U,
			MissingOperand = 3302221775U,
			DuplicateAction = 1949879021U,
			SearchStateQueued = 2882103486U,
			ExQueryPropertyBagRowNotSet = 3607950093U,
			Sunday = 1073167130U,
			WeatherUnitDefault = 3059751105U,
			RequestSecurityTokenException = 3120595407U,
			ExNoOccurrencesInRecurrence = 1768187831U,
			ExInvalidMclXml = 276036243U,
			ExInvalidIdFormat = 4168806856U,
			ExAdminAuditLogsFolderAccessDenied = 2161223297U,
			ClientCulture_0xC04 = 2721875973U,
			ExInvalidComparisionOperatorInPropertyComparisionFilter = 4170247214U,
			AsyncOperationTypeExportPST = 1937417240U
		}

		private enum ParamIDs
		{
			InvalidReceiveMeetingMessageCopiesException,
			AmDatabaseADException,
			AddressAndOriginMismatch,
			RepairingIsNotApplicableForCurrentMonitorState,
			MigrationInvalidTargetAddress,
			JunkEmailTrustedListXsoFormatException,
			ExSyncStateAlreadyExists,
			FailedToAcquireFederationRac,
			SaveFailureAfterPromotion,
			ErrorCannotSyncHoldObjectManagedByOtherOrg,
			TaskOperationFailedWithEcException,
			UserPhotoFileTooLarge,
			ExternalIdentityInconsistentSid,
			DataMoveReplicationConstraintFlushNotSatisfied,
			PublicFoldersNotEnabledForTenant,
			MigrationJobConnectionSettingsIncomplete,
			JunkEmailBlockedListOwnersEmailAddressException,
			ExServerNotInSite,
			InvalidSharingMessageException,
			CannotObtainSynchronizationUploadState,
			ExFolderDeletePropsFailed,
			ExConfigTypeNotMatched,
			UserCannotBeFoundFromContext,
			JunkEmailBlockedListXsoGenericException,
			MapiCannotCreateFolder,
			ExCantDeleteOpenedOccurrence,
			BindToWrongObjectType,
			idMailboxInfoStaleException,
			ExCannotSaveInvalidObject,
			AmServerTransientException,
			ExCalendarTypeNotSupported,
			FailedToAcquireUseLicense,
			AmReplayServiceDownException,
			ExDefaultFolderNotFound,
			JunkEmailBlockedListInternalToOrganizationException,
			FailedToAcquireUseLicenses,
			AmDbNotMountedNoViableServersException,
			ExNumberOfRowsToFetchInvalid,
			PropertyIsReadOnly,
			ActiveMonitoringServerException,
			AmServerDagNotFound,
			MigrationObjectsCountStringMailboxes,
			AddressAndRoutingTypeMismatch,
			RecipientAddressInvalidForExternalLicensing,
			MigrationBatchNotFoundError,
			ExInvalidMonthNthDayMask,
			PublicFolderPerServerThreadLimitExceeded,
			DataMoveReplicationConstraintNotSatisfiedForNonReplicatedDatabase,
			FailedToCreateLicenseGenerator,
			OscFolderForProviderNotFound,
			ImportItemThrewGrayException,
			NonCanonicalACL,
			UserPhotoFileTooSmall,
			InvalidWorkingPeriod,
			AmFailedToDeterminePAM,
			ExInvalidMAPIType,
			MapiCannotOpenAttachmentId,
			ExUnableToOpenOrCreateDefaultFolder,
			TeamMailboxMessageWelcomeSubject,
			ConversionCorruptTnef,
			ExNullSortOrderParameter,
			UserSidNotFound,
			PublicFolderSyncFolderHierarchyFailed,
			MailboxNotFoundByAdObjectId,
			idResourceQuarantinedDueToBlackHole,
			ExInvalidParameter,
			FailedToUnprotectAttachment,
			SearchMandatoryParameter,
			InvalidDuration,
			NotificationEmailSubjectFailed,
			InternalLicensingDisabledForTenant,
			ExBatchBuilderADLookupFailed,
			ActiveMonitoringServiceDown,
			ErrorStatisticsStartIndexIsOutOfBound,
			ExServerNotFound,
			CanUseApiOnlyWhenTimeZoneIsNull,
			ExUnsupportedMapiType,
			DataMoveReplicationConstraintNotSatisfiedNoHealthyCopies,
			DagNetworkManagementError,
			ActiveMonitoringServerTransientException,
			TeamMailboxMessageClosedSubject,
			ErrorSavingSearchObject,
			FolderSaveOperationResult,
			FailedToLocateTPDConfig,
			ExInvalidExceptionListWithDoubleEntry,
			ExCannotCreateFolder,
			ExInvalidDateTimeRange,
			AmServerNotFoundException,
			UserHasNoEventPermisson,
			RpcServerRequestAlreadyPending,
			ExUnsupportedABProvider,
			MigrationItemStatisticsString,
			ValidationFailureAfterPromotion,
			ExCurrentServerNotInSite,
			CannotResolvePropertyException,
			FailedToVerifyDRMPropsSignatureADError,
			RpcServerIgnorePendingDeleteTeamMailbox,
			MigrationObjectsCountStringGroups,
			ExInvalidMonthlyDayOfMonth,
			ErrorCalendarReminderNotMinutes,
			ExCorrelationFailedForOccurrence,
			ErrorTimeProposalInvalidDuration,
			MigrationFolderNotFound,
			MigrationMailboxDatabaseInfoNotAvailable,
			MultipleSystemMigrationMailboxes,
			ExInvalidNamedProperty,
			DagNetworkRenameDupName,
			ErrorInvalidQueryLanguage,
			ExValueOfWrongType,
			InvalidReminderTime,
			InvalidExternalDirectoryObjectIdError,
			idUnableToCreateDefaultCalendarGroupException,
			ExInvalidWeeklyDayMask,
			IncorrectEntriesInServiceLocationResponse,
			InconsistentCalendarMethod,
			CannotCreateSynchronizerEx,
			ExInvalidExceptionCount,
			RemoteAccountPolicyNotFound,
			IncorrectServerError,
			ExInvalidYearlyRecurrencePeriod,
			RpcServerDirectoryError,
			FailedToReadUserConfig,
			InvalidDueDate1,
			ExUnsupportedCodepage,
			EmailFormatNotSupported,
			ExInvalidTypeGroupCombination,
			MigrationBatchErrorString,
			ClosingTagExpected,
			AmServiceDownException,
			ExInvalidAttendeeType,
			AmDatabaseMasterIsInvalid,
			MigrationGroupMembersAttachmentCorrupted,
			UseMethodInstead,
			InvalidReminderOffset,
			ExSearchFolderNoAssociatedItem,
			DatabaseLocationNotAvailable,
			MigrationJobConnectionSettingsInvalid,
			MailboxSearchObjectNotExist,
			ExInvalidValueTypeForCalculatedProperty,
			PrincipalNotAllowedByPolicy,
			ExCorruptDataRecoverError,
			ActiveMonitoringUnknownGenericRpcCommand,
			ExTooManySubscriptionsOnPublicStore,
			ExCalculatedPropertyStreamAccessNotSupported,
			InvalidRmsUrl,
			AmRpcOperationNotImplemented,
			SharingFolderName,
			OperationNotAllowed,
			SharingPolicyNotFound,
			InvalidExternalSharingInitiatorException,
			ExSortGroupNotSupportedForProperty,
			ConversationContainsDuplicateMids,
			ParticipantPropertyTooBig,
			CopyToDumpsterFailure,
			ExNotSupportedConfigurationSearchFlags,
			InvalidParticipant,
			ExInvalidEventWatermarkBadOrigin,
			ExPropertyNotValidOnOccurrence,
			DefaultFolderAccessDenied,
			COWMailboxInfoCacheTimeout,
			ExFinalEventFound,
			FailedToFindLicenseUri,
			BadEnumValue,
			NotStreamablePropertyValue,
			ObjectMustBeOfType,
			ErrorCorruptedData,
			QueryUsageRightsPrelicenseResponseHasFailure,
			MigrationMailboxNotFoundOnServerError,
			StreamPropertyNotFound,
			idOscContactSourcesForContactParseError,
			ExIncorrectOriginalTimeInExtendedExceptionInfo,
			DeleteFromDumpsterFailure,
			DetailLevelNotAllowedByPolicy,
			ErrorCalendarReminderNegative,
			GroupMailboxAccessSidConstructionFailed,
			OperationTimedOut,
			MailboxVersionTooLow,
			ExCannotSaveSyncStateFolder,
			ExNotSupportedNotificationType,
			ConversionInvalidItemType,
			CannotFindRequestIndexEntry,
			ExBodyFormatConversionNotSupported,
			ExConfigExisted,
			CannotGetSynchronizeBuffers,
			TenantLicensesDistributionPointMismatch,
			ExUrlNotFound,
			InvalidICalElement,
			FailedToRpcAcquireUseLicenses,
			ExUnknownPattern,
			ExFilterNotSupportedForProperty,
			SearchObjectIsInvalid,
			FailedToDownloadServerLicensingMExData,
			ActiveMonitoringOperationFailedException,
			UnsupportedClientOperation,
			ExternalLicensingDisabledForEnterprise,
			JunkEmailBlockedListXsoTooBigException,
			AmDatabaseCopyNotFoundException,
			ValueNotRecognizedForAttribute,
			AmOperationFailedWithEcException,
			RuleParseError,
			FolderRuleErrorRecordForSpecificRule,
			DataMoveReplicationConstraintUnknown,
			ExPropertyValidationFailed,
			BadTimeFormatInChangeDate,
			ExDeleteNotSupportedForCalculatedProperty,
			DagNetworkCreateDupName,
			DataMoveReplicationConstraintNotSatisfiedInvalidConstraint,
			AmOperationFailedException,
			ExMismatchedSyncStateDataType,
			RpcServerIgnoreNotFoundTeamMailbox,
			AnchorDatabaseNotFound,
			ExNoMatchingStorePropertyDefinition,
			FailedToRetrieveUserLicense,
			InvalidMailboxType,
			FailedToFindServerInfo,
			FailedToReadSharedServerBoxRacIdentityFromIRMConfig,
			ExNonCalendarItemReturned,
			DelegateValidationFailed,
			ExResponseTypeNoSubjectPrefix,
			MigrationNSPINoUsersFound,
			JunkEmailTrustedListXsoGenericException,
			AttemptingSessionCreationAgainstWrongGroupMailbox,
			descInvalidTypeInBookingPolicyConfig,
			NoSupportException,
			MalformedWorkingHours,
			DataMoveReplicationConstraintNotSatisfiedThrottled,
			PublicFolderOperationDenied,
			ExInvalidO12BytesToSkip,
			ColumnError,
			ErrorValidateXsoDriverProperty,
			AmInvalidActionCodeException,
			FailedToReadIRMConfig,
			CultureMismatchAfterConnect,
			ExternalIdentityInvalidSid,
			MaxRemindersExceeded,
			NonUniqueAssociationError,
			ExConfigSerializeDictionaryFailed,
			PublicFolderSyncFolderFailed,
			ErrorUnsupportedConfigurationXmlCategory,
			FailedToRetrieveServerLicense,
			ExPropertyRequiresStreaming,
			ConversationWithoutRootNode,
			idClientSessionInfoTypeParseException,
			ExBatchBuilderError,
			ActiveManagerGenericRpcVersionNotSupported,
			ErrorTeamMailboxUserVersionUnqualified,
			ExInvalidMonthNthOccurence,
			ExInvalidEventWatermarkBadEventCounter,
			ErrorInvalidDateFormat,
			TaskRecurrenceNotSupported,
			CannotCreateCollector,
			DataBaseNotFoundError,
			UnsupportedReportType,
			ExceptionIsNotPublicFolder,
			ExTooManySubscriptions,
			ActiveMonitoringRpcVersionNotSupported,
			ExReplyToTooManyRecipients,
			OscSyncLockNotFound,
			ErrorFailedToDeletePublicFolder,
			RpcServerIgnoreClosedTeamMailbox,
			FailedToVerifyDRMPropsSignature,
			MigrationSystemMailboxNotFound,
			InvalidSmtpAddress,
			InvokingMethodNotSupported,
			FailedToGetOrgContainer,
			ValidationForServiceLocationResponseFailed,
			MigrationObjectsCountStringContacts,
			MailboxSearchEwsFailedExceptionWithError,
			ParsingErrorAt,
			ExDictionaryDataCorruptedDuplicateKey,
			MigrationNSPIMissingRequiredField,
			MigrationTransientError,
			ExFolderSetPropsFailed,
			AmReferralException,
			TaskServerTransientException,
			AmOperationNotValidOnCurrentRole,
			RangedParameter,
			IsNotMailboxOwner,
			JunkEmailBlockedListXsoDuplicateException,
			ExTooManyDuplicateDataColumns,
			MigrationErrorString,
			MigrationGroupMembersNotAvailable,
			RpcServerIgnoreUnlinkedTeamMailbox,
			AmServerException,
			ExConversationNotFound,
			ErrorNotSupportedLanguageWithInstalledLanguagePack,
			ErrorInPlaceHoldIdentityChanged,
			InvalidSharingDataException,
			MigrationOrganizationNotFound,
			AmDbMountNotAllowedDueToAcllErrorException,
			PublicFolderSyncFolderHierarchyFailedAfterMultipleAttempts,
			NoInternalEwsAvailableException,
			AmInvalidConfiguration,
			InvalidLocalDirectorySecurityIdentifier,
			CalendarOriginatorIdCorrupt,
			AppointmentActionNotSupported,
			TooManyActiveManagerClientRPCs,
			RightsNotAllowedByPolicy,
			AmServerNotFoundToVerifyRpcVersion,
			TaskOperationFailedException,
			ConversionMaxRecipientExceeded,
			NoSharingHandlerFoundException,
			ExternalUserNotFound,
			ExTenantAccessBlocked,
			ExUnresolvedRecipient,
			ExInvalidAttachmentId,
			ErrorFolderAlreadyExists,
			InvalidAddressFormat,
			ExOccurrenceNotPresent,
			ExComparisonOperatorNotSupported,
			ExternalLicensingDisabledForTenant,
			CantParseParticipant,
			ErrorFolderSave,
			ExMailboxAccessDenied,
			ExInvalidTypeInBlob,
			PromoteVeventFailure,
			FolderRuleErrorRecord,
			MalformedTimeZoneWorkingHours,
			ExInvalidFileTimeInRecurrenceBlob,
			ErrorInvalidInPlaceHoldIdentity,
			ExBodyConversionNotSupportedType,
			SaveConfigurationItem,
			RpcServerUnhandledException,
			TaskInvalidFlagStatus,
			RpcServerParameterSerializationError,
			ExTimeInExtendedInfoNotSameAsExceptionInfo,
			FailedToGetRmsTemplates,
			ExInvalidFolderType,
			ExItemNotFoundInConversation,
			ExInvalidMinutePeriod,
			ConversionMaxRecipientSizeExceeded,
			ErrorInvalidStatisticsStartIndex,
			MigrationInvalidStatus,
			QueryUsageRightsNoPrelicenseResponse,
			ExBatchBuilderNeedsPropertyToConvertRT,
			NonUniqueRecipientByExternalIdError,
			MultipleProvisioningMailboxes,
			ExMailboxMustBeAccessedAsOwner,
			CannotCreateOscFolderBecauseOfConflict,
			LicenseUriInvalidForTenant,
			ExConfigNameInvalid,
			ExInvalidMultivaluePropertyFilter,
			ExInvalidResponseType,
			ErrorTeamMailboxSendingNotifications,
			AnonymousPublishingUrlValidationException,
			MapiCannotModifyPropertyTable,
			Ex12NotSupportedDeleteItemFlags,
			DisplayNameRequiredForRoutingType,
			UserPhotoImageTooSmall,
			ProgramError,
			ErrorInvalidQuery,
			EulNotFoundInContainerItem,
			FailedToAquirePublishLicense,
			InvalidFolderId,
			PositiveParameter,
			RecipientAddressNotSpecifiedForExternalLicensing,
			RpcServerRequestSuccess,
			AdUserNotFoundException,
			idNotSupportedWithServerVersionException,
			RpcServerStorageError,
			QueryUsageRightsPrelicenseResponseFailedToExtractRights,
			UnexpectedTag,
			MigrationPermanentError,
			CannotShareFolderException,
			ErrorADUserFoundByReadOnlyButNotWrite,
			ConversionMaxEmbeddedDepthExceeded,
			RpcServerWrongRequestServer,
			CantFindCalendarFolderException,
			CannotCreateEmbeddedItem,
			ErrorTeamMailboxGetUserMailboxDatabaseFailed,
			SystemAPIFailed,
			OleConversionInitError,
			ExUnableToGetStreamProperty,
			ExMeetingCantCrossOtherOccurrences,
			ExConfigurationNotFound,
			ExNotSupportedCreateMode,
			AmDbAcllErrorNoReplicaInstance,
			SyncStateCollision,
			RmLicenseRetrieveFailed,
			DiscoveryMailboxCannotBeSourceOrTarget,
			ErrorNonUniqueLegacyDN,
			ExMclIsTooBig,
			ExInvalidPropertyType,
			MailboxDatabaseRequired,
			JunkEmailTrustedListXsoTooBigException,
			ExOperationNotSupportedForRoutingType,
			DiscoveryMailboxIsNotUnique,
			ConflictingObjectType,
			MapiCannotOpenMailbox,
			ExSyncStateCorrupted,
			ExInvalidHexString,
			MigrationUserSkippedItemString,
			FolderRuleErrorInvalidRecipient,
			MapiCannotMatchAttachmentIds,
			FailedToAcquireRacAndClc,
			NotificationEmailSubjectCreated,
			SyncStateMissing,
			FailedToFindTargetUriFromMExData,
			AmDatabaseNotFoundException,
			ExInvalidHexCharacter,
			ServerForDatabaseNotFound,
			RpcClientException,
			ErrorCouldNotUpdateMasterIdentityProperty,
			DataMoveReplicationConstraintSatisfied,
			UnknownOscProvider,
			ErrorTeamMailboxUserNotResolved,
			FolderRuleErrorGroupDoesNotResolve,
			DatabaseNotFound,
			ExSearchFolderCorruptOutlookBlob,
			ExCannotMoveOrCopyOccurrenceItem,
			JunkEmailBlockedListXsoFormatException,
			InvalidDueDate2,
			MigrationRunspaceError,
			TeamMailboxMessageReactivatedSubject,
			CompositeError,
			ExNullItemIdParameter,
			ExItemDoesNotBelongToCurrentFolder,
			AddressRequiredForRoutingType,
			InconsistentCalendarType,
			PropertyErrorString,
			ExStartDateCantBeGreaterThanMaximum,
			ExEndDateCantExceedMaxDate,
			CannotCreateManifestEx,
			ExNewerVersionedSyncState,
			ErrorInvalidTimeFormat,
			InvalidTagName,
			MigrationJobItemRecipientTypeMismatch,
			ExModifiedOccurrenceCrossingAdjacentOccurrenceBoundary,
			ExComparisonOperatorNotSupportedForProperty,
			NotAuthorizedtoAccessGroupMailbox,
			TaskServerException,
			ErrorInvalidQueryTooLong,
			ExConfigDataCorrupted,
			LicenseExpired,
			ElementHasUnsupportedValue,
			CannotResolvePropertyTagsToPropertyDefinitions,
			NonNegativeParameter,
			CreateConfigurationItem,
			ActiveMonitoringOperationFailedWithEcException,
			ExInvalidRecurrenceInterval,
			FederatedMailboxNotSet,
			JunkEmailValidationError,
			MigrationUnexpectedExchangePrincipalFound,
			RpcServerRequestThrottled,
			DefaultFolderNotFoundInPublicFolderMailbox,
			JunkEmailTrustedListInternalToOrganizationException,
			ExStartDateCantBeLessThanMinimum,
			ExPDLCorruptOutlookBlob,
			DataMoveReplicationConstraintNotSatisfied,
			ExInvalidLicense,
			ErrorCalendarReminderTooLarge,
			ExSetNotSupportedForCalculatedProperty,
			ExFilterNotSupported,
			JunkEmailTrustedListOwnersEmailAddressException,
			ErrorUnsupportedConfigurationXmlVersion,
			ExInvalidMultivalueElement,
			PublicFolderHierarchySessionNull,
			NotificationEmailSubjectCompleted,
			ErrorTeamMailboxUserTypeUnqualified,
			ConversationCreatorSidNotSet,
			ExNullParameter,
			ExEmptyCollection,
			MigrationAttachmentNotFound,
			ActiveManagerUnknownGenericRpcCommand,
			RecipientAddressInvalid,
			ExInvalidExceptionInfoSubstringLength,
			DagNetworkCannotRemoveActiveSubnet,
			ExInvalidNullParameterForChangeTypes,
			ConversionMaxBodyPartsExceeded,
			JunkEmailTrustedListXsoDuplicateException,
			CannotAuthenticateUserByTheClientSecurityContext,
			ExComparisonFilterPropertiesNotSupported,
			CannotGetLongTermIdFromId,
			RecoverableItemsAccessDeniedException,
			ErrorExTimeZoneValueMultipleGmtMatches,
			ExUnsupportedCharset,
			FolderRuleErrorInvalidGroup,
			CannotGetIdFromLongTermId,
			ErrorXsoObjectPropertyValidationError,
			ErrorInvalidItemHoldPeriod,
			ProvisioningMailboxNotFound,
			ExInvalidBase64StringFormat,
			ExInvalidWABObjectType,
			AggregatedMailboxNotFound,
			AnchorServerNotFound,
			ReminderPropertyNotSupported,
			RpcServerParameterInvalidError,
			ExSearchFolderAlreadyExists,
			CannotVerifyDRMPropsSignatureUserNotFound,
			DataMoveReplicationConstraintSatisfiedForNonReplicatedDatabase,
			ExCannotStampDefaultFolderId,
			FailedToRpcAcquireRacAndClc,
			InvalidCacheEntryId,
			FailedToDownloadCertificationMExData,
			MigrationGroupMembersAlreadyAvailable,
			TimeZoneReferenceWithNullTimeZone,
			ExInvalidBodyFormat,
			idUnableToAddDefaultTaskFolderToDefaultTaskGroup,
			ReplayServiceDown,
			FailedToAcquireTenantLicenses,
			ExModifiedOccurrenceCantHaveStartDateAsAdjacentOccurrence,
			WrongTimeZoneReference,
			ExConstraintViolationByteArrayLengthTooLong,
			ExPropertyError,
			ExInvalidChangeType,
			ExSaveFailedBecauseOfConflicts,
			SharingFolderNameWithSuffix,
			ExPropertyNotStreamable,
			MigrationInvalidTargetProxyAddress,
			RepairingIsNotSetSinceMonitorEntryIsNotFound,
			PublicFolderConnectionThreadLimitExceeded,
			TeamMailboxMessageMemberInvitationSubject,
			ExInvalidValueForFlagsCalculatedProperty,
			ADUserNotFoundId,
			InvalidUrlScheme,
			ExTypeSerializationNotSupported,
			MigrationMRSJobMissing,
			FailedToCheckPublishLicenseOwnership,
			CannotSynchronizeManifestEx,
			StoreDataInvalid,
			ExTooManyInstancesOnSeries,
			InvalidAddressError,
			ExGetNotSupportedForCalculatedProperty,
			ExTooManyObjects
		}
	}
}
