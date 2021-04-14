using System;
using System.Globalization;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaEventHandlerFactory : IHttpHandlerFactory
	{
		public IHttpHandler GetHandler(HttpContext httpContext, string requestType, string url, string pathTranslated)
		{
			ExTraceGlobals.OehCallTracer.TraceDebug(0L, "OwaEventHandlerFactory.GetHandler");
			string queryStringParameter = Utilities.GetQueryStringParameter(httpContext.Request, "ns");
			string queryStringParameter2 = Utilities.GetQueryStringParameter(httpContext.Request, "ev");
			ISessionContext sessionContext = OwaContext.Get(httpContext).SessionContext;
			ExTraceGlobals.OehDataTracer.TraceDebug<string, string>(0L, "Request namespace: '{0}', event: '{1}'", queryStringParameter, queryStringParameter2);
			OwaEventNamespaceAttribute owaEventNamespaceAttribute = OwaEventRegistry.FindNamespaceInfo(queryStringParameter);
			if (owaEventNamespaceAttribute == null)
			{
				throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Namespace '{0}' doesn't exist", new object[]
				{
					queryStringParameter
				}), null, this);
			}
			if (sessionContext != null && !sessionContext.IsProxy && !sessionContext.AreFeaturesEnabled(owaEventNamespaceAttribute.SegmentationFlags))
			{
				Utilities.EndResponse(httpContext, HttpStatusCode.Forbidden);
				return null;
			}
			OwaEventAttribute owaEventAttribute = owaEventNamespaceAttribute.FindEventInfo(queryStringParameter2);
			if (owaEventAttribute == null)
			{
				throw new OwaInvalidRequestException(string.Format(CultureInfo.InvariantCulture, "Event '{0}' doesn't exist", new object[]
				{
					queryStringParameter2
				}), null, this);
			}
			if (Globals.OwaVDirType == OWAVDirType.Calendar && !owaEventAttribute.AllowAnonymousAccess)
			{
				Utilities.EndResponse(httpContext, HttpStatusCode.BadRequest);
				return null;
			}
			if (sessionContext != null && !sessionContext.IsProxy && !sessionContext.AreFeaturesEnabled(owaEventAttribute.SegmentationFlags))
			{
				Utilities.EndResponse(httpContext, HttpStatusCode.Forbidden);
				return null;
			}
			OwaEventVerb owaEventVerb = OwaEventVerbAttribute.Parse(httpContext.Request.HttpMethod);
			ExTraceGlobals.OehDataTracer.TraceDebug<string>(0L, "Request verb: {0}", httpContext.Request.HttpMethod);
			if ((owaEventAttribute.AllowedVerbs & owaEventVerb) == OwaEventVerb.Unsupported)
			{
				ExTraceGlobals.OehTracer.TraceDebug<OwaEventVerb, OwaEventVerb>(0L, "Verb is not allowed, returning 405. Actual verb: {0}. Allowed: {1}.", owaEventVerb, owaEventAttribute.AllowedVerbs);
				Utilities.EndResponse(httpContext, HttpStatusCode.MethodNotAllowed);
				return null;
			}
			OwaEventHandlerBase owaEventHandlerBase = (OwaEventHandlerBase)Activator.CreateInstance(owaEventNamespaceAttribute.HandlerType);
			owaEventHandlerBase.EventInfo = owaEventAttribute;
			owaEventHandlerBase.OwaContext = OwaContext.Current;
			owaEventHandlerBase.Verb = owaEventVerb;
			if (Globals.CollectPerRequestPerformanceStats)
			{
				OwaContext.Current.OwaPerformanceData.SetOehRequestType(owaEventNamespaceAttribute.HandlerType.Name, owaEventAttribute.IsAsync ? owaEventAttribute.BeginMethodInfo.Name : owaEventAttribute.MethodInfo.Name);
			}
			if (owaEventAttribute.IsAsync)
			{
				OwaContext.Current.TryReleaseBudgetAndStopTiming();
				ExTraceGlobals.OehTracer.TraceDebug(0L, "Created async HTTP handler to server OEH request");
				OwaContext.Current.IsAsyncRequest = true;
				return new OwaEventAsyncHttpHandler(owaEventHandlerBase);
			}
			ExTraceGlobals.OehTracer.TraceDebug(0L, "Created sync HTTP handler to serve OEH request");
			return new OwaEventHttpHandler(owaEventHandlerBase);
		}

		public void ReleaseHandler(IHttpHandler handler)
		{
		}

		public const string EventRequestIdentifier = "oeh";

		public const string NamespaceName = "ns";

		public const string EventName = "ev";
	}
}
