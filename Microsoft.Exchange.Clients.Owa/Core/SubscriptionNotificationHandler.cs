using System;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class SubscriptionNotificationHandler : NotificationHandlerBase
	{
		public SubscriptionNotificationHandler(UserContext userContext, MailboxSession mailboxSession) : base(userContext, mailboxSession)
		{
			this.payload = new SubscriptionPayload(userContext, mailboxSession, this);
			this.payload.RegisterWithPendingRequestNotifier();
		}

		internal override void HandleNotification(Notification notif)
		{
			if (Globals.ArePerfCountersEnabled)
			{
				OwaSingleCounters.TotalMailboxNotifications.Increment();
			}
			lock (this.syncRoot)
			{
				if (this.isDisposed || this.missedNotifications || this.needReinitSubscriptions)
				{
					return;
				}
			}
			QueryNotification queryNotification = notif as QueryNotification;
			if (queryNotification != null)
			{
				if (QueryNotificationType.RowDeleted != queryNotification.EventType)
				{
					if (queryNotification.Row.Length < SubscriptionNotificationHandler.querySubscriptionProperties.Length)
					{
						ExTraceGlobals.CoreCallTracer.TraceDebug<QueryNotificationType, int, int>((long)this.GetHashCode(), "notification with type {0} has {1} rows, less than expected {2} rows.", queryNotification.EventType, queryNotification.Row.Length, SubscriptionNotificationHandler.querySubscriptionProperties.Length);
						return;
					}
					for (int i = 0; i < SubscriptionNotificationHandler.querySubscriptionProperties.Length; i++)
					{
						if (queryNotification.Row[i] == null)
						{
							ExTraceGlobals.CoreCallTracer.TraceDebug<QueryNotificationType, int>((long)this.GetHashCode(), "notification with type {0} has row {1} equal to null.", queryNotification.EventType, i);
							return;
						}
					}
				}
				StringBuilder stringBuilder = new StringBuilder();
				bool flag2 = false;
				bool flag3 = false;
				try
				{
					this.userContext.Lock();
					flag2 = true;
					this.missedNotifications = false;
					Utilities.ReconnectStoreSession(this.mailboxSession, this.userContext);
					using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
					{
						this.UpdateSubscriptionCache(queryNotification, stringWriter);
					}
				}
				catch (OwaLockTimeoutException ex)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "User context lock timed out for notification thread. Exception: {0}", ex.Message);
					this.missedNotifications = true;
					flag3 = true;
				}
				catch (Exception ex2)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in HandleNewMailNotification on the notification thread. Exception: {0}", ex2.Message);
					this.missedNotifications = true;
					flag3 = true;
				}
				finally
				{
					if (this.userContext.LockedByCurrentThread() && flag2)
					{
						Utilities.DisconnectStoreSessionSafe(this.mailboxSession);
						this.userContext.Unlock();
					}
				}
				try
				{
					if (0 < stringBuilder.Length && !flag3)
					{
						this.payload.AddPayload(stringBuilder);
						this.payload.PickupData();
					}
				}
				catch (Exception ex3)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in HandleNewMailNotification on the notification thread. Exception: {0}", ex3.Message);
					this.missedNotifications = true;
				}
				return;
			}
		}

		internal override void HandlePendingGetTimerCallback()
		{
			lock (this.syncRoot)
			{
				if (this.isDisposed)
				{
					return;
				}
			}
			bool flag2 = false;
			bool flag3 = false;
			try
			{
				this.userContext.Lock();
				flag2 = true;
				Utilities.ReconnectStoreSession(this.mailboxSession, this.userContext);
				lock (this.syncRoot)
				{
					if (this.needReinitSubscriptions)
					{
						this.InitSubscription();
						this.needReinitSubscriptions = false;
						this.missedNotifications = true;
					}
					if (this.missedNotifications)
					{
						if (!RecipientCache.RunGetCacheOperationUnderExceptionHandler(delegate
						{
							SubscriptionCache.GetCache(this.userContext, false);
						}, new RecipientCache.ExceptionHandler(this.HandleSubscriptionLoadException), this.GetHashCode()))
						{
							return;
						}
					}
				}
			}
			catch (OwaLockTimeoutException ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "User context lock timed out in the pending GET timer callback. Exception: {0}", ex.Message);
				this.missedNotifications = true;
				flag3 = true;
			}
			catch (Exception ex2)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in pending GET timer callback thread. Exception: {0}", ex2.Message);
				this.missedNotifications = true;
				flag3 = true;
			}
			finally
			{
				if (this.userContext.LockedByCurrentThread() && flag2)
				{
					Utilities.DisconnectStoreSessionSafe(this.mailboxSession);
					this.userContext.Unlock();
				}
			}
			try
			{
				if (this.missedNotifications && !flag3)
				{
					StringBuilder stringBuilder = new StringBuilder();
					using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
					{
						SubscriptionNotificationHandler.RefreshClientCache(stringWriter, this.userContext.SubscriptionCache);
					}
					if (0 < stringBuilder.Length)
					{
						this.payload.AddPayload(stringBuilder);
						this.payload.PickupData();
					}
					this.missedNotifications = false;
				}
			}
			catch (Exception ex3)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in pending GET timer callback thread. Exception: {0}", ex3.Message);
				this.missedNotifications = true;
			}
		}

		protected override void InitSubscription()
		{
			lock (this.syncRoot)
			{
				if (this.mapiSubscription == null)
				{
					using (Folder folder = Folder.Bind(this.mailboxSession, DefaultFolderType.Inbox))
					{
						this.DisposeInternal();
						this.result = folder.ItemQuery(ItemQueryType.Associated, null, null, SubscriptionNotificationHandler.querySubscriptionProperties);
						this.result.GetRows(1);
						this.mapiSubscription = Subscription.Create(this.result, new NotificationHandler(this.HandleNotification));
					}
				}
			}
		}

		private static void RefreshClientCache(TextWriter writer, SubscriptionCache cache)
		{
			writer.Write("updSc(\"");
			cache.RenderToJavascript(writer);
			writer.WriteLine("\");");
		}

		private void UpdateSubscriptionCache(QueryNotification notification, TextWriter writer)
		{
			bool flag = null == this.userContext.SubscriptionCache;
			if (!RecipientCache.RunGetCacheOperationUnderExceptionHandler(delegate
			{
				SubscriptionCache.GetCache(this.userContext);
			}, new RecipientCache.ExceptionHandler(this.HandleSubscriptionLoadException), this.GetHashCode()))
			{
				return;
			}
			SubscriptionCache subscriptionCache = this.userContext.SubscriptionCache;
			if (flag)
			{
				SubscriptionNotificationHandler.RefreshClientCache(writer, subscriptionCache);
				return;
			}
			int num;
			if (QueryNotificationType.RowDeleted == notification.EventType)
			{
				num = subscriptionCache.Delete(notification.Index);
			}
			else
			{
				object obj = notification.Row[0];
				if (!(obj is Guid))
				{
					return;
				}
				Guid id = (Guid)obj;
				string text = notification.Row[1] as string;
				if (text == null)
				{
					return;
				}
				string text2 = notification.Row[2] as string;
				if (text2 == null)
				{
					return;
				}
				obj = notification.Row[3];
				if (!(obj is int))
				{
					return;
				}
				SendAsState sendAsState = (SendAsState)obj;
				obj = notification.Row[4];
				if (!(obj is int))
				{
					return;
				}
				AggregationStatus status = (AggregationStatus)obj;
				string address = Utilities.DecodeIDNDomain(text);
				SubscriptionCacheEntry entry = new SubscriptionCacheEntry(id, address, text2, notification.Index, this.mailboxSession.PreferedCulture);
				switch (notification.EventType)
				{
				case QueryNotificationType.RowAdded:
					if (!SubscriptionManager.IsValidForSendAs(sendAsState, status))
					{
						return;
					}
					num = subscriptionCache.Add(entry);
					goto IL_14B;
				case QueryNotificationType.RowModified:
					num = subscriptionCache.Modify(entry, sendAsState, status);
					goto IL_14B;
				}
				num = -1;
			}
			IL_14B:
			if (-1 < num)
			{
				SubscriptionNotificationHandler.RefreshClientCache(writer, subscriptionCache);
			}
		}

		private void HandleSubscriptionLoadException(Exception e, int hashCode)
		{
			ExTraceGlobals.CoreCallTracer.TraceError<string>((long)hashCode, "Failed to get subscription cache from server on the notification thread. Exception: {0}", e.Message);
			this.missedNotifications = true;
		}

		private static readonly PropertyDefinition[] querySubscriptionProperties = new PropertyDefinition[]
		{
			MessageItemSchema.SharingInstanceGuid,
			AggregationSubscriptionMessageSchema.SharingInitiatorSmtp,
			AggregationSubscriptionMessageSchema.SharingInitiatorName,
			MessageItemSchema.SharingSendAsState,
			AggregationSubscriptionMessageSchema.SharingAggregationStatus
		};

		private SubscriptionPayload payload;

		private enum QuerySubscriptionRow
		{
			Id,
			SmtpAddress,
			DisplayName,
			SendAsState,
			Status
		}

		private delegate void GenericOperation();
	}
}
