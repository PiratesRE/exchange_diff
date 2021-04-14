using System;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class LocalCPUHealthMonitor : WlmResourceHealthMonitor
	{
		public LocalCPUHealthMonitor(WlmResource owner) : base(owner, ProcessorResourceKey.Local)
		{
			WorkloadType wlmWorkloadType = owner.WlmWorkloadType;
			if (wlmWorkloadType == WorkloadType.MailboxReplicationService)
			{
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUResourceHealth;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUDynamicCapacity;
				return;
			}
			if (wlmWorkloadType == WorkloadType.MailboxReplicationServiceHighPriority)
			{
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUResourceHealthHiPri;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUDynamicCapacityHiPri;
				return;
			}
			switch (wlmWorkloadType)
			{
			case WorkloadType.MailboxReplicationServiceInternalMaintenance:
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUResourceHealthInternalMaintenance;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUDynamicCapacityInternalMaintenance;
				return;
			case WorkloadType.MailboxReplicationServiceInteractive:
				base.ResourceHealthPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUResourceHealthCustomerExpectation;
				base.DynamicCapacityPerfCounter = MailboxReplicationServicePerformanceCounters.LocalCPUDynamicCapacityCustomerExpectation;
				return;
			default:
				return;
			}
		}
	}
}
