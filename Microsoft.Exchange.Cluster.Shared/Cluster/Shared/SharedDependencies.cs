using System;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Common.Extensions;
using Microsoft.Exchange.Cluster.Common.Registry;
using Microsoft.Practices.Unity;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal static class SharedDependencies
	{
		public static IUnityContainer Container
		{
			get
			{
				if (SharedDependencies.container == null)
				{
					lock (SharedDependencies.objectForLock)
					{
						if (SharedDependencies.container == null)
						{
							SharedDependencies.container = SharedDependencies.Initialize();
						}
					}
				}
				return SharedDependencies.container;
			}
		}

		public static void SetTestContainer(IUnityContainer c)
		{
			lock (SharedDependencies.objectForLock)
			{
				IUnityContainer unityContainer = SharedDependencies.container;
				SharedDependencies.container = c;
			}
		}

		internal static IAssert Assert
		{
			get
			{
				return SharedDependencies.Container.Resolve<IAssert>();
			}
		}

		[Conditional("DEBUG")]
		public static void AssertDbg(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				SharedDependencies.Assert.Debug(condition, formatString, parameters);
			}
		}

		public static void AssertRtl(bool condition, string formatString, params object[] parameters)
		{
			if (!condition)
			{
				SharedDependencies.Assert.Retail(condition, formatString, parameters);
			}
		}

		public static IDiagCoreImpl DiagCoreImpl
		{
			get
			{
				return SharedDependencies.Container.Resolve<IDiagCoreImpl>();
			}
		}

		public static IRegistryKeyFactory RegistryKeyProvider
		{
			get
			{
				return SharedDependencies.Container.Resolve<IRegistryKeyFactory>();
			}
		}

		public static IManagementClassHelper ManagementClassHelper
		{
			get
			{
				return SharedDependencies.Container.Resolve<IManagementClassHelper>();
			}
		}

		public static IAmServerNameLookup AmServerNameLookup
		{
			get
			{
				return SharedDependencies.Container.Resolve<IAmServerNameLookup>();
			}
		}

		public static IWritableAD WritableADHelper
		{
			get
			{
				return SharedDependencies.Container.Resolve<IWritableAD>();
			}
		}

		public static void RegisterAll()
		{
			if (SharedDependencies.container != null)
			{
				SharedDependencies.container.Dispose();
			}
			SharedDependencies.container = SharedDependencies.Initialize();
		}

		public static void UnregisterAll()
		{
			if (SharedDependencies.container != null)
			{
				SharedDependencies.container.Dispose();
			}
			SharedDependencies.container = new UnityContainer();
		}

		private static IUnityContainer Initialize()
		{
			return new UnityContainer().RegisterType<IDiagCoreImpl, ReplDiagCoreImpl>(new ContainerControlledLifetimeManager(), new InjectionMember[0]).RegisterInstance<IAssert>(Microsoft.Exchange.Cluster.Common.Extensions.Assert.Instance, new ContainerControlledLifetimeManager()).RegisterInstance<IRegistryKeyFactory>(RegistryKeyFactory.Instance, new ContainerControlledLifetimeManager()).RegisterInstance<IManagementClassHelper>(new ManagementClassHelper(), new ContainerControlledLifetimeManager()).RegisterInstance<IWritableAD>(new WritableADHelper(), new ContainerControlledLifetimeManager()).RegisterType<IAmServerNameLookup, AmServerNameCache>(new ContainerControlledLifetimeManager(), new InjectionMember[0]);
		}

		private static IUnityContainer container = null;

		private static object objectForLock = new object();
	}
}
