using System;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WebAppChannel : PushNotificationChannel<WebAppNotification>
	{
		public WebAppChannel(WebAppChannelSettings settings, ITracer tracer, WebAppErrorTracker errorTracker = null) : base((settings != null) ? settings.AppId : string.Empty, tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.State = WebAppChannelState.Sending;
			this.Settings = settings;
			this.ErrorTracker = (errorTracker ?? new WebAppErrorTracker(this.Settings.BackOffTimeInSeconds));
		}

		public WebAppChannelState State { get; private set; }

		private WebAppChannelSettings Settings { get; set; }

		private WebAppErrorTracker ErrorTracker { get; set; }

		public override void Send(WebAppNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			PushNotificationChannelContext<WebAppNotification> pushNotificationChannelContext = new PushNotificationChannelContext<WebAppNotification>(notification, cancelToken, base.Tracer);
			WebAppChannelState webAppChannelState = this.State;
			while (pushNotificationChannelContext.IsActive)
			{
				this.CheckCancellation(pushNotificationChannelContext);
				switch (this.State)
				{
				case WebAppChannelState.Sending:
					webAppChannelState = this.ProcessSending(pushNotificationChannelContext);
					break;
				case WebAppChannelState.Discarding:
					webAppChannelState = this.ProcessDiscarding(pushNotificationChannelContext);
					break;
				default:
					pushNotificationChannelContext.Drop(null);
					webAppChannelState = WebAppChannelState.Sending;
					break;
				}
				base.Tracer.TraceDebug<WebAppChannelState, WebAppChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, webAppChannelState);
				this.State = webAppChannelState;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[InternalDispose] Disposing the channel for '{0}'", base.AppId);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WebAppChannel>(this);
		}

		private void CheckCancellation(PushNotificationChannelContext<WebAppNotification> currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				base.Tracer.TraceDebug<WebAppChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private WebAppChannelState ProcessSending(PushNotificationChannelContext<WebAppNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<WebAppNotification>>((long)this.GetHashCode(), "[ProcessSending] Sending notification '{0}'", currentNotification);
			using (EsoRequest esoRequest = new EsoRequest(currentNotification.Notification.Action, "WebAppChannel", currentNotification.Notification.Payload))
			{
				esoRequest.Timeout = this.Settings.RequestTimeout;
				if (!currentNotification.Notification.IsMonitoring)
				{
					ICancelableAsyncResult asyncResult = esoRequest.BeginSend();
					bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
					DownloadResult downloadResult = esoRequest.EndSend(asyncResult);
					if (flag)
					{
						if (downloadResult.IsSucceeded)
						{
							PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.None);
							currentNotification.Done();
							this.ErrorTracker.ReportSuccess();
						}
						else
						{
							string text = (downloadResult.Exception != null) ? downloadResult.Exception.ToTraceString() : string.Empty;
							PushNotificationsCrimsonEvents.WebAppChannelUnknownError.Log<string, string, string>(base.AppId, currentNotification.ToString(), text);
							this.ErrorTracker.ReportError(WebAppErrorType.Unknown);
							currentNotification.Drop(null);
							if (this.ErrorTracker.ShouldBackOff)
							{
								base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Backing off because of notification errors until {0}", this.ErrorTracker.BackOffEndTime);
								PushNotificationsCrimsonEvents.WebAppChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
								PushNotificationsMonitoring.PublishFailureNotification("WebAppChannelBackOff", base.AppId, text);
								return WebAppChannelState.Discarding;
							}
						}
					}
				}
				else
				{
					PushNotificationsMonitoring.PublishSuccessNotification("NotificationProcessed", base.AppId);
					currentNotification.Done();
				}
			}
			return WebAppChannelState.Sending;
		}

		private WebAppChannelState ProcessDiscarding(PushNotificationChannelContext<WebAppNotification> currentNotification)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				currentNotification.Drop(null);
				return WebAppChannelState.Discarding;
			}
			this.ErrorTracker.Reset();
			return WebAppChannelState.Sending;
		}
	}
}
