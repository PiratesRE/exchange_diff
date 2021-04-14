using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal abstract class NotificationHandlerBase : DisposeTrackableBase
	{
		internal MailboxSession MailboxSession
		{
			get
			{
				return this.mailboxSession;
			}
		}

		internal bool MissedNotifications
		{
			get
			{
				return this.missedNotifications;
			}
			set
			{
				this.missedNotifications = value;
			}
		}

		public NotificationHandlerBase(UserContext userContext, MailboxSession mailboxSession)
		{
			this.userContext = userContext;
			this.mailboxSession = mailboxSession;
			this.syncRoot = new object();
		}

		internal virtual void Subscribe()
		{
			try
			{
				this.userContext.Lock();
				this.InitSubscription();
			}
			catch (OwaLockTimeoutException ex)
			{
				ExTraceGlobals.CoreCallTracer.TraceDebug<string>((long)this.GetHashCode(), "User context lock timed out in Subscribe. Exception: {0}", ex.Message);
			}
			finally
			{
				if (this.userContext.LockedByCurrentThread())
				{
					this.userContext.Unlock();
				}
			}
		}

		internal abstract void HandleNotification(Notification notif);

		internal abstract void HandlePendingGetTimerCallback();

		protected abstract void InitSubscription();

		internal virtual void HandleConnectionDroppedNotification(Notification notification)
		{
			lock (this.syncRoot)
			{
				if (!this.isDisposed)
				{
					this.needReinitSubscriptions = true;
				}
			}
		}

		protected override void InternalDispose(bool isDisposing)
		{
			bool flag = false;
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "NotificationHandlerBase.Dispose. IsDisposing: {0}", isDisposing);
			lock (this.syncRoot)
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
				bool flag3 = false;
				try
				{
					this.userContext.Lock();
					if (!this.mailboxSession.IsConnected)
					{
						Utilities.ReconnectStoreSession(this.mailboxSession, this.userContext);
						flag3 = true;
					}
					this.DisposeInternal();
				}
				catch (Exception ex)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Unexpected exception in NotificationHandlerBase Dispose. Exception: {0}", ex.Message);
				}
				finally
				{
					if (flag3)
					{
						Utilities.DisconnectStoreSessionSafe(this.mailboxSession);
					}
					if (this.userContext.LockedByCurrentThread())
					{
						this.userContext.Unlock();
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationHandlerBase>(this);
		}

		internal virtual void DisposeInternal()
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("User context needs to be locked while this operation is called");
			}
			this.DisposeInternal(false);
		}

		internal virtual void DisposeInternal(bool doNotDisposeQueryResult)
		{
			if (!this.userContext.LockedByCurrentThread())
			{
				throw new InvalidOperationException("User context needs to be locked while this operation is called");
			}
			if (!doNotDisposeQueryResult && this.result != null)
			{
				OwaMapiNotificationHandler.DisposeXSOObjects(this.result);
				this.result = null;
			}
			if (this.mapiSubscription != null)
			{
				OwaMapiNotificationHandler.DisposeXSOObjects(this.mapiSubscription);
				this.mapiSubscription = null;
			}
		}

		protected Subscription mapiSubscription;

		protected MailboxSession mailboxSession;

		protected UserContext userContext;

		protected QueryResult result;

		protected object syncRoot;

		protected bool isDisposed;

		protected bool missedNotifications;

		protected bool needReinitSubscriptions;
	}
}
