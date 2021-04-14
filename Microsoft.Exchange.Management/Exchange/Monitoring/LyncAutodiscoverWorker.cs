using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;

namespace Microsoft.Exchange.Monitoring
{
	internal class LyncAutodiscoverWorker
	{
		internal static string GetUcwaDiscoveryUrl(string userAddress, ICredentials credentials)
		{
			int num = userAddress.LastIndexOf("@");
			if (num <= 0)
			{
				return string.Empty;
			}
			string domain = userAddress.Substring(num + 1);
			LyncAnonymousAutodiscoverResult authenticatedAutodiscoverEndpoint = LyncAutodiscoverWorker.GetAuthenticatedAutodiscoverEndpoint(userAddress, domain);
			string authenticatedServerUri = authenticatedAutodiscoverEndpoint.AuthenticatedServerUri;
			if (string.IsNullOrEmpty(authenticatedServerUri.Trim()) || authenticatedServerUri.Length == 0)
			{
				return string.Empty;
			}
			LyncAutodiscoverResult ucwaUrl = LyncAutodiscoverWorker.GetUcwaUrl(authenticatedAutodiscoverEndpoint.AuthenticatedServerUri, credentials);
			return ucwaUrl.UcwaDiscoveryUrl;
		}

		internal static string ExecuteAnonymousLyncAutodiscoverRequests(bool sendInternal, string domain, string user)
		{
			LyncAutodiscoverWorker.details.AppendLine(string.Format("Executing: ExecuteAnonymousLyncAutodiscoverRequests", new object[0]));
			ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(LyncAutodiscoverWorker.CertificateValidationCallBack);
			string result = string.Empty;
			string arg = string.Format("?sipuri={0}", user);
			try
			{
				string text = sendInternal ? string.Format("http://{0}.{1}/autodiscover/autodiscoverservice.svc/root{2}", "lyncdiscoverinternal", domain, arg) : string.Format("http://{0}.{1}/autodiscover/autodiscoverservice.svc/root{2}", "lyncdiscover", domain, arg);
				string text2 = sendInternal ? string.Format("https://{0}.{1}/autodiscover/autodiscoverservice.svc/root{2}", "lyncdiscoverinternal", domain, arg) : string.Format("https://{0}.{1}/autodiscover/autodiscoverservice.svc/root{2}", "lyncdiscover", domain, arg);
				LyncAutodiscoverWorker.details.AppendLine(string.Format("HttpUrl:{0}", text));
				LyncAutodiscoverWorker.details.AppendLine(string.Format("HttpsUrl:{0}", text2));
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text);
				httpWebRequest.Accept = "application/vnd.microsoft.rtc.autodiscover+xml;v=1";
				LyncAutodiscoverRequestState lyncAutodiscoverRequestState = new LyncAutodiscoverRequestState();
				lyncAutodiscoverRequestState.Request = httpWebRequest;
				HttpWebRequest httpWebRequest2 = (HttpWebRequest)WebRequest.Create(text2);
				httpWebRequest2.Accept = "application/vnd.microsoft.rtc.autodiscover+xml;v=1";
				LyncAutodiscoverRequestState lyncAutodiscoverRequestState2 = new LyncAutodiscoverRequestState();
				lyncAutodiscoverRequestState2.Request = httpWebRequest2;
				LyncAutodiscoverWorker.allDone.Reset();
				IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(LyncAutodiscoverWorker.ProcessLyncAnonymousAutodiscoverResponse), lyncAutodiscoverRequestState);
				IAsyncResult asyncResult2 = httpWebRequest2.BeginGetResponse(new AsyncCallback(LyncAutodiscoverWorker.ProcessLyncAnonymousAutodiscoverResponse), lyncAutodiscoverRequestState2);
				ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(LyncAutodiscoverWorker.TimeoutCallback), httpWebRequest, 120000, true);
				ThreadPool.RegisterWaitForSingleObject(asyncResult2.AsyncWaitHandle, new WaitOrTimerCallback(LyncAutodiscoverWorker.TimeoutCallback), httpWebRequest2, 120000, true);
				LyncAutodiscoverWorker.allDone.WaitOne();
				if (lyncAutodiscoverRequestState.Response != null)
				{
					lyncAutodiscoverRequestState.Response.Close();
				}
				if (lyncAutodiscoverRequestState2.Response != null)
				{
					lyncAutodiscoverRequestState2.Response.Close();
				}
				if (string.IsNullOrEmpty(lyncAutodiscoverRequestState.TargetUrl) && string.IsNullOrEmpty(lyncAutodiscoverRequestState2.TargetUrl))
				{
					LyncAutodiscoverWorker.details.AppendLine(string.Format("Both http and https responses are empty.", new object[0]));
					return result;
				}
				if (!string.IsNullOrEmpty(lyncAutodiscoverRequestState.TargetUrl))
				{
					if (lyncAutodiscoverRequestState.IsRedirect)
					{
						LyncAutodiscoverWorker.details.AppendLine(string.Format("Redirecting to {0}.", lyncAutodiscoverRequestState.TargetUrl));
						result = LyncAutodiscoverWorker.ExecuteAnonymousLyncAutodiscoverRedirect(lyncAutodiscoverRequestState.TargetUrl, 0);
					}
					else
					{
						LyncAutodiscoverWorker.details.AppendLine(string.Format("Authenticated Endpoint: {0}.", lyncAutodiscoverRequestState.TargetUrl));
						result = lyncAutodiscoverRequestState.TargetUrl;
					}
				}
				else if (lyncAutodiscoverRequestState2.IsRedirect)
				{
					LyncAutodiscoverWorker.details.AppendLine(string.Format("Redirecting to {0}.", lyncAutodiscoverRequestState.TargetUrl));
					result = LyncAutodiscoverWorker.ExecuteAnonymousLyncAutodiscoverRedirect(lyncAutodiscoverRequestState.TargetUrl, 0);
				}
				else
				{
					LyncAutodiscoverWorker.details.AppendLine(string.Format("Authenticated Endpoint: {0}.", lyncAutodiscoverRequestState.TargetUrl));
					result = lyncAutodiscoverRequestState2.TargetUrl;
				}
			}
			catch (WebException ex)
			{
				LyncAutodiscoverWorker.details.AppendLine(string.Format("Exception: {0}.", ex.ToString()));
			}
			return result;
		}

		internal static string ExecuteAnonymousLyncAutodiscoverRedirect(string requestUrl, int redirectCount)
		{
			string empty = string.Empty;
			if (redirectCount >= 10)
			{
				return empty;
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
			httpWebRequest.Accept = "application/vnd.microsoft.rtc.autodiscover+xml;v=1";
			LyncAutodiscoverRequestState lyncAutodiscoverRequestState = new LyncAutodiscoverRequestState();
			lyncAutodiscoverRequestState.Request = httpWebRequest;
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode == HttpStatusCode.OK && LyncAutodiscoverWorker.GetAuthenticatedAutodiscoverEndpointFromHttpWebResponse(httpWebResponse, redirectCount, out empty))
				{
					return empty;
				}
			}
			catch (WebException)
			{
			}
			return empty;
		}

		internal static void ProcessLyncAnonymousAutodiscoverResponse(IAsyncResult asyncResult)
		{
			LyncAutodiscoverRequestState lyncAutodiscoverRequestState = (LyncAutodiscoverRequestState)asyncResult.AsyncState;
			HttpWebRequest request = lyncAutodiscoverRequestState.Request;
			if (request != null)
			{
				try
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
					string targetUrl;
					if (httpWebResponse.StatusCode == HttpStatusCode.OK && LyncAutodiscoverWorker.GetAuthenticatedAutodiscoverEndpointFromHttpWebResponse(httpWebResponse, 0, out targetUrl))
					{
						lyncAutodiscoverRequestState.TargetUrl = targetUrl;
					}
				}
				catch (WebException)
				{
					lyncAutodiscoverRequestState.TargetUrl = string.Empty;
				}
				LyncAutodiscoverWorker.allDone.Set();
			}
		}

		internal static bool GetAuthenticatedAutodiscoverEndpointFromHttpWebResponse(HttpWebResponse response, int redirectCount, out string endpoint)
		{
			endpoint = string.Empty;
			try
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(streamReader.ReadToEnd());
						using (XmlNodeList xmlNodeList = xmlDocument.SelectNodes("AutodiscoverResponse/Root/Link"))
						{
							if (xmlNodeList.Count == 0)
							{
								return false;
							}
							string redirectUrl = LyncAutodiscoverWorker.GetRedirectUrl(xmlNodeList);
							if (!string.IsNullOrEmpty(redirectUrl))
							{
								endpoint = LyncAutodiscoverWorker.ExecuteAnonymousLyncAutodiscoverRedirect(redirectUrl, redirectCount + 1);
							}
							else
							{
								endpoint = LyncAutodiscoverWorker.GetTargetAttributeValue(xmlNodeList, "href", "token", "OAuth");
							}
						}
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		internal static LyncAutodiscoverResult GetUcwaUrl(string authenticatedEndpointUrl, ICredentials credentials)
		{
			LyncAutodiscoverWorker.details.AppendLine("Calling GetUcwaUrl");
			LyncAutodiscoverResult lyncAutodiscoverResult = new LyncAutodiscoverResult();
			lyncAutodiscoverResult.IsUcwaSupported = false;
			lyncAutodiscoverResult.UcwaDiscoveryUrl = string.Empty;
			string text = string.Empty;
			try
			{
				ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(LyncAutodiscoverWorker.CertificateValidationCallBack);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(authenticatedEndpointUrl);
				httpWebRequest.Accept = "application/vnd.microsoft.rtc.autodiscover+xml;v=1";
				httpWebRequest.Timeout = 120000;
				httpWebRequest.Credentials = credentials;
				LyncAutodiscoverRequestState lyncAutodiscoverRequestState = new LyncAutodiscoverRequestState();
				lyncAutodiscoverRequestState.Request = httpWebRequest;
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse != null)
				{
					lyncAutodiscoverResult.Response = httpWebResponse.ToString();
				}
				else
				{
					LyncAutodiscoverWorker.details.AppendLine("Response is NULL");
				}
				if (httpWebResponse.StatusCode == HttpStatusCode.OK)
				{
					LyncAutodiscoverWorker.details.AppendLine("Calling GetUcwaUrlFromHttpWebResponse to get ucwa URL.");
					if (LyncAutodiscoverWorker.GetUcwaUrlFromHttpWebResponse(httpWebResponse, credentials, 0, out text))
					{
						LyncAutodiscoverWorker.details.AppendLine(string.Format("ucwa URL: {0}", text));
						lyncAutodiscoverResult.UcwaDiscoveryUrl = text;
						return lyncAutodiscoverResult;
					}
					LyncAutodiscoverWorker.details.AppendLine(string.Format("unable to find ucwa URL from response.", new object[0]));
				}
				else
				{
					LyncAutodiscoverWorker.details.AppendLine(string.Format("Http status code: {0}", httpWebResponse.StatusCode));
				}
			}
			catch (WebException ex)
			{
				LyncAutodiscoverWorker.details.AppendLine(string.Format("Exception: {0}", ex.ToString()));
				HttpWebResponse httpWebResponse2 = ex.Response as HttpWebResponse;
				if (httpWebResponse2 != null)
				{
					lyncAutodiscoverResult.Response = httpWebResponse2.ToString();
					HttpStatusCode statusCode = httpWebResponse2.StatusCode;
					if (statusCode == HttpStatusCode.Unauthorized)
					{
						text = ((ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
					}
				}
			}
			lyncAutodiscoverResult.UcwaDiscoveryUrl = text;
			lyncAutodiscoverResult.IsUcwaSupported = !string.IsNullOrWhiteSpace(text);
			lyncAutodiscoverResult.DiagnosticInfo = LyncAutodiscoverWorker.details.ToString();
			LyncAutodiscoverWorker.details.Clear();
			return lyncAutodiscoverResult;
		}

		internal static bool GetUcwaUrlFromHttpWebResponse(HttpWebResponse response, ICredentials credentials, int redirectCount, out string ucwaUrl)
		{
			ucwaUrl = string.Empty;
			try
			{
				using (Stream responseStream = response.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						XmlDocument xmlDocument = new XmlDocument();
						xmlDocument.LoadXml(streamReader.ReadToEnd());
						using (XmlNodeList xmlNodeList = xmlDocument.SelectNodes("AutodiscoverResponse/User/Link"))
						{
							if (xmlNodeList.Count == 0)
							{
								return false;
							}
							string redirectUrl = LyncAutodiscoverWorker.GetRedirectUrl(xmlNodeList);
							if (!string.IsNullOrEmpty(redirectUrl))
							{
								ucwaUrl = LyncAutodiscoverWorker.ExecuteAuthenticatedLyncAutodiscoverRedirect(redirectUrl, credentials, redirectCount + 1);
							}
							else
							{
								ucwaUrl = LyncAutodiscoverWorker.GetTargetAttributeValue(xmlNodeList, "href", "token", "Internal/Ucwa");
								if (string.IsNullOrEmpty(ucwaUrl))
								{
									ucwaUrl = LyncAutodiscoverWorker.GetTargetAttributeValue(xmlNodeList, "href", "token", "External/Ucwa");
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		internal static string ExecuteAuthenticatedLyncAutodiscoverRedirect(string requestUrl, ICredentials credentials, int redirectCount)
		{
			string result = string.Empty;
			if (redirectCount >= 10)
			{
				return result;
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUrl);
			httpWebRequest.Accept = "application/vnd.microsoft.rtc.autodiscover+xml;v=1";
			try
			{
				HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				if (httpWebResponse.StatusCode == HttpStatusCode.OK && LyncAutodiscoverWorker.GetUcwaUrlFromHttpWebResponse(httpWebResponse, credentials, redirectCount, out result))
				{
					return result;
				}
			}
			catch (WebException ex)
			{
				result = ((ex.InnerException == null) ? ex.Message : ex.InnerException.Message);
			}
			return result;
		}

		internal static string GetRedirectUrl(XmlNodeList linkNodes)
		{
			return LyncAutodiscoverWorker.GetTargetAttributeValue(linkNodes, "href", "token", "Redirect");
		}

		internal static string GetTargetAttributeValue(XmlNodeList nodes, string targetAttributeName, string selectAttributeName, string selectAttributeValue)
		{
			foreach (object obj in nodes)
			{
				XmlNode xmlNode = (XmlNode)obj;
				if (xmlNode.Attributes[selectAttributeName] == null)
				{
					return string.Empty;
				}
				if (xmlNode.Attributes[selectAttributeName].Value == selectAttributeValue)
				{
					if (xmlNode.Attributes[targetAttributeName] == null)
					{
						return string.Empty;
					}
					return xmlNode.Attributes[targetAttributeName].Value;
				}
			}
			return string.Empty;
		}

		internal static LyncAnonymousAutodiscoverResult GetAuthenticatedAutodiscoverEndpoint(string user, string domain)
		{
			LyncAnonymousAutodiscoverResult lyncAnonymousAutodiscoverResult = new LyncAnonymousAutodiscoverResult();
			LyncAutodiscoverWorker.details.Append("Issue http requests to the internal autodiscover servers");
			lyncAnonymousAutodiscoverResult.AuthenticatedServerUri = LyncAutodiscoverWorker.ExecuteAnonymousLyncAutodiscoverRequests(true, domain, user);
			if (string.IsNullOrEmpty(lyncAnonymousAutodiscoverResult.AuthenticatedServerUri))
			{
				LyncAutodiscoverWorker.details.Append("Issue http requests to the external autodiscover servers");
				lyncAnonymousAutodiscoverResult.AuthenticatedServerUri = LyncAutodiscoverWorker.ExecuteAnonymousLyncAutodiscoverRequests(false, domain, user);
			}
			lyncAnonymousAutodiscoverResult.DiagnosticInfo = LyncAutodiscoverWorker.details.ToString();
			LyncAutodiscoverWorker.details.Clear();
			return lyncAnonymousAutodiscoverResult;
		}

		private static void TimeoutCallback(object state, bool timedOut)
		{
			if (timedOut)
			{
				HttpWebRequest httpWebRequest = state as HttpWebRequest;
				if (httpWebRequest != null)
				{
					httpWebRequest.Abort();
				}
			}
		}

		private static bool CertificateValidationCallBack(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public const string LyncAutodiscoverUrlHttpFormat = "http://{0}.{1}/autodiscover/autodiscoverservice.svc/root{2}";

		public const string LyncAutodiscoverUrlHttpsFormat = "https://{0}.{1}/autodiscover/autodiscoverservice.svc/root{2}";

		public const string SipUriQueryStringFormat = "?sipuri={0}";

		public const string LyncAutodiscoverUrlPrefix = "lyncdiscover";

		public const string LyncAutodiscoverInternalUrlPrefix = "lyncdiscoverinternal";

		public const string LyncAutodiscoverAcceptType = "application/vnd.microsoft.rtc.autodiscover+xml;v=1";

		private const int DefaultTimeout = 120000;

		private const int MaxRedirects = 10;

		private static readonly ManualResetEvent allDone = new ManualResetEvent(false);

		private static StringBuilder details = new StringBuilder();
	}
}
