using System;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	internal sealed class StartupNotificationWatcher : CrimsonWatcher<StartupNotification>
	{
		public StartupNotificationWatcher() : base(null, true, "Microsoft-Exchange-ManagedAvailability/StartupNotification")
		{
		}

		public StartupNotificationWatcher.StartupNotificationArrivedDelegate StartupNotificationArrivedCallback { get; set; }

		protected override void ResultArrivedHandler(StartupNotification startupNotification)
		{
			if (this.StartupNotificationArrivedCallback != null)
			{
				this.StartupNotificationArrivedCallback(startupNotification);
			}
		}

		protected override string GetDefaultXPathQuery()
		{
			return CrimsonHelper.BuildXPathQueryString(base.ChannelName, null, base.QueryStartTime, base.QueryEndTime, base.QueryUserPropertyCondition);
		}

		public delegate void StartupNotificationArrivedDelegate(StartupNotification startupNotification);
	}
}
