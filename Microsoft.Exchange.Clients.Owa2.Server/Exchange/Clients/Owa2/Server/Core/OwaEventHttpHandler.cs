using System;
using System.Reflection;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class OwaEventHttpHandler : IHttpHandler
	{
		internal OwaEventHttpHandler(OwaEventHandlerBase eventHandler)
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
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventHttpHandler.ProcessRequest");
			try
			{
				ExTraceGlobals.OehTracer.TraceDebug(0L, "Parsing request");
				string text = httpContext.Request.QueryString["X-SuiteServiceProxyOrigin"] ?? httpContext.Request.Headers["X-SuiteServiceProxyOrigin"];
				if (!string.IsNullOrWhiteSpace(text))
				{
					httpContext.Response.AppendToLog("&SuiteServiceOrigin=" + text);
				}
				OwaEventParserBase parser = this.GetParser();
				this.EventHandler.SetParameterTable(parser.Parse());
				ExTraceGlobals.OehTracer.TraceDebug<string>(0L, "Invoking event {0}", this.EventHandler.EventInfo.Name);
				this.EventHandler.EventInfo.MethodInfo.Invoke(this.EventHandler, BindingFlags.InvokeMethod, null, null, null);
				this.FinishSuccesfulRequest();
			}
			catch (ThreadAbortException)
			{
			}
			catch (TargetInvocationException)
			{
				ExTraceGlobals.OehTracer.TraceDebug(0L, "Handler invocation target threw an exception");
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
