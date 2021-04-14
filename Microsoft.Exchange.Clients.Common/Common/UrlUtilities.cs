using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.TextConverters.Internal;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authentication;

namespace Microsoft.Exchange.Clients.Common
{
	public static class UrlUtilities
	{
		static UrlUtilities()
		{
			UrlUtilities.RemoteNotifiationDfpowaRequestPaths = new HashSet<string>(new string[]
			{
				"/dfpowa/remotenotification.ashx",
				"/dfpowa1/remotenotification.ashx",
				"/dfpowa2/remotenotification.ashx",
				"/dfpowa3/remotenotification.ashx",
				"/dfpowa4/remotenotification.ashx",
				"/dfpowa5/remotenotification.ashx"
			}, StringComparer.OrdinalIgnoreCase);
		}

		internal static bool IsSafeUrl(string urlString)
		{
			if (string.IsNullOrEmpty(urlString))
			{
				return false;
			}
			Uri uri;
			if (null == (uri = UrlUtilities.TryParseUri(urlString)))
			{
				return false;
			}
			string scheme = uri.Scheme;
			return !string.IsNullOrEmpty(scheme) && Uri.CheckSchemeName(scheme) && TextConvertersInternalHelpers.IsUrlSchemaSafe(scheme);
		}

		internal static Uri TryParseUri(string uriString)
		{
			return UrlUtilities.TryParseUri(uriString, UriKind.Absolute);
		}

		internal static Uri TryParseUri(string uriString, UriKind uriKind)
		{
			if (uriString == null)
			{
				throw new ArgumentNullException("uriString");
			}
			Uri result = null;
			if (!Uri.TryCreate(uriString, uriKind, out result))
			{
				return null;
			}
			return result;
		}

		public static bool IsAuthRedirectRequest(HttpRequest request)
		{
			bool result = false;
			if (request != null)
			{
				result = !string.IsNullOrEmpty(request.QueryString["authRedirect"]);
			}
			return result;
		}

		public static string ValidateFederatedDomainInURL(Uri uri)
		{
			if (uri.Segments.Length >= 3)
			{
				if (!uri.Segments[1].Equals("owa/", StringComparison.OrdinalIgnoreCase))
				{
					return null;
				}
				string text = null;
				if (uri.Segments[2].StartsWith("@"))
				{
					text = uri.Segments[2].Substring(1).TrimEnd(new char[]
					{
						'/'
					});
				}
				else
				{
					string text2 = uri.Segments[2].TrimEnd(new char[]
					{
						'/'
					});
					if (UrlUtilities.VersionFolderRegex.IsMatch(text2))
					{
						return null;
					}
					for (int i = 0; i < UrlUtilities.blackListForFederatedDomainInURL.Length; i++)
					{
						if (text2.EndsWith(UrlUtilities.blackListForFederatedDomainInURL[i], StringComparison.OrdinalIgnoreCase))
						{
							return null;
						}
					}
					if (text2.IndexOf('.') != -1)
					{
						text = text2;
					}
				}
				if (!string.IsNullOrEmpty(text) && SmtpAddress.IsValidDomain(text))
				{
					return text;
				}
			}
			return null;
		}

