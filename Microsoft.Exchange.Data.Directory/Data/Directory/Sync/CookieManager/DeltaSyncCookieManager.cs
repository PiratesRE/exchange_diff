using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Sync.CookieManager
{
	internal abstract class DeltaSyncCookieManager : CookieManager
	{
		internal string ServiceInstanceName { get; private set; }

		public ServerVersion SyncPropertySetVersion { get; protected set; }

		public bool IsSyncPropertySetUpgrading { get; protected set; }

		internal DeltaSyncCookieManager(string serviceInstanceName, int maxCookieHistoryCount, TimeSpan cookieHistoryInterval)
		{
			if (string.IsNullOrEmpty(serviceInstanceName))
			{
				throw new ArgumentNullException("serviceInstanceName");
			}
			this.ServiceInstanceName = serviceInstanceName;
			this.maxCookieHistoryCount = maxCookieHistoryCount;
			this.cookieHistoryInterval = cookieHistoryInterval;
			this.SyncPropertySetVersion = new ServerVersion(0, 0, 0, 0);
			this.IsSyncPropertySetUpgrading = false;
			this.nextTimeToUpdateIsSyncPropertySetUpgradeAllowed = DateTime.UtcNow;
			this.isSyncPropertySetUpgradeAllowed = false;
		}

		public bool SyncPropertySetUpgradeAvailable(ServerVersion version)
		{
			return this.IsSyncPropertySetUpgradeAllowed() && !this.IsSyncPropertySetUpgrading && ServerVersion.Compare(this.SyncPropertySetVersion, version) < 0;
		}

		public sealed override void WriteCookie(byte[] cookie, DateTime timestamp)
		{
			this.WriteCookie(cookie, null, timestamp, false, null, true);
		}

		public abstract void WriteCookie(byte[] cookie, IEnumerable<string> filteredContextIds, DateTime timestamp, bool isUpgradingCookie, ServerVersion version, bool more);

		public bool IsSyncPropertySetUpgradeAllowed()
		{
			if (this.nextTimeToUpdateIsSyncPropertySetUpgradeAllowed < DateTime.UtcNow)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 152, "IsSyncPropertySetUpgradeAllowed", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Sync\\CookieManager\\DeltaSyncCookieManager.cs");
				Organization orgContainer = topologyConfigurationSession.GetOrgContainer();
				this.isSyncPropertySetUpgradeAllowed = orgContainer.IsSyncPropertySetUpgradeAllowed;
				this.nextTimeToUpdateIsSyncPropertySetUpgradeAllowed = DateTime.UtcNow.AddMinutes(10.0);
			}
			return this.isSyncPropertySetUpgradeAllowed;
		}

		protected void UpdateSyncPropertySetVersion(bool isUpgradingCookie, ServerVersion version, bool more)
		{
			if (isUpgradingCookie)
			{
				this.IsSyncPropertySetUpgrading = true;
				this.SyncPropertySetVersion = version;
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_SyncPropertySetStartingUpgrade, this.ServiceInstanceName, new object[]
				{
					this.SyncPropertySetVersion
				});
				return;
			}
			if (this.IsSyncPropertySetUpgrading && !more)
			{
				this.IsSyncPropertySetUpgrading = false;
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_SyncPropertySetFinishedUpgrade, this.ServiceInstanceName, new object[]
				{
					this.SyncPropertySetVersion
				});
			}
		}

		protected int maxCookieHistoryCount;

		protected TimeSpan cookieHistoryInterval;

		protected bool isSyncPropertySetUpgradeAllowed;

		protected DateTime nextTimeToUpdateIsSyncPropertySetUpgradeAllowed;
	}
}
