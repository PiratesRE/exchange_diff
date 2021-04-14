using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Win32;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public abstract class OwaSettingsLoaderBase
	{
		internal OwaSettingsLoaderBase()
		{
			if (OwaSettingsLoaderBase.instanceCreated)
			{
				throw new InvalidOperationException("Cannot load more than one OwaSettings");
			}
			OwaSettingsLoaderBase.instanceCreated = true;
		}

		internal virtual void Load()
		{
			OwaRegistryKeys.Initialize();
			OwaConfigurationManager.CreateAndLoadConfigurationManager();
			this.ReadServerCulture();
			this.InitializeLocalVersionFolders();
			this.ReadAutoSaveInterval();
			this.ReadChangeExpiredPasswordEnabled();
		}

		internal virtual void Unload()
		{
		}

		internal void InitializeLocalVersionFolders()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Globals.LoadVersionFolders");
			this.localHostVersion = ServerVersion.CreateFromVersionString(Globals.ApplicationVersion);
			if (this.localHostVersion == null)
			{
				throw new OwaInvalidOperationException("The local host version is invalid");
			}
			string appDomainAppPath = HttpRuntime.AppDomainAppPath;
			string[] directories = Directory.GetDirectories(appDomainAppPath);
			this.localVersionFolders = new Dictionary<ServerVersion, ServerVersion>(new ServerVersion.ServerVersionComparer());
			ExTraceGlobals.CoreDataTracer.TraceDebug<string>(0L, "Looking for version folders under \"{0}\"...", appDomainAppPath);
			foreach (string path in directories)
			{
				string fileName = Path.GetFileName(path);
				ServerVersion serverVersion = ServerVersion.CreateFromVersionString(fileName);
				if (serverVersion != null)
				{
					ExTraceGlobals.CoreDataTracer.TraceDebug<string>(0L, "Added version folder \"{0}\"", serverVersion.ToString());
					this.localVersionFolders.Add(serverVersion, serverVersion);
				}
			}
		}

		private void ReadChangeExpiredPasswordEnabled()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Globals.ReadChangeExpiredPasswordEnabled");
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA"))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("ChangeExpiredPasswordEnabled");
					if (value is int)
					{
						this.changeExpiredPasswordEnabled = ((int)value != 0);
					}
				}
			}
		}

		private void ReadServerCulture()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Globals.ReadServerCulture");
			int num = 0;
			bool flag = false;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Language"))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("ENU", 1033);
					if (value is int)
					{
						num = (int)value;
						flag = true;
					}
				}
			}
			if (!flag)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "Failed to read server language from the registry, defaulting to {0}", 1033);
				this.serverCulture = Culture.GetCultureInfoInstance(1033);
				return;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<int>(0L, "Succesfully read server language from registry = {0}", num);
			if (Culture.IsSupportedCulture(num))
			{
				this.serverCulture = Culture.GetCultureInfoInstance(num);
				return;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<int, int>(0L, "The server culture read from the registry ({0}) is unsupported, defaulting to {1}", num, 1033);
			this.serverCulture = Culture.GetCultureInfoInstance(1033);
		}

		private void ReadAutoSaveInterval()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "Globals.ReadAutoSaveInterval");
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA"))
			{
				if (registryKey != null)
				{
					object value = registryKey.GetValue("AutoSaveInterval");
					if (value is int && (int)value > 0)
					{
						this.autoSaveInterval = (int)value * 60;
					}
				}
			}
		}

		public abstract bool IsPushNotificationsEnabled { get; }

		public abstract bool IsPullNotificationsEnabled { get; }

		public abstract bool IsFolderContentNotificationsEnabled { get; }

		public int MaxSearchStringLength
		{
			get
			{
				return 256;
			}
		}

		public CultureInfo ServerCulture
		{
			get
			{
				if (!Globals.IsInitialized && this.serverCulture == null)
				{
					this.serverCulture = Culture.GetCultureInfoInstance(1033);
				}
				return this.serverCulture;
			}
		}

		public ServerVersion LocalHostVersion
		{
			get
			{
				return this.localHostVersion;
			}
		}

		public Dictionary<ServerVersion, ServerVersion> LocalVersionFolders
		{
			get
			{
				return this.localVersionFolders;
			}
		}

		public bool ArePerfCountersEnabled
		{
			get
			{
				return PerformanceCounterManager.ArePerfCountersEnabled;
			}
		}

		public int AutoSaveInterval
		{
			get
			{
				return this.autoSaveInterval;
			}
		}

		public bool ChangeExpiredPasswordEnabled
		{
			get
			{
				return this.changeExpiredPasswordEnabled;
			}
		}

		public abstract int ConnectionCacheSize { get; }

		public bool ShowDebugInformation
		{
			get
			{
				bool flag = false;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["ShowDebugInformation"], out flag);
				return flag2 && flag;
			}
		}

		public bool EnableEmailReports
		{
			get
			{
				if (!this.ShowDebugInformation)
				{
					return false;
				}
				bool flag = false;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["EnableEmailReports"], out flag);
				return flag2 && flag;
			}
		}

		public abstract bool IsPreCheckinApp { get; }

		public abstract bool ListenAdNotifications { get; }

		public abstract bool RenderBreadcrumbsInAboutPage { get; }

		public bool CollectPerRequestPerformanceStats
		{
			get
			{
				bool flag = true;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["CollectPerRequestPerformanceStats"], out flag);
				return flag2 && flag;
			}
		}

		public bool CollectSearchStrings
		{
			get
			{
				bool flag = true;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["CollectSearchStrings"], out flag);
				return flag2 && flag;
			}
		}

		public bool DisablePrefixSearch
		{
			get
			{
				bool flag = true;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["DisablePrefixSearch"], out flag);
				return !flag2 || flag;
			}
		}

		public bool FilterETag
		{
			get
			{
				bool flag = false;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["FilterETag"], out flag);
				return flag2 && flag;
			}
		}

		public string ContentDeliveryNetworkEndpoint
		{
			get
			{
				string text = ConfigurationManager.AppSettings["ContentDeliveryNetworkEndpoint"];
				if (!string.IsNullOrEmpty(text))
				{
					if (text.EndsWith("/", StringComparison.OrdinalIgnoreCase))
					{
						text = text + Globals.VirtualRootName + "/";
					}
					else
					{
						text = text + "/" + Globals.VirtualRootName + "/";
					}
				}
				return text;
			}
		}

		public string ErrorReportAddress
		{
			get
			{
				string text = ConfigurationManager.AppSettings["ErrorReportAddress"];
				if (string.IsNullOrEmpty(text))
				{
					return "owadbg@microsoft.com";
				}
				return text;
			}
		}

		public abstract int MaximumTemporaryFilteredViewPerUser { get; }

		public abstract int MaximumFilteredViewInFavoritesPerUser { get; }

		public bool SendWatsonReports
		{
			get
			{
				bool flag = true;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["SendWatsonReport"], out flag);
				return !flag2 || flag;
			}
		}

		public bool SendClientWatsonReports
		{
			get
			{
				bool flag2;
				bool flag = bool.TryParse(ConfigurationManager.AppSettings["SendClientWatsonReport"], out flag2);
				return flag && flag2;
			}
		}

		public abstract bool DisableBreadcrumbs { get; }

		public int ServicePointConnectionLimit
		{
			get
			{
				int result = 0;
				bool flag = int.TryParse(ConfigurationManager.AppSettings["ServicePointConnectionLimit"], out result);
				if (flag)
				{
					return result;
				}
				return 65000;
			}
		}

		public bool ProxyToLocalHost
		{
			get
			{
				bool flag = false;
				bool flag2 = bool.TryParse(ConfigurationManager.AppSettings["ProxyToLocalHost"], out flag);
				return flag2 && flag;
			}
		}

		public abstract int MaxBreadcrumbs { get; }

		public abstract bool StoreTransientExceptionEventLogEnabled { get; }

		public abstract int StoreTransientExceptionEventLogThreshold { get; }

		public abstract int StoreTransientExceptionEventLogFrequencyInSeconds { get; }

		public abstract int MaxPendingRequestLifeInSeconds { get; }

		public abstract int MaxItemsInConversationExpansion { get; }

		public abstract int MaxItemsInConversationReadingPane { get; }

		public abstract long MaxBytesInConversationReadingPane { get; }

		public abstract bool HideDeletedItems { get; }

		public abstract string OCSServerName { get; }

		public abstract int ActivityBasedPresenceDuration { get; }

		public abstract int MailTipsMaxClientCacheSize { get; }

		public abstract int MailTipsMaxMailboxSourcedRecipientSize { get; }

		public abstract int MailTipsClientCacheEntryExpiryInHours { get; }

		internal abstract PhishingLevel MinimumSuspiciousPhishingLevel { get; }

		internal abstract int UserContextLockTimeout { get; }

		private const int FailoverServerLcid = 1033;

		private const bool DefaultProxyToLocalHost = false;

		private const int MaximumSearchStringLength = 256;

		private const int DefaultServicePointConnectionLimit = 65000;

		private static bool instanceCreated;

		private ServerVersion localHostVersion;

		private Dictionary<ServerVersion, ServerVersion> localVersionFolders;

		private CultureInfo serverCulture;

		private int autoSaveInterval = 300;

		private bool changeExpiredPasswordEnabled;
	}
}
