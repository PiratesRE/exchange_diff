using System;
using System.Net;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications
{
	internal static class PushNotificationsLogHelper
	{
		public static void LogServerVersion()
		{
			PushNotificationsCrimsonEvents.ServerVersion.Log<string>("15.00.1497.012");
		}

		public static void LogOnPremPublishingResponse(WebHeaderCollection headers)
		{
			if (PushNotificationsCrimsonEvents.OnPremPublisherServiceProxyHeaders.IsEnabled(PushNotificationsCrimsonEvent.Provider))
			{
				PushNotificationsCrimsonEvents.OnPremPublisherServiceProxyHeaders.Log<string>(headers.ToTraceString(null));
			}
		}

		public static void LogOnPremPublishingError(Exception error, WebHeaderCollection headers)
		{
			if (error == null)
			{
				return;
			}
			PushNotificationsCrimsonEvents.OnPremPublisherServiceProxyError.LogPeriodic<string, string>(error.ToString(), CrimsonConstants.DefaultLogPeriodicSuppressionInMinutes, headers.ToTraceString(null), error.ToTraceString());
		}
	}
}
