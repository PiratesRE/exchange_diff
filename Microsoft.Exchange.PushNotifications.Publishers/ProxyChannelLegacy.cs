using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.Client;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ProxyChannelLegacy : PushNotificationChannel<ProxyNotification>
	{
		public ProxyChannelLegacy(ProxyChannelSettings settings, ITracer tracer, OnPremPublisherServiceProxy onPremClient = null, ProxyErrorTracker errorTracker = null) : base(settings.AppId, tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			this.State = ProxyChannelLegacyState.Init;
			this.Settings = settings;
			if (onPremClient == null)
			{
				OAuthCredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(OrganizationId.ForestWideOrgId, this.Settings.Organization);
				oauthCredentialsForAppToken.Tracer = new PushNotificationsOutboundTracer(string.Format("{0} ({1})", settings.AppId, Guid.NewGuid()));
				this.ProxyClient = new OnPremPublisherServiceProxy(settings.ServiceUri, oauthCredentialsForAppToken);
			}
			else
			{
				this.ProxyClient = onPremClient;
			}
			this.ServerBackOffTime = ExDateTime.MinValue;
			this.ErrorTracker = (errorTracker ?? new ProxyErrorTracker(this.Settings.PublishRetryMax, this.Settings.BackOffTimeInSeconds, this.Settings.PublishRetryDelay));
		}

		public ProxyChannelLegacyState State { get; private set; }

		private ProxyChannelSettings Settings { get; set; }

		private OnPremPublisherServiceProxy ProxyClient { get; set; }

		private ProxyErrorTracker ErrorTracker { get; set; }

		private ExDateTime ServerBackOffTime { get; set; }

		public override void Send(ProxyNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			ProxyChannelLegacy.NotificationContext notificationContext = new ProxyChannelLegacy.NotificationContext(notification, cancelToken, base.Tracer);
			ProxyChannelLegacyState proxyChannelLegacyState = this.State;
			while (notificationContext.IsActive)
			{
				this.CheckCancellation(notificationContext);
				switch (this.State)
				{
				case ProxyChannelLegacyState.Init:
					proxyChannelLegacyState = this.ProcessInit(notificationContext);
					break;
				case ProxyChannelLegacyState.Delaying:
					proxyChannelLegacyState = this.ProcessDelaying(notificationContext);
					break;
				case ProxyChannelLegacyState.Publishing:
					proxyChannelLegacyState = this.ProcessPublishing(notificationContext);
					break;
				case ProxyChannelLegacyState.Discarding:
					proxyChannelLegacyState = this.ProcessDiscarding(notificationContext);
					break;
				default:
					notificationContext.Drop();
					proxyChannelLegacyState = ProxyChannelLegacyState.Init;
					break;
				}
				base.Tracer.TraceDebug<ProxyChannelLegacyState, ProxyChannelLegacyState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, proxyChannelLegacyState);
				this.State = proxyChannelLegacyState;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[InternalDispose] Disposing the channel for '{0}'", base.AppId);
				this.ProxyClient.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ProxyChannelLegacy>(this);
		}

		protected virtual bool ShouldBackOff(ExDateTime serverBackOffTime)
		{
			return this.ErrorTracker.ShouldBackOff || serverBackOffTime > ExDateTime.UtcNow;
		}

		protected virtual bool ShouldDelay()
		{
			return this.ErrorTracker.ShouldDelay;
		}

		private void CheckCancellation(ProxyChannelLegacy.NotificationContext currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				if (this.State == ProxyChannelLegacyState.Delaying)
				{
					this.State = ProxyChannelLegacyState.Init;
				}
				base.Tracer.TraceDebug<ProxyChannelLegacyState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private ProxyChannelLegacyState ProcessInit(ProxyChannelLegacy.NotificationContext currentNotification)
		{
			base.Tracer.TraceDebug((long)this.GetHashCode(), "[ProcessInit] NotificationContext in ProcessInit");
			this.ServerBackOffTime = ExDateTime.MinValue;
			return ProxyChannelLegacyState.Publishing;
		}

		private ProxyChannelLegacyState ProcessPublishing(ProxyChannelLegacy.NotificationContext currentNotification)
		{
			base.Tracer.TraceDebug<ProxyChannelLegacy.NotificationContext>((long)this.GetHashCode(), "[ProcessSending] Sending notification '{0}'", currentNotification);
			string text = null;
			try
			{
				AverageTimeCounterBase averageTimeCounterBase = new AverageTimeCounterBase(ProxyCounters.AveragePublishingTime, ProxyCounters.AveragePublishingTimeBase);
				averageTimeCounterBase.Start();
				IAsyncResult asyncResult = this.ProxyClient.BeginPublishOnPremNotifications(currentNotification.Notification.NotificationBatch, null, null);
				ICancelableAsyncResult cancelableAsyncResult = asyncResult as ICancelableAsyncResult;
				bool flag = this.WaitUntilDoneOrCancelled(cancelableAsyncResult, currentNotification);
				this.ProxyClient.EndPublishOnPremNotifications(cancelableAsyncResult);
				if (flag)
				{
					PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.None);
					averageTimeCounterBase.Stop();
					currentNotification.Done();
					this.ErrorTracker.ReportSuccess();
				}
			}
			catch (PushNotificationServerException<PushNotificationFault> pushNotificationServerException)
			{
				if (pushNotificationServerException.FaultContract == null)
				{
					text = pushNotificationServerException.ToTraceString();
					this.ErrorTracker.ReportError(ProxyErrorType.Unknown);
				}
				else
				{
					PushNotificationFault faultContract = pushNotificationServerException.FaultContract;
					text = string.Format("OriginatingServer:{0},ServerException:{1};FaultException:{2}", faultContract.OriginatingServer, pushNotificationServerException.ToTraceString(), faultContract.ToFullString());
					if (faultContract.BackOffTimeInMilliseconds > 0)
					{
						this.ServerBackOffTime = ExDateTime.UtcNow.AddMilliseconds((double)Math.Min(3600000, faultContract.BackOffTimeInMilliseconds));
						PushNotificationsCrimsonEvents.ProxyServerRequestedBackOff.Log<ExDateTime, string>(this.ServerBackOffTime, faultContract.OriginatingServer);
					}
					else if (faultContract.CanRetry)
					{
						this.ErrorTracker.ReportError(ProxyErrorType.Transient);
					}
					else
					{
						this.ErrorTracker.ReportError(ProxyErrorType.Permanent);
					}
				}
			}
			catch (PushNotificationTransientException exception)
			{
				text = exception.ToTraceString();
				this.ErrorTracker.ReportError(ProxyErrorType.Transient);
			}
			catch (PushNotificationPermanentException exception2)
			{
				text = exception2.ToTraceString();
				this.ErrorTracker.ReportError(ProxyErrorType.Permanent);
			}
			catch (Exception exception3)
			{
				text = exception3.ToTraceString();
				this.ErrorTracker.ReportError(ProxyErrorType.Unknown);
			}
			if (text != null)
			{
				base.Tracer.TraceError<string>((long)this.GetHashCode(), "[ProcessSending] An Exception was reported back from the service: {0}", text);
				PushNotificationsCrimsonEvents.ProxyPublishingError.Log<string, ProxyChannelLegacy.NotificationContext, string>(base.AppId, currentNotification, text);
			}
			if (this.ShouldBackOff(this.ServerBackOffTime))
			{
				base.Tracer.TraceDebug<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Will back off publishing notification for: {0}", this.ErrorTracker.BackOffEndTime);
				return ProxyChannelLegacyState.Discarding;
			}
			if (this.ShouldDelay())
			{
				base.Tracer.TraceDebug<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Will delay notification for: {0}", this.ErrorTracker.DelayEndTime);
				return ProxyChannelLegacyState.Delaying;
			}
			return ProxyChannelLegacyState.Publishing;
		}

		private ProxyChannelLegacyState ProcessDelaying(ProxyChannelLegacy.NotificationContext currentNotification)
		{
			base.Tracer.TraceDebug<ProxyChannelLegacy.NotificationContext, ExDateTime>((long)this.GetHashCode(), "[ProcessDelaying] Delaying notification {0} until {1} (UTC)", currentNotification, this.ErrorTracker.DelayEndTime);
			while (this.ShouldDelay() && !currentNotification.IsCancelled)
			{
				this.ErrorTracker.ConsumeDelay(this.Settings.PublishStepTimeout);
			}
			return ProxyChannelLegacyState.Publishing;
		}

		private ProxyChannelLegacyState ProcessDiscarding(ProxyChannelLegacy.NotificationContext currentNotification)
		{
			if (this.ShouldBackOff(this.ServerBackOffTime))
			{
				currentNotification.Drop();
				return ProxyChannelLegacyState.Discarding;
			}
			this.ErrorTracker.Reset();
			return ProxyChannelLegacyState.Publishing;
		}

		private bool WaitUntilDoneOrCancelled(ICancelableAsyncResult asyncResult, ProxyChannelLegacy.NotificationContext currentNotification)
		{
			int num = 0;
			while (!currentNotification.IsCancelled)
			{
				if (asyncResult.AsyncWaitHandle.WaitOne(this.Settings.PublishStepTimeout))
				{
					return true;
				}
				num++;
				if (num % 3 == 0)
				{
					base.Tracer.TraceDebug<int>((long)this.GetHashCode(), "[WaitUntilDoneOrCancelled] Still waiting for the operation to finish: '{0}'", num);
				}
			}
			base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[WaitUntilDoneOrCancelled] Current notification was cancelled: '{0}'", currentNotification.ToString());
			asyncResult.Cancel();
			return false;
		}

		public const int MaxServerBackOffTimeInMilliseconds = 3600000;

		private class NotificationContext
		{
			public NotificationContext(ProxyNotification notification, CancellationToken cancellationToken, ITracer tracer)
			{
				this.Notification = notification;
				this.CancellationToken = cancellationToken;
				this.Tracer = tracer;
			}

			public ProxyNotification Notification { get; private set; }

			public bool IsActive
			{
				get
				{
					return this.Notification != null;
				}
			}

			public bool IsCancelled
			{
				get
				{
					return this.CancellationToken.IsCancellationRequested;
				}
			}

			private CancellationToken CancellationToken { get; set; }

			private ITracer Tracer { get; set; }

			public void Done()
			{
				this.Tracer.TraceDebug<ProxyNotification>((long)this.GetHashCode(), "[Done] Done with notification '{0}'", this.Notification);
				this.Notification = null;
			}

			public void Drop()
			{
				if (PushNotificationsCrimsonEvents.ProxyNotificationDiscarded.IsEnabled(PushNotificationsCrimsonEvent.Provider))
				{
					PushNotificationsCrimsonEvents.ProxyNotificationDiscarded.Log<string, string>(this.Notification.AppId, this.Notification.ToFullString());
				}
				this.Tracer.TraceWarning<ProxyNotification>((long)this.GetHashCode(), "[Drop] Dropping notification '{0}'", this.Notification);
				this.Notification = null;
			}

			public override string ToString()
			{
				return this.Notification.ToString();
			}
		}
	}
}
