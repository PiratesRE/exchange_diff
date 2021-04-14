using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Servicelets.SACLWatcher.Messages
{
	public static class SACLWatcherEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorExchangeGroupNotFound = new ExEventLog.EventTuple(3221231473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorDomainControllerNotFound = new ExEventLog.EventTuple(3221231474U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorOpenPolicyFailed = new ExEventLog.EventTuple(3221231475U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorEnumerateRightsFailed = new ExEventLog.EventTuple(3221231476U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorAddAccountRightsFailed = new ExEventLog.EventTuple(3221231477U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WarningPrivilegeRemoved = new ExEventLog.EventTuple(2147489654U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InfoPrivilegeRecovered = new ExEventLog.EventTuple(1073747831U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorNoDomainController = new ExEventLog.EventTuple(3221232472U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorNoLocalDomain = new ExEventLog.EventTuple(3221232473U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			ErrorExchangeGroupNotFound = 3221231473U,
			ErrorDomainControllerNotFound,
			ErrorOpenPolicyFailed,
			ErrorEnumerateRightsFailed,
			ErrorAddAccountRightsFailed,
			WarningPrivilegeRemoved = 2147489654U,
			InfoPrivilegeRecovered = 1073747831U,
			ErrorNoDomainController = 3221232472U,
			ErrorNoLocalDomain
		}
	}
}
