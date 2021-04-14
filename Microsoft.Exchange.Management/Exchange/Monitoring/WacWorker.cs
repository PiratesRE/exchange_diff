using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Security.OAuth;

namespace Microsoft.Exchange.Monitoring
{
	internal class WacWorker
	{
		static WacWorker()
		{
			if (WacWorker.knownIssuesForWacProbe.Count == 0)
			{
				WacWorker.knownIssuesForWacProbe.Add(WacConnectivityErrors.WacUrlNetworkIssue, "Wac's Endpoint response is not HTTP 200");
				WacWorker.knownIssuesForWacProbe.Add(WacConnectivityErrors.WacInvalidResponse, "Wac's HTML response is HTTP 200 but does not contain a valid document Download Url");
				WacWorker.knownIssuesForWacProbe.Add(WacConnectivityErrors.WacExchangePipelineIssue, "Exchange Wopi Endpoint is up and running correctly, but still Wac is not able to connect to it");
				WacWorker.knownIssuesForWacProbe.Add(WacConnectivityErrors.WopiEndpointIssue, "Exchange Wopi Endpoint is not working correctly");
			}
		}

		internal static WacResult ExecuteWacRequest(string wacTemplateUrl, string owaTemplateUrl, ADUser user, StringBuilder diagnosticsDetails)
		{
			string text = string.Empty;
			string text2 = string.Empty;
			string text3 = string.Empty;
			WacRequestState wacRequestState = new WacRequestState();
			WacResult wacResult = new WacResult();
			try
			{
				string text4 = user.PrimarySmtpAddress.ToString();
				if (string.IsNullOrEmpty(text4))
				{
					diagnosticsDetails.AppendLine(string.Format("Primary SMTP Address not found for user {0}", user.Alias));
					wacResult.Error = wacRequestState.Error;
					return wacResult;
				}
				text = WacWorker.GenerateWopiSrcUrl(owaTemplateUrl, text4);
				LocalTokenIssuer localTokenIssuer = new LocalTokenIssuer(user.OrganizationId);
				TokenResult wacCallbackToken = localTokenIssuer.GetWacCallbackToken(new Uri(text, UriKind.Absolute), text4, Guid.NewGuid().ToString());
				text3 = wacCallbackToken.TokenString;
				text2 = WacWorker.GenerateWacIFrameUrl(text, wacTemplateUrl, text4, text3);
				text = string.Format("{0}&access_token={1}", text, text3);
				diagnosticsDetails.AppendLine("Probe Details:");
				diagnosticsDetails.AppendLine("WacIFrameUrl:");
				diagnosticsDetails.AppendLine(text2);
				diagnosticsDetails.AppendLine("ExchangeCheckFileUrl:");
				diagnosticsDetails.AppendLine(text);
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(text2);
				wacRequestState.Request = httpWebRequest;
				wacRequestState.WacIFrameUrl = text2;
				wacRequestState.WopiUrl = text;
				wacRequestState.Error = false;
				wacRequestState.DiagnosticsDetails = diagnosticsDetails;
				WacWorker.allDone.Reset();
				WacWorker.latencyMeasurementStart = DateTime.UtcNow;
				IAsyncResult asyncResult = httpWebRequest.BeginGetResponse(new AsyncCallback(WacWorker.ProcessWacResponse), wacRequestState);
				ThreadPool.RegisterWaitForSingleObject(asyncResult.AsyncWaitHandle, new WaitOrTimerCallback(WacWorker.TimeoutCallback), httpWebRequest, 120000, true);
				WacWorker.allDone.WaitOne();
				if (wacRequestState.Response != null)
				{
					wacRequestState.Response.Close();
				}
			}
			catch (WebException ex)
			{
				diagnosticsDetails.AppendLine("Stack Trace:");
				diagnosticsDetails.AppendLine(string.Format("Exception: {0}.", ex.ToString()));
				for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
				{
					diagnosticsDetails.AppendLine(string.Format("Inner Exception: {0}.", innerException.ToString()));
				}
				diagnosticsDetails.AppendLine("Diagnostic Tip: There is an unhandled exception occured while running Wac Probe. Please look into the exception details.");
				wacRequestState.Error = true;
			}
			wacResult.Error = wacRequestState.Error;
			return wacResult;
		}

