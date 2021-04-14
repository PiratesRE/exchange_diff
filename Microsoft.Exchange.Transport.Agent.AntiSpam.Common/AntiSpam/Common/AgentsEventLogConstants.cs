using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Agent.AntiSpam.Common
{
	public static class AgentsEventLogConstants
	{
		public const string EventSource = "MSExchange Antispam";

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AgentQueueFull = new ExEventLog.EventTuple(2147745892U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_DnsNotConfigured = new ExEventLog.EventTuple(2147745893U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_ProtocolAnalysisBg = new ExEventLog.EventTuple(2147745992U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterInitialized = new ExEventLog.EventTuple(262444U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterNotInitialized = new ExEventLog.EventTuple(3221487917U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterQuarantineMailboxIsInvalid = new ExEventLog.EventTuple(2147746094U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Medium, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterInitFailedUnauthorizedAccess = new ExEventLog.EventTuple(3221487919U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterInitFailedBadImageFormat = new ExEventLog.EventTuple(3221487920U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterInitFailedFSWatcherAlreadyInitialized = new ExEventLog.EventTuple(3221487921U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterInitFailedInsufficientBuffer = new ExEventLog.EventTuple(3221487922U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterWrapperNotResponding = new ExEventLog.EventTuple(3221487923U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterWrapperBeingRecycled = new ExEventLog.EventTuple(2147746100U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterWrapperSuccessfullyRecycled = new ExEventLog.EventTuple(262453U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterWrapperRecycleTimedout = new ExEventLog.EventTuple(3221487926U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterWrapperRecycleError = new ExEventLog.EventTuple(3221487927U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterWrapperSendingPingRequest = new ExEventLog.EventTuple(2147746104U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterWrapperErrorSubmittingMessage = new ExEventLog.EventTuple(2147746105U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Low, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ExSMimeFailedToInitialize = new ExEventLog.EventTuple(3221487930U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnexpectedFailureScanningMessage = new ExEventLog.EventTuple(3221487931U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToReadAntispamUpdateMode = new ExEventLog.EventTuple(3221487932U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AntispamUpdateModeChanged = new ExEventLog.EventTuple(262461U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ContentFilterInitFailedFileNotFound = new ExEventLog.EventTuple(3221487934U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UpdateAgentFileNotLoaded = new ExEventLog.EventTuple(2147746192U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToReadConfiguration = new ExEventLog.EventTuple(3221488116U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_ConnectionFilteringDnsNotConfigured = new ExEventLog.EventTuple(3221488216U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_SenderIdDnsNotConfigured = new ExEventLog.EventTuple(3221488316U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PartnerConfigurationLoadingError = new ExEventLog.EventTuple(3221488416U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AgentLogPathIsNull = new ExEventLog.EventTuple(3221488516U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SenderRecipientThrottlingAgentMessageRejected = new ExEventLog.EventTuple(264145U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_SenderRecipientThrottlingAgentMessageAccepted = new ExEventLog.EventTuple(264146U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AlertStringFormatExceptionGenerated = new ExEventLog.EventTuple(264147U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_AddressBookPolicyLoadingError = new ExEventLog.EventTuple(3221489716U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_SenderRecipientPairWithSubjectAutoNuked = new ExEventLog.EventTuple(3221489619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			AgentQueueFull = 2147745892U,
			DnsNotConfigured,
			ProtocolAnalysisBg = 2147745992U,
			ContentFilterInitialized = 262444U,
			ContentFilterNotInitialized = 3221487917U,
			ContentFilterQuarantineMailboxIsInvalid = 2147746094U,
			ContentFilterInitFailedUnauthorizedAccess = 3221487919U,
			ContentFilterInitFailedBadImageFormat,
			ContentFilterInitFailedFSWatcherAlreadyInitialized,
			ContentFilterInitFailedInsufficientBuffer,
			ContentFilterWrapperNotResponding,
			ContentFilterWrapperBeingRecycled = 2147746100U,
			ContentFilterWrapperSuccessfullyRecycled = 262453U,
			ContentFilterWrapperRecycleTimedout = 3221487926U,
			ContentFilterWrapperRecycleError,
			ContentFilterWrapperSendingPingRequest = 2147746104U,
			ContentFilterWrapperErrorSubmittingMessage,
			ExSMimeFailedToInitialize = 3221487930U,
			UnexpectedFailureScanningMessage,
			FailedToReadAntispamUpdateMode,
			AntispamUpdateModeChanged = 262461U,
			ContentFilterInitFailedFileNotFound = 3221487934U,
			UpdateAgentFileNotLoaded = 2147746192U,
			FailedToReadConfiguration = 3221488116U,
			ConnectionFilteringDnsNotConfigured = 3221488216U,
			SenderIdDnsNotConfigured = 3221488316U,
			PartnerConfigurationLoadingError = 3221488416U,
			AgentLogPathIsNull = 3221488516U,
			SenderRecipientThrottlingAgentMessageRejected = 264145U,
			SenderRecipientThrottlingAgentMessageAccepted,
			AlertStringFormatExceptionGenerated,
			AddressBookPolicyLoadingError = 3221489716U,
			SenderRecipientPairWithSubjectAutoNuked = 3221489619U
		}
	}
}
