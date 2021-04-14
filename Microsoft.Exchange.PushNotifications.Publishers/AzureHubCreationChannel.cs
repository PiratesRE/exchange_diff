using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureHubCreationChannel : AzureBaseChannel<AzureHubCreationNotification>
	{
		public AzureHubCreationChannel(AzureHubCreationChannelSettings settings, ITracer tracer, AzureClient azureClient = null, AzureHubCreationErrorTracker errorTracker = null) : base(settings.AppId, tracer, azureClient, null)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.State = AzureHubCreationChannelState.Init;
			this.Settings = settings;
			this.ErrorTracker = (errorTracker ?? new AzureHubCreationErrorTracker(this.Settings.AuthenticateRetryDelay, this.Settings.BackOffTimeInSeconds));
			this.HubsCreated = new FifoCache<AzureHubCreationNotification>(this.Settings.MaxHubCreatedCacheSize, AzureHubCreationChannel.AzureHubCreationNotificationComparer.Instance);
		}

		public AzureHubCreationChannelState State { get; private set; }

		private AzureHubCreationChannelSettings Settings { get; set; }

		private AzureHubCreationErrorTracker ErrorTracker { get; set; }

		private AcsAccessToken AccessToken { get; set; }

		private FifoCache<AzureHubCreationNotification> HubsCreated { get; set; }

		public override void Send(AzureHubCreationNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			PushNotificationChannelContext<AzureHubCreationNotification> pushNotificationChannelContext = new PushNotificationChannelContext<AzureHubCreationNotification>(notification, cancelToken, base.Tracer);
			AzureHubCreationChannelState azureHubCreationChannelState = this.State;
			while (pushNotificationChannelContext.IsActive)
			{
				this.CheckCancellation(pushNotificationChannelContext);
				switch (this.State)
				{
				case AzureHubCreationChannelState.Init:
					azureHubCreationChannelState = this.ProcessInit(pushNotificationChannelContext);
					break;
				case AzureHubCreationChannelState.Authenticating:
					azureHubCreationChannelState = this.ProcessAuthenticating(pushNotificationChannelContext);
					break;
				case AzureHubCreationChannelState.Delaying:
					azureHubCreationChannelState = this.ProcessDelaying(pushNotificationChannelContext);
					break;
				case AzureHubCreationChannelState.Sending:
					azureHubCreationChannelState = this.ProcessSending(pushNotificationChannelContext);
					break;
				case AzureHubCreationChannelState.Discarding:
					azureHubCreationChannelState = this.ProcessDiscarding(pushNotificationChannelContext);
					break;
				default:
					pushNotificationChannelContext.Drop(null);
					azureHubCreationChannelState = AzureHubCreationChannelState.Sending;
					break;
				}
				base.Tracer.TraceDebug<AzureHubCreationChannelState, AzureHubCreationChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, azureHubCreationChannelState);
				this.State = azureHubCreationChannelState;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureHubCreationChannel>(this);
		}

		private void CheckCancellation(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				if (this.State == AzureHubCreationChannelState.Delaying)
				{
					this.State = AzureHubCreationChannelState.Authenticating;
				}
				base.Tracer.TraceDebug<AzureHubCreationChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private AzureHubCreationChannelState ProcessInit(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			if (this.AccessToken != null)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[ProcessInit] Resetting the access token for {0}", currentNotification.AppId);
				this.AccessToken = null;
			}
			if (this.CheckHubCreated(currentNotification))
			{
				base.Tracer.TraceDebug((long)this.GetHashCode(), "[ProcessInit] Hub already marked as created.");
				currentNotification.Done();
				return AzureHubCreationChannelState.Init;
			}
			return AzureHubCreationChannelState.Authenticating;
		}

		private AzureHubCreationChannelState ProcessDelaying(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureHubCreationNotification>, ExDateTime>((long)this.GetHashCode(), "[ProcessDelaying] Delaying notification {0} until {1} (UTC)", currentNotification, this.ErrorTracker.DelayEndTime);
			while (this.ErrorTracker.ShouldDelay && !currentNotification.IsCancelled)
			{
				this.ErrorTracker.ConsumeDelay(this.Settings.RequestStepTimeout);
			}
			return AzureHubCreationChannelState.Init;
		}

		private AzureHubCreationChannelState ProcessDiscarding(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				currentNotification.Drop(null);
				return AzureHubCreationChannelState.Discarding;
			}
			this.ErrorTracker.Reset();
			return AzureHubCreationChannelState.Init;
		}

		private AzureHubCreationChannelState ProcessAuthenticating(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			Uri uri = this.Settings.AcsUriTemplate.CreateAcsTokenRequestUri(currentNotification.Notification);
			string text = this.Settings.AcsUriTemplate.CreateScopeUriString(currentNotification.Notification);
			base.Tracer.TraceDebug<Uri, string>((long)this.GetHashCode(), "[ProcessAuthenticating] Authenticating '{0}' with '{1}'.", uri, text);
			AzureHubCreationChannelState result;
			using (AcsAuthRequest acsAuthRequest = new AcsAuthRequest(uri, this.Settings.AcsUserName, this.Settings.AcsUserPassword, text))
			{
				acsAuthRequest.Timeout = this.Settings.RequestTimeout;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginSendAuthRequest(acsAuthRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureResponse azureResponse = base.AzureClient.EndSendAuthRequest(asyncResult);
				stopwatch.Stop();
				if (flag)
				{
					if (azureResponse.HasSucceeded)
					{
						this.AccessToken = new AcsAccessToken(azureResponse.OriginalBody);
						base.Tracer.TraceDebug((long)this.GetHashCode(), "[ProcessAuthenticating] Authentication succeeded");
						if (PushNotificationsCrimsonEvents.AzureChannelAuthenticationSucceeded.IsEnabled(PushNotificationsCrimsonEvent.Provider))
						{
							PushNotificationsCrimsonEvents.AzureChannelAuthenticationSucceeded.Log<string, string>(base.AppId, string.Format("{0}, {1}", this.AccessToken.ToString(), stopwatch.ElapsedMilliseconds));
						}
						result = AzureHubCreationChannelState.Sending;
					}
					else
					{
						if (base.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
						{
							string arg = string.Format("Notification: '{0}'; Response: {1}; Exception: {2}", currentNotification.Notification.ToFullString(), azureResponse.ToTraceString(), azureResponse.Exception.ToTraceString());
							base.Tracer.TraceError<string>((long)this.GetHashCode(), "[ProcessAuthenticating] Authentication request failed: {0}", arg);
						}
						PushNotificationsCrimsonEvents.AzureChannelAuthenticationError.LogPeriodic<string, string, string, string, string, long>(uri, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, currentNotification.Notification.Identifier, currentNotification.Notification.ToFullString(), acsAuthRequest.ToTraceString(), azureResponse.ToTraceString(), stopwatch.ElapsedMilliseconds);
						if (azureResponse.OriginalStatusCode != null && azureResponse.OriginalStatusCode.Value == HttpStatusCode.Unauthorized)
						{
							this.ErrorTracker.ReportError(AzureHubCreationErrorType.Permanent);
						}
						else
						{
							this.ErrorTracker.ReportError(AzureHubCreationErrorType.Unknown);
						}
						currentNotification.Drop(azureResponse.ToTraceString());
						if (this.ErrorTracker.ShouldBackOff)
						{
							base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessAuthenticating] Backing off because of Authentication errors until {0}", this.ErrorTracker.BackOffEndTime);
							PushNotificationsCrimsonEvents.AzureHubCreationChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
							result = AzureHubCreationChannelState.Discarding;
						}
						else
						{
							result = AzureHubCreationChannelState.Delaying;
						}
					}
				}
				else
				{
					result = AzureHubCreationChannelState.Init;
				}
			}
			return result;
		}

		private AzureHubCreationChannelState ProcessSending(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureHubCreationNotification>>((long)this.GetHashCode(), "[ProcessSending] Sending notification '{0}'", currentNotification);
			string resourceUri = this.Settings.UriTemplate.CreateTargetHubCreationStringUri(currentNotification.Notification);
			using (AzureHubCreationRequest azureHubCreationRequest = new AzureHubCreationRequest(currentNotification.Notification, this.AccessToken, resourceUri))
			{
				base.LogAzureRequest(currentNotification, azureHubCreationRequest);
				azureHubCreationRequest.Timeout = this.Settings.RequestTimeout;
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginHubCretionRequest(azureHubCreationRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureResponse azureResponse = base.AzureClient.EndHubCretionRequest(asyncResult);
				if (flag)
				{
					if (azureResponse.HasSucceeded || (azureResponse.OriginalStatusCode != null && azureResponse.OriginalStatusCode.Value == HttpStatusCode.Conflict))
					{
						this.AddHubCreated(currentNotification);
						base.LogAzureResponse(currentNotification, azureResponse);
						PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.AzureHubCreation);
						if (currentNotification.Notification.IsMonitoring)
						{
							PushNotificationsMonitoring.PublishSuccessNotification("HubCreationProcessed", currentNotification.Notification.AzureNamespace);
						}
						currentNotification.Done();
						this.ErrorTracker.ReportSuccess();
					}
					else
					{
						this.AnalyzeErrorResponse(azureHubCreationRequest, azureResponse, currentNotification);
						currentNotification.Drop(azureResponse.ToTraceString());
						if (this.ErrorTracker.ShouldBackOff)
						{
							base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Backing off because of notification errors until {0}", this.ErrorTracker.BackOffEndTime);
							PushNotificationsCrimsonEvents.AzureHubCreationChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
							PushNotificationsMonitoring.PublishFailureNotification("AzureHubCreationChannelBackOff", base.AppId, azureResponse.ToTraceString());
							return AzureHubCreationChannelState.Discarding;
						}
					}
				}
			}
			return AzureHubCreationChannelState.Init;
		}

		private void AnalyzeErrorResponse(AzureRequestBase request, AzureResponse response, PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			if (response.OriginalStatusCode == null)
			{
				this.ErrorTracker.ReportError(AzureHubCreationErrorType.Unknown);
				base.LogError("[AnalyzeErrorResponse] An unexpected error occurred sending a notification to Azure Notification Hub for '{0}'. Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureHubCreationChannelUnknownError.LogPeriodic<string, string, string, string, string>));
				return;
			}
			switch (response.OriginalStatusCode.Value)
			{
			case HttpStatusCode.BadRequest:
			case HttpStatusCode.Forbidden:
				this.ErrorTracker.ReportError(AzureHubCreationErrorType.Permanent);
				base.LogError("[AnalyzeErrorResponse] Permanent Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureHubCreationChannelPermanentError.LogPeriodic<string, string, string, string, string>));
				return;
			case HttpStatusCode.Unauthorized:
				this.ErrorTracker.ReportError(AzureHubCreationErrorType.Unauthorized);
				base.LogError("[AnalyzeErrorResponse] Invalid Acs Token; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureChannelAcsTokenInvalid.LogPeriodic<string, string, string, string, string>));
				return;
			}
			this.ErrorTracker.ReportError(AzureHubCreationErrorType.Unknown);
			base.LogError("[AnalyzeErrorResponse] Unknown Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureHubCreationChannelUnknownError.LogPeriodic<string, string, string, string, string>));
		}

		private bool CheckHubCreated(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			return !currentNotification.Notification.IsMonitoring && this.HubsCreated.Contains(currentNotification.Notification);
		}

		private void AddHubCreated(PushNotificationChannelContext<AzureHubCreationNotification> currentNotification)
		{
			if (currentNotification.Notification.IsMonitoring)
			{
				return;
			}
			this.HubsCreated.Add(currentNotification.Notification);
		}

		private class AzureHubCreationNotificationComparer : IEqualityComparer<AzureHubCreationNotification>
		{
			public bool Equals(AzureHubCreationNotification x, AzureHubCreationNotification y)
			{
				return (x == null && y == null) || (x != null && y != null && x.AppId == y.AppId && x.HubName == y.HubName);
			}

			public int GetHashCode(AzureHubCreationNotification obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return obj.AppId.GetHashCode() ^ obj.HubName.GetHashCode();
			}

			public static readonly AzureHubCreationChannel.AzureHubCreationNotificationComparer Instance = new AzureHubCreationChannel.AzureHubCreationNotificationComparer();
		}
	}
}
