using System;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ConnectedAccountsConfiguration : IConnectedAccountsConfiguration
	{
		private ConnectedAccountsConfiguration()
		{
			this.Load();
		}

		public static ConnectedAccountsConfiguration Instance
		{
			get
			{
				if (ConnectedAccountsConfiguration.instance == null)
				{
					lock (ConnectedAccountsConfiguration.instanceSyncLock)
					{
						if (ConnectedAccountsConfiguration.instance == null)
						{
							ConnectedAccountsConfiguration.instance = new ConnectedAccountsConfiguration();
						}
					}
				}
				return ConnectedAccountsConfiguration.instance;
			}
		}

		public bool LogonTriggeredSyncNowEnabled
		{
			get
			{
				return this.logonTriggeredSyncNowEnabled;
			}
		}

		public bool RefreshButtonTriggeredSyncNowEnabled
		{
			get
			{
				return this.refreshButtonTriggeredSyncNowEnabled;
			}
		}

		public TimeSpan RefreshButtonTriggeredSyncNowSuppressThreshold
		{
			get
			{
				return this.refreshButtonTriggeredSyncNowSuppressThreshold;
			}
		}

		public bool PeriodicSyncNowEnabled
		{
			get
			{
				return this.periodicSyncNowEnabled;
			}
		}

		public TimeSpan PeriodicSyncNowInterval
		{
			get
			{
				return this.periodicSyncNowInterval;
			}
		}

		public bool NotificationsEnabled
		{
			get
			{
				return this.notificationsEnabled;
			}
		}

		protected virtual void Load()
		{
			this.logonTriggeredSyncNowEnabled = BaseApplication.GetAppSetting<bool>("UserLogonTriggeredSyncNowEnabled", true);
			this.refreshButtonTriggeredSyncNowEnabled = BaseApplication.GetAppSetting<bool>("RefreshButtonTriggeredSyncNowEnabled", true);
			this.refreshButtonTriggeredSyncNowSuppressThreshold = BaseApplication.GetTimeSpanAppSetting("RefreshButtonTriggeredSyncNowSuppressThreshold", TimeSpan.FromSeconds(5.0));
			this.periodicSyncNowEnabled = BaseApplication.GetAppSetting<bool>("PeriodicSyncNowEnabled", true);
			this.periodicSyncNowInterval = BaseApplication.GetTimeSpanAppSetting("PeriodicSyncNowInterval", TimeSpan.FromMinutes(15.0));
			this.notificationsEnabled = (this.refreshButtonTriggeredSyncNowEnabled || this.logonTriggeredSyncNowEnabled || this.periodicSyncNowEnabled);
			ExTraceGlobals.ConnectedAccountsTracer.TraceDebug((long)this.GetHashCode(), "ConnectedAccountsConfiguration::Configs Loaded - logonTriggeredSyncNowEnabled:{0},periodicSyncNowEnabled:{1},refreshButtonTriggeredSyncNowEnabled:{2},periodicSyncNowInterval:{3},refreshButtonTriggeredSyncNowSuppressThreshold:{4}.", new object[]
			{
				this.logonTriggeredSyncNowEnabled,
				this.periodicSyncNowEnabled,
				this.refreshButtonTriggeredSyncNowEnabled,
				this.periodicSyncNowInterval,
				this.refreshButtonTriggeredSyncNowSuppressThreshold
			});
		}

		private static readonly object instanceSyncLock = new object();

		private static ConnectedAccountsConfiguration instance;

		private bool logonTriggeredSyncNowEnabled;

		private bool refreshButtonTriggeredSyncNowEnabled;

		private TimeSpan refreshButtonTriggeredSyncNowSuppressThreshold;

		private bool periodicSyncNowEnabled;

		private TimeSpan periodicSyncNowInterval;

		private bool notificationsEnabled;
	}
}
