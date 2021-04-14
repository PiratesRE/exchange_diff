using System;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal abstract class MapiNotificationHandlerBase : DisposeTrackableBase
	{
		internal event MapiNotificationHandlerBase.BeforeDisposeEventHandler OnBeforeDisposed;

		public MapiNotificationHandlerBase(IMailboxContext userContext, bool remoteSubscription) : this(null, userContext, remoteSubscription)
		{
		}

		public MapiNotificationHandlerBase(string subscriptionId, IMailboxContext userContext, bool remoteSubscription)
		{
			this.SubscriptionId = subscriptionId;
			this.userContext = userContext;
			this.connectionAliveTimerCount = 1;
			this.syncRoot = new object();
			this.remoteSubscription = remoteSubscription;
			if (!Globals.Owa2ServerUnitTestsHook)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "MapiNotificationHandlerBase.Constructor. Type: {0}, RemoteSusbcription: {1}.", base.GetType().Name, this.remoteSubscription);
				if ((this.remoteSubscription || this is ConnectionDroppedNotificationHandler) && this.userContext.NotificationManager != null)
				{
					this.userContext.NotificationManager.RemoteKeepAliveEvent += this.RemoteKeepAlive;
				}
				if (remoteSubscription)
				{
					if (this.userContext.NotificationManager != null)
					{
						this.userContext.NotificationManager.StartRemoteKeepAliveTimer();
					}
				}
				else
				{
					this.userContext.PendingRequestManager.KeepAlive += this.KeepAlive;
				}
				this.verboseLoggingEnabled = Globals.LogVerboseNotifications;
			}
		}

		public string SubscriptionId { get; protected set; }

		internal Subscription Subscription
		{
			get
			{
				return this.mapiSubscription;
			}
			set
			{
				this.mapiSubscription = value;
			}
		}

		internal IMailboxContext UserContext
		{
			get
			{
				return this.userContext;
			}
		}

		internal QueryResult QueryResult
		{
			get
			{
				return this.result;
			}
			set
			{
				this.result = value;
			}
		}

		internal bool NeedToReinitSubscriptions
		{
			get
			{
				return this.needReinitSubscriptions;
			}
			set
			{
				this.needReinitSubscriptions = value;
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

		internal bool NeedRefreshPayload { get; set; }

		internal bool RemoteSubscription
		{
			get
			{
				return this.remoteSubscription;
			}
		}

		protected object SyncRoot
		{
			get
			{
				return this.syncRoot;
			}
		}

		private protected bool IsDisposed_Reentrant
		{
			protected get
			{
				return this.isDisposed_reentrant;
			}
			private set
			{
				this.isDisposed_reentrant = value;
			}
		}

		protected string EventPrefix
		{
			get
			{
				if (this.eventPrefix == null)
				{
					this.eventPrefix = "MAPI." + base.GetType().Name + ".";
				}
				return this.eventPrefix;
			}
		}

		internal static void DisposeXSOObjects(object o, IMailboxContext userContext)
		{
			bool flag = false;
			try
			{
				if (!userContext.MailboxSessionLockedByCurrentThread())
				{
					ExTraceGlobals.UserContextTracer.TraceDebug(0, 0L, "MapiNotificationHandlerBase.DisposeXSOObjects(): Mailbox session not locked. Attempting to grab the lock.");
					userContext.LockAndReconnectMailboxSession(3000);
					flag = true;
				}
				IDisposable xsoObject = o as IDisposable;
				if (o != null)
				{
					try
					{
						OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
						{
							try
							{
								xsoObject.Dispose();
							}
							catch (StoragePermanentException ex2)
							{
								ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "MapiNotificationHandlerBase. Unable to dispose object.  exception {0}", ex2.Message);
							}
							catch (StorageTransientException ex3)
							{
								ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "MapiNotificationHandlerBase. Unable to dispose object.  exception {0}", ex3.Message);
							}
							catch (MapiExceptionObjectDisposed mapiExceptionObjectDisposed)
							{
								ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "MapiNotificationHandlerBase.Unable to dispose object.  exception {0}", mapiExceptionObjectDisposed.Message);
							}
							catch (ThreadAbortException ex4)
							{
								ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "MapiNotificationHandlerBase Unable to dispose object.  exception {0}", ex4.Message);
							}
							catch (ResourceUnhealthyException ex5)
							{
								ExTraceGlobals.UserContextTracer.TraceError<string>(0L, "MapiNotificationHandlerBase Unable to dispose object.  exception {0}", ex5.Message);
							}
						});
					}
					catch (GrayException ex)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "MapiNotificationHandlerBase.DisposeXSOObjects Unable to dispose object.  exception {0}", ex.Message);
					}
				}
			}
			finally
			{
				if (flag)
				{
					ExTraceGlobals.UserContextTracer.TraceDebug(0, 0L, "MapiNotificationHandlerBase.DisposeXSOObjects(): Attempting to release the lock taken.");
					userContext.UnlockAndDisconnectMailboxSession();
				}
			}
		}

		internal virtual void Subscribe()
		{
			lock (this.syncRoot)
			{
				if (base.IsDisposed)
				{
					throw new InvalidOperationException("Cannot call Subscribe on a Disposed object");
				}
				this.InitSubscription();
			}
		}

		internal abstract void HandleNotificationInternal(Notification notif, MapiNotificationsLogEvent logEvent, object context);

		internal abstract void HandlePendingGetTimerCallback(MapiNotificationsLogEvent logEvent);

		internal virtual void HandleConnectionDroppedNotification(Notification notification)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "MapiNotificationHandlerBase.HandleConnectionDroppedNotification. Type: {0}", base.GetType().Name);
			lock (this.syncRoot)
			{
				MapiNotificationsLogEvent logEvent = new MapiNotificationsLogEvent(this.UserContext.ExchangePrincipal, this.UserContext.Key.ToString(), this, this.EventPrefix + "HandleConnectionDroppedNotification");
				if (!this.IsDisposed_Reentrant)
				{
					this.needReinitSubscriptions = true;
				}
				if (this.ShouldLog(logEvent))
				{
					OwaServerTraceLogger.AppendToLog(logEvent);
				}
			}
		}

		internal void HandleNotification(Notification notification)
		{
			this.HandleNotification(notification, null);
		}

		internal void HandleNotification(Notification notification, object context)
		{
			MapiNotificationsLogEvent logEvent = new MapiNotificationsLogEvent(this.UserContext.ExchangePrincipal, this.UserContext.Key.ToString(), this, this.EventPrefix + "HandleNotification");
			try
			{
				if (base.IsDisposed)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "MapiNotificationHandlerBase.HandleNotification for {0}: Ignoring notification because we're disposed.", base.GetType().Name);
				}
				else if (this.MissedNotifications)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "MapiNotificationHandlerBase.HandleNotification for {0}: Ignoring notification because we've missed notifications.", base.GetType().Name);
				}
				else if (this.NeedToReinitSubscriptions)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "MapiNotificationHandlerBase.HandleNotification for {0}: Ignoring notification because we need to re-init subscription.", base.GetType().Name);
				}
				else
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						this.HandleNotificationInternal(notification, logEvent, context);
					});
				}
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string, string>((long)this.GetHashCode(), "MapiNotificationHandlerBase.HandleNotification for {0} encountered an exception: {1}", base.GetType().Name, ex.ToString());
				logEvent.HandledException = ex;
				this.MissedNotifications = true;
			}
			finally
			{
				if (this.ShouldLog(logEvent))
				{
					OwaServerTraceLogger.AppendToLog(logEvent);
				}
			}
		}

		internal virtual void DisposeSubscriptions()
		{
			this.DisposeSubscriptions(true);
		}

		internal virtual void DisposeSubscriptions(bool disposeQueryResult)
		{
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					try
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool, Type>((long)this.GetHashCode(), "MapiNotificationHandlerBase.DisposeInternal. doNotDisposeQueryResult: {0}, Type: {1}", disposeQueryResult, this.GetType());
						if (disposeQueryResult && this.result != null)
						{
							MapiNotificationHandlerBase.DisposeXSOObjects(this.result, this.UserContext);
							this.result = null;
						}
						if (this.mapiSubscription != null)
						{
							MapiNotificationHandlerBase.DisposeXSOObjects(this.mapiSubscription, this.UserContext);
							this.mapiSubscription = null;
						}
					}
					catch (StoragePermanentException ex2)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceError<string, SmtpAddress, Type>((long)this.GetHashCode(), "Unexpected exception in MapiNotificationHandlerBase Dispose. User: {1}. Exception: {0}, type: {2}", ex2.Message, this.UserContext.PrimarySmtpAddress, this.GetType());
					}
					catch (StorageTransientException ex3)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceError<string, SmtpAddress, Type>((long)this.GetHashCode(), "Unexpected exception in MapiNotificationHandlerBase Dispose. User: {1}. Exception: {0}, type: {2}", ex3.Message, this.UserContext.PrimarySmtpAddress, this.GetType());
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "MapiNotificationHandlerBase.Dispose Unable to dispose object.  exception {0}", ex.Message);
			}
		}

		protected abstract void InitSubscriptionInternal();

		protected override void InternalDispose(bool isDisposing)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool, SmtpAddress, Type>((long)this.GetHashCode(), "MapiNotificationHandlerBase.Dispose. IsDisposing: {0}, User: {1}, Type: {2}", isDisposing, this.UserContext.PrimarySmtpAddress, base.GetType());
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					lock (this.syncRoot)
					{
						if (this.OnBeforeDisposed != null)
						{
							ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "MapiNotificationHandlerBase.InternalDispose Call OnBeforeDisposed.");
							this.OnBeforeDisposed(new ConnectionDroppedNotificationHandler.ConnectionDroppedEventHandler(this.HandleConnectionDroppedNotification));
						}
						if ((this.remoteSubscription || this is ConnectionDroppedNotificationHandler) && this.userContext.NotificationManager != null)
						{
							this.userContext.NotificationManager.RemoteKeepAliveEvent -= this.RemoteKeepAlive;
						}
						if (!this.remoteSubscription && this.userContext.PendingRequestManager != null)
						{
							this.userContext.PendingRequestManager.KeepAlive -= this.KeepAlive;
						}
						if ((!Globals.Owa2ServerUnitTestsHook && !this.IsDisposed_Reentrant) || this.Subscription != null || this.QueryResult != null)
						{
							if (isDisposing)
							{
								this.IsDisposed_Reentrant = true;
								this.CleanupSubscriptions();
							}
						}
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "MapiNotificationHandlerBase.Dispose Unable to dispose object.  exception {0}", ex.Message);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MapiNotificationHandlerBase>(this);
		}

		protected void InitSubscription()
		{
			MapiNotificationsLogEvent logEvent = new MapiNotificationsLogEvent(this.UserContext.ExchangePrincipal, this.UserContext.Key.ToString(), this, this.EventPrefix + "InitSubscription");
			if (this.IsDisposed_Reentrant)
			{
				return;
			}
			try
			{
				this.userContext.LockAndReconnectMailboxSession(3000);
				this.NeedRefreshPayload = false;
				if (this.NeedToReinitSubscriptions)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<Type>((long)this.GetHashCode(), "MapiNotificationHandlerBase.InitSubscription need to cleanup subscription before reinit for type: {0}", base.GetType());
					this.CleanupSubscriptions();
					this.NeedRefreshPayload = true;
				}
				if (this.Subscription == null)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<Type, SmtpAddress>((long)this.GetHashCode(), "Notification Handler type: {0} needs to init subscriptions. User: {1}", base.GetType(), this.UserContext.PrimarySmtpAddress);
					if (this.QueryResult != null)
					{
						MapiNotificationHandlerBase.DisposeXSOObjects(this.QueryResult, this.UserContext);
					}
					this.QueryResult = null;
					this.InitSubscriptionInternal();
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<Type>((long)this.GetHashCode(), "MapiNotificationHandlerBase.InitSubscription subscription successfully initialized for type: {0}", base.GetType());
				}
				this.NeedToReinitSubscriptions = false;
			}
			finally
			{
				if (this.userContext.MailboxSessionLockedByCurrentThread())
				{
					this.userContext.UnlockAndDisconnectMailboxSession();
				}
				if (this.ShouldLog(logEvent))
				{
					OwaServerTraceLogger.AppendToLog(logEvent);
				}
			}
		}

		private void KeepAlive(object sender, EventArgs e)
		{
			int num = Interlocked.Increment(ref this.connectionAliveTimerCount);
			if (num % 5 == 0)
			{
				MapiNotificationsLogEvent logEvent = new MapiNotificationsLogEvent(this.UserContext.ExchangePrincipal, this.UserContext.Key.ToString(), this, this.EventPrefix + "KeepAlive");
				this.ProcessKeepAlive(logEvent);
			}
		}

		private void RemoteKeepAlive(object sender, EventArgs e)
		{
			MapiNotificationsLogEvent logEvent = new MapiNotificationsLogEvent(this.UserContext.ExchangePrincipal, this.UserContext.Key.ToString(), this, this.EventPrefix + "RemoteKeepAlive");
			this.ProcessKeepAlive(logEvent);
		}

		private void ProcessKeepAlive(MapiNotificationsLogEvent logEvent)
		{
			try
			{
				if (!base.IsDisposed)
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						this.HandlePendingGetTimerCallback(logEvent);
					});
				}
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string, string>((long)this.GetHashCode(), "MapiNotificationHandlerBase.KeepAlive for {0} encountered an exception: {1}", base.GetType().Name, ex.ToString());
				logEvent.HandledException = ex;
			}
			finally
			{
				if (this.ShouldLog(logEvent))
				{
					OwaServerTraceLogger.AppendToLog(logEvent);
				}
			}
		}

		private bool ShouldLog(MapiNotificationsLogEvent logEvent)
		{
			return this.verboseLoggingEnabled || logEvent.HandledException != null || base.IsDisposed;
		}

		private void CleanupSubscriptions()
		{
			if (this.Subscription != null)
			{
				MapiNotificationHandlerBase.DisposeXSOObjects(this.Subscription, this.UserContext);
			}
			this.Subscription = null;
			if (this.QueryResult != null)
			{
				MapiNotificationHandlerBase.DisposeXSOObjects(this.QueryResult, this.UserContext);
			}
			this.QueryResult = null;
		}

		private readonly bool verboseLoggingEnabled;

		private readonly bool remoteSubscription;

		private object syncRoot;

		private Subscription mapiSubscription;

		private IMailboxContext userContext;

		private QueryResult result;

		private bool isDisposed_reentrant;

		private bool missedNotifications;

		private bool needReinitSubscriptions;

		private int connectionAliveTimerCount;

		private string eventPrefix;

		internal delegate void BeforeDisposeEventHandler(ConnectionDroppedNotificationHandler.ConnectionDroppedEventHandler connectionDroppedEventHandler);
	}
}
