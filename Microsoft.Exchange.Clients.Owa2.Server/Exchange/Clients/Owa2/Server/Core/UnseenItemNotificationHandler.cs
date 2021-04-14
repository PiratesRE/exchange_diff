using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class UnseenItemNotificationHandler : MapiNotificationHandlerBase
	{
		public UnseenItemNotificationHandler(IMailboxContext userContext, IRecipientSession adSession) : base(userContext, true)
		{
			this.adSession = adSession;
		}

		internal string AddMemberSubscription(string subscriptionId, UserMailboxLocator mailboxLocator)
		{
			if (this.groupNotificationLocator == null)
			{
				throw new InvalidOperationException("Cannot add member subscription before subscribing to store notifications");
			}
			string subscriptionId2 = ModernGroupNotificationLocator.GetSubscriptionId(mailboxLocator);
			lock (this.syncNotifierCache)
			{
				if (!this.notifierCache.ContainsKey(subscriptionId2))
				{
					UnseenItemNotifier unseenItemNotifier = new UnseenItemNotifier(subscriptionId, base.UserContext, subscriptionId2, mailboxLocator);
					unseenItemNotifier.RegisterWithPendingRequestNotifier();
					this.notifierCache.Add(subscriptionId2, unseenItemNotifier);
				}
			}
			return subscriptionId2;
		}

		internal void RemoveSubscription(string subscriptionId)
		{
			lock (this.syncNotifierCache)
			{
				if (this.notifierCache.ContainsKey(subscriptionId))
				{
					UnseenItemNotifier unseenItemNotifier = this.notifierCache[subscriptionId];
					unseenItemNotifier.UnregisterWithPendingRequestNotifier();
					this.notifierCache.Remove(subscriptionId);
				}
			}
		}

		internal bool HasNotifiers()
		{
			bool result;
			lock (this.syncNotifierCache)
			{
				result = (this.notifierCache.Count > 0);
			}
			return result;
		}

		internal override void HandleNotificationInternal(Notification notification, MapiNotificationsLogEvent logEvent, object context)
		{
			if (!(notification is QueryNotification))
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "UnseenItemNotificationHandler.HandleNotificationInternal: Received a null QueryNotification object for group {0}", base.UserContext.PrimarySmtpAddress);
				logEvent.NullNotification = true;
				return;
			}
			lock (base.SyncRoot)
			{
				if (!base.IsDisposed)
				{
					this.GenerateAndAddGroupNotificationPayload();
				}
			}
		}

		internal override void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent)
		{
			lock (base.SyncRoot)
			{
				base.InitSubscription();
				if (base.MissedNotifications)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "UnseenItemNotificationHandler.HandlePendingGetTimerCallback this.MissedNotifications == true. SubscriptionId: {0}", base.SubscriptionId);
					base.NeedRefreshPayload = true;
				}
				if (base.NeedRefreshPayload)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "UnseenItemNotificationHandler.HandlePendingGetTimerCallback NeedRefreshPayload. SubscriptionId: {0}", base.SubscriptionId);
					this.GenerateAndAddGroupNotificationPayload();
					base.NeedRefreshPayload = false;
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "UnseenItemNotificationHandler.HandlePendingGetTimerCallback setting this.MissedNotifications = false. SubscriptionId: {0}", base.SubscriptionId);
				base.MissedNotifications = false;
			}
		}

		protected virtual UnseenItemNotificationHandler.NotifierData[] GetUnSeenData()
		{
			UnseenItemNotifier[] notifierList = this.GetNotifierList();
			IMemberSubscriptionItem[] array = null;
			try
			{
				base.UserContext.LockAndReconnectMailboxSession(3000);
				this.unseenItemsReader.LoadLastNItemReceiveDates(base.UserContext.MailboxSession);
				array = this.groupNotificationLocator.GetMemberSubscriptions(base.UserContext.MailboxSession, from n in notifierList
				select n.UserMailboxLocator);
			}
			finally
			{
				if (base.UserContext.MailboxSessionLockedByCurrentThread())
				{
					base.UserContext.UnlockAndDisconnectMailboxSession();
				}
			}
			UnseenItemNotificationHandler.NotifierData[] array2 = new UnseenItemNotificationHandler.NotifierData[notifierList.Length];
			for (int i = 0; i < notifierList.Length; i++)
			{
				array2[i] = new UnseenItemNotificationHandler.NotifierData(notifierList[i], array[i].LastUpdateTimeUTC);
			}
			return array2;
		}

		protected override void InitSubscriptionInternal()
		{
			if (!base.UserContext.MailboxSessionLockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling method UnseenItemNotificationHandler.InitSubscriptionInternal");
			}
			if (this.unseenItemsReader != null)
			{
				this.unseenItemsReader.Dispose();
			}
			this.groupNotificationLocator = new ModernGroupNotificationLocator(this.adSession);
			this.unseenItemsReader = UnseenItemsReader.Create(base.UserContext.MailboxSession);
			StoreObjectId defaultFolderId = base.UserContext.MailboxSession.GetDefaultFolderId(DefaultFolderType.Inbox);
			using (Folder folder = Folder.Bind(base.UserContext.MailboxSession, defaultFolderId))
			{
				base.QueryResult = this.GetQueryResult(folder);
				base.QueryResult.GetRows(base.QueryResult.EstimatedRowCount);
				base.Subscription = Subscription.Create(base.QueryResult, new NotificationHandler(base.HandleNotification));
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "UnseenItemNotificationHandler.InitSubscriptionInternal succeeded for group {0}", base.UserContext.PrimarySmtpAddress);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool, SmtpAddress, Type>((long)this.GetHashCode(), "UnseenItemNotificationHandler.Dispose. IsDisposing: {0}, User: {1}, Type: {2}", isDisposing, base.UserContext.PrimarySmtpAddress, base.GetType());
			lock (base.SyncRoot)
			{
				if (isDisposing && this.unseenItemsReader != null)
				{
					MapiNotificationHandlerBase.DisposeXSOObjects(this.unseenItemsReader, base.UserContext);
					this.unseenItemsReader = null;
				}
				base.InternalDispose(isDisposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<UnseenItemNotificationHandler>(this);
		}

		private void GenerateAndAddGroupNotificationPayload()
		{
			foreach (UnseenItemNotificationHandler.NotifierData notifierData in this.GetUnSeenData())
			{
				notifierData.Notifier.AddGroupNotificationPayload(this.GetPayload(notifierData));
				notifierData.Notifier.PickupData();
			}
		}

		private QueryResult GetQueryResult(Folder folder)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "UnseenItemNotificationHandler.GetQueryResult for group {0}", base.UserContext.PrimarySmtpAddress);
			return folder.ItemQuery(ItemQueryType.None, null, UnseenItemNotificationHandler.UnseenItemSortBy, UnseenItemNotificationHandler.UnseenItemQueryProperties);
		}

		protected UnseenItemNotifier[] GetNotifierList()
		{
			UnseenItemNotifier[] result;
			lock (this.syncNotifierCache)
			{
				UnseenItemNotifier[] array = new UnseenItemNotifier[this.notifierCache.Values.Count];
				this.notifierCache.Values.CopyTo(array, 0);
				result = array;
			}
			return result;
		}

		private UnseenItemNotificationPayload GetPayload(UnseenItemNotificationHandler.NotifierData data)
		{
			return new UnseenItemNotificationPayload
			{
				SubscriptionId = data.Notifier.PayloadSubscriptionId,
				UnseenData = new UnseenDataType(this.unseenItemsReader.GetUnseenItemCount(data.LastVisitedDateUTC), ExDateTimeConverter.ToUtcXsdDateTime(data.LastVisitedDateUTC)),
				Source = MailboxLocation.FromMailboxContext(base.UserContext)
			};
		}

		protected readonly Dictionary<string, UnseenItemNotifier> notifierCache = new Dictionary<string, UnseenItemNotifier>();

		protected IUnseenItemsReader unseenItemsReader;

		private readonly object syncNotifierCache = new object();

		private static readonly PropertyDefinition[] UnseenItemQueryProperties = new PropertyDefinition[]
		{
			ItemSchema.Id
		};

		private static readonly SortBy[] UnseenItemSortBy = new SortBy[]
		{
			new SortBy(StoreObjectSchema.LastModifiedTime, SortOrder.Descending)
		};

		private readonly IRecipientSession adSession;

		private ModernGroupNotificationLocator groupNotificationLocator;

		internal class NotifierData
		{
			public UnseenItemNotifier Notifier { get; private set; }

			public ExDateTime LastVisitedDateUTC { get; private set; }

			public NotifierData(UnseenItemNotifier notifier, ExDateTime lastVisitedDateUTC)
			{
				this.Notifier = notifier;
				this.LastVisitedDateUTC = lastVisitedDateUTC;
			}
		}
	}
}
