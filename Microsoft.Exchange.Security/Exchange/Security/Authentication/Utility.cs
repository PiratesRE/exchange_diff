using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.DirectoryServices;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Security.Authentication
{
	public static class Utility
	{
		public static bool IsResourceRequest(string localPath)
		{
			bool result = false;
			foreach (string value in Utility.resourcesExtensions)
			{
				if (localPath.EndsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					result = true;
					break;
				}
			}
			return result;
		}

		public static bool IsOwaRequestWithRoutingHint(HttpRequest request)
		{
			string localPath = request.Url.LocalPath;
			int num = localPath.IndexOf("owa/", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return false;
			}
			int num2 = localPath.IndexOfAny(new char[]
			{
				'@',
				'/'
			}, num + "owa/".Length);
			return num2 != -1 && localPath[num2] == '@' && Utility.routingHintRegex.IsMatch(localPath);
		}

		public static bool HasResourceRoutingHint(HttpRequest request)
		{
			HttpCookie httpCookie = request.Cookies["X-BEResource"];
			return httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value);
		}

		public static bool IsAnonymousResourceRequest(HttpRequest request)
		{
			HttpCookie httpCookie = request.Cookies["X-AnonResource"];
			return httpCookie != null && !string.IsNullOrEmpty(httpCookie.Value) && string.Compare(httpCookie.Value, "true", CultureInfo.InvariantCulture, CompareOptions.IgnoreCase) == 0;
		}

		internal static ADRawEntry GetVirtualDirectoryObject(Guid vDirObjectGuid, ITopologyConfigurationSession session, params PropertyDefinition[] virtualDirectoryPropertyDefinitions)
		{
			ADObjectId entryId;
			if (vDirObjectGuid == Guid.Empty)
			{
				string text = HttpRuntime.AppDomainAppVirtualPath.Replace("'", "''");
				if (text[0] == '/')
				{
					text = text.Substring(1);
				}
				Server server = session.FindLocalServer();
				string text2 = HttpRuntime.AppDomainAppId;
				if (text2[0] == '/')
				{
					text2 = text2.Substring(1);
				}
				int num = text2.IndexOf('/');
				text2 = text2.Substring(num);
				text2 = string.Format(CultureInfo.InvariantCulture, "IIS://{0}{1}", new object[]
				{
					server.Fqdn,
					text2
				});
				num = text2.LastIndexOf('/');
				using (DirectoryEntry directoryEntry = new DirectoryEntry(text2.Substring(0, num)))
				{
					using (DirectoryEntry parent = directoryEntry.Parent)
					{
						if (parent != null)
						{
							text2 = (((string)parent.Properties["ServerComment"].Value) ?? string.Empty);
						}
					}
				}
				entryId = new ADObjectId(server.DistinguishedName).GetDescendantId("Protocols", "HTTP", new string[]
				{
					string.Format(CultureInfo.InvariantCulture, "{0} ({1})", new object[]
					{
						text,
						text2
					})
				});
			}
			else
			{
				entryId = new ADObjectId(vDirObjectGuid);
			}
			return session.ReadADRawEntry(entryId, virtualDirectoryPropertyDefinitions);
		}

		internal static bool IsSignOutCleanupRequest(this HttpRequest request)
		{
			return request.QueryString["wa"] == "wsignoutcleanup1.0";
		}

		internal static bool PreferAdfsAuthentication(this HttpRequest request)
		{
			return (!VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).ActiveMonitoring.AllowBasicAuthForOutsideInMonitoringMailboxes.Enabled || !request.IsActiveMonitoringUserAgent()) && (!AdfsFederationAuthModule.HasOtherAuthenticationMethod || request.ExplicitPreferAdfsAuthentication() || (request.QueryString["cross"] == null && (request.QueryString["rfr"] == null || "admin".Equals(request.QueryString["rfr"], StringComparison.OrdinalIgnoreCase))));
		}

		internal static bool ExplicitPreferAdfsAuthentication(this HttpRequest request)
		{
			string text = request.QueryString["cross"];
			return text != null && text != "0";
		}

		internal static bool IsAuthenticatedByAdfs(this HttpRequest request)
		{
			return request.Cookies["TimeWindow"] != null;
		}

		internal static bool IsAdfsLogoffRequest(this HttpRequest request)
		{
			string[] segments = request.Url.Segments;
			return request.IsAuthenticatedByAdfs() && (request.Url.LocalPath.EndsWith("/logoff.aspx", StringComparison.OrdinalIgnoreCase) || request.Url.LocalPath.EndsWith("/logoff.owa", StringComparison.OrdinalIgnoreCase));
		}

		internal static bool IsOWAPingRequest(HttpRequest request)
		{
			return request.Path.EndsWith("ping.owa", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsNotOwaGetOrOehRequest(this HttpRequest request)
		{
			bool flag = false;
			if (request.Url.Segments.Length > 1)
			{
				flag = request.Url.Segments[1].Equals("owa/", StringComparison.OrdinalIgnoreCase);
			}
			return flag && request.IsNotGetOrOehRequest();
		}

		internal static bool IsNotGetOrOehRequest(this HttpRequest request)
		{
			return !request.HttpMethod.Equals("GET", StringComparison.OrdinalIgnoreCase) || request.Url.Query.Contains("oeh=1&");
		}

		internal static bool IsAjaxRequest(this HttpRequest request)
		{
			string a = request.Headers["x-requested-with"];
			return a == "XMLHttpRequest";
		}

		internal static bool IsAnonymousAuthFolderRequest(this HttpRequest request)
		{
			return (HttpRuntime.AppDomainAppVirtualPath.Equals("/ecp", StringComparison.OrdinalIgnoreCase) || HttpRuntime.AppDomainAppVirtualPath.Equals("/owa", StringComparison.OrdinalIgnoreCase)) && request.Url.Segments.Length >= 3 && request.Url.Segments[2].Equals("auth/", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool TryReadConfigBool(string key, out bool value)
		{
			return bool.TryParse(ConfigurationManager.AppSettings[key], out value);
		}

		internal static bool TryReadConfigInt(string key, out int value)
		{
			return int.TryParse(ConfigurationManager.AppSettings[key], out value);
		}

		internal static void DeleteFbaAuthCookies(HttpRequest httpRequest, HttpResponse httpResponse)
		{
			if (!Utility.isPartnerHostedOnly.Member && !VariantConfiguration.InvariantNoFlightingSnapshot.Cafe.NoFormBasedAuthentication.Enabled)
			{
				Utility.DeleteCookie(httpRequest, httpResponse, "cadata", null, false);
				Utility.DeleteCookie(httpRequest, httpResponse, "cadataKey", null, false);
				Utility.DeleteCookie(httpRequest, httpResponse, "cadataIV", null, false);
				Utility.DeleteCookie(httpRequest, httpResponse, "cadataSig", null, false);
				Utility.DeleteCookie(httpRequest, httpResponse, "cadataTTL", null, false);
			}
			Utility.DeleteCookie(httpRequest, httpResponse, "X-DFPOWA-Vdir", null, false);
			Utility.DeleteCookie(httpRequest, httpResponse, "TargetServer", null, false);
			Utility.DeleteCookie(httpRequest, httpResponse, "ExchClientVer", null, false);
		}

		internal static void DeleteCookie(HttpRequest httpRequest, HttpResponse httpResponse, string name, string path = null, bool forceToAddNewCookie = false)
		{
			if (httpRequest == null)
			{
				throw new ArgumentNullException("httpRequest");
			}
			if (httpResponse == null)
			{
				throw new ArgumentNullException("httpResponse");
			}
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name can not be null or empty string");
			}
			if (httpRequest.Cookies[name] == null || httpRequest.Cookies[name].Value == null)
			{
				return;
			}
			HttpCookie httpCookie = httpResponse.Cookies[name];
			bool flag = forceToAddNewCookie || httpCookie == null;
			if (flag)
			{
				httpCookie = new HttpCookie(name, string.Empty);
			}
			httpCookie.Expires = DateTime.UtcNow.AddYears(-30);
			if (path != null)
			{
				httpCookie.Path = path;
			}
			if (flag)
			{
				httpResponse.Cookies.Add(httpCookie);
			}
		}

		internal static X509Certificate2[] GetCertificates()
		{
			LocalConfiguration configuration = ConfigProvider.Instance.Configuration;
			X509Certificate2[] certificates = configuration.Certificates;
			if (certificates == null || certificates.Length < 1)
			{
				throw new AdfsConfigurationException(AdfsConfigErrorReason.NoCertificates, "Encryption certificate is absent");
			}
			return certificates;
		}

		internal static AdfsAuthCountersInstance AdfsAuthCountersInstance
		{
			get
			{
				return Utility.counters.Member;
			}
		}

		internal static bool IsActiveMonitoringUserAgent(this HttpRequest request)
		{
			return request.UserAgent != null && request.UserAgent.IndexOf("ACTIVEMONITORING", StringComparison.OrdinalIgnoreCase) >= 0;
		}

		public const string FbaLogoffPage = "logoff.aspx";

		public const string EcpLogoffUrlSegment = "/logoff.aspx";

		public const string OwaLogoffUrlSegment = "/logoff.owa";

		public const string PreferAdfsParameter = "cross";

		public const string UserActionQueryString = "UA";

		public const string TimeoutLogoutPage = "auth/TimeoutLogout.aspx";

		public const string AJAXHeaderName = "x-requested-with";

		public const string AJAXHeaderValue = "XMLHttpRequest";

		public const string Admin = "admin";

		private const string OehParameter = "oeh=1&";

		private const string HttpMethodGet = "GET";

		private const string OWAPingUrl = "ping.owa";

		private const string ActiveMonitoringUserAgentIdentifier = "ACTIVEMONITORING";

		private static readonly IList<string> resourcesExtensions = new List<string>
		{
			".gif",
			".jpg",
			".css",
			".xap",
			".js",
			".wav",
			".htm",
			".html",
			".png",
			".msi",
			".ico",
			".mp3",
			".axd",
			".eot",
			".ttf",
			".svg",
			".woff"
		}.AsReadOnly();

		private static readonly Regex routingHintRegex = new Regex("/owa/[a-fA-F0-9]{8}-([a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}@[A-Z0-9.-]+\\.[A-Z]{2,4}/prem/\\d{2}\\.\\d{1,}\\.\\d{1,}\\.\\d{1,}/", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		private static LazyMember<AdfsAuthCountersInstance> counters = new LazyMember<AdfsAuthCountersInstance>(() => AdfsAuthCounters.GetInstance(Process.GetCurrentProcess().ProcessName));

		private static LazyMember<bool> isPartnerHostedOnly = new LazyMember<bool>(delegate()
		{
			try
			{
				if (Datacenter.IsPartnerHostedOnly(true))
				{
					return true;
				}
			}
			catch (CannotDetermineExchangeModeException)
			{
			}
			return false;
		});
	}
}
