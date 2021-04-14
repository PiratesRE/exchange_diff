using System;
using System.Reflection;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaEventAsyncHttpHandler : IHttpAsyncHandler, IHttpHandler
	{
		internal OwaEventAsyncHttpHandler(OwaEventHandlerBase eventHandler)
		{
			this.eventHandler = eventHandler;
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		public OwaEventHandlerBase EventHandler
		{
			get
			{
				return this.eventHandler;
			}
		}

		public void ProcessRequest(HttpContext httpContext)
		{
		}

		public IAsyncResult BeginProcessRequest(HttpContext httpContext, AsyncCallback callback, object context)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventAsyncHttpHandler.BeginProcessRequest");
			string text = httpContext.Request.QueryString["X-SuiteServiceProxyOrigin"] ?? httpContext.Request.Headers["X-SuiteServiceProxyOrigin"];
			if (!string.IsNullOrWhiteSpace(text))
			{
				httpContext.Response.AppendToLog("&SuiteServiceOrigin=" + text);
			}
			ExTraceGlobals.OehTracer.TraceDebug(0L, "Parsing request");
			OwaEventParserBase parser = this.GetParser();
			this.EventHandler.SetParameterTable(parser.Parse());
			ExTraceGlobals.OehTracer.TraceDebug(0L, "Invoking handler");
			object[] parameters = new object[]
			{
				callback,
				context
			};
			object obj = this.EventHandler.EventInfo.BeginMethodInfo.Invoke(this.EventHandler, BindingFlags.InvokeMethod, null, parameters, null);
			return (IAsyncResult)obj;
		}

		public void EndProcessRequest(IAsyncResult asyncResult)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventAsyncHttpHandler.EndProcessRequest");
			try
			{
				object[] parameters = new object[]
				{
					asyncResult
				};
				this.EventHandler.EventInfo.EndMethodInfo.Invoke(this.EventHandler, BindingFlags.InvokeMethod, null, parameters, null);
				this.FinishSuccesfulRequest();
			}
			catch (ThreadAbortException)
			{
			}
			catch (TargetInvocationException)
			{
				ExTraceGlobals.OehTracer.TraceDebug(0L, "Handler async end invocation target threw an exception");
			}
			finally
			{
				if (this.eventHandler != null)
				{
					this.eventHandler.Dispose();
				}
				this.eventHandler = null;
			}
		}

		internal OwaEventParserBase GetParser()
		{
			return new OwaEventUrlParser(this.EventHandler);
		}

		protected void FinishSuccesfulRequest()
		{
			if (!this.EventHandler.DontWriteHeaders)
			{
				this.EventHandler.HttpContext.Response.AppendHeader("X-OWA-EventResult", "0");
				HttpUtilities.MakePageNoCacheNoStore(this.EventHandler.HttpContext.Response);
				this.EventHandler.HttpContext.Response.ContentType = HttpUtilities.GetContentTypeString(this.EventHandler.ResponseContentType);
			}
		}

		private OwaEventHandlerBase eventHandler;
	}
}
