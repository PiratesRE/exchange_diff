using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WebAppMonitoringNotification : WebAppNotification
	{
		public WebAppMonitoringNotification(string appId, string deviceId) : base(appId, OrganizationId.ForestWideOrgId, "PublishO365Notification", new O365Notification("::AE82E53440744F2798C276818CE8BD5C::", deviceId).ToJson())
		{
		}

		public override bool IsMonitoring
		{
			get
			{
				return true;
			}
		}
	}
}
