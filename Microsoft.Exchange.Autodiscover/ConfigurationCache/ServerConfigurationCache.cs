using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Autodiscover;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Autodiscover.ConfigurationCache
{
	internal sealed class ServerConfigurationCache
	{
		static ServerConfigurationCache()
		{
			int num = 15;
			int.TryParse(ConfigurationManager.AppSettings["Autodiscover.CacheDefaultRefreshCycleInterval"], out num);
			if (num < 1 || num > 60)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<int, int>(0L, "[ServerConfigurationCache] refresh cycle interval value {0} is out of range. Reset to default value {1}", num, 15);
				num = 15;
			}
			using (ActivityContext.SuppressThreadScope())
			{
				ServerConfigurationCache.cacheRefreshTimer = new Timer(new TimerCallback(ServerConfigurationCache.UpdateCacheCallback), null, TimeSpan.FromSeconds(0.0), TimeSpan.FromMinutes((double)num));
				AppDomain.CurrentDomain.DomainUnload += ServerConfigurationCache.ApplicationDomainUnload;
			}
		}

		private ServerConfigurationCache()
		{
			this.oabExtensionAttribute = null;
			this.DoWrappedRegistryRead("SYSTEM\\CurrentControlSet\\Services\\MSExchange Autodiscover", delegate(RegistryKey key)
			{
				this.oabExtensionAttribute = (string)key.GetValue("OAB Extension Attribute");
			});
			this.serverCache = new ServerCache();
			this.mdbCache = new MailboxDatabaseCache();
			this.oabCache = new OabCache();
			this.outlookProviderCache = new OutlookProviderCache();
			this.clientAccessArrayCache = new ClientAccessArrayCache();
			this.siteCache = new SiteCache();
			this.scpCache = new ADServiceConnectionPointCache();
			this.allCaches = new List<IConfigCache>();
			this.allCaches.Add(this.serverCache);
			this.allCaches.Add(this.mdbCache);
			this.allCaches.Add(this.outlookProviderCache);
			this.allCaches.Add(this.clientAccessArrayCache);
			this.allCaches.Add(this.siteCache);
			this.allCaches.Add(this.scpCache);
			if (VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Autodiscover.LoadNegoExSspNames.Enabled)
			{
				this.DoWrappedRegistryRead("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ExchangeLiveServices", delegate(RegistryKey key)
				{
					string[] array = (string[])key.GetValue("NegoExSSPNames");
					if (array != null)
					{
						foreach (string item in array)
						{
							this.negoExSSPNames.Add(item);
						}
					}
				});
			}
			this.DoWrappedRegistryRead("SYSTEM\\CurrentControlSet\\Services\\MSExchangeRPC", delegate(RegistryKey key)
			{
				string text = key.GetValue("MinimumMapiHttpAutodiscoverVersion") as string;
				if (string.IsNullOrEmpty(text))
				{
					this.enableMapiHttpAutodiscover = null;
					this.minimumMapiHttpAutodiscoverVersion = null;
					return;
				}
				int num;
				if (int.TryParse(text, out num) && num == 0)
				{
					this.enableMapiHttpAutodiscover = new bool?(false);
					this.minimumMapiHttpAutodiscoverVersion = null;
					return;
				}
				Version version;
				if (Version.TryParse(text, out version))
				{
					this.minimumMapiHttpAutodiscoverVersion = new Version(version.Major, version.Minor, version.Build, 0);
					this.enableMapiHttpAutodiscover = new bool?(true);
					return;
				}
				this.enableMapiHttpAutodiscover = null;
				this.minimumMapiHttpAutodiscoverVersion = null;
			});
			this.allCachesAreLoaded = false;
		}

		private void DoWrappedRegistryRead(string registrySubkeyName, ServerConfigurationCache.WrappedRegistryDelegate registryDelegate)
		{
			Exception ex = null;
			try
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(registrySubkeyName))
				{
					if (registryKey != null)
					{
						registryDelegate(registryKey);
					}
				}
			}
			catch (SecurityException ex2)
			{
				ex = ex2;
			}
			catch (UnauthorizedAccessException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				ExTraceGlobals.FrameworkTracer.TraceError<string, Exception>(0L, "Failed to check for registry key {0}. The exception is: {1}", registrySubkeyName, ex);
			}
		}

		public bool AllCachesAreLoaded
		{
			get
			{
				return this.allCachesAreLoaded;
			}
		}

		internal static ServerConfigurationCache Singleton
		{
			get
			{
				return ServerConfigurationCache.singleton;
			}
		}

		internal ServerCache ServerCache
		{
			get
			{
				return this.serverCache;
			}
		}

		internal MailboxDatabaseCache MdbCache
		{
			get
			{
				return this.mdbCache;
			}
		}

		internal OabCache OabCache
		{
			get
			{
				return this.oabCache;
			}
		}

		internal OutlookProviderCache OutlookProviderCache
		{
			get
			{
				return this.outlookProviderCache;
			}
		}

		internal ClientAccessArrayCache ClientAccessArrayCache
		{
			get
			{
				return this.clientAccessArrayCache;
			}
		}

		internal SiteCache SiteCache
		{
			get
			{
				return this.siteCache;
			}
		}

		internal ADServiceConnectionPointCache ScpCache
		{
			get
			{
				return this.scpCache;
			}
		}

		internal string OabExtensionAttribute
		{
			get
			{
				return this.oabExtensionAttribute;
			}
		}

		internal HashSet<string> NegoExSSPNames
		{
			get
			{
				return this.negoExSSPNames;
			}
		}

		internal bool? EnableMapiHttpAutodiscover
		{
			get
			{
				return this.enableMapiHttpAutodiscover;
			}
		}

		internal Version MinimumMapiHttpAutodiscoverVersion
		{
			get
			{
				if (this.EnableMapiHttpAutodiscover == null || !this.EnableMapiHttpAutodiscover.Value)
				{
					throw new InvalidOperationException("EnableMapiHttpAutodiscover must be true for this property to be valid.");
				}
				return this.minimumMapiHttpAutodiscoverVersion;
			}
		}

		public static void ApplicationDomainUnload(object sender, EventArgs e)
		{
			if (ServerConfigurationCache.cacheRefreshTimer != null)
			{
				ServerConfigurationCache.cacheRefreshTimer.Dispose();
				ServerConfigurationCache.cacheRefreshTimer = null;
			}
		}

		internal static void UpdateCacheCallback(object stateInfo)
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 460, "UpdateCacheCallback", "f:\\15.00.1497\\sources\\dev\\autodisc\\src\\ConfigurationCache\\ServerConfigurationCache.cs");
			tenantOrTopologyConfigurationSession.ServerTimeout = new TimeSpan?(ServerConfigurationCache.systemConfigLookupTimeout);
			ServerConfigurationCache.Singleton.RefreshCaches(tenantOrTopologyConfigurationSession);
		}

		internal void RefreshCaches(IConfigurationSession configSession)
		{
			int num = 0;
			foreach (IConfigCache configCache in this.allCaches)
			{
				try
				{
					configCache.Refresh(configSession);
					num++;
				}
				catch (LocalizedException ex)
				{
					ExTraceGlobals.FrameworkTracer.TraceError<string, string>(0L, "[RefreshCaches()] 'LocalizedException' Message=\"{0}\";StackTrace=\"{1}\"", ex.Message, ex.StackTrace);
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrWebException, Common.PeriodicKey, new object[]
					{
						ex.Message,
						ex.StackTrace
					});
				}
			}
			if (num == this.allCaches.Count)
			{
				this.allCachesAreLoaded = true;
			}
		}

		internal const string RpcConfigurationKeyName = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeRPC";

		internal const string MapiHttpValueName = "MinimumMapiHttpAutodiscoverVersion";

		private const int DefaultRefreshCycleInterval = 15;

		private const int MinRefreshCycleInterval = 1;

		private const int MaxRefreshCycleInterval = 60;

		private static readonly ServerConfigurationCache singleton = new ServerConfigurationCache();

		private static readonly TimeSpan systemConfigLookupTimeout = TimeSpan.FromMinutes(1.0);

		private static Timer cacheRefreshTimer;

		private readonly ServerCache serverCache;

		private readonly MailboxDatabaseCache mdbCache;

		private readonly OabCache oabCache;

		private readonly ClientAccessArrayCache clientAccessArrayCache;

		private readonly SiteCache siteCache;

		private readonly OutlookProviderCache outlookProviderCache;

		private readonly ADServiceConnectionPointCache scpCache;

		private readonly List<IConfigCache> allCaches;

		private string oabExtensionAttribute;

		private bool allCachesAreLoaded;

		private HashSet<string> negoExSSPNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private bool? enableMapiHttpAutodiscover;

		private Version minimumMapiHttpAutodiscoverVersion;

		private delegate void WrappedRegistryDelegate(RegistryKey key);
	}
}
