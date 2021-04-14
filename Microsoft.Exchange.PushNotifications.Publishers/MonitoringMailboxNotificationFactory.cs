using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class MonitoringMailboxNotificationFactory
	{
		public MonitoringMailboxNotificationFactory()
		{
			this.RecipientFactories = new Dictionary<string, IMonitoringMailboxNotificationRecipientFactory>();
		}

		private Dictionary<string, IMonitoringMailboxNotificationRecipientFactory> RecipientFactories { get; set; }

		public void RegisterAppToMonitor(string appId, IMonitoringMailboxNotificationRecipientFactory recipientFactory)
		{
			this.RecipientFactories.Add(appId, recipientFactory);
		}

		public MailboxNotificationBatch CreateMonitoringNotificationBatch()
		{
			if (this.RecipientFactories.Count <= 0)
			{
				throw new InvalidOperationException("The factory has no registered apps");
			}
			List<MailboxNotificationRecipient> list = new List<MailboxNotificationRecipient>(this.RecipientFactories.Count);
			int num = 0;
			foreach (string text in this.RecipientFactories.Keys)
			{
				list.Add(this.RecipientFactories[text].CreateMonitoringRecipient(text, num++));
			}
			return new MailboxNotificationBatch
			{
				Notifications = new List<MailboxNotification>(1),
				Notifications = 
				{
					new MailboxNotification(MailboxNotificationPayload.CreateMonitoringPayload(""), list)
				}
			};
		}

		public MailboxNotificationBatch CreateMonitoringNotificationBatchForAzure(string monitoringTenantId, string deviceTokenPrefix)
		{
			if (this.RecipientFactories.Count <= 0)
			{
				throw new InvalidOperationException("The factory has no registered apps");
			}
			List<MailboxNotificationRecipient> list = new List<MailboxNotificationRecipient>(this.RecipientFactories.Count);
			string recipientId = string.Empty;
			foreach (string text in this.RecipientFactories.Keys)
			{
				if (string.IsNullOrEmpty(deviceTokenPrefix))
				{
					recipientId = Guid.NewGuid().ToString();
				}
				else
				{
					recipientId = this.GetMonitoringDeviceToken(deviceTokenPrefix, text);
				}
				list.Add(this.RecipientFactories[text].CreateMonitoringRecipient(text, recipientId));
			}
			return new MailboxNotificationBatch
			{
				Notifications = new List<MailboxNotification>(1),
				Notifications = 
				{
					new MailboxNotification(MailboxNotificationPayload.CreateMonitoringPayload(monitoringTenantId), list)
				}
			};
		}

		public MailboxNotificationBatch CreateExternalMonitoringNotificationBatch()
		{
			List<MailboxNotificationRecipient> list = new List<MailboxNotificationRecipient>(1);
			list.Add(ApnsNotificationFactory.Default.CreateMonitoringRecipient("MonitoringProbeAppId", 0));
			return new MailboxNotificationBatch
			{
				Notifications = new List<MailboxNotification>(1),
				Notifications = 
				{
					new MailboxNotification(MailboxNotificationPayload.CreateMonitoringPayload(""), list)
				}
			};
		}

		public string GetMonitoringDeviceToken(string deviceTokenString, string appId)
		{
			deviceTokenString += appId;
			switch (PushNotificationsMonitoring.CannedAppPlatformSet[appId])
			{
			case PushNotificationPlatform.APNS:
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (char c in deviceTokenString)
				{
					int num = (int)c;
					stringBuilder.AppendFormat("{0:x2}", Convert.ToUInt32(num.ToString()));
				}
				return stringBuilder.ToString();
			}
			case PushNotificationPlatform.WNS:
				return string.Format("http://127.0.0.1:0/send?id={0}", deviceTokenString);
			case PushNotificationPlatform.GCM:
				return deviceTokenString;
			}
			throw new InvalidOperationException(string.Format("App {0} is not supported for monitoring", appId));
		}

		public const string MonitoringProbeAppId = "MonitoringProbeAppId";
	}
}
