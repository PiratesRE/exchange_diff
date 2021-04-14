using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.AirSync
{
	public static class AirSyncEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BadItemReportConversionException = new ExEventLog.EventTuple(3221488623U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AirSyncException = new ExEventLog.EventTuple(2147746800U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidHbiLimits = new ExEventLog.EventTuple(2147746801U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MaxHbiTooHigh = new ExEventLog.EventTuple(2147746802U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MinHbiTooLow = new ExEventLog.EventTuple(2147746803U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AirSyncLoaded = new ExEventLog.EventTuple(2147746805U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AirSyncUnloaded = new ExEventLog.EventTuple(2147746806U, 3, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveDirectoryTransientError = new ExEventLog.EventTuple(3221488631U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ContentIndexingNotEnabled = new ExEventLog.EventTuple(2147746810U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DisabledDeviceIdTryingToSync = new ExEventLog.EventTuple(2147746811U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DeviceIdAndDeviceTypeMustBePresent = new ExEventLog.EventTuple(2147746812U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NonconformingDeviceError = new ExEventLog.EventTuple(2147746813U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ConnectionFailed = new ExEventLog.EventTuple(2147746814U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxOffline = new ExEventLog.EventTuple(2147746815U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxAccessDenied = new ExEventLog.EventTuple(2147746816U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AccountDisabled = new ExEventLog.EventTuple(2147746817U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxQuotaExceeded = new ExEventLog.EventTuple(2147746818U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_BadProxyServerVersion = new ExEventLog.EventTuple(2147746820U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UserOnNewMailboxCannotSync = new ExEventLog.EventTuple(2147746821U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UserOnLegacyMailboxCannotSync = new ExEventLog.EventTuple(2147746822U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UserHasBeenDisabled = new ExEventLog.EventTuple(2147746823U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxConnectionFailed = new ExEventLog.EventTuple(2147746824U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalValueHasBeenDefaulted = new ExEventLog.EventTuple(2147746825U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ProxyResultTimedOut = new ExEventLog.EventTuple(2147746826U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SecondCasFailureSSL = new ExEventLog.EventTuple(2147746827U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SecondCasFailureNTLM = new ExEventLog.EventTuple(2147746828U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ClientDisabledFromSyncEvent = new ExEventLog.EventTuple(2147746829U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SetPrivilegesFailure = new ExEventLog.EventTuple(3221488654U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MultipleDefaultMobileMailboxPoliciesDetected = new ExEventLog.EventTuple(2147746831U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AverageHbiTooLow = new ExEventLog.EventTuple(2147746832U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HeartbeatSampleSizeTooHigh = new ExEventLog.EventTuple(2147746833U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_HeartbeatAlertThresholdTooHigh = new ExEventLog.EventTuple(2147746834U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiscoveryInfoMissingError = new ExEventLog.EventTuple(2147746836U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalValueHasInvalidCharacters = new ExEventLog.EventTuple(2147746838U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalValueHasBeenDefaultedAndIgnored = new ExEventLog.EventTuple(2147746839U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CustomTypeDoesNotStartWithIPM = new ExEventLog.EventTuple(2147746840U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RunAsLocalSystem = new ExEventLog.EventTuple(3221488665U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UpLevelProxyNotSupported = new ExEventLog.EventTuple(2147746844U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToCreateADDevicesContainer = new ExEventLog.EventTuple(3221488669U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnableToCreateADDevice = new ExEventLog.EventTuple(3221488670U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UserHasBeenBlocked = new ExEventLog.EventTuple(2147746847U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidFireWallConfiguration = new ExEventLog.EventTuple(3221488716U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToRegisterADNotification = new ExEventLog.EventTuple(2147746893U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToLoadADSettings = new ExEventLog.EventTuple(2147746894U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveDirectoryExternalError = new ExEventLog.EventTuple(3221488719U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveDirectoryOperationError = new ExEventLog.EventTuple(3221488720U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AirSyncFatalException = new ExEventLog.EventTuple(3221488721U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AirSyncUnhandledException = new ExEventLog.EventTuple(3221488722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SyncStateExisted = new ExEventLog.EventTuple(2147746899U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InvalidSyncStateVersion = new ExEventLog.EventTuple(2147746900U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ServerExternalUrlConfigurationError = new ExEventLog.EventTuple(3221488725U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoAdminMailRecipientsError = new ExEventLog.EventTuple(3221488726U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_NoGccStoredSecretKey = new ExEventLog.EventTuple(3221488727U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_DirectoryAccessDenied = new ExEventLog.EventTuple(3221488728U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_NoMailboxRights = new ExEventLog.EventTuple(3221488729U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_NoOrgSettings = new ExEventLog.EventTuple(3221488730U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_NoPerfCounterTimer = new ExEventLog.EventTuple(2147746907U, 2, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_NoDeviceClassContainer = new ExEventLog.EventTuple(3221488732U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyDeviceClassNodes = new ExEventLog.EventTuple(3221488733U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalValueOutOfRange = new ExEventLog.EventTuple(3221488734U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalValueNotParsable = new ExEventLog.EventTuple(3221488735U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalValueRegistryReadFailure = new ExEventLog.EventTuple(3221488736U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GlobalValueRegistryValueMissing = new ExEventLog.EventTuple(3221488737U, 2, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			Requests = 1,
			Configuration,
			Server
		}

		internal enum Message : uint
		{
			BadItemReportConversionException = 3221488623U,
			AirSyncException = 2147746800U,
			InvalidHbiLimits,
			MaxHbiTooHigh,
			MinHbiTooLow,
			AirSyncLoaded = 2147746805U,
			AirSyncUnloaded,
			ActiveDirectoryTransientError = 3221488631U,
			ContentIndexingNotEnabled = 2147746810U,
			DisabledDeviceIdTryingToSync,
			DeviceIdAndDeviceTypeMustBePresent,
			NonconformingDeviceError,
			ConnectionFailed,
			MailboxOffline,
			MailboxAccessDenied,
			AccountDisabled,
			MailboxQuotaExceeded,
			BadProxyServerVersion = 2147746820U,
			UserOnNewMailboxCannotSync,
			UserOnLegacyMailboxCannotSync,
			UserHasBeenDisabled,
			MailboxConnectionFailed,
			GlobalValueHasBeenDefaulted,
			ProxyResultTimedOut,
			SecondCasFailureSSL,
			SecondCasFailureNTLM,
			ClientDisabledFromSyncEvent,
			SetPrivilegesFailure = 3221488654U,
			MultipleDefaultMobileMailboxPoliciesDetected = 2147746831U,
			AverageHbiTooLow,
			HeartbeatSampleSizeTooHigh,
			HeartbeatAlertThresholdTooHigh,
			DiscoveryInfoMissingError = 2147746836U,
			GlobalValueHasInvalidCharacters = 2147746838U,
			GlobalValueHasBeenDefaultedAndIgnored,
			CustomTypeDoesNotStartWithIPM,
			RunAsLocalSystem = 3221488665U,
			UpLevelProxyNotSupported = 2147746844U,
			UnableToCreateADDevicesContainer = 3221488669U,
			UnableToCreateADDevice,
			UserHasBeenBlocked = 2147746847U,
			InvalidFireWallConfiguration = 3221488716U,
			FailedToRegisterADNotification = 2147746893U,
			FailedToLoadADSettings,
			ActiveDirectoryExternalError = 3221488719U,
			ActiveDirectoryOperationError,
			AirSyncFatalException,
			AirSyncUnhandledException,
			SyncStateExisted = 2147746899U,
			InvalidSyncStateVersion,
			ServerExternalUrlConfigurationError = 3221488725U,
			NoAdminMailRecipientsError,
			NoGccStoredSecretKey,
			DirectoryAccessDenied,
			NoMailboxRights,
			NoOrgSettings,
			NoPerfCounterTimer = 2147746907U,
			NoDeviceClassContainer = 3221488732U,
			TooManyDeviceClassNodes,
			GlobalValueOutOfRange,
			GlobalValueNotParsable,
			GlobalValueRegistryReadFailure,
			GlobalValueRegistryValueMissing
		}
	}
}
