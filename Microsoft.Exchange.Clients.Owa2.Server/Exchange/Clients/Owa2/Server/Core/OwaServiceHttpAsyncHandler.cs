using System;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Web;
using Microsoft.Exchange.Clients.Owa2.Server.Diagnostics;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaServiceHttpAsyncHandler : OwaServiceHttpHandler, IHttpAsyncHandler, IHttpHandler
	{
		internal OwaServiceHttpAsyncHandler(HttpContext httpContext, OWAService service, ServiceMethodInfo methodInfo) : base(httpContext, service, methodInfo)
		{
		}

		public IAsyncResult BeginProcessRequest(HttpContext httpContext, AsyncCallback cb, object extraData)
		{
			OwaServerLogger.LogWcfLatency(httpContext);
			IAsyncResult result;
			try
			{
				result = this.InternalBeginProcessRequest(httpContext, cb, extraData);
			}
			catch (Exception ex)
			{
				base.FaultHandler.ProvideFault(ex, httpContext.Response);
				result = new AsyncResult(cb, null, true)
				{
					Exception = ex
				};
			}
			return result;
		}

		public void EndProcessRequest(IAsyncResult result)
		{
			AsyncResult asyncResult = result as AsyncResult;
			if (asyncResult != null && asyncResult.Exception != null)
			{
				return;
			}
			HttpContext.Current = base.HttpContext;
			try
			{
				this.InternalEndProcessRequest(result);
			}
			catch (Exception error)
			{
				base.FaultHandler.ProvideFault(error, base.HttpContext.Response);
			}
		}

		private IAsyncResult InternalBeginProcessRequest(HttpContext httpContext, AsyncCallback cb, object extraData)
		{
			try
			{
				HttpRequest request = httpContext.Request;
				HttpResponse response = httpContext.Response;
				base.Initialize(response);
				if (base.ServiceMethodInfo.IsHttpGet)
				{
					return base.MethodDispatcher.InvokeBeginGetMethod(base.ServiceMethodInfo, base.Service, request, cb);
				}
				return base.MethodDispatcher.InvokeBeginMethod(base.ServiceMethodInfo, base.Service, request, cb);
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
			return null;
		}

		private void InternalEndProcessRequest(IAsyncResult result)
		{
			try
			{
				base.MethodDispatcher.InvokeEndMethod(base.ServiceMethodInfo, base.Service, result, base.HttpContext.Response);
			}
			catch (TargetInvocationException ex)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<Exception>(0L, "Method invocation target threw an exception: {0}", ex.InnerException);
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
