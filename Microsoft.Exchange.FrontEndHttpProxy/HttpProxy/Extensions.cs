using System;
using System.Net;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class Extensions
	{
		public static int GetTraceContext(this HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			object obj = httpContext.Items[Constants.TraceContextKey];
			if (obj != null)
			{
				return (int)obj;
			}
			return httpContext.GetHashCode();
		}

		public static RequestDetailsLogger GetLogger(this HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException();
			}
			return RequestDetailsLoggerBase<RequestDetailsLogger>.GetCurrent(httpContext);
		}

		public static string GetSerializedAccessTokenString(this HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			string result = null;
			try
			{
				IIdentity callerIdentity = httpContext.GetCallerIdentity();
				using (ClientSecurityContext clientSecurityContext = callerIdentity.CreateClientSecurityContext(true))
				{
					SerializedAccessToken serializedAccessToken = new SerializedAccessToken(callerIdentity.GetSafeName(true), callerIdentity.AuthenticationType, clientSecurityContext);
					result = serializedAccessToken.ToString();
				}
			}
			catch (AuthzException ex)
			{
				throw new HttpException(401, ex.Message);
			}
			return result;
		}

		public static SerializedClientSecurityContext GetSerializedClientSecurityContext(this HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			SerializedClientSecurityContext result = null;
			try
			{
				IIdentity callerIdentity = httpContext.GetCallerIdentity();
				using (ClientSecurityContext clientSecurityContext = callerIdentity.CreateClientSecurityContext(true))
				{
					result = SerializedClientSecurityContext.CreateFromClientSecurityContext(clientSecurityContext, callerIdentity.GetSafeName(true), callerIdentity.AuthenticationType);
				}
			}
			catch (AuthzException ex)
			{
				throw new HttpException(401, ex.Message);
			}
			return result;
		}

		public static byte[] CreateSerializedSecurityAccessToken(this HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			SerializedSecurityAccessToken serializedSecurityAccessToken = new SerializedSecurityAccessToken();
			try
			{
				IIdentity callerIdentity = httpContext.GetCallerIdentity();
				using (ClientSecurityContext clientSecurityContext = callerIdentity.CreateClientSecurityContext(true))
				{
					clientSecurityContext.SetSecurityAccessToken(serializedSecurityAccessToken);
				}
			}
			catch (AuthzException ex)
			{
				throw new HttpException(401, ex.Message);
			}
			return serializedSecurityAccessToken.GetSecurityContextBytes();
		}

		public static IIdentity GetCallerIdentity(this HttpContext httpContext)
		{
			IIdentity identity = httpContext.User.Identity;
			if (identity.GetType().Equals(typeof(GenericIdentity)) && string.Equals(identity.AuthenticationType, "LiveIdBasic", StringComparison.OrdinalIgnoreCase))
			{
				identity = LiveIdBasicHelper.GetCallerIdentity(httpContext);
			}
			return identity;
		}

		public static HttpMethod GetHttpMethod(this HttpRequest request)
		{
			HttpMethod result = HttpMethod.Unknown;
			if (!Enum.TryParse<HttpMethod>(request.HttpMethod, true, out result))
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "Extensions.GetHttpMethod. HttpMethod unrecognised or has no enum value: {0}", request.HttpMethod);
			}
			return result;
		}

		public static bool IsDownLevelClient(this HttpRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "Extensions.IsDownLevelClient. user-agent = {0}", (request.UserAgent == null) ? string.Empty : request.UserAgent);
			string a;
			UserAgentParser.UserAgentVersion userAgentVersion;
			string a2;
			UserAgentParser.Parse(request.UserAgent, out a, out userAgentVersion, out a2);
			return (!string.Equals(a, "rv:", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 11 || !string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase)) && (!string.Equals(a, "MSIE", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 7 || (!string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 98; Win 9x 4.90", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 2000", StringComparison.OrdinalIgnoreCase))) && (!string.Equals(a, "Safari", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 3 || !string.Equals(a2, "Macintosh", StringComparison.OrdinalIgnoreCase)) && (!string.Equals(a, "Firefox", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 3 || (!string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 98; Win 9x 4.90", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Windows 2000", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Macintosh", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Linux", StringComparison.OrdinalIgnoreCase))) && (!string.Equals(a, "Chrome", StringComparison.OrdinalIgnoreCase) || userAgentVersion.Build < 1 || (!string.Equals(a2, "Windows NT", StringComparison.OrdinalIgnoreCase) && !string.Equals(a2, "Macintosh", StringComparison.OrdinalIgnoreCase)));
		}

		public static string GetFullRawUrl(this HttpRequest httpRequest)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			string text = httpRequest.Url.IsDefaultPort ? string.Empty : (":" + httpRequest.Url.Port.ToString());
			return string.Concat(new string[]
			{
				httpRequest.Url.Scheme,
				"://",
				httpRequest.Url.Host,
				text,
				httpRequest.RawUrl
			});
		}

		public static bool IsAnyWsSecurityRequest(this HttpRequest request)
		{
			return ProtocolHelper.IsAnyWsSecurityRequest(request.Url.LocalPath);
		}

		public static bool IsWsSecurityRequest(this HttpRequest request)
		{
			return ProtocolHelper.IsWsSecurityRequest(request.Url.LocalPath);
		}

		public static bool IsPartnerAuthRequest(this HttpRequest request)
		{
			return ProtocolHelper.IsPartnerAuthRequest(request.Url.LocalPath);
		}

		public static bool IsX509CertAuthRequest(this HttpRequest request)
		{
			return ProtocolHelper.IsX509CertAuthRequest(request.Url.LocalPath);
		}

		public static bool IsChangePasswordLogoff(this HttpRequest request)
		{
			return request.QueryString["ChgPwd"] == "1";
		}

		public static bool CanHaveBody(this HttpRequest request)
		{
			HttpMethod httpMethod = request.GetHttpMethod();
			return httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Head;
		}

		public static bool IsRequestChunked(this HttpRequest request)
		{
			string text = request.Headers["Transfer-Encoding"];
			return text != null && text.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool IsChunkedResponse(this HttpWebResponse response)
		{
			string text = response.Headers["Transfer-Encoding"];
			return text != null && text.IndexOf("chunked", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public static bool HasBody(this HttpRequest request)
		{
			return request.CanHaveBody() && (request.IsRequestChunked() || request.ContentLength > 0);
		}

		public static string GetBaseUrl(this HttpRequest httpRequest)
		{
			return new UriBuilder
			{
				Host = httpRequest.Url.Host,
				Port = httpRequest.Url.Port,
				Scheme = httpRequest.Url.Scheme,
				Path = httpRequest.ApplicationPath
			}.Uri.ToString();
		}

		public static string GetTestBackEndUrl(this HttpRequest clientRequest)
		{
			return clientRequest.Headers[Constants.TestBackEndUrlRequestHeaderKey];
		}

		public static void LogSharedCacheCall(this IRequestContext requestContext, long latency, string diagnostics)
		{
			if (requestContext == null)
			{
				throw new ArgumentNullException("requestContext");
			}
			requestContext.LatencyTracker.HandleSharedCacheLatency(latency);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(requestContext.Logger, "SharedCache", diagnostics);
			PerfCounters.UpdateMovingAveragePerformanceCounter(PerfCounters.HttpProxyCountersInstance.MovingAverageSharedCacheLatency, latency);
		}

		public static string GetFriendlyName(this OrganizationId organizationId)
		{
			if (organizationId != null && organizationId.OrganizationalUnit != null)
			{
				return organizationId.OrganizationalUnit.Name;
			}
			return null;
		}

		public static bool TryGetSite(this ServiceTopology serviceTopology, string fqdn, out Site site)
		{
			if (serviceTopology == null)
			{
				throw new ArgumentNullException("serviceTopology");
			}
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentNullException("fqdn");
			}
			site = null;
			try
			{
				site = serviceTopology.GetSite(fqdn, "f:\\15.00.1497\\sources\\dev\\cafe\\src\\HttpProxy\\Misc\\Extensions.cs", "TryGetSite", 495);
			}
			catch (ServerNotFoundException)
			{
				return false;
			}
			catch (ServerNotInSiteException)
			{
				return false;
			}
			return true;
		}

		internal static void SetActivityScopeOnCurrentThread(this HttpContext httpContext, RequestDetailsLogger logger)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException();
			}
			if (logger != null)
			{
				ActivityContext.SetThreadScope(logger.ActivityScope);
			}
		}

		internal static void Shuffle<T>(this T[] array, Random random)
		{
			for (int i = array.Length - 1; i > 0; i--)
			{
				int num = random.Next(i + 1);
				T t = array[i];
				array[i] = array[num];
				array[num] = t;
			}
		}
	}
}
