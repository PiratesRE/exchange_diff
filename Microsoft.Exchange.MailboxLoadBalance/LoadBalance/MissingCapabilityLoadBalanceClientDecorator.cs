using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MissingCapabilityLoadBalanceClientDecorator : DisposeTrackableBase, ILoadBalanceService, IVersionedService, IDisposeTrackable, IDisposable
	{
		protected MissingCapabilityLoadBalanceClientDecorator(ILoadBalanceService service, DirectoryServer targetServer)
		{
			this.TargetServer = targetServer;
			this.service = service;
		}

		protected DirectoryServer TargetServer { get; set; }

		public virtual void BeginMailboxMove(BandMailboxRebalanceData rebalanceData, LoadMetric metric)
		{
			this.service.BeginMailboxMove(rebalanceData, metric);
		}

		public virtual SoftDeleteMailboxRemovalCheckRemoval CheckSoftDeletedMailboxRemoval(SoftDeletedRemovalData data)
		{
			return this.service.CheckSoftDeletedMailboxRemoval(data);
		}

		public virtual void CleanupSoftDeletedMailboxesOnDatabase(DirectoryIdentity identity, ByteQuantifiedSize targetSize)
		{
			this.service.CleanupSoftDeletedMailboxesOnDatabase(identity, targetSize);
		}

		public virtual void ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			this.service.ExchangeVersionInformation(clientVersion, out serverVersion);
		}

		public virtual Band[] GetActiveBands()
		{
			return this.service.GetActiveBands();
		}

		public virtual HeatMapCapacityData GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData)
		{
			return this.service.GetCapacitySummary(objectIdentity, refreshData);
		}

		public virtual DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryDatabase database)
		{
			return this.service.GetDatabaseSizeInformation(database);
		}

		public virtual DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryIdentity database)
		{
			return this.service.GetDatabaseSizeInformation(database);
		}

		public virtual LoadContainer GetLocalServerData(Band[] bands)
		{
			return this.service.GetLocalServerData(bands);
		}

		public BatchCapacityDatum GetConsumerBatchCapacity(int numberOfMailboxes, ByteQuantifiedSize expectedBatchSize)
		{
			return new BatchCapacityDatum
			{
				MaximumNumberOfMailboxes = 0
			};
		}

		public MailboxProvisioningResult GetLocalDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			throw new NotSupportedException("GetLocalDatabaseForProvisioning is not cross version compat.");
		}

		public MailboxProvisioningResult GetDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			throw new NotSupportedException("GetDatabaseForProvisioning is not cross version compat.");
		}

		protected override void InternalDispose(bool disposing)
		{
			if (this.service != null)
			{
				this.service.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MissingCapabilityLoadBalanceClientDecorator>(this);
		}

		private readonly ILoadBalanceService service;
	}
}
