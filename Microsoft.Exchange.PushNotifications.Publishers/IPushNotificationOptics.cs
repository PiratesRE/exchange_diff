using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IPushNotificationOptics
	{
		void ReportReceived(MulticastNotification notification, PushNotificationPublishingContext context);

		void ReportDiscardedByValidation(MulticastNotification notification);

		void ReportReceived(Notification notification, PushNotificationPublishingContext context);

		void ReportDiscardedByValidation(Notification notification);

		void ReportDiscardedByUnsuitablePublisher(Notification notification);

		void ReportDiscardedByUnknownPublisher(Notification notification);

		void ReportDiscardedByUnknownMapping(Notification notification);

		void ReportDiscardedByFailedMapping(Notification notification);

		void ReportDiscardedByDisposedPublisher(Notification notification);

		void ReportProcessed(Notification notification, PushNotification pushNotification, PushNotificationPublishingContext context);

		void ReportDiscardedByMissmatchingType(PushNotification notification);

		void ReportDiscardedByValidation(PushNotification notification, Exception ex);

		void ReportDiscardedByUnsuitablePublisher(PushNotification notification);

		void ReportDiscardedByUnknownPublisher(PushNotification notification);

		void ReportDiscardedByThrottling(PushNotification notification, OverBudgetException obe);

		void ReportDiscardedByDisposedPublisher(PushNotification notification);
	}
}
