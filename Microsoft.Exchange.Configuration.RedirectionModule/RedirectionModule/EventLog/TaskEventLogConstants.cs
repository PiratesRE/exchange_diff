using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Configuration.RedirectionModule.EventLog
{
	public static class TaskEventLogConstants
	{
		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_ServerError = new ExEventLog.EventTuple(3221225622U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_FailedSidMapping = new ExEventLog.EventTuple(3221225623U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_FailedWindowsIdMapping = new ExEventLog.EventTuple(3221225624U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_FailedPUIDMapping = new ExEventLog.EventTuple(3221225625U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_FailedExtractMemberName = new ExEventLog.EventTuple(3221225626U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_FailedToResolveForestRedirection = new ExEventLog.EventTuple(3221225627U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_UsingManagementSiteLink = new ExEventLog.EventTuple(3221225629U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_LiveIdRedirection_TargetSitePresentOnResponsibleForSite = new ExEventLog.EventTuple(3221225630U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_TenantRedirection_ServerError = new ExEventLog.EventTuple(3221225722U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TenantRedirection_FailedToResolveForestRedirection = new ExEventLog.EventTuple(3221225723U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ReaderWriterLock_Timeout = new ExEventLog.EventTuple(3221225724U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			LiveIdRedirection_ServerError = 3221225622U,
			LiveIdRedirection_FailedSidMapping,
			LiveIdRedirection_FailedWindowsIdMapping,
			LiveIdRedirection_FailedPUIDMapping,
			LiveIdRedirection_FailedExtractMemberName,
			LiveIdRedirection_FailedToResolveForestRedirection,
			LiveIdRedirection_UsingManagementSiteLink = 3221225629U,
			LiveIdRedirection_TargetSitePresentOnResponsibleForSite,
			TenantRedirection_ServerError = 3221225722U,
			TenantRedirection_FailedToResolveForestRedirection,
			ReaderWriterLock_Timeout
		}
	}
}
