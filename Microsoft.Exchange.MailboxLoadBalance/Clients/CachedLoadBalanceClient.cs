using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.Clients
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CachedLoadBalanceClient : CachedClient, ILoadBalanceService, IVersionedService, IDisposeTrackable, IDisposable
	{
		public CachedLoadBalanceClient(ILoadBalanceService client) : base(client as IWcfClient)
		{
			this.client = client;
		}

		public SoftDeleteMailboxRemovalCheckRemoval CheckSoftDeletedMailboxRemoval(SoftDeletedRemovalData data)
		{
			return this.client.CheckSoftDeletedMailboxRemoval(data);
		}

		public void CleanupSoftDeletedMailboxesOnDatabase(DirectoryIdentity identity, ByteQuantifiedSize targetSize)
		{
			this.client.CleanupSoftDeletedMailboxesOnDatabase(identity, targetSize);
		}

		void ILoadBalanceService.BeginMailboxMove(BandMailboxRebalanceData rebalanceData, LoadMetric metric)
		{
			this.client.BeginMailboxMove(rebalanceData, metric);
		}

		void IVersionedService.ExchangeVersionInformation(VersionInformation clientVersion, out VersionInformation serverVersion)
		{
			this.client.ExchangeVersionInformation(clientVersion, out serverVersion);
		}

		Band[] ILoadBalanceService.GetActiveBands()
		{
			return this.client.GetActiveBands();
		}

		public HeatMapCapacityData GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData)
		{
			return this.client.GetCapacitySummary(objectIdentity, refreshData);
		}

		DatabaseSizeInfo ILoadBalanceService.GetDatabaseSizeInformation(DirectoryIdentity database)
		{
			return this.client.GetDatabaseSizeInformation(database);
		}

		DatabaseSizeInfo ILoadBalanceService.GetDatabaseSizeInformation(DirectoryDatabase database)
		{
			return this.client.GetDatabaseSizeInformation(database);
		}

		LoadContainer ILoadBalanceService.GetLocalServerData(Band[] bands)
		{
			return this.client.GetLocalServerData(bands);
		}

		public BatchCapacityDatum GetConsumerBatchCapacity(int numberOfMailboxes, ByteQuantifiedSize expectedBatchSize)
		{
			return this.client.GetConsumerBatchCapacity(numberOfMailboxes, expectedBatchSize);
		}

		public MailboxProvisioningResult GetLocalDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			return this.client.GetLocalDatabaseForProvisioning(provisioningData);
		}

		public MailboxProvisioningResult GetDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			return this.client.GetDatabaseForProvisioning(provisioningData);
		}

		internal override void Cleanup()
		{
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CachedLoadBalanceClient>(this);
		}

		private readonly ILoadBalanceService client;
	}
}
