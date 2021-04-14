using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WnsMonitoringNotification : WnsXmlNotification
	{
		public WnsMonitoringNotification(string appId, string deviceId) : base(appId, OrganizationId.ForestWideOrgId, deviceId)
		{
		}

		public override bool IsMonitoring
		{
			get
			{
				return true;
			}
		}

		protected override void WriteWnsXmlPayload(WnsPayloadWriter wpw)
		{
			wpw.WriteElementStart("monitoring", false);
			wpw.WriteElementEnd();
		}
	}
}