		internal static void ProcessWacResponse(IAsyncResult asyncResult)
		{
			WacRequestState wacRequestState = (WacRequestState)asyncResult.AsyncState;
			int num = (int)(DateTime.UtcNow - WacWorker.latencyMeasurementStart).TotalMilliseconds;
			wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Latency: {0}.", num));
			HttpWebRequest request = wacRequestState.Request;
			if (request != null)
			{
				try
				{
					HttpWebResponse httpWebResponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
					wacRequestState.DiagnosticsDetails.AppendLine(string.Format("X-CorrelationId: {0}.", httpWebResponse.Headers.Get("X-CorrelationId")));
					wacRequestState.DiagnosticsDetails.AppendLine(string.Format("X-OfficeFE: {0}.", httpWebResponse.Headers.Get("X-OfficeFE")));
					wacRequestState.DiagnosticsDetails.AppendLine(string.Format("X-OfficeVersion: {0}.", httpWebResponse.Headers.Get("X-OfficeVersion")));
					wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Wac IFrameUrl Request's StatusDescription: {0}.", httpWebResponse.StatusDescription));
					wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Wac IFrameUrl Request's StatusCode: {0}.", (int)httpWebResponse.StatusCode));
					if (httpWebResponse.StatusCode == HttpStatusCode.OK)
					{
						string text = null;
						using (Stream responseStream = httpWebResponse.GetResponseStream())
						{
							using (StreamReader streamReader = new StreamReader(responseStream))
							{
								text = streamReader.ReadToEnd();
							}
						}
						if (text.Contains(HttpUtility.UrlEncode("DummyAttachment")))
						{
							wacRequestState.DiagnosticsDetails.AppendLine(string.Format("WacProbe succeeded, we are successfully able to verify Wac server's connectivy to Exchange's server", new object[0]));
							wacRequestState.Error = false;
							WacWorker.allDone.Set();
						}
						else
						{
							wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Probe Error: {0}", WacWorker.knownIssuesForWacProbe[WacConnectivityErrors.WacInvalidResponse]));
							wacRequestState.DiagnosticsDetails.AppendLine("Diagnostic Tip: Please determine the response from Exchange's CheckFile Wopi reqest");
							wacRequestState.Error = true;
							WacWorker.CheckExchangeWopiEndpoint(wacRequestState.WopiUrl, wacRequestState.DiagnosticsDetails);
						}
					}
					else
					{
						wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Probe Error: {0}", WacWorker.knownIssuesForWacProbe[WacConnectivityErrors.WacUrlNetworkIssue]));
						wacRequestState.DiagnosticsDetails.AppendLine("Diagnostic Tip: Please use the Wac IFrameUrl Request's StatusCode, X-CorrelationId and X-OfficeFE server while reaching out to Wac911");
						wacRequestState.Error = true;
						WacWorker.allDone.Set();
					}
				}
				catch (WebException ex)
				{
					wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Wac Request Url: {0}.", wacRequestState.WacIFrameUrl));
					wacRequestState.DiagnosticsDetails.AppendLine("Stack Trace:");
					wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Exception: {0}.", ex.ToString()));
					for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
					{
						wacRequestState.DiagnosticsDetails.AppendLine(string.Format("Inner Exception: {0}.", innerException.ToString()));
					}
					wacRequestState.DiagnosticsDetails.AppendLine("Diagnostic Tip: There is an unhandled exception occured while hitting Wac endpoint.");
					wacRequestState.DiagnosticsDetails.AppendLine("Diagnostic Tip: Please look into the exception details.");
					wacRequestState.Error = true;
				}
				WacWorker.allDone.Set();
			}
		}

		private static string GenerateWacIFrameUrl(string wopiUrl, string wacTemplateUrl, string emailAddress, string accessToken)
		{
			string text = wacTemplateUrl.Replace("UI_LLCC", string.Format("ui={0}&", Thread.CurrentThread.CurrentCulture.Name));
			text = text.Replace("DC_LLCC", string.Format("rs={0}&", Thread.CurrentThread.CurrentCulture.Name));
			Uri arg = new Uri(text);
			return string.Format("{0}WOPISrc={1}&access_token={2}", arg, HttpUtility.UrlEncode(wopiUrl), accessToken);
		}

