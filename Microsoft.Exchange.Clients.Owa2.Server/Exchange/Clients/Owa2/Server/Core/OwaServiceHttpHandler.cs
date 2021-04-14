using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaServiceHttpHandler : OwaServiceHttpHandlerBase, IHttpHandler
	{
		internal OwaServiceHttpHandler(HttpContext httpContext, OWAService service, ServiceMethodInfo methodInfo) : base(httpContext, service, methodInfo)
		{
			this.FaultHandler = new OWAFaultHandler();
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		private protected OWAFaultHandler FaultHandler { protected get; private set; }

		public void ProcessRequest(HttpContext httpContext)
		{
			try
			{
				this.InternalProcessRequest(httpContext);
			}
			catch (Exception error)
			{
				this.FaultHandler.ProvideFault(error, httpContext.Response);
			}
		}

		private void InternalProcessRequest(HttpContext httpContext)
		{
			ExTraceGlobals.CoreTracer.TraceDebug(0L, "OwaServiceHttpHandler.ProcessRequest");
			OwaServerLogger.LogWcfLatency(httpContext);
			try
			{
				HttpRequest request = httpContext.Request;
				HttpResponse response = httpContext.Response;
				base.Initialize(response);
				if (base.ServiceMethodInfo.IsHttpGet)
				{
					Uri url = request.Url;
					new Uri(request.Path, UriKind.Relative);
					base.MethodDispatcher.InvokeGetMethod(base.ServiceMethodInfo, base.Service, request, response);
				}
				else
				{
					base.MethodDispatcher.InvokeMethod(base.ServiceMethodInfo, base.Service, request, response);
				}
			}
			catch (TargetInvocationException ex)
			{
				ExTraceGlobals.CoreTracer.TraceError<Exception>(0L, "Method invocation target threw an exception: {0}", ex.InnerException);
				ExceptionDispatchInfo exceptionDispatchInfo = ExceptionDispatchInfo.Capture(ex.InnerException ?? ex);
				exceptionDispatchInfo.Throw();
			}
			finally
			{
				base.MethodDispatcher.DisposeParameters();
			}
		}
	}
}
