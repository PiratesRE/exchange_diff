using System;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Exchange.Cluster.Replay.Dumpster;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.HA.FailureItem;
using Microsoft.Exchange.Isam.Esebcli;
using Microsoft.Practices.Unity;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal static class Dependencies
	{
		public static IUnityContainer Container
		{
			get
			{
				if (Dependencies.container == null)
				{
					lock (Dependencies.objectForLock)
					{
						if (Dependencies.container == null)
						{
							Dependencies.container = Dependencies.Initialize();
						}
					}
				}
				return Dependencies.container;
			}
		}

		public static void SetTestContainer(IUnityContainer c)
		{
			lock (Dependencies.objectForLock)
			{
				IUnityContainer unityContainer = Dependencies.container;
				Dependencies.container = c;
			}
		}

		public static IWatson Watson
		{
			get
			{
				return Dependencies.Container.Resolve<IWatson>();
			}
		}

		internal static IAssert Assert
		{
			get
			{
				return Dependencies.Container.Resolve<IAssert>();
			}
		}

		[Conditional("DEBUG")]
		public static void AssertDbg(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				Dependencies.Assert.Debug(condition, formatString, parameters);
			}
		}

		public static void AssertRtl(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				Dependencies.Assert.Retail(condition, formatString, parameters);
			}
		}

		public static IFailureItemPublisher FailureItemPublisher
		{
			get
			{
				return Dependencies.Container.Resolve<IFailureItemPublisher>();
			}
		}

		public static IRegistryKeyFactory RegistryKeyProvider
		{
			get
			{
				return Dependencies.Container.Resolve<IRegistryKeyFactory>();
			}
		}

		public static IADSessionFactory ADSessionFactory
		{
			get
			{
				return Dependencies.Container.Resolve<IADSessionFactory>();
			}
		}

		public static IReplayAdObjectLookup ReplayAdObjectLookup
		{
			get
			{
				return Dependencies.Container.Resolve<IReplayAdObjectLookup>(ReplayAdObjectLookupMapping.IgnoreInvalidAdSession.ToString());
			}
		}

		public static IReplayAdObjectLookup ReplayAdObjectLookupPartiallyConsistent
		{
			get
			{
				return Dependencies.Container.Resolve<IReplayAdObjectLookup>(ReplayAdObjectLookupMapping.PartiallyConsistentAdSession.ToString());
			}
		}

		public static IReplayCoreManager ReplayCoreManager
		{
			get
			{
				return Dependencies.Container.Resolve<IReplayCoreManager>();
			}
		}

		public static IRunConfigurationUpdater ConfigurationUpdater
		{
			get
			{
				return Dependencies.Container.Resolve<IRunConfigurationUpdater>();
			}
		}

		public static IMonitoringADConfigProvider MonitoringADConfigProvider
		{
			get
			{
				return Dependencies.Container.Resolve<IMonitoringADConfigProvider>();
			}
		}

		public static IADConfig ADConfig
		{
			get
			{
				return Dependencies.Container.Resolve<IADConfig>();
			}
		}

		public static ICopyStatusClientLookup MonitoringCopyStatusClientLookup
		{
			get
			{
				return Dependencies.Container.Resolve<ICopyStatusClientLookup>();
			}
		}

		public static ISafetyNetVersionCheck SafetyNetVersionCheck
		{
			get
			{
				return Dependencies.Container.Resolve<ISafetyNetVersionCheck>();
			}
		}

		public static IThreadPoolThreadCountManager ThreadPoolThreadCountManager
		{
			get
			{
				return Dependencies.sm_threadpoolManager;
			}
		}

		public static ITcpConnector TcpConnector
		{
			get
			{
				return Dependencies.Container.Resolve<ITcpConnector>();
			}
		}

		public static IFindComponent ComponentFinder
		{
			get
			{
				return Dependencies.Container.Resolve<IFindComponent>();
			}
		}

		public static IManagementClassHelper ManagementClassHelper
		{
			get
			{
				return Dependencies.Container.Resolve<IManagementClassHelper>();
			}
		}

		public static IAmRpcClientHelper AmRpcClientWrapper
		{
			get
			{
				return Dependencies.Container.Resolve<IAmRpcClientHelper>();
			}
		}

		public static IReplayRpcClient ReplayRpcClientWrapper
		{
			get
			{
				return Dependencies.Container.Resolve<IReplayRpcClient>();
			}
		}

		public static IListMDBStatus GetStoreListMDBStatusInstance(string serverNameOrFqdn)
		{
			return Dependencies.GetStoreListMDBStatusInstance(serverNameOrFqdn, null);
		}

		public static IListMDBStatus GetStoreListMDBStatusInstance(string serverNameOrFqdn, string clientTypeId)
		{
			IStoreRpcFactory storeRpcFactory = Dependencies.Container.Resolve<IStoreRpcFactory>();
			return storeRpcFactory.ConstructListMDBStatus(serverNameOrFqdn, clientTypeId);
		}

		public static IStoreMountDismount GetStoreMountDismountInstance(string serverNameOrFqdn)
		{
			IStoreRpcFactory storeRpcFactory = Dependencies.Container.Resolve<IStoreRpcFactory>();
			return storeRpcFactory.ConstructMountDismount(serverNameOrFqdn);
		}

		public static IStoreRpc GetNewStoreControllerInstance(string serverNameOrFqdn)
		{
			return Dependencies.GetNewStoreControllerInstance(serverNameOrFqdn, null);
		}

		public static IStoreRpc GetNewStoreControllerInstance(string serverNameOrFqdn, string clientTypeId)
		{
			IStoreRpcFactory storeRpcFactory = Dependencies.Container.Resolve<IStoreRpcFactory>();
			return storeRpcFactory.Construct(serverNameOrFqdn, clientTypeId);
		}

		public static IStoreRpc GetNewStoreControllerInstanceNoTimeout(string serverNameOrFqdn)
		{
			IStoreRpcFactory storeRpcFactory = Dependencies.Container.Resolve<IStoreRpcFactory>();
			return storeRpcFactory.ConstructWithNoTimeout(serverNameOrFqdn);
		}

		public static void RegisterAll()
		{
			if (Dependencies.container != null)
			{
				Dependencies.container.Dispose();
			}
			Dependencies.container = Dependencies.Initialize();
		}

		public static void UnregisterAll()
		{
			if (Dependencies.container != null)
			{
				Dependencies.container.Dispose();
			}
			Dependencies.container = new UnityContainer();
		}

		private static IUnityContainer Initialize()
		{
			return new UnityContainer().RegisterType<IWatson, Watson>(new ContainerControlledLifetimeManager(), new InjectionMember[0]).RegisterType<IFailureItemPublisher, FailureItemPublisherImpl>(new ContainerControlledLifetimeManager(), new InjectionMember[0]).RegisterType<IADSessionFactory, ADSessionWrapperFactoryImpl>(new ContainerControlledLifetimeManager(), new InjectionMember[0]).RegisterType<ITcpConnector, TcpConnector>(new ContainerControlledLifetimeManager(), new InjectionMember[0]).RegisterType<IStoreRpcFactory, StoreRpcControllerFactory>(new ContainerControlledLifetimeManager(), new InjectionMember[0]).RegisterType<IFindComponent, ComponentFinder>(new ContainerControlledLifetimeManager(), new InjectionMember[0]).RegisterInstance<IReplayAdObjectLookup>(ReplayAdObjectLookupMapping.IgnoreInvalidAdSession.ToString(), new NoncachingReplayAdObjectLookup()).RegisterInstance<IReplayAdObjectLookup>(ReplayAdObjectLookupMapping.PartiallyConsistentAdSession.ToString(), new NoncachingReplayAdObjectLookupPartiallyConsistent()).RegisterInstance<IAssert>(Microsoft.Exchange.Cluster.Common.Extensions.Assert.Instance, new ContainerControlledLifetimeManager()).RegisterInstance<IRegistryKeyFactory>(RegistryKeyFactory.Instance, new ContainerControlledLifetimeManager()).RegisterInstance<IManagementClassHelper>(new ManagementClassHelper(), new ContainerControlledLifetimeManager()).RegisterInstance<IAmRpcClientHelper>(new AmRpcClientWrapper(), new ContainerControlledLifetimeManager()).RegisterInstance<IReplayRpcClient>(new ReplayRpcClientWrapper(), new ContainerControlledLifetimeManager()).RegisterType<IEsebcli, Esebcli>(new InjectionMember[0]);
		}

		private static readonly IThreadPoolThreadCountManager sm_threadpoolManager = new ThreadPoolThreadCountManager();

		private static IUnityContainer container = null;

		private static object objectForLock = new object();
	}
}
