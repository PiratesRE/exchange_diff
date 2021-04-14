using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PendingRequestManager : DisposeTrackableBase
	{
		internal PendingRequestManager() : this(null, ListenerChannelsManager.Instance)
		{
		}

		internal PendingRequestManager(IMailboxContext userContext, ListenerChannelsManager listenerChannelsManager)
		{
			this.notifiersStateLock = new OwaRWLockWrapper();
			this.notifierDataAvailableState = new Dictionary<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState>();
			this.userContext = userContext;
			this.listenerChannelsManager = listenerChannelsManager;
		}

		public event EventHandler<EventArgs> ClientDisconnected;

		public event EventHandler<EventArgs> KeepAlive;

		internal bool ShouldDispose { get; set; }

		internal RemoteNotifier GetRemoteNotifier
		{
			get
			{
				if (this.remoteNotifier == null)
				{
					lock (this.syncRoot)
					{
						if (this.remoteNotifier == null)
						{
							this.remoteNotifier = this.CreateRemoteNotifier();
							this.AddPendingRequestNotifier(this.remoteNotifier);
						}
					}
				}
				return this.remoteNotifier;
			}
		}

		internal void AddPendingRequestNotifier(IPendingRequestNotifier notifier)
		{
			try
			{
				if (notifier == null)
				{
					throw new ArgumentNullException("notifier");
				}
				if (this.notifiersStateLock.LockWriterElastic(5000))
				{
					this.notifierDataAvailableState.Add(notifier, new PendingRequestManager.PendingNotifierState());
					notifier.DataAvailable += this.OnNotifierDataAvailable;
				}
			}
			finally
			{
				if (this.notifiersStateLock.IsWriterLockHeld)
				{
					this.notifiersStateLock.ReleaseWriterLock();
				}
			}
		}

		internal void RemovePendingRequestNotifier(IPendingRequestNotifier notifier)
		{
			try
			{
				if (notifier == null)
				{
					throw new ArgumentNullException("notifier");
				}
				if (this.notifiersStateLock.LockWriterElastic(5000))
				{
					this.notifierDataAvailableState.Remove(notifier);
					notifier.DataAvailable -= this.OnNotifierDataAvailable;
				}
			}
			finally
			{
				if (this.notifiersStateLock.IsWriterLockHeld)
				{
					this.notifiersStateLock.ReleaseWriterLock();
				}
			}
		}

		internal PendingRequestChannel AddPendingGetChannel(string channelId)
		{
			if (this.pendingRequestChannels != null)
			{
				lock (this.pendingRequestChannels)
				{
					if (this.pendingRequestChannels != null && this.pendingRequestChannels.Count <= 10 && !this.pendingRequestChannels.ContainsKey(channelId))
					{
						PendingRequestChannel pendingRequestChannel = new PendingRequestChannel(this, channelId);
						this.pendingRequestChannels.Add(channelId, pendingRequestChannel);
						this.listenerChannelsManager.AddPendingGetChannel(channelId, this);
						if (this.userContext.ExchangePrincipal != null && !string.IsNullOrEmpty(this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString()))
						{
							OwaServerLogger.AppendToLog(new PendingRequestChannelLogEvent(this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString(), channelId));
						}
						return pendingRequestChannel;
					}
				}
			}
			return null;
		}

		internal PendingRequestChannel GetPendingGetChannel(string channelId)
		{
			PendingRequestChannel result = null;
			if (this.pendingRequestChannels != null)
			{
				this.pendingRequestChannels.TryGetValue(channelId, out result);
			}
			return result;
		}

		internal void RemovePendingGetChannel(string channelId)
		{
			if (this.pendingRequestChannels != null)
			{
				lock (this.pendingRequestChannels)
				{
					PendingRequestChannel pendingGetChannel = this.GetPendingGetChannel(channelId);
					if (pendingGetChannel != null)
					{
						this.pendingRequestChannels.Remove(channelId);
						pendingGetChannel.Dispose();
					}
					this.listenerChannelsManager.RemovePendingGetChannel(channelId);
				}
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "[PendingRequestManager::RemovePendingGetChannel] ChannelId: {0}", channelId);
			try
			{
				OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
				{
					if (this.userContext.NotificationManager != null)
					{
						this.userContext.NotificationManager.ReleaseSubscriptionsForChannelId(channelId);
					}
				});
			}
			catch (GrayException ex)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "MapiNotificationHandlerBase.DisposeXSOObjects Unable to dispose object.  exception {0}", ex.Message);
			}
		}

		internal bool HasAnyActivePendingGetChannel()
		{
			if (this.pendingRequestChannels != null)
			{
				lock (this.pendingRequestChannels)
				{
					if (this.pendingRequestChannels != null)
					{
						foreach (KeyValuePair<string, PendingRequestChannel> keyValuePair in this.pendingRequestChannels)
						{
							if (keyValuePair.Value.IsActive)
							{
								return true;
							}
						}
					}
				}
				return false;
			}
			return false;
		}

		internal int GetChannelCount()
		{
			if (this.pendingRequestChannels != null)
			{
				lock (this.pendingRequestChannels)
				{
					if (this.pendingRequestChannels != null)
					{
						return this.pendingRequestChannels.Count;
					}
				}
				return 0;
			}
			return 0;
		}

		internal long GetMarkAndReset(string channelId)
		{
			long result = 0L;
			if (this.channelNotificationMarks != null && this.channelNotificationMarks.ContainsKey(channelId))
			{
				result = this.channelNotificationMarks[channelId];
			}
			this.channelNotificationMarks[channelId] = 0L;
			return result;
		}

		internal void FireKeepAlive()
		{
			EventHandler<EventArgs> keepAlive = this.KeepAlive;
			if (keepAlive != null)
			{
				keepAlive(this, new EventArgs());
			}
		}

		internal void FireClientDisconnect()
		{
			EventArgs e = new EventArgs();
			EventHandler<EventArgs> clientDisconnected = this.ClientDisconnected;
			if (clientDisconnected != null)
			{
				clientDisconnected(this, e);
			}
		}

		internal void OnNotifierDataAvailable(object sender, EventArgs args)
		{
			bool flag = false;
			try
			{
				if (!this.notifiersStateLock.IsReaderLockHeld)
				{
					this.notifiersStateLock.LockReaderElastic(5000);
					flag = true;
				}
				if (sender == null)
				{
					throw new ArgumentNullException("sender");
				}
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, int>((long)this.GetHashCode(), "OnNotifierDataAvailable called by {0} / {1}", sender.GetType().Name, sender.GetHashCode());
				IPendingRequestNotifier key = (IPendingRequestNotifier)sender;
				PendingRequestManager.PendingNotifierState pendingNotifierState = null;
				if (!this.notifierDataAvailableState.TryGetValue(key, out pendingNotifierState))
				{
					throw new ArgumentException("The sender object is not registered in the manager class");
				}
				int num = pendingNotifierState.CompareExchangeState(1, 0);
				if (num != 0)
				{
					throw new OwaInvalidOperationException("OnNotifierDataAvailable should not be called if the manager did not consume the notifier's data yet. Notifier:" + sender.ToString());
				}
				this.WriteNotification(false);
			}
			catch (Exception e)
			{
				this.HandleException(sender.GetType().Name + ".Error", e);
			}
			finally
			{
				if (flag)
				{
					this.notifiersStateLock.ReleaseReaderLock();
				}
			}
		}

		protected virtual RemoteNotifier CreateRemoteNotifier()
		{
			return new RemoteNotifier(this.userContext);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PendingRequestManager>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (!this.disposed && isDisposing)
			{
				try
				{
					OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
					{
						try
						{
							bool flag = false;
							try
							{
								flag = this.notifiersStateLock.LockWriterElastic(5000);
							}
							catch (OwaLockTimeoutException)
							{
								ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Dispose was called but the writer lock is not available:");
								return;
							}
							if (!flag)
							{
								ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "Dispose was called but the writer lock is not available:");
								return;
							}
							foreach (KeyValuePair<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState> keyValuePair in this.notifierDataAvailableState)
							{
								keyValuePair.Value.Dispose();
							}
							this.notifierDataAvailableState = null;
						}
						finally
						{
							if (this.notifiersStateLock.IsWriterLockHeld)
							{
								this.notifiersStateLock.ReleaseWriterLock();
							}
						}
						lock (this.pendingRequestChannels)
						{
							if (this.pendingRequestChannels != null)
							{
								foreach (string channelId in this.pendingRequestChannels.Keys)
								{
									PendingRequestChannel pendingGetChannel = this.GetPendingGetChannel(channelId);
									if (pendingGetChannel != null)
									{
										pendingGetChannel.Dispose();
									}
									this.listenerChannelsManager.RemovePendingGetChannel(channelId);
								}
							}
							this.pendingRequestChannels.Clear();
							this.pendingRequestChannels = null;
						}
						if (this.budget != null)
						{
							this.budget.Dispose();
							this.budget = null;
						}
					});
				}
				catch (GrayException ex)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceError<string>(0L, "MapiNotificationHandlerBase.DisposeXSOObjects Unable to dispose object.  exception {0}", ex.Message);
				}
				this.disposed = true;
			}
		}

		private void HandleException(string eventId, Exception e)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "There was an exception on the notification thread: {0}", e.ToString());
			OwaServerLogger.AppendToLog(new ExceptionLogEvent(eventId, this.userContext, e));
			if (this.pendingRequestChannels != null)
			{
				lock (this.pendingRequestChannels)
				{
					if (this.pendingRequestChannels != null)
					{
						foreach (KeyValuePair<string, PendingRequestChannel> keyValuePair in this.pendingRequestChannels)
						{
							keyValuePair.Value.HandleException(e, false);
						}
					}
				}
			}
		}

		private void WriteNotification(bool asyncOperation)
		{
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<SmtpAddress>((long)this.GetHashCode(), "Writing notifications for {0}.", this.userContext.ExchangePrincipal.MailboxInfo.PrimarySmtpAddress);
			bool flag = false;
			try
			{
				if (!this.notifiersStateLock.IsReaderLockHeld)
				{
					this.notifiersStateLock.LockReaderElastic(5000);
					flag = true;
				}
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
				foreach (KeyValuePair<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState> keyValuePair in this.notifierDataAvailableState)
				{
					IPendingRequestNotifier key = keyValuePair.Key;
					PendingRequestManager.PendingNotifierState value2 = keyValuePair.Value;
					int num2 = value2.CompareExchangeState(0, 1);
					if (num2 != 1)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "PendingRequestManager.WriteNotification is skipping notifier {0} / {1} with state {2}.", key.GetType().Name, key.GetHashCode(), num2);
					}
					else
					{
						this.WriteNotification(asyncOperation, num, key, value2);
					}
				}
				if (flag)
				{
					this.notifiersStateLock.ReleaseReaderLock();
					flag = false;
				}
			}
			finally
			{
				if (flag)
				{
					this.notifiersStateLock.ReleaseReaderLock();
				}
			}
		}

		private void WriteNotification(bool asyncOperation, double throttleInterval, IPendingRequestNotifier notifier, PendingRequestManager.PendingNotifierState notifierState)
		{
			bool flag = false;
			if (notifier.ShouldThrottle)
			{
				int num = notifierState.IncrementOnDataAvailableThrottleCount();
				if (num > 100)
				{
					flag = true;
				}
				else if (num == 100 && throttleInterval <= 10.0)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Start throttling mechanism - timer was started and from now on notifier {0} / {1} will be on throttling mode ", notifier.GetType().Name, notifier.GetHashCode());
					flag = true;
					if (notifierState.ThrottleTimer == null)
					{
						notifierState.ThrottleTimer = new Timer(new TimerCallback(this.ThrottleTimeout), notifier, 20000, -1);
					}
					else
					{
						notifierState.ThrottleTimer.Change(20000, -1);
					}
				}
				if (num <= 100 && throttleInterval > 10.0 && num != 1)
				{
					notifierState.ExchangeOnDataAvailableThrottleCount(1);
				}
			}
			if (flag)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, int>((long)this.GetHashCode(), "PendingRequestManager.WriteNotification throttled notifier: {0} / {1}", notifier.GetType().Name, notifier.GetHashCode());
				return;
			}
			ExTraceGlobals.NotificationsCallTracer.TraceDebug<string, int>((long)this.GetHashCode(), "PendingRequestManager.WriteNotification is reading data from the notifier. Notifier: {0} / {1}", notifier.GetType().Name, notifier.GetHashCode());
			try
			{
				List<NotificationPayloadBase> payloadList = (List<NotificationPayloadBase>)notifier.ReadDataAndResetState();
				if (notifier.SubscriptionId != null)
				{
					Pusher.Instance.Distribute(payloadList, notifier.ContextKey, notifier.SubscriptionId);
				}
				if (this.pendingRequestChannels != null)
				{
					lock (this.pendingRequestChannels)
					{
						if (this.pendingRequestChannels != null)
						{
							if (this.budget == null)
							{
								this.budget = StandardBudget.Acquire(this.userContext.ExchangePrincipal.Sid, BudgetType.Owa, this.userContext.ExchangePrincipal.MailboxInfo.OrganizationId.ToADSessionSettings());
							}
							this.budget.CheckOverBudget();
							this.budget.StartLocal("PendingRequestManager.WriteNotification", default(TimeSpan));
							try
							{
								using (Dictionary<string, PendingRequestChannel>.Enumerator enumerator = this.pendingRequestChannels.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										KeyValuePair<string, PendingRequestChannel> channel = enumerator.Current;
										try
										{
											OwaDiagnostics.SendWatsonReportsForGrayExceptions(delegate()
											{
												KeyValuePair<string, PendingRequestChannel> channel = channel;
												channel.Value.WritePayload(asyncOperation, payloadList);
												Dictionary<string, long> dictionary = this.channelNotificationMarks;
												KeyValuePair<string, PendingRequestChannel> channel2 = channel;
												if (dictionary.ContainsKey(channel2.Key))
												{
													Dictionary<string, long> dictionary3;
													Dictionary<string, long> dictionary2 = dictionary3 = this.channelNotificationMarks;
													KeyValuePair<string, PendingRequestChannel> channel3 = channel;
													string key;
													dictionary2[key = channel3.Key] = dictionary3[key] + (long)payloadList.Count;
													return;
												}
												Dictionary<string, long> dictionary4 = this.channelNotificationMarks;
												KeyValuePair<string, PendingRequestChannel> channel4 = channel;
												dictionary4.Add(channel4.Key, (long)payloadList.Count);
											});
										}
										catch (GrayException ex)
										{
											Exception ex2 = (ex.InnerException != null) ? ex.InnerException : ex;
											ExTraceGlobals.NotificationsCallTracer.TraceError((long)this.GetHashCode(), "Exception when writing the notifications to the client. Notifier {0} / {1}, exception message: {2}, stack: {3};", new object[]
											{
												notifier.GetType().Name,
												notifier.GetHashCode(),
												ex2.Message,
												ex2.StackTrace
											});
										}
									}
								}
							}
							finally
							{
								this.budget.EndLocal();
							}
						}
					}
				}
			}
			catch (Exception ex3)
			{
				ExTraceGlobals.NotificationsCallTracer.TraceError<string, int, string>((long)this.GetHashCode(), "Exception when writing the notifications to the client. Notifier {0} / {1}, exception message: {2};", notifier.GetType().Name, notifier.GetHashCode(), (ex3.InnerException != null) ? ex3.InnerException.Message : ex3.Message);
				throw;
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
					throw new ArgumentException("State parameter is invalid");
				}
				if (!this.notifiersStateLock.IsReaderLockHeld)
				{
					this.notifiersStateLock.LockReaderElastic(5000);
					flag = true;
				}
				PendingRequestManager.PendingNotifierState pendingNotifierState = null;
				if (!this.notifierDataAvailableState.TryGetValue(pendingRequestNotifier, out pendingNotifierState))
				{
					throw new ArgumentException("The sender object is not registered in the manager class");
				}
				pendingNotifierState.ExchangeOnDataAvailableThrottleCount(0);
				ExTraceGlobals.NotificationsCallTracer.TraceDebug<int>((long)this.GetHashCode(), "Notifier {0} is not on throttle period anymore", pendingRequestNotifier.GetHashCode());
				this.OnNotifierDataAvailable(pendingRequestNotifier, null);
			}
			catch (Exception e)
			{
				this.HandleException("NotificationThrottleTimeout", e);
			}
			finally
			{
				if (flag)
				{
					this.notifiersStateLock.ReleaseReaderLock();
				}
			}
		}

		private const int LockTimeout = 5000;

		private const int ThrottleIntervalInSec = 10;

		private const int ThrottleTimerCallbackInMilliSeconds = 20000;

		private const int ThrottlingThreshold = 100;

		private const int MaxChannels = 10;

		private readonly object syncRoot = new object();

		private IStandardBudget budget;

		private TimeSpan? startThrottleTime;

		private Dictionary<IPendingRequestNotifier, PendingRequestManager.PendingNotifierState> notifierDataAvailableState;

		private OwaRWLockWrapper notifiersStateLock;

		private Dictionary<string, PendingRequestChannel> pendingRequestChannels = new Dictionary<string, PendingRequestChannel>();

		private Dictionary<string, long> channelNotificationMarks = new Dictionary<string, long>();

		private bool disposed;

		private IMailboxContext userContext;

		private ListenerChannelsManager listenerChannelsManager;

		private RemoteNotifier remoteNotifier;

		private class PendingNotifierState : DisposeTrackableBase
		{
			public Timer ThrottleTimer
			{
				get
				{
					return this.throttleTimer;
				}
				set
				{
					this.throttleTimer = value;
				}
			}

			internal int CompareExchangeState(int value, int comparand)
			{
				return Interlocked.CompareExchange(ref this.state, value, comparand);
			}

			internal int ExchangeOnDataAvailableThrottleCount(int value)
			{
				return Interlocked.Exchange(ref this.onDataAvailableThrottleCount, value);
			}

			internal int IncrementOnDataAvailableThrottleCount()
			{
				return Interlocked.Increment(ref this.onDataAvailableThrottleCount);
			}

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

			private int state;

			private int onDataAvailableThrottleCount;

			private Timer throttleTimer;

			private bool disposed;
		}
	}
}
