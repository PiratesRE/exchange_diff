using System;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Core.Types;
using Microsoft.Exchange.Services.EventLogs;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class PushSubscription : SubscriptionBase, IDisposable
	{
		public PushSubscription(PushSubscriptionRequest subscriptionRequest, IdAndSession[] folderIds, Guid subscriptionOwnerObjectGuid, Subscriptions subscriptions) : base(subscriptionRequest, folderIds, subscriptionOwnerObjectGuid)
		{
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug<int>((long)this.GetHashCode(), "PushSubscription constructor called: HashCode {0}", this.GetHashCode());
			this.budgetKey = CallContext.Current.Budget.Owner;
			this.statusFrequencyInMillisecs = 60000 * subscriptionRequest.StatusFrequency;
			this.clientUrl = subscriptionRequest.Url;
			this.callerData = subscriptionRequest.CallerData;
			this.subscriptions = subscriptions;
			StoreSession storeSessionForOperation = base.GetStoreSessionForOperation(folderIds);
			this.mdbGuid = storeSessionForOperation.MdbGuid;
			this.retryTimer = new Timer(new TimerCallback(PushSubscription.RetryTimerCallback), this, -1, -1);
			this.currentState = PushSubscription.NotificationState.Idle;
			this.RegisterEventHandler(30000L);
			this.requestVersion = ExchangeVersion.Current;
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug<int, string>((long)this.GetHashCode(), "PushSubscription constructor exit: HashCode {0}, SubscriptionId {1}", this.GetHashCode(), base.SubscriptionId);
		}

		private static void EventAvailableOrTimerCallback(object state, bool timedOut)
		{
			PushSubscription pushSubscription = (PushSubscription)state;
			pushSubscription.DoEventAvailableOrTimer();
		}

		private static void ProcessResultCallback(object state, SendNotificationResult result, Exception exception)
		{
			PushSubscription pushSubscription = (PushSubscription)state;
			if (result != null && result.SubscriptionStatus != SubscriptionStatus.Invalid)
			{
				pushSubscription.ProcessNotificationResult(result);
				PerformanceMonitor.UpdatePushStatusCounter(true);
				return;
			}
			pushSubscription.HandleFailedNotice(exception);
			PerformanceMonitor.UpdatePushStatusCounter(false);
		}

		private static void RetryTimerCallback(object state)
		{
			PushSubscription pushSubscription = (PushSubscription)state;
			pushSubscription.TrySendNotificationAsync();
		}

		private static string GetExceptionDetailsForLogging(Exception exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (exception != null)
			{
				stringBuilder.Append(string.Format("{0}: {1} ", exception.GetType().Name, exception.Message));
				if (exception is WebException)
				{
					WebException ex = (WebException)exception;
					HttpWebResponse httpWebResponse = (HttpWebResponse)ex.Response;
					if (httpWebResponse != null)
					{
						stringBuilder.Append(string.Format("StatusCode: {0} {1} ", httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
					}
					else
					{
						stringBuilder.Append(string.Format("Status: {0} ", ex.Status.ToString()));
					}
				}
				else if (exception is SoapException)
				{
					SoapException ex2 = (SoapException)exception;
					stringBuilder.Append(string.Format("Code: {0} Detail: {1} ", ex2.Code, ex2.Detail.InnerText));
				}
				stringBuilder.Append(exception.StackTrace);
			}
			return stringBuilder.ToString();
		}

		private void RegisterEventHandler(long timeoutInMillisecs)
		{
			WaitHandle eventAvailableWaitHandle = base.EventQueue.EventAvailableWaitHandle;
			WaitOrTimerCallback callBack = new WaitOrTimerCallback(PushSubscription.EventAvailableOrTimerCallback);
			this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(eventAvailableWaitHandle, callBack, this, timeoutInMillisecs, true);
		}

		private void DoEventAvailableOrTimer()
		{
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string, PushSubscription.NotificationState>((long)this.GetHashCode(), "DoEventAvailableOrTimer called: SubscriptionId {0}, State {1}", base.SubscriptionId, this.currentState);
			lock (this.lockObject)
			{
				if (this.currentState != PushSubscription.NotificationState.Idle)
				{
					ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "DoEventAvailableOrTimer: Subscription {0} was *not* idle", base.SubscriptionId);
					return;
				}
				this.currentState = PushSubscription.NotificationState.Sending;
				this.notificationData = null;
				this.retryInterval = 0;
				this.failedSendCount = 0;
			}
			ExchangeVersion.ExecuteWithSpecifiedVersion(this.requestVersion, new ExchangeVersion.ExchangeVersionDelegate(this.TrySendNotificationAsync));
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string, PushSubscription.NotificationState>((long)this.GetHashCode(), "DoEventAvailableOrTimer exit: SubscriptionId {0}, State {1}", base.SubscriptionId, this.currentState);
		}

		private void TrySendNotificationAsync()
		{
			this.ChangeRetryTimer(-1, -1);
			this.BeginSendNotification();
		}

		private void HandleFailedNotice(Exception sendException)
		{
			if (this.failedSendCount == 0)
			{
				string exceptionDetailsForLogging = PushSubscription.GetExceptionDetailsForLogging(sendException);
				ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushNotificationFailedFirstTime, exceptionDetailsForLogging, new object[]
				{
					base.SubscriptionId,
					this.clientUrl,
					exceptionDetailsForLogging
				});
				ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "PushSubscription::HandleFailedNotice: Failed to send notification for SubscriptionId {0} to endpoint [{2}]: Details: {1}", base.SubscriptionId, exceptionDetailsForLogging, this.clientUrl);
				this.retryInterval = 30000;
				this.failedSendCount = 1;
				this.ChangeRetryTimer(this.retryInterval, -1);
				return;
			}
			this.failedSendCount++;
			this.retryInterval *= 2;
			if (this.retryInterval > this.statusFrequencyInMillisecs)
			{
				this.ChangeRetryTimer(-1, -1);
				this.RemoveSubscription(false);
				string exceptionDetailsForLogging2 = PushSubscription.GetExceptionDetailsForLogging(sendException);
				ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushSubscriptionFailedFinal, exceptionDetailsForLogging2, new object[]
				{
					base.SubscriptionId,
					this.clientUrl,
					this.failedSendCount,
					exceptionDetailsForLogging2
				});
				ExTraceGlobals.PushSubscriptionTracer.TraceError<string, string, string>((long)this.GetHashCode(), "PushSubscription::HandleFailedNotice: too many retry attempts, giving up on SubscriptionId {0} for endpoint [{2}] Details: {1}", base.SubscriptionId, exceptionDetailsForLogging2, this.clientUrl);
				return;
			}
			string exceptionDetailsForLogging3 = PushSubscription.GetExceptionDetailsForLogging(sendException);
			ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushNotificationFailedRetry, exceptionDetailsForLogging3, new object[]
			{
				base.SubscriptionId,
				this.clientUrl,
				this.failedSendCount,
				exceptionDetailsForLogging3
			});
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug((long)this.GetHashCode(), "PushSubscription::HandleFailedNotice: {0} retry attempt failed on SubscriptionId {1} for endpoint [{3}] Details: {2}", new object[]
			{
				this.failedSendCount,
				base.SubscriptionId,
				exceptionDetailsForLogging3,
				this.clientUrl
			});
			this.ChangeRetryTimer(this.retryInterval, -1);
		}

		private void ProcessNotificationResult(SendNotificationResult result)
		{
			if (result.SubscriptionStatus == SubscriptionStatus.Unsubscribe)
			{
				this.RemoveSubscription(true);
				return;
			}
			if (this.currentState == PushSubscription.NotificationState.Error)
			{
				this.RemoveSubscription(false);
				return;
			}
			if (this.currentState == PushSubscription.NotificationState.Sending)
			{
				this.currentState = PushSubscription.NotificationState.Idle;
				this.notificationData = null;
				this.ChangeRetryTimer(-1, -1);
				this.RegisterEventHandler((long)this.statusFrequencyInMillisecs);
			}
		}

		private void RemoveSubscription(bool isUnsubscribe)
		{
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string, bool>((long)this.GetHashCode(), "RemoveSubscription: SubscriptionId {0}, IsUnsubscribe {1}", base.SubscriptionId, isUnsubscribe);
			if (isUnsubscribe)
			{
				PerformanceMonitor.UpdateUnsubscribeCounter();
			}
			lock (this.lockObject)
			{
				this.currentState = PushSubscription.NotificationState.Dead;
				try
				{
					this.subscriptions.Delete(base.SubscriptionId);
				}
				catch (SubscriptionNotFoundException)
				{
					ExTraceGlobals.PushSubscriptionTracer.TraceError<string>((long)this.GetHashCode(), "RemoveSubscription: Tried to remove subscription {0}. It had already been deleted.", base.SubscriptionId);
				}
			}
		}

		private void BeginSendNotification()
		{
			if (this.notificationData == null)
			{
				this.notificationData = this.BuildNotificationMessage();
			}
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "BeginSendNotification: SubscriptionId {0}", base.SubscriptionId);
			NotificationServiceClient notificationServiceClient = new NotificationServiceClient(this.clientUrl, this.callerData);
			notificationServiceClient.Timeout = 60000;
			notificationServiceClient.ResponseLimitInBytes = 1024;
			if (this.requestVersion.Supports(ExchangeVersion.Exchange2007SP1))
			{
				notificationServiceClient.RequestServerVersionValue = new NotificationRequestServerVersion(this.requestVersion.Version);
			}
			notificationServiceClient.SendNotificationAsync(this.notificationData, new NotificationServiceClient.SendNotificationResultCallback(PushSubscription.ProcessResultCallback), this);
		}

		private SendNotificationResponse BuildNotificationMessage()
		{
			SendNotificationResponse sendNotificationResponse = new SendNotificationResponse();
			ServiceResult<EwsNotificationType>[] serviceResults = ExceptionHandler<EwsNotificationType>.Execute(new ExceptionHandler<EwsNotificationType>.CreateServiceResults(this.BuildNotification), new ExceptionHandler<EwsNotificationType>.GenerateMessageXmlForServiceError(this.AddSubscriptionIdToServiceError));
			sendNotificationResponse.BuildForResults<EwsNotificationType>(serviceResults);
			return sendNotificationResponse;
		}

		private ServiceResult<EwsNotificationType>[] BuildNotification()
		{
			ServiceResult<EwsNotificationType>[] result;
			try
			{
				FaultInjection.GenerateFault((FaultInjection.LIDs)2833657149U);
				using (IEwsBudget ewsBudget = EwsBudget.Acquire(this.budgetKey))
				{
					try
					{
						ewsBudget.CheckOverBudget();
						ewsBudget.StartLocal("PushSubscription.ServiceResult", default(TimeSpan));
						ServiceResult<EwsNotificationType>[] array = new ServiceResult<EwsNotificationType>[1];
						EwsNotificationType events = this.GetEvents(base.LastWatermarkSent);
						array[0] = new ServiceResult<EwsNotificationType>(events);
						result = array;
					}
					finally
					{
						ewsBudget.LogEndStateToIIS();
					}
				}
			}
			catch (EventNotFoundException ex)
			{
				ExTraceGlobals.PushSubscriptionTracer.TraceDebug<EventNotFoundException>((long)this.GetHashCode(), "BuildNotification caught exception: {0}, subscription marked for deletion", ex);
				ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushNotificationReadEventsFailed, base.MailboxId.MailboxGuid.ToString(), new object[]
				{
					base.SubscriptionId,
					this.clientUrl,
					ex
				});
				this.currentState = PushSubscription.NotificationState.Error;
				throw;
			}
			catch (FinalEventException ex2)
			{
				ExTraceGlobals.GetEventsCallTracer.TraceDebug<FinalEventException, Event>((long)this.GetHashCode(), "BuildNotification caught exception: {0}, FinalEvent: {1}, subscription marked for deletion", ex2, ex2.FinalEvent);
				ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushNotificationReadEventsFailed, base.MailboxId.MailboxGuid.ToString(), new object[]
				{
					base.SubscriptionId,
					this.clientUrl,
					ex2
				});
				this.currentState = PushSubscription.NotificationState.Error;
				throw;
			}
			catch (ReadEventsFailedException ex3)
			{
				if (ex3.InnerException != null)
				{
					ExTraceGlobals.PushSubscriptionTracer.TraceDebug<Exception>((long)this.GetHashCode(), "BuildNotification caught exception: {0}, subscription marked for deletion", ex3.InnerException);
				}
				ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushNotificationReadEventsFailed, this.mdbGuid.ToString(), new object[]
				{
					base.SubscriptionId,
					this.clientUrl,
					ex3
				});
				this.currentState = PushSubscription.NotificationState.Error;
				throw;
			}
			catch (ReadEventsFailedTransientException ex4)
			{
				if (ex4.InnerException != null)
				{
					ExTraceGlobals.PushSubscriptionTracer.TraceDebug<Exception>((long)this.GetHashCode(), "BuildNotification caught exception: {0}, subscription marked for deletion", ex4.InnerException);
				}
				ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushNotificationReadEventsFailed, this.mdbGuid.ToString(), new object[]
				{
					base.SubscriptionId,
					this.clientUrl,
					ex4
				});
				this.currentState = PushSubscription.NotificationState.Error;
				throw;
			}
			catch (OverBudgetException ex5)
			{
				ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "BuildNotification caught overbudget exception.  Subscription marked for deletion.  Budget snapshot: {0}", ex5.Snapshot);
				ServiceDiagnostics.Logger.LogEvent(ServicesEventLogConstants.Tuple_PushNotificationReadEventsFailed, base.MailboxId.MailboxGuid.ToString(), new object[]
				{
					base.SubscriptionId,
					this.clientUrl,
					ex5
				});
				this.currentState = PushSubscription.NotificationState.Error;
				throw;
			}
			return result;
		}

		private void ChangeRetryTimer(int dueTime, int period)
		{
			lock (this.lockObject)
			{
				if (this.retryTimer != null)
				{
					this.retryTimer.Change(dueTime, period);
				}
			}
		}

		private void AddSubscriptionIdToServiceError(ServiceError serviceError, Exception exception)
		{
			serviceError.AddConstantValueProperty("SubscriptionId", base.SubscriptionId);
		}

		protected override int EventQueueSize
		{
			get
			{
				return 25;
			}
		}

		protected override void Dispose(bool isDisposing)
		{
			ExTraceGlobals.PushSubscriptionTracer.TraceDebug<string>((long)this.GetHashCode(), "Dispose: SubscriptionId {0}", base.SubscriptionId);
			if (!this.isDisposed && isDisposing)
			{
				lock (this.lockObject)
				{
					this.currentState = PushSubscription.NotificationState.Dead;
					if (this.retryTimer != null)
					{
						this.retryTimer.Change(-1, -1);
						this.retryTimer.Dispose();
						this.retryTimer = null;
					}
					if (this.registeredWaitHandle != null)
					{
						this.registeredWaitHandle.Unregister(base.EventQueue.EventAvailableWaitHandle);
						this.registeredWaitHandle = null;
					}
					base.Dispose(isDisposing);
				}
			}
		}

		private const int PushEventQueueSize = 25;

		private const int RetryIntervalInMillisecs = 30000;

		private const int RequestTimeoutInMillisecs = 60000;

		private const int FirstTimeHearbeatInMillisecs = 30000;

		private const int SendNotificationResponseLimitInBytes = 1024;

		private const string SubscriptionIdErrorName = "SubscriptionId";

		private readonly string callerData;

		private BudgetKey budgetKey;

		private int statusFrequencyInMillisecs;

		private RegisteredWaitHandle registeredWaitHandle;

		private Timer retryTimer;

		private string clientUrl;

		private Subscriptions subscriptions;

		private PushSubscription.NotificationState currentState;

		private int retryInterval;

		private int failedSendCount;

		private SendNotificationResponse notificationData;

		private ExchangeVersion requestVersion;

		private Guid mdbGuid;

		private enum NotificationState
		{
			Idle,
			Sending,
			Error,
			Dead
		}
	}
}
