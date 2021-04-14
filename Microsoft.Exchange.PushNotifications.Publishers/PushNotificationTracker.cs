using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal static class PushNotificationTracker
	{
		public static void ReportCreated(PushNotification pushNotification, ExDateTime timestamp, Notification notification)
		{
			PushNotificationTracker.LogPushNotificationState(pushNotification, "NotificationCreated", new ExDateTime?(timestamp), notification.Identifier, PushNotificationPlatform.None);
		}

		public static void ReportSent(PushNotification pushNotification, PushNotificationPlatform platform = PushNotificationPlatform.None)
		{
			PushNotificationTracker.LogPushNotificationState(pushNotification, "NotificationSent", null, null, platform);
		}

		public static void ReportDropped(PushNotification pushNotification, string traces = "")
		{
			PushNotificationsCrimsonEvents.NotificationDropped.Log<string, string, string, string>(pushNotification.RecipientId, pushNotification.AppId, pushNotification.Identifier, traces);
		}

		private static void LogPushNotificationState(PushNotification pushNotification, string state, ExDateTime? timestamp = null, string notificationId = null, PushNotificationPlatform platform = PushNotificationPlatform.None)
		{
			string text = (timestamp != null) ? timestamp.Value.ToString("o") : string.Empty;
			PushNotificationsCrimsonEvents.NotificationTiming.Log<string, string, string, string, string, string, string, bool>(pushNotification.AppId, pushNotification.Identifier, state, (platform == PushNotificationPlatform.None) ? string.Empty : platform.ToString(), text, pushNotification.RecipientId, notificationId ?? string.Empty, pushNotification.IsBackgroundSyncAvailable);
		}

		public const string NotificationSentLabel = "NotificationSent";

		public const string NotificationCreatedLabel = "NotificationCreated";
	}
}
