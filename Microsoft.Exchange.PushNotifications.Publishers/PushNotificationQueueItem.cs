using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationQueueItem<TNotif> where TNotif : PushNotification
	{
		public TNotif Notification { get; set; }

		public AverageTimeCounterBase QueueTimeCounter { get; set; }
	}
}
