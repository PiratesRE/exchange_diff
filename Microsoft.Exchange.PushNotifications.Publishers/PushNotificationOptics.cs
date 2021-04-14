using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationOptics : IPushNotificationOptics
	{
		static PushNotificationOptics()
		{
			foreach (ExPerformanceCounter exPerformanceCounter in PublisherManagerCounters.AllCounters)
			{
				exPerformanceCounter.Reset();
			}
		}

		public void ReportReceived(MulticastNotification notification, PushNotificationPublishingContext context)
		{
			PublisherManagerCounters.TotalMulticastNotificationRequests.Increment();
		}

		public void ReportDiscardedByValidation(MulticastNotification notification)
		{
			PublisherManagerCounters.TotalInvalidMulticastNotifications.Increment();
			PushNotificationsCrimsonEvents.InvalidMulticastNotification.Log<string>(notification.ToNullableString((MulticastNotification x) => x.ToFullString()));
		}

		public void ReportReceived(Notification notification, PushNotificationPublishingContext context)
		{
			PublisherManagerCounters.TotalNotificationRequests.Increment();
		}

		public void ReportDiscardedByValidation(Notification notification)
		{
			PublisherManagerCounters.TotalDiscardedNotifications.Increment();
			PublisherManagerCounters.TotalInvalidNotifications.Increment();
			PushNotificationsCrimsonEvents.InvalidNotification.Log<string, string>(notification.ToNullableString((Notification x) => x.ToFullString()), notification.ValidationErrors.ToNullableString(null));
		}

		public void ReportDiscardedByUnsuitablePublisher(Notification notification)
		{
			PublisherManagerCounters.TotalDiscardedNotifications.Increment();
			if (PushNotificationsCrimsonEvents.PushNotificationUnsuitableAppId.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.PushNotificationUnsuitableAppId.LogPeriodic<string, string, string>(notification.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.Identifier, notification.ToFullString());
			}
		}

		public void ReportDiscardedByUnknownPublisher(Notification notification)
		{
			PublisherManagerCounters.TotalDiscardedNotifications.Increment();
			PushNotificationsCrimsonEvents.PushNotificationUnknownAppId.LogPeriodic<string, string, string>(notification.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.Identifier, notification.ToFullString());
		}

		public void ReportDiscardedByUnknownMapping(Notification notification)
		{
			PublisherManagerCounters.TotalDiscardedNotifications.Increment();
			PushNotificationsCrimsonEvents.NotificationMappingNotFound.LogPeriodic<string, string>(notification.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.ToFullString());
		}

		public void ReportDiscardedByFailedMapping(Notification notification)
		{
			PublisherManagerCounters.TotalDiscardedNotifications.Increment();
			PushNotificationsCrimsonEvents.NotificationMappingFailed.LogPeriodic<string, string>(notification.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.ToFullString());
		}

		public void ReportDiscardedByDisposedPublisher(Notification notification)
		{
			PublisherManagerCounters.TotalDiscardedNotifications.Increment();
		}

		public void ReportProcessed(Notification notification, PushNotification pushNotification, PushNotificationPublishingContext context)
		{
			NotificationTracker.ReportReceived(notification, context.Source);
			PushNotificationTracker.ReportCreated(pushNotification, context.ReceivedTime, notification);
		}

		public void ReportDiscardedByMissmatchingType(PushNotification notification)
		{
			PublisherManagerCounters.TotalDiscardedPushNotifications.Increment();
			PushNotificationsCrimsonEvents.InvalidPushNotificationType.Log<string, string>(notification.AppId, notification.ToFullString());
		}

		public void ReportDiscardedByValidation(PushNotification notification, Exception ex)
		{
			PublisherManagerCounters.TotalInvalidNotifications.Increment();
			PublisherManagerCounters.TotalDiscardedPushNotifications.Increment();
			string nullable = (notification != null) ? notification.AppId : null;
			PushNotificationsCrimsonEvents.InvalidPushNotification.Log<string, string, string>(nullable.ToNullableString(), notification.ToNullableString((PushNotification x) => x.ToFullString()), ex.ToNullableString((Exception x) => x.ToTraceString()));
		}

		public void ReportDiscardedByUnsuitablePublisher(PushNotification notification)
		{
			PublisherManagerCounters.TotalDiscardedPushNotifications.Increment();
			if (PushNotificationsCrimsonEvents.PushNotificationUnsuitableAppId.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.PushNotificationUnsuitableAppId.LogPeriodic<string, string, string>(notification.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.Identifier, notification.ToFullString());
			}
		}

		public void ReportDiscardedByUnknownPublisher(PushNotification notification)
		{
			PublisherManagerCounters.TotalDiscardedPushNotifications.Increment();
			PushNotificationsCrimsonEvents.PushNotificationUnknownAppId.LogPeriodic<string, string, string>(notification.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.Identifier, notification.ToFullString());
		}

		public void ReportDiscardedByThrottling(PushNotification notification, OverBudgetException obe)
		{
			PublisherManagerCounters.TotalDiscardedPushNotifications.Increment();
			string text = obe.ToNullableString((OverBudgetException x) => x.ToTraceString());
			PushNotificationTracker.ReportDropped(notification, text);
			PushNotificationsCrimsonEvents.DeviceOverBudget.LogPeriodic<string, string, string>(notification.RecipientId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, notification.AppId, notification.ToFullString(), text);
		}

		public void ReportDiscardedByDisposedPublisher(PushNotification notification)
		{
			PublisherManagerCounters.TotalDiscardedPushNotifications.Increment();
		}

		public static readonly PushNotificationOptics Default = new PushNotificationOptics();
	}
}
