using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ReminderNotifier : PendingRequestNotifierBase
	{
		internal ReminderNotifier(string subscriptionId, IMailboxContext userContext) : base(subscriptionId, userContext)
		{
			this.refreshAll = false;
		}

		public void Clear(bool clearRefreshPayload)
		{
			this.shouldGetRemindersPayload = null;
			if (clearRefreshPayload)
			{
				this.refreshAll = false;
			}
		}

		internal void AddGetRemindersPayload(ReminderNotificationPayload payload)
		{
			lock (this)
			{
				if (!this.refreshAll)
				{
					this.shouldGetRemindersPayload = payload;
				}
			}
		}

		internal void AddRefreshPayload()
		{
			lock (this)
			{
				this.Clear(false);
				if (!this.refreshAll)
				{
					this.refreshAll = true;
				}
			}
		}

		protected override IList<NotificationPayloadBase> ReadDataAndResetStateInternal()
		{
			List<NotificationPayloadBase> list = new List<NotificationPayloadBase>();
			if (this.refreshAll)
			{
				list.Add(new ReminderNotificationPayload
				{
					Reload = true,
					Source = MailboxLocation.FromMailboxContext(base.UserContext)
				});
			}
			else if (this.shouldGetRemindersPayload != null)
			{
				list.Add(this.shouldGetRemindersPayload);
			}
			this.Clear(true);
			return list;
		}

		protected override bool IsDataAvailableForPickup()
		{
			return this.shouldGetRemindersPayload != null || this.refreshAll;
		}

		private bool refreshAll;

		private ReminderNotificationPayload shouldGetRemindersPayload;
	}
}
