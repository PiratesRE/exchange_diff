using System;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	public class PendingGetRequestHandler : IHttpAsyncHandler, IHttpHandler
	{
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
		{
			AsyncResult asyncResult = new AsyncResult(cb, extraData);
			string subscriptionId = PendingGetRequestHandler.GetSubscriptionId(context);
			IPendingGetConnection pendingGetConnection = PendingGetConnectionCache.Instance.AddOrGetConnection(subscriptionId);
			ExTraceGlobals.PendingGetPublisherTracer.TraceDebug<string>((long)this.GetHashCode(), "[BeginProcessRequest] PendingGetRequestHandler is requested from the client (subscription id = '{0}')", subscriptionId);
			long timeoutInMilliseconds = PendingGetRequestHandler.GetTimeoutInMilliseconds(context);
			int unseenEmailNotificationId = PendingGetRequestHandler.GetUnseenEmailNotificationId(context);
			PendingGetContext pendingGetContext = new PendingGetContext
			{
				AsyncResult = asyncResult,
				Response = new PendingGetResponse(context.Response)
			};
			pendingGetConnection.SubscribeToUnseenEmailNotification(pendingGetContext, timeoutInMilliseconds, unseenEmailNotificationId);
			return asyncResult;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			ExTraceGlobals.PendingGetPublisherTracer.TraceDebug((long)this.GetHashCode(), "[EndProcessRequest] PendingGetRequestHandler finishes request");
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}

		private static int GetUnseenEmailNotificationId(HttpContext context)
		{
			string parameter = PendingGetRequestHandler.GetParameter(context, "US");
			int result;
			if (int.TryParse(parameter, out result))
			{
				return result;
			}
			return 0;
		}

		private static long GetTimeoutInMilliseconds(HttpContext context)
		{
			string parameter = PendingGetRequestHandler.GetParameter(context, "T");
			long result;
			if (long.TryParse(parameter, out result))
			{
				return result;
			}
			return 0L;
		}

		private static string GetSubscriptionId(HttpContext context)
		{
			return PendingGetRequestHandler.GetParameter(context, "S");
		}

		private static string GetParameter(HttpContext context, string parameterName)
		{
			return context.Request.QueryString[parameterName];
		}

		private const string TimeoutParameter = "T";

		private const string UnseenEmailNotificationIdParameter = "US";

		private const string SubscriptionIdParameter = "S";
	}
}
