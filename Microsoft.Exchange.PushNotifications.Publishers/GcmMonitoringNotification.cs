using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class GcmMonitoringNotification : GcmNotification
	{
		public GcmMonitoringNotification(string appId, string deviceId) : base(appId, OrganizationId.ForestWideOrgId, deviceId, new GcmPayload(new int?(5), null, null, BackgroundSyncType.None), "c", null, null)
		{
		}

		public override bool IsMonitoring
		{
			get
			{
				return true;
			}
		}

		public override bool DryRun
		{
			get
			{
				return true;
			}
		}
	}
}
