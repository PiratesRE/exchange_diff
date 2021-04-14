using System;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public abstract class HttpProbe : ProbeWorkItem
	{
		public bool DisallowProxy { get; set; }

		protected HttpWebRequestUtility WebRequest { get; set; }

		protected string MsppErrorCode { get; set; }

		protected Task<WebResponse> StartGetLogonPage(string startUrl)
		{
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "HttpProbe.StartGetLogonPage: Sending request to: {0}", startUrl, null, "StartGetLogonPage", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 122);
			base.Result.FailureContext = startUrl;
			ProbeResult result = base.Result;
			result.ExecutionContext += string.Format("SendLogonGetRequestTo:{0}\r\n", startUrl);
			return this.WebRequest.SendGetRequest(startUrl, true, this.DisallowProxy);
		}

		protected Task<WebResponse> StartLiveIdLogin(string body)
		{
			this.wctx = this.WebRequest.GetDataBetweenTokens(body, "&wctx=", "&mkt=EN-US");
			string text = this.WebRequest.GetDataBetweenTokens(body, "name=\"PPFT\" id=\"i0327\" value=\"", "\"/>");
			text = this.WebRequest.UrlEncode(text);
			string text2 = this.WebRequest.ParseJavascriptVariable(body, "srf_uPost");
			if (text2 == null)
			{
				string text3 = string.Format("javascript variable {0} not found in the response body.", "srf_uPost");
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.StartLiveIdLogin]: {0}", text3, null, "StartLiveIdLogin", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 151);
				throw new FormatException(text3);
			}
			string format = "login={0}&passwd={1}&PPFT={2}";
			string arg = HttpUtility.UrlEncode(base.Definition.Account);
			string arg2 = HttpUtility.UrlEncode(base.Definition.AccountPassword);
			string postBody = string.Format(format, arg, arg2, text);
			WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.StartLiveIdLogin]: Log in as {0} to {1}", arg, text2, null, "StartLiveIdLogin", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 160);
			base.Result.FailureContext = text2;
			return this.WebRequest.SendPostRequest(text2, postBody, false, this.DisallowProxy);
		}

		protected Task<WebResponse> StartLocalLogin()
		{
			string format = "destination={0}&flags=0&forcedownlevel=0&isUtf8=1&username={1}&trusted=0&password={2}";
			string arg = HttpUtility.UrlEncode("https://localhost/owa/");
			string arg2 = HttpUtility.UrlEncode(base.Definition.Account);
			string arg3 = HttpUtility.UrlEncode(base.Definition.AccountPassword);
			string postBody = string.Format(format, arg, arg2, arg3);
			base.Result.FailureContext = base.Definition.Endpoint;
			return this.WebRequest.SendPostRequest(base.Definition.Endpoint, postBody);
		}

		protected Task<WebResponse> StartMicrosoftOnlineLogin(string body)
		{
			string dataBetweenTokens = this.WebRequest.GetDataBetweenTokens(body, "id=\"NAP\" value=\"", "\">");
			string text = this.WebRequest.GetDataBetweenTokens(body, "id=\"ANON\" value=\"", "\">");
			text = HttpUtility.UrlEncode(text);
			string text2 = this.WebRequest.GetDataBetweenTokens(body, "id=\"wresult\" value=\"", "\">");
			text2 = this.WebRequest.UrlDecodeQuotes(text2);
			text2 = HttpUtility.UrlEncode(text2);
			string format = "wctx={0}&NAP={1}&wresult={2}&wa=wsignin1.0&ANON={3}";
			string postBody = string.Format(format, new object[]
			{
				this.wctx,
				dataBetweenTokens,
				text2,
				text
			});
			string dataBetweenTokens2 = this.WebRequest.GetDataBetweenTokens(body, "id=\"fmHF\" action=\"", "\" method=");
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.StartMicrosoftOnlineLogin]: Sending request to {0}", dataBetweenTokens2, null, "StartMicrosoftOnlineLogin", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 210);
			base.Result.FailureContext = dataBetweenTokens2;
			return this.WebRequest.SendPostRequest(dataBetweenTokens2, postBody, true, this.DisallowProxy);
		}

		protected string GetLocalResponseBody(Task<WebResponse> task)
		{
			HttpWebResponse httpResponse = this.WebRequest.GetHttpResponse(task);
			base.Result.FailureContext = httpResponse.ResponseUri.ToString();
			foreach (object obj in httpResponse.Headers.Keys)
			{
				string text = (string)obj;
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetLocalResponseBody]: Header: {0}= \t{1}", text, httpResponse.Headers[text], null, "GetLocalResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 233);
			}
			string httpResponseBody = this.WebRequest.GetHttpResponseBody(httpResponse);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetLocalResponseBody]: Response Body String:\r\n{0}", httpResponseBody, null, "GetLocalResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 237);
			string text2 = httpResponse.Headers["X-DiagInfo"];
			if (!string.IsNullOrEmpty(text2))
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("X-DiagInfo:{0}\r\n", text2);
			}
			string text3 = this.WebRequest.ParseJavascriptVariable(httpResponseBody, "srf_sErr");
			if (text3 != null)
			{
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetLocalResponseBody]: {0}", text3, null, "GetLocalResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 251);
				throw new WebException(text3);
			}
			string value = httpResponse.Headers["Set-Cookie"];
			if (string.IsNullOrWhiteSpace(value))
			{
				WTFDiagnostics.TraceError(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetLocalResponseBody]: No cookie is set. Login failed", null, "GetLocalResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 259);
				throw new WebException("Login failed");
			}
			return httpResponseBody;
		}

		protected string GetResponseBody(Task<WebResponse> task)
		{
			HttpWebResponse httpResponse = this.WebRequest.GetHttpResponse(task);
			base.Result.FailureContext = httpResponse.ResponseUri.ToString();
			WTFDiagnostics.TraceInformation<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetResponseBody]: Response Status: {0}", httpResponse.StatusCode.ToString(), null, "GetResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 280);
			foreach (object obj in httpResponse.Headers.Keys)
			{
				string text = (string)obj;
				WTFDiagnostics.TraceInformation<string, string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetResponseBody]: Header: {0}= \t{1}", text, httpResponse.Headers[text], null, "GetResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 285);
			}
			string text2 = httpResponse.Headers["X-DiagInfo"];
			if (!string.IsNullOrEmpty(text2))
			{
				ProbeResult result = base.Result;
				result.ExecutionContext += string.Format("X-DiagInfo:{0}\r\n", text2);
			}
			string httpResponseBody = this.WebRequest.GetHttpResponseBody(httpResponse);
			WTFDiagnostics.TraceDebug<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetResponseBody]: Response Body String:\r\n{0}", string.IsNullOrEmpty(httpResponseBody) ? "[Empty Body]" : httpResponseBody, null, "GetResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 297);
			string text3 = this.WebRequest.ParseJavascriptVariable(httpResponseBody, "srf_sErr");
			if (text3 != null)
			{
				string text4 = string.Format("Response from URL {0} contains JS variable srf_sErr with error: {1}", httpResponse.ResponseUri.ToString(), text3);
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetResponseBody]: Error from srf_sErr: {0}", text4, null, "GetResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 304);
				throw new WebException(text4);
			}
			if (HttpStatusCode.OK == httpResponse.StatusCode || HttpStatusCode.Found == httpResponse.StatusCode)
			{
				return httpResponseBody;
			}
			string text5 = string.Format("{0}:{1}", httpResponse.StatusCode, httpResponseBody);
			WTFDiagnostics.TraceError<string>(ExTraceGlobals.HTTPTracer, base.TraceContext, "[HttpProbe.GetResponseBody]: {0}", text5, null, "GetResponseBody", "f:\\15.00.1497\\sources\\dev\\monitoring\\src\\ActiveMonitoring\\Components\\Common\\Http\\HttpProbe.cs", 313);
			if (task.Exception != null)
			{
				throw new WebException(text5, task.Exception.Flatten().InnerException);
			}
			throw new WebException(text5);
		}

		protected const string NextPostUrlOpeningTag = "id=\"fmHF\" action=\"";

		protected const string NextPostUrlClosingTag = "\" method=";

		protected const string PpftOpeningTag = "name=\"PPFT\" id=\"i0327\" value=\"";

		protected const string PpftClosingTag = "\"/>";

		private const string WctxOpeningTag = "&wctx=";

		private const string WctxClosingTag = "&mkt=EN-US";

		private const string NapOpeningTag = "id=\"NAP\" value=\"";

		private const string NapClosingTag = "\">";

		private const string WresultOpeningTag = "id=\"wresult\" value=\"";

		private const string WresultClosingTag = "\">";

		private const string AnonOpeningTag = "id=\"ANON\" value=\"";

		private const string AnonClosingTag = "\">";

		private string wctx;
	}
}
