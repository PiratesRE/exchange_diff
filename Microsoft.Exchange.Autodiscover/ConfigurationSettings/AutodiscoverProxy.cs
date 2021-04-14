using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	internal sealed class AutodiscoverProxy
	{
		static AutodiscoverProxy()
		{
			CertificateValidationManager.RegisterCallback("AutodiscoverProxy", delegate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				if (!(sender is HttpWebRequest))
				{
					return false;
				}
				HttpWebRequest httpWebRequest = (HttpWebRequest)sender;
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_ErrProviderRedirectServerCertificate, Common.PeriodicKey, new object[]
				{
					(httpWebRequest.RequestUri != null) ? httpWebRequest.RequestUri.Host : "Unknown Host",
					(certificate == null) ? "No Certificate" : certificate.Subject,
					sslPolicyErrors
				});
				return SslConfiguration.AllowExternalUntrustedCerts;
			});
		}

		internal static bool TryParseOutlookUserAgent(string userAgent, out int majorVersion, out int? minorVersion, out int? buildNumber)
		{
			majorVersion = 0;
			minorVersion = null;
			buildNumber = null;
			if (string.IsNullOrEmpty(userAgent))
			{
				return false;
			}
			Match match = AutodiscoverProxy.outlookUserAgentRegEx.Match(userAgent);
			if (!match.Success)
			{
				return false;
			}
			if (!match.Groups["buildNumber"].Success)
			{
				return int.TryParse(match.Groups["majorVersion2"].Value, out majorVersion);
			}
			if (!int.TryParse(match.Groups["majorVersion"].Value, out majorVersion))
			{
				return false;
			}
			int value;
			if (!int.TryParse(match.Groups["minorVersion"].Value, out value))
			{
				majorVersion = 0;
				return false;
			}
			minorVersion = new int?(value);
			if (!int.TryParse(match.Groups["buildNumber"].Value, out value))
			{
				majorVersion = 0;
				minorVersion = null;
				return false;
			}
			buildNumber = new int?(value);
			return true;
		}

		internal static bool IsProxyingNeededForClient(HttpRequest request)
		{
			return AutodiscoverProxy.IsProxyingNeededForClient(request.IsAuthenticated, request.Path, Common.SafeGetUserAgent(request), request.Headers.Get("Authorization"));
		}

		internal static bool IsProxyingNeededForClient(bool isAuthenticated, string path, string userAgent, string authorization)
		{
			int length;
			if (authorization != null && (length = authorization.IndexOf(' ')) >= 0)
			{
				authorization = authorization.Substring(0, length);
			}
			bool enabled = VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.RedirectOutlookClient.Enabled;
			ExTraceGlobals.AuthenticationTracer.TraceDebug(-1L, "AutodiscoverProxy::IsProxyNeededForClient. path = {0}, userAgent = \"{1}\", authorization = \"{2}\", isRedirectOutlookClientEnabled = {3}", new object[]
			{
				path,
				userAgent,
				authorization,
				enabled
			});
			return enabled && !string.IsNullOrEmpty(path) && string.Equals(path, "/autodiscover/autodiscover.xml", StringComparison.OrdinalIgnoreCase) && !(authorization != "Basic") && AutodiscoverProxy.CanRedirectOutlookClient(userAgent);
		}

		internal static bool CanRedirectOutlookClient(string userAgent)
		{
			int num;
			int? num2;
			int? num3;
			if (AutodiscoverProxy.TryParseOutlookUserAgent(userAgent, out num, out num2, out num3))
			{
				if (num < 14)
				{
					return true;
				}
				if (num == 14 && num2 != null && num2.Value == 0 && num3 != null && num3.Value < 4327)
				{
					return true;
				}
			}
			return false;
		}

		internal static bool IsRedirectFaultInjectionEnabledOnRequest(bool canFollowRedirect)
		{
			string text = FaultInjection.TraceTest<string>((FaultInjection.LIDs)2535861565U);
			return text != null && !canFollowRedirect && VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.RedirectOutlookClient.Enabled;
		}

		internal string ProxyingAutodiscoverRequestIfApplicable(ProxyRequestData proxyRequestData, string redirectServer)
		{
			if (proxyRequestData == null)
			{
				return null;
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "AutodiscoverProxy::ProxyingAutodiscoverRequestIfApplicable. Entry. redirectServer = {0}", redirectServer);
			string result = null;
			if (!AutodiscoverProxy.TryProxyAutodiscoverRequest(proxyRequestData, redirectServer, out result))
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<ProxyRequestData, string>(0L, "AutodiscoverProxy::ProxyingAutodiscoverRequestIfApplicable. Cannot proxy this request. proxyRequestData = {0}, redirectServer = {1}", proxyRequestData, redirectServer);
			}
			else
			{
				proxyRequestData.Response.AddHeader("X-Autodiscover-Proxy", redirectServer);
			}
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "AutodiscoverProxy::ProxyingAutodiscoverRequestIfApplicable. Exit. redirectServer = {0}", redirectServer);
			return result;
		}

		private static string GetResponse(Stream responseStream)
		{
			string result;
			using (StreamReader streamReader = new StreamReader(responseStream))
			{
				result = streamReader.ReadToEnd();
			}
			return result;
		}

		private static bool TryProxyAutodiscoverRequest(ProxyRequestData proxyRequestData, string redirectServer, out string rawResponse)
		{
			ExTraceGlobals.AuthenticationTracer.TraceDebug<string>(0L, "AutodiscoverProxy::TryProxyAutodiscoverRequest. Entrypoint. redirectServer = {0}.", redirectServer);
			rawResponse = null;
			if (string.IsNullOrEmpty(redirectServer))
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<string>(0L, "AutodiscoverProxy::TryProxyAutodiscoverRequest. This is an invalid server name. redirectServer = {0}.", redirectServer);
				return false;
			}
			try
			{
				HttpWebRequest httpWebRequest = proxyRequestData.CloneRequest(redirectServer);
				CertificateValidationManager.SetComponentId(httpWebRequest, "AutodiscoverProxy");
				using (WebResponse response = httpWebRequest.GetResponse())
				{
					using (Stream responseStream = response.GetResponseStream())
					{
						rawResponse = AutodiscoverProxy.GetResponse(responseStream);
						if (rawResponse == null)
						{
							ExTraceGlobals.AuthenticationTracer.TraceError<string>(0L, "AutodiscoverProxy::TryProxyAutodiscoverRequest. received wrong number of user response. rawResponse = {0}.", rawResponse);
							return false;
						}
						ExTraceGlobals.AuthenticationTracer.TraceDebug<string, string>(0L, "AutodiscoverProxy::TryProxyAutodiscoverRequest. Received response for user. redirectServer = {0}, rawResponse = {1}.", redirectServer, rawResponse);
					}
				}
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<LocalizedException>(0L, "AutodiscoverProxy::TryProxyAutodiscoverRequest caught with a LocalizedException. LocalizedException = {0}.", arg);
				return false;
			}
			catch (WebException arg2)
			{
				ExTraceGlobals.AuthenticationTracer.TraceError<WebException>(0L, "AutodiscoverProxy::TryProxyAutodiscoverRequest caught with a WebException. WebException = {0}.", arg2);
				return false;
			}
			return true;
		}

		private const string ComponentId = "AutodiscoverProxy";

		private static readonly Regex outlookUserAgentRegEx = new Regex("Microsoft\\sOffice/(?<majorVersion2>\\d+)\\.\\d+\\s\\([\\w|\\d|\\s|.|;]+\\sOutlook\\s(?<majorVersion>\\d+)\\.(?<minorVersion>\\d+)(\\.(?<buildNumber>\\d+)){0,1};", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	}
}
