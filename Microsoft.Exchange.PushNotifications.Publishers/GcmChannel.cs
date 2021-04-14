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
	internal sealed class GcmChannel : PushNotificationChannel<GcmNotification>
	{
		public GcmChannel(GcmChannelSettings settings, ITracer tracer, GcmClient gcmClient = null, GcmErrorTracker errorTracker = null) : base(settings.AppId, tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.State = GcmChannelState.Sending;
			this.Settings = settings;
			this.GcmClient = (gcmClient ?? new GcmClient(this.Settings.ServiceUri, new HttpClient()));
			this.ErrorTracker = (errorTracker ?? new GcmErrorTracker(this.Settings.BackOffTimeInSeconds));
		}

		public GcmChannelState State { get; private set; }

		private GcmChannelSettings Settings { get; set; }

		private GcmClient GcmClient { get; set; }

		private GcmErrorTracker ErrorTracker { get; set; }

		public override void Send(GcmNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			PushNotificationChannelContext<GcmNotification> pushNotificationChannelContext = new PushNotificationChannelContext<GcmNotification>(notification, cancelToken, base.Tracer);
			GcmChannelState gcmChannelState = this.State;
			while (pushNotificationChannelContext.IsActive)
			{
				this.CheckCancellation(pushNotificationChannelContext);
				switch (this.State)
				{
				case GcmChannelState.Sending:
					gcmChannelState = this.ProcessSending(pushNotificationChannelContext);
					break;
				case GcmChannelState.Discarding:
					gcmChannelState = this.ProcessDiscarding(pushNotificationChannelContext);
					break;
				default:
					pushNotificationChannelContext.Drop(null);
					gcmChannelState = GcmChannelState.Sending;
					break;
				}
				base.Tracer.TraceDebug<GcmChannelState, GcmChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, gcmChannelState);
				this.State = gcmChannelState;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[InternalDispose] Disposing the channel for '{0}'", base.AppId);
				this.GcmClient.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<GcmChannel>(this);
		}

		private void CheckCancellation(PushNotificationChannelContext<GcmNotification> currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				base.Tracer.TraceDebug<GcmChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private GcmChannelState ProcessSending(PushNotificationChannelContext<GcmNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<GcmNotification>>((long)this.GetHashCode(), "[ProcessSending] Sending notification '{0}'", currentNotification);
			using (GcmRequest gcmRequest = new GcmRequest(currentNotification.Notification))
			{
				gcmRequest.SetSenderAuthToken(this.Settings.SenderAuthToken.AsUnsecureString());
				gcmRequest.Timeout = this.Settings.RequestTimeout;
				if (!currentNotification.Notification.IsMonitoring)
				{
					ICancelableAsyncResult asyncResult = this.GcmClient.BeginSendNotification(gcmRequest);
					bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
					GcmResponse gcmResponse = this.GcmClient.EndSendNotification(asyncResult);
					if (flag)
					{
						if (gcmResponse.TransportStatus == GcmTransportStatusCode.Success && gcmResponse.ResponseStatus == GcmResponseStatusCode.Success)
						{
							PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.None);
							currentNotification.Done();
							this.ErrorTracker.ReportSuccess();
						}
						else
						{
							string text = this.AnalyzeErrorResponse(gcmResponse, currentNotification);
							currentNotification.Drop(text);
							if (this.ErrorTracker.ShouldBackOff)
							{
								base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Backing off because of notification errors until {0}", this.ErrorTracker.BackOffEndTime);
								PushNotificationsCrimsonEvents.GcmChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
								PushNotificationsMonitoring.PublishFailureNotification("GcmChannelBackOff", base.AppId, text);
								return GcmChannelState.Discarding;
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
			return GcmChannelState.Sending;
		}

		private GcmChannelState ProcessDiscarding(PushNotificationChannelContext<GcmNotification> currentNotification)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				currentNotification.Drop(null);
				return GcmChannelState.Discarding;
			}
			this.ErrorTracker.Reset();
			return GcmChannelState.Sending;
		}

		private string AnalyzeErrorResponse(GcmResponse response, PushNotificationChannelContext<GcmNotification> currentNotification)
		{
			if (response.TransportStatus == GcmTransportStatusCode.Success)
			{
				switch (response.ResponseStatus)
				{
				case GcmResponseStatusCode.InvalidRegistration:
				case GcmResponseStatusCode.MismatchSenderId:
				case GcmResponseStatusCode.NotRegistered:
				{
					InvalidPushNotificationException ex = new InvalidPushNotificationException(Strings.GcmInvalidNotificationReported(response.ToFullString()));
					this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(currentNotification.Notification, ex));
					return ex.Message;
				}
				}
				return this.LogUnknownError("[AnalyzeErrorResponse] GCM reported the notification was built incorrectly for {0}. Notification: '{1}'. Response: '{2}'", currentNotification.Notification, response, new Action<string, string, string>(PushNotificationsCrimsonEvents.GcmChannelMalformedNotification.Log<string, string, string>));
			}
			switch (response.TransportStatus)
			{
			case GcmTransportStatusCode.Unauthorized:
				return this.LogTransportError("[AnalyzeErrorResponse] Unauthorized error for {0}: {1}", response, new Action<string, string>(PushNotificationsCrimsonEvents.GcmChannelAuthenticationError.Log<string, string>));
			case GcmTransportStatusCode.ServerUnavailable:
			case GcmTransportStatusCode.InternalServerError:
				if (response.BackOffEndTime != null)
				{
					this.ErrorTracker.SetRetryAfter(response.BackOffEndTime.Value);
				}
				return this.LogTransportError("[AnalyzeErrorResponse] Internal server error for {0}: {1}", response, new Action<string, string>(PushNotificationsCrimsonEvents.GcmChannelServiceError.Log<string, string>));
			case GcmTransportStatusCode.Timeout:
				return this.LogTransportError("[AnalyzeErrorResponse] Timeout error for {0}: {1}", response, new Action<string, string>(PushNotificationsCrimsonEvents.GcmChannelServiceError.Log<string, string>));
			default:
				return this.LogUnknownError("[AnalyzeErrorResponse] Unknown error for {0}. Notification: '{1}'. Response: '{2}'", currentNotification.Notification, response, new Action<string, string, string>(PushNotificationsCrimsonEvents.GcmChannelUnknownError.Log<string, string, string>));
			}
		}

		private string LogTransportError(string traceTemplate, GcmResponse response, Action<string, string> log)
		{
			string text = string.Format(traceTemplate, base.AppId, response.ToFullString());
			base.Tracer.TraceError((long)this.GetHashCode(), text);
			log(base.AppId, response.ToFullString());
			this.ErrorTracker.ReportError(GcmErrorType.Transport);
			return text;
		}

		private string LogUnknownError(string traceTemplate, GcmNotification notification, GcmResponse response, Action<string, string, string> log)
		{
			string text = string.Format(traceTemplate, base.AppId, notification.ToFullString(), response.ToFullString());
			base.Tracer.TraceError((long)this.GetHashCode(), text);
			log(base.AppId, notification.ToFullString(), response.ToFullString());
			this.ErrorTracker.ReportError(GcmErrorType.Unknown);
			return text;
		}
	}
}
