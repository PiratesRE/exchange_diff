using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.EventLogs
{
	internal static class FrontEndHttpProxyEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ApplicationStart = new ExEventLog.EventTuple(1073742825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ApplicationShutdown = new ExEventLog.EventTuple(1073742826U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InternalServerError = new ExEventLog.EventTuple(3221226475U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorLoadingSslCert = new ExEventLog.EventTuple(3221227473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TooManyOutstandingRequests = new ExEventLog.EventTuple(2147485650U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RefreshingDownLevelServerMap = new ExEventLog.EventTuple(1073744825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorRefreshDownLevelServerMap = new ExEventLog.EventTuple(3221228474U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RefreshingDownLevelServerStatus = new ExEventLog.EventTuple(1073744827U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PingingDownLevelServer = new ExEventLog.EventTuple(1073744828U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MarkingDownLevelServerUnhealthy = new ExEventLog.EventTuple(2147486653U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RefreshingBackEndServerCache = new ExEventLog.EventTuple(1073744830U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RefreshingDatabaseBackEndServer = new ExEventLog.EventTuple(1073744831U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.High, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorRefreshingDatabaseBackEndServer = new ExEventLog.EventTuple(2147486656U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			Core = 1
		}

		internal enum Message : uint
		{
			ApplicationStart = 1073742825U,
			ApplicationShutdown,
			InternalServerError = 3221226475U,
			ErrorLoadingSslCert = 3221227473U,
			TooManyOutstandingRequests = 2147485650U,
			RefreshingDownLevelServerMap = 1073744825U,
			ErrorRefreshDownLevelServerMap = 3221228474U,
			RefreshingDownLevelServerStatus = 1073744827U,
			PingingDownLevelServer,
			MarkingDownLevelServerUnhealthy = 2147486653U,
			RefreshingBackEndServerCache = 1073744830U,
			RefreshingDatabaseBackEndServer,
			ErrorRefreshingDatabaseBackEndServer = 2147486656U
		}
	}
}