		public static bool IsSpecialDomainUrl(Uri uri, string domain)
		{
			if (uri.Segments.Length >= 3)
			{
				if (!uri.Segments[1].Equals("owa/", StringComparison.OrdinalIgnoreCase))
				{
					return false;
				}
				string text;
				if (uri.Segments[2].StartsWith("@"))
				{
					text = uri.Segments[2].Substring(1).TrimEnd(new char[]
					{
						'/'
					});
				}
				else
				{
					text = uri.Segments[2].TrimEnd(new char[]
					{
						'/'
					});
				}
				if (!string.IsNullOrEmpty(text) && text.Equals(domain, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		public static void RewriteFederatedDomainInURL(HttpContext httpContext, out string domain)
		{
			domain = string.Empty;
			Uri url = httpContext.Request.Url;
			domain = UrlUtilities.ValidateFederatedDomainInURL(url);
			UrlUtilities.RewriteRealmParameterInURL(httpContext, domain);
		}

		public static void RewriteFederatedDomainInURL(HttpContext httpContext)
		{
			string empty = string.Empty;
			UrlUtilities.RewriteFederatedDomainInURL(httpContext, out empty);
		}

		public static void RewriteDomainInVanityDomainURL(HttpContext httpContext)
		{
			Uri url = httpContext.Request.Url;
			if (url.Segments.Length >= 1)
			{
				string text = url.Segments[0];
				string text2 = text.Substring(text.IndexOf(".") + 1).TrimEnd(new char[]
				{
					'/'
				});
				if (text2.IndexOf(".") <= 0)
				{
					text2 = text;
				}
				UrlUtilities.RewriteRealmParameterInURL(httpContext, text2);
			}
		}

		public static void RewriteDomainFromTenantSpecificURL(HttpContext httpContext, string tenantUrl)
		{
			if (!string.IsNullOrEmpty(tenantUrl))
			{
				string text = tenantUrl.Substring(tenantUrl.IndexOf(".") + 1).TrimEnd(new char[]
				{
					'/'
				});
				if (text.IndexOf(".") <= 0)
				{
					text = tenantUrl;
				}
				UrlUtilities.RewriteRealmParameterInURL(httpContext, text);
			}
		}

		public static void RewriteRealmParameterInURL(HttpContext httpContext, string domain)
		{
			Uri url = httpContext.Request.Url;
			UrlUtilities.SaveOriginalRequestUrlToContext(httpContext, url);
			if (!string.IsNullOrEmpty(domain) && SmtpAddress.IsValidDomain(domain))
			{
				StringBuilder stringBuilder = new StringBuilder();
				int i = 0;
				while (i < url.Segments.Length)
				{
					string text = url.Segments[i];
					if (i != 2 || text.Equals("closewindow.aspx", StringComparison.OrdinalIgnoreCase) || text.Equals("logoff.aspx", StringComparison.OrdinalIgnoreCase))
					{
						goto IL_77;
					}
					int num = text.IndexOf('@');
					if (num > 0 && num < text.Length - 2)
					{
						goto IL_77;
					}
					IL_7F:
					i++;
					continue;
					IL_77:
					stringBuilder.Append(text);
					goto IL_7F;
				}
				if (string.IsNullOrEmpty(url.Query))
				{
					stringBuilder.Append("?");
				}
				else
				{
					stringBuilder.Append(url.Query);
					stringBuilder.Append("&");
				}
				stringBuilder.Append("realm");
				stringBuilder.Append("=");
				stringBuilder.Append(HttpUtility.UrlEncode(domain));
				httpContext.RewritePath(stringBuilder.ToString());
			}
		}

		public static void RewriteParameterInURL(HttpContext httpContext, string name, string value)
		{
			string parameter = string.Format("{0}={1}", name, value);
			UrlUtilities.RewriteParameterInURL(httpContext, parameter);
		}

		public static void RewriteParameterInURL(HttpContext httpContext, string parameter)
		{
			Uri url = httpContext.Request.Url;
			UrlUtilities.SaveOriginalRequestUrlToContext(httpContext, url);
			StringBuilder stringBuilder = new StringBuilder(url.PathAndQuery);
			if (string.IsNullOrEmpty(url.Query))
			{
				stringBuilder.Append("?");
			}
			else
			{
				stringBuilder.Append("&");
			}
			stringBuilder.Append(parameter);
			httpContext.RewritePath(stringBuilder.ToString());
		}

		public static bool IsWacRequest(HttpRequest request)
		{
			string a = HttpUtility.UrlDecode(request.Url.AbsolutePath);
			if (UrlUtilities.isPreCheckinApp)
			{
				return (string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) || string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase)) && (string.Equals(a, "/dfpowa/wopi/files/@/owaatt", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa1/wopi/files/@/owaatt", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa1/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa2/wopi/files/@/owaatt", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa2/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa3/wopi/files/@/owaatt", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa3/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa4/wopi/files/@/owaatt", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa4/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa5/wopi/files/@/owaatt", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/dfpowa5/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase));
			}
			return (string.Equals(request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase) || string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase)) && (string.Equals(a, "/owa/wopi/files/@/owaatt", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "/owa/wopi/files/@/owaatt/contents", StringComparison.OrdinalIgnoreCase));
		}

		public static bool IsRemoteNotificationRequest(HttpRequest request)
		{
			if (!string.Equals(request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			string text = HttpUtility.UrlDecode(request.Url.AbsolutePath);
			if (UrlUtilities.isPreCheckinApp)
			{
				return UrlUtilities.RemoteNotifiationDfpowaRequestPaths.Contains(text);
			}
			return string.Equals(text, "/owa/remotenotification.ashx", StringComparison.OrdinalIgnoreCase);
		}

		public static bool IsRealmRewrittenFromPathToQuery(HttpContext httpContext)
		{
			return UrlUtilities.IsRealmRewrittenFromPathToQuery(httpContext, UrlUtilities.GetOriginalRequestUrlFromContext(httpContext));
		}

		public static bool IsRealmRewrittenFromPathToQuery(HttpContext httpContext, Uri originalUri)
		{
			bool result = false;
			HttpRequest request = httpContext.Request;
			string value = Uri.EscapeDataString(request.QueryString["realm"] ?? string.Empty);
			if (!string.IsNullOrEmpty(value))
			{
				bool flag = request.Url.AbsolutePath.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
				bool flag2 = originalUri.AbsolutePath.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
				if (!flag && flag2)
				{
					result = true;
				}
			}
			return result;
		}

		public static Uri GetOriginalRequestUrlFromContext(HttpContext httpContext)
		{
			Uri result = httpContext.Request.Url;
			string text = httpContext.Request.Headers.Get("X-OWA-OriginalRequestUrl");
			if (!string.IsNullOrEmpty(text))
			{
				Uri uri = UrlUtilities.TryParseUri(HttpUtility.UrlDecode(text));
				if (uri != null)
				{
					result = uri;
				}
			}
			return result;
		}

		public static void SaveOriginalRequestUrlToContext(HttpContext httpContext, Uri originalURL)
		{
			if (string.IsNullOrEmpty(httpContext.Request.Headers.Get("X-OWA-OriginalRequestUrl")))
			{
				httpContext.Request.Headers.Add("X-OWA-OriginalRequestUrl", HttpUtility.UrlEncode(originalURL.AbsoluteUri));
			}
		}

		public static void SaveOriginalRequestHostSchemePortToContext(HttpContext httpContext)
		{
			if (string.IsNullOrWhiteSpace(httpContext.Request.Headers.Get("X-OriginalRequestHost")))
			{
				httpContext.Request.Headers.Add("X-OriginalRequestHost", httpContext.Request.Url.Host);
			}
			if (string.IsNullOrWhiteSpace(httpContext.Request.Headers.Get("X-OriginalRequestHostSchemePort")))
			{
				httpContext.Request.Headers.Add("X-OriginalRequestHostSchemePort", string.Format("{0}:{1}:{2}", httpContext.Request.Url.Port.ToString(), httpContext.Request.Url.Scheme, httpContext.Request.Url.Host));
			}
		}

		public static bool ShouldRedirectQueryParamsAsHashes(Uri originalUrl, out string uriQueryOptimized)
		{
			bool result = false;
			uriQueryOptimized = null;
			if (originalUrl.AbsolutePath != null && originalUrl.AbsolutePath.StartsWith("/owa/", StringComparison.OrdinalIgnoreCase) && !originalUrl.Query.Contains("exsvurl=1", StringComparison.OrdinalIgnoreCase))
			{
				NameValueCollection nameValueCollection = HttpUtility.ParseQueryString(originalUrl.Query);
				NameValueCollection nameValueCollection2 = HttpUtility.ParseQueryString(string.Empty);
				StringBuilder stringBuilder = new StringBuilder();
				using (IEnumerator enumerator = nameValueCollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						string queryParam = (string)enumerator.Current;
						if ((from s in UrlUtilities.ImportantQueryParamNames
						where s.Equals(queryParam, StringComparison.OrdinalIgnoreCase)
						select s).Any<string>())
						{
							nameValueCollection2.Add(queryParam, nameValueCollection[queryParam]);
						}
						else if (stringBuilder.Length <= 0)
						{
							stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "#{0}={1}", new object[]
							{
								queryParam,
								nameValueCollection[queryParam]
							});
						}
						else
						{
							stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "&{0}={1}", new object[]
							{
								queryParam,
								nameValueCollection[queryParam]
							});
						}
					}
				}
				UriBuilder uriBuilder = new UriBuilder(originalUrl);
				uriBuilder.Query = nameValueCollection2.ToString();
				if (originalUrl != uriBuilder.Uri)
				{
					result = true;
					uriQueryOptimized = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
					{
						uriBuilder.Uri.AbsoluteUri,
						stringBuilder.ToString()
					});
				}
			}
			return result;
		}

		public static string GetPathWithExplictLogonHint(Uri requestUrl, string explicitLogonUser)
		{
			return ClientAuthenticationHelper.GetPathWithExplictLogonHint(requestUrl, explicitLogonUser);
		}

		public const string RealmParameter = "realm";

		public const string WacFileIdParameter = "owaatt";

		public const string WacCheckFileRequestPath = "/owa/wopi/files/@/owaatt";

		public const string WacGetFileRequestPath = "/owa/wopi/files/@/owaatt/contents";

		public const string WacDfpowaCheckFileRequestPath = "/dfpowa/wopi/files/@/owaatt";

		public const string WacDfpowaGetFileRequestPath = "/dfpowa/wopi/files/@/owaatt/contents";

		public const string WacDfpowa1CheckFileRequestPath = "/dfpowa1/wopi/files/@/owaatt";

		public const string WacDfpowa1GetFileRequestPath = "/dfpowa1/wopi/files/@/owaatt/contents";

		public const string WacDfpowa2CheckFileRequestPath = "/dfpowa2/wopi/files/@/owaatt";

		public const string WacDfpowa2GetFileRequestPath = "/dfpowa2/wopi/files/@/owaatt/contents";

		public const string WacDfpowa3CheckFileRequestPath = "/dfpowa3/wopi/files/@/owaatt";

		public const string WacDfpowa3GetFileRequestPath = "/dfpowa3/wopi/files/@/owaatt/contents";

		public const string WacDfpowa4CheckFileRequestPath = "/dfpowa4/wopi/files/@/owaatt";

		public const string WacDfpowa4GetFileRequestPath = "/dfpowa4/wopi/files/@/owaatt/contents";

		public const string WacDfpowa5CheckFileRequestPath = "/dfpowa5/wopi/files/@/owaatt";

		public const string WacDfpowa5GetFileRequestPath = "/dfpowa5/wopi/files/@/owaatt/contents";

		public const string SaveUrlOnLogoffParameter = "exsvurl=1";

		public const string OwaVdir = "owa";

		public const string OwaVdirWithSlash = "/owa/";

		public const string EcpCloseWindowPage = "closewindow.aspx";

		public const string OwaLogoffPage = "logoff.aspx";

		public const string AuthParamName = "authRedirect";

		public const string VersionParam = "ver";

		public const string ClientExistingVersionParam = "cver";

		public const string BootOnlineParam = "bO";

		public const string AppcacheCorruptParam = "aC";

		private const string RemoteNotifiationRequestPath = "/owa/remotenotification.ashx";

		private const string ExplicitLogonUser = "X-OWA-ExplicitLogonUser";

		private const string OrignalRequestUrlKey = "X-OWA-OriginalRequestUrl";

		private const string OriginalRequestHostKey = "X-OriginalRequestHost";

		private const string OriginalRequestHostSchemePort = "X-OriginalRequestHostSchemePort";

		private const string OriginalRequestHostSchemePortDelimiter = ":";

		private const string OriginalRequestHostSchemePortFormatString = "{0}:{1}:{2}";

		private static readonly HashSet<string> RemoteNotifiationDfpowaRequestPaths;

		private static readonly string[] blackListForFederatedDomainInURL = new string[]
		{
			".aspx",
			".owa",
			".gif",
			".ashx",
			"ico",
			"css",
			"jpg",
			"xap",
			"js",
			"wav",
			"htm",
			"html",
			"png",
			"msi",
			"manifest",
			".reco",
			".crx",
			"wopi",
			".ttf",
			".eot",
			".woff",
			".svg",
			".svc",
			".owa2"
		};

		private static readonly Regex VersionFolderRegex = new Regex("^[0-9]{1,4}\\.[0-9]{1,4}\\.[0-9]{1,4}\\.[0-9]{1,4}$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

		private static readonly bool isPreCheckinApp = new BoolAppSettingsEntry("IsPreCheckinApp", false, null).Value;

		private static readonly string[] ImportantQueryParamNames = new string[]
		{
			"layout",
			"url",
			"flights"
		};
	}
}
