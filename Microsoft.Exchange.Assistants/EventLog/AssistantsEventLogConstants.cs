using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Assistants.EventLog
{
	public static class AssistantsEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseManagerStartedPrivateDatabase = new ExEventLog.EventTuple(271145U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseManagerStoppedDatabase = new ExEventLog.EventTuple(271146U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AssistantFailedToProcessEvent = new ExEventLog.EventTuple(2147754795U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeBasedAssistantFailed = new ExEventLog.EventTuple(2147754796U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AssistantSkippingEvent = new ExEventLog.EventTuple(2147754798U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxSessionException = new ExEventLog.EventTuple(2147754801U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseManagerTransientFailure = new ExEventLog.EventTuple(2147754802U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonEvent = new ExEventLog.EventTuple(3221496628U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CrashEvent = new ExEventLog.EventTuple(3221496629U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MissingSystemMailbox = new ExEventLog.EventTuple(2147754807U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DeadMailbox = new ExEventLog.EventTuple(2147754808U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeWindowBegin = new ExEventLog.EventTuple(271161U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeWindowEnd = new ExEventLog.EventTuple(271162U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeBasedAssistantStartFailed = new ExEventLog.EventTuple(2147754812U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeDemandJobBegin = new ExEventLog.EventTuple(271165U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeDemandJobEnd = new ExEventLog.EventTuple(271166U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeWindowBeginError = new ExEventLog.EventTuple(2147754815U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseManagerStartedPublicDatabase = new ExEventLog.EventTuple(271168U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SkippedMailboxes = new ExEventLog.EventTuple(271169U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_OnDemandStartError = new ExEventLog.EventTuple(2147754818U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PoisonMailbox = new ExEventLog.EventTuple(3221496643U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CrashMailbox = new ExEventLog.EventTuple(3221496644U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GovernorFailure = new ExEventLog.EventTuple(2147754821U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GovernorRecovery = new ExEventLog.EventTuple(271174U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GovernorGiveUp = new ExEventLog.EventTuple(2147754823U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GovernorRetry = new ExEventLog.EventTuple(271176U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeDemandEmptyJob = new ExEventLog.EventTuple(2147754825U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TimeHalt = new ExEventLog.EventTuple(271178U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_RpcError = new ExEventLog.EventTuple(3221496653U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseGovernorFailure = new ExEventLog.EventTuple(2147754830U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ServerGovernorFailure = new ExEventLog.EventTuple(2147754831U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_GrayException = new ExEventLog.EventTuple(1074013008U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_GenericException = new ExEventLog.EventTuple(2147754833U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseNotProcessedInTimeWindow = new ExEventLog.EventTuple(2147754834U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WorkCycleCheckpointError = new ExEventLog.EventTuple(2147754835U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MaximumConcurrentThreads = new ExEventLog.EventTuple(2147754836U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseUnhealthy = new ExEventLog.EventTuple(2147754837U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShutdownAssistantsThreadHanging = new ExEventLog.EventTuple(2147754901U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ShutdownAssistantsThreadHangPersisted = new ExEventLog.EventTuple(2147754902U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DatabaseStatusThreadResumed = new ExEventLog.EventTuple(2147754903U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MailboxesWithDecayedWatermarks = new ExEventLog.EventTuple(271256U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_RetryAssistantFailedToStart = new ExEventLog.EventTuple(2147754905U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AssistantFailedToStart = new ExEventLog.EventTuple(2147754906U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Expert, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			Assistants = 1
		}

		internal enum Message : uint
		{
			DatabaseManagerStartedPrivateDatabase = 271145U,
			DatabaseManagerStoppedDatabase,
			AssistantFailedToProcessEvent = 2147754795U,
			TimeBasedAssistantFailed,
			AssistantSkippingEvent = 2147754798U,
			MailboxSessionException = 2147754801U,
			DatabaseManagerTransientFailure,
			PoisonEvent = 3221496628U,
			CrashEvent,
			MissingSystemMailbox = 2147754807U,
			DeadMailbox,
			TimeWindowBegin = 271161U,
			TimeWindowEnd,
			TimeBasedAssistantStartFailed = 2147754812U,
			TimeDemandJobBegin = 271165U,
			TimeDemandJobEnd,
			TimeWindowBeginError = 2147754815U,
			DatabaseManagerStartedPublicDatabase = 271168U,
			SkippedMailboxes,
			OnDemandStartError = 2147754818U,
			PoisonMailbox = 3221496643U,
			CrashMailbox,
			GovernorFailure = 2147754821U,
			GovernorRecovery = 271174U,
			GovernorGiveUp = 2147754823U,
			GovernorRetry = 271176U,
			TimeDemandEmptyJob = 2147754825U,
			TimeHalt = 271178U,
			RpcError = 3221496653U,
			DatabaseGovernorFailure = 2147754830U,
			ServerGovernorFailure,
			GrayException = 1074013008U,
			GenericException = 2147754833U,
			DatabaseNotProcessedInTimeWindow,
			WorkCycleCheckpointError,
			MaximumConcurrentThreads,
			DatabaseUnhealthy,
			ShutdownAssistantsThreadHanging = 2147754901U,
			ShutdownAssistantsThreadHangPersisted,
			DatabaseStatusThreadResumed,
			MailboxesWithDecayedWatermarks = 271256U,
			RetryAssistantFailedToStart = 2147754905U,
			AssistantFailedToStart
		}
	}
}
