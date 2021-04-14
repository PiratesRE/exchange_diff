using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class Errors
	{
		internal const int MapiCallFailed = -2147467259;

		internal const int MapiNotEnoughMemory = -2147024882;

		internal const int MapiInvalidParameter = -2147024809;

		internal const int MapiInterfaceNotSupported = -2147467262;

		internal const int MapiNoAccess = -2147024891;

		internal const int MapiNoSupport = -2147221246;

		internal const int MapiBadCharWidth = -2147221245;

		internal const int MapiStringTooLong = -2147221243;

		internal const int MapiUnknownFlags = -2147221242;

		internal const int MapiInvalidEntryId = -2147221241;

		internal const int MapiInvalidObject = -2147221240;

		internal const int MapiObjectChanged = -2147221239;

		internal const int MapiObjectDeleted = -2147221238;

		internal const int MapiBusy = -2147221237;

		internal const int MapiNotEnoughDisk = -2147221235;

		internal const int MapiNotEnoughResources = -2147221234;

		internal const int MapiNotFound = -2147221233;

		internal const int MapiVersion = -2147221232;

		internal const int MapiLogonFailed = -2147221231;

		internal const int MapiSessionLimit = -2147221230;

		internal const int MapiUserCancel = -2147221229;

		internal const int MapiUnableToAbort = -2147221228;

		internal const int MapiNetworkError = -2147221227;

		internal const int MapiDiskError = -2147221226;

		internal const int MapiTooComplex = -2147221225;

		internal const int MapiBadColumn = -2147221224;

		internal const int MapiExtendedError = -2147221223;

		internal const int MapiComputed = -2147221222;

		internal const int MapiCorruptData = -2147221221;

		internal const int MapiUnconfigured = -2147221220;

		internal const int MapiFailOneProvider = -2147221219;

		internal const int MapiUnknownCpid = -2147221218;

		internal const int MapiUnknownLcid = -2147221217;

		internal const int MapiPasswordChangeRequired = -2147221216;

		internal const int MapiPasswordExpired = -2147221215;

		internal const int MapiInvalidWorkstationAccount = -2147221214;

		internal const int MapiInvalidAccessTime = -2147221213;

		internal const int MapiAccountDisabled = -2147221212;

		internal const int MapiConflict = -2147221211;

		internal const int MapiEndOfSession = -2147220992;

		internal const int MapiUnknownEntryId = -2147220991;

		internal const int MapiMissingRequiredColumn = -2147220990;

		internal const int MapiBadValue = -2147220735;

		internal const int MapiInvalidType = -2147220734;

		internal const int MapiTypeNoSupport = -2147220733;

		internal const int MapiUnexpectedType = -2147220732;

		internal const int MapiTooBig = -2147220731;

		internal const int MapiDeclineCopy = -2147220730;

		internal const int MapiUnexpectedId = -2147220729;

		internal const int MapiUnableToComplete = -2147220480;

		internal const int MapiTimeout = -2147220479;

		internal const int MapiTableEmpty = -2147220478;

		internal const int MapiTableTooBig = -2147220477;

		internal const int MapiInvalidBookmark = -2147220475;

		internal const int MapiWait = -2147220224;

		internal const int MapiCancel = -2147220223;

		internal const int MapiNotMe = -2147220222;

		internal const int MapiCorruptStore = -2147219968;

		internal const int MapiNotInQueue = -2147219967;

		internal const int MapiNoSuppress = -2147219966;

		internal const int MapiCollision = -2147219964;

		internal const int MapiNotInitialized = -2147219963;

		internal const int MapiNonStandard = -2147219962;

		internal const int MapiNoRecipients = -2147219961;

		internal const int MapiSubmitted = -2147219960;

		internal const int MapiHasFolders = -2147219959;

		internal const int MapiHasMessages = -2147219958;

		internal const int MapiFolderCycle = -2147219957;

		internal const int MapiRecursionLimit = -2147219956;

		internal const int MapiDataLoss = -2147220347;

		internal const int MapiTooManyRecips = -1073478950;

		internal const int MapiLockIdLimit = -2147219955;

		internal const int MapiTooManyMountedDatabases = -2147219954;

		internal const int MapiPartialItem = -2147219834;

		internal const int MapiAmbiguousRecip = -2147219712;

		internal const int MapiNamedPropQuotaExceeded = -2147219200;

		internal const int SyncObjectDeleted = -2147219456;

		internal const int SyncIgnore = -2147219455;

		internal const int SyncConflict = -2147219454;

		internal const int SyncNoParent = -2147219453;

		internal const int SyncIncest = -2147219452;

		internal const int SyncUnsynchronized = -2147219451;

		internal const int ErrorCanNotComplete = -2147023893;

		internal const int ErrorPathNotFound = -2147024893;

		internal const int ErrorInsufficientBuffer = -2147024774;

		internal const int ErrorCanceled = -2147023673;

		internal const int FailCallback = -2147220967;

		internal const int StgInsufficientMemory = -2147287032;

		internal const int StgInvalidParameter = -2147286953;

		internal const int StgLockViolation = -2147287007;

		internal const int StreamSizeError = -2147286928;

		internal const int StreamSeekError = -2147287015;

		internal const int Win32ErrorDiskFull = -2147024784;
	}
}
