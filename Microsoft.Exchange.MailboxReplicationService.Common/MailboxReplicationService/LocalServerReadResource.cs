using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class LocalServerReadResource : LocalServerResource
	{
		private LocalServerReadResource(WorkloadType workloadType) : base(workloadType)
		{
			WorkloadType wlmWorkloadType = base.WlmWorkloadType;
			if (wlmWorkloadType != WorkloadType.MailboxReplicationService)
			{
				if (wlmWorkloadType != WorkloadType.MailboxReplicationServiceHighPriority)
				{
					switch (wlmWorkloadType)
					{
					case WorkloadType.MailboxReplicationServiceInternalMaintenance:
						base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationReadInternalMaintenance;
						break;
					case WorkloadType.MailboxReplicationServiceInteractive:
						base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationReadCustomerExpectation;
						break;
					}
				}
				else
				{
					base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationReadHiPri;
				}
			}
			else
			{
				base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationRead;
			}
			base.TransferRatePerfCounter = LocalServerReadResource.ReadTransferRatePerfCounter;
		}

		public override int StaticCapacity
		{
			get
			{
				int config;
				using (base.ConfigContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxActiveMovesPerSourceServer");
				}
				return config;
			}
		}

		public override string ResourceType
		{
			get
			{
				return string.Format("ServerRead{0}", base.WlmWorkloadTypeSuffix);
			}
		}

		public static readonly WlmResourceCache<LocalServerReadResource> Cache = new WlmResourceCache<LocalServerReadResource>((Guid id, WorkloadType wlt) => new LocalServerReadResource(wlt));

		public static readonly PerfCounterWithAverageRate ReadTransferRatePerfCounter = new PerfCounterWithAverageRate(null, MailboxReplicationServicePerformanceCounters.ReadTransferRate, MailboxReplicationServicePerformanceCounters.ReadTransferRateBase, 1024, TimeSpan.FromSeconds(1.0));
	}
}
