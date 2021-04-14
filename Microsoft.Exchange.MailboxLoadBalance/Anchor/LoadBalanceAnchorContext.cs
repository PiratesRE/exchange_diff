using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.AnchorService.Storage;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MigrationWorkflowService;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Clients;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalance.Drain;
using Microsoft.Exchange.MailboxLoadBalance.LoadBalance;
using Microsoft.Exchange.MailboxLoadBalance.Logging;
using Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors;
using Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors.Policies;
using Microsoft.Exchange.MailboxLoadBalance.Providers;
using Microsoft.Exchange.MailboxLoadBalance.Provisioning;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing;
using Microsoft.Exchange.MailboxLoadBalance.QueueProcessing.Rubs;
using Microsoft.Exchange.MailboxLoadBalance.ServiceSupport;
using Microsoft.Exchange.MailboxLoadBalance.SoftDeletedRemoval;
using Microsoft.Exchange.MailboxLoadBalance.TopologyExtractors;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.MailboxLoadBalance.Anchor
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class LoadBalanceAnchorContext : AnchorContext
	{
		public LoadBalanceAnchorContext() : base("MailboxLoadBalance", OrganizationCapability.Management, LoadBalanceADSettings.DefaultContext)
		{
			base.Config.UpdateConfig<TimeSpan>("IdleRunDelay", TimeSpan.FromHours(12.0));
			base.Config.UpdateConfig<TimeSpan>("ActiveRunDelay", TimeSpan.FromHours(1.0));
			this.directory = new Lazy<IDirectoryProvider>(new Func<IDirectoryProvider>(this.CreateDirectoryInstance));
			this.clientFactory = new Lazy<IClientFactory>(new Func<IClientFactory>(this.CreateClientFactoryInstance));
			this.requestQueueManager = new Lazy<IRequestQueueManager>(new Func<IRequestQueueManager>(this.CreateQueueManagerInstance));
			this.service = new Lazy<MailboxLoadBalanceService>(new Func<MailboxLoadBalanceService>(this.CreateServiceInstance));
			this.extractorFactoryContextPool = new Lazy<TopologyExtractorFactoryContextPool>(new Func<TopologyExtractorFactoryContextPool>(this.CreateExtractorFactoryPool));
			this.moveInjector = new Lazy<MoveInjector>(new Func<MoveInjector>(this.CreateMoveInjector));
			this.databaseSelector = new Lazy<DatabaseSelector>(new Func<DatabaseSelector>(this.CreateDatabaseSelector));
			this.logCollector = new Lazy<ObjectLogCollector>(new Func<ObjectLogCollector>(this.CreateLogCollector));
			this.heatMap = new Lazy<IHeatMap>(new Func<IHeatMap>(this.CreateForestHeatMap));
			this.provisioningHeatMap = new Lazy<IHeatMap>(new Func<IHeatMap>(this.CreateProvisioningHeatMap));
			this.localProvisioningHeatMap = new Lazy<IHeatMap>(new Func<IHeatMap>(this.CreateLocalProvisioningHeatMap));
			this.localServerDatabaseSelector = new Lazy<DatabaseSelector>(new Func<DatabaseSelector>(this.CreateLocalServerDatabaseSelector));
			this.CmdletPool = this.CreateCmdletPool();
			this.StorePort = new StoreAdapter(base.Logger);
			this.DrainControl = new DatabaseDrainControl(this);
		}

		public override OrganizationCapability ActiveCapability
		{
			get
			{
				return OrganizationCapability.Management;
			}
		}

		public virtual DirectoryServer CentralServer
		{
			get
			{
				IAnchorADProvider anchorADProvider = this.CreateAnchorActiveDirectoryProvider();
				ADUser[] array = anchorADProvider.GetOrganizationMailboxesByCapability(base.AnchorCapability).ToArray<ADUser>();
				if (array.Length == 0)
				{
					throw new LoadBalanceAnchorMailboxNotFoundException(base.AnchorCapability.ToString());
				}
				ADUser aduser = (from user in array
				orderby user.Guid
				select user).First<ADUser>();
				string databaseServerFqdn = anchorADProvider.GetDatabaseServerFqdn(aduser.Database.ObjectGuid, false);
				return this.Directory.GetServerByFqdn(new Fqdn(databaseServerFqdn));
			}
		}

		public IClientFactory ClientFactory
		{
			get
			{
				return this.clientFactory.Value;
			}
		}

		public CmdletExecutionPool CmdletPool { get; private set; }

		public virtual DatabaseSelector DatabaseSelector
		{
			get
			{
				return this.databaseSelector.Value;
			}
		}

		public IDirectoryProvider Directory
		{
			get
			{
				return this.directory.Value;
			}
		}

		public DatabaseDrainControl DrainControl { get; private set; }

		public virtual IHeatMap HeatMap
		{
			get
			{
				return this.heatMap.Value;
			}
		}

		public virtual DatabaseSelector LocalServerDatabaseSelector
		{
			get
			{
				return this.localServerDatabaseSelector.Value;
			}
		}

		public virtual IHeatMap LocalServerHeatMap
		{
			get
			{
				return this.localProvisioningHeatMap.Value;
			}
		}

		public ObjectLogCollector LogCollector
		{
			get
			{
				return this.logCollector.Value;
			}
		}

		public MoveInjector MoveInjector
		{
			get
			{
				return this.moveInjector.Value;
			}
		}

		public virtual IHeatMap ProvisioningHeatMap
		{
			get
			{
				return this.provisioningHeatMap.Value;
			}
		}

		public IRequestQueueManager QueueManager
		{
			get
			{
				return this.requestQueueManager.Value;
			}
		}

		public MailboxLoadBalanceService Service
		{
			get
			{
				return this.service.Value;
			}
		}

		public virtual ILoadBalanceSettings Settings
		{
			get
			{
				return (ILoadBalanceSettings)base.Config;
			}
		}

		public virtual IStorePort StorePort { get; private set; }

		public TopologyExtractorFactoryContextPool TopologyExtractorFactoryContextPool
		{
			get
			{
				return this.extractorFactoryContextPool.Value;
			}
		}

		public SoftDeleteMailboxRemovalCheckRemoval CheckSoftDeletedMailboxRemoval(SoftDeletedRemovalData data)
		{
			if (!this.Settings.SoftDeletedCleanupEnabled)
			{
				return SoftDeleteMailboxRemovalCheckRemoval.DisallowRemoval("SoftDeletedRemoval is disabled on the target database '{0}', so no removal check can be performed.", new object[]
				{
					data.TargetDatabase.Name
				});
			}
			DateTime removalCutoffDate = DateTime.UtcNow.Add(TimeSpan.Zero - this.Settings.MinimumSoftDeletedMailboxCleanupAge);
			SoftDeletedMailboxRemovalCheck softDeletedMailboxRemovalCheck = new DisconnectDateCheck(data, this.Directory, removalCutoffDate);
			SoftDeletedMailboxRemovalCheck softDeletedMailboxRemovalCheck2 = new ItemCountCheck(data, this.Directory);
			SoftDeletedMailboxRemovalCheck next = new MoveHistoryCheck(data, this);
			softDeletedMailboxRemovalCheck.SetNext(softDeletedMailboxRemovalCheck2);
			softDeletedMailboxRemovalCheck2.SetNext(next);
			return softDeletedMailboxRemovalCheck.GetRemovalResult();
		}

		public virtual void CleanupSoftDeletedMailboxesOnDatabase(DirectoryIdentity databaseIdentity, ByteQuantifiedSize targetSize)
		{
			SoftDeletedMailboxRemover softDeletedMailboxRemover = new SoftDeletedMailboxRemover((DirectoryDatabase)this.Directory.GetDirectoryObject(databaseIdentity), this, targetSize, this.LogCollector);
			softDeletedMailboxRemover.RemoveFromDatabase();
		}

		public virtual IBandSettingsProvider CreateBandSettingsStorage()
		{
			return new BandSettingsStorage(this.CreateLoadBalancingAnchorDataProvider(), this);
		}

		public override CacheProcessorBase[] CreateCacheComponents(WaitHandle stopEvent)
		{
			return new CacheProcessorBase[]
			{
				new FirstOrgCacheScanner(this, stopEvent),
				new AutomaticLoadBalanceCacheComponent(this, stopEvent)
			};
		}

		public virtual ILoadBalance CreateLoadBalancer(ILogger logger)
		{
			if (this.Settings.LoadBalanceBlocked)
			{
				throw new AutomaticMailboxLoadBalancingNotAllowedException();
			}
			ILoadBalance result;
			using (IBandSettingsProvider bandSettingsProvider = this.CreateBandSettingsStorage())
			{
				ILoadBalance loadBalance = new BandBasedLoadBalance(bandSettingsProvider.GetBandSettings().ToList<Band>(), logger, this.Settings);
				result = loadBalance;
			}
			return result;
		}

		public virtual IAnchorDataProvider CreateLoadBalancingAnchorDataProvider()
		{
			return AnchorDataProvider.CreateProviderForMigrationMailboxFolder(this, (AnchorADProvider)this.CreateAnchorActiveDirectoryProvider(), "MailboxLoadBalance");
		}

		public void CreateSoftDeletedDatabaseCleanupRequests(DirectoryIdentity databaseIdentity, ByteQuantifiedSize targetSize)
		{
			LocalDatabaseSoftDeletedCleanupRequest request = new LocalDatabaseSoftDeletedCleanupRequest(databaseIdentity, targetSize, this);
			this.QueueManager.GetProcessingQueue(this.Directory.GetDirectoryObject(databaseIdentity)).EnqueueRequest(request);
		}

		public Band[] GetActiveBands()
		{
			Band[] activeBands;
			using (ILoadBalanceService loadBalanceClientForCentralServer = this.ClientFactory.GetLoadBalanceClientForCentralServer())
			{
				activeBands = loadBalanceClientForCentralServer.GetActiveBands();
			}
			return activeBands;
		}

		public ICapacityProjection GetCapacityProjection(ByteQuantifiedSize averageMailboxSize)
		{
			HeatMapCapacityData heatMapData = this.HeatMap.ToCapacityData();
			CapacityProjectionData capacityProjectionData = CapacityProjectionData.FromSettings(this.Settings);
			AvailableCapacityProjection availableCapacityProjection = new AvailableCapacityProjection(heatMapData, capacityProjectionData, this.Settings.QueryBufferPeriodDays, averageMailboxSize, base.Logger);
			ConsumerSizeProjection consumerSizeProjection = new ConsumerSizeProjection(heatMapData, capacityProjectionData, averageMailboxSize, this.Settings.QueryBufferPeriodDays, (double)this.Settings.MaximumConsumerMailboxSizePercent / 100.0, base.Logger);
			return new MinimumCapacityProjection(base.Logger, new ICapacityProjection[]
			{
				consumerSizeProjection,
				availableCapacityProjection
			});
		}

		public virtual TopologyExtractorFactoryContext GetTopologyExtractorFactoryContext()
		{
			return this.TopologyExtractorFactoryContextPool.GetContext(this.ClientFactory, this.GetActiveBands(), LoadBalanceUtils.GetNonMovableOrgsList(this.Settings), base.Logger);
		}

		public void InitializeForestHeatMap()
		{
			base.Logger.LogVerbose("Using {0} as the heat map.", new object[]
			{
				this.HeatMap
			});
			base.Logger.LogVerbose("Using {0} as the provisioning heat map.", new object[]
			{
				this.ProvisioningHeatMap
			});
		}

		public void InitializeLocalServerHeatMap()
		{
			base.Logger.LogVerbose("Using {0} as the heat map.", new object[]
			{
				this.LocalServerHeatMap
			});
		}

		public virtual SoftDeletedMoveHistory RetrieveSoftDeletedMailboxMoveHistory(Guid mailboxGuid, Guid targetDatabaseGuid, Guid sourceDatabaseGuid)
		{
			return SoftDeletedMoveHistory.GetHistoryForSourceDatabase(mailboxGuid, targetDatabaseGuid, sourceDatabaseGuid);
		}

		public virtual bool TryRemoveSoftDeletedMailbox(Guid mailboxGuid, Guid databaseGuid, out Exception exception)
		{
			SoftDeletedMailboxRemovalRequest softDeletedMailboxRemovalRequest = new SoftDeletedMailboxRemovalRequest(mailboxGuid, databaseGuid, base.Logger, this.CmdletPool, this.Settings);
			this.QueueManager.MainProcessingQueue.EnqueueRequest(softDeletedMailboxRemovalRequest);
			bool flag = softDeletedMailboxRemovalRequest.WaitExecution(TimeSpan.FromMinutes(3.0));
			exception = softDeletedMailboxRemovalRequest.Exception;
			return flag && softDeletedMailboxRemovalRequest.Exception == null;
		}

		protected virtual IAnchorADProvider CreateAnchorActiveDirectoryProvider()
		{
			return new AnchorADProvider(this, OrganizationId.ForestWideOrgId, null);
		}

		protected virtual IClientFactory CreateClientFactoryInstance()
		{
			ClientFactory result = new ClientFactory(base.Logger, this);
			if (this.Settings.ClientCacheTimeToLive == TimeSpan.Zero)
			{
				return result;
			}
			return new CachingClientFactory(this.Settings.ClientCacheTimeToLive, result, base.Logger);
		}

		protected virtual DatabaseSelector CreateDatabaseSelector()
		{
			if (this.Settings.UseHeatMapProvisioning)
			{
				return new HeatMapDatabaseSelector(this.ProvisioningHeatMap, base.Logger);
			}
			return new ProvisioningLayerDatabaseSelector(this.Directory, base.Logger);
		}

		protected virtual IDirectoryProvider CreateDirectoryInstance()
		{
			return new DirectoryProvider(this.ClientFactory, LocalServer.GetServer(), this.Settings, this.GetDirectoryListeners(), base.Logger, this);
		}

		protected virtual TopologyExtractorFactoryContextPool CreateExtractorFactoryPool()
		{
			return new TopologyExtractorFactoryContextPool();
		}

		protected IHeatMap CreateForestHeatMap()
		{
			if (this.Settings.UseHeatMapProvisioning)
			{
				return new CachedHeatMap(this, new ForestHeatMapConstructionRequest(this));
			}
			return new ForestHeatMap(this);
		}

		protected virtual ObjectLogCollector CreateLogCollector()
		{
			return new ObjectLogCollector();
		}

		protected override ILogger CreateLogger(string applicationName, AnchorConfig config)
		{
			return new AnchorLogger(applicationName, config, ExTraceGlobals.MailboxLoadBalanceTracer, new ExEventLog(new Guid("2822A8AF-B86C-4A21-B2D2-78E381039C3D"), "MSExchange Mailbox Load Balance"));
		}

		protected virtual MoveInjector CreateMoveInjector()
		{
			return new MoveInjector(this);
		}

		protected IHeatMap CreateProvisioningHeatMap()
		{
			return new ChainedHeatMap(new IHeatMap[]
			{
				this.HeatMap,
				this.LocalServerHeatMap
			});
		}

		protected virtual IRequestQueueManager CreateQueueManagerInstance()
		{
			if (this.Settings.DisableWlm)
			{
				return new RequestQueueManager();
			}
			return this.CreateRubsQueue();
		}

		protected virtual MailboxLoadBalanceService CreateServiceInstance()
		{
			return new MailboxLoadBalanceService(this);
		}

		protected virtual IMailboxPolicy[] GetMailboxPolicies(IEventNotificationSender eventNotificationSender)
		{
			return new IMailboxPolicy[]
			{
				new PolicyActivationControl(new MailboxProvisioningConstraintPolicy(eventNotificationSender), this.Settings),
				new PolicyActivationControl(new ZeroItemsPendingUpgradePolicy(this.Settings), this.Settings)
			};
		}

		private CmdletExecutionPool CreateCmdletPool()
		{
			return new CmdletExecutionPool(this);
		}

		private IHeatMap CreateLocalProvisioningHeatMap()
		{
			return new CachedHeatMap(this, new LocalServerHeatMapConstructionRequest(this));
		}

		private DatabaseSelector CreateLocalServerDatabaseSelector()
		{
			return new HeatMapDatabaseSelector(this.LocalServerHeatMap, base.Logger);
		}

		private IRequestQueueManager CreateRubsQueue()
		{
			LoadBalanceWorkload loadBalanceWorkload = new LoadBalanceWorkload(this.Settings);
			SystemWorkloadManager.Initialize(new LoadBalanceActivityLogger());
			SystemWorkloadManager.RegisterWorkload(loadBalanceWorkload);
			return loadBalanceWorkload;
		}

		private IEnumerable<IDirectoryListener> GetDirectoryListeners()
		{
			yield return new MailboxProcessorDispatcher(this, new Func<IRequestQueueManager, IList<MailboxProcessor>>(this.GetMailboxProcessors));
			yield break;
		}

		private IList<MailboxProcessor> GetMailboxProcessors(IRequestQueueManager queueManager)
		{
			IGetMoveInfo getMoveInfo = new GetMoveInfo(base.Logger, this.CmdletPool);
			IEventNotificationSender eventNotificationSender = new EventNotificationSender();
			IMailboxPolicy[] mailboxPolicies = this.GetMailboxPolicies(eventNotificationSender);
			return new MailboxProcessor[]
			{
				new MailboxStatisticsLogger(base.Logger, this.LogCollector),
				new MailboxPolicyProcessor(base.Logger, getMoveInfo, this.MoveInjector, mailboxPolicies)
			};
		}

		internal const string LoadBalanceApplicationName = "MailboxLoadBalance";

		private readonly Lazy<IClientFactory> clientFactory;

		private readonly Lazy<DatabaseSelector> databaseSelector;

		private readonly Lazy<IDirectoryProvider> directory;

		private readonly Lazy<TopologyExtractorFactoryContextPool> extractorFactoryContextPool;

		private readonly Lazy<IHeatMap> heatMap;

		private readonly Lazy<IHeatMap> localProvisioningHeatMap;

		private readonly Lazy<DatabaseSelector> localServerDatabaseSelector;

		private readonly Lazy<ObjectLogCollector> logCollector;

		private readonly Lazy<MoveInjector> moveInjector;

		private readonly Lazy<IHeatMap> provisioningHeatMap;

		private readonly Lazy<IRequestQueueManager> requestQueueManager;

		private readonly Lazy<MailboxLoadBalanceService> service;
	}
}
