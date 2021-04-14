using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class ApnsMonitoringNotification : ApnsNotification
	{
		public ApnsMonitoringNotification(string appId, string deviceId) : base(appId, OrganizationId.ForestWideOrgId, deviceId, 1, DateTime.UtcNow)
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
