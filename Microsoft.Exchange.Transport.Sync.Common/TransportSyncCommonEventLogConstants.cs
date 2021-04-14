using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	internal static class TransportSyncCommonEventLogConstants
	{
		public const string EventSource = "MSExchangeTransportSyncCommon";

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_VerificationEmailNotSent = new ExEventLog.EventTuple(3221487619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PerformanceCounterAccessDenied = new ExEventLog.EventTuple(3221487620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			SyncCommon = 1
		}

		internal enum Message : uint
		{
			VerificationEmailNotSent = 3221487619U,
			PerformanceCounterAccessDenied
		}
	}
}
