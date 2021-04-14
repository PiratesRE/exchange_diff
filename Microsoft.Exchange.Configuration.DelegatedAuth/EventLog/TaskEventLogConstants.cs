using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.DelegatedAuthentication.EventLog
{
	public static class TaskEventLogConstants
	{
		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_RequestNotAuthenticated = new ExEventLog.EventTuple(3221225672U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_FailedToResolveCurrentUser = new ExEventLog.EventTuple(3221225673U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_FailedToDecryptSecurityToken = new ExEventLog.EventTuple(3221225674U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_ExpiredSecurityToken = new ExEventLog.EventTuple(3221225675U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_NoGroupMembershipOnSecurityToken = new ExEventLog.EventTuple(3221225676U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_CannotResolveForestRedirection = new ExEventLog.EventTuple(3221225677U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_AccessDenied = new ExEventLog.EventTuple(3221225678U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_FailedToResolveSecretKey = new ExEventLog.EventTuple(3221225679U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_ServerError = new ExEventLog.EventTuple(3221225680U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_FailedToDecodeBase64SecurityToken = new ExEventLog.EventTuple(3221225681U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_FailedToResolveTargetOrganization = new ExEventLog.EventTuple(3221225682U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_DelegatedPrincipalCacheIsFull = new ExEventLog.EventTuple(3221225683U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DelegatedAuth_FailedToReadMultiple = new ExEventLog.EventTuple(3221225684U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			DelegatedAuth_RequestNotAuthenticated = 3221225672U,
			DelegatedAuth_FailedToResolveCurrentUser,
			DelegatedAuth_FailedToDecryptSecurityToken,
			DelegatedAuth_ExpiredSecurityToken,
			DelegatedAuth_NoGroupMembershipOnSecurityToken,
			DelegatedAuth_CannotResolveForestRedirection,
			DelegatedAuth_AccessDenied,
			DelegatedAuth_FailedToResolveSecretKey,
			DelegatedAuth_ServerError,
			DelegatedAuth_FailedToDecodeBase64SecurityToken,
			DelegatedAuth_FailedToResolveTargetOrganization,
			DelegatedAuth_DelegatedPrincipalCacheIsFull,
			DelegatedAuth_FailedToReadMultiple
		}
	}
}
