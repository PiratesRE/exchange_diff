using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LoadBalancerClient : VersionedClientBase<ILoadBalanceService>, ILoadBalanceService, IVersionedService, IDisposeTrackable, IDisposable
	{
		private LoadBalancerClient(Binding binding, EndpointAddress remoteAddress, IDirectoryProvider directory, ILogger logger) : base(binding, remoteAddress, logger)
		{
			this.directory = directory;
		}

		public static LoadBalancerClient Create(string serverName, IDirectoryProvider directory, ILogger logger)
		{
			Func<Binding, EndpointAddress, ILogger, LoadBalancerClient> constructor = (Binding binding, EndpointAddress endpointAddress, ILogger l) => new LoadBalancerClient(binding, endpointAddress, directory, l);
			return VersionedClientBase<ILoadBalanceService>.CreateClient<LoadBalancerClient>(serverName, LoadBalanceService.EndpointAddress, constructor, logger);
		}

		public void BeginMailboxMove(BandMailboxRebalanceData rebalanceData, LoadMetric metric)
		{
			base.CallService(delegate()
			{
				this.Channel.BeginMailboxMove(rebalanceData, metric);
			});
		}

		public DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryIdentity database)
		{
			return base.CallService<DatabaseSizeInfo>(() => this.Channel.GetDatabaseSizeInformation(database));
		}

		public LoadContainer GetLocalServerData(Band[] bands)
		{
			return base.CallService<LoadContainer>(delegate()
			{
				LoadContainer localServerData = this.Channel.GetLocalServerData(bands);
				localServerData.Accept(new DirectoryReconnectionVisitor(this.directory, this.Logger));
				return localServerData;
			});
		}

		public HeatMapCapacityData GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData)
		{
			return base.CallService<HeatMapCapacityData>(() => this.Channel.GetCapacitySummary(objectIdentity, refreshData));
		}

		public BatchCapacityDatum GetConsumerBatchCapacity(int numberOfMailboxes, ByteQuantifiedSize expectedBatchSize)
		{
			return base.CallService<BatchCapacityDatum>(() => this.Channel.GetConsumerBatchCapacity(numberOfMailboxes, expectedBatchSize));
		}

		public MailboxProvisioningResult GetLocalDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			return base.CallService<MailboxProvisioningResult>(() => this.Channel.GetLocalDatabaseForProvisioning(provisioningData));
		}

		public MailboxProvisioningResult GetDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			return base.CallService<MailboxProvisioningResult>(() => this.Channel.GetDatabaseForProvisioning(provisioningData));
		}

		public DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryDatabase database)
		{
			return base.CallService<DatabaseSizeInfo>(() => this.Channel.GetDatabaseSizeInformation(database));
		}

		public SoftDeleteMailboxRemovalCheckRemoval CheckSoftDeletedMailboxRemoval(SoftDeletedRemovalData data)
		{
			return base.CallService<SoftDeleteMailboxRemovalCheckRemoval>(() => this.Channel.CheckSoftDeletedMailboxRemoval(data));
		}

		public Band[] GetActiveBands()
		{
			return base.CallService<Band[]>(() => base.Channel.GetActiveBands());
		}

		public void CleanupSoftDeletedMailboxesOnDatabase(DirectoryIdentity identity, ByteQuantifiedSize targetSize)
		{
			base.CallService(delegate()
			{
				this.Channel.CleanupSoftDeletedMailboxesOnDatabase(identity, targetSize);
			});
		}

		private readonly IDirectoryProvider directory;
	}
}
