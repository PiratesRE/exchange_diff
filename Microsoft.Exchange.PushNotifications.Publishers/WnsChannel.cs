using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WnsChannel : PushNotificationChannel<WnsNotification>
	{
		public WnsChannel(WnsChannelSettings settings, ITracer tracer, WnsClient wnsClient = null, WnsErrorTracker errorTracker = null) : base(settings.AppId, tracer)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.State = WnsChannelState.Init;
			this.Settings = settings;
			this.WnsClient = (wnsClient ?? new WnsClient(new HttpClient()));
			this.ErrorTracker = (errorTracker ?? new WnsErrorTracker(this.Settings.AuthenticateRetryMax, this.Settings.AuthenticateRetryDelay, this.Settings.BackOffTimeInSeconds));
		}

		public WnsChannelState State { get; private set; }

		private WnsChannelSettings Settings { get; set; }

		private WnsClient WnsClient { get; set; }

		private WnsErrorTracker ErrorTracker { get; set; }

		private WnsAccessToken AccessToken { get; set; }

		public override void Send(WnsNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			PushNotificationChannelContext<WnsNotification> pushNotificationChannelContext = new PushNotificationChannelContext<WnsNotification>(notification, cancelToken, base.Tracer);
			WnsChannelState wnsChannelState = this.State;
			while (pushNotificationChannelContext.IsActive)
			{
				this.CheckCancellation(pushNotificationChannelContext);
				switch (this.State)
				{
				case WnsChannelState.Init:
					wnsChannelState = this.ProcessInit(pushNotificationChannelContext);
					break;
				case WnsChannelState.Authenticating:
					wnsChannelState = this.ProcessAuthenticating(pushNotificationChannelContext);
					break;
				case WnsChannelState.Delaying:
					wnsChannelState = this.ProcessDelaying(pushNotificationChannelContext);
					break;
				case WnsChannelState.Sending:
					wnsChannelState = this.ProcessSending(pushNotificationChannelContext);
					break;
				case WnsChannelState.Discarding:
					wnsChannelState = this.ProcessDiscarding(pushNotificationChannelContext);
					break;
				default:
					pushNotificationChannelContext.Drop(this.State.ToString());
					wnsChannelState = WnsChannelState.Init;
					break;
				}
				base.Tracer.TraceDebug<WnsChannelState, WnsChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, wnsChannelState);
				this.State = wnsChannelState;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[InternalDispose] Disposing the channel for '{0}'", base.AppId);
				this.WnsClient.Dispose();
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WnsChannel>(this);
		}

		private void CheckCancellation(PushNotificationChannelContext<WnsNotification> currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				if (this.State == WnsChannelState.Delaying)
				{
					this.State = WnsChannelState.Authenticating;
				}
				base.Tracer.TraceDebug<WnsChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private WnsChannelState ProcessInit(PushNotificationChannelContext<WnsNotification> currentNotification)
		{
			if (this.AccessToken != null)
			{
				base.Tracer.TraceDebug((long)this.GetHashCode(), "[ProcessInit] Resetting the access token");
				this.AccessToken = null;
			}
			return WnsChannelState.Authenticating;
		}

		private WnsChannelState ProcessAuthenticating(PushNotificationChannelContext<WnsNotification> currentNotification)
		{
			base.Tracer.TraceDebug<string, Uri>((long)this.GetHashCode(), "[ProcessAuthenticating] Authenticating '{0}' with '{1}", this.Settings.AppSid, this.Settings.AuthenticationUri);
			WnsChannelState result;
			using (WnsAuthRequest wnsAuthRequest = new WnsAuthRequest(this.Settings.AuthenticationUri, this.Settings.AppSid, this.Settings.AppSecret))
			{
				wnsAuthRequest.Timeout = this.Settings.RequestTimeout;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ICancelableAsyncResult asyncResult = this.WnsClient.BeginSendAuthRequest(wnsAuthRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				WnsResult<WnsAccessToken> wnsResult = this.WnsClient.EndSendAuthRequest(asyncResult);
				stopwatch.Stop();
				if (flag)
				{
					if (wnsResult.Response != null)
					{
						this.AccessToken = wnsResult.Response;
						this.ErrorTracker.ReportAuthenticationSuccess();
						base.Tracer.TraceDebug((long)this.GetHashCode(), "[ProcessAuthenticating] Authentication succeeded");
						PushNotificationsCrimsonEvents.WnsChannelAuthenticationSucceeded.Log<string, long>(base.AppId, stopwatch.ElapsedMilliseconds);
						result = WnsChannelState.Sending;
					}
					else
					{
						string text = (wnsResult.Exception != null) ? wnsResult.Exception.ToTraceString() : "<null>";
						base.Tracer.TraceError<string>((long)this.GetHashCode(), "[ProcessAuthenticating] Authentication request failed: {0}", text);
						PushNotificationsCrimsonEvents.WnsChannelAuthenticationError.Log<string, long, string>(base.AppId, stopwatch.ElapsedMilliseconds, text);
						this.ErrorTracker.ReportAuthenticationFailure();
						if (this.ErrorTracker.ShouldBackOff)
						{
							base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessAuthenticating] Backing off because of Authentication errors until {0}", this.ErrorTracker.BackOffEndTime);
							PushNotificationsCrimsonEvents.WnsChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
							this.RaiseBackOffMonitoringEvent(text);
							result = WnsChannelState.Discarding;
						}
						else
						{
							result = WnsChannelState.Delaying;
						}
					}
				}
				else
				{
					result = WnsChannelState.Authenticating;
				}
			}
			return result;
		}

		private WnsChannelState ProcessSending(PushNotificationChannelContext<WnsNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<WnsNotification>>((long)this.GetHashCode(), "[ProcessSending] Sending notification '{0}'", currentNotification);
			using (WnsRequest wnsRequest = currentNotification.Notification.CreateWnsRequest())
			{
				wnsRequest.Authorization = this.AccessToken.ToWnsAuthorizationString();
				wnsRequest.Timeout = this.Settings.RequestTimeout;
				if (!currentNotification.Notification.IsMonitoring)
				{
					ICancelableAsyncResult asyncResult = this.WnsClient.BeginSendNotificationRequest(wnsRequest);
					bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
					WnsResult<WnsResponse> wnsResult = this.WnsClient.EndSendNotificationRequest(asyncResult);
					if (flag)
					{
						if (wnsResult.Response == WnsResponse.Succeeded)
						{
							base.Tracer.TraceDebug((long)this.GetHashCode(), string.Format("[ProcessSending] WnsResponse: Succeeded.", new object[0]));
							PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.None);
							currentNotification.Done();
							this.ErrorTracker.ReportWnsRequestSuccess();
						}
						else
						{
							base.Tracer.TraceDebug((long)this.GetHashCode(), string.Format("[ProcessSending] WnsResponse:{0}; Error:{1}", wnsResult.Response, wnsResult.Exception.ToTraceString()));
							string text;
							if (wnsResult.Response != null && wnsResult.Response.ResponseCode == HttpStatusCode.Unauthorized)
							{
								text = string.Format("[ProcessSending] Access token expired: {0}", wnsResult.Response);
								base.Tracer.TraceDebug((long)this.GetHashCode(), text);
								PushNotificationsCrimsonEvents.WnsChannelAccessTokenExpired.Log<string, int, WnsResponse>(base.AppId, this.AccessToken.GetUsageTimeInMinutes(), wnsResult.Response);
								this.ErrorTracker.ReportWnsRequestFailure(WnsResultErrorType.AuthTokenExpired);
								if (!this.ErrorTracker.ShouldBackOff)
								{
									return WnsChannelState.Init;
								}
								this.AccessToken = null;
							}
							else
							{
								text = this.AnalyzeWnsResultError(wnsResult, currentNotification);
							}
							currentNotification.Drop(text);
							if (this.ErrorTracker.ShouldBackOff)
							{
								base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Backing off because of notification errors until {0}", this.ErrorTracker.BackOffEndTime);
								PushNotificationsCrimsonEvents.WnsChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
								this.RaiseBackOffMonitoringEvent(text);
								return WnsChannelState.Discarding;
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
			return WnsChannelState.Sending;
		}

		private WnsChannelState ProcessDelaying(PushNotificationChannelContext<WnsNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<WnsNotification>, ExDateTime>((long)this.GetHashCode(), "[ProcessDelaying] Delaying notification {0} until {1} (UTC)", currentNotification, this.ErrorTracker.DelayEndTime);
			while (this.ErrorTracker.ShouldDelay && !currentNotification.IsCancelled)
			{
				this.ErrorTracker.ConsumeDelay(this.Settings.RequestStepTimeout);
			}
			return WnsChannelState.Authenticating;
		}

		private WnsChannelState ProcessDiscarding(PushNotificationChannelContext<WnsNotification> currentNotification)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				currentNotification.Drop(this.State.ToString());
				return WnsChannelState.Discarding;
			}
			this.ErrorTracker.Reset();
			if (this.AccessToken != null)
			{
				return WnsChannelState.Sending;
			}
			return WnsChannelState.Authenticating;
		}

		private string AnalyzeWnsResultError(WnsResult<WnsResponse> wnsResult, PushNotificationChannelContext<WnsNotification> currentNotification)
		{
			string text = string.Empty;
			if (wnsResult.Response == null)
			{
				string text2 = (wnsResult.Exception != null) ? wnsResult.Exception.ToTraceString() : string.Empty;
				text = string.Format("[AnalyzeWnsResultError] WNS request failed for notification {0}: {1}", currentNotification, text2);
				base.Tracer.TraceError((long)this.GetHashCode(), text);
				PushNotificationsCrimsonEvents.WnsChannelUnknownError.Log<string, PushNotificationChannelContext<WnsNotification>, string>(base.AppId, currentNotification, text2);
				this.ErrorTracker.ReportWnsRequestFailure(wnsResult.IsTimeout ? WnsResultErrorType.Timeout : WnsResultErrorType.Unknown);
				return text;
			}
			HttpStatusCode responseCode = wnsResult.Response.ResponseCode;
			if (responseCode <= HttpStatusCode.RequestEntityTooLarge)
			{
				switch (responseCode)
				{
				case HttpStatusCode.BadRequest:
				case HttpStatusCode.MethodNotAllowed:
					break;
				case HttpStatusCode.Unauthorized:
				case HttpStatusCode.PaymentRequired:
				case HttpStatusCode.ProxyAuthenticationRequired:
				case HttpStatusCode.RequestTimeout:
				case HttpStatusCode.Conflict:
					goto IL_229;
				case HttpStatusCode.Forbidden:
				case HttpStatusCode.NotFound:
				case HttpStatusCode.Gone:
				{
					InvalidPushNotificationException ex = new InvalidPushNotificationException(Strings.WnsChannelInvalidNotificationReported(wnsResult.Response.ToString()), wnsResult.Exception);
					text = ex.Message;
					this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(currentNotification.Notification, ex));
					return text;
				}
				case HttpStatusCode.NotAcceptable:
					text = string.Format("[AnalyzeWnsResultError] WNS is throttling the channel: {0}", wnsResult.Response);
					base.Tracer.TraceError((long)this.GetHashCode(), text);
					PushNotificationsCrimsonEvents.WnsChannelThrottlingError.Log<string, WnsResponse>(base.AppId, wnsResult.Response);
					this.ErrorTracker.ReportWnsRequestFailure(WnsResultErrorType.Throttle);
					return text;
				default:
					if (responseCode != HttpStatusCode.RequestEntityTooLarge)
					{
						goto IL_229;
					}
					break;
				}
				text = string.Format("[AnalyzeWnsResultError] WNS reported the notification was built incorrectly: '{0}'. '{1}'", wnsResult.Response, currentNotification.Notification.ToFullString());
				base.Tracer.TraceError((long)this.GetHashCode(), text);
				PushNotificationsCrimsonEvents.WnsChannelMalformedNotification.Log<string, WnsResponse, string>(base.AppId, wnsResult.Response, currentNotification.Notification.ToFullString());
				return text;
			}
			if (responseCode == HttpStatusCode.InternalServerError || responseCode == HttpStatusCode.ServiceUnavailable)
			{
				text = string.Format("[AnalyzeWnsResultError] WNS reported a service error: {0}", wnsResult.Response);
				base.Tracer.TraceError((long)this.GetHashCode(), text);
				PushNotificationsCrimsonEvents.WnsChannelServiceError.LogPeriodic<string, WnsResponse>(wnsResult.Response.ResponseCode, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, wnsResult.Response);
				this.ErrorTracker.ReportWnsRequestFailure(WnsResultErrorType.ServerUnavailable);
				return text;
			}
			IL_229:
			string text3 = wnsResult.Response.ToString();
			text = string.Format("[AnalyzeWnsResultError] WNS request failed for notification {0}: {1}", currentNotification, text3);
			base.Tracer.TraceError((long)this.GetHashCode(), text);
			PushNotificationsCrimsonEvents.WnsChannelUnknownError.Log<string, PushNotificationChannelContext<WnsNotification>, string>(base.AppId, currentNotification, text3);
			this.ErrorTracker.ReportWnsRequestFailure(WnsResultErrorType.Unknown);
			return text;
		}

		private void RaiseBackOffMonitoringEvent(string traces = "")
		{
			PushNotificationsMonitoring.PublishFailureNotification("WnsChannelBackOff", base.AppId, traces);
		}
	}
}
