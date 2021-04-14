using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureChannel : AzureBaseChannel<AzureNotification>
	{
		public AzureChannel(AzureChannelSettings settings, ITracer tracer, AzureClient azureClient = null, AzureErrorTracker errorTracker = null) : base(settings.AppId, tracer, azureClient, null)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.State = AzureChannelState.Init;
			this.Settings = settings;
			this.ErrorTracker = (errorTracker ?? new AzureErrorTracker(this.Settings.BackOffTimeInSeconds));
			this.DevicesRegistered = new Cache<string, CachableString>((long)(64 * this.Settings.MaxDevicesRegistrationCacheSize), TimeSpan.FromHours(6.0), TimeSpan.FromSeconds(0.0));
		}

		public AzureChannelState State { get; private set; }

		private AzureChannelSettings Settings { get; set; }

		private AzureErrorTracker ErrorTracker { get; set; }

		private Cache<string, CachableString> DevicesRegistered { get; set; }

		public override void Send(AzureNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			PushNotificationChannelContext<AzureNotification> pushNotificationChannelContext = new PushNotificationChannelContext<AzureNotification>(notification, cancelToken, base.Tracer);
			AzureChannelState azureChannelState = this.State;
			while (pushNotificationChannelContext.IsActive)
			{
				this.CheckCancellation(pushNotificationChannelContext);
				switch (this.State)
				{
				case AzureChannelState.Init:
					azureChannelState = this.ProcessInit(pushNotificationChannelContext);
					break;
				case AzureChannelState.ReadRegistration:
					azureChannelState = this.ProcessReadRegistration(pushNotificationChannelContext);
					break;
				case AzureChannelState.NewRegistration:
					azureChannelState = this.ProcessNewRegistration(pushNotificationChannelContext);
					break;
				case AzureChannelState.Sending:
					azureChannelState = this.ProcessSending(pushNotificationChannelContext);
					break;
				case AzureChannelState.Discarding:
					azureChannelState = this.ProcessDiscarding(pushNotificationChannelContext);
					break;
				default:
					pushNotificationChannelContext.Drop(null);
					azureChannelState = AzureChannelState.Sending;
					break;
				}
				base.Tracer.TraceDebug<AzureChannelState, AzureChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, azureChannelState);
				this.State = azureChannelState;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureChannel>(this);
		}

		private void CheckCancellation(PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				base.Tracer.TraceDebug<AzureChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private AzureChannelState ProcessInit(PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureNotification>>((long)this.GetHashCode(), "[ProcessInit] Initial State for notification '{0}'", currentNotification);
			if (!this.ShouldPerformRegistration(currentNotification) || this.DevicesRegistered.ContainsKey(currentNotification.Notification.RecipientId))
			{
				return AzureChannelState.Sending;
			}
			return AzureChannelState.ReadRegistration;
		}

		private AzureChannelState ProcessReadRegistration(PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureNotification>>((long)this.GetHashCode(), "[ProcessReadRegistration] Reading device for notification '{0}'", currentNotification);
			currentNotification.Notification.Validate();
			string text = this.Settings.UriTemplate.CreateReadRegistrationStringUri(currentNotification.Notification);
			using (AzureReadRegistrationRequest azureReadRegistrationRequest = new AzureReadRegistrationRequest(this.CreateAzureSasToken(this.Settings.AzureSasTokenProvider, text), text))
			{
				base.LogAzureRequest(currentNotification, azureReadRegistrationRequest);
				azureReadRegistrationRequest.Timeout = this.Settings.RequestTimeout;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginReadRegistrationRequest(azureReadRegistrationRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureReadRegistrationResponse azureReadRegistrationResponse = base.AzureClient.EndReadRegistrationRequest(asyncResult);
				stopwatch.Stop();
				if (flag)
				{
					if (azureReadRegistrationResponse.HasSucceeded)
					{
						if (PushNotificationsCrimsonEvents.AzureChannelReadRegistrationSucceeded.IsEnabled(PushNotificationsCrimsonEvent.Provider))
						{
							PushNotificationsCrimsonEvents.AzureChannelReadRegistrationSucceeded.Log<string, string>(base.AppId, azureReadRegistrationResponse.ToTraceString());
						}
						if (azureReadRegistrationResponse.HasRegistration)
						{
							this.AddRegistrationToDevicesRegistered(currentNotification.Notification);
							return AzureChannelState.Sending;
						}
						return AzureChannelState.NewRegistration;
					}
					else
					{
						base.Tracer.TraceError<string>((long)this.GetHashCode(), "[ProcessReadRegistration] An unexpected error occurred trying to read a device registration: {0}", azureReadRegistrationResponse.ToTraceString());
						this.ErrorTracker.ReportError(AzureErrorType.Transient);
						PushNotificationsCrimsonEvents.AzureChannelReadRegistrationError.LogPeriodic<string, string, string, string, string, long>(currentNotification.Notification.RecipientId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, currentNotification.Notification.Identifier, currentNotification.Notification.ToFullString(), azureReadRegistrationRequest.ToTraceString(), azureReadRegistrationResponse.ToTraceString(), stopwatch.ElapsedMilliseconds);
						currentNotification.Drop(azureReadRegistrationResponse.ToTraceString());
						if (this.ShouldBackOff(azureReadRegistrationResponse.ToTraceString()))
						{
							return AzureChannelState.Discarding;
						}
					}
				}
			}
			return AzureChannelState.Init;
		}

		private AzureChannelState ProcessNewRegistration(PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureNotification>>((long)this.GetHashCode(), "[ProcessNewRegistration] Registering device for notification '{0}'", currentNotification);
			currentNotification.Notification.Validate();
			string text = this.Settings.UriTemplate.CreateNewRegistrationStringUri(currentNotification.Notification);
			using (AzureNewRegistrationRequest azureNewRegistrationRequest = new AzureNewRegistrationRequest(currentNotification.Notification, this.CreateAzureSasToken(this.Settings.AzureSasTokenProvider, text), text, this.Settings.RegistrationTemplate))
			{
				base.LogAzureRequest(currentNotification, azureNewRegistrationRequest);
				azureNewRegistrationRequest.Timeout = this.Settings.RequestTimeout;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginNewRegistrationRequest(azureNewRegistrationRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureResponse azureResponse = base.AzureClient.EndNewRegistrationRequest(asyncResult);
				stopwatch.Stop();
				if (flag)
				{
					if (azureResponse.HasSucceeded)
					{
						if (PushNotificationsCrimsonEvents.AzureChannelNewRegistrationSucceeded.IsEnabled(PushNotificationsCrimsonEvent.Provider))
						{
							PushNotificationsCrimsonEvents.AzureChannelNewRegistrationSucceeded.Log<string, string>(base.AppId, azureResponse.ToTraceString());
						}
						this.AddRegistrationToDevicesRegistered(currentNotification.Notification);
						return AzureChannelState.Sending;
					}
					base.Tracer.TraceError<string>((long)this.GetHashCode(), "[ProcessNewRegistration] An unexpected error occurred trying to register a device: {0}", azureResponse.ToTraceString());
					this.ErrorTracker.ReportError(AzureErrorType.Transient);
					PushNotificationsCrimsonEvents.AzureChannelNewRegistrationError.LogPeriodic<string, string, string, string, string, long>(currentNotification.Notification.RecipientId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, currentNotification.Notification.Identifier, currentNotification.Notification.ToFullString(), azureNewRegistrationRequest.ToTraceString(), azureResponse.ToTraceString(), stopwatch.ElapsedMilliseconds);
					currentNotification.Drop(azureResponse.ToTraceString());
					if (this.ShouldBackOff(azureResponse.ToTraceString()))
					{
						return AzureChannelState.Discarding;
					}
				}
			}
			return AzureChannelState.Init;
		}

		private AzureChannelState ProcessSending(PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureNotification>>((long)this.GetHashCode(), "[ProcessSending] Sending notification '{0}'", currentNotification);
			currentNotification.Notification.Validate();
			string text = this.Settings.UriTemplate.CreateSendNotificationStringUri(currentNotification.Notification);
			string azureTag = null;
			CachableString cachableString;
			if (this.ShouldPerformRegistration(currentNotification) && this.DevicesRegistered.TryGetValue(currentNotification.Notification.RecipientId, out cachableString))
			{
				azureTag = cachableString.StringItem;
			}
			using (AzureSendRequest azureSendRequest = new AzureSendRequest(currentNotification.Notification, this.CreateAzureSasToken(this.Settings.AzureSasTokenProvider, text), text, azureTag))
			{
				base.LogAzureRequest(currentNotification, azureSendRequest);
				azureSendRequest.Timeout = this.Settings.RequestTimeout;
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginSendNotificationRequest(azureSendRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureResponse azureResponse = base.AzureClient.EndSendNotificationRequest(asyncResult);
				if (flag)
				{
					if (azureResponse.HasSucceeded)
					{
						base.LogAzureResponse(currentNotification, azureResponse);
						PushNotificationTracker.ReportSent(currentNotification.Notification, PushNotificationPlatform.Azure);
						if (currentNotification.Notification.IsMonitoring)
						{
							PushNotificationsMonitoring.PublishSuccessNotification("NotificationProcessed", base.AppId);
						}
						currentNotification.Done();
						this.ErrorTracker.ReportSuccess();
					}
					else
					{
						this.AnalyzeErrorResponse(azureSendRequest, azureResponse, currentNotification);
						currentNotification.Drop(azureResponse.ToTraceString());
						if (this.ShouldBackOff(azureResponse.ToTraceString()))
						{
							return AzureChannelState.Discarding;
						}
					}
				}
			}
			return AzureChannelState.Init;
		}

		private void AddRegistrationToDevicesRegistered(AzureNotification notification)
		{
			if (notification.IsMonitoring)
			{
				return;
			}
			if (!this.DevicesRegistered.TryAdd(notification.RecipientId, new CachableString(notification.RecipientId)))
			{
				PushNotificationsCrimsonEvents.AzureChannelRecipientIdRejectedByCache.LogPeriodic<string, string>(notification.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, string.Format("Item:{0} - Size:{1}", notification.ToFullString(), this.DevicesRegistered.Count));
			}
		}

		private void AnalyzeErrorResponse(AzureRequestBase request, AzureResponse response, PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			if (response.OriginalStatusCode == null)
			{
				this.ErrorTracker.ReportError(AzureErrorType.Unknown);
				base.LogError("[AnalyzeErrorResponse] An unexpected error occurred sending a notification to Azure Notification Hub for Notification: '{0}'. Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureChannelUnknownError.LogPeriodic<string, string, string, string, string>));
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
					base.LogError("[AnalyzeErrorResponse] Permanent Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureChannelPermanentError.LogPeriodic<string, string, string, string, string>));
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
			base.LogError("[AnalyzeErrorResponse] Transient Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureChannelTransientError.LogPeriodic<string, string, string, string, string>));
			return;
			IL_F3:
			this.ErrorTracker.ReportError(AzureErrorType.Unknown);
			base.LogError("[AnalyzeErrorResponse] Unknown Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureChannelUnknownError.LogPeriodic<string, string, string, string, string>));
		}

		private AzureChannelState ProcessDiscarding(PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				currentNotification.Drop(null);
				return AzureChannelState.Discarding;
			}
			this.ErrorTracker.Reset();
			return AzureChannelState.Sending;
		}

		private bool ShouldBackOff(string traces)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessNewRegistration] Backing off because of notification errors until {0}", this.ErrorTracker.BackOffEndTime);
				PushNotificationsCrimsonEvents.AzureChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
				PushNotificationsMonitoring.PublishFailureNotification("AzureChannelBackOff", base.AppId, traces);
				return true;
			}
			return false;
		}

		private bool ShouldPerformRegistration(PushNotificationChannelContext<AzureNotification> currentNotification)
		{
			return this.Settings.IsRegistrationEnabled || currentNotification.Notification.IsRegistrationEnabled;
		}

		private const int DefaultCacheExpirationTimeInHours = 6;

		private const int DefaultHashedDeviceIdSizeInBytes = 64;
	}
}
