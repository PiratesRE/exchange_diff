using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Utils;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzureDeviceRegistrationChannel : AzureBaseChannel<AzureDeviceRegistrationNotification>
	{
		public AzureDeviceRegistrationChannel(AzureDeviceRegistrationChannelSettings settings, ITracer tracer, EventHandler<MissingHubEventArgs> missingHubHandler, AzureClient azureClient = null, AzureErrorTracker errorTracker = null, Cache<string, CachableString> devicesRegistered = null, FifoDictionaryCache<string, string> registrationIds = null) : base(settings.AppId, tracer, azureClient, missingHubHandler)
		{
			ArgumentValidator.ThrowIfNull("settings", settings);
			settings.Validate();
			this.State = AzureDeviceRegistrationChannelState.Init;
			this.Settings = settings;
			this.ErrorTracker = (errorTracker ?? new AzureErrorTracker(this.Settings.BackOffTimeInSeconds));
			this.DevicesRegistered = (devicesRegistered ?? new Cache<string, CachableString>((long)(41 * this.Settings.MaxDevicesRegistrationCacheSize), TimeSpan.FromHours(6.0), TimeSpan.FromSeconds(0.0)));
			this.RegistrationIds = (registrationIds ?? new FifoDictionaryCache<string, string>(this.Settings.MaxDevicesRegistrationCacheSize, null));
		}

		public AzureDeviceRegistrationChannelState State { get; private set; }

		private AzureDeviceRegistrationChannelSettings Settings { get; set; }

		private AzureErrorTracker ErrorTracker { get; set; }

		private Cache<string, CachableString> DevicesRegistered { get; set; }

		private FifoDictionaryCache<string, string> RegistrationIds { get; set; }

		public override void Send(AzureDeviceRegistrationNotification notification, CancellationToken cancelToken)
		{
			base.CheckDisposed();
			ArgumentValidator.ThrowIfNull("notification", notification);
			if (!notification.IsValid)
			{
				this.OnInvalidNotificationFound(new InvalidNotificationEventArgs(notification, new InvalidPushNotificationException(notification.ValidationErrors[0])));
				return;
			}
			PushNotificationChannelContext<AzureDeviceRegistrationNotification> pushNotificationChannelContext = new PushNotificationChannelContext<AzureDeviceRegistrationNotification>(notification, cancelToken, base.Tracer);
			AzureDeviceRegistrationChannelState azureDeviceRegistrationChannelState = this.State;
			while (pushNotificationChannelContext.IsActive)
			{
				this.CheckCancellation(pushNotificationChannelContext);
				switch (this.State)
				{
				case AzureDeviceRegistrationChannelState.Init:
					azureDeviceRegistrationChannelState = this.ProcessInit(pushNotificationChannelContext);
					break;
				case AzureDeviceRegistrationChannelState.ReadRegistration:
					azureDeviceRegistrationChannelState = this.ProcessReadRegistration(pushNotificationChannelContext);
					break;
				case AzureDeviceRegistrationChannelState.CreateRegistrationId:
					azureDeviceRegistrationChannelState = this.ProcessCreateRegistrationId(pushNotificationChannelContext);
					break;
				case AzureDeviceRegistrationChannelState.Sending:
					azureDeviceRegistrationChannelState = this.ProcessSending(pushNotificationChannelContext);
					break;
				case AzureDeviceRegistrationChannelState.Discarding:
					azureDeviceRegistrationChannelState = this.ProcessDiscarding(pushNotificationChannelContext);
					break;
				default:
					pushNotificationChannelContext.Drop(null);
					azureDeviceRegistrationChannelState = AzureDeviceRegistrationChannelState.Sending;
					break;
				}
				base.Tracer.TraceDebug<AzureDeviceRegistrationChannelState, AzureDeviceRegistrationChannelState>((long)this.GetHashCode(), "[Send] Transitioning from {0} to {1}", this.State, azureDeviceRegistrationChannelState);
				this.State = azureDeviceRegistrationChannelState;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AzureDeviceRegistrationChannel>(this);
		}

		private void CheckCancellation(PushNotificationChannelContext<AzureDeviceRegistrationNotification> currentNotification)
		{
			if (currentNotification.IsCancelled)
			{
				base.Tracer.TraceDebug<AzureDeviceRegistrationChannelState>((long)this.GetHashCode(), "[CheckCancellation] Cancellation requested. Current state is {0}", this.State);
				throw new OperationCanceledException();
			}
		}

		private AzureDeviceRegistrationChannelState ProcessInit(PushNotificationChannelContext<AzureDeviceRegistrationNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureDeviceRegistrationNotification>>((long)this.GetHashCode(), "[ProcessInit] Initial State for notification '{0}'", currentNotification);
			if (this.DevicesRegistered.ContainsKey(currentNotification.Notification.RecipientId))
			{
				if (PushNotificationsCrimsonEvents.ElementAlreadyInCache.IsEnabled(PushNotificationsCrimsonEvent.Provider))
				{
					PushNotificationsCrimsonEvents.ElementAlreadyInCache.Log<string, string>(base.AppId, currentNotification.Notification.RecipientId);
				}
				currentNotification.Done();
				return AzureDeviceRegistrationChannelState.Init;
			}
			if (this.RegistrationIds.ContainsKey(currentNotification.Notification.RecipientId))
			{
				return AzureDeviceRegistrationChannelState.Sending;
			}
			return AzureDeviceRegistrationChannelState.ReadRegistration;
		}

		private AzureDeviceRegistrationChannelState ProcessDiscarding(PushNotificationChannelContext<AzureDeviceRegistrationNotification> currentNotification)
		{
			if (this.ErrorTracker.ShouldBackOff)
			{
				currentNotification.Drop(null);
				return AzureDeviceRegistrationChannelState.Discarding;
			}
			this.ErrorTracker.Reset();
			return AzureDeviceRegistrationChannelState.Init;
		}

		private AzureDeviceRegistrationChannelState ProcessReadRegistration(PushNotificationChannelContext<AzureDeviceRegistrationNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureDeviceRegistrationNotification>>((long)this.GetHashCode(), "[ProcessReadRegistration] Reading device for notification '{0}'", currentNotification);
			currentNotification.Notification.Validate();
			AzureUriTemplate uriTemplate = currentNotification.Notification.UriTemplate;
			string text = uriTemplate.CreateReadRegistrationStringUri(currentNotification.Notification);
			using (AzureReadRegistrationRequest azureReadRegistrationRequest = new AzureReadRegistrationRequest(this.CreateAzureSasToken(currentNotification.Notification.AzureSasTokenProvider, text), text))
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
						if (PushNotificationsCrimsonEvents.AzureDeviceReadRegistrationSucceeded.IsEnabled(PushNotificationsCrimsonEvent.Provider))
						{
							PushNotificationsCrimsonEvents.AzureDeviceReadRegistrationSucceeded.Log<string, string>(base.AppId, azureReadRegistrationResponse.ToTraceString());
						}
						if (!azureReadRegistrationResponse.HasRegistration)
						{
							return AzureDeviceRegistrationChannelState.CreateRegistrationId;
						}
						this.RegistrationIds.Add(currentNotification.Notification.RecipientId, azureReadRegistrationResponse.RegistrationId);
						if (azureReadRegistrationResponse.ExpirationTimeUtc <= ExDateTime.UtcNow.AddHours(24.0))
						{
							return AzureDeviceRegistrationChannelState.Sending;
						}
						this.AddRegistrationToDevicesRegistered(currentNotification.Notification);
						currentNotification.Done();
						this.ErrorTracker.ReportSuccess();
					}
					else
					{
						if (!string.IsNullOrEmpty(currentNotification.Notification.HubName) && azureReadRegistrationResponse.OriginalStatusCode != null && azureReadRegistrationResponse.OriginalStatusCode.Value == HttpStatusCode.NotFound)
						{
							this.FireMissingHubEvent(currentNotification.Notification.TargetAppId, currentNotification.Notification.HubName);
						}
						else
						{
							this.AnalyzeErrorResponse(azureReadRegistrationRequest, azureReadRegistrationResponse, currentNotification);
						}
						currentNotification.Drop(azureReadRegistrationResponse.ToTraceString());
						if (this.ErrorTracker.ShouldBackOff)
						{
							base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessReadRegistration] Backing off because of Authentication errors until {0}", this.ErrorTracker.BackOffEndTime);
							PushNotificationsCrimsonEvents.AzureDeviceRegistrationChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
							return AzureDeviceRegistrationChannelState.Discarding;
						}
					}
				}
			}
			return AzureDeviceRegistrationChannelState.Init;
		}

		private AzureDeviceRegistrationChannelState ProcessCreateRegistrationId(PushNotificationChannelContext<AzureDeviceRegistrationNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureDeviceRegistrationNotification>>((long)this.GetHashCode(), "[ProcessCreateRegistrationId] Registering device for notification '{0}'", currentNotification);
			currentNotification.Notification.Validate();
			AzureUriTemplate uriTemplate = currentNotification.Notification.UriTemplate;
			string text = uriTemplate.CreateNewRegistrationIdStringUri(currentNotification.Notification);
			using (AzureNewRegistrationIdRequest azureNewRegistrationIdRequest = new AzureNewRegistrationIdRequest(currentNotification.Notification, this.CreateAzureSasToken(currentNotification.Notification.AzureSasTokenProvider, text), text))
			{
				base.LogAzureRequest(currentNotification, azureNewRegistrationIdRequest);
				azureNewRegistrationIdRequest.Timeout = this.Settings.RequestTimeout;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginNewRegistrationIdRequest(azureNewRegistrationIdRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureNewRegistrationIdResponse azureNewRegistrationIdResponse = base.AzureClient.EndNewRegistrationIdRequest(asyncResult);
				stopwatch.Stop();
				if (flag)
				{
					if (azureNewRegistrationIdResponse.HasSucceeded)
					{
						if (PushNotificationsCrimsonEvents.AzureDeviceNewRegistrationIdSucceeded.IsEnabled(PushNotificationsCrimsonEvent.Provider))
						{
							PushNotificationsCrimsonEvents.AzureDeviceNewRegistrationIdSucceeded.Log<string, string>(base.AppId, azureNewRegistrationIdResponse.ToTraceString());
						}
						this.RegistrationIds.Add(currentNotification.Notification.RecipientId, azureNewRegistrationIdResponse.RegistrationId);
						return AzureDeviceRegistrationChannelState.Sending;
					}
					if (!string.IsNullOrEmpty(currentNotification.Notification.HubName) && azureNewRegistrationIdResponse.OriginalStatusCode != null && azureNewRegistrationIdResponse.OriginalStatusCode.Value == HttpStatusCode.NotFound)
					{
						this.FireMissingHubEvent(currentNotification.Notification.TargetAppId, currentNotification.Notification.HubName);
					}
					else
					{
						this.AnalyzeErrorResponse(azureNewRegistrationIdRequest, azureNewRegistrationIdResponse, currentNotification);
					}
					currentNotification.Drop(azureNewRegistrationIdResponse.ToTraceString());
					if (this.ErrorTracker.ShouldBackOff)
					{
						base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessCreateRegistrationId] Backing off because of Authentication errors until {0}", this.ErrorTracker.BackOffEndTime);
						PushNotificationsCrimsonEvents.AzureDeviceRegistrationChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
						return AzureDeviceRegistrationChannelState.Discarding;
					}
				}
			}
			return AzureDeviceRegistrationChannelState.Init;
		}

		private AzureDeviceRegistrationChannelState ProcessSending(PushNotificationChannelContext<AzureDeviceRegistrationNotification> currentNotification)
		{
			base.Tracer.TraceDebug<PushNotificationChannelContext<AzureDeviceRegistrationNotification>>((long)this.GetHashCode(), "[ProcessSending] Registering device for notification '{0}'", currentNotification);
			currentNotification.Notification.Validate();
			string registrationId;
			this.RegistrationIds.TryGetValue(currentNotification.Notification.RecipientId, out registrationId);
			AzureUriTemplate uriTemplate = currentNotification.Notification.UriTemplate;
			string text = uriTemplate.CreateOrUpdateRegistrationStringUri(currentNotification.Notification, registrationId);
			using (AzureCreateOrUpdateRegistrationRequest azureCreateOrUpdateRegistrationRequest = new AzureCreateOrUpdateRegistrationRequest(currentNotification.Notification, this.CreateAzureSasToken(currentNotification.Notification.AzureSasTokenProvider, text), text))
			{
				base.LogAzureRequest(currentNotification, azureCreateOrUpdateRegistrationRequest);
				azureCreateOrUpdateRegistrationRequest.Timeout = this.Settings.RequestTimeout;
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				ICancelableAsyncResult asyncResult = base.AzureClient.BeginCreateOrUpdateRegistrationRequest(azureCreateOrUpdateRegistrationRequest);
				bool flag = base.WaitUntilDoneOrCancelled(asyncResult, currentNotification, this.Settings.RequestStepTimeout);
				AzureResponse azureResponse = base.AzureClient.EndCreateOrUpdateRegistrationRequest(asyncResult);
				stopwatch.Stop();
				if (flag)
				{
					if (azureResponse.HasSucceeded)
					{
						base.LogAzureResponse(currentNotification, azureResponse);
						if (currentNotification.Notification.IsMonitoring)
						{
							PushNotificationsMonitoring.PublishSuccessNotification("DeviceRegistrationProcessed", currentNotification.Notification.TargetAppId);
						}
						this.AddRegistrationToDevicesRegistered(currentNotification.Notification);
						currentNotification.Done();
						this.ErrorTracker.ReportSuccess();
					}
					else
					{
						this.AnalyzeErrorResponse(azureCreateOrUpdateRegistrationRequest, azureResponse, currentNotification);
						currentNotification.Drop(azureResponse.ToTraceString());
						if (this.ErrorTracker.ShouldBackOff)
						{
							base.Tracer.TraceError<ExDateTime>((long)this.GetHashCode(), "[ProcessSending] Backing off because of Authentication errors until {0}", this.ErrorTracker.BackOffEndTime);
							PushNotificationsCrimsonEvents.AzureDeviceRegistrationChannelTransitionToDiscarding.Log<string, ExDateTime>(base.AppId, this.ErrorTracker.BackOffEndTime);
							PushNotificationsMonitoring.PublishFailureNotification("AzureDeviceRegistrationChannelBackOff", base.AppId, azureResponse.ToTraceString());
							return AzureDeviceRegistrationChannelState.Discarding;
						}
					}
				}
			}
			return AzureDeviceRegistrationChannelState.Init;
		}

		private void AddRegistrationToDevicesRegistered(AzureDeviceRegistrationNotification deviceRegistration)
		{
			if (deviceRegistration.IsMonitoring)
			{
				return;
			}
			if (!this.DevicesRegistered.TryAdd(deviceRegistration.RecipientId, AzureDeviceRegistrationChannel.EmptyCachableString))
			{
				PushNotificationsCrimsonEvents.AzureDeviceRecipientIdRejectedByCache.LogPeriodic<string, string>(deviceRegistration.UriTemplate.UriTemplate, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, string.Format("Item:{0} - Size:{1}", deviceRegistration.ToFullString(), this.DevicesRegistered.Count));
			}
		}

		private void AnalyzeErrorResponse(AzureRequestBase request, AzureResponse response, PushNotificationChannelContext<AzureDeviceRegistrationNotification> currentNotification)
		{
			if (response.OriginalStatusCode == null)
			{
				this.ErrorTracker.ReportError(AzureErrorType.Unknown);
				base.LogError("[AnalyzeErrorResponse] An unexpected error occurred sending a notification to Azure Notification Hub for Notification: '{0}'. Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureDeviceRegistrationChannelUnknownError.LogPeriodic<string, string, string, string, string>));
				return;
			}
			HttpStatusCode value = response.OriginalStatusCode.Value;
			if (value <= HttpStatusCode.RequestEntityTooLarge)
			{
				switch (value)
				{
				case HttpStatusCode.BadRequest:
				case HttpStatusCode.NotFound:
					this.ErrorTracker.ReportError(AzureErrorType.Permanent);
					base.LogError("[AnalyzeErrorResponse] Permanent Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureDeviceRegistrationChannelPermanentError.LogPeriodic<string, string, string, string, string>));
					return;
				case HttpStatusCode.Unauthorized:
				case HttpStatusCode.Forbidden:
					break;
				case HttpStatusCode.PaymentRequired:
					goto IL_F3;
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
			base.LogError("[AnalyzeErrorResponse] Transient Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureDeviceRegistrationChannelTransientError.LogPeriodic<string, string, string, string, string>));
			return;
			IL_F3:
			this.ErrorTracker.ReportError(AzureErrorType.Unknown);
			base.LogError("[AnalyzeErrorResponse] Unknown Error; Notification: '{0}'; Request: {1}; Response: {2}.", currentNotification.Notification, request, response, new Func<object, TimeSpan, string, string, string, string, string, bool>(PushNotificationsCrimsonEvents.AzureDeviceRegistrationChannelUnknownError.LogPeriodic<string, string, string, string, string>));
		}

		private const int DefaultCacheExpirationTimeInHours = 6;

		private const int DefaultHashedDeviceIdSizeInBytes = 41;

		private static readonly CachableString EmptyCachableString = new CachableString(string.Empty);
	}
}
