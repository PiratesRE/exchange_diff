using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public enum ErrorCodeValue
	{
		NoError,
		StoreTestFailure = 1000,
		UnknownUser = 1003,
		DatabaseRolledBack = 1011,
		DatabaseBadVersion = 1105,
		DatabaseError = 1108,
		InvalidCollapseState = 1118,
		NoDeleteSubmitMessage = 1125,
		RecoveryMDBMismatch = 1165,
		SearchFolderScopeViolation = 1168,
		SearchEvaluationInProgress = 1177,
		NestedSearchChainTooDeep = 1181,
		CorruptSearchScope = 1183,
		CorruptSearchBacklink,
		GlobalCounterRangeExceeded,
		CorruptMidsetDeleted,
		RpcFormat = 1206,
		QuotaExceeded = 1241,
		MaxSubmissionExceeded,
		MaxAttachmentExceeded,
		ShutoffQuotaExceeded = 1245,
		MaxObjectsExceeded,
		MessagePerFolderCountReceiveQuotaExceeded = 1252,
		FolderHierarchyChildrenCountReceiveQuotaExceeded,
		FolderHierarchyDepthReceiveQuotaExceeded,
		DynamicSearchFoldersPerScopeCountReceiveQuotaExceeded,
		FolderHierarchySizeReceiveQuotaExceeded,
		NotVisible = 1270,
		NotExpanded,
		NotCollapsed,
		Leaf,
		MessageCycle = 1284,
		Rejected = 2030,
		UnknownMailbox = 2203,
		DisabledMailbox = 2412,
		AdUnavailable = 2414,
		ADPropertyError = 2418,
		RpcServerTooBusy,
		RpcServerUnavailable = 2423,
		EventsDeleted = 2428,
		MaxPoolExceeded = 2605,
		EventNotFound = 2607,
		InvalidPool = 2617,
		BlockModeInitFailed = 2619,
		UnexpectedMailboxState = 2634,
		SoftDeletedMailbox,
		DatabaseStateConflict,
		RpcInvalidSession,
		PublicFolderDumpstersLimitExceeded,
		InvalidMultiMailboxSearchRequest = 2800,
		InvalidMultiMailboxKeywordStatsRequest,
		MultiMailboxSearchFailed,
		MaxMultiMailboxSearchExceeded,
		MultiMailboxSearchOperationFailed,
		MultiMailboxSearchNonFullTextSearch,
		MultiMailboxSearchTimeOut,
		MultiMailboxKeywordStatsTimeOut,
		MultiMailboxSearchInvalidSortBy,
		MultiMailboxSearchNonFullTextSortBy,
		MultiMailboxSearchInvalidPagination,
		MultiMailboxSearchNonFullTextPropertyInPagination,
		MultiMailboxSearchMailboxNotFound,
		MultiMailboxSearchInvalidRestriction,
		FullTextIndexCallFailed = 2820,
		UserInformationAlreadyExists = 2830,
		UserInformationLockTimeout,
		UserInformationNotFound,
		UserInformationNoAccess,
		UserInformationPropertyError,
		UserInformationSoftDeleted,
		InterfaceNotSupported = -2147467262,
		CallFailed = -2147467259,
		StreamAccessDenied = -2147287035,
		StgInsufficientMemory = -2147287032,
		StreamSeekError = -2147287015,
		LockViolation = -2147287007,
		StreamInvalidParam = -2147286953,
		NotSupported = -2147221246,
		BadCharWidth,
		StringTooLong = -2147221243,
		UnknownFlags,
		InvalidEntryId,
		InvalidObject,
		ObjectChanged,
		ObjectDeleted,
		Busy,
		NotEnoughDisk = -2147221235,
		NotEnoughResources,
		NotFound,
		VersionMismatch,
		LogonFailed,
		SessionLimit,
		UserCancel,
		UnableToAbort,
		NetworkError,
		DiskError,
		TooComplex,
		ConditionViolation = 2604,
		BadColumn = -2147221224,
		ExtendedError,
		Computed,
		CorruptData,
		Unconfigured,
		FailOneProvider,
		UnknownCPID,
		UnknownLCID,
		PasswordChangeRequired,
		PasswordExpired,
		InvalidWorkstationAccount,
		InvalidAccessTime,
		AccountDisabled,
		EndOfSession = -2147220992,
		UnknownEntryId,
		MissingRequiredColumn,
		FailCallback = -2147220967,
		BadValue = -2147220735,
		InvalidType,
		TypeNoSupport,
		UnexpectedType,
		TooBig,
		DeclineCopy,
		UnexpectedId,
		UnableToComplete = -2147220480,
		Timeout,
		TableEmpty,
		TableTooBig,
		InvalidBookmark = -2147220475,
		DataLoss = -2147220347,
		Wait = -2147220224,
		Cancel,
		NotMe,
		CorruptStore = -2147219968,
		NotInQueue,
		NoSuppress,
		Collision = -2147219964,
		NotInitialized,
		NonStandard,
		NoRecipients,
		Submitted,
		HasFolders,
		HasMessages,
		FolderCycle,
		RootFolder = -2147219957,
		RecursionLimit,
		LockIdLimit,
		TooManyMountedDatabases,
		PartialItem = -2147219834,
		AmbiguousRecip = -2147219712,
		SyncObjectDeleted = -2147219456,
		SyncIgnore,
		SyncConflict,
		SyncNoParent,
		SyncIncest,
		ErrorPathNotFound = -2147024893,
		NoAccess = -2147024891,
		NotEnoughMemory = -2147024882,
		InvalidParameter = -2147024809,
		ErrorCanNotComplete = -2147023893,
		NamedPropQuotaExceeded = -2147219200,
		TooManyRecips = 1285,
		TooManyProps,
		ParameterOverflow = 1104,
		BadFolderName = 1116,
		SearchFolder = 1120,
		NotSearchFolder,
		FolderSetReceive,
		NoReceiveFolder,
		InvalidRecipients = 1127,
		BufferTooSmall = 1149,
		RequiresRefResolve,
		NullObject = 1209,
		SendAsDenied = 1244,
		DestinationNullObject = 1283,
		NoService = 262659,
		ErrorsReturned = 263040,
		PositionChanged = 263297,
		ApproxCount,
		CancelMessage = 263552,
		PartialCompletion = 263808,
		SecurityRequiredLow,
		SecuirtyRequiredMedium,
		PartialItems = 263815,
		SyncProgress = 264224,
		SyncClientChangeNewer,
		Exiting = 1005,
		MdbNotInitialized = 1142,
		ServerOutOfMemory = 1008,
		MailboxInTransit = 1292,
		BackupInProgress,
		InvalidBackupSequence = 1295,
		WrongServer = 1144,
		MailboxQuarantined = 2611,
		MountInProgress,
		DismountInProgress,
		CannotRegisterNewReplidGuidMapping = 2620,
		CannotRegisterNewNamedPropertyMapping,
		CannotPreserveMailboxSignature = 2632,
		ExceptionThrown = 5000,
		SessionLocked,
		DuplicateObject = 1401,
		DuplicateDelivery = 2603,
		UnregisteredNamedProp = 1274,
		TaskRequestFailed = 5002,
		NoReplicaHere = 1128,
		NoReplicaAvailable
	}
}
