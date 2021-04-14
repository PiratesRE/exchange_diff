using System;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureMonitoringNotification : AzureNotification
	{
		public AzureMonitoringNotification(string appId, string hubName, string recipientId) : base(appId, recipientId, hubName, new AzurePayload(new int?(6), null, null, null), false)
		{
		}

		public override bool IsMonitoring
		{
			get
			{
				return true;
			}
		}

		public override string RecipientId
		{
			get
			{
				return string.Format("{0}{1}", base.RecipientId, "MonitoringTag");
			}
		}

		private const string MonitoringAzureTagSuffix = "MonitoringTag";
	}
}
