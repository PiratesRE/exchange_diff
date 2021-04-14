using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics.Components.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal class TimeBasedAdminRpcDatabaseDriver : TimeBasedDatabaseDriver
	{
		internal TimeBasedAdminRpcDatabaseDriver(ThrottleGovernor parentGovernor, DatabaseInfo databaseInfo, ITimeBasedAssistantType timeBasedAssistantType, PoisonMailboxControl poisonControl, PerformanceCountersPerDatabaseInstance databaseCounters) : base(parentGovernor, databaseInfo, timeBasedAssistantType, poisonControl, databaseCounters)
		{
		}

		public override void RunNow(Guid mailboxGuid, string parameters)
		{
			MailboxData mailboxData = base.Assistant.CreateOnDemandMailboxData(mailboxGuid, parameters);
			if (mailboxData == null)
			{
				return;
			}
			ExTraceGlobals.TimeBasedDatabaseDriverTracer.TraceDebug<TimeBasedAdminRpcDatabaseDriver, Guid>((long)this.GetHashCode(), "{0}: RunNow: about to start processing mailbox {1} on this database.", this, mailboxGuid);
			base.RunNow(mailboxData);
		}

		protected override List<MailboxData> GetMailboxesForCurrentWindow(out int totalMailboxOnDatabaseCount, out int notInterestingMailboxCount, out int filteredMailboxCount, out int failedFilteringCount)
		{
			List<MailboxData> list = base.Assistant.GetMailboxesToProcess();
			list = (list ?? new List<MailboxData>());
			totalMailboxOnDatabaseCount = list.Count;
			notInterestingMailboxCount = 0;
			filteredMailboxCount = 0;
			failedFilteringCount = 0;
			Guid activityId = Guid.NewGuid();
			foreach (MailboxData mailboxData in list)
			{
				AssistantsLog.LogMailboxInterestingEvent(activityId, base.Assistant.NonLocalizedName, base.Assistant as AssistantBase, null, mailboxData.MailboxGuid, mailboxData.DisplayName);
			}
			return list;
		}
	}
}
