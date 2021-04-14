using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class ConnectionDroppedNotificationHandler : DisposeTrackableBase
	{
		public ConnectionDroppedNotificationHandler(UserContext userContext, MailboxSession mailboxSession)
		{
			this.userContext = userContext;
			this.mailboxSession = mailboxSession;
			ConnectionDroppedPayload connectionDroppedPayload = new ConnectionDroppedPayload(userContext, mailboxSession, this);
			connectionDroppedPayload.RegisterWithPendingRequestNotifier();
		}

		internal event ConnectionDroppedNotificationHandler.ConnectionDroppedEventHandler OnConnectionDropped;

		protected override void InternalDispose(bool isDisposing)
		{
			bool flag = false;
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "ConnectionDroppedNotificationHandler.Dispose. IsDisposing: {0}", isDisposing);
			lock (this)
			{
				if (this.isDisposed)
				{
					return;
				}
				if (isDisposing)
				{
					this.isDisposed = true;
					flag = true;
				}
			}
			if (flag)
			{
				this.DisposeInternal();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ConnectionDroppedNotificationHandler>(this);
		}

		public void Subscribe()
		{
			this.InitSubscription();
		}

		internal void HandleNotification(Notification notification)
		{
			lock (this)
			{
				if (this.isDisposed || this.needReinitSubscription)
				{
					return;
				}
				this.needReinitSubscription = true;
			}
			if (this.OnConnectionDropped != null)
			{
				this.OnConnectionDropped(notification);
			}
		}

		internal void HandlePendingGetTimerCallback()
		{
			bool flag = false;
			lock (this)
			{
				if (this.isDisposed)
				{
					return;
				}
				flag = this.needReinitSubscription;
			}
			if (flag)
			{
				try
				{
					this.userContext.Lock();
					Utilities.ReconnectStoreSession(this.mailboxSession, this.userContext);
					lock (this)
					{
						if (this.needReinitSubscription)
						{
							this.DisposeInternal();
							this.InitSubscription();
							this.needReinitSubscription = false;
						}
					}
				}
				catch (Exception ex)
				{
					ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in pending GET timer callback thread. Exception: {0}", ex.Message);
					this.needReinitSubscription = true;
				}
				finally
				{
					if (this.userContext.LockedByCurrentThread())
					{
						Utilities.DisconnectStoreSessionSafe(this.mailboxSession);
						this.userContext.Unlock();
					}
				}
			}
		}

		private void InitSubscription()
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("UserContext lock should be acquired before calling this method");
			}
			lock (this)
			{
				if (this.mapiSubscription != null)
				{
					throw new InvalidOperationException("There is an existing undisposed subscription. Dispose it before creating a new one");
				}
				this.mapiSubscription = Subscription.CreateMailboxSubscription(this.mailboxSession, new NotificationHandler(this.HandleNotification), NotificationType.ConnectionDropped);
			}
		}

		private void DisposeInternal()
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("User context needs to be locked while this operation is called");
			}
			if (this.mapiSubscription != null)
			{
				OwaMapiNotificationHandler.DisposeXSOObjects(this.mapiSubscription);
				this.mapiSubscription = null;
			}
		}

		private Subscription mapiSubscription;

		private MailboxSession mailboxSession;

		private UserContext userContext;

		private bool isDisposed;

		private bool needReinitSubscription;

		internal delegate void ConnectionDroppedEventHandler(Notification notification);
	}
}
