using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Clients.Owa.Premium;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class PendingRequestManager : DisposeTrackableBase
	{
		public event EventHandler<EventArgs> ClientDisconnected;

		public event EventHandler<EventArgs> KeepAlive;

		internal ChunkedHttpResponse ChunkedHttpResponse
		{
			get
			{
				return this.response;
			}
		}

		internal PendingRequestManager(UserContext userContext)
		{
			if (userContext == null)
			{
				throw new ArgumentNullException("userContext");
			}
			this.userContext = userContext;
			this.notifiersStateLock = new OwaRWLockWrapper();
			this.notifierDataAvaiableState = new Dictionary<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState>();
		}

		public static bool IsPendingGetRequired(UserContext userContext)
		{
			return userContext.IsInstantMessageEnabled() || PerformanceConsole.IsPerformanceConsoleEnabled(userContext) || userContext.IsPushNotificationsEnabled;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PendingRequestManager>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (!this.disposed)
			{
				if (isDisposing)
				{
					this.HandleFinishRequestFromClient();
					if (this.pendingRequestAliveTimer != null)
					{
						this.pendingRequestAliveTimer.Dispose();
						this.pendingRequestAliveTimer = null;
					}
					if (this.pendingRequestEventHandler != null && !this.pendingRequestEventHandler.IsDisposed && this.lockTracker.TryReleaseAllLocks(new PendingNotifierLockTracker.ReleaseAllLocksCallback(this.pendingRequestEventHandler.Dispose)))
					{
						this.pendingRequestEventHandler.Dispose();
					}
					this.pendingRequestEventHandler = null;
					LockCookie? lockCookie = null;
					try
					{
						try
						{
							this.notifiersStateLock.LockWriterElastic(5000);
						}
						catch (OwaLockTimeoutException)
						{
							ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Dispose was called but the writer lock is not available:");
							return;
						}
						foreach (KeyValuePair<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState> keyValuePair in this.notifierDataAvaiableState)
						{
							keyValuePair.Value.Dispose();
						}
					}
					finally
					{
						if (this.notifiersStateLock.IsWriterLockHeld)
						{
							if (lockCookie != null)
							{
								LockCookie value = lockCookie.Value;
								this.notifiersStateLock.DowngradeFromWriterLock(ref value);
							}
							else
							{
								this.notifiersStateLock.ReleaseWriterLock();
							}
						}
						if (lockCookie != null && !this.notifiersStateLock.IsReaderLockHeld)
						{
							ExAssert.RetailAssert(true, "Lost readerwriterlock that was acquired before entering the method");
						}
					}
				}
				this.disposed = true;
			}
		}

		internal void AddPendingRequestNotifier(IPendingRequestNotifier notifier)
		{
			LockCookie? lockCookie = null;
			try
			{
				if (notifier == null)
				{
					throw new ArgumentNullException("notifier");
				}
				this.notifiersStateLock.LockWriterElastic(5000);
				this.notifierDataAvaiableState.Add(notifier, new PendingRequestManager.PendingNotifierState());
				notifier.DataAvailable += this.OnNotifierDataAvailable;
			}
			finally
			{
				if (this.notifiersStateLock.IsWriterLockHeld)
				{
					if (lockCookie != null)
					{
						LockCookie value = lockCookie.Value;
						this.notifiersStateLock.DowngradeFromWriterLock(ref value);
					}
					else
					{
						this.notifiersStateLock.ReleaseWriterLock();
					}
				}
				if (lockCookie != null && !this.notifiersStateLock.IsReaderLockHeld)
				{
					ExAssert.RetailAssert(true, "Lost readerwriterlock that was acquired before entering the method");
				}
			}
		}

		internal void OnNotifierDataAvailable(object sender, EventArgs args)
		{
			bool flag = false;
			try
			{
				this.notifiersStateLock.LockReaderElastic(5000);
				flag = true;
				if (sender == null)
				{
					throw new ArgumentNullException("sender");
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "OnNotifierDataAvailable called by a notifier.Notifier:{0}", sender.GetHashCode());
				IPendingRequestNotifier key = (IPendingRequestNotifier)sender;
				PendingRequestManager.PendingNotifierState pendingNotifierState = null;
				if (!this.notifierDataAvaiableState.TryGetValue(key, out pendingNotifierState))
				{
					throw new ArgumentException("The sender object is not registered in the manager class");
				}
				int num = Interlocked.CompareExchange(ref pendingNotifierState.State, 1, 0);
				if (num != 0)
				{
					throw new OwaInvalidOperationException("OnNotifierDataAvailable should not be called if the manager did not consume the notifier's data yet. Notifier:" + sender.ToString());
				}
				if (this.lockTracker.TryAcquireLock())
				{
					this.WriteNotification(false);
				}
			}
			catch (Exception e)
			{
				this.HandleException(e, false);
			}
			finally
			{
				if (flag)
				{
					this.notifiersStateLock.ReleaseReaderLock();
				}
			}
		}

		internal IAsyncResult BeginSendNotification(AsyncCallback callback, object extraData, bool isUserContextFullyInitialized, PendingRequestEventHandler pendingRequestHandler)
		{
			bool flag = this.lockTracker.SetPipeAvailable(false);
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Setting the pipe to AVAILABLE");
			try
			{
				this.pendingRequestEventHandler = pendingRequestHandler;
				this.asyncResult = new OwaAsyncResult(callback, extraData);
				try
				{
					this.response = (ChunkedHttpResponse)extraData;
					this.response.WriteIsRequestAlive(true);
					if (!isUserContextFullyInitialized)
					{
						this.response.WriteReInitializeOWA();
					}
					this.lastWriteTime = DateTime.UtcNow.Ticks;
					this.disposePendingRequest = false;
				}
				finally
				{
					flag = !this.lockTracker.TryReleaseLock();
				}
				if (flag)
				{
					this.WriteNotification(true);
				}
				this.startPendingRequestTime = DateTime.UtcNow.Ticks;
				this.lastDisconnectedTime = 0L;
				if (this.pendingRequestAliveTimer == null)
				{
					this.pendingRequestAliveTimer = new Timer(new TimerCallback(this.ElapsedConnectionAliveTimeout), null, 40000, 40000);
				}
			}
			catch (Exception e)
			{
				this.HandleException(e, true);
			}
			return this.asyncResult;
		}

		internal void EndSendNotification(IAsyncResult async)
		{
			OwaAsyncResult owaAsyncResult = (OwaAsyncResult)async;
			if (!this.lockTracker.IsLockOwner())
			{
				throw new OwaInvalidOperationException("A thread that is not the owner of the lock can't call WriteNotification!", owaAsyncResult.Exception, this);
			}
			this.disposePendingRequest = false;
			this.response.WriteIsRequestAlive(false);
			if (owaAsyncResult.Exception != null)
			{
				throw new OwaNotificationPipeException("An exception happened while handling the pending connection asynchronously", owaAsyncResult.Exception);
			}
		}

		internal void RecordFinishPendingRequest()
		{
			this.lockTracker.SetPipeUnavailable();
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Setting the pipe to UNAVAILABLE");
		}

		internal bool HandleFinishRequestFromClient()
		{
			return this.HandleFinishRequestFromClient(false);
		}

		internal bool HandleFinishRequestFromClient(bool requestRestart)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<bool>((long)this.GetHashCode(), "The client requested the end of the current notification pipe.Should a restart request be sent ? {0}", requestRestart);
			this.disposePendingRequest = true;
			if (this.lockTracker.TryAcquireLockOnlyIfSucceed())
			{
				try
				{
					this.CloseCurrentPendingRequest(false, requestRestart);
				}
				finally
				{
					this.lockTracker.TryReleaseLock();
				}
				return true;
			}
			return false;
		}

		private void HandleException(Exception e, bool finishSync)
		{
			try
			{
				this.asyncResult.Exception = e;
			}
			catch (OwaInvalidOperationException)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception not reported on pending get request. Exception:{0};", e);
				return;
			}
			if (this.lockTracker.IsLockOwner())
			{
				this.asyncResult.CompleteRequest(finishSync);
				return;
			}
			if (this.lockTracker.TryAcquireLockOnlyIfSucceed())
			{
				try
				{
					this.asyncResult.CompleteRequest(finishSync);
					return;
				}
				finally
				{
					this.lockTracker.TryReleaseLock();
				}
			}
			this.disposePendingRequest = true;
		}

		private void WriteNotification(bool asyncOperation)
		{
			if (!this.lockTracker.IsLockOwner())
			{
				throw new OwaInvalidOperationException("A thread that is not the owner of the lock can't call WriteNotification!");
			}
			bool flag = false;
			bool flag2 = false;
			try
			{
				while (!flag)
				{
					if (this.disposePendingRequest)
					{
						this.CloseCurrentPendingRequest(asyncOperation, true);
						break;
					}
					this.notifiersStateLock.LockReaderElastic(5000);
					flag2 = true;
					TimeSpan value = new TimeSpan(DateTime.UtcNow.Ticks);
					double num = 0.0;
					if (this.startThrottleTime != null)
					{
						num = value.Subtract(this.startThrottleTime.Value).TotalSeconds;
					}
					if (this.startThrottleTime == null || num > 10.0)
					{
						this.startThrottleTime = new TimeSpan?(value);
					}
					foreach (KeyValuePair<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState> keyValuePair in this.notifierDataAvaiableState)
					{
						bool flag3 = false;
						int num2 = Interlocked.CompareExchange(ref keyValuePair.Value.State, 0, 1);
						if (num2 == 1)
						{
							if (keyValuePair.Key.ShouldThrottle)
							{
								int num3 = Interlocked.Increment(ref keyValuePair.Value.OnDataAvailableThrottleCount);
								if (num3 > 100)
								{
									flag3 = true;
								}
								else if (num3 == 100 && num <= 10.0)
								{
									ExTraceGlobals.NotificationsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Start throttling mechanism - timer was started and from now on notifier {0} will be on throttling mode ", keyValuePair.Key.GetHashCode());
									flag3 = true;
									if (keyValuePair.Value.ThrottleTimer == null)
									{
										keyValuePair.Value.ThrottleTimer = new Timer(new TimerCallback(this.ThrottleTimeout), keyValuePair.Key, 20000, -1);
									}
									else
									{
										keyValuePair.Value.ThrottleTimer.Change(20000, -1);
									}
								}
								if (num3 <= 100 && num > 10.0 && num3 != 1)
								{
									Interlocked.Exchange(ref keyValuePair.Value.OnDataAvailableThrottleCount, 1);
								}
							}
							ExTraceGlobals.NotificationsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "PendingRequestManager.WriteNotification is reading data from the notifier. Notifier:{0}", keyValuePair.Key.GetHashCode());
							if (!flag3)
							{
								try
								{
									string text = keyValuePair.Key.ReadDataAndResetState();
									if (!string.IsNullOrEmpty(text))
									{
										this.response.Write(text);
										this.lastWriteTime = DateTime.UtcNow.Ticks;
									}
								}
								catch (Exception ex)
								{
									this.lockTracker.TryReleaseLock();
									ExTraceGlobals.NotificationsCallTracer.TraceError<string>((long)this.GetHashCode(), "Exception when writing the notifications to the client. Exception message:{0};", (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
									throw;
								}
							}
							flag = this.lockTracker.TryReleaseLock();
							if (flag)
							{
								break;
							}
						}
					}
					this.notifiersStateLock.ReleaseReaderLock();
					flag2 = false;
				}
			}
			finally
			{
				if (flag2)
				{
					this.notifiersStateLock.ReleaseReaderLock();
				}
			}
		}

		private void ThrottleTimeout(object state)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Throttle Timeout method was called - throttle timeout elapsed");
			IPendingRequestNotifier pendingRequestNotifier = state as IPendingRequestNotifier;
			bool flag = false;
			try
			{
				if (pendingRequestNotifier == null)
				{
					throw new ArgumentException("State paramenter is invalid");
				}
				this.notifiersStateLock.LockReaderElastic(5000);
				flag = true;
				PendingRequestManager.PendingNotifierState pendingNotifierState = null;
				if (!this.notifierDataAvaiableState.TryGetValue(pendingRequestNotifier, out pendingNotifierState))
				{
					throw new ArgumentException("The sender object is not registered in the manager class");
				}
				Interlocked.Exchange(ref pendingNotifierState.OnDataAvailableThrottleCount, 0);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Notifier {0} is not on throttle period anymore", pendingRequestNotifier.GetHashCode());
				this.OnNotifierDataAvailable(pendingRequestNotifier, null);
			}
			catch (Exception e)
			{
				this.HandleException(e, false);
			}
			finally
			{
				if (flag)
				{
					this.notifiersStateLock.ReleaseReaderLock();
				}
			}
		}

		private void ElapsedConnectionAliveTimeout(object state)
		{
			bool requestRestart = false;
			OwaAsyncResult owaAsyncResult = this.asyncResult;
			EventHandler<EventArgs> keepAlive = this.KeepAlive;
			if (keepAlive != null)
			{
				keepAlive(this, new EventArgs());
			}
			if (DateTime.UtcNow.Ticks - this.startPendingRequestTime > PendingRequestManager.MaxPendingRequestOpenTimeIn100NanoSec)
			{
				this.disposePendingRequest = true;
				requestRestart = true;
			}
			try
			{
				if (this.lockTracker.TryAcquireLockOnlyIfSucceed())
				{
					try
					{
						if (this.lastWriteTime == 0L)
						{
							this.lastWriteTime = DateTime.UtcNow.Ticks;
						}
						bool flag = false;
						try
						{
							this.notifiersStateLock.LockReaderElastic(5000);
							flag = true;
							IPendingRequestNotifier[] array = this.notifierDataAvaiableState.Keys.ToArray<IPendingRequestNotifier>();
							this.notifiersStateLock.ReleaseReaderLock();
							flag = false;
							if (array != null)
							{
								for (int i = 0; i < array.Length; i++)
								{
									array[i].ConnectionAliveTimer();
								}
							}
						}
						finally
						{
							if (flag)
							{
								this.notifiersStateLock.ReleaseReaderLock();
							}
						}
						if (DateTime.UtcNow.Ticks - this.lastWriteTime >= 100000000L)
						{
							if (this.disposePendingRequest)
							{
								this.CloseCurrentPendingRequest(false, requestRestart);
								this.lockTracker.TryReleaseLock(owaAsyncResult.IsCompleted);
								return;
							}
							this.response.WriteEmptyNotification();
							this.lastWriteTime = DateTime.UtcNow.Ticks;
						}
					}
					catch (Exception e)
					{
						this.HandleException(e, false);
						this.lockTracker.TryReleaseLock(owaAsyncResult.IsCompleted);
						return;
					}
					if (!this.lockTracker.TryReleaseLock(owaAsyncResult.IsCompleted))
					{
						try
						{
							this.WriteNotification(false);
						}
						catch (Exception e2)
						{
							this.HandleException(e2, false);
						}
					}
				}
			}
			finally
			{
				if (!this.ChunkedHttpResponse.IsClientConnected)
				{
					if (this.lastDisconnectedTime == 0L)
					{
						this.lastDisconnectedTime = DateTime.UtcNow.Ticks;
						goto IL_23D;
					}
					if (DateTime.UtcNow.Ticks - this.lastDisconnectedTime <= 700000000L || this.userContext.IsUserRequestLockHeld)
					{
						goto IL_23D;
					}
					this.lastDisconnectedTime = 0L;
					try
					{
						EventArgs e3 = new EventArgs();
						EventHandler<EventArgs> clientDisconnected = this.ClientDisconnected;
						if (clientDisconnected != null)
						{
							clientDisconnected(this, e3);
						}
						goto IL_23D;
					}
					catch (Exception ex)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Exception during ClientDisconnected event: {0}", ex.ToString());
						goto IL_23D;
					}
				}
				this.lastDisconnectedTime = 0L;
				IL_23D:;
			}
		}

		private void CloseCurrentPendingRequest(bool completedSynchronously, bool requestRestart)
		{
			if (!this.lockTracker.IsLockOwner())
			{
				throw new OwaInvalidOperationException("A thread that is not the owner of the lock can't call WriteNotification!");
			}
			if (requestRestart)
			{
				try
				{
					this.response.RestartRequest();
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Send code to the client that will restart the notification pipe");
				}
				catch (OwaNotificationPipeWriteException)
				{
				}
			}
			this.asyncResult.CompleteRequest(completedSynchronously);
		}

		private const int LockTimeout = 5000;

		private const int EmptyNotificationIntervalInMilliSeconds = 40000;

		private const int MinEmptyNotificationIntervalInMilliSeconds = 10000;

		private const long TicksPerSecond = 10000000L;

		private const long TicksPerMillisecond = 10000L;

		private const int ThrottleIntervalInSec = 10;

		private const int MinimumWaitTimeForClientActivityInSeconds = 70;

		private const int ThrottleTimerCallbackInMilliSeconds = 20000;

		private const int ThrottlingThreshold = 100;

		private static readonly long MaxPendingRequestOpenTimeIn100NanoSec = (long)Globals.MaxPendingRequestLifeInSeconds * 10000000L;

		private readonly UserContext userContext;

		private TimeSpan? startThrottleTime;

		private Dictionary<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState> notifierDataAvaiableState;

		private OwaRWLockWrapper notifiersStateLock;

		private volatile bool disposePendingRequest;

		private OwaAsyncResult asyncResult;

		private ChunkedHttpResponse response;

		private long lastWriteTime;

		private long lastDisconnectedTime;

		private long startPendingRequestTime;

		private Timer pendingRequestAliveTimer;

		private PendingNotifierLockTracker lockTracker = new PendingNotifierLockTracker();

		private bool disposed;

		private PendingRequestEventHandler pendingRequestEventHandler;

		private class PendingNotifierState : DisposeTrackableBase
		{
			protected override void InternalDispose(bool isDisposing)
			{
				if (!this.disposed)
				{
					if (isDisposing && this.ThrottleTimer != null)
					{
						this.ThrottleTimer.Dispose();
						this.ThrottleTimer = null;
					}
					this.disposed = true;
				}
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<PendingRequestManager.PendingNotifierState>(this);
			}

			internal int State;

			internal int OnDataAvailableThrottleCount;

			internal Timer ThrottleTimer;

			private bool disposed;
		}
	}
}
