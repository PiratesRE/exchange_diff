using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal abstract class AzureBaseChannel<TNotif> : PushNotificationChannel<TNotif> where TNotif : PushNotification
	{
		public AzureBaseChannel(string appId, ITracer tracer, AzureClient azureClient = null, EventHandler<MissingHubEventArgs> missingHubHandler = null) : base(appId, tracer)
		{
			this.MissingHubDetected = missingHubHandler;
			this.AzureClient = (azureClient ?? new AzureClient(new HttpClient()));
		}

		private event EventHandler<MissingHubEventArgs> MissingHubDetected;

		private protected AzureClient AzureClient { protected get; private set; }

		protected virtual void FireMissingHubEvent(string targetAppId, string hubName)
		{
			if (this.MissingHubDetected != null)
			{
				this.MissingHubDetected(this, new MissingHubEventArgs(targetAppId, hubName));
			}
			PushNotificationsCrimsonEvents.MissingAzureHub.LogPeriodic<string, string, bool>(hubName, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, hubName, this.MissingHubDetected != null);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				base.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[InternalDispose] Disposing the channel for '{0}'", base.AppId);
				this.AzureClient.Dispose();
			}
		}

		protected virtual AzureSasToken CreateAzureSasToken(IAzureSasTokenProvider sasTokenProvider, string targetResourceUri)
		{
			ArgumentValidator.ThrowIfNull("sasTokenProvider", sasTokenProvider);
			AzureSasToken azureSasToken = sasTokenProvider.CreateSasToken(targetResourceUri);
			if (azureSasToken == null || !azureSasToken.IsValid())
			{
				PushNotificationsCrimsonEvents.InvalidAzureSasToken.LogPeriodic<string, string>(base.AppId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, azureSasToken.ToNullableString(null));
				throw new PushNotificationPermanentException(Strings.CannotCreateValidSasToken(base.AppId, azureSasToken.ToNullableString(null)));
			}
			return azureSasToken;
		}

		protected void LogAzureResponse(PushNotificationChannelContext<TNotif> notification, AzureResponse response)
		{
			if (base.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ITracer tracer = base.Tracer;
				long id = (long)this.GetHashCode();
				string formatString = "[LogAzureResponse] Azure Response {0} {1} '{2}'";
				string appId = base.AppId;
				TNotif notification2 = notification.Notification;
				tracer.TraceDebug<string, string, string>(id, formatString, appId, notification2.Identifier, response.ToTraceString());
			}
			if (PushNotificationsCrimsonEvents.AzureNotificationResponse.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				AzureNotificationResponseEvent azureNotificationResponse = PushNotificationsCrimsonEvents.AzureNotificationResponse;
				string appId2 = base.AppId;
				TNotif notification3 = notification.Notification;
				azureNotificationResponse.Log<string, string, string>(appId2, notification3.Identifier, response.ToTraceString());
			}
		}

		protected void LogAzureRequest(PushNotificationChannelContext<TNotif> notification, AzureRequestBase request)
		{
			if (base.Tracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ITracer tracer = base.Tracer;
				long id = (long)this.GetHashCode();
				string formatString = "[LogAzureRequest] Azure Request {0} {1} '{2}'";
				string appId = base.AppId;
				TNotif notification2 = notification.Notification;
				tracer.TraceDebug<string, string, string>(id, formatString, appId, notification2.Identifier, request.ToTraceString());
			}
			if (PushNotificationsCrimsonEvents.AzureNotificationRequest.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				AzureNotificationRequestEvent azureNotificationRequest = PushNotificationsCrimsonEvents.AzureNotificationRequest;
				string appId2 = base.AppId;
				TNotif notification3 = notification.Notification;
				azureNotificationRequest.Log<string, string, string>(appId2, notification3.Identifier, request.ToTraceString());
			}
		}

		protected void LogError(string traceTemplate, TNotif notification, AzureRequestBase request, AzureResponse response, Func<object, TimeSpan, string, string, string, string, string, bool> log)
		{
			if (base.Tracer.IsTraceEnabled(TraceType.ErrorTrace))
			{
				string arg = string.Format(traceTemplate, notification.ToFullString(), request.ToTraceString(), response.ToTraceString());
				base.Tracer.TraceError((long)this.GetHashCode(), string.Format("AppId:{0}, Trace:{1}", base.AppId, arg));
			}
			log(notification.RecipientId, CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, base.AppId, notification.Identifier, notification.ToFullString(), request.ToTraceString(), response.ToTraceString());
		}
	}
}
