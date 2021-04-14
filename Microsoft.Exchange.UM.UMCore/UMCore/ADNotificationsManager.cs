using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class ADNotificationsManager : DisposableBase
	{
		private ADNotificationsManager()
		{
		}

		internal static ADNotificationsManager Instance
		{
			get
			{
				return ADNotificationsManager.instance;
			}
		}

		internal ADNotificationHandler Server
		{
			get
			{
				lock (this.syncLock)
				{
					if (this.server == null)
					{
						this.server = new ServerNotificationHandler();
					}
				}
				return this.server;
			}
		}

		internal ADNotificationHandler CallRouterSettings
		{
			get
			{
				lock (this.syncLock)
				{
					if (this.callRouterSettings == null)
					{
						this.callRouterSettings = new CallRouterSettingsNotificationHandler();
					}
				}
				return this.callRouterSettings;
			}
		}

		internal ADNotificationHandler UMDialPlan
		{
			get
			{
				lock (this.syncLock)
				{
					if (this.dialPlan == null)
					{
						this.dialPlan = new UMDialPlanNotificationHandler();
					}
				}
				return this.dialPlan;
			}
		}

		internal ADNotificationHandler UMIPGateway
		{
			get
			{
				lock (this.syncLock)
				{
					if (this.gateway == null)
					{
						this.gateway = new UMIPGatewayNotificationHandler();
					}
				}
				return this.gateway;
			}
		}

		internal ADNotificationHandler UMHuntGroup
		{
			get
			{
				lock (this.syncLock)
				{
					if (this.huntGroup == null)
					{
						this.huntGroup = new UMHuntGroupNotificationHandler();
					}
				}
				return this.huntGroup;
			}
		}

		internal ADNotificationHandler UMAutoAttendant
		{
			get
			{
				lock (this.syncLock)
				{
					if (this.autoAttendant == null)
					{
						this.autoAttendant = new UMAutoAttendantNotificationHandler();
					}
				}
				return this.autoAttendant;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.server != null)
				{
					this.server.Dispose();
				}
				if (this.callRouterSettings != null)
				{
					this.callRouterSettings.Dispose();
				}
				if (this.dialPlan != null)
				{
					this.dialPlan.Dispose();
				}
				if (this.gateway != null)
				{
					this.gateway.Dispose();
				}
				if (this.huntGroup != null)
				{
					this.huntGroup.Dispose();
				}
				if (this.autoAttendant != null)
				{
					this.autoAttendant.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ADNotificationsManager>(this);
		}

		private static ADNotificationsManager instance = new ADNotificationsManager();

		private object syncLock = new object();

		private ADNotificationHandler callRouterSettings;

		private ADNotificationHandler server;

		private ADNotificationHandler dialPlan;

		private ADNotificationHandler gateway;

		private ADNotificationHandler autoAttendant;

		private ADNotificationHandler huntGroup;
	}
}
