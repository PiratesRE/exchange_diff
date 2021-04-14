using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class DatabaseWriteResource : DatabaseResource
	{
		private DatabaseWriteResource(Guid mdbGuid, WorkloadType workloadType) : base(mdbGuid, workloadType)
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
						base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationWriteInternalMaintenance;
						break;
					case WorkloadType.MailboxReplicationServiceInteractive:
						base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationWriteCustomerExpectation;
						break;
					}
				}
				else
				{
					base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationWriteHiPri;
				}
			}
			else
			{
				base.UtilizationPerfCounter = mdbhelper.PerfCounter.UtilizationWrite;
			}
			base.TransferRatePerfCounter = mdbhelper.WriteTransferRate;
		}

		public override int StaticCapacity
		{
			get
			{
				int config;
				using (base.ConfigContext.Activate())
				{
					config = ConfigBase<MRSConfigSchema>.GetConfig<int>("MaxActiveMovesPerTargetMDB");
				}
				return config;
			}
		}

		public override string ResourceType
		{
			get
			{
				return string.Format("MdbWrite{0}", base.WlmWorkloadTypeSuffix);
			}
		}

		public override List<WlmResourceHealthMonitor> GetWlmResources()
		{
			return new List<WlmResourceHealthMonitor>
			{
				new MDBLatencyHealthMonitor(this, base.ResourceGuid),
				new MDBReplicationHealthMonitor(this, base.ResourceGuid),
				new MDBAvailabilityHealthMonitor(this, base.ResourceGuid),
				new CIAgeOfLastNotificationHealthMonitor(this, base.ResourceGuid),
				new DiskLatencyHealthMonitor(this, base.ResourceGuid)
			};
		}

		public static readonly WlmResourceCache<DatabaseWriteResource> Cache = new WlmResourceCache<DatabaseWriteResource>((Guid id, WorkloadType wlt) => new DatabaseWriteResource(id, wlt));
	}
}
