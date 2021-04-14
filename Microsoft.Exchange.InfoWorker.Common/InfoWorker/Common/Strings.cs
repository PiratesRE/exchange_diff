using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal static class Strings
	{
		static Strings()
		{
			Strings.stringIDs.Add(4118843607U, "TrackingErrorCrossPremiseMisconfiguration");
			Strings.stringIDs.Add(887069165U, "LogFieldsResultSizeCopied");
			Strings.stringIDs.Add(2434190600U, "MailtipsApplicationName");
			Strings.stringIDs.Add(3039411601U, "descNotDefaultCalendar");
			Strings.stringIDs.Add(3645107181U, "TrackingPermanentError");
			Strings.stringIDs.Add(1560475167U, "WrongTargetServer");
			Strings.stringIDs.Add(4159961541U, "InvalidSearchQuery");
			Strings.stringIDs.Add(2177919315U, "ProgressCompletingSearch");
			Strings.stringIDs.Add(3855823295U, "descWorkHoursStartTimeInvalid");
			Strings.stringIDs.Add(996171111U, "descMailboxLogonFailed");
			Strings.stringIDs.Add(3025732470U, "InstallAssistantsServiceTask");
			Strings.stringIDs.Add(4098403379U, "MessageInvalidTimeZoneTransitionGroupNullId");
			Strings.stringIDs.Add(3696858122U, "FailedCommunicationException");
			Strings.stringIDs.Add(3416023611U, "LogFieldsNumberUnsuccessfulMailboxes");
			Strings.stringIDs.Add(4244107904U, "SearchNotStarted");
			Strings.stringIDs.Add(2960157992U, "TrackingErrorConnectivity");
			Strings.stringIDs.Add(946397903U, "TargetFolder");
			Strings.stringIDs.Add(1774313355U, "Unsearchable");
			Strings.stringIDs.Add(3752482865U, "LogFieldsRecipients");
			Strings.stringIDs.Add(357978241U, "InvalidPreviewItemInResultRows");
			Strings.stringIDs.Add(2642055223U, "descInvalidSmptAddres");
			Strings.stringIDs.Add(2249986005U, "TrackingErrorBudgetExceeded");
			Strings.stringIDs.Add(1952477638U, "LogFieldsKeywordMbxs");
			Strings.stringIDs.Add(859073841U, "descQueryGenerationNotRequired");
			Strings.stringIDs.Add(3528487049U, "TrackingNoDefaultDomain");
			Strings.stringIDs.Add(457909195U, "InvalidChangeKeyReturned");
			Strings.stringIDs.Add(3832545148U, "ArchiveMailbox");
			Strings.stringIDs.Add(4235476896U, "LogFieldsKeywordHitCount");
			Strings.stringIDs.Add(824833090U, "LogFieldsName");
			Strings.stringIDs.Add(3196432253U, "LogFieldsStatus");
			Strings.stringIDs.Add(1572073070U, "LogFieldsEndDate");
			Strings.stringIDs.Add(1970283657U, "LogFieldsSenders");
			Strings.stringIDs.Add(1734826510U, "TrackingErrorTimeBudgetExceeded");
			Strings.stringIDs.Add(127105864U, "TrackingLogVersionIncompatible");
			Strings.stringIDs.Add(4195952492U, "AutodiscoverFailedException");
			Strings.stringIDs.Add(215769503U, "descInvalidMaxNonWorkHourResultsPerDay");
			Strings.stringIDs.Add(1477200533U, "descNullAutoDiscoverResponse");
			Strings.stringIDs.Add(1446717588U, "UnexpectedRemoteDataException");
			Strings.stringIDs.Add(4270171893U, "LogFieldsIncludeKeywordStatistics");
			Strings.stringIDs.Add(2870421805U, "UnexpectedError");
			Strings.stringIDs.Add(2171828426U, "descInvalidMaximumResults");
			Strings.stringIDs.Add(1023107612U, "descOOFCannotReadOofConfigData");
			Strings.stringIDs.Add(366714169U, "descInvalidSecurityDescriptor");
			Strings.stringIDs.Add(540675128U, "InvalidResultMerge");
			Strings.stringIDs.Add(252553502U, "SearchObjectNotFound");
			Strings.stringIDs.Add(1359157944U, "TrackingInstanceBudgetExceeded");
			Strings.stringIDs.Add(3961981453U, "MessageInvalidTimeZoneOutOfRange");
			Strings.stringIDs.Add(3014888409U, "LogFieldsStartDate");
			Strings.stringIDs.Add(273956641U, "NotOperator");
			Strings.stringIDs.Add(959712720U, "LogFieldsSourceRecipients");
			Strings.stringIDs.Add(1261494857U, "TrackingErrorQueueViewerRpc");
			Strings.stringIDs.Add(2084277545U, "SearchLogHeader");
			Strings.stringIDs.Add(609234870U, "MessageInvalidTimeZoneInvalidOffsetFormat");
			Strings.stringIDs.Add(669181240U, "descInvalidCredentials");
			Strings.stringIDs.Add(3577556142U, "LogFieldsSearchOperation");
			Strings.stringIDs.Add(3644766027U, "MessageInvalidTimeZoneNonFirstTransition");
			Strings.stringIDs.Add(184705256U, "descMeetingSuggestionsDurationTooSmall");
			Strings.stringIDs.Add(1884462766U, "DeleteItemsFailed");
			Strings.stringIDs.Add(2550395136U, "PendingSynchronizationException");
			Strings.stringIDs.Add(4277240717U, "LogFieldsNumberMailboxesToSearch");
			Strings.stringIDs.Add(2182808069U, "LogFieldsKeywordKeyword");
			Strings.stringIDs.Add(2422079678U, "ObjectNotFound");
			Strings.stringIDs.Add(3865092385U, "MessageInvalidTimeZoneMissedPeriod");
			Strings.stringIDs.Add(3051609629U, "LogFieldsResultSize");
			Strings.stringIDs.Add(447822483U, "MessageInvalidTimeZoneDayOfWeekValue");
			Strings.stringIDs.Add(3099813970U, "TrackingBusy");
			Strings.stringIDs.Add(3278774409U, "InvalidAppointmentException");
			Strings.stringIDs.Add(1105384606U, "descInvalidNetworkServiceContext");
			Strings.stringIDs.Add(1524653606U, "ResultsNotDeduped");
			Strings.stringIDs.Add(610144303U, "ErrorTimeZone");
			Strings.stringIDs.Add(2456116760U, "BatchSynchronizationFailedException");
			Strings.stringIDs.Add(876631183U, "descStartAndEndTimesOutSideFreeBusyData");
			Strings.stringIDs.Add(4252318527U, "descWin32InteropError");
			Strings.stringIDs.Add(3219875537U, "TrackingErrorLegacySender");
			Strings.stringIDs.Add(3436346834U, "descNoEwsResponse");
			Strings.stringIDs.Add(3301704787U, "LogFieldsResultsLink");
			Strings.stringIDs.Add(2977121749U, "TrackingErrorLegacySenderMultiMessageSearch");
			Strings.stringIDs.Add(2205699811U, "LogFieldsManagedBy");
			Strings.stringIDs.Add(1676524031U, "KeywordHitEmptyQuery");
			Strings.stringIDs.Add(2152565173U, "SearchQueryEmpty");
			Strings.stringIDs.Add(2705707569U, "ServerShutdown");
			Strings.stringIDs.Add(377459750U, "TrackingErrorSuffixForAdministrator");
			Strings.stringIDs.Add(4273761671U, "SubscriptionNotFoundException");
			Strings.stringIDs.Add(2881797395U, "LogFieldsResultSizeEstimate");
			Strings.stringIDs.Add(3863825871U, "descNullDateInChangeDate");
			Strings.stringIDs.Add(232000386U, "SharingFolderNotFoundException");
			Strings.stringIDs.Add(1607453502U, "LogMailAll");
			Strings.stringIDs.Add(3106977401U, "descInvalidScheduledOofDuration");
			Strings.stringIDs.Add(1899968567U, "TrackingTransientErrorMultiMessageSearch");
			Strings.stringIDs.Add(2680389304U, "AqsParserError");
			Strings.stringIDs.Add(829039958U, "LegacyOofMessage");
			Strings.stringIDs.Add(118976146U, "descInvalidClientSecurityContext");
			Strings.stringIDs.Add(3912319220U, "LogMailFooter");
			Strings.stringIDs.Add(2822320878U, "descInvalidFormatInEmail");
			Strings.stringIDs.Add(2977444274U, "descNoCalendar");
			Strings.stringIDs.Add(1018641786U, "UnexpectedUserResponses");
			Strings.stringIDs.Add(2215464039U, "LogFieldsIdentity");
			Strings.stringIDs.Add(3854866410U, "FreeBusyApplicationName");
			Strings.stringIDs.Add(2236533269U, "SearchAborted");
			Strings.stringIDs.Add(1756460035U, "AutodiscoverTimedOut");
			Strings.stringIDs.Add(3871609939U, "LogFieldsSearchDumpster");
			Strings.stringIDs.Add(274121238U, "descInvalidAuthorizationContext");
			Strings.stringIDs.Add(4205846133U, "LogFieldsMessageTypes");
			Strings.stringIDs.Add(3832116504U, "PositiveParameter");
			Strings.stringIDs.Add(1114362743U, "LogMailSeeAttachment");
			Strings.stringIDs.Add(3835343804U, "descMeetingSuggestionsInvalidTimeInterval");
			Strings.stringIDs.Add(2769462193U, "KeywordStatsNotRequested");
			Strings.stringIDs.Add(1382860876U, "RbacTargetMailboxAuthorizationError");
			Strings.stringIDs.Add(636866827U, "SortedResultNullParameters");
			Strings.stringIDs.Add(1214298741U, "LogFieldsLastEndTime");
			Strings.stringIDs.Add(55350379U, "TrackingErrorInvalidADData");
			Strings.stringIDs.Add(3068064914U, "LogFieldsResume");
			Strings.stringIDs.Add(3770945073U, "descWorkHoursStartEndInvalid");
			Strings.stringIDs.Add(872236584U, "SearchAlreadStarted");
			Strings.stringIDs.Add(908708480U, "MessageInvalidTimeZoneReferenceToPeriod");
			Strings.stringIDs.Add(489766921U, "ProgressSearchingInProgress");
			Strings.stringIDs.Add(3558390196U, "descCorruptUserOofSettingsXmlDocument");
			Strings.stringIDs.Add(1295345518U, "CrossForestNotSupported");
			Strings.stringIDs.Add(3240652606U, "LogFieldsNumberSuccessfulMailboxes");
			Strings.stringIDs.Add(1087048587U, "descIdentityArrayEmpty");
			Strings.stringIDs.Add(2265146620U, "MessageInvalidTimeZoneReferenceToGroup");
			Strings.stringIDs.Add(1894419184U, "descInvalidTimeZoneBias");
			Strings.stringIDs.Add(1223102710U, "descPublicFolderServerNotFound");
			Strings.stringIDs.Add(2319913820U, "TrackingErrorCrossPremiseAuthentication");
			Strings.stringIDs.Add(3558532322U, "MessageInvalidTimeZonePeriodNullId");
			Strings.stringIDs.Add(147513433U, "StorePermanantError");
			Strings.stringIDs.Add(3376565836U, "TrackingErrorTransientUnexpected");
			Strings.stringIDs.Add(1389565281U, "TrackingErrorModerationDecisionLogsFromE14Rtm");
			Strings.stringIDs.Add(3931032456U, "TrackingErrorCASUriDiscovery");
			Strings.stringIDs.Add(2307745798U, "descFailedToFindPublicFolderServer");
			Strings.stringIDs.Add(1664401949U, "MessageTrackingApplicationName");
			Strings.stringIDs.Add(3586747816U, "descAutoDiscoverThruDirectoryFailed");
			Strings.stringIDs.Add(468011246U, "MessageInvalidTimeZoneDuplicatePeriods");
			Strings.stringIDs.Add(1053420363U, "LogFieldsPercentComplete");
			Strings.stringIDs.Add(863256271U, "descFreeBusyAndSuggestionsNull");
			Strings.stringIDs.Add(3837654127U, "LogFieldsSuccessfulMailboxes");
			Strings.stringIDs.Add(534171625U, "UninstallAssistantsServiceTask");
			Strings.stringIDs.Add(2865084951U, "ProgressSearching");
			Strings.stringIDs.Add(193137347U, "SearchInvalidPagination");
			Strings.stringIDs.Add(2006533237U, "LogFieldsSearchQuery");
			Strings.stringIDs.Add(1759034327U, "TrackingTotalBudgetExceeded");
			Strings.stringIDs.Add(2697293287U, "descSuggestionMustStartOnThirtyMinuteBoundary");
			Strings.stringIDs.Add(3124261155U, "SearchServerShutdown");
			Strings.stringIDs.Add(4107635786U, "descFailedToCreateLegacyOofRule");
			Strings.stringIDs.Add(4123771088U, "PhotosApplicationName");
			Strings.stringIDs.Add(1880771860U, "ADUserNotFoundException");
			Strings.stringIDs.Add(4285214785U, "ProgressSearchingSources");
			Strings.stringIDs.Add(2804157263U, "LogMailNone");
			Strings.stringIDs.Add(1233757588U, "LogFieldsStatusMailRecipients");
			Strings.stringIDs.Add(3305370967U, "LogFieldsResultNumber");
			Strings.stringIDs.Add(4287340460U, "TrackingErrorPermanentUnexpected");
			Strings.stringIDs.Add(1954290077U, "ProgressCompleting");
			Strings.stringIDs.Add(270334674U, "descInvalidUserOofSettings");
			Strings.stringIDs.Add(1275939085U, "LogFieldsResultNumberEstimate");
			Strings.stringIDs.Add(3222940042U, "LogFieldsKeywordHits");
			Strings.stringIDs.Add(3351215994U, "UnknownError");
			Strings.stringIDs.Add(1265936670U, "MessageInvalidTimeZonePeriodNullBias");
			Strings.stringIDs.Add(2937454784U, "MailboxSeachCountIncludeUnsearchable");
			Strings.stringIDs.Add(859087807U, "LogMailBlank");
			Strings.stringIDs.Add(3091073294U, "descWorkHoursEndTimeInvalid");
			Strings.stringIDs.Add(2858750991U, "ProgessCreatingThreads");
			Strings.stringIDs.Add(2569697819U, "descOOFCannotReadExternalOofOptions");
			Strings.stringIDs.Add(1910512436U, "PrimaryMailbox");
			Strings.stringIDs.Add(2852570616U, "MessageInvalidTimeZoneCustomTimeZoneThreeElements");
			Strings.stringIDs.Add(763976563U, "TrackingTransientError");
			Strings.stringIDs.Add(1809134602U, "LogFieldsLastStartTime");
			Strings.stringIDs.Add(1824636832U, "CopyItemsFailed");
			Strings.stringIDs.Add(1910425077U, "DummyApplicationName");
			Strings.stringIDs.Add(3276944824U, "MessageInvalidTimeZoneDuplicateGroups");
			Strings.stringIDs.Add(1614878877U, "ExecutingUserNeedSmtpAddress");
			Strings.stringIDs.Add(3442221872U, "MessageInvalidTimeZoneMoreThanTwoPeriodsUnsupported");
			Strings.stringIDs.Add(3955434964U, "descMailRecipientNotFound");
			Strings.stringIDs.Add(2671831934U, "descNullUserName");
			Strings.stringIDs.Add(1407995413U, "NonNegativeParameter");
			Strings.stringIDs.Add(1633879312U, "LogFieldsEstimateNotExcludeDuplicates");
			Strings.stringIDs.Add(18603439U, "StoreTransientError");
			Strings.stringIDs.Add(1541817855U, "LogFieldsLogLevel");
			Strings.stringIDs.Add(2131297471U, "TrackingWSRequestCorrupt");
			Strings.stringIDs.Add(2936774726U, "SearchDisabled");
			Strings.stringIDs.Add(1705124535U, "descNullCredentialsToServiceDiscoveryRequest");
			Strings.stringIDs.Add(697045372U, "LogFieldsUnsuccessfulMailboxes");
			Strings.stringIDs.Add(2530022313U, "MessageInvalidTimeZoneTimeZoneNotFound");
			Strings.stringIDs.Add(3753969000U, "MessageInvalidTimeZoneIntValueIsInvalid");
			Strings.stringIDs.Add(3488925402U, "ScheduleConfigurationSchedule");
			Strings.stringIDs.Add(3675304455U, "descOofRuleSaveException");
			Strings.stringIDs.Add(3342799224U, "LogFieldsTargetMailbox");
			Strings.stringIDs.Add(1441197009U, "descInvalidSuggestionsTimeRange");
			Strings.stringIDs.Add(670434636U, "descFailedToGetUserOofPolicy");
			Strings.stringIDs.Add(983402700U, "descMeetingSuggestionsDurationTooLarge");
			Strings.stringIDs.Add(2411109197U, "ProgressOpening");
			Strings.stringIDs.Add(1471504673U, "TrackingErrorBudgetExceededMultiMessageSearch");
			Strings.stringIDs.Add(2991551487U, "descLocalServerObjectNotFound");
			Strings.stringIDs.Add(2706524390U, "RecoverableItems");
			Strings.stringIDs.Add(3367175223U, "descProxyRequestFailed");
			Strings.stringIDs.Add(516398220U, "ProgessCreating");
			Strings.stringIDs.Add(924370705U, "descClientDisconnected");
			Strings.stringIDs.Add(3760653578U, "SearchThrottled");
			Strings.stringIDs.Add(1620607520U, "descInvalidTransitionTime");
			Strings.stringIDs.Add(2976781692U, "TargetMailboxOutOfSpace");
			Strings.stringIDs.Add(1848520589U, "LogFieldsLastRunBy");
			Strings.stringIDs.Add(4135638738U, "UnableToReadServiceTopology");
			Strings.stringIDs.Add(607437832U, "descElcRootFolderName");
			Strings.stringIDs.Add(647128549U, "descNoMailTipsInEwsResponseMessage");
			Strings.stringIDs.Add(4209067056U, "descFailedToGetRules");
			Strings.stringIDs.Add(794348359U, "LogFieldsStoppedBy");
			Strings.stringIDs.Add(2268440642U, "descProxyRequestProcessingSocketError");
			Strings.stringIDs.Add(3466285021U, "descProxyRequestNotAllowed");
			Strings.stringIDs.Add(1848500058U, "descInvalidAccessLevel");
			Strings.stringIDs.Add(784192305U, "TrackingPermanentErrorMultiMessageSearch");
			Strings.stringIDs.Add(2804255286U, "LogFieldsCreatedBy");
			Strings.stringIDs.Add(1225651387U, "LowSystemResource");
			Strings.stringIDs.Add(854567314U, "TrackingErrorLogSearchServiceDown");
			Strings.stringIDs.Add(2312386357U, "LogMailNotApplicable");
			Strings.stringIDs.Add(3289802537U, "descNoFreeBusyAccess");
			Strings.stringIDs.Add(348874544U, "ADUserMisconfiguredException");
			Strings.stringIDs.Add(2033796534U, "ProgressOpeningTarget");
			Strings.stringIDs.Add(1554201361U, "descOOFVirusDetectedOofReplyMessage");
			Strings.stringIDs.Add(2912574056U, "MessageInvalidTimeZonePeriodNullName");
			Strings.stringIDs.Add(3332140560U, "MessageInvalidTimeZoneFirstTransition");
			Strings.stringIDs.Add(3216622764U, "InvalidContactException");
			Strings.stringIDs.Add(3436010521U, "ErrorRemoveOngoingSearch");
			Strings.stringIDs.Add(1594184545U, "TrackingErrorReadStatus");
			Strings.stringIDs.Add(3134958540U, "TrackingErrorCrossForestAuthentication");
			Strings.stringIDs.Add(1892050364U, "descInvalidFreeBusyViewType");
			Strings.stringIDs.Add(3341082622U, "descLogonAsNetworkServiceFailed");
			Strings.stringIDs.Add(2837247303U, "TrackingErrorCrossForestMisconfiguration");
			Strings.stringIDs.Add(2100288003U, "MessageInvalidTimeZoneMissedGroup");
			Strings.stringIDs.Add(478692077U, "NoKeywordStatsForCopySearch");
			Strings.stringIDs.Add(3248017497U, "descMailboxFailover");
			Strings.stringIDs.Add(1104837598U, "LogFieldsExcludeDuplicateMessages");
			Strings.stringIDs.Add(1743551038U, "LogFieldsErrors");
			Strings.stringIDs.Add(3607788283U, "mailTipsTenant");
			Strings.stringIDs.Add(3153221581U, "SearchArgument");
			Strings.stringIDs.Add(3176897035U, "TrackingErrorCrossPremiseMisconfigurationMultiMessageSearch");
			Strings.stringIDs.Add(2497578736U, "descNullEmailToAutoDiscoverRequest");
		}

		public static LocalizedString TrackingErrorCrossPremiseMisconfiguration
		{
			get
			{
				return new LocalizedString("TrackingErrorCrossPremiseMisconfiguration", "Ex162699", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchServerFailed(string displayName, int code, string message)
		{
			return new LocalizedString("SearchServerFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				code,
				message
			});
		}

		public static LocalizedString descElcCannotFindDefaultFolder(string folderName, string mailboxName)
		{
			return new LocalizedString("descElcCannotFindDefaultFolder", "ExDFC4E4", false, true, Strings.ResourceManager, new object[]
			{
				folderName,
				mailboxName
			});
		}

		public static LocalizedString LogMailHeaderInstructions(string name)
		{
			return new LocalizedString("LogMailHeaderInstructions", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LogFieldsResultSizeCopied
		{
			get
			{
				return new LocalizedString("LogFieldsResultSizeCopied", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descPublicFolderRequestProcessingError(string exceptionMessage, string currentRequestString)
		{
			return new LocalizedString("descPublicFolderRequestProcessingError", "ExE57497", false, true, Strings.ResourceManager, new object[]
			{
				exceptionMessage,
				currentRequestString
			});
		}

		public static LocalizedString MailtipsApplicationName
		{
			get
			{
				return new LocalizedString("MailtipsApplicationName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descConfigurationInformationNotFound(string dnsDomainName)
		{
			return new LocalizedString("descConfigurationInformationNotFound", "Ex49339A", false, true, Strings.ResourceManager, new object[]
			{
				dnsDomainName
			});
		}

		public static LocalizedString descNotDefaultCalendar
		{
			get
			{
				return new LocalizedString("descNotDefaultCalendar", "ExCD1E1C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingPermanentError
		{
			get
			{
				return new LocalizedString("TrackingPermanentError", "Ex99D8B3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidReferenceItemInPreviewResult(string url)
		{
			return new LocalizedString("InvalidReferenceItemInPreviewResult", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString WrongTargetServer
		{
			get
			{
				return new LocalizedString("WrongTargetServer", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descVirusScanInProgress(string emailAddress)
		{
			return new LocalizedString("descVirusScanInProgress", "ExB58C61", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString InvalidSearchQuery
		{
			get
			{
				return new LocalizedString("InvalidSearchQuery", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InconsistentSortedResults(string mailboxGuid, string referenceItem, string item)
		{
			return new LocalizedString("InconsistentSortedResults", "", false, false, Strings.ResourceManager, new object[]
			{
				mailboxGuid,
				referenceItem,
				item
			});
		}

		public static LocalizedString InvalidSortedResultParameter(string first, string second)
		{
			return new LocalizedString("InvalidSortedResultParameter", "", false, false, Strings.ResourceManager, new object[]
			{
				first,
				second
			});
		}

		public static LocalizedString ProgressCompletingSearch
		{
			get
			{
				return new LocalizedString("ProgressCompletingSearch", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidOwaUrlInPreviewResult(string errorHint, string url)
		{
			return new LocalizedString("InvalidOwaUrlInPreviewResult", "", false, false, Strings.ResourceManager, new object[]
			{
				errorHint,
				url
			});
		}

		public static LocalizedString DeleteItemFailedForMessage(string exception)
		{
			return new LocalizedString("DeleteItemFailedForMessage", "", false, false, Strings.ResourceManager, new object[]
			{
				exception
			});
		}

		public static LocalizedString descWorkHoursStartTimeInvalid
		{
			get
			{
				return new LocalizedString("descWorkHoursStartTimeInvalid", "Ex9A9647", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMissingMailboxArrayElement(string index)
		{
			return new LocalizedString("descMissingMailboxArrayElement", "ExAC1F58", false, true, Strings.ResourceManager, new object[]
			{
				index
			});
		}

		public static LocalizedString descMailboxLogonFailed
		{
			get
			{
				return new LocalizedString("descMailboxLogonFailed", "Ex210994", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InstallAssistantsServiceTask
		{
			get
			{
				return new LocalizedString("InstallAssistantsServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZoneTransitionGroupNullId
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneTransitionGroupNullId", "Ex63A053", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedCommunicationException
		{
			get
			{
				return new LocalizedString("FailedCommunicationException", "Ex1F3BFA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsNumberUnsuccessfulMailboxes
		{
			get
			{
				return new LocalizedString("LogFieldsNumberUnsuccessfulMailboxes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMisconfiguredOrganizationRelationship(string organizationRelationship)
		{
			return new LocalizedString("descMisconfiguredOrganizationRelationship", "", false, false, Strings.ResourceManager, new object[]
			{
				organizationRelationship
			});
		}

		public static LocalizedString SearchNotStarted
		{
			get
			{
				return new LocalizedString("SearchNotStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorConnectivity
		{
			get
			{
				return new LocalizedString("TrackingErrorConnectivity", "Ex3D9B9F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNotAGroupOrUserOrContact(string emailAddress)
		{
			return new LocalizedString("descNotAGroupOrUserOrContact", "Ex2F4000", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString TargetFolder
		{
			get
			{
				return new LocalizedString("TargetFolder", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descProxyForPersonalNotAllowed(string recipient)
		{
			return new LocalizedString("descProxyForPersonalNotAllowed", "Ex0FB03A", false, true, Strings.ResourceManager, new object[]
			{
				recipient
			});
		}

		public static LocalizedString Unsearchable
		{
			get
			{
				return new LocalizedString("Unsearchable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsRecipients
		{
			get
			{
				return new LocalizedString("LogFieldsRecipients", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPreviewItemInResultRows
		{
			get
			{
				return new LocalizedString("InvalidPreviewItemInResultRows", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidSmptAddres
		{
			get
			{
				return new LocalizedString("descInvalidSmptAddres", "ExD96D91", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorBudgetExceeded
		{
			get
			{
				return new LocalizedString("TrackingErrorBudgetExceeded", "ExFCC67B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsKeywordMbxs
		{
			get
			{
				return new LocalizedString("LogFieldsKeywordMbxs", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownDayOfWeek(string dayOfWeek)
		{
			return new LocalizedString("UnknownDayOfWeek", "Ex2DDE2A", false, true, Strings.ResourceManager, new object[]
			{
				dayOfWeek
			});
		}

		public static LocalizedString descInvalidMonth(int min, int max)
		{
			return new LocalizedString("descInvalidMonth", "", false, false, Strings.ResourceManager, new object[]
			{
				min,
				max
			});
		}

		public static LocalizedString InvalidKeywordStatsRequest(string mdbGuid, string server)
		{
			return new LocalizedString("InvalidKeywordStatsRequest", "", false, false, Strings.ResourceManager, new object[]
			{
				mdbGuid,
				server
			});
		}

		public static LocalizedString descQueryGenerationNotRequired
		{
			get
			{
				return new LocalizedString("descQueryGenerationNotRequired", "ExC90C22", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingNoDefaultDomain
		{
			get
			{
				return new LocalizedString("TrackingNoDefaultDomain", "ExD1A01B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhotoRetrievalFailedUnauthorizedAccessError(string innerExceptionMessage)
		{
			return new LocalizedString("PhotoRetrievalFailedUnauthorizedAccessError", "", false, false, Strings.ResourceManager, new object[]
			{
				innerExceptionMessage
			});
		}

		public static LocalizedString ArgumentValidationFailedException(string argumentName)
		{
			return new LocalizedString("ArgumentValidationFailedException", "", false, false, Strings.ResourceManager, new object[]
			{
				argumentName
			});
		}

		public static LocalizedString descDeliveryRestricted(string emailAddress)
		{
			return new LocalizedString("descDeliveryRestricted", "ExF083C1", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString UnknownRecurrenceRange(string range)
		{
			return new LocalizedString("UnknownRecurrenceRange", "Ex69C13A", false, true, Strings.ResourceManager, new object[]
			{
				range
			});
		}

		public static LocalizedString InvalidChangeKeyReturned
		{
			get
			{
				return new LocalizedString("InvalidChangeKeyReturned", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogMailHeader(string name, string status)
		{
			return new LocalizedString("LogMailHeader", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				status
			});
		}

		public static LocalizedString ArchiveMailbox
		{
			get
			{
				return new LocalizedString("ArchiveMailbox", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsKeywordHitCount
		{
			get
			{
				return new LocalizedString("LogFieldsKeywordHitCount", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMailTipsSenderNotUnique(string address)
		{
			return new LocalizedString("descMailTipsSenderNotUnique", "Ex462EA0", false, true, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString LogFieldsName
		{
			get
			{
				return new LocalizedString("LogFieldsName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsStatus
		{
			get
			{
				return new LocalizedString("LogFieldsStatus", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsEndDate
		{
			get
			{
				return new LocalizedString("LogFieldsEndDate", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFailedMailboxesResultDuplicateEntries(string url)
		{
			return new LocalizedString("InvalidFailedMailboxesResultDuplicateEntries", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString RbacSourceMailboxAuthorizationError(string name)
		{
			return new LocalizedString("RbacSourceMailboxAuthorizationError", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LogFieldsSenders
		{
			get
			{
				return new LocalizedString("LogFieldsSenders", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorTimeBudgetExceeded
		{
			get
			{
				return new LocalizedString("TrackingErrorTimeBudgetExceeded", "Ex53FB82", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingLogVersionIncompatible
		{
			get
			{
				return new LocalizedString("TrackingLogVersionIncompatible", "Ex0A6A33", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descUnknownDefFolder(string adFolder, string mailbox)
		{
			return new LocalizedString("descUnknownDefFolder", "Ex40A559", false, true, Strings.ResourceManager, new object[]
			{
				adFolder,
				mailbox
			});
		}

		public static LocalizedString AutodiscoverFailedException
		{
			get
			{
				return new LocalizedString("AutodiscoverFailedException", "ExB63563", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidMaxNonWorkHourResultsPerDay
		{
			get
			{
				return new LocalizedString("descInvalidMaxNonWorkHourResultsPerDay", "Ex1A7FF0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNullAutoDiscoverResponse
		{
			get
			{
				return new LocalizedString("descNullAutoDiscoverResponse", "Ex182BA2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedRemoteDataException
		{
			get
			{
				return new LocalizedString("UnexpectedRemoteDataException", "ExA312C1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsIncludeKeywordStatistics
		{
			get
			{
				return new LocalizedString("LogFieldsIncludeKeywordStatistics", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnexpectedError
		{
			get
			{
				return new LocalizedString("UnexpectedError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidMaximumResults
		{
			get
			{
				return new LocalizedString("descInvalidMaximumResults", "ExD7AB2B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descOOFCannotReadOofConfigData
		{
			get
			{
				return new LocalizedString("descOOFCannotReadOofConfigData", "ExFFEE20", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descProxyRequestProcessingError(string exceptionMessage, string currentRequestString)
		{
			return new LocalizedString("descProxyRequestProcessingError", "ExCEEA5B", false, true, Strings.ResourceManager, new object[]
			{
				exceptionMessage,
				currentRequestString
			});
		}

		public static LocalizedString descInvalidSecurityDescriptor
		{
			get
			{
				return new LocalizedString("descInvalidSecurityDescriptor", "ExE95D66", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidResultMerge
		{
			get
			{
				return new LocalizedString("InvalidResultMerge", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchObjectNotFound
		{
			get
			{
				return new LocalizedString("SearchObjectNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descSoapAutoDiscoverRequestUserSettingError(string url, string settingName, string errorMessage)
		{
			return new LocalizedString("descSoapAutoDiscoverRequestUserSettingError", "ExE23D0D", false, true, Strings.ResourceManager, new object[]
			{
				url,
				settingName,
				errorMessage
			});
		}

		public static LocalizedString TrackingInstanceBudgetExceeded
		{
			get
			{
				return new LocalizedString("TrackingInstanceBudgetExceeded", "Ex2C68C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToCreateELCRoot(string mailboxName)
		{
			return new LocalizedString("descFailedToCreateELCRoot", "ExF5B44D", false, true, Strings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString MessageInvalidTimeZoneOutOfRange
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneOutOfRange", "Ex65F4C5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsStartDate
		{
			get
			{
				return new LocalizedString("LogFieldsStartDate", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NotOperator
		{
			get
			{
				return new LocalizedString("NotOperator", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descElcFolderExists(string folderName)
		{
			return new LocalizedString("descElcFolderExists", "ExB3CC72", false, true, Strings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString descFailedToCreateOneOrMoreOrganizationalFolders(string message)
		{
			return new LocalizedString("descFailedToCreateOneOrMoreOrganizationalFolders", "Ex9A45E4", false, true, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString LogFieldsSourceRecipients
		{
			get
			{
				return new LocalizedString("LogFieldsSourceRecipients", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorQueueViewerRpc
		{
			get
			{
				return new LocalizedString("TrackingErrorQueueViewerRpc", "Ex44F6D9", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchServerError(int errorCode)
		{
			return new LocalizedString("SearchServerError", "", false, false, Strings.ResourceManager, new object[]
			{
				errorCode
			});
		}

		public static LocalizedString InvalidItemHashInPreviewResult(string url)
		{
			return new LocalizedString("InvalidItemHashInPreviewResult", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString descFailedToSyncFolder(string folderName, string mailboxName)
		{
			return new LocalizedString("descFailedToSyncFolder", "ExCF9A93", false, true, Strings.ResourceManager, new object[]
			{
				folderName,
				mailboxName
			});
		}

		public static LocalizedString SearchLogHeader
		{
			get
			{
				return new LocalizedString("SearchLogHeader", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZoneInvalidOffsetFormat
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneInvalidOffsetFormat", "Ex0D4D83", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidCredentials
		{
			get
			{
				return new LocalizedString("descInvalidCredentials", "Ex4A709C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsSearchOperation
		{
			get
			{
				return new LocalizedString("LogFieldsSearchOperation", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxRefinerInRefinersSection(string url)
		{
			return new LocalizedString("MailboxRefinerInRefinersSection", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString descRequestStreamTooBig(string allowedSize, string actualSize)
		{
			return new LocalizedString("descRequestStreamTooBig", "Ex029E9A", false, true, Strings.ResourceManager, new object[]
			{
				allowedSize,
				actualSize
			});
		}

		public static LocalizedString MessageInvalidTimeZoneNonFirstTransition
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneNonFirstTransition", "Ex70E8B3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMeetingSuggestionsDurationTooSmall
		{
			get
			{
				return new LocalizedString("descMeetingSuggestionsDurationTooSmall", "Ex6BFC3C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descProtocolNotFoundInAutoDiscoverResponse(string protocol, string response)
		{
			return new LocalizedString("descProtocolNotFoundInAutoDiscoverResponse", "ExD0B00D", false, true, Strings.ResourceManager, new object[]
			{
				protocol,
				response
			});
		}

		public static LocalizedString DeleteItemsFailed
		{
			get
			{
				return new LocalizedString("DeleteItemsFailed", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToFindSearchObject(string id)
		{
			return new LocalizedString("UnableToFindSearchObject", "", false, false, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString PendingSynchronizationException
		{
			get
			{
				return new LocalizedString("PendingSynchronizationException", "Ex36C2F8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsNumberMailboxesToSearch
		{
			get
			{
				return new LocalizedString("LogFieldsNumberMailboxesToSearch", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMinimumRequiredVersionProxyServerNotFound(int currentVersion, int requiredVersion, string email)
		{
			return new LocalizedString("descMinimumRequiredVersionProxyServerNotFound", "ExBF1341", false, true, Strings.ResourceManager, new object[]
			{
				currentVersion,
				requiredVersion,
				email
			});
		}

		public static LocalizedString descIndividualMailboxLimitReached(string emailAddress, int mailboxCount)
		{
			return new LocalizedString("descIndividualMailboxLimitReached", "Ex396E68", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress,
				mailboxCount
			});
		}

		public static LocalizedString descTagNotInAD(string tagDN)
		{
			return new LocalizedString("descTagNotInAD", "ExCE2883", false, true, Strings.ResourceManager, new object[]
			{
				tagDN
			});
		}

		public static LocalizedString descAvailabilityAddressSpaceFailed(string id)
		{
			return new LocalizedString("descAvailabilityAddressSpaceFailed", "Ex481C63", false, true, Strings.ResourceManager, new object[]
			{
				id
			});
		}

		public static LocalizedString descUnsupportedSecurityDescriptorVersion(string mailboxAddress, string expectedVersion, string actualVersion)
		{
			return new LocalizedString("descUnsupportedSecurityDescriptorVersion", "ExF83A8E", false, true, Strings.ResourceManager, new object[]
			{
				mailboxAddress,
				expectedVersion,
				actualVersion
			});
		}

		public static LocalizedString SearchMailboxNotFound(string mailboxGuid, string databaseName, string server)
		{
			return new LocalizedString("SearchMailboxNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				mailboxGuid,
				databaseName,
				server
			});
		}

		public static LocalizedString SearchTaskCancelledPrimary(string displayName, string mailboxGuid)
		{
			return new LocalizedString("SearchTaskCancelledPrimary", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString descCannotResolveEmailAddress(string emailAddress)
		{
			return new LocalizedString("descCannotResolveEmailAddress", "ExB6A32C", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString LogFieldsKeywordKeyword
		{
			get
			{
				return new LocalizedString("LogFieldsKeywordKeyword", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ObjectNotFound
		{
			get
			{
				return new LocalizedString("ObjectNotFound", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMissingProperty(string propertyName, string unexpectedObject)
		{
			return new LocalizedString("descMissingProperty", "ExD328B9", false, true, Strings.ResourceManager, new object[]
			{
				propertyName,
				unexpectedObject
			});
		}

		public static LocalizedString MessageInvalidTimeZoneMissedPeriod
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneMissedPeriod", "ExF09C89", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsResultSize
		{
			get
			{
				return new LocalizedString("LogFieldsResultSize", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descAutoDiscoverFailed(string email)
		{
			return new LocalizedString("descAutoDiscoverFailed", "Ex28E399", false, true, Strings.ResourceManager, new object[]
			{
				email
			});
		}

		public static LocalizedString UnabledToLocateMailboxServer(string server)
		{
			return new LocalizedString("UnabledToLocateMailboxServer", "Ex15DAE8", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString ArchiveSearchPopulationFailed(string displayName, string mailboxGuid)
		{
			return new LocalizedString("ArchiveSearchPopulationFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString MessageInvalidTimeZoneDayOfWeekValue
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneDayOfWeekValue", "ExD43B58", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidSearchRequest(string mdbGuid, string server)
		{
			return new LocalizedString("InvalidSearchRequest", "", false, false, Strings.ResourceManager, new object[]
			{
				mdbGuid,
				server
			});
		}

		public static LocalizedString DiscoverySearchCIFailure(string mdbGuid, string server, int errorCode)
		{
			return new LocalizedString("DiscoverySearchCIFailure", "", false, false, Strings.ResourceManager, new object[]
			{
				mdbGuid,
				server,
				errorCode
			});
		}

		public static LocalizedString descInvalidTargetAddress(string mailbox)
		{
			return new LocalizedString("descInvalidTargetAddress", "Ex11472D", false, true, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString TrackingBusy
		{
			get
			{
				return new LocalizedString("TrackingBusy", "Ex886638", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descCannotBindToElcRootFolder(string rootFolderId, string mailboxName)
		{
			return new LocalizedString("descCannotBindToElcRootFolder", "Ex77513F", false, true, Strings.ResourceManager, new object[]
			{
				rootFolderId,
				mailboxName
			});
		}

		public static LocalizedString descSoapAutoDiscoverRequestError(string url, string exceptionMessage)
		{
			return new LocalizedString("descSoapAutoDiscoverRequestError", "Ex4A38DB", false, true, Strings.ResourceManager, new object[]
			{
				url,
				exceptionMessage
			});
		}

		public static LocalizedString descAutoDiscoverRequestError(string exceptionMessage, string currentRequestString)
		{
			return new LocalizedString("descAutoDiscoverRequestError", "ExC47A40", false, true, Strings.ResourceManager, new object[]
			{
				exceptionMessage,
				currentRequestString
			});
		}

		public static LocalizedString InvalidAppointmentException
		{
			get
			{
				return new LocalizedString("InvalidAppointmentException", "Ex8CC700", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchOverBudget(int maximumSearches)
		{
			return new LocalizedString("SearchOverBudget", "", false, false, Strings.ResourceManager, new object[]
			{
				maximumSearches
			});
		}

		public static LocalizedString descUnknownWebRequestType(string request)
		{
			return new LocalizedString("descUnknownWebRequestType", "Ex622664", false, true, Strings.ResourceManager, new object[]
			{
				request
			});
		}

		public static LocalizedString descInvalidNetworkServiceContext
		{
			get
			{
				return new LocalizedString("descInvalidNetworkServiceContext", "ExB0EAC3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descIdentityArrayTooBig(string allowedSize, string actualSize)
		{
			return new LocalizedString("descIdentityArrayTooBig", "Ex74173A", false, true, Strings.ResourceManager, new object[]
			{
				allowedSize,
				actualSize
			});
		}

		public static LocalizedString ResultsNotDeduped
		{
			get
			{
				return new LocalizedString("ResultsNotDeduped", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorTimeZone
		{
			get
			{
				return new LocalizedString("ErrorTimeZone", "Ex53B2E7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingLogsCorruptOnServer(string server)
		{
			return new LocalizedString("TrackingLogsCorruptOnServer", "ExC58339", false, true, Strings.ResourceManager, new object[]
			{
				server
			});
		}

		public static LocalizedString BatchSynchronizationFailedException
		{
			get
			{
				return new LocalizedString("BatchSynchronizationFailedException", "ExDA1698", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descStartAndEndTimesOutSideFreeBusyData
		{
			get
			{
				return new LocalizedString("descStartAndEndTimesOutSideFreeBusyData", "Ex7F4DE6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descWin32InteropError
		{
			get
			{
				return new LocalizedString("descWin32InteropError", "Ex252861", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descExceededMaxRedirectionDepth(string email, int maxDepth)
		{
			return new LocalizedString("descExceededMaxRedirectionDepth", "Ex89C252", false, true, Strings.ResourceManager, new object[]
			{
				email,
				maxDepth
			});
		}

		public static LocalizedString SearchNonFullTextSortByProperty(string sortByProperty)
		{
			return new LocalizedString("SearchNonFullTextSortByProperty", "", false, false, Strings.ResourceManager, new object[]
			{
				sortByProperty
			});
		}

		public static LocalizedString TrackingErrorLegacySender
		{
			get
			{
				return new LocalizedString("TrackingErrorLegacySender", "Ex19374A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descCrossForestServiceMissing(string mailbox)
		{
			return new LocalizedString("descCrossForestServiceMissing", "Ex86F5F2", false, true, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString descNoEwsResponse
		{
			get
			{
				return new LocalizedString("descNoEwsResponse", "Ex00AF6A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CrossServerCallFailed(string errorHint, string errorCode, string errorMessage)
		{
			return new LocalizedString("CrossServerCallFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				errorHint,
				errorCode,
				errorMessage
			});
		}

		public static LocalizedString PrimarySearchFolderTimeout(string displayName, string mailboxGuid)
		{
			return new LocalizedString("PrimarySearchFolderTimeout", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString LogFieldsResultsLink
		{
			get
			{
				return new LocalizedString("LogFieldsResultsLink", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorUserObjectCorrupt(string user, string attribute)
		{
			return new LocalizedString("TrackingErrorUserObjectCorrupt", "ExB2C799", false, true, Strings.ResourceManager, new object[]
			{
				user,
				attribute
			});
		}

		public static LocalizedString descVirusDetected(string emailAddress)
		{
			return new LocalizedString("descVirusDetected", "ExD6DC3B", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString FailedToConvertSearchCriteriaToRestriction(string query, string database, string failure)
		{
			return new LocalizedString("FailedToConvertSearchCriteriaToRestriction", "", false, false, Strings.ResourceManager, new object[]
			{
				query,
				database,
				failure
			});
		}

		public static LocalizedString PrimarySearchPopulationFailed(string displayName, string mailboxGuid)
		{
			return new LocalizedString("PrimarySearchPopulationFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString TrackingErrorLegacySenderMultiMessageSearch
		{
			get
			{
				return new LocalizedString("TrackingErrorLegacySenderMultiMessageSearch", "ExAE0B5D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsManagedBy
		{
			get
			{
				return new LocalizedString("LogFieldsManagedBy", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KeywordHitEmptyQuery
		{
			get
			{
				return new LocalizedString("KeywordHitEmptyQuery", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KeywordStatisticsSearchDisabled(int maximumMailboxes)
		{
			return new LocalizedString("KeywordStatisticsSearchDisabled", "", false, false, Strings.ResourceManager, new object[]
			{
				maximumMailboxes
			});
		}

		public static LocalizedString descInvalidTypeInBookingPolicyConfig(string type, string parameter)
		{
			return new LocalizedString("descInvalidTypeInBookingPolicyConfig", "Ex4B544F", false, true, Strings.ResourceManager, new object[]
			{
				type,
				parameter
			});
		}

		public static LocalizedString SearchQueryEmpty
		{
			get
			{
				return new LocalizedString("SearchQueryEmpty", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ServerShutdown
		{
			get
			{
				return new LocalizedString("ServerShutdown", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorSuffixForAdministrator
		{
			get
			{
				return new LocalizedString("TrackingErrorSuffixForAdministrator", "Ex98DD8C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SubscriptionNotFoundException
		{
			get
			{
				return new LocalizedString("SubscriptionNotFoundException", "Ex0935AF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyMailboxStatsServerResponse(string url)
		{
			return new LocalizedString("EmptyMailboxStatsServerResponse", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString PreviewSearchDisabled(int maxPreviewMailboxes, int maxResultsOnlyMailboxes)
		{
			return new LocalizedString("PreviewSearchDisabled", "", false, false, Strings.ResourceManager, new object[]
			{
				maxPreviewMailboxes,
				maxResultsOnlyMailboxes
			});
		}

		public static LocalizedString LogFieldsResultSizeEstimate
		{
			get
			{
				return new LocalizedString("LogFieldsResultSizeEstimate", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNullDateInChangeDate
		{
			get
			{
				return new LocalizedString("descNullDateInChangeDate", "Ex3632C8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SharingFolderNotFoundException
		{
			get
			{
				return new LocalizedString("SharingFolderNotFoundException", "ExAD861F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToCreateOrganizationalFolder(string folderName, string mailboxName)
		{
			return new LocalizedString("descFailedToCreateOrganizationalFolder", "Ex1F3461", false, true, Strings.ResourceManager, new object[]
			{
				folderName,
				mailboxName
			});
		}

		public static LocalizedString LogMailAll
		{
			get
			{
				return new LocalizedString("LogMailAll", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ArchiveSearchFolderTimeout(string displayName, string mailboxGuid)
		{
			return new LocalizedString("ArchiveSearchFolderTimeout", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString descInvalidScheduledOofDuration
		{
			get
			{
				return new LocalizedString("descInvalidScheduledOofDuration", "ExCC478D", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descSoapAutoDiscoverRequestUserSettingInvalidError(string url, string settingName)
		{
			return new LocalizedString("descSoapAutoDiscoverRequestUserSettingInvalidError", "ExB106D1", false, true, Strings.ResourceManager, new object[]
			{
				url,
				settingName
			});
		}

		public static LocalizedString descInvalidSmtpAddress(string emailAddress)
		{
			return new LocalizedString("descInvalidSmtpAddress", "Ex2B6F09", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString TrackingTransientErrorMultiMessageSearch
		{
			get
			{
				return new LocalizedString("TrackingTransientErrorMultiMessageSearch", "Ex693ED5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString AqsParserError
		{
			get
			{
				return new LocalizedString("AqsParserError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LegacyOofMessage
		{
			get
			{
				return new LocalizedString("LegacyOofMessage", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidClientSecurityContext
		{
			get
			{
				return new LocalizedString("descInvalidClientSecurityContext", "ExEDC8FE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogMailFooter
		{
			get
			{
				return new LocalizedString("LogMailFooter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidFormatInEmail
		{
			get
			{
				return new LocalizedString("descInvalidFormatInEmail", "ExEE9BA0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNoCalendar
		{
			get
			{
				return new LocalizedString("descNoCalendar", "Ex85D1EA", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DiscoverySearchAborted(string queryCorrelationId, string mdbGuid, string server)
		{
			return new LocalizedString("DiscoverySearchAborted", "", false, false, Strings.ResourceManager, new object[]
			{
				queryCorrelationId,
				mdbGuid,
				server
			});
		}

		public static LocalizedString UnexpectedUserResponses
		{
			get
			{
				return new LocalizedString("UnexpectedUserResponses", "Ex3F8449", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsIdentity
		{
			get
			{
				return new LocalizedString("LogFieldsIdentity", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FreeBusyApplicationName
		{
			get
			{
				return new LocalizedString("FreeBusyApplicationName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descDateMustHaveZeroTimeSpan(string argument)
		{
			return new LocalizedString("descDateMustHaveZeroTimeSpan", "Ex8C6BED", false, true, Strings.ResourceManager, new object[]
			{
				argument
			});
		}

		public static LocalizedString SearchAborted
		{
			get
			{
				return new LocalizedString("SearchAborted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchServerErrorMessage(string message)
		{
			return new LocalizedString("SearchServerErrorMessage", "", false, false, Strings.ResourceManager, new object[]
			{
				message
			});
		}

		public static LocalizedString SearchInvalidSortSpecification(string sortByProperty)
		{
			return new LocalizedString("SearchInvalidSortSpecification", "", false, false, Strings.ResourceManager, new object[]
			{
				sortByProperty
			});
		}

		public static LocalizedString InvalidRecipientArrayInPreviewResult(string url)
		{
			return new LocalizedString("InvalidRecipientArrayInPreviewResult", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString SearchTooManyMailboxes(int search)
		{
			return new LocalizedString("SearchTooManyMailboxes", "", false, false, Strings.ResourceManager, new object[]
			{
				search
			});
		}

		public static LocalizedString InvalidPreviewSearchResults(string url)
		{
			return new LocalizedString("InvalidPreviewSearchResults", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString AutodiscoverTimedOut
		{
			get
			{
				return new LocalizedString("AutodiscoverTimedOut", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsSearchDumpster
		{
			get
			{
				return new LocalizedString("LogFieldsSearchDumpster", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descRecipientVersionNotSupported(string email, long version, long minimumVersion)
		{
			return new LocalizedString("descRecipientVersionNotSupported", "ExEB0F33", false, true, Strings.ResourceManager, new object[]
			{
				email,
				version,
				minimumVersion
			});
		}

		public static LocalizedString SearchTaskTimeoutArchive(string displayName, string mailboxGuid)
		{
			return new LocalizedString("SearchTaskTimeoutArchive", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString descInvalidAuthorizationContext
		{
			get
			{
				return new LocalizedString("descInvalidAuthorizationContext", "Ex9B5A48", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidGoodThreshold(int startValue, int endValue)
		{
			return new LocalizedString("descInvalidGoodThreshold", "Ex2D689F", false, true, Strings.ResourceManager, new object[]
			{
				startValue,
				endValue
			});
		}

		public static LocalizedString LogFieldsMessageTypes
		{
			get
			{
				return new LocalizedString("LogFieldsMessageTypes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PositiveParameter
		{
			get
			{
				return new LocalizedString("PositiveParameter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNotAValidExchangePrincipal(string emailAddress)
		{
			return new LocalizedString("descNotAValidExchangePrincipal", "Ex13EC65", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString LogMailSeeAttachment
		{
			get
			{
				return new LocalizedString("LogMailSeeAttachment", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMeetingSuggestionsInvalidTimeInterval
		{
			get
			{
				return new LocalizedString("descMeetingSuggestionsInvalidTimeInterval", "Ex97292C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString KeywordStatsNotRequested
		{
			get
			{
				return new LocalizedString("KeywordStatsNotRequested", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMisconfiguredIntraOrganizationConnector(string intraOrganizationConnector)
		{
			return new LocalizedString("descMisconfiguredIntraOrganizationConnector", "", false, false, Strings.ResourceManager, new object[]
			{
				intraOrganizationConnector
			});
		}

		public static LocalizedString descSoapAutoDiscoverInvalidResponseError(string url)
		{
			return new LocalizedString("descSoapAutoDiscoverInvalidResponseError", "Ex40B39B", false, true, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString RbacTargetMailboxAuthorizationError
		{
			get
			{
				return new LocalizedString("RbacTargetMailboxAuthorizationError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SortedResultNullParameters
		{
			get
			{
				return new LocalizedString("SortedResultNullParameters", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString EmptyRefinerServerResponse(string url)
		{
			return new LocalizedString("EmptyRefinerServerResponse", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString descProxyNoResultError(string recipient, string source)
		{
			return new LocalizedString("descProxyNoResultError", "Ex779AFF", false, true, Strings.ResourceManager, new object[]
			{
				recipient,
				source
			});
		}

		public static LocalizedString LogFieldsLastEndTime
		{
			get
			{
				return new LocalizedString("LogFieldsLastEndTime", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownRecurrencePattern(string patternName)
		{
			return new LocalizedString("UnknownRecurrencePattern", "Ex8DB95F", false, true, Strings.ResourceManager, new object[]
			{
				patternName
			});
		}

		public static LocalizedString TrackingErrorInvalidADData
		{
			get
			{
				return new LocalizedString("TrackingErrorInvalidADData", "Ex4DDA29", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descEmptySecurityDescriptor(string mailboxAddress)
		{
			return new LocalizedString("descEmptySecurityDescriptor", "ExC95235", false, true, Strings.ResourceManager, new object[]
			{
				mailboxAddress
			});
		}

		public static LocalizedString CreateFolderFailed(string name)
		{
			return new LocalizedString("CreateFolderFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LogFieldsResume
		{
			get
			{
				return new LocalizedString("LogFieldsResume", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descWorkHoursStartEndInvalid
		{
			get
			{
				return new LocalizedString("descWorkHoursStartEndInvalid", "ExB5214F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchAlreadStarted
		{
			get
			{
				return new LocalizedString("SearchAlreadStarted", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchWorkerError(string mailbox, string message)
		{
			return new LocalizedString("SearchWorkerError", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox,
				message
			});
		}

		public static LocalizedString descAutoDiscoverFailedWithException(string email, string exception)
		{
			return new LocalizedString("descAutoDiscoverFailedWithException", "ExA21E87", false, true, Strings.ResourceManager, new object[]
			{
				email,
				exception
			});
		}

		public static LocalizedString MessageInvalidTimeZoneReferenceToPeriod
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneReferenceToPeriod", "ExCEC857", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProgressSearchingInProgress
		{
			get
			{
				return new LocalizedString("ProgressSearchingInProgress", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descCorruptUserOofSettingsXmlDocument
		{
			get
			{
				return new LocalizedString("descCorruptUserOofSettingsXmlDocument", "Ex63FB06", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CrossForestNotSupported
		{
			get
			{
				return new LocalizedString("CrossForestNotSupported", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchTransientError(string searchType, string message)
		{
			return new LocalizedString("SearchTransientError", "", false, false, Strings.ResourceManager, new object[]
			{
				searchType,
				message
			});
		}

		public static LocalizedString descTimeIntervalTooBig(string propertyName, string allowedTimeSpanInDays, string actualTimeSpanInDays)
		{
			return new LocalizedString("descTimeIntervalTooBig", "ExC408D3", false, true, Strings.ResourceManager, new object[]
			{
				propertyName,
				allowedTimeSpanInDays,
				actualTimeSpanInDays
			});
		}

		public static LocalizedString descMissingIntraforestCAS(string mailbox)
		{
			return new LocalizedString("descMissingIntraforestCAS", "Ex7D1A6C", false, true, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString LogFieldsNumberSuccessfulMailboxes
		{
			get
			{
				return new LocalizedString("LogFieldsNumberSuccessfulMailboxes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descIdentityArrayEmpty
		{
			get
			{
				return new LocalizedString("descIdentityArrayEmpty", "ExA95C53", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidFailedMailboxesResultWebServiceResponse(string url)
		{
			return new LocalizedString("InvalidFailedMailboxesResultWebServiceResponse", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString SearchAdminRpcInvalidQuery(string searchType, string query)
		{
			return new LocalizedString("SearchAdminRpcInvalidQuery", "", false, false, Strings.ResourceManager, new object[]
			{
				searchType,
				query
			});
		}

		public static LocalizedString MessageInvalidTimeZoneReferenceToGroup
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneReferenceToGroup", "Ex0C86D3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidTimeZoneBias
		{
			get
			{
				return new LocalizedString("descInvalidTimeZoneBias", "ExB64900", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descPublicFolderServerNotFound
		{
			get
			{
				return new LocalizedString("descPublicFolderServerNotFound", "Ex91C2B3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MaxAllowedKeywordsExceeded(int keywordsCount, int maxAllowedKeywordCount)
		{
			return new LocalizedString("MaxAllowedKeywordsExceeded", "", false, false, Strings.ResourceManager, new object[]
			{
				keywordsCount,
				maxAllowedKeywordCount
			});
		}

		public static LocalizedString TrackingErrorCrossPremiseAuthentication
		{
			get
			{
				return new LocalizedString("TrackingErrorCrossPremiseAuthentication", "Ex9A5B7F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZonePeriodNullId
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZonePeriodNullId", "Ex579284", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SortIComparableTypeException(string type)
		{
			return new LocalizedString("SortIComparableTypeException", "", false, false, Strings.ResourceManager, new object[]
			{
				type
			});
		}

		public static LocalizedString SourceMailboxUserNotFoundInAD(string name)
		{
			return new LocalizedString("SourceMailboxUserNotFoundInAD", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString StorePermanantError
		{
			get
			{
				return new LocalizedString("StorePermanantError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorTransientUnexpected
		{
			get
			{
				return new LocalizedString("TrackingErrorTransientUnexpected", "ExB9110B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorModerationDecisionLogsFromE14Rtm
		{
			get
			{
				return new LocalizedString("TrackingErrorModerationDecisionLogsFromE14Rtm", "ExA7889C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descProxyRemoteServerError(string source)
		{
			return new LocalizedString("descProxyRemoteServerError", "Ex002282", false, true, Strings.ResourceManager, new object[]
			{
				source
			});
		}

		public static LocalizedString descInvalidTimeInterval(string propertyName)
		{
			return new LocalizedString("descInvalidTimeInterval", "Ex15E887", false, true, Strings.ResourceManager, new object[]
			{
				propertyName
			});
		}

		public static LocalizedString TrackingErrorCASUriDiscovery
		{
			get
			{
				return new LocalizedString("TrackingErrorCASUriDiscovery", "ExE86234", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToFindPublicFolderServer
		{
			get
			{
				return new LocalizedString("descFailedToFindPublicFolderServer", "Ex9260B3", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageTrackingApplicationName
		{
			get
			{
				return new LocalizedString("MessageTrackingApplicationName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descAutoDiscoverThruDirectoryFailed
		{
			get
			{
				return new LocalizedString("descAutoDiscoverThruDirectoryFailed", "Ex9D46ED", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TargetFolderNotFound(string folderName)
		{
			return new LocalizedString("TargetFolderNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString MessageInvalidTimeZoneDuplicatePeriods
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneDuplicatePeriods", "Ex99CA23", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descCannotFindOrganizationalFolderInMailbox(string folderName, string mailboxName)
		{
			return new LocalizedString("descCannotFindOrganizationalFolderInMailbox", "ExFFA08F", false, true, Strings.ResourceManager, new object[]
			{
				folderName,
				mailboxName
			});
		}

		public static LocalizedString UnknownBodyFormat(string format)
		{
			return new LocalizedString("UnknownBodyFormat", "Ex0B9762", false, true, Strings.ResourceManager, new object[]
			{
				format
			});
		}

		public static LocalizedString SearchFolderTimeout(string mailbox)
		{
			return new LocalizedString("SearchFolderTimeout", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString LogMailSimpleHeader(string status)
		{
			return new LocalizedString("LogMailSimpleHeader", "", false, false, Strings.ResourceManager, new object[]
			{
				status
			});
		}

		public static LocalizedString InvalidIdInPreviewResult(string url)
		{
			return new LocalizedString("InvalidIdInPreviewResult", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString LogFieldsPercentComplete
		{
			get
			{
				return new LocalizedString("LogFieldsPercentComplete", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFreeBusyAndSuggestionsNull
		{
			get
			{
				return new LocalizedString("descFreeBusyAndSuggestionsNull", "ExC506D2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsSuccessfulMailboxes
		{
			get
			{
				return new LocalizedString("LogFieldsSuccessfulMailboxes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CorruptedFolder(string mailbox)
		{
			return new LocalizedString("CorruptedFolder", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString UninstallAssistantsServiceTask
		{
			get
			{
				return new LocalizedString("UninstallAssistantsServiceTask", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProgressSearching
		{
			get
			{
				return new LocalizedString("ProgressSearching", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchInvalidPagination
		{
			get
			{
				return new LocalizedString("SearchInvalidPagination", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsSearchQuery
		{
			get
			{
				return new LocalizedString("LogFieldsSearchQuery", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingTotalBudgetExceeded
		{
			get
			{
				return new LocalizedString("TrackingTotalBudgetExceeded", "Ex9C58F4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descSuggestionMustStartOnThirtyMinuteBoundary
		{
			get
			{
				return new LocalizedString("descSuggestionMustStartOnThirtyMinuteBoundary", "Ex63022C", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchServerShutdown
		{
			get
			{
				return new LocalizedString("SearchServerShutdown", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descUnsupportedSecurityDescriptorHeader(string mailboxAddress, string expectedHeaderLength, string actualHeaderLength)
		{
			return new LocalizedString("descUnsupportedSecurityDescriptorHeader", "Ex175878", false, true, Strings.ResourceManager, new object[]
			{
				mailboxAddress,
				expectedHeaderLength,
				actualHeaderLength
			});
		}

		public static LocalizedString OWAServiceUrlFailure(string name, string message)
		{
			return new LocalizedString("OWAServiceUrlFailure", "", false, false, Strings.ResourceManager, new object[]
			{
				name,
				message
			});
		}

		public static LocalizedString descFailedToCreateLegacyOofRule
		{
			get
			{
				return new LocalizedString("descFailedToCreateLegacyOofRule", "Ex24070E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhotosApplicationName
		{
			get
			{
				return new LocalizedString("PhotosApplicationName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchAdminRpcCallMaxSearches(string mailboxGuid)
		{
			return new LocalizedString("SearchAdminRpcCallMaxSearches", "", false, false, Strings.ResourceManager, new object[]
			{
				mailboxGuid
			});
		}

		public static LocalizedString descInvalidDayOrder(int min, int max)
		{
			return new LocalizedString("descInvalidDayOrder", "ExBAECFF", false, true, Strings.ResourceManager, new object[]
			{
				min,
				max
			});
		}

		public static LocalizedString SearchAdminRpcCallAccessDenied(string displayName, string mailboxGuid)
		{
			return new LocalizedString("SearchAdminRpcCallAccessDenied", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString ADUserNotFoundException
		{
			get
			{
				return new LocalizedString("ADUserNotFoundException", "Ex4F874A", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidPreviewResultWebServiceResponse(string url)
		{
			return new LocalizedString("InvalidPreviewResultWebServiceResponse", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString ProgressSearchingSources
		{
			get
			{
				return new LocalizedString("ProgressSearchingSources", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogMailNone
		{
			get
			{
				return new LocalizedString("LogMailNone", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsStatusMailRecipients
		{
			get
			{
				return new LocalizedString("LogFieldsStatusMailRecipients", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descOnlyDefaultFreeBusyIntervalSupported(int fbInterval)
		{
			return new LocalizedString("descOnlyDefaultFreeBusyIntervalSupported", "Ex941130", false, true, Strings.ResourceManager, new object[]
			{
				fbInterval
			});
		}

		public static LocalizedString LogFieldsResultNumber
		{
			get
			{
				return new LocalizedString("LogFieldsResultNumber", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorPermanentUnexpected
		{
			get
			{
				return new LocalizedString("TrackingErrorPermanentUnexpected", "ExA3DA08", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descResultSetTooBig(string allowedSize, string actualSize)
		{
			return new LocalizedString("descResultSetTooBig", "Ex389125", false, true, Strings.ResourceManager, new object[]
			{
				allowedSize,
				actualSize
			});
		}

		public static LocalizedString ProgressCompleting
		{
			get
			{
				return new LocalizedString("ProgressCompleting", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidUserOofSettings
		{
			get
			{
				return new LocalizedString("descInvalidUserOofSettings", "ExCD9903", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsResultNumberEstimate
		{
			get
			{
				return new LocalizedString("LogFieldsResultNumberEstimate", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descTimeoutExpired(string state)
		{
			return new LocalizedString("descTimeoutExpired", "ExA5689D", false, true, Strings.ResourceManager, new object[]
			{
				state
			});
		}

		public static LocalizedString LogFieldsKeywordHits
		{
			get
			{
				return new LocalizedString("LogFieldsKeywordHits", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnknownError
		{
			get
			{
				return new LocalizedString("UnknownError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZonePeriodNullBias
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZonePeriodNullBias", "Ex776834", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MailboxSeachCountIncludeUnsearchable
		{
			get
			{
				return new LocalizedString("MailboxSeachCountIncludeUnsearchable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogMailBlank
		{
			get
			{
				return new LocalizedString("LogMailBlank", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descWorkHoursEndTimeInvalid
		{
			get
			{
				return new LocalizedString("descWorkHoursEndTimeInvalid", "ExA6460B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RefinerValueNullOrCountZero(string url)
		{
			return new LocalizedString("RefinerValueNullOrCountZero", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString descNotAContactOrUser(string emailAddress)
		{
			return new LocalizedString("descNotAContactOrUser", "Ex5506B1", false, true, Strings.ResourceManager, new object[]
			{
				emailAddress
			});
		}

		public static LocalizedString descFreeBusyDLLimitReached(int allowedSize)
		{
			return new LocalizedString("descFreeBusyDLLimitReached", "Ex503E92", false, true, Strings.ResourceManager, new object[]
			{
				allowedSize
			});
		}

		public static LocalizedString ProgessCreatingThreads
		{
			get
			{
				return new LocalizedString("ProgessCreatingThreads", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descOOFCannotReadExternalOofOptions
		{
			get
			{
				return new LocalizedString("descOOFCannotReadExternalOofOptions", "Ex2B01CC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PrimaryMailbox
		{
			get
			{
				return new LocalizedString("PrimaryMailbox", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToFindElcRoot(string mailboxName)
		{
			return new LocalizedString("descFailedToFindElcRoot", "Ex354896", false, true, Strings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString MessageInvalidTimeZoneCustomTimeZoneThreeElements
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneCustomTimeZoneThreeElements", "Ex04F1B2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingTransientError
		{
			get
			{
				return new LocalizedString("TrackingTransientError", "ExCC0EAB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsLastStartTime
		{
			get
			{
				return new LocalizedString("LogFieldsLastStartTime", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToFetchPreviewItems(string mailbox)
		{
			return new LocalizedString("FailedToFetchPreviewItems", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString CopyItemsFailed
		{
			get
			{
				return new LocalizedString("CopyItemsFailed", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString DummyApplicationName
		{
			get
			{
				return new LocalizedString("DummyApplicationName", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZoneDuplicateGroups
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneDuplicateGroups", "ExAA88C0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidMergedFreeBusyInterval(string minimumValue, string maximumValue)
		{
			return new LocalizedString("descInvalidMergedFreeBusyInterval", "Ex3161D9", false, true, Strings.ResourceManager, new object[]
			{
				minimumValue,
				maximumValue
			});
		}

		public static LocalizedString UnknownRecurrenceOrderType(string name)
		{
			return new LocalizedString("UnknownRecurrenceOrderType", "ExFEC19C", false, true, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString ExecutingUserNeedSmtpAddress
		{
			get
			{
				return new LocalizedString("ExecutingUserNeedSmtpAddress", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZoneMoreThanTwoPeriodsUnsupported
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneMoreThanTwoPeriodsUnsupported", "Ex5D4BB5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMailRecipientNotFound
		{
			get
			{
				return new LocalizedString("descMailRecipientNotFound", "Ex8FF96B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNullUserName
		{
			get
			{
				return new LocalizedString("descNullUserName", "Ex2FA90E", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NonNegativeParameter
		{
			get
			{
				return new LocalizedString("NonNegativeParameter", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsEstimateNotExcludeDuplicates
		{
			get
			{
				return new LocalizedString("LogFieldsEstimateNotExcludeDuplicates", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToGetOrganizationalFoldersForMailbox(string mailboxName)
		{
			return new LocalizedString("descFailedToGetOrganizationalFoldersForMailbox", "ExAE2798", false, true, Strings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString descElcNoMatchingOrgFolder(string folderName)
		{
			return new LocalizedString("descElcNoMatchingOrgFolder", "ExA8325E", false, true, Strings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString StoreTransientError
		{
			get
			{
				return new LocalizedString("StoreTransientError", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidTransitionBias(int min, int max)
		{
			return new LocalizedString("descInvalidTransitionBias", "ExDD6AC9", false, true, Strings.ResourceManager, new object[]
			{
				min,
				max
			});
		}

		public static LocalizedString LogFieldsLogLevel
		{
			get
			{
				return new LocalizedString("LogFieldsLogLevel", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchUserNotFound(string displayName)
		{
			return new LocalizedString("SearchUserNotFound", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName
			});
		}

		public static LocalizedString TrackingWSRequestCorrupt
		{
			get
			{
				return new LocalizedString("TrackingWSRequestCorrupt", "Ex3B789B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchDisabled
		{
			get
			{
				return new LocalizedString("SearchDisabled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhotoRetrievalFailedIOError(string innerExceptionMessage)
		{
			return new LocalizedString("PhotoRetrievalFailedIOError", "", false, false, Strings.ResourceManager, new object[]
			{
				innerExceptionMessage
			});
		}

		public static LocalizedString DatabaseLocationUnavailable(string mailbox)
		{
			return new LocalizedString("DatabaseLocationUnavailable", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString SearchTooManyKeywords(int keywords)
		{
			return new LocalizedString("SearchTooManyKeywords", "", false, false, Strings.ResourceManager, new object[]
			{
				keywords
			});
		}

		public static LocalizedString descNullCredentialsToServiceDiscoveryRequest
		{
			get
			{
				return new LocalizedString("descNullCredentialsToServiceDiscoveryRequest", "ExE5E275", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsUnsuccessfulMailboxes
		{
			get
			{
				return new LocalizedString("LogFieldsUnsuccessfulMailboxes", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZoneTimeZoneNotFound
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneTimeZoneNotFound", "Ex50B464", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZoneIntValueIsInvalid
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneIntValueIsInvalid", "Ex6F7FEF", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToCreateElcRootRetry(string mailboxName)
		{
			return new LocalizedString("descFailedToCreateElcRootRetry", "Ex4D5CA4", false, true, Strings.ResourceManager, new object[]
			{
				mailboxName
			});
		}

		public static LocalizedString SearchAdminRpcSearchCallTimedout(string prefix, int mailboxesCount, string database, string query)
		{
			return new LocalizedString("SearchAdminRpcSearchCallTimedout", "", false, false, Strings.ResourceManager, new object[]
			{
				prefix,
				mailboxesCount,
				database,
				query
			});
		}

		public static LocalizedString ScheduleConfigurationSchedule
		{
			get
			{
				return new LocalizedString("ScheduleConfigurationSchedule", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descOofRuleSaveException
		{
			get
			{
				return new LocalizedString("descOofRuleSaveException", "Ex8FBABC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingRpcError(int errorCode)
		{
			return new LocalizedString("TrackingRpcError", "Ex0705CC", false, true, Strings.ResourceManager, new object[]
			{
				errorCode
			});
		}

		public static LocalizedString LogFieldsTargetMailbox
		{
			get
			{
				return new LocalizedString("LogFieldsTargetMailbox", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchNonFullTextPaginationProperty(string paginationClause)
		{
			return new LocalizedString("SearchNonFullTextPaginationProperty", "", false, false, Strings.ResourceManager, new object[]
			{
				paginationClause
			});
		}

		public static LocalizedString descPFNotSupported(string mailbox)
		{
			return new LocalizedString("descPFNotSupported", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString descInvalidSuggestionsTimeRange
		{
			get
			{
				return new LocalizedString("descInvalidSuggestionsTimeRange", "Ex2C2FBC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToGetUserOofPolicy
		{
			get
			{
				return new LocalizedString("descFailedToGetUserOofPolicy", "ExC7E9E0", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NestedFanout(string mailbox)
		{
			return new LocalizedString("NestedFanout", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString descMeetingSuggestionsDurationTooLarge
		{
			get
			{
				return new LocalizedString("descMeetingSuggestionsDurationTooLarge", "Ex8A792F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProgressOpening
		{
			get
			{
				return new LocalizedString("ProgressOpening", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMailTipsSenderNotFound(string address)
		{
			return new LocalizedString("descMailTipsSenderNotFound", "Ex4C4966", false, true, Strings.ResourceManager, new object[]
			{
				address
			});
		}

		public static LocalizedString TrackingErrorBudgetExceededMultiMessageSearch
		{
			get
			{
				return new LocalizedString("TrackingErrorBudgetExceededMultiMessageSearch", "ExFB7FE7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descLocalServerObjectNotFound
		{
			get
			{
				return new LocalizedString("descLocalServerObjectNotFound", "ExB4A1CC", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CISearchFailed(string mailbox)
		{
			return new LocalizedString("CISearchFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString RecoverableItems
		{
			get
			{
				return new LocalizedString("RecoverableItems", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descProxyRequestFailed
		{
			get
			{
				return new LocalizedString("descProxyRequestFailed", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString CouldNotFindOrgRelationship(string domain)
		{
			return new LocalizedString("CouldNotFindOrgRelationship", "", false, false, Strings.ResourceManager, new object[]
			{
				domain
			});
		}

		public static LocalizedString ProgessCreating
		{
			get
			{
				return new LocalizedString("ProgessCreating", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidYear(int min, int max)
		{
			return new LocalizedString("descInvalidYear", "", false, false, Strings.ResourceManager, new object[]
			{
				min,
				max
			});
		}

		public static LocalizedString descClientDisconnected
		{
			get
			{
				return new LocalizedString("descClientDisconnected", "ExAFAFB1", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchThrottled
		{
			get
			{
				return new LocalizedString("SearchThrottled", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidTransitionTime
		{
			get
			{
				return new LocalizedString("descInvalidTransitionTime", "ExE9C5D7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidMailboxInMailboxStatistics(string url)
		{
			return new LocalizedString("InvalidMailboxInMailboxStatistics", "", false, false, Strings.ResourceManager, new object[]
			{
				url
			});
		}

		public static LocalizedString TargetMailboxOutOfSpace
		{
			get
			{
				return new LocalizedString("TargetMailboxOutOfSpace", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsLastRunBy
		{
			get
			{
				return new LocalizedString("LogFieldsLastRunBy", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString UnableToReadServiceTopology
		{
			get
			{
				return new LocalizedString("UnableToReadServiceTopology", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descElcRootFolderName
		{
			get
			{
				return new LocalizedString("descElcRootFolderName", "ExF0E936", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNoMailTipsInEwsResponseMessage
		{
			get
			{
				return new LocalizedString("descNoMailTipsInEwsResponseMessage", "Ex746289", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descFailedToGetRules
		{
			get
			{
				return new LocalizedString("descFailedToGetRules", "ExD47947", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LogFieldsStoppedBy
		{
			get
			{
				return new LocalizedString("LogFieldsStoppedBy", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descAutoDiscoverBadRedirectLocation(string emailAddress, string redirectAddress)
		{
			return new LocalizedString("descAutoDiscoverBadRedirectLocation", "", false, false, Strings.ResourceManager, new object[]
			{
				emailAddress,
				redirectAddress
			});
		}

		public static LocalizedString SearchTaskTimeoutPrimary(string displayName, string mailboxGuid)
		{
			return new LocalizedString("SearchTaskTimeoutPrimary", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString descProxyRequestProcessingSocketError
		{
			get
			{
				return new LocalizedString("descProxyRequestProcessingSocketError", "Ex9AF0F5", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descProxyRequestNotAllowed
		{
			get
			{
				return new LocalizedString("descProxyRequestNotAllowed", "ExF290A7", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceMailboxVersionError(string name)
		{
			return new LocalizedString("SourceMailboxVersionError", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString InsufficientSpaceOnTargetMailbox(string estimated, string available)
		{
			return new LocalizedString("InsufficientSpaceOnTargetMailbox", "", false, false, Strings.ResourceManager, new object[]
			{
				estimated,
				available
			});
		}

		public static LocalizedString descInvalidAccessLevel
		{
			get
			{
				return new LocalizedString("descInvalidAccessLevel", "Ex104820", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingPermanentErrorMultiMessageSearch
		{
			get
			{
				return new LocalizedString("TrackingPermanentErrorMultiMessageSearch", "Ex1036C2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString PhotoRetrievalFailedWin32Error(string innerExceptionMessage)
		{
			return new LocalizedString("PhotoRetrievalFailedWin32Error", "", false, false, Strings.ResourceManager, new object[]
			{
				innerExceptionMessage
			});
		}

		public static LocalizedString LogFieldsCreatedBy
		{
			get
			{
				return new LocalizedString("LogFieldsCreatedBy", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString LowSystemResource
		{
			get
			{
				return new LocalizedString("LowSystemResource", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorLogSearchServiceDown
		{
			get
			{
				return new LocalizedString("TrackingErrorLogSearchServiceDown", "Ex32E8EE", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descSoapAutoDiscoverResponseError(string url, string errorMessage)
		{
			return new LocalizedString("descSoapAutoDiscoverResponseError", "Ex6C7F2F", false, true, Strings.ResourceManager, new object[]
			{
				url,
				errorMessage
			});
		}

		public static LocalizedString LogMailNotApplicable
		{
			get
			{
				return new LocalizedString("LogMailNotApplicable", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNoFreeBusyAccess
		{
			get
			{
				return new LocalizedString("descNoFreeBusyAccess", "ExD48DD4", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ADUserMisconfiguredException
		{
			get
			{
				return new LocalizedString("ADUserMisconfiguredException", "Ex1896E6", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ProgressOpeningTarget
		{
			get
			{
				return new LocalizedString("ProgressOpeningTarget", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descOOFVirusDetectedOofReplyMessage
		{
			get
			{
				return new LocalizedString("descOOFVirusDetectedOofReplyMessage", "Ex21097B", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZonePeriodNullName
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZonePeriodNullName", "ExAB806F", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString MessageInvalidTimeZoneFirstTransition
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneFirstTransition", "Ex28AF95", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descErrorEwsResponse(int errorCode)
		{
			return new LocalizedString("descErrorEwsResponse", "Ex16A609", false, true, Strings.ResourceManager, new object[]
			{
				errorCode
			});
		}

		public static LocalizedString descInvalidConfigForCrossForestRequest(string mailbox)
		{
			return new LocalizedString("descInvalidConfigForCrossForestRequest", "Ex8264F7", false, true, Strings.ResourceManager, new object[]
			{
				mailbox
			});
		}

		public static LocalizedString InvalidContactException
		{
			get
			{
				return new LocalizedString("InvalidContactException", "ExC950E2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString ErrorRemoveOngoingSearch
		{
			get
			{
				return new LocalizedString("ErrorRemoveOngoingSearch", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchAdminRpcCallFailed(string mailboxGuid, int errorCode)
		{
			return new LocalizedString("SearchAdminRpcCallFailed", "", false, false, Strings.ResourceManager, new object[]
			{
				mailboxGuid,
				errorCode
			});
		}

		public static LocalizedString SearchTaskCancelled(string mailboxGuids, string databaseGuid)
		{
			return new LocalizedString("SearchTaskCancelled", "", false, false, Strings.ResourceManager, new object[]
			{
				mailboxGuids,
				databaseGuid
			});
		}

		public static LocalizedString descMissingArgument(string argument)
		{
			return new LocalizedString("descMissingArgument", "ExB6FDB1", false, true, Strings.ResourceManager, new object[]
			{
				argument
			});
		}

		public static LocalizedString TrackingErrorReadStatus
		{
			get
			{
				return new LocalizedString("TrackingErrorReadStatus", "Ex065EFB", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SearchTaskCancelledArchive(string displayName, string mailboxGuid)
		{
			return new LocalizedString("SearchTaskCancelledArchive", "", false, false, Strings.ResourceManager, new object[]
			{
				displayName,
				mailboxGuid
			});
		}

		public static LocalizedString TrackingErrorCrossForestAuthentication
		{
			get
			{
				return new LocalizedString("TrackingErrorCrossForestAuthentication", "Ex1FADC8", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descInvalidFreeBusyViewType
		{
			get
			{
				return new LocalizedString("descInvalidFreeBusyViewType", "ExE51E78", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString FailedToGetItem(string messageClass, string folder)
		{
			return new LocalizedString("FailedToGetItem", "", false, false, Strings.ResourceManager, new object[]
			{
				messageClass,
				folder
			});
		}

		public static LocalizedString descProxyRequestProcessingIOError(string exceptionMessage)
		{
			return new LocalizedString("descProxyRequestProcessingIOError", "ExEF88FC", false, true, Strings.ResourceManager, new object[]
			{
				exceptionMessage
			});
		}

		public static LocalizedString descInputFolderNamesContainDuplicates(string folderName)
		{
			return new LocalizedString("descInputFolderNamesContainDuplicates", "ExB58B40", false, true, Strings.ResourceManager, new object[]
			{
				folderName
			});
		}

		public static LocalizedString descLogonAsNetworkServiceFailed
		{
			get
			{
				return new LocalizedString("descLogonAsNetworkServiceFailed", "ExE7C7B2", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorCrossForestMisconfiguration
		{
			get
			{
				return new LocalizedString("TrackingErrorCrossForestMisconfiguration", "ExDDEADD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString RangedParameter(int max)
		{
			return new LocalizedString("RangedParameter", "", false, false, Strings.ResourceManager, new object[]
			{
				max
			});
		}

		public static LocalizedString descPrimaryDefaultCorrupted(string policyName, int tagCount)
		{
			return new LocalizedString("descPrimaryDefaultCorrupted", "Ex80411F", false, true, Strings.ResourceManager, new object[]
			{
				policyName,
				tagCount
			});
		}

		public static LocalizedString MessageInvalidTimeZoneMissedGroup
		{
			get
			{
				return new LocalizedString("MessageInvalidTimeZoneMissedGroup", "Ex0E6923", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString NoKeywordStatsForCopySearch
		{
			get
			{
				return new LocalizedString("NoKeywordStatsForCopySearch", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descMailboxFailover
		{
			get
			{
				return new LocalizedString("descMailboxFailover", "ExAA06FD", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString InvalidUnknownMailboxInPreviewResult(string url, string legacyDn, string mailboxGuid)
		{
			return new LocalizedString("InvalidUnknownMailboxInPreviewResult", "", false, false, Strings.ResourceManager, new object[]
			{
				url,
				legacyDn,
				mailboxGuid
			});
		}

		public static LocalizedString LogFieldsExcludeDuplicateMessages
		{
			get
			{
				return new LocalizedString("LogFieldsExcludeDuplicateMessages", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString SourceMailboxCrossPremiseError(string name)
		{
			return new LocalizedString("SourceMailboxCrossPremiseError", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString LogFieldsErrors
		{
			get
			{
				return new LocalizedString("LogFieldsErrors", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString mailTipsTenant
		{
			get
			{
				return new LocalizedString("mailTipsTenant", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descE14orHigherProxyServerNotFound(string email, int serverVersion)
		{
			return new LocalizedString("descE14orHigherProxyServerNotFound", "ExD7FBFA", false, true, Strings.ResourceManager, new object[]
			{
				email,
				serverVersion
			});
		}

		public static LocalizedString RemoteMailbox(string name)
		{
			return new LocalizedString("RemoteMailbox", "", false, false, Strings.ResourceManager, new object[]
			{
				name
			});
		}

		public static LocalizedString SearchArgument
		{
			get
			{
				return new LocalizedString("SearchArgument", "", false, false, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString TrackingErrorCrossPremiseMisconfigurationMultiMessageSearch
		{
			get
			{
				return new LocalizedString("TrackingErrorCrossPremiseMisconfigurationMultiMessageSearch", "Ex4CD829", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString descNullEmailToAutoDiscoverRequest
		{
			get
			{
				return new LocalizedString("descNullEmailToAutoDiscoverRequest", "ExB7D446", false, true, Strings.ResourceManager, new object[0]);
			}
		}

		public static LocalizedString GetLocalizedString(Strings.IDs key)
		{
			return new LocalizedString(Strings.stringIDs[(uint)key], Strings.ResourceManager, new object[0]);
		}

		private static Dictionary<uint, string> stringIDs = new Dictionary<uint, string>(235);

		private static ExchangeResourceManager ResourceManager = ExchangeResourceManager.GetResourceManager("Microsoft.Exchange.InfoWorker.Common.Strings", typeof(Strings).GetTypeInfo().Assembly);

		public enum IDs : uint
		{
			TrackingErrorCrossPremiseMisconfiguration = 4118843607U,
			LogFieldsResultSizeCopied = 887069165U,
			MailtipsApplicationName = 2434190600U,
			descNotDefaultCalendar = 3039411601U,
			TrackingPermanentError = 3645107181U,
			WrongTargetServer = 1560475167U,
			InvalidSearchQuery = 4159961541U,
			ProgressCompletingSearch = 2177919315U,
			descWorkHoursStartTimeInvalid = 3855823295U,
			descMailboxLogonFailed = 996171111U,
			InstallAssistantsServiceTask = 3025732470U,
			MessageInvalidTimeZoneTransitionGroupNullId = 4098403379U,
			FailedCommunicationException = 3696858122U,
			LogFieldsNumberUnsuccessfulMailboxes = 3416023611U,
			SearchNotStarted = 4244107904U,
			TrackingErrorConnectivity = 2960157992U,
			TargetFolder = 946397903U,
			Unsearchable = 1774313355U,
			LogFieldsRecipients = 3752482865U,
			InvalidPreviewItemInResultRows = 357978241U,
			descInvalidSmptAddres = 2642055223U,
			TrackingErrorBudgetExceeded = 2249986005U,
			LogFieldsKeywordMbxs = 1952477638U,
			descQueryGenerationNotRequired = 859073841U,
			TrackingNoDefaultDomain = 3528487049U,
			InvalidChangeKeyReturned = 457909195U,
			ArchiveMailbox = 3832545148U,
			LogFieldsKeywordHitCount = 4235476896U,
			LogFieldsName = 824833090U,
			LogFieldsStatus = 3196432253U,
			LogFieldsEndDate = 1572073070U,
			LogFieldsSenders = 1970283657U,
			TrackingErrorTimeBudgetExceeded = 1734826510U,
			TrackingLogVersionIncompatible = 127105864U,
			AutodiscoverFailedException = 4195952492U,
			descInvalidMaxNonWorkHourResultsPerDay = 215769503U,
			descNullAutoDiscoverResponse = 1477200533U,
			UnexpectedRemoteDataException = 1446717588U,
			LogFieldsIncludeKeywordStatistics = 4270171893U,
			UnexpectedError = 2870421805U,
			descInvalidMaximumResults = 2171828426U,
			descOOFCannotReadOofConfigData = 1023107612U,
			descInvalidSecurityDescriptor = 366714169U,
			InvalidResultMerge = 540675128U,
			SearchObjectNotFound = 252553502U,
			TrackingInstanceBudgetExceeded = 1359157944U,
			MessageInvalidTimeZoneOutOfRange = 3961981453U,
			LogFieldsStartDate = 3014888409U,
			NotOperator = 273956641U,
			LogFieldsSourceRecipients = 959712720U,
			TrackingErrorQueueViewerRpc = 1261494857U,
			SearchLogHeader = 2084277545U,
			MessageInvalidTimeZoneInvalidOffsetFormat = 609234870U,
			descInvalidCredentials = 669181240U,
			LogFieldsSearchOperation = 3577556142U,
			MessageInvalidTimeZoneNonFirstTransition = 3644766027U,
			descMeetingSuggestionsDurationTooSmall = 184705256U,
			DeleteItemsFailed = 1884462766U,
			PendingSynchronizationException = 2550395136U,
			LogFieldsNumberMailboxesToSearch = 4277240717U,
			LogFieldsKeywordKeyword = 2182808069U,
			ObjectNotFound = 2422079678U,
			MessageInvalidTimeZoneMissedPeriod = 3865092385U,
			LogFieldsResultSize = 3051609629U,
			MessageInvalidTimeZoneDayOfWeekValue = 447822483U,
			TrackingBusy = 3099813970U,
			InvalidAppointmentException = 3278774409U,
			descInvalidNetworkServiceContext = 1105384606U,
			ResultsNotDeduped = 1524653606U,
			ErrorTimeZone = 610144303U,
			BatchSynchronizationFailedException = 2456116760U,
			descStartAndEndTimesOutSideFreeBusyData = 876631183U,
			descWin32InteropError = 4252318527U,
			TrackingErrorLegacySender = 3219875537U,
			descNoEwsResponse = 3436346834U,
			LogFieldsResultsLink = 3301704787U,
			TrackingErrorLegacySenderMultiMessageSearch = 2977121749U,
			LogFieldsManagedBy = 2205699811U,
			KeywordHitEmptyQuery = 1676524031U,
			SearchQueryEmpty = 2152565173U,
			ServerShutdown = 2705707569U,
			TrackingErrorSuffixForAdministrator = 377459750U,
			SubscriptionNotFoundException = 4273761671U,
			LogFieldsResultSizeEstimate = 2881797395U,
			descNullDateInChangeDate = 3863825871U,
			SharingFolderNotFoundException = 232000386U,
			LogMailAll = 1607453502U,
			descInvalidScheduledOofDuration = 3106977401U,
			TrackingTransientErrorMultiMessageSearch = 1899968567U,
			AqsParserError = 2680389304U,
			LegacyOofMessage = 829039958U,
			descInvalidClientSecurityContext = 118976146U,
			LogMailFooter = 3912319220U,
			descInvalidFormatInEmail = 2822320878U,
			descNoCalendar = 2977444274U,
			UnexpectedUserResponses = 1018641786U,
			LogFieldsIdentity = 2215464039U,
			FreeBusyApplicationName = 3854866410U,
			SearchAborted = 2236533269U,
			AutodiscoverTimedOut = 1756460035U,
			LogFieldsSearchDumpster = 3871609939U,
			descInvalidAuthorizationContext = 274121238U,
			LogFieldsMessageTypes = 4205846133U,
			PositiveParameter = 3832116504U,
			LogMailSeeAttachment = 1114362743U,
			descMeetingSuggestionsInvalidTimeInterval = 3835343804U,
			KeywordStatsNotRequested = 2769462193U,
			RbacTargetMailboxAuthorizationError = 1382860876U,
			SortedResultNullParameters = 636866827U,
			LogFieldsLastEndTime = 1214298741U,
			TrackingErrorInvalidADData = 55350379U,
			LogFieldsResume = 3068064914U,
			descWorkHoursStartEndInvalid = 3770945073U,
			SearchAlreadStarted = 872236584U,
			MessageInvalidTimeZoneReferenceToPeriod = 908708480U,
			ProgressSearchingInProgress = 489766921U,
			descCorruptUserOofSettingsXmlDocument = 3558390196U,
			CrossForestNotSupported = 1295345518U,
			LogFieldsNumberSuccessfulMailboxes = 3240652606U,
			descIdentityArrayEmpty = 1087048587U,
			MessageInvalidTimeZoneReferenceToGroup = 2265146620U,
			descInvalidTimeZoneBias = 1894419184U,
			descPublicFolderServerNotFound = 1223102710U,
			TrackingErrorCrossPremiseAuthentication = 2319913820U,
			MessageInvalidTimeZonePeriodNullId = 3558532322U,
			StorePermanantError = 147513433U,
			TrackingErrorTransientUnexpected = 3376565836U,
			TrackingErrorModerationDecisionLogsFromE14Rtm = 1389565281U,
			TrackingErrorCASUriDiscovery = 3931032456U,
			descFailedToFindPublicFolderServer = 2307745798U,
			MessageTrackingApplicationName = 1664401949U,
			descAutoDiscoverThruDirectoryFailed = 3586747816U,
			MessageInvalidTimeZoneDuplicatePeriods = 468011246U,
			LogFieldsPercentComplete = 1053420363U,
			descFreeBusyAndSuggestionsNull = 863256271U,
			LogFieldsSuccessfulMailboxes = 3837654127U,
			UninstallAssistantsServiceTask = 534171625U,
			ProgressSearching = 2865084951U,
			SearchInvalidPagination = 193137347U,
			LogFieldsSearchQuery = 2006533237U,
			TrackingTotalBudgetExceeded = 1759034327U,
			descSuggestionMustStartOnThirtyMinuteBoundary = 2697293287U,
			SearchServerShutdown = 3124261155U,
			descFailedToCreateLegacyOofRule = 4107635786U,
			PhotosApplicationName = 4123771088U,
			ADUserNotFoundException = 1880771860U,
			ProgressSearchingSources = 4285214785U,
			LogMailNone = 2804157263U,
			LogFieldsStatusMailRecipients = 1233757588U,
			LogFieldsResultNumber = 3305370967U,
			TrackingErrorPermanentUnexpected = 4287340460U,
			ProgressCompleting = 1954290077U,
			descInvalidUserOofSettings = 270334674U,
			LogFieldsResultNumberEstimate = 1275939085U,
			LogFieldsKeywordHits = 3222940042U,
			UnknownError = 3351215994U,
			MessageInvalidTimeZonePeriodNullBias = 1265936670U,
			MailboxSeachCountIncludeUnsearchable = 2937454784U,
			LogMailBlank = 859087807U,
			descWorkHoursEndTimeInvalid = 3091073294U,
			ProgessCreatingThreads = 2858750991U,
			descOOFCannotReadExternalOofOptions = 2569697819U,
			PrimaryMailbox = 1910512436U,
			MessageInvalidTimeZoneCustomTimeZoneThreeElements = 2852570616U,
			TrackingTransientError = 763976563U,
			LogFieldsLastStartTime = 1809134602U,
			CopyItemsFailed = 1824636832U,
			DummyApplicationName = 1910425077U,
			MessageInvalidTimeZoneDuplicateGroups = 3276944824U,
			ExecutingUserNeedSmtpAddress = 1614878877U,
			MessageInvalidTimeZoneMoreThanTwoPeriodsUnsupported = 3442221872U,
			descMailRecipientNotFound = 3955434964U,
			descNullUserName = 2671831934U,
			NonNegativeParameter = 1407995413U,
			LogFieldsEstimateNotExcludeDuplicates = 1633879312U,
			StoreTransientError = 18603439U,
			LogFieldsLogLevel = 1541817855U,
			TrackingWSRequestCorrupt = 2131297471U,
			SearchDisabled = 2936774726U,
			descNullCredentialsToServiceDiscoveryRequest = 1705124535U,
			LogFieldsUnsuccessfulMailboxes = 697045372U,
			MessageInvalidTimeZoneTimeZoneNotFound = 2530022313U,
			MessageInvalidTimeZoneIntValueIsInvalid = 3753969000U,
			ScheduleConfigurationSchedule = 3488925402U,
			descOofRuleSaveException = 3675304455U,
			LogFieldsTargetMailbox = 3342799224U,
			descInvalidSuggestionsTimeRange = 1441197009U,
			descFailedToGetUserOofPolicy = 670434636U,
			descMeetingSuggestionsDurationTooLarge = 983402700U,
			ProgressOpening = 2411109197U,
			TrackingErrorBudgetExceededMultiMessageSearch = 1471504673U,
			descLocalServerObjectNotFound = 2991551487U,
			RecoverableItems = 2706524390U,
			descProxyRequestFailed = 3367175223U,
			ProgessCreating = 516398220U,
			descClientDisconnected = 924370705U,
			SearchThrottled = 3760653578U,
			descInvalidTransitionTime = 1620607520U,
			TargetMailboxOutOfSpace = 2976781692U,
			LogFieldsLastRunBy = 1848520589U,
			UnableToReadServiceTopology = 4135638738U,
			descElcRootFolderName = 607437832U,
			descNoMailTipsInEwsResponseMessage = 647128549U,
			descFailedToGetRules = 4209067056U,
			LogFieldsStoppedBy = 794348359U,
			descProxyRequestProcessingSocketError = 2268440642U,
			descProxyRequestNotAllowed = 3466285021U,
			descInvalidAccessLevel = 1848500058U,
			TrackingPermanentErrorMultiMessageSearch = 784192305U,
			LogFieldsCreatedBy = 2804255286U,
			LowSystemResource = 1225651387U,
			TrackingErrorLogSearchServiceDown = 854567314U,
			LogMailNotApplicable = 2312386357U,
			descNoFreeBusyAccess = 3289802537U,
			ADUserMisconfiguredException = 348874544U,
			ProgressOpeningTarget = 2033796534U,
			descOOFVirusDetectedOofReplyMessage = 1554201361U,
			MessageInvalidTimeZonePeriodNullName = 2912574056U,
			MessageInvalidTimeZoneFirstTransition = 3332140560U,
			InvalidContactException = 3216622764U,
			ErrorRemoveOngoingSearch = 3436010521U,
			TrackingErrorReadStatus = 1594184545U,
			TrackingErrorCrossForestAuthentication = 3134958540U,
			descInvalidFreeBusyViewType = 1892050364U,
			descLogonAsNetworkServiceFailed = 3341082622U,
			TrackingErrorCrossForestMisconfiguration = 2837247303U,
			MessageInvalidTimeZoneMissedGroup = 2100288003U,
			NoKeywordStatsForCopySearch = 478692077U,
			descMailboxFailover = 3248017497U,
			LogFieldsExcludeDuplicateMessages = 1104837598U,
			LogFieldsErrors = 1743551038U,
			mailTipsTenant = 3607788283U,
			SearchArgument = 3153221581U,
			TrackingErrorCrossPremiseMisconfigurationMultiMessageSearch = 3176897035U,
			descNullEmailToAutoDiscoverRequest = 2497578736U
		}

		private enum ParamIDs
		{
			SearchServerFailed,
			descElcCannotFindDefaultFolder,
			LogMailHeaderInstructions,
			descPublicFolderRequestProcessingError,
			descConfigurationInformationNotFound,
			InvalidReferenceItemInPreviewResult,
			descVirusScanInProgress,
			InconsistentSortedResults,
			InvalidSortedResultParameter,
			InvalidOwaUrlInPreviewResult,
			DeleteItemFailedForMessage,
			descMissingMailboxArrayElement,
			descMisconfiguredOrganizationRelationship,
			descNotAGroupOrUserOrContact,
			descProxyForPersonalNotAllowed,
			UnknownDayOfWeek,
			descInvalidMonth,
			InvalidKeywordStatsRequest,
			PhotoRetrievalFailedUnauthorizedAccessError,
			ArgumentValidationFailedException,
			descDeliveryRestricted,
			UnknownRecurrenceRange,
			LogMailHeader,
			descMailTipsSenderNotUnique,
			InvalidFailedMailboxesResultDuplicateEntries,
			RbacSourceMailboxAuthorizationError,
			descUnknownDefFolder,
			descProxyRequestProcessingError,
			descSoapAutoDiscoverRequestUserSettingError,
			descFailedToCreateELCRoot,
			descElcFolderExists,
			descFailedToCreateOneOrMoreOrganizationalFolders,
			SearchServerError,
			InvalidItemHashInPreviewResult,
			descFailedToSyncFolder,
			MailboxRefinerInRefinersSection,
			descRequestStreamTooBig,
			descProtocolNotFoundInAutoDiscoverResponse,
			UnableToFindSearchObject,
			descMinimumRequiredVersionProxyServerNotFound,
			descIndividualMailboxLimitReached,
			descTagNotInAD,
			descAvailabilityAddressSpaceFailed,
			descUnsupportedSecurityDescriptorVersion,
			SearchMailboxNotFound,
			SearchTaskCancelledPrimary,
			descCannotResolveEmailAddress,
			descMissingProperty,
			descAutoDiscoverFailed,
			UnabledToLocateMailboxServer,
			ArchiveSearchPopulationFailed,
			InvalidSearchRequest,
			DiscoverySearchCIFailure,
			descInvalidTargetAddress,
			descCannotBindToElcRootFolder,
			descSoapAutoDiscoverRequestError,
			descAutoDiscoverRequestError,
			SearchOverBudget,
			descUnknownWebRequestType,
			descIdentityArrayTooBig,
			TrackingLogsCorruptOnServer,
			descExceededMaxRedirectionDepth,
			SearchNonFullTextSortByProperty,
			descCrossForestServiceMissing,
			CrossServerCallFailed,
			PrimarySearchFolderTimeout,
			TrackingErrorUserObjectCorrupt,
			descVirusDetected,
			FailedToConvertSearchCriteriaToRestriction,
			PrimarySearchPopulationFailed,
			KeywordStatisticsSearchDisabled,
			descInvalidTypeInBookingPolicyConfig,
			EmptyMailboxStatsServerResponse,
			PreviewSearchDisabled,
			descFailedToCreateOrganizationalFolder,
			ArchiveSearchFolderTimeout,
			descSoapAutoDiscoverRequestUserSettingInvalidError,
			descInvalidSmtpAddress,
			DiscoverySearchAborted,
			descDateMustHaveZeroTimeSpan,
			SearchServerErrorMessage,
			SearchInvalidSortSpecification,
			InvalidRecipientArrayInPreviewResult,
			SearchTooManyMailboxes,
			InvalidPreviewSearchResults,
			descRecipientVersionNotSupported,
			SearchTaskTimeoutArchive,
			descInvalidGoodThreshold,
			descNotAValidExchangePrincipal,
			descMisconfiguredIntraOrganizationConnector,
			descSoapAutoDiscoverInvalidResponseError,
			EmptyRefinerServerResponse,
			descProxyNoResultError,
			UnknownRecurrencePattern,
			descEmptySecurityDescriptor,
			CreateFolderFailed,
			SearchWorkerError,
			descAutoDiscoverFailedWithException,
			SearchTransientError,
			descTimeIntervalTooBig,
			descMissingIntraforestCAS,
			InvalidFailedMailboxesResultWebServiceResponse,
			SearchAdminRpcInvalidQuery,
			MaxAllowedKeywordsExceeded,
			SortIComparableTypeException,
			SourceMailboxUserNotFoundInAD,
			descProxyRemoteServerError,
			descInvalidTimeInterval,
			TargetFolderNotFound,
			descCannotFindOrganizationalFolderInMailbox,
			UnknownBodyFormat,
			SearchFolderTimeout,
			LogMailSimpleHeader,
			InvalidIdInPreviewResult,
			CorruptedFolder,
			descUnsupportedSecurityDescriptorHeader,
			OWAServiceUrlFailure,
			SearchAdminRpcCallMaxSearches,
			descInvalidDayOrder,
			SearchAdminRpcCallAccessDenied,
			InvalidPreviewResultWebServiceResponse,
			descOnlyDefaultFreeBusyIntervalSupported,
			descResultSetTooBig,
			descTimeoutExpired,
			RefinerValueNullOrCountZero,
			descNotAContactOrUser,
			descFreeBusyDLLimitReached,
			descFailedToFindElcRoot,
			FailedToFetchPreviewItems,
			descInvalidMergedFreeBusyInterval,
			UnknownRecurrenceOrderType,
			descFailedToGetOrganizationalFoldersForMailbox,
			descElcNoMatchingOrgFolder,
			descInvalidTransitionBias,
			SearchUserNotFound,
			PhotoRetrievalFailedIOError,
			DatabaseLocationUnavailable,
			SearchTooManyKeywords,
			descFailedToCreateElcRootRetry,
			SearchAdminRpcSearchCallTimedout,
			TrackingRpcError,
			SearchNonFullTextPaginationProperty,
			descPFNotSupported,
			NestedFanout,
			descMailTipsSenderNotFound,
			CISearchFailed,
			CouldNotFindOrgRelationship,
			descInvalidYear,
			InvalidMailboxInMailboxStatistics,
			descAutoDiscoverBadRedirectLocation,
			SearchTaskTimeoutPrimary,
			SourceMailboxVersionError,
			InsufficientSpaceOnTargetMailbox,
			PhotoRetrievalFailedWin32Error,
			descSoapAutoDiscoverResponseError,
			descErrorEwsResponse,
			descInvalidConfigForCrossForestRequest,
			SearchAdminRpcCallFailed,
			SearchTaskCancelled,
			descMissingArgument,
			SearchTaskCancelledArchive,
			FailedToGetItem,
			descProxyRequestProcessingIOError,
			descInputFolderNamesContainDuplicates,
			RangedParameter,
			descPrimaryDefaultCorrupted,
			InvalidUnknownMailboxInPreviewResult,
			SourceMailboxCrossPremiseError,
			descE14orHigherProxyServerNotFound,
			RemoteMailbox
		}
	}
}
