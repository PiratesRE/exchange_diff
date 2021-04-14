using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class CIAgeOfLastNotificationHealthMonitor : WlmResourceHealthMonitor
	{
		public CIAgeOfLastNotificationHealthMonitor(WlmResource owner, Guid mdbGuid) : base(owner, new CiAgeOfLastNotificationResourceKey(mdbGuid))
		{
			MailboxReplicationServicePerMdbPerformanceCountersInstance perfCounter = MDBPerfCounterHelperCollection.GetMDBHelper(mdbGuid, true).PerfCounter;
			WorkloadType wlmWorkloadType = owner.WlmWorkloadType;
			if (wlmWorkloadType == WorkloadType.MailboxReplicationService)
			{
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthCIAgeOfLastNotification;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityCIAgeOfLastNotification;
				return;
			}
			if (wlmWorkloadType == WorkloadType.MailboxReplicationServiceHighPriority)
			{
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthCIAgeOfLastNotificationHiPri;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityCIAgeOfLastNotificationHiPri;
				return;
			}
			switch (wlmWorkloadType)
			{
			case WorkloadType.MailboxReplicationServiceInternalMaintenance:
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthCIAgeOfLastNotificationInternalMaintenance;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityCIAgeOfLastNotificationInternalMaintenance;
				return;
			case WorkloadType.MailboxReplicationServiceInteractive:
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthCIAgeOfLastNotificationCustomerExpectation;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityCIAgeOfLastNotificationCustomerExpectation;
				return;
			default:
				return;
			}
		}
	}
}
