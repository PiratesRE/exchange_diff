using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
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
				OwaEventParserBase parser = this.GetParser();
				this.EventHandler.SetParameterTable(parser.Parse());
				ExTraceGlobals.OehTracer.TraceDebug<string>(0L, "Invoking event {0}", this.EventHandler.EventInfo.Name);
				if (Globals.CanaryProtectionRequired && !UserAgentUtilities.IsMonitoringRequest(httpContext.Request.UserAgent))
				{
					Utilities.VerifyEventHandlerCanary(this.EventHandler);
				}
				this.EventHandler.EventInfo.MethodInfo.Invoke(this.EventHandler, BindingFlags.InvokeMethod, null, null, null);
				this.FinishSuccesfulRequest();
			}
			catch (ThreadAbortException)
			{
				this.EventHandler.OwaContext.UnlockMinResourcesOnCriticalError();
			}
			catch (TargetInvocationException ex)
			{
				ExTraceGlobals.OehTracer.TraceDebug(0L, "Handler invocation target threw an exception");
				Utilities.HandleException(this.EventHandler.OwaContext, ex.InnerException, this.EventHandler.ShowErrorInPage);
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

		internal static void RenderError(OwaContext owaContext, TextWriter writer, string message, string messageDetails, OwaEventHandlerErrorCode errorCode, Exception exception)
		{
			if (owaContext == null)
			{
				throw new ArgumentNullException("owaContext");
			}
			owaContext.HttpContext.Response.Clear();
			OwaEventHttpHandler.RenderErrorDiv(owaContext, writer, message, messageDetails, errorCode, exception);
			if (owaContext.CustomErrorInfo != null)
			{
				writer.Write("<div id=divCstInfo");
				foreach (KeyValuePair<string, string> keyValuePair in owaContext.CustomErrorInfo)
				{
					writer.Write(" ");
					writer.Write(keyValuePair.Key);
					writer.Write("=");
					writer.Write("\"");
					writer.Write(Utilities.HtmlEncode(keyValuePair.Value));
					writer.Write("\"");
				}
				writer.Write("></div>");
				owaContext.CustomErrorInfo = null;
			}
			owaContext.HttpContext.Response.ContentType = Utilities.GetContentTypeString(OwaEventContentType.Html);
			owaContext.HttpContext.Response.Headers.Remove("X-OWA-EventResult");
			owaContext.HttpContext.Response.AppendHeader("X-OWA-EventResult", "1");
		}

		internal static void RenderErrorDiv(OwaContext owaContext, TextWriter writer, string message, string messageDetails, OwaEventHandlerErrorCode errorCode, Exception exception)
		{
			if (message == null)
			{
				throw new ArgumentNullException("message");
			}
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}
			writer.Write("<div id=err _msg=\"");
			Utilities.HtmlEncode(message, writer);
			writer.Write("\"");
			if (messageDetails != null)
			{
				writer.Write(" _details=\"");
				Utilities.HtmlEncode(messageDetails, writer);
				writer.Write("\"");
			}
			if (errorCode != OwaEventHandlerErrorCode.NotSet)
			{
				writer.Write(" _cd=");
				writer.Write((int)errorCode);
			}
			writer.Write(">");
			if (Globals.ShowDebugInformation && exception != null)
			{
				writer.Write("<div id=debugInfo>");
				Utilities.RenderDebugInformation(writer, owaContext, exception);
				writer.Write("</div>");
			}
			writer.Write("</div>");
		}

		protected void FinishSuccesfulRequest()
		{
			if (!this.EventHandler.DontWriteHeaders)
			{
				this.EventHandler.HttpContext.Response.AppendHeader("X-OWA-EventResult", "0");
				if (this.EventHandler.EventInfo.Name != "RenderADPhoto" && this.EventHandler.EventInfo.Name != "RenderImage")
				{
					Utilities.MakePageNoCacheNoStore(this.EventHandler.HttpContext.Response);
				}
				this.EventHandler.HttpContext.Response.ContentType = Utilities.GetContentTypeString(this.EventHandler.ResponseContentType);
			}
		}

		internal OwaEventParserBase GetParser()
		{
			OwaEventParserBase result;
			if (this.EventHandler.EventInfo.IsInternal)
			{
				result = new OwaEventInternalParser(this.EventHandler);
			}
			else
			{
				switch (this.EventHandler.Verb)
				{
				case OwaEventVerb.Post:
					result = new OwaEventXmlParser(this.EventHandler);
					break;
				case OwaEventVerb.Get:
					result = new OwaEventUrlParser(this.EventHandler);
					break;
				default:
					return null;
				}
			}
			return result;
		}

		protected OwaEventHandlerBase eventHandler;
	}
}
