using System;
using System.Web;
using Microsoft.Exchange.Services.Core;
using Microsoft.Exchange.Services.DispatchPipe.Base;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Services.DispatchPipe.Ews
{
	internal class EwsServiceHttpAsyncHandler : IHttpAsyncHandler, IHttpHandler
	{
		public EwsServiceHttpAsyncHandler(HttpContext httpContext, EWSService service, ServiceMethodInfo methodInfo)
		{
			this.httpContext = httpContext;
			this.ewsMethodDispatcher = new EwsMethodDispatcher(service, methodInfo);
		}

		public IAsyncResult BeginProcessRequest(HttpContext httpContext, AsyncCallback callback, object state)
		{
			IAsyncResult asyncResult = null;
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				asyncResult = this.ewsMethodDispatcher.InvokeBeginMethod(httpContext, callback, state);
			});
			return asyncResult;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
			{
				this.ewsMethodDispatcher.InvokeEndMethod(result, this.httpContext.Response);
			});
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			throw new NotImplementedException();
		}

		private HttpContext httpContext;

		private EwsMethodDispatcher ewsMethodDispatcher;
	}
}
