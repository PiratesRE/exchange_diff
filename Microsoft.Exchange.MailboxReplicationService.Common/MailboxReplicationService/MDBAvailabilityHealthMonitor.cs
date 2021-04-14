using System;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MDBAvailabilityHealthMonitor : WlmResourceHealthMonitor
	{
		public MDBAvailabilityHealthMonitor(WlmResource owner, Guid mdbGuid) : base(owner, new MdbAvailabilityResourceHealthMonitorKey(mdbGuid))
		{
			MailboxReplicationServicePerMdbPerformanceCountersInstance perfCounter = MDBPerfCounterHelperCollection.GetMDBHelper(mdbGuid, true).PerfCounter;
			WorkloadType wlmWorkloadType = owner.WlmWorkloadType;
			if (wlmWorkloadType == WorkloadType.MailboxReplicationService)
			{
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBAvailability;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBAvailability;
				return;
			}
			if (wlmWorkloadType == WorkloadType.MailboxReplicationServiceHighPriority)
			{
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBAvailabilityHiPri;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBAvailabilityHiPri;
				return;
			}
			switch (wlmWorkloadType)
			{
			case WorkloadType.MailboxReplicationServiceInternalMaintenance:
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBAvailabilityInternalMaintenance;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBAvailabilityInternalMaintenance;
				return;
			case WorkloadType.MailboxReplicationServiceInteractive:
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBAvailabilityCustomerExpectation;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBAvailabilityCustomerExpectation;
				return;
			default:
				return;
			}
		}
	}
}
