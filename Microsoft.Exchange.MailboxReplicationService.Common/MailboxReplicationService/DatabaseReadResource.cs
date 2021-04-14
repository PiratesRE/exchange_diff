using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DatabaseReadResource : DatabaseResource
	{
		private DatabaseReadResource(Guid mdbGuid, WorkloadType workloadType) : base(mdbGuid, workloadType)
		{
			MDBPerfCounterHelper mdbhelper = MDBPerfCounterHelperCollection.GetMDBHelper(mdbGuid, true);
			WorkloadType wlmWorkloadType = base.WlmWorkloadType;
			if (wlmWorkloadType != WorkloadType.MailboxReplicationService)
			{
				if (wlmWorkloadType != WorkloadType.MailboxReplicationServiceHighPriority)
				{
					switch (wlmWorkloadType)
					{
					case WorkloadType.MailboxReplicationServiceInternalMaintenance:
						base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationReadInternalMaintenance;
						break;
					case WorkloadType.MailboxReplicationServiceInteractive:
						base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationReadCustomerExpectation;
						break;
					}
				}
				else
				{
					base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationReadHiPri;
				}
			}
			else
			{
				base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationRead;
			}
			base.TransferRatePerfCounter = mdbhelper.ReadTransferRate;
		}

		public override int StaticCapacity
		{
			get
			{
				int config;
				using (base.ConfigContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxActiveMovesPerSourceMDB");
				}
				return config;
			}
		}

		public override string ResourceType
		{
			get
			{
				return string.Format("MdbRead{0}", base.WlmWorkloadTypeSuffix);
			}
		}

		public override List<WlmResourceHealthMonitor> GetWlmResources()
		{
			return new List<WlmResourceHealthMonitor>
			{
				new MDBLatencyHealthMonitor(this, base.ResourceGuid),
				new DiskLatencyHealthMonitor(this, base.ResourceGuid)
			};
		}

		public static readonly WlmResourceCache<DatabaseReadResource> Cache = new WlmResourceCache<DatabaseReadResource>((Guid id, WorkloadType wlt) => new DatabaseReadResource(id, wlt));
	}
}
