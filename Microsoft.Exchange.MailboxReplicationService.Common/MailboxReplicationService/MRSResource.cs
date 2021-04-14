using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class MRSResource : LocalServerResource
	{
		private MRSResource(WorkloadType workloadType) : base(workloadType)
		{
			WorkloadType wlmWorkloadType = base.WlmWorkloadType;
			if (wlmWorkloadType != WorkloadType.MailboxReplicationService)
			{
				if (wlmWorkloadType != WorkloadType.MailboxReplicationServiceHighPriority)
				{
					switch (wlmWorkloadType)
					{
					case WorkloadType.MailboxReplicationServiceInternalMaintenance:
						base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationMRSInternalMaintenance;
						break;
					case WorkloadType.MailboxReplicationServiceInteractive:
						base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationMRSCustomerExpectation;
						break;
					}
				}
				else
				{
					base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationMRSHiPri;
				}
			}
			else
			{
				base.UtilizationPerfCounter = MailboxReplicationServicePerformanceCounters.UtilizationMRS;
			}
			base.TransferRatePerfCounter = MRSResource.MRSTransferRatePerfCounter;
		}

		public override int StaticCapacity
		{
			get
			{
				int config;
				using (base.ConfigContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxTotalRequestsPerMRS");
				}
				return config;
			}
		}

		public override string ResourceType
		{
			get
			{
				return string.Format("MRS{0}", base.WlmWorkloadTypeSuffix);
			}
		}

		public static readonly WlmResourceCache<MRSResource> Cache = new WlmResourceCache<MRSResource>((Guid id, WorkloadType wlt) => new MRSResource(wlt));

		public static readonly ADObjectId Id = new ADObjectId(new Guid("6647EA79-5A87-400D-8659-E1181164044F"));

		public static readonly PerfCounterWithAverageRate MRSTransferRatePerfCounter = new PerfCounterWithAverageRate(null, MailboxReplicationServicePerformanceCounters.MRSTransferRate, MailboxReplicationServicePerformanceCounters.MRSTransferRateBase, 1024, TimeSpan.FromSeconds(1.0));
	}
}
