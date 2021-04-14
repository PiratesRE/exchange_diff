using System;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications
{
	internal static class NotificationTracker
	{
		public static void ReportCreated(MulticastNotification notification, ExDateTime timestamp)
		{
			foreach (Notification notification2 in notification.GetFragments())
			{
				NotificationTracker.LogNotificationState(notification2, "AssistantBatchProcessing", new ExDateTime?(timestamp), null);
			}
		}

		public static void ReportCreated(Notification notification, ExDateTime timestamp)
		{
			NotificationTracker.LogNotificationState(notification, "AssistantBatchProcessing", new ExDateTime?(timestamp), null);
		}

		public static void ReportReceived(MulticastNotification notification, string source)
		{
			foreach (Notification notification2 in notification.GetFragments())
			{
				NotificationTracker.LogNotificationState(notification2, "PublisherBatchProcessing", null, source);
			}
		}

		public static void ReportReceived(Notification notification, string source)
		{
			NotificationTracker.LogNotificationState(notification, "PublisherBatchProcessing", null, source);
		}

		public static void ReportDropped(MulticastNotification notification, string traces = "")
		{
			foreach (Notification notification2 in notification.GetFragments())
			{
				NotificationTracker.ReportDropped(notification2, traces);
			}
		}

		public static void ReportDropped(Notification notification, string traces = "")
		{
			PushNotificationsCrimsonEvents.NotificationDropped.Log<string, string, string, string>(notification.RecipientId, notification.AppId, notification.Identifier, traces);
		}

		private static void LogNotificationState(Notification notification, string state, ExDateTime? timestamp = null, string source = null)
		{
			string text = (timestamp != null) ? timestamp.Value.ToString("o") : string.Empty;
			PushNotificationsCrimsonEvents.NotificationTiming.Log<string, string, string, string, string, string, string, string>(notification.AppId, notification.Identifier, state, source ?? string.Empty, text, notification.RecipientId, string.Empty, string.Empty);
		}

		public const string NotificationCreatedLabel = "AssistantBatchProcessing";

		public const string NotificationReceivedLabel = "PublisherBatchProcessing";
	}
}
