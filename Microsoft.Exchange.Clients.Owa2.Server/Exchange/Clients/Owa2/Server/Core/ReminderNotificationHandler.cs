using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class ReminderNotificationHandler : MapiNotificationHandlerBase
	{
		public ReminderNotificationHandler(string subscriptionId, IMailboxContext userContext) : base(subscriptionId, userContext, false)
		{
			this.reminderNotifier = new ReminderNotifier(subscriptionId, userContext);
			this.reminderNotifier.RegisterWithPendingRequestNotifier();
		}

		internal override void HandleNotificationInternal(Notification notif, MapiNotificationsLogEvent logEvent, object context)
		{
			if (notif == null)
			{
				return;
			}
			ReminderNotificationPayload reminderNotificationPayload = new ReminderNotificationPayload(true);
			reminderNotificationPayload.SubscriptionId = base.SubscriptionId;
			reminderNotificationPayload.Source = MailboxLocation.FromMailboxContext(base.UserContext);
			this.reminderNotifier.AddGetRemindersPayload(reminderNotificationPayload);
			this.reminderNotifier.PickupData();
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
			lock (base.SyncRoot)
			{
				base.InitSubscription();
				if (base.MissedNotifications)
				{
					base.NeedRefreshPayload = true;
				}
				base.MissedNotifications = false;
			}
			if (base.NeedRefreshPayload)
			{
				ReminderNotificationPayload reminderNotificationPayload = new ReminderNotificationPayload(true);
				reminderNotificationPayload.SubscriptionId = base.SubscriptionId;
				reminderNotificationPayload.Source = MailboxLocation.FromMailboxContext(base.UserContext);
				this.reminderNotifier.AddGetRemindersPayload(reminderNotificationPayload);
				base.NeedRefreshPayload = false;
			}
			this.reminderNotifier.PickupData();
		}

		protected override void InitSubscriptionInternal()
		{
			this.SetUpRemindersSubscription(0);
		}

		private void SetUpRemindersSubscription(int currentRetryCount)
		{
			if (!base.UserContext.MailboxSessionLockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method ReminderNotificationHandler.SetUpRemindersSubscription");
			}
			try
			{
				using (SearchFolder searchFolder = SearchFolder.Bind(base.UserContext.MailboxSession, DefaultFolderType.Reminders))
				{
					base.QueryResult = searchFolder.ItemQuery(ItemQueryType.None, null, ReminderNotificationHandler.sorts, ReminderNotificationHandler.querySubscriptionProperties);
					base.QueryResult.GetRows(1);
					base.Subscription = Subscription.Create(base.QueryResult, new NotificationHandler(base.HandleNotification));
				}
			}
			catch (QueryInProgressException)
			{
				if (currentRetryCount >= 5)
				{
					throw;
				}
				this.SetUpRemindersSubscription(currentRetryCount + 1);
			}
		}

		private const int MaxSubscriptionSetUpRetryCount = 5;

		private static readonly PropertyDefinition[] querySubscriptionProperties = new PropertyDefinition[]
		{
			ItemSchema.Id,
			ItemSchema.ReminderIsSet,
			ItemSchema.ReminderNextTime,
			ItemSchema.ReminderDueBy
		};

		private static readonly SortBy[] sorts = new SortBy[]
		{
			new SortBy(ItemSchema.ReminderIsSet, SortOrder.Descending),
			new SortBy(ItemSchema.ReminderNextTime, SortOrder.Descending)
		};

		private ReminderNotifier reminderNotifier;
	}
}
