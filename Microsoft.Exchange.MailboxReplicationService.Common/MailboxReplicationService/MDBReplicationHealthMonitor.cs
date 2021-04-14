using System;
using Microsoft.Exchange.Data.Storage.ResourceHealth;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MDBReplicationHealthMonitor : WlmResourceHealthMonitor
	{
		public MDBReplicationHealthMonitor(WlmResource owner, Guid mdbGuid) : base(owner, new MdbReplicationResourceHealthMonitorKey(mdbGuid))
		{
			MailboxReplicationServicePerMdbPerformanceCountersInstance perfCounter = MDBPerfCounterHelperCollection.GetMDBHelper(mdbGuid, true).PerfCounter;
			WorkloadType wlmWorkloadType = owner.WlmWorkloadType;
			if (wlmWorkloadType == WorkloadType.MailboxReplicationService)
			{
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBReplication;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBReplication;
				return;
			}
			if (wlmWorkloadType == WorkloadType.MailboxReplicationServiceHighPriority)
			{
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBReplicationHiPri;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBReplicationHiPri;
				return;
			}
			switch (wlmWorkloadType)
			{
			case WorkloadType.MailboxReplicationServiceInternalMaintenance:
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBReplicationInternalMaintenance;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBReplicationInternalMaintenance;
				return;
			case WorkloadType.MailboxReplicationServiceInteractive:
				base.ResourceHealthPerfCounter = perfCounter.ResourceHealthMDBReplicationCustomerExpectation;
				base.DynamicCapacityPerfCounter = perfCounter.DynamicCapacityMDBReplicationCustomerExpectation;
				return;
			default:
				return;
			}
		}
	}
}
