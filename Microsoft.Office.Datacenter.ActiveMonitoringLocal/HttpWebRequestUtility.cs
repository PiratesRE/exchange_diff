using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.ActiveMonitoring;
using Microsoft.Office.Datacenter.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class HttpWebRequestUtility
	{
		public HttpWebRequestUtility(TracingContext traceContext)
		{
			this.traceContext = traceContext;
		}

		public CookieCollection LastResponseCookies { get; private set; }

		public TimeSpan Timeout
		{
			get
			{
				if (this.timeoutValue == -1)
				{
					return TimeSpan.Zero;
				}
				return TimeSpan.FromMilliseconds((double)this.timeoutValue);
			}
			set
			{
				this.timeoutValue = Convert.ToInt32(Math.Round(value.TotalMilliseconds, 0));
			}
		}

		public string UserAgent { get; set; }

		public HttpWebRequest CreateCommonHttpWebRequest(string url, bool allowRedirect, bool disallowProxy = false)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Accept = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, */*";
			httpWebRequest.KeepAlive = true;
			httpWebRequest.UserAgent = (this.UserAgent ?? "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Active Monitoring; MSEXCHMON 14.0)");
			httpWebRequest.AllowAutoRedirect = allowRedirect;
			httpWebRequest.Headers.Add("Accept-Language", "en-us");
			httpWebRequest.CookieContainer = this.cookies;
			string cookieHeader = httpWebRequest.CookieContainer.GetCookieHeader(httpWebRequest.RequestUri);
			httpWebRequest.Headers.Set("Cookie", cookieHeader);
			if (disallowProxy)
			{
				httpWebRequest.Proxy = null;
			}
			if (this.timeoutValue != -1)
			{
				httpWebRequest.Timeout = this.timeoutValue;
			}
			return httpWebRequest;
		}

		public HttpWebRequest CreateBasicHttpWebRequest(string url, bool allowRedirect)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.AllowAutoRedirect = allowRedirect;
			return httpWebRequest;
		}

		public Task<WebResponse> SendRequest(HttpWebRequest request)
		{
			WTFDiagnostics.TraceDebug<Uri, WebHeaderCollection>(ExTraceGlobals.HTTPTracer, this.traceContext, "HttpWebRequestUtility.SendRequest: url={0}, headers={1}", request.Address, request.Headers, null, "SendRequest", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\Http\\HttpWebRequestUtility.cs", 176);
			WTFDiagnostics.FaultInjectionTraceTest<HttpWebRequest>(FaultInjectionLid.HttpWebRequestUtility_SendRequest, ref request);
			return Task.Factory.FromAsync<WebResponse>(new Func<AsyncCallback, object, IAsyncResult>(request.BeginGetResponse), new Func<IAsyncResult, WebResponse>(request.EndGetResponse), null, TaskCreationOptions.AttachedToParent);
		}

		public Task<WebResponse> SendGetRequest(string url, bool allowRedirect, bool disallowProxy)
		{
			HttpWebRequest httpWebRequest = this.CreateCommonHttpWebRequest(url, allowRedirect, disallowProxy);
			httpWebRequest.Method = "GET";
			return this.SendRequest(httpWebRequest);
		}

		public Task<WebResponse> SendGetRequest(string url, bool allowRedirect, string userName, string password, bool useBasicAuthentication = true)
		{
			HttpWebRequest request = this.PrepareRequest(url, allowRedirect, userName, password, useBasicAuthentication);
			return this.SendRequest(request);
		}

		public HttpWebRequest PrepareRequest(string url, bool allowRedirect, string userName, string password, bool useBasicAuthentication)
		{
			HttpWebRequest httpWebRequest = this.CreateCommonHttpWebRequest(url, allowRedirect, false);
			httpWebRequest.Method = "GET";
			if (useBasicAuthentication)
			{
				string str = Convert.ToBase64String(Encoding.Default.GetBytes(userName + ":" + password));
				httpWebRequest.Headers.Add("Authorization", "Basic " + str);
			}
			else
			{
				httpWebRequest.Credentials = new NetworkCredential(userName, password);
			}
			return httpWebRequest;
		}

		public Task WriteBody(HttpWebRequest request, string body)
		{
			Task<Stream> task = Task.Factory.FromAsync<Stream>(new Func<AsyncCallback, object, IAsyncResult>(request.BeginGetRequestStream), new Func<IAsyncResult, Stream>(request.EndGetRequestStream), null, TaskCreationOptions.AttachedToParent);
			return task.ContinueWith(delegate(Task<Stream> s)
			{
				using (StreamWriter streamWriter = new StreamWriter(s.Result))
				{
					streamWriter.Write(body);
				}
			}, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		public Task WriteBody(HttpWebRequest request, MemoryStream body)
		{
			Task<Stream> task = Task.Factory.FromAsync<Stream>(new Func<AsyncCallback, object, IAsyncResult>(request.BeginGetRequestStream), new Func<IAsyncResult, Stream>(request.EndGetRequestStream), null, TaskCreationOptions.AttachedToParent);
			return task.ContinueWith(delegate(Task<Stream> s)
			{
				body.Seek(0L, SeekOrigin.Begin);
				body.CopyTo(s.Result, 11);
			}, TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled);
		}

		public Task<WebResponse> SendPostRequest(string url, string postBody, bool allowRedirect, bool disallowProxy)
		{
			if (!postBody.ToLower().Contains("password"))
			{
				WTFDiagnostics.TraceDebug<string, string>(ExTraceGlobals.HTTPTracer, this.traceContext, "HttpWebRequestUtility.SendPostRequest: Sending request with URL={0}, body={1}", url, postBody, null, "SendPostRequest", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\Http\\HttpWebRequestUtility.cs", 298);
			}
			HttpWebRequest request = this.CreateCommonHttpWebRequest(url, allowRedirect, disallowProxy);
			request.Method = "POST";
			request.ContentType = "application/x-www-form-urlencoded";
			return this.WriteBody(request, postBody).ContinueWith<Task<WebResponse>>((Task t) => this.SendRequest(request), TaskContinuationOptions.AttachedToParent | TaskContinuationOptions.NotOnFaulted | TaskContinuationOptions.NotOnCanceled).Unwrap<WebResponse>();
		}

		public Task<WebResponse> SendPostRequest(string url, string postBody)
		{
			return this.SendPostRequest(url, postBody, false, false);
		}

		public HttpWebResponse GetHttpResponse(Task<WebResponse> task)
		{
			HttpWebResponse httpWebResponse = null;
			try
			{
				httpWebResponse = (HttpWebResponse)task.Result;
				WTFDiagnostics.FaultInjectionTraceTest<HttpWebResponse>(FaultInjectionLid.HttpWebRequestUtility_GetHttpResponse, ref httpWebResponse);
			}
			catch (AggregateException ex)
			{
				WebException ex2 = ex.Flatten().InnerException as WebException;
				if (ex2 == null || ex2.Response == null)
				{
					WTFDiagnostics.TraceError<WebException>(ExTraceGlobals.HTTPTracer, this.traceContext, "HttpWebRequestUtility.GetHttpResponse: exception: {0}", ex2, null, "GetHttpResponse", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\Http\\HttpWebRequestUtility.cs", 354);
					throw;
				}
				httpWebResponse = (HttpWebResponse)ex2.Response;
			}
			this.cookies.Add(httpWebResponse.Cookies);
			this.LastResponseCookies = httpWebResponse.Cookies;
			WTFDiagnostics.TraceDebug<Uri, WebHeaderCollection>(ExTraceGlobals.HTTPTracer, this.traceContext, "HttpWebRequestUtility.GetHttpResponse: response from url={0} has headers={1}", httpWebResponse.ResponseUri, httpWebResponse.Headers, null, "GetHttpResponse", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\Http\\HttpWebRequestUtility.cs", 365);
			return httpWebResponse;
		}

		public string GetHttpResponseBody(HttpWebResponse response)
		{
			string text = null;
			Stream responseStream = response.GetResponseStream();
			using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
			{
				text = streamReader.ReadToEnd();
			}
			WTFDiagnostics.TraceDebug<Uri, string>(ExTraceGlobals.HTTPTracer, this.traceContext, "HttpWebRequestUtility.GetHttpResponseBody: response from url={0} has body={1}", response.ResponseUri, text, null, "GetHttpResponseBody", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\Http\\HttpWebRequestUtility.cs", 385);
			return text;
		}

		public string GetDataBetweenTokens(string body, string openingToken, string closingToken)
		{
			int num = body.IndexOf(openingToken);
			if (-1 == num)
			{
				string text = string.Format("Tag or token {0} not found in the response body.", openingToken);
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.HTTPTracer, this.traceContext, "HttpWebRequestUtility.GetDataBetweenTokens: {0}", text, null, "GetDataBetweenTokens", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\Http\\HttpWebRequestUtility.cs", 402);
				throw new FormatException(text);
			}
			num += openingToken.Length;
			int num2 = body.IndexOf(closingToken, num);
			if (-1 == num2)
			{
				string text2 = string.Format("Tag or token {0} not found in the response body.", closingToken);
				WTFDiagnostics.TraceError<string>(ExTraceGlobals.HTTPTracer, this.traceContext, "HttpWebRequestUtility.GetDataBetweenTokens: {0}", text2, null, "GetDataBetweenTokens", "f:\\15.00.1497\\sources\\dev\\common\\src\\WorkerTaskFramework\\ActiveMonitoring\\Helpers\\Http\\HttpWebRequestUtility.cs", 411);
				throw new FormatException(text2);
			}
			return body.Substring(num, num2 - num);
		}

		public string UrlEncode(string toEncode)
		{
			string text = HttpUtility.UrlEncode(toEncode);
			return text.Replace("!", "%21");
		}

		public string UrlDecodeQuotes(string toDecode)
		{
			return toDecode.Replace("&quot;", "\"");
		}

		public string ParseJavascriptVariable(string body, string variableName)
		{
			Regex regex = new Regex(string.Format("{0}\\s*=\\s*((\"(?<VALUE>[^\"]*)\"\\s*;)|(?<VALUE>[^;\"]*);)", Regex.Escape(variableName)));
			Match match = regex.Match(body);
			if (!match.Success)
			{
				return null;
			}
			char[] trimChars = new char[]
			{
				'\''
			};
			string text = match.Result("${VALUE}");
			return text.Trim(trimChars);
		}

		public string ParseJsonField(string body, string name)
		{
			if (string.IsNullOrEmpty(body))
			{
				return null;
			}
			Regex regex = new Regex(string.Format("{0}\\s*:\\s*['\"](?<value>[^'\"]*)['\"]", Regex.Escape(name)));
			Match match = regex.Match(body);
			if (!match.Success)
			{
				return null;
			}
			return match.Result("${value}");
		}

		public string ParseFormAction(string body)
		{
			if (string.IsNullOrEmpty(body))
			{
				return null;
			}
			string pattern = "action[\\s]*=(?:(?:\\s*(?<value>[^'\"\\s]+)\\s)|[\\s]*[\"](?<value>[^\"]*)[\"]|[\\s]*['](?<value>[^']*)['])";
			Regex regex = new Regex(pattern, RegexOptions.Compiled);
			Match match = regex.Match(body);
			if (!match.Success)
			{
				return null;
			}
			return match.Result("${value}");
		}

		public const string CertificateValidationComponentId = "DefaultAMComponent";

		private const string DefaultAcceptHeader = "image/gif, image/x-xbitmap, image/jpeg, image/pjpeg, */*";

		private const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; Active Monitoring; MSEXCHMON 14.0)";

		private const string DefaultAcceptLanguage = "en-us";

		private CookieContainer cookies = new CookieContainer();

		private TracingContext traceContext;

		private int timeoutValue = -1;
	}
}
