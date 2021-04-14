using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ErrorCodes
	{
		internal const int None = 0;

		internal const int StoreTestFailure = 1000;

		internal const int UnknownUser = 1003;

		internal const int Exiting = 1005;

		internal const int LoginPerm = 1010;

		internal const int DatabaseError = 1108;

		internal const int UnsupportedProp = 1110;

		internal const int NoMessages = 2053;

		internal const int WarnCancelMessage = 263552;

		internal const int NoReplicaHere = 1128;

		internal const int NoReplicaAvailable = 1129;

		internal const int NoRecordFound = 1132;

		internal const int FormatError = 1261;

		internal const int MdbNotInit = 1142;

		internal const int RequiresRefResolve = 1150;

		internal const int MailboxInTransit = 1292;

		internal const int InvalidRecips = 1127;

		internal const int NotPrivateMDB = 1163;

		internal const int IsintegMDB = 1164;

		internal const int RecoveryMDBMismatch = 1165;

		internal const int SearchFolderNotEmpty = 1167;

		internal const int SearchFolderScopeViolation = 1168;

		internal const int CannotDeriveMsgViewFromBase = 1169;

		internal const int MsgHeaderIndexMismatch = 1170;

		internal const int MsgHeaderViewTableMismatch = 1171;

		internal const int CategViewTableMismatch = 1172;

		internal const int CorruptConversation = 1173;

		internal const int ConversationNotFound = 1174;

		internal const int ConversationMemberNotFound = 1175;

		internal const int VersionStoreBusy = 1176;

		internal const int SearchEvaluationInProgress = 1177;

		internal const int RecursiveSearchChainTooDeep = 1181;

		internal const int EmbeddedMessagePropertyCopyFailed = 1182;

		internal const int GlobalCounterRangeExceeded = 1185;

		internal const int CorruptMidsetDeleted = 1186;

		internal const int AssertionFailedError = 1199;

		internal const int NullObject = 1209;

		internal const int RpcAuthentication = 1212;

		internal const int TooManyRecips = 1285;

		internal const int TooManyProps = 1286;

		internal const int QuotaExceeded = 1241;

		internal const int MaxSubmissionExceeded = 1242;

		internal const int ShutoffQuotaExceeded = 1245;

		internal const int MessageTooBig = 1236;

		internal const int FormNotValid = 1237;

		internal const int NotAuthorized = 1238;

		internal const int FolderDisabled = 1275;

		internal const int InvalidRTF = 1235;

		internal const int SendAsDenied = 1244;

		internal const int NoCreateRight = 1279;

		internal const int DataLoss = 1157;

		internal const int MaxTimeExpired = 1140;

		internal const int NoCreateSubfolderRight = 1282;

		internal const int WrongMailbox = 1608;

		internal const int FolderNotCleanedUp = 1251;

		internal const int MessagePerFolderCountReceiveQuotaExceeded = 1252;

		internal const int FolderHierarchyChildrenCountReceiveQuotaExceeded = 1253;

		internal const int FolderHierarchyDepthReceiveQuotaExceeded = 1254;

		internal const int DynamicSearchFoldersPerScopeCountReceiveQuotaExceeded = 1255;

		internal const int FolderHierarchySizeReceiveQuotaExceeded = 1256;

		internal const int PublicRoot = 1280;

		internal const int MsgCycle = 1284;

		internal const int MaxAttachmentExceeded = 1243;

		internal const int WrongServer = 1144;

		internal const int VirusScanInProgress = 1290;

		internal const int VirusDetected = 1291;

		internal const int BackupInProgress = 1293;

		internal const int VirusMessageDeleted = 1294;

		internal const int PropsDontMatch = 1305;

		internal const int DuplicateObject = 1401;

		internal const int ChgPassword = 1612;

		internal const int PwdExpired = 1613;

		internal const int InvWkstn = 1614;

		internal const int InvLogonHrs = 1615;

		internal const int AcctDisabled = 1616;

		internal const int RuleVersion = 1700;

		internal const int RuleFormat = 1701;

		internal const int RuleSendAsDenied = 1702;

		internal const int CorruptStore = -2147219968;

		internal const int CorruptData = -2147221221;

		internal const int NotInQueue = -2147219967;

		internal const int NotInitialized = -2147219963;

		internal const int DuplicateName = -2147219964;

		internal const int FolderHasContents = -2147219958;

		internal const int FolderHasChildren = -2147219959;

		internal const int Incest = -2147219957;

		internal const int NotImplemented = -2147221246;

		internal const int PropBadValue = -2147220735;

		internal const int InvalidType = -2147220734;

		internal const int TypeNotSupported = -2147220733;

		internal const int NoFreeJses = 1100;

		internal const int MaxObjsExceeded = 1246;

		internal const int BufferTooSmall = 1149;

		internal const int ProtocolDisabled = 2008;

		internal const int CrossPostDenied = 2039;

		internal const int NoRpcInterface = 2084;

		internal const int AmbiguousAlias = 2202;

		internal const int UnknownMailbox = 2203;

		internal const int CorruptEvent = 2405;

		internal const int CorruptWatermark = 2406;

		internal const int EventError = 2407;

		internal const int WatermarkError = 2408;

		internal const int NonCanonicalACL = 2409;

		internal const int MailboxDisabled = 2412;

		internal const int ClientVerDisallowed = 1247;

		internal const int ServerPaused = 1151;

		internal const int ADUnavailable = 2414;

		internal const int ADError = 2415;

		internal const int ADNotFound = 2417;

		internal const int ADPropertyError = 2418;

		internal const int NotEncrypted = 2416;

		internal const int RpcServerTooBusy = 2419;

		internal const int RpcOutOfMemory = 2420;

		internal const int RpcServerOutOfMemory = 2421;

		internal const int RpcOutOfResources = 2422;

		internal const int RpcServerUnavailable = 2423;

		internal const int ADDuplicateEntry = 2424;

		internal const int ImailConversion = 2425;

		internal const int ImailConversionProhibited = 2427;

		internal const int EventsDeleted = 2428;

		internal const int SubsystemStopping = 2429;

		internal const int SAUnavailable = 2430;

		internal const int CIStopping = 2600;

		internal const int FxInvalidState = 2601;

		internal const int FxUnexpectedMarker = 2602;

		internal const int DuplicateDelivery = 2603;

		internal const int ConditionViolation = 2604;

		internal const int RpcInvalidHandle = 2606;

		internal const int EventNotFound = 2607;

		internal const int PropNotPromoted = 2608;

		internal const int LowDatabaseDiskSpace = 2609;

		internal const int LowDatabaseLogDiskSpace = 2610;

		internal const int MailboxQuarantined = 2611;

		internal const int MountInProgress = 2612;

		internal const int DismountInProgress = 2613;

		internal const int InvalidPool = 2617;

		internal const int VirusScannerError = 2618;

		internal const int GranularReplInitFailed = 2619;

		internal const int CannotRegisterNewReplidGuidMapping = 2620;

		internal const int CannotRegisterNewNamedPropertyMapping = 2621;

		internal const int GranularReplInvalidParameter = 2625;

		internal const int GranularReplStillInUse = 2626;

		internal const int GranularReplCommunicationFailed = 2628;

		internal const int CannotPreserveMailboxSignature = 2632;

		internal const int UnexpectedState = 2634;

		internal const int MailboxSoftDeleted = 2635;

		internal const int DatabaseStateConflict = 2636;

		internal const int RpcInvalidSession = 2637;

		internal const int MaxThreadsPerMdbExceeded = 2700;

		internal const int MaxThreadsPerSCTExceeded = 2701;

		internal const int WrongProvisionedFid = 2702;

		internal const int ISIntegMdbTaskExceeded = 2703;

		internal const int ISIntegQueueFull = 2704;

		internal const int InvalidMultiMailboxSearchRequest = 2800;

		internal const int InvalidMultiMailboxKeywordStatsRequest = 2801;

		internal const int MultiMailboxSearchFailed = 2802;

		internal const int MaxMultiMailboxSearchExceeded = 2803;

		internal const int MultiMailboxSearchOperationFailed = 2804;

		internal const int MultiMailboxSearchNonFullTextSearch = 2805;

		internal const int MultiMailboxSearchTimeOut = 2806;

		internal const int MultiMailboxKeywordStatsTimeOut = 2807;

		internal const int MultiMailboxSearchInvalidSortBy = 2808;

		internal const int MultiMailboxSearchNonFullTextSortBy = 2809;

		internal const int MultiMailboxSearchInvalidPagination = 2810;

		internal const int MultiMailboxSearchNonFullTextPropertyInPagination = 2811;

		internal const int MultiMailboxSearchMailboxNotFound = 2812;

		internal const int MultiMailboxSearchInvalidRestriction = 2813;

		internal const int UserInformationAlreadyExists = 2830;

		internal const int UserInformationLockTimeout = 2831;

		internal const int UserInformationNotFound = 2832;

		internal const int UserInformationNoAccess = 2833;

		internal const int JetWarningColumnMaxTruncated = 1512;

		internal const int JetErrorDatabaseBufferDependenciesCorrupted = -255;

		internal const int JetErrorLogWriteFail = -510;

		internal const int JetErrorBadParentPageLink = -338;

		internal const int JetErrorMissingLogFile = -528;

		internal const int JetErrorLogDiskFull = -529;

		internal const int JetErrorRequiredLogFilesMissing = -543;

		internal const int JetErrorConsistentTimeMismatch = -551;

		internal const int JetErrorCommittedLogFilesMissing = -582;

		internal const int JetErrorCommittedLogFilesCorrupt = -586;

		internal const int JetErrorUnicodeTranslationFail = -602;

		internal const int JetErrorCheckpointDepthTooDeep = -614;

		internal const int JetErrorOutOfMemory = -1011;

		internal const int JetErrorOutOfCursors = -1013;

		internal const int JetErrorOutOfBuffers = -1014;

		internal const int JetErrorRecordDeleted = -1017;

		internal const int JetErrorReadVerifyFailure = -1018;

		internal const int JetErrorPageNotInitialized = -1019;

		internal const int JetErrorDiskIO = -1022;

		internal const int JetErrorRecordTooBig = -1026;

		internal const int JetErrorInvalidBufferSize = -1047;

		internal const int JetErrorInvalidLanguageId = -1062;

		internal const int JetErrorVersionStoreOutOfMemoryAndCleanupTimedOut = -1066;

		internal const int JetErrorVersionStoreOutOfMemory = -1069;

		internal const int JetErrorInstanceNameInUse = -1086;

		internal const int JetErrorInstanceUnavailable = -1090;

		internal const int JetErrorInstanceUnavailableDueToFatalLogDiskFull = -1092;

		internal const int JetErrorOutOfSessions = -1101;

		internal const int JetErrorWriteConflict = -1102;

		internal const int JetErrorInvalidSesid = -1104;

		internal const int JetErrorDatabaseNotFound = -1203;

		internal const int JetErrorDatabaseCorrupted = -1206;

		internal const int JetErrorAttachedDatabaseMismatch = -1216;

		internal const int JetErrorDatabaseInvalidPath = -1217;

		internal const int JetErrorTableLocked = -1302;

		internal const int JetErrorTableDuplicate = -1303;

		internal const int JetErrorTableInUse = -1304;

		internal const int JetErrorObjectNotFound = -1305;

		internal const int JetErrorTooManyOpenTables = -1311;

		internal const int JetErrorTooManyOpenTablesAndCleanupTimedOut = -1313;

		internal const int JetErrorIndexNotFound = -1404;

		internal const int JetErrorColumnTooBig = -1506;

		internal const int JetErrorColumnNotFound = -1507;

		internal const int JetErrorBadColumnId = -1517;

		internal const int JetErrorDefaultValueTooBig = -1524;

		internal const int JetErrorLVCorrupted = -1526;

		internal const int JetErrorRecordNotFound = -1601;

		internal const int JetErrorNoCurrentRecord = -1603;

		internal const int JetErrorKeyDuplicate = -1605;

		internal const int JetErrorDiskFull = -1808;

		internal const int JetErrorFileNotFound = -1811;

		internal const int JetErrorFileIOBeyondEOF = -4001;
	}
}
