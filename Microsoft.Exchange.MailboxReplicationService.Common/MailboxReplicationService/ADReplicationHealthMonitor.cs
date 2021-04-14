using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class ADReplicationHealthMonitor : WlmResourceHealthMonitor
	{
		public ADReplicationHealthMonitor(WlmResource owner) : base(owner, ADResourceKey.Key)
		{
			WorkloadType wlmWorkloadType = owner.WlmWorkloadType;
			if (wlmWorkloadType == WorkloadType.MailboxReplicationService)
			{
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationResourceHealth;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationDynamicCapacity;
				return;
			}
			if (wlmWorkloadType == WorkloadType.MailboxReplicationServiceHighPriority)
			{
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationResourceHealthHiPri;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationDynamicCapacityHiPri;
				return;
			}
			switch (wlmWorkloadType)
			{
			case WorkloadType.MailboxReplicationServiceInternalMaintenance:
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationResourceHealthInternalMaintenance;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationDynamicCapacityInternalMaintenance;
				return;
			case WorkloadType.MailboxReplicationServiceInteractive:
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationResourceHealthCustomerExpectation;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.ADReplicationDynamicCapacityCustomerExpectation;
				return;
			default:
				return;
			}
		}
	}
}
