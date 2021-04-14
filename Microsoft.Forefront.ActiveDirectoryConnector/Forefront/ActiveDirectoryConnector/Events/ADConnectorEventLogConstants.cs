using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Forefront.ActiveDirectoryConnector.Events
{
	public static class ADConnectorEventLogConstants
	{
		public const string EventSource = "Filtering ADConnector";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherStarted = new ExEventLog.EventTuple(1073743824U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherStartException = new ExEventLog.EventTuple(3221227473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherStopped = new ExEventLog.EventTuple(1073743834U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherStopException = new ExEventLog.EventTuple(3221227483U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherProcessId = new ExEventLog.EventTuple(1073743844U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherGetProcessIdException = new ExEventLog.EventTuple(3221227493U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherChangeHandlersRegistered = new ExEventLog.EventTuple(1073743854U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherRegisterConfigurationChangeHandlersReadServerConfigFailed = new ExEventLog.EventTuple(3221227503U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherChangeHandlersUnRegistered = new ExEventLog.EventTuple(1073743864U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherServerConfigUpdateNotification = new ExEventLog.EventTuple(1073743874U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherServerConfigUpdateNoChanges = new ExEventLog.EventTuple(1073743875U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherServerConfigUpdateException = new ExEventLog.EventTuple(3221227524U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherReadServerConfigFailed = new ExEventLog.EventTuple(3221227532U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherGetFilteringSettingsFromServerConfigException = new ExEventLog.EventTuple(3221227533U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherSetFilteringSettingsToFips = new ExEventLog.EventTuple(1073743894U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherServerConfigUpdateErrorAddingSnapin = new ExEventLog.EventTuple(3221227543U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ADFilteringSettingWatcherServerConfigUpdateErrorSettingFilteringServiceSettings = new ExEventLog.EventTuple(3221227544U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			ADFilteringSettingWatcherStarted = 1073743824U,
			ADFilteringSettingWatcherStartException = 3221227473U,
			ADFilteringSettingWatcherStopped = 1073743834U,
			ADFilteringSettingWatcherStopException = 3221227483U,
			ADFilteringSettingWatcherProcessId = 1073743844U,
			ADFilteringSettingWatcherGetProcessIdException = 3221227493U,
			ADFilteringSettingWatcherChangeHandlersRegistered = 1073743854U,
			ADFilteringSettingWatcherRegisterConfigurationChangeHandlersReadServerConfigFailed = 3221227503U,
			ADFilteringSettingWatcherChangeHandlersUnRegistered = 1073743864U,
			ADFilteringSettingWatcherServerConfigUpdateNotification = 1073743874U,
			ADFilteringSettingWatcherServerConfigUpdateNoChanges,
			ADFilteringSettingWatcherServerConfigUpdateException = 3221227524U,
			ADFilteringSettingWatcherReadServerConfigFailed = 3221227532U,
			ADFilteringSettingWatcherGetFilteringSettingsFromServerConfigException,
			ADFilteringSettingWatcherSetFilteringSettingsToFips = 1073743894U,
			ADFilteringSettingWatcherServerConfigUpdateErrorAddingSnapin = 3221227543U,
			ADFilteringSettingWatcherServerConfigUpdateErrorSettingFilteringServiceSettings
		}
	}
}
