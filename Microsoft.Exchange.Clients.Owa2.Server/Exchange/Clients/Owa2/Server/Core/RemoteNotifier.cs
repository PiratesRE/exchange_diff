using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class RemoteNotifier : PendingRequestNotifierBase
	{
		public RemoteNotifier(IMailboxContext context) : base(context)
		{
		}

		internal void AddRemoteNotificationPayload(RemoteNotificationPayload remoteNotificationPayload)
		{
			lock (this)
			{
				if (this.reloadAll)
				{
					NotificationStatisticsManager.Instance.NotificationReceived(remoteNotificationPayload);
					NotificationStatisticsManager.Instance.NotificationDropped(remoteNotificationPayload, NotificationState.CreatedOrReceived);
				}
				else if (this.queue.Count >= 40)
				{
					NotificationStatisticsManager.Instance.NotificationReceived(remoteNotificationPayload);
					NotificationStatisticsManager.Instance.NotificationDropped(remoteNotificationPayload, NotificationState.CreatedOrReceived);
					NotificationStatisticsManager.Instance.NotificationDropped(this.queue, NotificationState.CreatedOrReceived);
					this.queue.Clear();
					ReloadAllNotificationPayload reloadAllNotificationPayload = new ReloadAllNotificationPayload();
					reloadAllNotificationPayload.Source = new TypeLocation(base.GetType());
					this.queue.Enqueue(reloadAllNotificationPayload);
					NotificationStatisticsManager.Instance.NotificationCreated(reloadAllNotificationPayload);
					this.reloadAll = true;
				}
				else
				{
					this.queue.Enqueue(remoteNotificationPayload);
					NotificationStatisticsManager.Instance.NotificationReceived(remoteNotificationPayload);
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			if (this.queue.Count > 0)
			{
				foreach (NotificationPayloadBase item in this.queue)
				{
					list.Add(item);
				}
				this.queue.Clear();
				this.reloadAll = false;
			}
			return list;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.AreThereNotifications();
		}

		private bool AreThereNotifications()
		{
			return this.queue.Count > 0;
		}

		public const int MaxRemoteNotificationQueueSize = 40;

		protected Queue<NotificationPayloadBase> queue = new Queue<NotificationPayloadBase>();

		private bool reloadAll;
	}
}