		private static string GenerateWopiSrcUrl(string owaTemplateUrl, string emailAddress)
		{
			string arg = string.Format(owaTemplateUrl, HttpUtility.UrlEncode(emailAddress));
			return string.Format("{0}?{1}={2}", arg, "owaatt", "Exch_WopiTest");
		}

		private static void CheckExchangeWopiEndpoint(string wopiUrl, StringBuilder diagnosticsDetails)
		{
			HttpWebResponse httpWebResponse = null;
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(wopiUrl);
				httpWebRequest.Accept = "text/html, application/xhtml+xml, */*";
				httpWebRequest.KeepAlive = true;
				httpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)";
				httpWebRequest.Headers.Add("Accept-Language", "en-US");
				httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				string text = null;
				using (Stream responseStream = httpWebResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						text = streamReader.ReadToEnd();
					}
				}
				diagnosticsDetails.AppendLine(string.Format("CheckFileInfo request-id: {0}.", httpWebResponse.Headers.Get("request-id")));
				diagnosticsDetails.AppendLine(string.Format("CheckFileInfo X-CalculatedBETarget: {0}.", httpWebResponse.Headers.Get("X-CalculatedBETarget")));
				diagnosticsDetails.AppendLine(string.Format("CheckFileInfo X-FEServer: {0}.", httpWebResponse.Headers.Get("X-FEServer")));
				diagnosticsDetails.AppendLine(string.Format("CheckFileInfo X-WOPI-ServerError: {0}.", httpWebResponse.Headers.Get("X-WOPI-ServerError")));
				diagnosticsDetails.AppendLine(string.Format("CheckFileInfo Request's StatusDescription: {0}.", httpWebResponse.StatusDescription));
				diagnosticsDetails.AppendLine(string.Format("CheckFileInfo Request's StatusCode: {0}.", (int)httpWebResponse.StatusCode));
				if (text.Contains("DummyAttachment"))
				{
					diagnosticsDetails.AppendLine(string.Format("Probe Error: {0}", WacWorker.knownIssuesForWacProbe[WacConnectivityErrors.WacExchangePipelineIssue]));
					diagnosticsDetails.AppendLine("Diagnostic Tip: Please use the Wac's X-CorrelationId and X-OfficeFE server while reaching out to Wac911 for logs on their end.");
				}
				else
				{
					diagnosticsDetails.AppendLine(string.Format("Probe Error: {0}", WacWorker.knownIssuesForWacProbe[WacConnectivityErrors.WopiEndpointIssue]));
					diagnosticsDetails.AppendLine("Diagnostic Tip: Please use the CheckFileInfo's StatusCode, Description, request-id, X-CalculatedBETarget, X-FEServer and X-WOPI-ServerError details for further digging into Exchange's server");
				}
			}
			catch (WebException ex)
			{
				diagnosticsDetails.AppendLine(string.Format("CheckFileInfo Request Url: {0}.", wopiUrl));
				diagnosticsDetails.AppendLine("Stack Trace:");
				diagnosticsDetails.AppendLine(string.Format("Exception: {0}.", ex.ToString()));
				for (Exception innerException = ex.InnerException; innerException != null; innerException = innerException.InnerException)
				{
					diagnosticsDetails.AppendLine(string.Format("Inner Exception: {0}.", innerException.ToString()));
				}
				diagnosticsDetails.AppendLine("Diagnostic Tip: There is an unhandled exception occured while hitting CheckFileInfo endpoint.");
				diagnosticsDetails.AppendLine("Diagnostic Tip: Please look into the exception details.");
				diagnosticsDetails.AppendLine("Diagnostic Tip: Also You can try hitting the Wopi Request Url from your browser");
			}
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

		private const string AttachmentName = "DummyAttachment";

		private const string WacFileIdParameter = "owaatt";

		private const string WacFileRep = "Exch_WopiTest";

		private const int DefaultTimeout = 120000;

		private const int MaxRedirects = 10;

		private static Dictionary<WacConnectivityErrors, string> knownIssuesForWacProbe = new Dictionary<WacConnectivityErrors, string>(4);

		private static DateTime latencyMeasurementStart;

		private static readonly ManualResetEvent allDone = new ManualResetEvent(false);
	}
}
