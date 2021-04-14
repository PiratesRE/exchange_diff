using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	public static class MSExchangeTransportLogSearchEventLogConstants
	{
		public const string EventSource = "MSExchangeTransportLogSearch";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceStartSuccess = new ExEventLog.EventTuple(269145U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceStopSuccess = new ExEventLog.EventTuple(269146U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceStopFailure = new ExEventLog.EventTuple(3221494620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchReadConfigFailed = new ExEventLog.EventTuple(3221494621U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchUnAuthorizedFileAccess = new ExEventLog.EventTuple(2147752801U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchIOException = new ExEventLog.EventTuple(2147752802U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchNullOrEmptyLogPath = new ExEventLog.EventTuple(3221494627U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchLogFileCorrupted = new ExEventLog.EventTuple(2147752804U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceStartFailureInit = new ExEventLog.EventTuple(3221494629U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceStartFailureSessionManager = new ExEventLog.EventTuple(3221494630U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceStartFailureMTGLog = new ExEventLog.EventTuple(3221494631U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceCreateDirectoryFailed = new ExEventLog.EventTuple(3221494632U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchClientQuery = new ExEventLog.EventTuple(269164U, 2, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceIndexingComplete = new ExEventLog.EventTuple(269147U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceIndexFileCorrupt = new ExEventLog.EventTuple(3221494633U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LogSearchServiceLogFileTooLarge = new ExEventLog.EventTuple(3221494634U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ActiveUserStatisticsLogPathIsNull = new ExEventLog.EventTuple(2147752811U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServerStatisticsLogPathIsNull = new ExEventLog.EventTuple(2147752812U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TransportSyncLogEntryReadingFailure = new ExEventLog.EventTuple(3221494637U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TransportSyncLogEntryProcessingFailure = new ExEventLog.EventTuple(3221494639U, 3, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RegistryAccessDenied = new ExEventLog.EventTuple(3221494648U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RegistryExchangeInstallPathNotFound = new ExEventLog.EventTuple(3221494649U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorReadingAppConfig = new ExEventLog.EventTuple(3221494650U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			General = 1,
			Client_Monitoring,
			Transport_Sync
		}

		internal enum Message : uint
		{
			LogSearchServiceStartSuccess = 269145U,
			LogSearchServiceStopSuccess,
			LogSearchServiceStopFailure = 3221494620U,
			LogSearchReadConfigFailed,
			LogSearchUnAuthorizedFileAccess = 2147752801U,
			LogSearchIOException,
			LogSearchNullOrEmptyLogPath = 3221494627U,
			LogSearchLogFileCorrupted = 2147752804U,
			LogSearchServiceStartFailureInit = 3221494629U,
			LogSearchServiceStartFailureSessionManager,
			LogSearchServiceStartFailureMTGLog,
			LogSearchServiceCreateDirectoryFailed,
			LogSearchClientQuery = 269164U,
			LogSearchServiceIndexingComplete = 269147U,
			LogSearchServiceIndexFileCorrupt = 3221494633U,
			LogSearchServiceLogFileTooLarge,
			ActiveUserStatisticsLogPathIsNull = 2147752811U,
			ServerStatisticsLogPathIsNull,
			TransportSyncLogEntryReadingFailure = 3221494637U,
			TransportSyncLogEntryProcessingFailure = 3221494639U,
			RegistryAccessDenied = 3221494648U,
			RegistryExchangeInstallPathNotFound,
			ErrorReadingAppConfig
		}
	}
}
