using System;
using System.Web;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;

namespace Microsoft.Exchange.RpcHttpModules
{
	public abstract class RpcHttpModule : IHttpModule
	{
		internal HttpApplicationBase Application
		{
			get
			{
				return this.httpApplication;
			}
		}

		public virtual void Init(HttpApplication application)
		{
			this.httpApplication = new HttpApplicationWrapper(application);
			this.InitializeModule(application);
		}

		public virtual void Dispose()
		{
		}

		internal abstract void InitializeModule(HttpApplication application);

		internal virtual void OnBeginRequest(HttpContextBase context)
		{
		}

		internal virtual void OnAuthorizeRequest(HttpContextBase context)
		{
		}

		internal virtual void OnPostAuthorizeRequest(HttpContextBase context)
		{
		}

		internal virtual void OnEndRequest(HttpContextBase context)
		{
		}

		internal void SetMockApplicationWrapper(HttpApplicationBase application)
		{
			this.httpApplication = application;
		}

		internal void SendErrorResponse(HttpContextBase context, int httpStatusCode, string httpStatusText)
		{
			this.SendErrorResponse(context, httpStatusCode, 0, httpStatusText);
		}

		internal void SendErrorResponse(HttpContextBase context, int httpStatusCode, int httpSubStatusCode, string httpStatusText)
		{
			this.SendErrorResponse(context, httpStatusCode, httpSubStatusCode, httpStatusText, null);
		}

		internal void SendErrorResponse(HttpContextBase context, int httpStatusCode, int httpSubStatusCode, string httpStatusText, Action<HttpResponseBase> customResponseAction)
		{
			HttpResponseBase response = context.Response;
			response.Clear();
			response.StatusCode = httpStatusCode;
			response.SubStatusCode = httpSubStatusCode;
			response.StatusDescription = httpStatusText;
			if (customResponseAction != null)
			{
				customResponseAction(response);
			}
			this.Application.CompleteRequest();
		}

		protected string GetOutlookSessionId(HttpContextBase context)
		{
			string text = context.Items["OutlookSession"] as string;
			if (string.IsNullOrEmpty(text))
			{
				HttpCookie httpCookie = context.Request.Cookies["OutlookSession"];
				if (httpCookie != null)
				{
					text = httpCookie.Value.Trim();
					context.Items["OutlookSession"] = text;
				}
			}
			return text;
		}

		protected Guid GetRequestId(HttpContextBase context)
		{
			if (context.Items["LogRequestId"] != null)
			{
				return (Guid)context.Items["LogRequestId"];
			}
			ActivityScope activityScope = null;
			Guid activityId;
			try
			{
				IActivityScope activityScope2 = ActivityContext.GetCurrentActivityScope();
				if (activityScope2 == null)
				{
					activityScope = ActivityContext.Start(null);
					activityScope2 = activityScope;
				}
				activityScope2.UpdateFromMessage(context.Request);
				context.Items["LogRequestId"] = activityScope2.ActivityId;
				activityId = activityScope2.ActivityId;
			}
			finally
			{
				if (activityScope != null)
				{
					activityScope.Dispose();
				}
			}
			return activityId;
		}

		public const int HttpStatusCodeRoutingError = 555;

		public const string SessionIdPrefix = "SessionId=";

		internal const string OutlookSessionCookieName = "OutlookSession";

		internal const string RequestIdHttpContextKeyName = "LogRequestId";

		private HttpApplicationBase httpApplication;
	}
}
