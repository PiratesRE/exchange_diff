using System;
using System.Diagnostics;
using System.Net;
using System.Web;
using Microsoft.Exchange.Clients.Common;
using Microsoft.Exchange.Clients.EventLogs;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class ProxyUtilities
	{
		internal static bool ShouldCopyProxyRequestHeader(string headerName)
		{
			return !WebHeaderCollection.IsRestricted(headerName) && StringComparer.OrdinalIgnoreCase.Compare(headerName, "accept-encoding") != 0 && StringComparer.OrdinalIgnoreCase.Compare(headerName, "cookie") != 0 && StringComparer.OrdinalIgnoreCase.Compare(headerName, "authorization") != 0 && 0 != StringComparer.OrdinalIgnoreCase.Compare(headerName, "proxy-authorization");
		}

		internal static bool ShouldCopyProxyResponseHeader(string headerName)
		{
			return !WebHeaderCollection.IsRestricted(headerName) && StringComparer.OrdinalIgnoreCase.Compare(headerName, "set-cookie") != 0 && StringComparer.OrdinalIgnoreCase.Compare(headerName, "server") != 0 && StringComparer.OrdinalIgnoreCase.Compare(headerName, "x-powered-by") != 0 && StringComparer.OrdinalIgnoreCase.Compare(headerName, "x-aspnet-version") != 0 && 0 != StringComparer.OrdinalIgnoreCase.Compare(headerName, "www-authenticate");
		}

		internal static HttpWebRequest CreateHttpWebRequestForProxying(OwaContext owaContext, Uri requestUri)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
			httpWebRequest.PreAuthenticate = true;
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.Credentials = CredentialCache.DefaultNetworkCredentials.GetCredential(requestUri, "Kerberos");
			if (httpWebRequest.Credentials == null)
			{
				throw new OwaInvalidOperationException("Can't get credentials for the proxy request");
			}
			httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip;
			CertificateValidationManager.SetComponentId(httpWebRequest, requestUri.IsLoopback ? "OWA_IgnoreCertErrors" : "OWA");
			GccUtils.CopyClientIPEndpointsForServerToServerProxy(owaContext.HttpContext, httpWebRequest);
			return httpWebRequest;
		}

		internal static HttpWebRequest GetProxyRequestInstance(HttpRequest originalRequest, OwaContext owaContext, Uri requestUri)
		{
			string name = "X-LogonType";
			string value = "Public";
			HttpWebRequest httpWebRequest = ProxyUtilities.CreateHttpWebRequestForProxying(owaContext, requestUri);
			for (int i = 0; i < originalRequest.Headers.Count; i++)
			{
				string text = originalRequest.Headers.Keys[i];
				if (ProxyUtilities.ShouldCopyProxyRequestHeader(text))
				{
					httpWebRequest.Headers[text] = originalRequest.Headers[text];
				}
			}
			httpWebRequest.UserAgent = originalRequest.UserAgent;
			string text2 = originalRequest.Headers["referer"];
			if (text2 != null)
			{
				httpWebRequest.Referer = text2;
			}
			text2 = originalRequest.Headers["accept"];
			if (text2 != null)
			{
				httpWebRequest.Accept = text2;
			}
			text2 = originalRequest.Headers["transfer-encoding"];
			if (text2 != null)
			{
				httpWebRequest.TransferEncoding = text2;
			}
			httpWebRequest.ContentLength = (long)originalRequest.ContentLength;
			httpWebRequest.ContentType = originalRequest.ContentType;
			httpWebRequest.UserAgent = originalRequest.UserAgent;
			if (httpWebRequest.Headers[name] == null && owaContext.SessionContext != null && owaContext.SessionContext.IsPublicRequest(originalRequest))
			{
				httpWebRequest.Headers.Add(name, value);
			}
			httpWebRequest.Headers["X-OWA-ProxyUri"] = owaContext.LocalHostName;
			httpWebRequest.Headers["X-OWA-ProxyVersion"] = Globals.ApplicationVersion;
			if (Globals.OwaVDirType == OWAVDirType.OWA)
			{
				httpWebRequest.Headers["X-OWA-ProxySid"] = owaContext.LogonIdentity.UserSid.ToString();
			}
			httpWebRequest.Headers["X-OWA-ProxyCanary"] = Utilities.GetCurrentCanary(owaContext.SessionContext);
			if (owaContext.RequestType == OwaRequestType.WebPart)
			{
				httpWebRequest.Headers["X-OWA-ProxyWebPart"] = "1";
			}
			if (owaContext.TryGetUserContext() != null && owaContext.UserContext.ProxyUserContextCookie != null)
			{
				ExTraceGlobals.ProxyDataTracer.TraceDebug<UserContextCookie>(0L, "Setting proxy user context cookie: {0}", owaContext.UserContext.ProxyUserContextCookie);
				Cookie netCookie = owaContext.UserContext.ProxyUserContextCookie.NetCookie;
				ProxyUtilities.AddCookieToProxyRequest(httpWebRequest, netCookie, requestUri.Host);
			}
			else
			{
				ExTraceGlobals.ProxyDataTracer.TraceDebug(0L, "No user context cookie used for the proxy request");
			}
			foreach (string name2 in ProxyUtilities.ProxyAllowedCookies)
			{
				if (owaContext.HttpContext.Request.Cookies[name2] != null)
				{
					Cookie cookie = new Cookie(name2, owaContext.HttpContext.Request.Cookies[name2].Value);
					ProxyUtilities.AddCookieToProxyRequest(httpWebRequest, cookie, requestUri.Host);
				}
			}
			return httpWebRequest;
		}

		private static void AddCookieToProxyRequest(HttpWebRequest request, Cookie cookie, string domain)
		{
			if (request.CookieContainer == null)
			{
				request.CookieContainer = new CookieContainer();
			}
			cookie.Domain = domain;
			request.CookieContainer.Add(cookie);
		}

		internal static void UpdateProxyClientDataCollectingCookieFromResponse(HttpWebResponse proxyResponse, HttpResponse originalResponse)
		{
			if (proxyResponse.Cookies["owacsdc"] != null)
			{
				HttpCookie cookie = new HttpCookie("owacsdc", proxyResponse.Cookies["owacsdc"].Value);
				originalResponse.Cookies.Add(cookie);
			}
		}

		internal static void UpdateProxyUserContextIdFromResponse(HttpWebResponse response, UserContext userContext)
		{
			ExTraceGlobals.UserContextCallTracer.TraceDebug(0L, "UpdateProxyUserContextIdFromResponse");
			string text = response.Headers["Set-Cookie"];
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			int num = text.IndexOf("UserContext", StringComparison.Ordinal);
			if (num < 0)
			{
				return;
			}
			int num2 = text.IndexOf(';', num);
			if (num2 < 0)
			{
				num2 = text.Length - 1;
			}
			string text2 = text.Substring(num, num2 - num);
			if (!text2.StartsWith("UserContext", StringComparison.OrdinalIgnoreCase))
			{
				throw new OwaInvalidOperationException("Invalid user context cookie found in proxy response");
			}
			int num3 = text2.IndexOf('=');
			if (num3 < 0)
			{
				throw new OwaInvalidOperationException("Invalid user context cookie found in proxy response");
			}
			string cookieName = text2.Substring(0, num3);
			string cookieValue = text2.Substring(num3 + 1, text2.Length - num3 - 1);
			string cookieId = null;
			if (!UserContextCookie.TryParseCookieName(cookieName, out cookieId))
			{
				throw new OwaInvalidOperationException("Invalid user context cookie found in proxy response");
			}
			string canaryString = null;
			string mailboxUniqueKey = null;
			if (!UserContextCookie.TryParseCookieValue(cookieValue, out canaryString, out mailboxUniqueKey))
			{
				throw new OwaInvalidOperationException("Invalid user context cookie found in proxy response");
			}
			Canary canary = Canary.RestoreCanary(canaryString, userContext.LogonIdentity.UniqueId);
			userContext.ProxyUserContextCookie = UserContextCookie.Create(cookieId, canary, mailboxUniqueKey);
			ExTraceGlobals.UserContextTracer.TraceDebug<UserContextCookie>(0L, "Found set-cookie returned by second CAS: {0}", userContext.ProxyUserContextCookie);
			if (userContext.ProxyUserContextCookie == null)
			{
				throw new OwaInvalidOperationException("Invalid user context cookie found in proxy response");
			}
		}

		internal static IAsyncResult BeginGetResponse(HttpWebRequest request, AsyncCallback asyncCallback, object context, out Stopwatch requestClock)
		{
			if (Globals.ArePerfCountersEnabled)
			{
				long num = (long)request.Headers.ToByteArray().Length;
				if (request.ContentLength > 0L)
				{
					num += request.ContentLength;
				}
				OwaSingleCounters.ProxyRequestBytes.IncrementBy(num);
			}
			requestClock = Stopwatch.StartNew();
			ProxyUtilities.TraceProxyRequest(request);
			return request.BeginGetResponse(asyncCallback, context);
		}

		internal static HttpWebResponse EndGetResponse(HttpWebRequest request, IAsyncResult asyncResult, Stopwatch requestClock)
		{
			requestClock.Stop();
			long elapsedMilliseconds = requestClock.ElapsedMilliseconds;
			HttpWebResponse result;
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
				ProxyUtilities.TraceProxyResponse(request, (int)httpWebResponse.StatusCode, elapsedMilliseconds);
				result = httpWebResponse;
			}
			catch (WebException exception)
			{
				ProxyUtilities.TraceFailedProxyRequest(request, exception, elapsedMilliseconds);
				throw;
			}
			return result;
		}

		private static void TraceProxyRequest(HttpWebRequest request)
		{
			ExTraceGlobals.ProxyRequestTracer.TraceDebug<string, Uri, long>((long)request.GetHashCode(), "Request: {0} {1}, content-length:{2}", request.Method, request.RequestUri, request.ContentLength);
		}

		private static void TraceProxyResponse(HttpWebRequest request, int httpStatusCode, long elapsedMilliseconds)
		{
			ExTraceGlobals.ProxyRequestTracer.TraceDebug<int, long>((long)request.GetHashCode(), "Response: HTTP {0}, time:{1} ms.", httpStatusCode, elapsedMilliseconds);
		}

		private static void TraceFailedProxyRequest(HttpWebRequest request, WebException exception, long elapsedMilliseconds)
		{
			if (!ExTraceGlobals.ProxyRequestTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				return;
			}
			if (exception.Status == WebExceptionStatus.ProtocolError)
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)exception.Response;
				int statusCode = (int)httpWebResponse.StatusCode;
				ExTraceGlobals.ProxyRequestTracer.TraceDebug<int, long>((long)request.GetHashCode(), "Response: HTTP {0}, time:{1} ms.", statusCode, elapsedMilliseconds);
				return;
			}
			string arg = exception.Status.ToString();
			string arg2 = (exception.Message != null) ? exception.Message : "N/A";
			ExTraceGlobals.ProxyRequestTracer.TraceDebug<string, string, long>((long)request.GetHashCode(), "Response (failed): {0}, {1}, time:{2} ms.", arg, arg2, elapsedMilliseconds);
		}

		internal static void ThrowMalformedCasUriException(OwaContext owaContext, string malformedUri)
		{
			string text = owaContext.ExchangePrincipal.LegacyDn;
			if (text == null)
			{
				text = string.Empty;
			}
			OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorWrongUriFormat, malformedUri, new object[]
			{
				owaContext.LocalHostName,
				text,
				malformedUri
			});
			throw new OwaProxyException(string.Format("The format of the uri is wrong: {0}", malformedUri), string.Format(Strings.ErrorWrongUriFormat, malformedUri));
		}

		internal static Uri TryCreateCasUri(string uriString, bool needVdirValidation)
		{
			if (string.IsNullOrEmpty(uriString) || !Uri.IsWellFormedUriString(uriString, UriKind.Absolute))
			{
				return null;
			}
			Uri uri;
			if (null == (uri = Utilities.TryParseUri(uriString)))
			{
				return null;
			}
			if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
			{
				return null;
			}
			if (needVdirValidation)
			{
				if (uri.Segments.Length != 2 && uri.Segments.Length != 3)
				{
					return null;
				}
				if (StringComparer.OrdinalIgnoreCase.Compare(uri.Segments[0], "/") != 0 || (StringComparer.OrdinalIgnoreCase.Compare(uri.Segments[1], Globals.VirtualRootName) != 0 && StringComparer.OrdinalIgnoreCase.Compare(uri.Segments[1], Globals.VirtualRootName + "/") != 0))
				{
					return null;
				}
				if (uri.Segments.Length == 3)
				{
					string value = UrlUtilities.ValidateFederatedDomainInURL(uri);
					if (string.IsNullOrEmpty(value))
					{
						return null;
					}
				}
			}
			if (!string.IsNullOrEmpty(uri.Query))
			{
				return null;
			}
			return uri;
		}

		internal static void EnsureProxyUrlSslPolicy(OwaContext owaContext, ProxyUri secondCasUri)
		{
			if (!OwaRegistryKeys.AllowProxyingWithoutSsl && secondCasUri.Uri.Scheme != Uri.UriSchemeHttps)
			{
				OwaDiagnostics.LogEvent(ClientsEventLogConstants.Tuple_ProxyErrorSslConnection, secondCasUri.ToString(), new object[]
				{
					owaContext.LocalHostName,
					secondCasUri.ToString(),
					secondCasUri.ToString()
				});
				throw new OwaProxyException(string.Format("The URI found for proxying does not start with \"https\". Value={0}", secondCasUri.ToString()), LocalizedStrings.GetNonEncoded(-750997814));
			}
		}

		internal static bool IsVersionFolderInProxy(ServerVersion version)
		{
			return Globals.LocalVersionFolders.ContainsKey(version);
		}

		internal const int HttpStatusNeedIdentity = 441;

		internal const int HttpStatusRetryRequest = 241;

		internal const int HttpStatusProxyPingSucceeded = 242;

		internal const int HttpStatusNeedLanguage = 442;

		internal const string CertificateValidationComponentId = "OWA";

		internal const string CertificateIgnoreComponentId = "OWA_IgnoreCertErrors";

		private static readonly string[] ProxyAllowedCookies = new string[]
		{
			"UpdatedUserSettings",
			"mkt"
		};

		internal enum CauseOfUnkownRequestExecution
		{
			None,
			NoCASFoundForInSiteMailbox,
			NoCASFoundForCrossSiteMailboxToRedirect,
			NoCASFoundForCrossSiteMailboxToProxy
		}

		public enum LegacyRedirectFailureCause
		{
			None,
			NoCasFound
		}
	}
}
