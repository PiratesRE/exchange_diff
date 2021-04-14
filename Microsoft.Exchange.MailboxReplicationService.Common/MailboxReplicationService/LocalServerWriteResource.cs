using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class LocalServerWriteResource : LocalServerResource
	{
		private LocalServerWriteResource(WorkloadType workloadType) : base(workloadType)
		{
			WorkloadType wlmWorkloadType = base.WlmWorkloadType;
			if (wlmWorkloadType != WorkloadType.MailboxReplicationService)
			{
				if (wlmWorkloadType != WorkloadType.MailboxReplicationServiceHighPriority)
				{
					switch (wlmWorkloadType)
					{
					case WorkloadType.MailboxReplicationServiceInternalMaintenance:
						base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationWriteInternalMaintenance;
						break;
					case WorkloadType.MailboxReplicationServiceInteractive:
						base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationWriteCustomerExpectation;
						break;
					}
				}
				else
				{
					base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationWriteHiPri;
				}
			}
			else
			{
				base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationWrite;
			}
			base.TransferRatePerfCounter = LocalServerWriteResource.WriteTransferRatePerfCounter;
		}

		public override int StaticCapacity
		{
			get
			{
				int config;
				using (base.ConfigContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxActiveMovesPerTargetServer");
				}
				return config;
			}
		}

		public override string ResourceType
		{
			get
			{
				return string.Format("ServerWrite{0}", base.WlmWorkloadTypeSuffix);
			}
		}

		public static readonly WlmResourceCache<LocalServerWriteResource> Cache = new WlmResourceCache<LocalServerWriteResource>((Guid id, WorkloadType wlt) => new LocalServerWriteResource(wlt));

		public static readonly PerfCounterWithAverageRate WriteTransferRatePerfCounter = new PerfCounterWithAverageRate(null, MailboxReplicationServicePerformanceCounters.WriteTransferRate, MailboxReplicationServicePerformanceCounters.WriteTransferRateBase, 1024, TimeSpan.FromSeconds(1.0));
	}
}
