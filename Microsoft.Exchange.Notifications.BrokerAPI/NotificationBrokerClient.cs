using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Notifications.Broker;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.NotificationsBroker;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Notifications.Broker
{
	internal class NotificationBrokerClient : DisposeTrackableBase, INotificationBrokerClient, IDisposable
	{
		public NotificationBrokerClient() : this(Consumer.Current.ConsumerId)
		{
		}

		public NotificationBrokerClient(ConsumerId consumerId)
		{
			this.consumerId = consumerId;
			this.logger = NotificationBrokerClient.GetDefaultLogger();
		}

		public void Subscribe(BrokerSubscription subscription)
		{
			ArgumentValidator.ThrowIfNull("subscription", subscription);
			this.LogLatency("Subscribe", delegate(NotificationBrokerClientLogEvent logEvent)
			{
				this.SubscribeInternal(subscription, logEvent);
			});
		}

		public void Unsubscribe(BrokerSubscription subscription)
		{
			ArgumentValidator.ThrowIfNull("subscription", subscription);
			this.LogLatency("Unsubscribe", delegate(NotificationBrokerClientLogEvent logEvent)
			{
				this.UnsubscribeInternal(subscription, logEvent);
			});
		}

		public void StartNotificationCallbacks(Action<BrokerNotification> notificationCallback)
		{
			ArgumentValidator.ThrowIfNull("notificationCallback", notificationCallback);
			this.LogLatency("StartNotificationCallbacks", delegate(NotificationBrokerClientLogEvent logEvent)
			{
				this.StartNotificationCallbacksInternal(notificationCallback, logEvent);
			});
		}

		public void StopNotificationCallbacks()
		{
			this.LogLatency("StopNotificationCallbacks", delegate(NotificationBrokerClientLogEvent logEvent)
			{
				this.StopNotificationCallbacksInternal(logEvent);
			});
		}

		protected override void InternalDispose(bool isDisposing)
		{
			if (isDisposing)
			{
				lock (this.mutex)
				{
					this.StopNotificationCallbacks();
					while (this.rpcClientPool.Count > 0)
					{
						this.rpcClientPool.Dequeue().Dispose();
					}
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<NotificationBrokerClient>(this);
		}

		private static IExtensibleLogger GetDefaultLogger()
		{
			if (NotificationBrokerClient.defaultLogger == null)
			{
				lock (NotificationBrokerClient.defaultLoggerLock)
				{
					if (NotificationBrokerClient.defaultLogger == null)
					{
						NotificationBrokerClient.defaultLogger = new NotificationBrokerClientLogger();
						AppDomain.CurrentDomain.DomainUnload += NotificationBrokerClient.DisposeDefaultLogger;
					}
				}
			}
			return NotificationBrokerClient.defaultLogger;
		}

		private static void DisposeDefaultLogger(object sender, EventArgs e)
		{
			lock (NotificationBrokerClient.defaultLoggerLock)
			{
				if (NotificationBrokerClient.defaultLogger != null)
				{
					NotificationBrokerClient.defaultLogger.Dispose();
				}
			}
		}

		private static void GetNextNotificationCallback(IAsyncResult asyncResult)
		{
			NotificationBrokerClient notificationBrokerClient = (NotificationBrokerClient)asyncResult.AsyncState;
			notificationBrokerClient.OnNotification(asyncResult);
		}

		private void LogLatency(string action, Action<NotificationBrokerClientLogEvent> operation)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("action", action);
			NotificationBrokerClientLogEvent notificationBrokerClientLogEvent = new NotificationBrokerClientLogEvent(this.consumerId, action);
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				operation(notificationBrokerClientLogEvent);
			}
			finally
			{
				notificationBrokerClientLogEvent.Latency = stopwatch.ElapsedMilliseconds;
				this.logger.LogEvent(notificationBrokerClientLogEvent);
			}
		}

		private void SubscribeInternal(BrokerSubscription subscription, NotificationBrokerClientLogEvent logEvent)
		{
			logEvent.SubscriptionId = new Guid?(subscription.SubscriptionId);
			NotificationsBrokerRpcClient notificationsBrokerRpcClient = null;
			try
			{
				notificationsBrokerRpcClient = this.GetRpcClient();
				string subscription2 = subscription.ToJson();
				ICancelableAsyncResult asyncResult = notificationsBrokerRpcClient.BeginSubscribe(null, null, subscription2, null, null);
				BrokerStatus brokerStatus = notificationsBrokerRpcClient.EndSubscribe(asyncResult);
				logEvent.Status = new BrokerStatus?(brokerStatus);
				if (brokerStatus != BrokerStatus.Success)
				{
					throw new NotificationsBrokerStatusException(brokerStatus);
				}
			}
			catch (Exception exception)
			{
				logEvent.Exception = exception;
				if (notificationsBrokerRpcClient != null)
				{
					notificationsBrokerRpcClient.Dispose();
					notificationsBrokerRpcClient = null;
				}
				throw;
			}
			finally
			{
				if (notificationsBrokerRpcClient != null)
				{
					this.ReleaseRpcClient(notificationsBrokerRpcClient);
				}
			}
		}

		private void UnsubscribeInternal(BrokerSubscription subscription, NotificationBrokerClientLogEvent logEvent)
		{
			logEvent.SubscriptionId = new Guid?(subscription.SubscriptionId);
			NotificationsBrokerRpcClient notificationsBrokerRpcClient = null;
			try
			{
				notificationsBrokerRpcClient = this.GetRpcClient();
				subscription.Parameters = null;
				subscription.Receiver = null;
				string subscription2 = subscription.ToJson();
				ICancelableAsyncResult asyncResult = notificationsBrokerRpcClient.BeginUnsubscribe(null, null, subscription2, null, null);
				BrokerStatus brokerStatus = notificationsBrokerRpcClient.EndUnsubscribe(asyncResult);
				logEvent.Status = new BrokerStatus?(brokerStatus);
				if (brokerStatus != BrokerStatus.Success)
				{
					throw new NotificationsBrokerStatusException(brokerStatus);
				}
			}
			catch (Exception exception)
			{
				logEvent.Exception = exception;
				if (notificationsBrokerRpcClient != null)
				{
					notificationsBrokerRpcClient.Dispose();
					notificationsBrokerRpcClient = null;
				}
				throw;
			}
			finally
			{
				if (notificationsBrokerRpcClient != null)
				{
					this.ReleaseRpcClient(notificationsBrokerRpcClient);
				}
			}
		}

		private void StartNotificationCallbacksInternal(Action<BrokerNotification> notificationCallback, NotificationBrokerClientLogEvent logEvent)
		{
			lock (this.mutex)
			{
				if (this.notificationCallback != null)
				{
					throw new NotificationsBrokerException(ClientAPIStrings.CallbackAlreadyRegistered);
				}
				this.hangingRpcClient = this.GetRpcClient();
				this.notificationCallback = notificationCallback;
				this.CallGetNextNotification();
			}
		}

		private void StopNotificationCallbacksInternal(NotificationBrokerClientLogEvent logEvent)
		{
			lock (this.mutex)
			{
				if (this.retryTimer != null)
				{
					this.retryTimer.Dispose(true);
					this.retryTimer = null;
				}
				this.notificationCallback = null;
				if (this.hangingAsyncResult != null)
				{
					this.hangingAsyncResult.Cancel();
					this.hangingAsyncResult = null;
				}
				if (this.hangingRpcClient != null)
				{
					this.ReleaseRpcClient(this.hangingRpcClient);
					this.hangingRpcClient = null;
				}
			}
		}

		private NotificationsBrokerRpcClient GetRpcClient()
		{
			NotificationsBrokerRpcClient result;
			lock (this.mutex)
			{
				NotificationsBrokerRpcClient notificationsBrokerRpcClient;
				if (this.rpcClientPool.Count == 0)
				{
					RpcBindingInfo rpcBindingInfo = new RpcBindingInfo();
					rpcBindingInfo.ProtocolSequence = "ncalrpc";
					rpcBindingInfo.DefaultOmittedProperties();
					notificationsBrokerRpcClient = new NotificationsBrokerRpcClient(rpcBindingInfo);
				}
				else
				{
					notificationsBrokerRpcClient = this.rpcClientPool.Dequeue();
				}
				result = notificationsBrokerRpcClient;
			}
			return result;
		}

		private void ReleaseRpcClient(NotificationsBrokerRpcClient rpcClient)
		{
			lock (this.mutex)
			{
				if (this.rpcClientPool.Count < 100)
				{
					this.rpcClientPool.Enqueue(rpcClient);
				}
				else
				{
					rpcClient.Dispose();
				}
			}
		}

		private void CallGetNextNotification()
		{
			lock (this.mutex)
			{
				this.hangingAsyncResult = this.hangingRpcClient.BeginGetNextNotification(null, null, (int)this.consumerId, this.lastNotificationId, new CancelableAsyncCallback(NotificationBrokerClient.GetNextNotificationCallback), this);
			}
		}

		private void OnNotification(IAsyncResult asyncResult)
		{
			string notificationJson = null;
			Action<BrokerNotification> callbackToCall = null;
			BrokerNotification notification = null;
			this.LogLatency("ReceiveNotification", delegate(NotificationBrokerClientLogEvent logEvent)
			{
				lock (this.mutex)
				{
					if (this.hangingRpcClient != null)
					{
						BrokerStatus brokerStatus = BrokerStatus.None;
						try
						{
							brokerStatus = this.hangingRpcClient.EndGetNextNotification((ICancelableAsyncResult)asyncResult, out notificationJson);
							this.hangingAsyncResult = null;
							if (brokerStatus == BrokerStatus.Success)
							{
								notification = BrokerNotification.FromJson(notificationJson);
								this.lastNotificationId = notification.NotificationId;
								callbackToCall = this.notificationCallback;
								logEvent.SubscriptionId = new Guid?(notification.SubscriptionId);
								logEvent.NotificationId = new Guid?(notification.NotificationId);
								logEvent.SequenceId = new int?(notification.SequenceNumber);
							}
						}
						catch (RpcException ex)
						{
							logEvent.Exception = ex;
							brokerStatus = BrokerStatus.UnknownError;
							ExTraceGlobals.ClientTracer.TraceError<RpcException>((long)this.GetHashCode(), "NotificationsBroker.OnNotification exception: {0}", ex);
						}
						logEvent.Status = new BrokerStatus?(brokerStatus);
						if (this.notificationCallback != null)
						{
							if (brokerStatus == BrokerStatus.Success)
							{
								this.CallGetNextNotification();
							}
							else if (this.retryTimer == null)
							{
								this.retryTimer = new GuardedTimer(delegate(object param0)
								{
									lock (this.mutex)
									{
										if (this.notificationCallback != null)
										{
											this.CallGetNextNotification();
										}
										this.retryTimer.Dispose(false);
										this.retryTimer = null;
									}
								}, null, TimeSpan.FromSeconds(1.0), TimeSpan.FromMilliseconds(-1.0));
							}
						}
					}
				}
			});
			if (callbackToCall != null && notificationJson != null)
			{
				this.LogLatency("ProcessNotification", delegate(NotificationBrokerClientLogEvent logEvent)
				{
					logEvent.SubscriptionId = new Guid?(notification.SubscriptionId);
					logEvent.NotificationId = new Guid?(notification.NotificationId);
					logEvent.SequenceId = new int?(notification.SequenceNumber);
					this.notificationCallback(notification);
				});
			}
		}

		private static readonly object defaultLoggerLock = new object();

		private static NotificationBrokerClientLogger defaultLogger = null;

		private readonly IExtensibleLogger logger;

		private object mutex = new object();

		private Queue<NotificationsBrokerRpcClient> rpcClientPool = new Queue<NotificationsBrokerRpcClient>();

		private NotificationsBrokerRpcClient hangingRpcClient;

		private ConsumerId consumerId;

		private Guid lastNotificationId;

		private Action<BrokerNotification> notificationCallback;

		private ICancelableAsyncResult hangingAsyncResult;

		private GuardedTimer retryTimer;
	}
}
