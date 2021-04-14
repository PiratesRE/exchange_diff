using System;
using System.Net;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public static class HttpUtilities
	{
		public static string GetQueryStringParameter(HttpRequest httpRequest, string name)
		{
			return HttpUtilities.GetQueryStringParameter(httpRequest, name, true);
		}

		public static string GetQueryStringParameter(HttpRequest httpRequest, string name, bool required)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name");
			}
			string text = httpRequest.QueryString[name];
			if (text == null && required)
			{
				throw new OwaInvalidRequestException(string.Format("Required URL parameter missing: {0}", name));
			}
			return text;
		}

		public static string GetFormParameter(HttpRequest httpRequest, string name, bool required)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentNullException("name", "name cannot be null or empty");
			}
			string text = httpRequest.Form[name];
			if (text == null && required)
			{
				throw new OwaInvalidRequestException(string.Format("Required form parameter missing: {0}", name));
			}
			return text;
		}

		public static void EndResponse(HttpContext httpContext, HttpStatusCode statusCode)
		{
			HttpUtilities.EndResponse(httpContext, statusCode, HttpCacheability.NoCache);
		}

		public static void EndResponse(HttpContext httpContext, HttpStatusCode statusCode, HttpCacheability cacheability)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<int>(0L, "HttpUtilities.EndResponse: statusCode={0}", (int)statusCode);
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			if (cacheability == HttpCacheability.NoCache)
			{
				HttpUtilities.MakePageNoCacheNoStore(httpContext.Response);
			}
			httpContext.Response.StatusCode = (int)statusCode;
			try
			{
				httpContext.Response.Flush();
				httpContext.ApplicationInstance.CompleteRequest();
			}
			catch (HttpException arg)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<HttpException>(0L, "Failed to flush and send response to client. {0}", arg);
			}
			httpContext.Response.End();
		}

		internal static string GetContentTypeString(OwaEventContentType contentType)
		{
			switch (contentType)
			{
			case OwaEventContentType.Html:
				return "text/html";
			case OwaEventContentType.Javascript:
				return "application/x-javascript";
			case OwaEventContentType.PlainText:
				return "text/plain";
			case OwaEventContentType.Css:
				return "text/css";
			case OwaEventContentType.Jpeg:
				return "image/jpeg";
			default:
				throw new ArgumentOutOfRangeException("contentType");
			}
		}

		internal static void MakePageNoCacheNoStore(HttpResponse response)
		{
			response.Cache.SetCacheability(HttpCacheability.NoCache);
			response.Cache.SetNoStore();
		}

		internal static void DeleteCookie(HttpResponse response, string name)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name can not be null or empty string");
			}
			bool flag = false;
			for (int i = 0; i < response.Cookies.Count; i++)
			{
				HttpCookie httpCookie = response.Cookies[i];
				if (httpCookie.Name != null && string.Equals(httpCookie.Name, name, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				response.Cookies.Add(new HttpCookie(name, string.Empty));
			}
			response.Cookies[name].Expires = (DateTime)ExDateTime.UtcNow.AddYears(-30);
		}

		internal static bool IsPostRequest(HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			return string.Equals(request.HttpMethod, "post", StringComparison.OrdinalIgnoreCase);
		}

		internal static string GetRequestCorrelationId(HttpContext httpContext)
		{
			ExAssert.RetailAssert(httpContext != null, "httpContext is null");
			string text = httpContext.Request.Headers["X-OWA-CorrelationId"];
			if (string.IsNullOrEmpty(text))
			{
				text = "<empty>";
			}
			return text;
		}

		internal const string ISO8601DateTimeMsPattern = "yyyy-MM-ddTHH:mm:ss.fff";

		internal static readonly string[] TransferrableHeaders = new string[]
		{
			"X-OWA-CorrelationId",
			"X-OWA-ClientBegin",
			"X-FrontEnd-Begin",
			"X-FrontEnd-End",
			"X-BackEnd-Begin",
			"X-BackEnd-End",
			"X-FrontEnd-Handler-Begin",
			"X-EXT-ClientName",
			"X-EXT-CorrelationId",
			"X-EXT-ProxyBegin",
			"X-EXT-ACSBegin",
			"X-EXT-ACSEnd"
		};
	}
}
