using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationQueue<TNotif> where TNotif : PushNotification
	{
		public PushNotificationQueue(int capacity)
		{
			this.notificationQueue = new BlockingCollection<PushNotificationQueueItem<TNotif>>(capacity);
		}

		public virtual bool TryAdd(PushNotificationQueueItem<TNotif> item, int millisecondsTimeout, CancellationToken cancellationToken)
		{
			return this.notificationQueue.TryAdd(item, millisecondsTimeout, cancellationToken);
		}

		public virtual bool TryTake(out PushNotificationQueueItem<TNotif> item, int millisecondsTimeout, CancellationToken cancellationToken)
		{
			return this.notificationQueue.TryTake(out item, millisecondsTimeout, cancellationToken);
		}

		public virtual IEnumerable<PushNotificationQueueItem<TNotif>> GetConsumingEnumerable(CancellationToken cancellationToken)
		{
			return this.notificationQueue.GetConsumingEnumerable(cancellationToken);
		}

		public virtual void CompleteAdding()
		{
			this.notificationQueue.CompleteAdding();
		}

		public virtual void Dispose()
		{
			this.notificationQueue.Dispose();
		}

		private BlockingCollection<PushNotificationQueueItem<TNotif>> notificationQueue;
	}
}
