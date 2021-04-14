using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Extensibility.EventLog
{
	internal static class EdgeExtensibilityEventLogConstants
	{
		public const string EventSource = "MSExchange Extensibility";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentTooSlow = new ExEventLog.EventTuple(2147746842U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentFault = new ExEventLog.EventTuple(2147746843U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentFactoryCreationFailure = new ExEventLog.EventTuple(3221488668U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentInstanceCreationFailure = new ExEventLog.EventTuple(3221488669U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentVersionMismatch = new ExEventLog.EventTuple(3221488670U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentFactoryStartupDelay = new ExEventLog.EventTuple(2147746847U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentCompletedTwice = new ExEventLog.EventTuple(3221488672U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_MExAgentDidNotCallResume = new ExEventLog.EventTuple(3221488673U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			MExRuntime = 1
		}

		internal enum Message : uint
		{
			MExAgentTooSlow = 2147746842U,
			MExAgentFault,
			MExAgentFactoryCreationFailure = 3221488668U,
			MExAgentInstanceCreationFailure,
			MExAgentVersionMismatch,
			MExAgentFactoryStartupDelay = 2147746847U,
			MExAgentCompletedTwice = 3221488672U,
			MExAgentDidNotCallResume
		}
	}
}
