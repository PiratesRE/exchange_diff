using System;
using Microsoft.Practices.Unity;

namespace Microsoft.Office.Datacenter.Monitoring.ActiveMonitoring.Recovery
{
	public static class Dependencies
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

		public static ILamRpc LamRpc
		{
			get
			{
				return Dependencies.Container.Resolve<ILamRpc>();
			}
		}

		public static IThrottleHelper ThrottleHelper
		{
			get
			{
				return Dependencies.Container.Resolve<IThrottleHelper>();
			}
		}

		public static void RegisterInterfaces(ILamRpc lamRpc, IThrottleHelper throttleHelper)
		{
			Dependencies.lamRpc = lamRpc;
			Dependencies.throttleHelper = throttleHelper;
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
				Dependencies.container = null;
			}
		}

		private static IUnityContainer Initialize()
		{
			return new UnityContainer().RegisterInstance<ILamRpc>(Dependencies.lamRpc, new ContainerControlledLifetimeManager()).RegisterInstance<IThrottleHelper>(Dependencies.throttleHelper, new ContainerControlledLifetimeManager());
		}

		private static ILamRpc lamRpc = null;

		private static IThrottleHelper throttleHelper = null;

		private static IUnityContainer container = null;

		private static object objectForLock = new object();
	}
}
