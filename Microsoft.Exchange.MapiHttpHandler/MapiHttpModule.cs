using System;
using System.Web;

namespace Microsoft.Exchange.MapiHttp
{
	public abstract class MapiHttpModule : IHttpModule
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

		internal virtual void OnPreRequestHandlerExecute(HttpContextBase context)
		{
		}

		internal virtual void OnPostRequestHandlerExecute(HttpContextBase context)
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

		private HttpApplicationBase httpApplication;
	}
}
