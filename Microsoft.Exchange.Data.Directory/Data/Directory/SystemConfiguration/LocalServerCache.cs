using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class LocalServerCache
	{
		public static event LocalServerCache.LocalServerChangeHandler Change;

		public static string LocalServerFqdn
		{
			get
			{
				if (LocalServerCache.localServerFqdn == null)
				{
					LocalServerCache.localServerFqdn = NativeHelpers.GetLocalComputerFqdn(true);
				}
				return LocalServerCache.localServerFqdn;
			}
		}

		public static Server LocalServer
		{
			get
			{
				LocalServerCache.InitializeIfNeeded();
				return LocalServerCache.localServer;
			}
		}

		internal static void Initialize()
		{
			LocalServerCache.ReadLocalServer();
		}

		private static void InitializeIfNeeded()
		{
			if (!LocalServerCache.IsInitialized())
			{
				lock (LocalServerCache.locker)
				{
					if (!LocalServerCache.IsInitialized())
					{
						LocalServerCache.timeout = DateTime.UtcNow + LocalServerCache.ExpirationTime;
						try
						{
							LocalServerCache.ReadLocalServer();
							LocalServerCache.SubscribeForNotifications();
						}
						catch (LocalizedException arg)
						{
							LocalServerCache.Tracer.TraceError<LocalizedException>(0L, "LocalServerCache: unable to initialize due exception: {0}", arg);
						}
						if (LocalServerCache.notification != null)
						{
							LocalServerCache.timeout = DateTime.MaxValue;
						}
						LocalServerCache.initialized = true;
					}
				}
			}
		}

		private static bool IsInitialized()
		{
			return !(DateTime.UtcNow > LocalServerCache.timeout) && LocalServerCache.initialized;
		}

		private static void SubscribeForNotifications()
		{
			if (LocalServerCache.notification == null && LocalServerCache.localServer != null)
			{
				LocalServerCache.notification = ADNotificationAdapter.RegisterChangeNotification<Server>(LocalServerCache.localServer.Id, new ADNotificationCallback(LocalServerCache.NotificationHandler));
			}
		}

		private static void NotificationHandler(ADNotificationEventArgs args)
		{
			LocalServerCache.Tracer.TraceDebug(0L, "LocalServerCache: local server object changed");
			try
			{
				LocalServerCache.ReadLocalServer();
			}
			catch (LocalizedException arg)
			{
				LocalServerCache.Tracer.TraceError<LocalizedException>(0L, "LocalServerCache: failed to read local server object from AD due exception: {0}", arg);
				return;
			}
			if (LocalServerCache.Change != null)
			{
				LocalServerCache.Tracer.TraceDebug(0L, "LocalServerCache: notifying subscribers of change.");
				LocalServerCache.Change(LocalServerCache.localServer);
			}
		}

		private static void ReadLocalServer()
		{
			LocalServerCache.Tracer.TraceDebug(0L, "LocalServerCache: reading local server object");
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 208, "ReadLocalServer", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\ConfigurationCache\\LocalServerCache.cs");
			LocalServerCache.localServer = topologyConfigurationSession.ReadLocalServer();
		}

		private static readonly Trace Tracer = ExTraceGlobals.SystemConfigurationCacheTracer;

		private static readonly TimeSpan ExpirationTime = TimeSpan.FromMinutes(5.0);

		private static DateTime timeout;

		private static bool initialized;

		private static object locker = new object();

		private static ADNotificationRequestCookie notification;

		private static string localServerFqdn;

		private static Server localServer;

		public delegate void LocalServerChangeHandler(Server server);
	}
}
