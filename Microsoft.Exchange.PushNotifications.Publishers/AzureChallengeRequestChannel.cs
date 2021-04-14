using System;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureChallengeRequestChannel : AzureBaseChannel<AzureChallengeRequestNotification>
	{
		public AzureChallengeRequestChannel(AzureChallengeRequestChannelSettings settings, ITracer tracer, EventHandler<MissingHubEventArgs> missingHubHandler, AzureClient azureClient = null, AzureErrorTracker errorTracker = null) : base(settings.AppId, tracer, azureClient, missingHubHandler)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.State = AzureChallengeRequestChannelState.Init;
			this.Settings = settings;
			this.ErrorTracker = (errorTracker ?? new AzureErrorTracker(this.Settings.BackOffTimeInSeconds));
		}

		public AzureChallengeRequestChannelState State { get; private set; }

		private AzureChallengeRequestChannelSettings Settings { get; set; }

		private AzureErrorTracker ErrorTracker { get; set; }

		public override void Send(AzureChallengeRequestNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			PushNotificationChannelContext<AzureChallengeRequestNotification> pushNotificationChannelContext = new PushNotificationChannelContext<AzureChallengeRequestNotification>(notification, cancelToken, base.Tracer);
			AzureChallengeRequestChannelState azureChallengeRequestChannelState = this.State;
			while (pushNotificationChannelContext.IsActive)
			{
				this.CheckCancellation(pushNotificationChannelContext);
				switch (this.State)
				{
				case AzureChallengeRequestChannelState.Init:
					azureChallengeRequestChannelState = this.ProcessInit(pushNotificationChannelContext);
					break;
				case AzureChallengeRequestChannelState.Sending:
					azureChallengeRequestChannelState = this.ProcessSending(pushNotificationChannelContext);
					break;
				case AzureChallengeRequestChannelState.Discarding:
					azureChallengeRequestChannelState = this.ProcessDiscarding(pushNotificationChannelContext);
					break;
				default:
					pushNotificationChannelContext.Drop(null);
					azureChallengeRequestChannelState = AzureChallengeRequestChannelState.Sending;
					break;
				}
				base.Tracer.TraceDebug<AzureChallengeRequestChannelState, AzureChallengeRequestChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, azureChallengeRequestChannelState);
				this.State = azureChallengeRequestChannelState;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureChallengeRequestChannel>(this);
		}

		private void CheckCancellation(PushNotificationChannelContext<AzureChallengeRequestNotification> currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				base.Tracer.TraceDebug<AzureChallengeRequestChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private AzureChallengeRequestChannelState ProcessInit(PushNotificationChannelContext<AzureChallengeRequestNotification> currentNotification)
		{
			return AzureChallengeRequestChannelState.Sending;
		}

		private AzureChallengeRequestChannelState ProcessDelaying(PushNotificationChannelContext<AzureChallengeRequestNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureChallengeRequestNotification>, ExDateTime>((long)this.GetHashCode(), "[ProcessDelaying] Delaying notification {0} until {1} (UTC)", currentNotification, this.ErrorTracker.DelayEndTime);
			while (this.ErrorTracker.ShouldDelay && !currentNotification.IsCancelled)
			{
				this.ErrorTracker.ConsumeDelay(this.Settings.RequestStepTimeout);
			}
			return AzureChallengeRequestChannelState.Init;
		}

		private AzureChallengeRequestChannelState ProcessDiscarding(PushNotificationChannelContext<AzureChallengeRequestNotification> currentNotification)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				currentNotification.Drop(null);
				return AzureChallengeRequestChannelState.Discarding;
			}
			this.ErrorTracker.Reset();
			return AzureChallengeRequestChannelState.Init;
		}

		private AzureChallengeRequestChannelState ProcessSending(PushNotificationChannelContext<AzureChallengeRequestNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureChallengeRequestNotification>>((long)this.GetHashCode(), "[ProcessSending] Sending notification '{0}'", currentNotification);
			AzureUriTemplate uriTemplate = currentNotification.Notification.UriTemplate;
			string text = uriTemplate.CreateIssueRegistrationSecretStringUri(currentNotification.Notification.TargetAppId, currentNotification.Notification.HubName);
			using (AzureRegistrationChallengeRequest azureRegistrationChallengeRequest = new AzureRegistrationChallengeRequest(currentNotification.Notification, this.CreateAzureSasToken(currentNotification.Notification.AzureSasTokenProvider, text), text))
			{
				base.LogAzureRequest(currentNotification, azureRegistrationChallengeRequest);
				azureRegistrationChallengeRequest.Timeout = this.Settings.RequestTimeout;
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginRegistrationChallengeRequest(azureRegistrationChallengeRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureResponse azureResponse = base.AzureClient.EndRegistrationChallengeRequest(asyncResult);
				if (flag)
				{
					if (azureResponse.HasSucceeded)
					{
						base.LogAzureResponse(currentNotification, azureResponse);
						PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.AzureChallengeRequest);
						if (currentNotification.Notification.IsMonitoring)
						{
							PushNotificationsMonitoring.PublishSuccessNotification("ChallengeRequestProcessed", currentNotification.Notification.TargetAppId);
						}
						currentNotification.Done();
						this.ErrorTracker.ReportSuccess();
					}
					else
					{
						if (!string.IsNullOrEmpty(currentNotification.Notification.HubName) && azureResponse.OriginalStatusCode != null && azureResponse.OriginalStatusCode.Value == HttpStatusCode.NotFound)
						{
							this.FireMissingHubEvent(currentNotification.Notification.TargetAppId, currentNotification.Notification.HubName);
						}
						else
						{
							this.AnalyzeErrorResponse(azureRegistrationChallengeRequest, azureResponse, currentNotification);
						}
						currentNotification.Drop(azureResponse.ToTraceString());
						if (this.ErrorTracker.ShouldBackOff)
						{
							base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Backing off because of notification errors until {0}", this.ErrorTracker.BackOffEndTime);
							PushNotificationsCrimsonEvents.AzureIssueChallengeChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
							PushNotificationsMonitoring.PublishFailureNotification("AzureChallengeRequestChannelBackOff", base.AppId, azureResponse.ToTraceString());
							return AzureChallengeRequestChannelState.Discarding;
						}
					}
				}
			}
			return AzureChallengeRequestChannelState.Init;
		}

		private void AnalyzeErrorResponse(AzureRequestBase request, AzureResponse response, PushNotificationChannelContext<AzureChallengeRequestNotification> currentNotification)
		{
			if (response.OriginalStatusCode == null)
			{
				this.ErrorTracker.ReportError(AzureErrorType.Unknown);
				base.LogError("[AnalyzeErrorResponse] An unexpected error occurred sending a notification to Azure Notification Hub for Notification: '{0}'. Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureIssueChallengeChannelUnknownError.LogPeriodic<string, string, string, string, string>));
				return;
			}
			HttpStatusCode value = response.OriginalStatusCode.Value;
			if (value <= HttpStatusCode.RequestEntityTooLarge)
			{
				switch (value)
				{
				case HttpStatusCode.BadRequest:
				case HttpStatusCode.Unauthorized:
				case HttpStatusCode.NotFound:
					this.ErrorTracker.ReportError(AzureErrorType.Permanent);
					base.LogError("[AnalyzeErrorResponse] Permanent Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureIssueChallengeChannelPermanentError.LogPeriodic<string, string, string, string, string>));
					return;
				case HttpStatusCode.PaymentRequired:
					goto IL_F3;
				case HttpStatusCode.Forbidden:
					break;
				default:
					if (value != HttpStatusCode.RequestEntityTooLarge)
					{
						goto IL_F3;
					}
					break;
				}
			}
			else if (value != HttpStatusCode.InternalServerError && value != HttpStatusCode.ServiceUnavailable)
			{
				goto IL_F3;
			}
			this.ErrorTracker.ReportError(AzureErrorType.Transient);
			base.LogError("[AnalyzeErrorResponse] Transient Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureIssueChallengeChannelTransientError.LogPeriodic<string, string, string, string, string>));
			return;
			IL_F3:
			this.ErrorTracker.ReportError(AzureErrorType.Unknown);
			base.LogError("[AnalyzeErrorResponse] Unknown Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureIssueChallengeChannelUnknownError.LogPeriodic<string, string, string, string, string>));
		}
	}
}
