using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.WebServices.Autodiscover;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Monitoring
{
	public sealed class TestOAuthConnectivityHelper
	{
		public static ResultType SendExchangeOAuthRequest(ADUser user, string orgDomain, Uri targetUri, out string diagnosticMessage, bool appOnly = false, bool useCachedToken = false, bool reloadConfig = false)
		{
			string domain = TestOAuthConnectivityHelper.GetDomain(user, orgDomain);
			if (domain == null)
			{
				diagnosticMessage = Strings.NullUserError;
				return ResultType.Error;
			}
			ICredentials icredentials = TestOAuthConnectivityHelper.GetICredentials(false, user, domain);
			OAuthCredentials oauthCredentials = icredentials as OAuthCredentials;
			if (icredentials == null)
			{
				diagnosticMessage = Strings.NullUserError;
				return ResultType.Error;
			}
			string value = "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\" xmlns:t=\"http://schemas.microsoft.com/exchange/services/2006/types\">\r\n                                <soap:Header>\r\n                                <t:RequestServerVersion Version=\"Exchange2012\"/>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                <GetFolder xmlns=\"http://schemas.microsoft.com/exchange/services/2006/messages\">\r\n                                    <FolderShape>\r\n                                    <t:BaseShape>IdOnly</t:BaseShape>\r\n                                    </FolderShape>\r\n                                    <FolderIds>\r\n                                    <t:DistinguishedFolderId Id=\"inbox\"/>\r\n                                    </FolderIds>\r\n                                </GetFolder>\r\n                                </soap:Body>\r\n                            </soap:Envelope>";
			ValidationResultCollector resultCollector = new ValidationResultCollector();
			LocalConfiguration localConfiguration = LocalConfiguration.Load(resultCollector);
			oauthCredentials.Tracer = new TestOAuthConnectivityHelper.TaskOauthOutboundTracer();
			oauthCredentials.LocalConfiguration = localConfiguration;
			Guid value2 = Guid.NewGuid();
			oauthCredentials.ClientRequestId = new Guid?(value2);
			HttpWebResponse httpWebResponse = null;
			ResultType result = ResultType.Success;
			string text = string.Empty;
			string s = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			string value3 = TestOAuthConnectivityHelper.CheckReloadConfig(reloadConfig);
			string value4 = TestOAuthConnectivityHelper.CheckUseCachedToken(useCachedToken);
			stringBuilder.AppendLine(value3);
			stringBuilder.AppendLine(value4);
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(targetUri.Scheme + "://" + targetUri.Host + "/ews/Exchange.asmx"));
				httpWebRequest.Method = "POST";
				httpWebRequest.ContentType = "text/xml";
				httpWebRequest.Headers.Add("X-ExCompId", "OauthPartnerProbe:");
				httpWebRequest.Headers.Add("client-request-id", value2.ToString());
				httpWebRequest.Headers.Add("request-id", value2.ToString());
				httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
				httpWebRequest.UserAgent = "AMProbe/OAUTH/Exchange";
				httpWebRequest.Credentials = icredentials;
				StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream());
				streamWriter.Write(value);
				streamWriter.Close();
				httpWebResponse = (httpWebRequest.GetResponse() as HttpWebResponse);
				if (httpWebResponse != null && httpWebResponse.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception(Strings.HttpWebRequestFailure(httpWebResponse.StatusCode.ToString()));
				}
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					s = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				text = ex.ToString();
				result = ResultType.Error;
			}
			finally
			{
				if (httpWebResponse != null)
				{
					TestOAuthConnectivityHelper.LogHttpResponseHeaders(httpWebResponse.Headers, ref stringBuilder);
					httpWebResponse.Close();
				}
			}
			stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
			stringBuilder.AppendLine(Strings.ClientRequestId(value2.ToString()));
			stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
			stringBuilder.AppendLine(Strings.TestOAuthResponseDetails("Exchange"));
			stringBuilder.AppendLine(Strings.ResponseMessage(s));
			if (text != string.Empty)
			{
				stringBuilder.AppendLine(Strings.ExceptionHeader);
				stringBuilder.AppendLine(text);
			}
			diagnosticMessage = stringBuilder.ToString();
			return result;
		}

		public static ResultType SendAutodiscoverOAuthRequest(ADUser user, string orgDomain, Uri targetUri, out string diagnosticMessage, bool appOnly = false, bool useCachedToken = false, bool reloadConfig = false)
		{
			string domain = TestOAuthConnectivityHelper.GetDomain(user, orgDomain);
			if (domain == null)
			{
				diagnosticMessage = Strings.NullUserError;
				return ResultType.Error;
			}
			ICredentials icredentials = TestOAuthConnectivityHelper.GetICredentials(appOnly, user, domain);
			OAuthCredentials oauthCredentials = icredentials as OAuthCredentials;
			if (icredentials == null)
			{
				diagnosticMessage = Strings.NullUserError;
				return ResultType.Error;
			}
			StringBuilder stringBuilder = new StringBuilder();
			ValidationResultCollector resultCollector = new ValidationResultCollector();
			LocalConfiguration localConfiguration = LocalConfiguration.Load(resultCollector);
			oauthCredentials.Tracer = new TestOAuthConnectivityHelper.TaskOauthOutboundTracer();
			oauthCredentials.LocalConfiguration = localConfiguration;
			Guid value = Guid.NewGuid();
			oauthCredentials.ClientRequestId = new Guid?(value);
			string value2 = TestOAuthConnectivityHelper.CheckReloadConfig(reloadConfig);
			string value3 = TestOAuthConnectivityHelper.CheckUseCachedToken(useCachedToken);
			stringBuilder.AppendLine(value2);
			stringBuilder.AppendLine(value3);
			AutodiscoverService autodiscoverService = new AutodiscoverService(4);
			autodiscoverService.Url = new Uri(targetUri.Scheme + "://" + targetUri.Host + "/autodiscover/autodiscover.svc");
			autodiscoverService.TraceEnabled = true;
			autodiscoverService.Credentials = new OAuthCredentials(oauthCredentials);
			ResultType result = ResultType.Success;
			try
			{
				string text = (user == null) ? ("@" + domain) : user.PrimarySmtpAddress.ToString();
				GetUserSettingsResponse userSettings = autodiscoverService.GetUserSettings(text, new UserSettingName[]
				{
					58,
					75
				});
				if (userSettings.ErrorCode != null && (userSettings.ErrorCode != 3 || !(text == "@" + domain)))
				{
					result = ResultType.Error;
				}
			}
			catch (Exception ex)
			{
				stringBuilder.AppendLine(ex.ToString());
				result = ResultType.Error;
			}
			stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
			stringBuilder.AppendLine(Strings.ClientRequestId(value.ToString()));
			stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
			stringBuilder.AppendLine(Strings.TestOAuthResponseDetails("Exchange"));
			stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
			stringBuilder.AppendLine(Strings.TestOAuthResponseDetails("Autodiscover"));
			stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
			diagnosticMessage = stringBuilder.ToString();
			return result;
		}

		public static ResultType SendGenericOAuthRequest(ADUser user, string orgDomain, Uri targetUri, out string diagnosticMessage, bool appOnly = false, bool useCachedToken = false, bool reloadConfig = false)
		{
			string domain = TestOAuthConnectivityHelper.GetDomain(user, orgDomain);
			if (domain == null)
			{
				diagnosticMessage = Strings.NullUserError;
				return ResultType.Error;
			}
			ICredentials icredentials = TestOAuthConnectivityHelper.GetICredentials(appOnly, user, domain);
			OAuthCredentials oauthCredentials = icredentials as OAuthCredentials;
			if (icredentials == null)
			{
				diagnosticMessage = Strings.NullUserError;
				return ResultType.Error;
			}
			ValidationResultCollector resultCollector = new ValidationResultCollector();
			LocalConfiguration localConfiguration = LocalConfiguration.Load(resultCollector);
			oauthCredentials.Tracer = new TestOAuthConnectivityHelper.TaskOauthOutboundTracer();
			oauthCredentials.LocalConfiguration = localConfiguration;
			Guid value = Guid.NewGuid();
			oauthCredentials.ClientRequestId = new Guid?(value);
			HttpWebResponse httpWebResponse = null;
			ResultType result = ResultType.Success;
			string text = string.Empty;
			string s = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			string value2 = TestOAuthConnectivityHelper.CheckReloadConfig(reloadConfig);
			string value3 = TestOAuthConnectivityHelper.CheckUseCachedToken(useCachedToken);
			stringBuilder.AppendLine(value2);
			stringBuilder.AppendLine(value3);
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(new Uri(targetUri.ToString()));
				httpWebRequest.Method = "GET";
				httpWebRequest.ContentType = "text/xml";
				httpWebRequest.Headers.Add("X-ExCompId", "OauthPartnerProbe:");
				httpWebRequest.Headers.Add("client-request-id", value.ToString());
				httpWebRequest.Headers.Add("request-id", value.ToString());
				httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
				httpWebRequest.UserAgent = "AMProbe/OAUTH/Exchange";
				httpWebRequest.Credentials = icredentials;
				httpWebResponse = (httpWebRequest.GetResponse() as HttpWebResponse);
				if (httpWebResponse != null && httpWebResponse.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception(Strings.ResponseMessage(httpWebResponse.StatusCode.ToString()));
				}
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					s = streamReader.ReadToEnd();
				}
			}
			catch (Exception ex)
			{
				text = ex.ToString();
				result = ResultType.Error;
			}
			finally
			{
				if (httpWebResponse != null)
				{
					TestOAuthConnectivityHelper.LogHttpResponseHeaders(httpWebResponse.Headers, ref stringBuilder);
					httpWebResponse.Close();
				}
			}
			stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
			stringBuilder.AppendLine(Strings.ClientRequestId(value.ToString()));
			stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
			stringBuilder.AppendLine(Strings.TestOAuthResponseDetails("Generic"));
			stringBuilder.AppendLine(Strings.ResponseMessage(s));
			if (text != string.Empty)
			{
				stringBuilder.AppendLine(Strings.ExceptionHeader);
				stringBuilder.AppendLine(text);
			}
			diagnosticMessage = stringBuilder.ToString();
			return result;
		}

		public static ResultType SendLyncOAuthRequest(ADUser user, Uri targetUri, out string diagnosticMessage, bool appOnly = false, bool useCachedToken = false, bool reloadConfig = false)
		{
			int startIndex = user.GetFederatedIdentity().Identity.IndexOf('@') + 1;
			string domain = user.GetFederatedIdentity().Identity.Substring(startIndex);
			StringBuilder stringBuilder = new StringBuilder();
			ICredentials icredentials = TestOAuthConnectivityHelper.GetICredentials(appOnly, user, domain);
			OAuthCredentials oauthCredentials = icredentials as OAuthCredentials;
			string text = string.Empty;
			foreach (ProxyAddress proxyAddress in user.EmailAddresses)
			{
				if (proxyAddress.ToString().Contains("sip:"))
				{
					text = TestOAuthConnectivityHelper.FromSipFormat(proxyAddress.ToString());
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = user.PrimarySmtpAddress.ToString();
			}
			if (string.IsNullOrEmpty(text))
			{
				diagnosticMessage = Strings.EMailAddressNotFound(user.Alias);
				return ResultType.Error;
			}
			Guid value = Guid.NewGuid();
			oauthCredentials.ClientRequestId = new Guid?(value);
			stringBuilder.AppendLine(Strings.ClientRequestId(value.ToString()));
			string value2 = TestOAuthConnectivityHelper.CheckReloadConfig(reloadConfig);
			string value3 = TestOAuthConnectivityHelper.CheckUseCachedToken(useCachedToken);
			stringBuilder.AppendLine(value2);
			stringBuilder.AppendLine(value3);
			ValidationResultCollector resultCollector = new ValidationResultCollector();
			LocalConfiguration localConfiguration = LocalConfiguration.Load(resultCollector);
			oauthCredentials.Tracer = new TestOAuthConnectivityHelper.TaskOauthOutboundTracer();
			oauthCredentials.LocalConfiguration = localConfiguration;
			LyncAnonymousAutodiscoverResult lyncAnonymousAutodiscoverResult = null;
			try
			{
				lyncAnonymousAutodiscoverResult = LyncAutodiscoverWorker.GetAuthenticatedAutodiscoverEndpoint(text, domain);
			}
			catch (WebException ex)
			{
				stringBuilder.AppendLine(Strings.DiagnosticsHeader);
				stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
				stringBuilder.AppendLine(Strings.AutodiscoverFailure);
				stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
				stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
				stringBuilder.AppendLine(ex.ToString());
				diagnosticMessage = stringBuilder.ToString();
				return ResultType.Error;
			}
			catch (Exception ex2)
			{
				stringBuilder.AppendLine(Strings.DiagnosticsHeader);
				stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
				stringBuilder.AppendLine(Strings.AutodiscoverFailure);
				stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
				stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
				stringBuilder.AppendLine(ex2.ToString());
				diagnosticMessage = stringBuilder.ToString();
				return ResultType.Error;
			}
			if (string.IsNullOrEmpty(lyncAnonymousAutodiscoverResult.AuthenticatedServerUri))
			{
				stringBuilder.AppendLine(Strings.DiagnosticsHeader);
				stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
				stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
				stringBuilder.AppendLine(Strings.NoAuthenticatedServerUri);
				stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
				diagnosticMessage = stringBuilder.ToString();
				return ResultType.Error;
			}
			stringBuilder.AppendLine(Strings.DiagnosticsHeader);
			stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
			LyncAutodiscoverResult lyncAutodiscoverResult = null;
			try
			{
				lyncAutodiscoverResult = LyncAutodiscoverWorker.GetUcwaUrl(lyncAnonymousAutodiscoverResult.AuthenticatedServerUri, icredentials);
			}
			catch (WebException ex3)
			{
				stringBuilder.AppendLine(Strings.DiagnosticsHeader);
				stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
				stringBuilder.AppendLine(Strings.UCWADiscoveryUrlException);
				stringBuilder.AppendLine(lyncAutodiscoverResult.Response);
				stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
				stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
				stringBuilder.AppendLine(ex3.ToString());
				diagnosticMessage = stringBuilder.ToString();
				return ResultType.Error;
			}
			catch (Exception ex4)
			{
				stringBuilder.AppendLine(Strings.DiagnosticsHeader);
				stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
				stringBuilder.AppendLine(Strings.UCWADiscoveryUrlException);
				stringBuilder.AppendLine(lyncAutodiscoverResult.Response);
				stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
				stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
				stringBuilder.AppendLine(ex4.ToString());
				diagnosticMessage = stringBuilder.ToString();
				return ResultType.Error;
			}
			if (string.IsNullOrEmpty(lyncAutodiscoverResult.UcwaDiscoveryUrl))
			{
				stringBuilder.AppendLine(Strings.DiagnosticsHeader);
				stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
				stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
				stringBuilder.AppendLine(Strings.UCWADiscoveryUrlEmpty);
				stringBuilder.AppendLine(lyncAutodiscoverResult.Response);
				stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
				diagnosticMessage = stringBuilder.ToString();
				return ResultType.Error;
			}
			stringBuilder.AppendLine(Strings.DiagnosticsHeader);
			stringBuilder.AppendLine(lyncAnonymousAutodiscoverResult.DiagnosticInfo);
			stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
			stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
			diagnosticMessage = stringBuilder.ToString();
			return ResultType.Success;
		}

		public static ResultType SendSPOAuthRequest(ADUser user, Uri targetUri, out string diagnosticMessage, bool appOnly = false, bool useCachedToken = false, bool reloadConfig = false)
		{
			int startIndex = user.GetFederatedIdentity().Identity.IndexOf('@') + 1;
			string domain = user.GetFederatedIdentity().Identity.Substring(startIndex);
			ICredentials icredentials = TestOAuthConnectivityHelper.GetICredentials(appOnly, user, domain);
			OAuthCredentials oauthCredentials = icredentials as OAuthCredentials;
			if (icredentials == null)
			{
				diagnosticMessage = Strings.NullUserError;
				return ResultType.Error;
			}
			Guid value = Guid.NewGuid();
			oauthCredentials.ClientRequestId = new Guid?(value);
			ValidationResultCollector resultCollector = new ValidationResultCollector();
			LocalConfiguration localConfiguration = LocalConfiguration.Load(resultCollector);
			oauthCredentials.Tracer = new TestOAuthConnectivityHelper.TaskOauthOutboundTracer();
			oauthCredentials.LocalConfiguration = localConfiguration;
			string text = targetUri.Scheme + "://" + targetUri.Host + "/_vti_bin/listdata.svc";
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentType = "text/xml";
			httpWebRequest.Headers.Add("X-ExCompId", "OauthPartnerProbe:");
			httpWebRequest.Headers.Add("client-request-id", value.ToString());
			httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Bearer");
			httpWebRequest.UserAgent = "AMProbe/OAUTH/Sharepoint";
			httpWebRequest.Credentials = icredentials;
			httpWebRequest.PreAuthenticate = true;
			string value2 = string.Empty;
			StringBuilder stringBuilder = new StringBuilder(1024);
			stringBuilder.AppendLine(Strings.ClientRequestId(value.ToString()));
			string value3 = TestOAuthConnectivityHelper.CheckReloadConfig(reloadConfig);
			string value4 = TestOAuthConnectivityHelper.CheckUseCachedToken(useCachedToken);
			stringBuilder.AppendLine(value3);
			stringBuilder.AppendLine(value4);
			HttpWebResponse httpWebResponse = null;
			ResultType result = ResultType.Success;
			string text2 = string.Empty;
			try
			{
				stringBuilder.AppendLine(Strings.OAuthRequestEndPoint(text));
				httpWebResponse = (httpWebRequest.GetResponse() as HttpWebResponse);
				if (httpWebResponse != null && httpWebResponse.StatusCode != HttpStatusCode.OK)
				{
					throw new Exception(Strings.HttpWebRequestFailure(httpWebResponse.StatusCode.ToString()));
				}
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					value2 = streamReader.ReadToEnd();
				}
			}
			catch (WebException ex)
			{
				text2 = ex.ToString();
				result = ResultType.Error;
			}
			finally
			{
				if (httpWebResponse != null)
				{
					TestOAuthConnectivityHelper.LogHttpResponseHeaders(httpWebResponse.Headers, ref stringBuilder);
				}
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
			}
			stringBuilder.AppendLine(Strings.TestOutboundOauthLog);
			stringBuilder.AppendLine(Strings.TestOAuthResponseDetails("SharePoint"));
			stringBuilder.AppendLine(oauthCredentials.Tracer.ToString());
			stringBuilder.AppendLine(Strings.ResponseHeader);
			stringBuilder.AppendLine(value2);
			if (text2 != string.Empty)
			{
				stringBuilder.AppendLine(Strings.ExceptionHeader);
				stringBuilder.AppendLine(text2);
			}
			diagnosticMessage = stringBuilder.ToString();
			return result;
		}

		private static string GetDomain(ADUser user, string orgDomain)
		{
			if (!string.IsNullOrEmpty(orgDomain))
			{
				return orgDomain;
			}
			if (user == null)
			{
				return null;
			}
			string text = user.PrimarySmtpAddress.ToString();
			int startIndex = text.IndexOf('@') + 1;
			return text.Substring(startIndex);
		}

		private static string CheckUseCachedToken(bool useCachedToken)
		{
			string result = string.Empty;
			if (!useCachedToken)
			{
				result = Strings.ClearingCache;
				ACSTokenCache.Instance.ClearCache();
			}
			return result;
		}

		private static string CheckReloadConfig(bool reloadConfig)
		{
			DateTime lastRefreshDateTime = ConfigProvider.Instance.LastRefreshDateTime;
			TimeSpan timeSpan = DateTime.UtcNow.Subtract(lastRefreshDateTime);
			string result = Strings.LastConfigLoadTime(lastRefreshDateTime.ToString(), Math.Round(timeSpan.TotalMinutes).ToString());
			if (reloadConfig)
			{
				ConfigProvider.Instance.ManuallyReload();
			}
			return result;
		}

		private static void LogHttpResponseHeaders(WebHeaderCollection headers, ref StringBuilder detail)
		{
			for (int i = 0; i < headers.Count; i++)
			{
				string key = headers.GetKey(i);
				string[] values = headers.GetValues(key);
				if (values.Length > 0)
				{
					detail.AppendLine(Strings.HeaderValues(key));
					for (int j = 0; j < values.Length; j++)
					{
						detail.Append(string.Format("\t{0}", values[j]));
					}
				}
			}
		}

		internal static string FromSipFormat(string sipAddress)
		{
			if (string.IsNullOrEmpty(sipAddress))
			{
				return string.Empty;
			}
			if (sipAddress.Length > "sip:".Length && string.Compare(sipAddress.Substring(0, "sip:".Length), "sip:", StringComparison.OrdinalIgnoreCase) == 0)
			{
				return sipAddress.Substring("sip:".Length);
			}
			return sipAddress;
		}

		internal static ICredentials GetICredentials(bool appOnly, ADUser user, string domain)
		{
			ICredentials result;
			if (appOnly)
			{
				OrganizationId organizationId = OrganizationId.FromAcceptedDomain(domain);
				result = OAuthCredentials.GetOAuthCredentialsForAppToken(organizationId, domain);
			}
			else
			{
				if (user == null)
				{
					throw new Exception(Strings.NullUserError);
				}
				OrganizationId organizationId = user.OrganizationId;
				if (organizationId == null)
				{
					throw new Exception(Strings.NullOrgIdException(user.Name));
				}
				result = OAuthCredentials.GetOAuthCredentialsForAppActAsToken(organizationId, user, domain);
			}
			return result;
		}

		private const string SipPrefix = "sip:";

		public const string ComponentId = "OauthPartnerProbe:";

		private const string SharepointUserAgentString = "AMProbe/OAUTH/Sharepoint";

		private const string ExchangeUserAgentString = "AMProbe/OAUTH/Exchange";

		private const string ewsUriExtension = "/ews/Exchange.asmx";

		private const string autoDUriExtension = "/autodiscover/autodiscover.svc";

		private const string spUriExtension = "/_vti_bin/listdata.svc";

		private sealed class TaskOauthOutboundTracer : IOutboundTracer
		{
			public void LogInformation(int hashCode, string formatString, params object[] args)
			{
				this.result.Append("Information:");
				this.result.AppendLine(string.Format(formatString, args));
			}

			public void LogWarning(int hashCode, string formatString, params object[] args)
			{
				this.result.Append("Warning:");
				this.result.AppendLine(string.Format(formatString, args));
			}

			public void LogError(int hashCode, string formatString, params object[] args)
			{
				this.result.Append("Error:");
				this.result.AppendLine(string.Format(formatString, args));
			}

			public void LogToken(int hashCode, string tokenString)
			{
				this.result.Append("Token:");
				this.result.AppendLine(tokenString);
			}

			public override string ToString()
			{
				return this.result.ToString();
			}

			private readonly StringBuilder result = new StringBuilder();
		}
	}
}
