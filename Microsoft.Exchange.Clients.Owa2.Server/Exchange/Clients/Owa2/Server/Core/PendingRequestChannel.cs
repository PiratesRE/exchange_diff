using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class PendingRequestChannel : DisposeTrackableBase
	{
		public PendingRequestChannel(PendingRequestManager pendingRequestManager, string channelId)
		{
			this.pendingRequestManager = pendingRequestManager;
			this.channelId = channelId;
			this.notificationMark = 0L;
		}

		internal ChunkedHttpResponse ChunkedHttpResponse
		{
			get
			{
				return this.response;
			}
		}

		internal bool IsActive
		{
			get
			{
				return this.response != null && this.response.IsClientConnected;
			}
		}

		internal bool ShouldBeFinalized
		{
			get
			{
				return this.checkClientInactiveCounter > 2 && this.pendingRequestManager.GetChannelCount() > 1;
			}
		}

		internal long MaxTicksPerPendingRequest
		{
			get
			{
				return this.maxTicksPerPendingRequest;
			}
			set
			{
				this.maxTicksPerPendingRequest = value;
			}
		}

		internal IAsyncResult BeginSendNotification(AsyncCallback callback, object extraData, PendingRequestEventHandler pendingRequestHandler, bool hierarchySubscriptionActive, string channelId)
		{
			bool flag = this.lockTracker.SetPipeAvailable(false);
			ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[PendingRequestChannel.BeginSendNotification] Setting the pipe to AVAILABLE");
			try
			{
				this.pendingRequestEventHandler = pendingRequestHandler;
				this.asyncResult = new OwaAsyncResult(callback, extraData, channelId);
				try
				{
					this.response = (ChunkedHttpResponse)extraData;
					this.WriteIsRequestAlive(true);
					this.notificationMark = 0L;
					if (!hierarchySubscriptionActive)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "[PendingRequestChannel.BeginSendNotification] hierarchySubscriptionActive is false");
						this.WriteReinitializeSubscriptions();
					}
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
				if (this.accountValidationTimer == null && this.response.AccountValidationContext != null)
				{
					this.accountValidationTimer = new Timer(new TimerCallback(this.AccountValidationTimerCallback), null, 300000, 300000);
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
			this.WriteIsRequestAlive(false);
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

		internal void AddPayload(List<NotificationPayloadBase> payloadList)
		{
			lock (this.syncRoot)
			{
				if (this.payloadList != null)
				{
					if (this.reloadNeeded)
					{
						NotificationStatisticsManager.Instance.NotificationDropped(payloadList, NotificationState.Dispatching);
					}
					else if (this.payloadList.Count < 250)
					{
						this.payloadList.AddRange(payloadList);
					}
					else
					{
						this.reloadNeeded = true;
						this.notificationMark += (long)this.payloadList.Count;
						this.payloadList.Clear();
					}
				}
			}
		}

		internal void WritePayload(bool asyncOperation, List<NotificationPayloadBase> payloadList)
		{
			this.AddPayload(payloadList);
			if (this.lockTracker.TryAcquireLock())
			{
				this.WriteNotification(asyncOperation);
			}
		}

		internal void HandleException(Exception e, bool finishSync)
		{
			if (this.disposePendingRequest)
			{
				return;
			}
			if (this.asyncResult == null)
			{
				return;
			}
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

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PendingRequestChannel>(this);
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (!this.disposed && isDisposing)
			{
				if (this.pendingRequestAliveTimer != null)
				{
					this.pendingRequestAliveTimer.Dispose();
					this.pendingRequestAliveTimer = null;
				}
				if (this.accountValidationTimer != null)
				{
					this.accountValidationTimer.Dispose();
					this.accountValidationTimer = null;
				}
				lock (this.syncRoot)
				{
					if (this.payloadList != null)
					{
						this.payloadList.Clear();
						this.payloadList = null;
					}
				}
				if (this.pendingRequestEventHandler != null && !this.pendingRequestEventHandler.IsDisposed && this.lockTracker.TryReleaseAllLocks(new PendingNotifierLockTracker.ReleaseAllLocksCallback(this.pendingRequestEventHandler.Dispose)))
				{
					this.pendingRequestEventHandler.Dispose();
				}
				this.pendingRequestEventHandler = null;
			}
			this.disposed = true;
		}

		private void WriteNotification(bool asyncOperation)
		{
			if (!this.lockTracker.IsLockOwner())
			{
				throw new OwaInvalidOperationException("A thread that is not the owner of the lock can't call WriteNotification!");
			}
			bool flag = false;
			while (!flag)
			{
				if (this.disposePendingRequest)
				{
					this.CloseCurrentPendingRequest(asyncOperation, true);
					return;
				}
				try
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("[");
					lock (this.syncRoot)
					{
						if (this.payloadList != null)
						{
							if (!this.reloadNeeded)
							{
								foreach (NotificationPayloadBase notificationPayloadBase in this.payloadList)
								{
									RemoteNotificationPayload remoteNotificationPayload = notificationPayloadBase as RemoteNotificationPayload;
									if (remoteNotificationPayload != null)
									{
										stringBuilder.Append(remoteNotificationPayload.RemotePayload).Append(",");
										this.notificationMark += (long)remoteNotificationPayload.NotificationsCount;
									}
									else
									{
										stringBuilder.Append(JsonConverter.ToJSON(notificationPayloadBase)).Append(",");
										this.notificationMark += 1L;
									}
								}
								if (stringBuilder.Length > 1)
								{
									stringBuilder.Remove(stringBuilder.Length - 1, 1);
									stringBuilder.Append("]");
									this.Write(stringBuilder.ToString());
								}
								NotificationStatisticsManager.Instance.NotificationDispatched(this.channelId, this.payloadList);
								this.payloadList.Clear();
							}
							else
							{
								ReloadAllNotificationPayload payload = new ReloadAllNotificationPayload
								{
									Source = new TypeLocation(base.GetType())
								};
								NotificationStatisticsManager.Instance.NotificationCreated(payload);
								NotificationStatisticsManager.Instance.NotificationDispatched(this.channelId, payload);
							}
							this.reloadNeeded = false;
							this.WriteNotificationMark(this.notificationMark);
						}
					}
				}
				finally
				{
					flag = this.lockTracker.TryReleaseLock();
				}
				if (flag)
				{
					return;
				}
			}
		}

		private void AccountValidationTimerCallback(object state)
		{
			if (this.response.AccountValidationContext != null)
			{
				AccountState accountState = this.response.AccountValidationContext.CheckAccount();
				if (accountState != AccountState.AccountEnabled)
				{
					ExTraceGlobals.NotificationsCallTracer.TraceDebug((long)this.GetHashCode(), "The account is no longer in an 'Enabled' state");
					this.disposePendingRequest = true;
					if (this.lockTracker.TryAcquireLockOnlyIfSucceed())
					{
						try
						{
							this.CloseCurrentPendingRequest(false, true);
						}
						finally
						{
							this.lockTracker.TryReleaseLock();
						}
					}
				}
			}
		}

		private void ElapsedConnectionAliveTimeout(object state)
		{
			bool requestRestart = false;
			OwaAsyncResult owaAsyncResult = this.asyncResult;
			this.pendingRequestManager.FireKeepAlive();
			if (DateTime.UtcNow.Ticks - this.startPendingRequestTime > this.MaxTicksPerPendingRequest)
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
						if (DateTime.UtcNow.Ticks - this.lastWriteTime >= 100000000L)
						{
							if (this.disposePendingRequest)
							{
								this.CloseCurrentPendingRequest(false, requestRestart);
								this.lockTracker.TryReleaseLock(owaAsyncResult.IsCompleted);
								return;
							}
							this.WriteEmptyNotification();
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
				if (this.IsActive)
				{
					this.checkClientInactiveCounter = 0;
				}
				else
				{
					this.checkClientInactiveCounter++;
				}
				if (this.ShouldBeFinalized)
				{
					this.pendingRequestManager.RemovePendingGetChannel(this.channelId);
				}
				if (!this.pendingRequestManager.HasAnyActivePendingGetChannel())
				{
					if (this.lastDisconnectedTime == 0L)
					{
						this.lastDisconnectedTime = DateTime.UtcNow.Ticks;
						goto IL_1A9;
					}
					if (DateTime.UtcNow.Ticks - this.lastDisconnectedTime <= 700000000L)
					{
						goto IL_1A9;
					}
					this.lastDisconnectedTime = 0L;
					try
					{
						this.pendingRequestManager.FireClientDisconnect();
						goto IL_1A9;
					}
					catch (Exception ex)
					{
						ExTraceGlobals.NotificationsCallTracer.TraceDebug<string>((long)this.GetHashCode(), "Exception during ClientDisconnected event: {0}", ex.ToString());
						goto IL_1A9;
					}
				}
				this.lastDisconnectedTime = 0L;
				IL_1A9:;
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

		private void WriteIsRequestAlive(bool isAlive)
		{
			this.response.WriteIsRequestAlive(isAlive, this.notificationMark);
			this.lastWriteTime = DateTime.UtcNow.Ticks;
		}

		private void WriteReinitializeSubscriptions()
		{
			this.response.WriteReinitializeSubscriptions();
			this.lastWriteTime = DateTime.UtcNow.Ticks;
		}

		private void WriteNotificationMark(long mark)
		{
			this.response.WritePendingGeMark(mark);
			this.lastWriteTime = DateTime.UtcNow.Ticks;
		}

		private void WriteEmptyNotification()
		{
			this.response.WriteEmptyNotification();
			this.lastWriteTime = DateTime.UtcNow.Ticks;
		}

		private void Write(string notification)
		{
			this.response.Write(notification);
			this.lastWriteTime = DateTime.UtcNow.Ticks;
		}

		private const int AccountValidationIntervalInMilliSeconds = 300000;

		private const int EmptyNotificationIntervalInMilliSeconds = 40000;

		private const int MinEmptyNotificationIntervalInMilliSeconds = 10000;

		private const int MinimumWaitTimeForClientActivityInSeconds = 70;

		private const int MaxPayloadThreshold = 250;

		private const int MaxChecksBeforeFinalize = 2;

		private static readonly long DefaultMaxTicksPerPendingRequest = (long)((ulong)-1294967296);

		private readonly object syncRoot = new object();

		private readonly string channelId;

		private volatile bool disposePendingRequest;

		private OwaAsyncResult asyncResult;

		private ChunkedHttpResponse response;

		private long lastWriteTime;

		private long lastDisconnectedTime;

		private long startPendingRequestTime;

		private int checkClientInactiveCounter;

		private Timer pendingRequestAliveTimer;

		private Timer accountValidationTimer;

		private PendingNotifierLockTracker lockTracker = new PendingNotifierLockTracker();

		private long maxTicksPerPendingRequest = PendingRequestChannel.DefaultMaxTicksPerPendingRequest;

		private List<NotificationPayloadBase> payloadList = new List<NotificationPayloadBase>();

		private bool reloadNeeded;

		private PendingRequestManager pendingRequestManager;

		private bool disposed;

		private PendingRequestEventHandler pendingRequestEventHandler;

		private long notificationMark;
	}
}
