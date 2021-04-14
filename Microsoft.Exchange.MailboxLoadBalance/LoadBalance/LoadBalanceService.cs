using System;
using System.Linq;
using System.ServiceModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalance.LoadBalance
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.Single)]
	internal class LoadBalanceService : VersionedServiceBase, ILoadBalanceService, IVersionedService, IDisposeTrackable, IDisposable
	{
		public LoadBalanceService(MailboxLoadBalanceService service, LoadBalanceAnchorContext serviceContext) : base(serviceContext.Logger)
		{
			if (service == null)
			{
				throw new ArgumentNullException("service");
			}
			this.serviceImpl = service;
			this.serviceContext = serviceContext;
		}

		public static ServiceEndpointAddress EndpointAddress
		{
			get
			{
				return LoadBalanceService.EndpointAddressHook.Value;
			}
		}

		protected override VersionInformation ServiceVersion
		{
			get
			{
				return LoadBalancerVersionInformation.LoadBalancerVersion;
			}
		}

		public void BeginMailboxMove(BandMailboxRebalanceData rebalanceData, LoadMetric metric)
		{
			base.ForwardExceptions(delegate()
			{
				rebalanceData.ConvertToFromSerializationFormat();
				this.serviceImpl.MoveMailboxes(rebalanceData);
			});
		}

		public SoftDeleteMailboxRemovalCheckRemoval CheckSoftDeletedMailboxRemoval(SoftDeletedRemovalData data)
		{
			return base.ForwardExceptions<SoftDeleteMailboxRemovalCheckRemoval>(() => this.serviceContext.CheckSoftDeletedMailboxRemoval(data));
		}

		public void CleanupSoftDeletedMailboxesOnDatabase(DirectoryIdentity databaseIdentity, ByteQuantifiedSize targetSize)
		{
			base.ForwardExceptions(delegate()
			{
				this.serviceContext.CreateSoftDeletedDatabaseCleanupRequests(databaseIdentity, targetSize);
			});
		}

		public Band[] GetActiveBands()
		{
			return base.ForwardExceptions<Band[]>(new Func<Band[]>(this.GetLocalBandDefinition));
		}

		public HeatMapCapacityData GetCapacitySummary(DirectoryIdentity objectIdentity, bool refreshData)
		{
			return base.ForwardExceptions<HeatMapCapacityData>(() => this.GetCapacityDatum(objectIdentity, refreshData));
		}

		public BatchCapacityDatum GetConsumerBatchCapacity(int numberOfMailboxes, ByteQuantifiedSize expectedBatchSize)
		{
			return base.ForwardExceptions<BatchCapacityDatum>(delegate()
			{
				if (numberOfMailboxes <= 0)
				{
					throw new ArgumentOutOfRangeException("numberOfMailboxes", numberOfMailboxes, "Number of mailbox must be greater than zero.");
				}
				BatchCapacityProjection batchCapacityProjection = new BatchCapacityProjection(numberOfMailboxes);
				ICapacityProjection capacityProjection = this.serviceContext.GetCapacityProjection(expectedBatchSize / numberOfMailboxes);
				return new MinimumCapacityProjection(this.Logger, new ICapacityProjection[]
				{
					batchCapacityProjection,
					capacityProjection
				}).GetCapacity();
			});
		}

		public MailboxProvisioningResult GetDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			return base.ForwardExceptions<MailboxProvisioningResult>(() => this.serviceContext.DatabaseSelector.GetDatabase(provisioningData));
		}

		public DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryDatabase database)
		{
			return base.ForwardExceptions<DatabaseSizeInfo>(() => this.GetDatabaseSize(database));
		}

		public DatabaseSizeInfo GetDatabaseSizeInformation(DirectoryIdentity database)
		{
			return base.ForwardExceptions<DatabaseSizeInfo>(() => this.GetDatabaseSize((DirectoryDatabase)this.serviceContext.Directory.GetDirectoryObject(database)));
		}

		public MailboxProvisioningResult GetLocalDatabaseForProvisioning(MailboxProvisioningData provisioningData)
		{
			return base.ForwardExceptions<MailboxProvisioningResult>(() => this.serviceContext.LocalServerDatabaseSelector.GetDatabase(provisioningData));
		}

		public LoadContainer GetLocalServerData(Band[] bands)
		{
			return base.ForwardExceptions<LoadContainer>(delegate()
			{
				this.serviceContext.LocalServerHeatMap.UpdateBands(bands);
				LoadContainer loadTopology = this.serviceContext.LocalServerHeatMap.GetLoadTopology();
				bool convertBandToBandData = !this.ClientVersion[3];
				return (LoadContainer)loadTopology.ToSerializationFormat(convertBandToBandData);
			});
		}

		protected internal static IDisposable SetEndpointAddress(ServiceEndpointAddress endpointAddress)
		{
			return LoadBalanceService.EndpointAddressHook.SetTestHook(endpointAddress);
		}

		private HeatMapCapacityData GetCapacityDatum(DirectoryIdentity objectIdentity, bool refreshData)
		{
			DirectoryIdentity identity = this.serviceContext.Directory.GetLocalServer().Identity;
			if (objectIdentity.ObjectType == DirectoryObjectType.Server)
			{
				return this.GetServerCapacityDatum(objectIdentity, refreshData, identity);
			}
			if (objectIdentity.ObjectType == DirectoryObjectType.Forest)
			{
				return this.GetLocalForestCapacityDatum();
			}
			if (objectIdentity.ObjectType == DirectoryObjectType.DatabaseAvailabilityGroup)
			{
				return this.GetDagCapacityDatum(objectIdentity);
			}
			if (objectIdentity.ObjectType != DirectoryObjectType.Database)
			{
				throw new CannotRetrieveCapacityDataException(objectIdentity.ToString());
			}
			return this.GetDatabaseCapacityDatum(objectIdentity, refreshData, identity);
		}

		private HeatMapCapacityData GetDagCapacityDatum(DirectoryIdentity objectIdentity)
		{
			TopologyExtractorFactoryContext topologyExtractorFactoryContext = this.serviceContext.GetTopologyExtractorFactoryContext();
			DirectoryDatabaseAvailabilityGroup directoryObject = (DirectoryDatabaseAvailabilityGroup)this.serviceContext.Directory.GetDirectoryObject(objectIdentity);
			TopologyExtractorFactory loadBalancingCentralFactory = topologyExtractorFactoryContext.GetLoadBalancingCentralFactory();
			LoadContainer loadContainer = loadBalancingCentralFactory.GetExtractor(directoryObject).ExtractTopology();
			return loadContainer.ToCapacityData();
		}

		private HeatMapCapacityData GetDatabaseCapacityDatum(DirectoryIdentity objectIdentity, bool refreshData, DirectoryIdentity localServerIdentity)
		{
			DirectoryDatabase directoryDatabase = (DirectoryDatabase)this.serviceContext.Directory.GetDirectoryObject(objectIdentity);
			DirectoryServer directoryServer = directoryDatabase.ActivationOrder.FirstOrDefault<DirectoryServer>();
			if (directoryServer == null || localServerIdentity.Equals(directoryServer.Identity))
			{
				TopologyExtractorFactoryContext topologyExtractorFactoryContext = this.serviceContext.GetTopologyExtractorFactoryContext();
				TopologyExtractorFactory loadBalancingLocalFactory = topologyExtractorFactoryContext.GetLoadBalancingLocalFactory(refreshData);
				LoadContainer loadContainer = loadBalancingLocalFactory.GetExtractor(directoryDatabase).ExtractTopology();
				return loadContainer.ToCapacityData();
			}
			HeatMapCapacityData capacitySummary;
			using (ILoadBalanceService loadBalanceClientForDatabase = this.serviceContext.ClientFactory.GetLoadBalanceClientForDatabase(directoryDatabase))
			{
				capacitySummary = loadBalanceClientForDatabase.GetCapacitySummary(objectIdentity, refreshData);
			}
			return capacitySummary;
		}

		private DatabaseSizeInfo GetDatabaseSize(DirectoryDatabase database)
		{
			DatabaseSizeInfo databaseSpaceData;
			using (IPhysicalDatabase physicalDatabaseConnection = this.serviceContext.ClientFactory.GetPhysicalDatabaseConnection(database))
			{
				databaseSpaceData = physicalDatabaseConnection.GetDatabaseSpaceData();
			}
			return databaseSpaceData;
		}

		private Band[] GetLocalBandDefinition()
		{
			Band[] result;
			using (IBandSettingsProvider bandSettingsProvider = this.serviceContext.CreateBandSettingsStorage())
			{
				result = bandSettingsProvider.GetBandSettings().ToArray<Band>();
			}
			return result;
		}

		private HeatMapCapacityData GetLocalForestCapacityDatum()
		{
			return this.serviceContext.HeatMap.ToCapacityData();
		}

		private HeatMapCapacityData GetServerCapacityDatum(DirectoryIdentity objectIdentity, bool refreshData, DirectoryIdentity localServerIdentity)
		{
			if (objectIdentity.Equals(localServerIdentity))
			{
				return this.serviceContext.LocalServerHeatMap.ToCapacityData();
			}
			DirectoryServer server = (DirectoryServer)this.serviceContext.Directory.GetDirectoryObject(objectIdentity);
			HeatMapCapacityData capacitySummary;
			using (ILoadBalanceService loadBalanceClientForServer = this.serviceContext.ClientFactory.GetLoadBalanceClientForServer(server, false))
			{
				capacitySummary = loadBalanceClientForServer.GetCapacitySummary(objectIdentity, refreshData);
			}
			return capacitySummary;
		}

		private const string EndpointSuffix = "Microsoft.Exchange.MailboxLoadBalance.LoadBalanceService";

		private static readonly Hookable<ServiceEndpointAddress> EndpointAddressHook = Hookable<ServiceEndpointAddress>.Create(true, new ServiceEndpointAddress("Microsoft.Exchange.MailboxLoadBalance.LoadBalanceService"));

		private readonly LoadBalanceAnchorContext serviceContext;

		private readonly MailboxLoadBalanceService serviceImpl;
	}
}
