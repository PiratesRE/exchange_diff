using System;
using System.Globalization;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public sealed class OwaEventHandlerFactory : IHttpHandlerFactory
	{
		public IHttpHandler GetHandler(HttpContext httpContext, string requestType, string url, string pathTranslated)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventHandlerFactory.GetHandler");
			string queryStringParameter = HttpUtilities.GetQueryStringParameter(httpContext.Request, "ns");
			string queryStringParameter2 = HttpUtilities.GetQueryStringParameter(httpContext.Request, "ev");
			ExTraceGlobals.OehDataTracer.TraceDebug<string, string>(0L, "Request namespace: '{0}', event: '{1}'", queryStringParameter, queryStringParameter2);
			HttpApplication applicationInstance = httpContext.ApplicationInstance;
			OwaEventRegistry owaEventRegistry = (OwaEventRegistry)applicationInstance.Application["OwaEventRegistry"];
			if (owaEventRegistry == null)
			{
				HttpUtilities.EndResponse(httpContext, HttpStatusCode.MethodNotAllowed);
				return null;
			}
			OwaEventNamespaceAttribute owaEventNamespaceAttribute = owaEventRegistry.FindNamespaceInfo(queryStringParameter);
			if (owaEventNamespaceAttribute == null)
			{
				throw new OwaException(string.Format(CultureInfo.InvariantCulture, "Namespace '{0}' doesn't exist", new object[]
				{
					queryStringParameter
				}), null, this);
			}
			OwaEventAttribute owaEventAttribute = owaEventNamespaceAttribute.FindEventInfo(queryStringParameter2);
			if (owaEventAttribute == null)
			{
				throw new OwaException(string.Format(CultureInfo.InvariantCulture, "Event '{0}' doesn't exist", new object[]
				{
					queryStringParameter2
				}), null, this);
			}
			OwaEventVerb owaEventVerb = OwaEventVerbAttribute.Parse(httpContext.Request.HttpMethod);
			ExTraceGlobals.OehDataTracer.TraceDebug<string>(0L, "Request verb: {0}", httpContext.Request.HttpMethod);
			if ((owaEventAttribute.AllowedVerbs & owaEventVerb) == OwaEventVerb.Unsupported)
			{
				ExTraceGlobals.OehTracer.TraceDebug<OwaEventVerb, OwaEventVerb>(0L, "Verb is not allowed, returning 405. Actual verb: {0}. Allowed: {1}.", owaEventVerb, owaEventAttribute.AllowedVerbs);
				HttpUtilities.EndResponse(httpContext, HttpStatusCode.MethodNotAllowed);
				return null;
			}
			OwaEventHandlerBase owaEventHandlerBase = (OwaEventHandlerBase)Activator.CreateInstance(owaEventNamespaceAttribute.HandlerType);
			owaEventHandlerBase.EventInfo = owaEventAttribute;
			owaEventHandlerBase.Verb = owaEventVerb;
			if (owaEventAttribute.IsAsync)
			{
				ExTraceGlobals.OehTracer.TraceDebug(0L, "Created async HTTP handler to server OEH request");
				return new OwaEventAsyncHttpHandler(owaEventHandlerBase);
			}
			return new OwaEventHttpHandler(owaEventHandlerBase);
		}

		public void ReleaseHandler(IHttpHandler handler)
		{
		}

		public const string EventRequestIdentifier = "oeh2";

		public const string NamespaceName = "ns";

		public const string EventName = "ev";
	}
}
