using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.DiagnosticsAggregation.Messages
{
	public static class MSExchangeDiagnosticsAggregationEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiagnosticsAggregationServiceletIsDisabled = new ExEventLog.EventTuple(1073742825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiagnosticsAggregationServiceUnexpectedException = new ExEventLog.EventTuple(3221226475U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiagnosticsAggregationServiceletLoadFailed = new ExEventLog.EventTuple(3221226476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DiagnosticsAggregationRehostingFailed = new ExEventLog.EventTuple(3221226478U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			DiagnosticsAggregationServiceletIsDisabled = 1073742825U,
			DiagnosticsAggregationServiceUnexpectedException = 3221226475U,
			DiagnosticsAggregationServiceletLoadFailed,
			DiagnosticsAggregationRehostingFailed = 3221226478U
		}
	}
}
