using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class HttpSession : IHttpSession
	{
		static HttpSession()
		{
			Array values = Enum.GetValues(typeof(HttpStatusCode));
			HttpSession.AllHttpStatusCodes = new HttpStatusCode[values.Length];
			int num = 0;
			foreach (object obj in values)
			{
				HttpSession.AllHttpStatusCodes[num] = (HttpStatusCode)obj;
				num++;
			}
		}

		public HttpSession(IRequestAdapter requestAdapter, IExceptionAnalyzer exceptionAnalyzer, IResponseTracker responseTracker)
		{
			if (requestAdapter == null)
			{
				throw new ArgumentNullException("requestAdapter");
			}
			if (exceptionAnalyzer == null)
			{
				throw new ArgumentNullException("exceptionAnalyzer");
			}
			this.requestAdapter = requestAdapter;
			this.exceptionAnalyzer = exceptionAnalyzer;
			this.responseTracker = responseTracker;
			this.requestAdapter.ResponseTracker = this.responseTracker;
			this.retryCountMapping[RequestTarget.LiveIdConsumer] = 2;
			this.retryCountMapping[RequestTarget.LiveIdBusiness] = 2;
			this.retryCountMapping[RequestTarget.Hotmail] = 2;
			this.retryCountMapping[RequestTarget.Akamai] = 2;
			this.dynamicHeaders.Add(new DynamicHeader("x-owa-canary", delegate(Uri uri)
			{
				Cookie cookie = this.cookieContainer.GetCookies(uri)["X-OWA-CANARY"];
				if (cookie != null)
				{
					return cookie.Value;
				}
				return null;
			}));
		}

		public event EventHandler<HttpWebEventArgs> SendingRequest;

		public event EventHandler<HttpWebEventArgs> ResponseReceived;

		public event EventHandler<TestEventArgs> TestStarted;

		public event EventHandler<TestEventArgs> TestFinished;

		public string UserAgent
		{
			get
			{
				return this.userAgent;
			}
			set
			{
				this.userAgent = value;
			}
		}

		public object EventState { get; set; }

		public IResponseTracker ResponseTracker
		{
			get
			{
				return this.responseTracker;
			}
		}

		public Dictionary<RequestTarget, int> RetryCountMapping
		{
			get
			{
				return this.retryCountMapping;
			}
		}

		public ExCookieContainer CookieContainer
		{
			get
			{
				return this.cookieContainer;
			}
			set
			{
				this.cookieContainer = value;
			}
		}

		public Dictionary<string, string> PersistentHeaders
		{
			get
			{
				return this.persistentHeaders;
			}
		}

		public AuthenticationData? AuthenticationData { get; set; }

		public SslValidationOptions SslValidationOptions
		{
			get
			{
				return this.sslValidationOptions;
			}
			set
			{
				this.sslValidationOptions = value;
			}
		}

		public void NotifyTestStarted(TestId testId)
		{
			lock (this.testStack)
			{
				this.testStack.Push(testId);
			}
			this.OnTestStarted(testId);
		}

		public void NotifyTestFinished(TestId testId)
		{
			lock (this.testStack)
			{
				if (this.testStack.Count > 0)
				{
					this.testStack.Pop();
				}
			}
			this.OnTestFinished(testId);
		}

		public IAsyncResult BeginGet(TestId stepId, string uri, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			Uri requestUri = new Uri(uri);
			HttpWebRequestWrapper request = HttpWebRequestWrapper.CreateRequest(stepId, requestUri, "GET", null, this.cookieContainer, this.persistentHeaders, this.userAgent, this.dynamicHeaders);
			return this.BeginSend(request, callback, asyncState);
		}

		public T EndGet<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse)
		{
			return this.EndSend<T>(result, HttpSession.OkStatusCode, processResponse, true);
		}

		public T EndGet<T>(IAsyncResult result, HttpStatusCode[] expectedStatusCodes, Func<HttpWebResponseWrapper, T> processResponse)
		{
			return this.EndSend<T>(result, expectedStatusCodes, processResponse, true);
		}

		public IAsyncResult BeginPost(TestId stepId, string uri, RequestBody body, string contentType, Dictionary<string, string> headers, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			Uri requestUri = new Uri(uri);
			HttpWebRequestWrapper request = this.BuildPostRequest(stepId, requestUri, body, contentType, headers);
			return this.BeginSend(request, callback, asyncState);
		}

		private HttpWebRequestWrapper BuildPostRequest(TestId stepId, Uri requestUri, RequestBody body, string contentType, Dictionary<string, string> headers)
		{
			HttpWebRequestWrapper httpWebRequestWrapper = HttpWebRequestWrapper.CreateRequest(stepId, requestUri, "POST", body, this.cookieContainer, this.persistentHeaders, this.userAgent, this.dynamicHeaders);
			httpWebRequestWrapper.Headers.Add("Content-Type", contentType);
			if (headers != null)
			{
				foreach (string text in headers.Keys)
				{
					httpWebRequestWrapper.Headers.Add(text, headers[text]);
				}
			}
			return httpWebRequestWrapper;
		}

		public IAsyncResult BeginPost(TestId stepId, string uri, RequestBody body, string contentType, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			return this.BeginPost(stepId, uri, body, contentType, null, callback, asyncState);
		}

		public T EndPost<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse)
		{
			return this.EndPost<T>(result, HttpSession.OkOrRedirectionStatusCode, processResponse);
		}

		public T EndPost<T>(IAsyncResult result, HttpStatusCode[] expectedStatusCodes, Func<HttpWebResponseWrapper, T> processResponse)
		{
			return this.EndSend<T>(result, expectedStatusCodes, processResponse, true);
		}

		public IAsyncResult BeginGetFollowingRedirections(TestId stepId, string uri, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			return this.BeginGetFollowingRedirections(stepId, uri, RedirectionOptions.FollowUntilNo302, callback, asyncState);
		}

		public IAsyncResult BeginGetFollowingRedirections(TestId stepId, string uri, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			Uri requestUri = new Uri(uri);
			return this.BeginGetFollowingRedirections(stepId, requestUri, redirectionOptions, callback, asyncState);
		}

		public IAsyncResult BeginGetFollowingRedirections(TestId stepId, Uri requestUri, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			if (asyncState == null)
			{
				asyncState = new Dictionary<string, object>();
			}
			HttpWebRequestWrapper request = HttpWebRequestWrapper.CreateRequest(stepId, requestUri, "GET", null, this.cookieContainer, this.persistentHeaders, this.userAgent, this.dynamicHeaders);
			return this.BeginSendFollowingRedirections(requestUri, request, redirectionOptions, callback, asyncState);
		}

		public IAsyncResult BeginPostFollowingRedirections(TestId stepId, string uri, RequestBody body, string contentType, Dictionary<string, string> headers, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			Uri requestUri = new Uri(uri);
			return this.BeginPostFollowingRedirections(stepId, requestUri, body, contentType, headers, redirectionOptions, callback, asyncState);
		}

		public IAsyncResult BeginPostFollowingRedirections(TestId stepId, Uri requestUri, RequestBody body, string contentType, Dictionary<string, string> headers, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			if (asyncState == null)
			{
				asyncState = new Dictionary<string, object>();
			}
			HttpWebRequestWrapper request = this.BuildPostRequest(stepId, requestUri, body, contentType, headers);
			return this.BeginSendFollowingRedirections(requestUri, request, redirectionOptions, callback, asyncState);
		}

		public IAsyncResult BeginSendFollowingRedirections(Uri uri, HttpWebRequestWrapper request, RedirectionOptions redirectionOptions, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			asyncState["RequestWrapper"] = request;
			asyncState["RedirectionCount"] = 0;
			asyncState["RedirectionOptions"] = redirectionOptions;
			asyncState["OriginalUri"] = uri;
			IAsyncResult asyncResult = new LazyAsyncResult(callback, asyncState);
			asyncState["OperationAsyncResult"] = asyncResult;
			this.BeginSend(request, new AsyncCallback(this.RedirectionCallback), asyncState);
			return asyncResult;
		}

		public T EndPostFollowingRedirections<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse)
		{
			return this.EndGetFollowingRedirections<T>(result, processResponse);
		}

		public T EndGetFollowingRedirections<T>(IAsyncResult result, Func<HttpWebResponseWrapper, T> processResponse)
		{
			LazyAsyncResult lazyAsyncResult = result as LazyAsyncResult;
			if (!lazyAsyncResult.IsCompleted)
			{
				lazyAsyncResult.AsyncWaitHandle.WaitOne();
			}
			if (lazyAsyncResult.Exception != null)
			{
				throw lazyAsyncResult.Exception as Exception;
			}
			if (processResponse != null)
			{
				HttpWebResponseWrapper response = lazyAsyncResult.ResultObject as HttpWebResponseWrapper;
				try
				{
					return processResponse(response);
				}
				catch (Exception exception)
				{
					this.exceptionAnalyzer.Analyze(response.Request.StepId, response.Request, response, exception, this.responseTracker, delegate(ScenarioException scenarioException)
					{
						if (this.responseTracker != null)
						{
							this.responseTracker.TrackFailedResponse(response, scenarioException);
						}
					});
					throw;
				}
			}
			return default(T);
		}

		public void CloseConnections()
		{
			this.requestAdapter.CloseConnections();
		}

		public ScenarioException VerifyScenarioExceededRunTime(TimeSpan? maxAllowedTime)
		{
			if (maxAllowedTime == null || this.responseTracker == null)
			{
				return null;
			}
			TimeSpan timeSpan = TimeSpan.FromTicks(this.responseTracker.Items.Sum((ResponseTrackerItem r) => r.TotalLatency.Ticks));
			if (timeSpan <= maxAllowedTime.Value)
			{
				return null;
			}
			long largestLatency = this.responseTracker.Items.Max((ResponseTrackerItem r) => r.TotalLatency.Ticks);
			ResponseTrackerItem responseTrackerItem = (from r in this.responseTracker.Items
			where r.TotalLatency.Ticks == largestLatency
			select r).First<ResponseTrackerItem>();
			responseTrackerItem.FailingServer = responseTrackerItem.RespondingServer;
			ScenarioException exceptionForScenarioTimeout = this.exceptionAnalyzer.GetExceptionForScenarioTimeout(maxAllowedTime.Value, timeSpan, responseTrackerItem);
			this.responseTracker.TrackItemCausingScenarioTimeout(responseTrackerItem, exceptionForScenarioTimeout);
			return exceptionForScenarioTimeout;
		}

		public virtual List<string> GetHostNames(RequestTarget requestTarget)
		{
			return this.exceptionAnalyzer.GetHostNames(requestTarget);
		}

		private void RedirectionCallback(IAsyncResult result)
		{
			Dictionary<string, object> dictionary = result.AsyncState as Dictionary<string, object>;
			HttpStatusCode[] expectedStatusCodes = null;
			int? num = dictionary["RedirectionCount"] as int?;
			ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<int?>((long)this.GetHashCode(), "Redirection callback invoked. Current redir count: {0}", num);
			if (num >= 15)
			{
				expectedStatusCodes = HttpSession.OkStatusCode;
				ExTraceGlobals.MonitoringWebClientTracer.TraceDebug((long)this.GetHashCode(), "Max redirection count reached");
			}
			LazyAsyncResult lazyAsyncResult = dictionary["OperationAsyncResult"] as LazyAsyncResult;
			RedirectionOptions redirectionOptions = (RedirectionOptions)dictionary["RedirectionOptions"];
			Uri uri = dictionary["OriginalUri"] as Uri;
			try
			{
				var <>f__AnonymousType = this.EndSend(result, expectedStatusCodes, (HttpWebResponseWrapper response) => new
				{
					StatusCode = response.StatusCode,
					Location = response.Headers["Location"],
					Response = response
				}, false);
				HttpStatusCode httpStatusCode = <>f__AnonymousType.StatusCode;
				string text = <>f__AnonymousType.Location;
				CasRedirectPage casRedirectPage = null;
				if (<>f__AnonymousType.StatusCode == HttpStatusCode.OK && CasRedirectPage.TryParse(<>f__AnonymousType.Response, out casRedirectPage))
				{
					text = casRedirectPage.TargetUrl;
					httpStatusCode = HttpStatusCode.Found;
				}
				LiveIdRedirectPage liveIdRedirectPage = null;
				if (<>f__AnonymousType.StatusCode == HttpStatusCode.OK && LiveIdRedirectPage.TryParse(<>f__AnonymousType.Response, out liveIdRedirectPage))
				{
					text = liveIdRedirectPage.TargetUrl;
					httpStatusCode = HttpStatusCode.Found;
				}
				if (httpStatusCode == HttpStatusCode.Found || httpStatusCode == HttpStatusCode.MovedPermanently)
				{
					HttpWebRequestWrapper httpWebRequestWrapper = dictionary["RequestWrapper"] as HttpWebRequestWrapper;
					Uri uri2;
					if (Uri.IsWellFormedUriString(text, UriKind.Absolute))
					{
						uri2 = new Uri(text);
					}
					else if (Uri.IsWellFormedUriString(text, UriKind.Relative))
					{
						uri2 = new Uri(httpWebRequestWrapper.RequestUri, text);
					}
					else if (!Uri.TryCreate(text, UriKind.Absolute, out uri2) && !Uri.TryCreate(httpWebRequestWrapper.RequestUri, text, out uri2))
					{
						throw new ArgumentException("Server returned malformed location: " + text);
					}
					if (redirectionOptions == RedirectionOptions.StopOnFirstCrossDomainRedirect)
					{
						if (!uri2.Host.EndsWith(uri.Host, StringComparison.OrdinalIgnoreCase))
						{
							lazyAsyncResult.Complete(<>f__AnonymousType.Response, null);
							return;
						}
					}
					else if (redirectionOptions == RedirectionOptions.FollowUntilNo302ExpectCrossDomainOnFirstRedirect)
					{
						if (num == 0 && uri2.Host.Equals(uri.Host))
						{
							lazyAsyncResult.Complete(<>f__AnonymousType.Response, null);
							return;
						}
					}
					else if (redirectionOptions == RedirectionOptions.FollowUntilNo302OrSpecificRedirection)
					{
						string[] array = dictionary["LastExpectedRedirection"] as string[];
						if (array != null && array.Length > 0)
						{
							foreach (string value in array)
							{
								if (uri2.PathAndQuery.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0)
								{
									lazyAsyncResult.Complete(<>f__AnonymousType.Response, null);
									return;
								}
							}
						}
					}
					ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Following redirection to {0}", uri2);
					this.OnResponseReceived(<>f__AnonymousType.Response.Request, <>f__AnonymousType.Response);
					HttpWebRequestWrapper request = HttpWebRequestWrapper.CreateRequest(httpWebRequestWrapper.StepId, uri2, "GET", null, this.cookieContainer, this.persistentHeaders, this.userAgent, this.dynamicHeaders);
					num++;
					dictionary["RedirectionCount"] = num;
					this.BeginSend(request, new AsyncCallback(this.RedirectionCallback), dictionary);
				}
				else
				{
					lazyAsyncResult.Complete(<>f__AnonymousType.Response, null);
				}
			}
			catch (Exception ex)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception thrown: {0}", ex);
				lazyAsyncResult.Complete(null, ex);
			}
		}

		private IAsyncResult BeginSend(HttpWebRequestWrapper request, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "BeginSend invoked for {0}", request.RequestUri);
			if (asyncState == null)
			{
				asyncState = new Dictionary<string, object>();
			}
			this.OnSendingRequest(request);
			asyncState["RequestWrapper"] = request;
			ResponseTrackerItem responseTrackerItem = null;
			if (this.responseTracker != null)
			{
				RequestTarget requestTarget = this.exceptionAnalyzer.GetRequestTarget(request);
				responseTrackerItem = this.responseTracker.TrackRequest(request.StepId, requestTarget, request);
				asyncState["ResponseTrackerItem"] = responseTrackerItem;
			}
			IAsyncResult result;
			try
			{
				result = this.requestAdapter.BeginGetResponse(request, this.cookieContainer, this.sslValidationOptions, this.AuthenticationData, this.GetRetryCount(request), callback, asyncState);
			}
			catch (Exception ex)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception thrown: {0}", ex);
				LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(callback, asyncState);
				lazyAsyncResult.Complete(null, ex);
				result = lazyAsyncResult;
			}
			finally
			{
				if (responseTrackerItem != null)
				{
					this.responseTracker.TrackSentRequest(responseTrackerItem, request);
				}
			}
			return result;
		}

		private int GetRetryCount(HttpWebRequestWrapper request)
		{
			RequestTarget requestTarget = this.exceptionAnalyzer.GetRequestTarget(request);
			if (this.retryCountMapping.ContainsKey(requestTarget))
			{
				return this.retryCountMapping[requestTarget];
			}
			return 0;
		}

		private T EndSend<T>(IAsyncResult result, HttpStatusCode[] expectedStatusCodes, Func<HttpWebResponseWrapper, T> processResponse, bool fireResponseReceivedEvent = true)
		{
			Exception exception = null;
			Dictionary<string, object> dictionary = result.AsyncState as Dictionary<string, object>;
			HttpWebRequestWrapper httpWebRequestWrapper = dictionary["RequestWrapper"] as HttpWebRequestWrapper;
			ResponseTrackerItem item = dictionary["ResponseTrackerItem"] as ResponseTrackerItem;
			CafeErrorPageValidationRules cafeErrorPageValidationRules = CafeErrorPageValidationRules.None;
			if (dictionary.ContainsKey("CafeErrorPageValidationRules"))
			{
				cafeErrorPageValidationRules = ((dictionary["CafeErrorPageValidationRules"] as CafeErrorPageValidationRules?) ?? CafeErrorPageValidationRules.None);
			}
			ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "EndSend invoked for {0}", httpWebRequestWrapper.RequestUri);
			HttpWebResponseWrapper httpWebResponseWrapper = null;
			try
			{
				httpWebResponseWrapper = this.requestAdapter.EndGetResponse(result);
				if (httpWebResponseWrapper == null)
				{
					throw new ArgumentOutOfRangeException("response shouldn't be null on a successful request");
				}
			}
			catch (HttpWebResponseWrapperException ex)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceError<HttpWebResponseWrapperException>((long)this.GetHashCode(), "HttpWebResponseWrapperException thrown: {0}", ex);
				httpWebResponseWrapper = ex.Response;
				exception = ex;
			}
			catch (Exception ex2)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception thrown: {0}", ex2);
				exception = ex2;
			}
			if (httpWebResponseWrapper != null)
			{
				if (fireResponseReceivedEvent)
				{
					this.OnResponseReceived(httpWebRequestWrapper, httpWebResponseWrapper);
				}
				if (this.responseTracker != null)
				{
					this.responseTracker.TrackResponse(item, httpWebResponseWrapper);
				}
			}
			else if (this.responseTracker != null)
			{
				RequestTarget requestTarget = this.exceptionAnalyzer.GetRequestTarget(httpWebRequestWrapper);
				this.responseTracker.TrackFailedRequest(httpWebRequestWrapper.StepId, requestTarget, httpWebRequestWrapper, exception);
			}
			return this.AnalyzeResponse<T>(httpWebRequestWrapper, httpWebResponseWrapper, exception, expectedStatusCodes, cafeErrorPageValidationRules, processResponse);
		}

		private T AnalyzeResponse<T>(HttpWebRequestWrapper request, HttpWebResponseWrapper response, Exception exception, HttpStatusCode[] expectedStatusCodes, CafeErrorPageValidationRules cafeErrorPageValidationRules, Func<HttpWebResponseWrapper, T> processResponse)
		{
			T result = default(T);
			if (response != null)
			{
				exception = this.exceptionAnalyzer.VerifyResponse(request, response, cafeErrorPageValidationRules);
				if (exception == null && expectedStatusCodes != null)
				{
					if (!Array.Exists<HttpStatusCode>(expectedStatusCodes, (HttpStatusCode statusCode) => statusCode == response.StatusCode))
					{
						ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<HttpStatusCode>((long)this.GetHashCode(), "Unexpected response code received: {0}", response.StatusCode);
						exception = new UnexpectedStatusCodeException(MonitoringWebClientStrings.UnexpectedResponseCodeReceived, request, response, expectedStatusCodes, response.StatusCode);
					}
				}
				if (exception == null)
				{
					try
					{
						if (processResponse != null)
						{
							ExTraceGlobals.MonitoringWebClientTracer.TraceDebug((long)this.GetHashCode(), "Invoking response processing callback");
							result = processResponse(response);
						}
					}
					catch (Exception ex)
					{
						exception = ex;
					}
				}
			}
			if (exception != null)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Exception>((long)this.GetHashCode(), "Invoking exception analyzer for: {0}", exception);
				this.exceptionAnalyzer.Analyze(request.StepId, request, response, exception, this.responseTracker, delegate(ScenarioException scenarioException)
				{
					if (this.responseTracker != null)
					{
						this.responseTracker.TrackFailedResponse(response, scenarioException);
					}
				});
			}
			return result;
		}

		private void OnSendingRequest(HttpWebRequestWrapper request)
		{
			EventHandler<HttpWebEventArgs> sendingRequest = this.SendingRequest;
			if (sendingRequest != null)
			{
				sendingRequest(this, new HttpWebEventArgs(request, this.EventState));
			}
		}

		private void OnResponseReceived(HttpWebRequestWrapper request, HttpWebResponseWrapper response)
		{
			EventHandler<HttpWebEventArgs> responseReceived = this.ResponseReceived;
			if (responseReceived != null)
			{
				responseReceived(this, new HttpWebEventArgs(request, response, this.EventState));
			}
		}

		private void OnTestStarted(TestId testId)
		{
			EventHandler<TestEventArgs> testStarted = this.TestStarted;
			if (testStarted != null)
			{
				testStarted(this, new TestEventArgs(testId, this.EventState));
			}
		}

		private void OnTestFinished(TestId testId)
		{
			EventHandler<TestEventArgs> testFinished = this.TestFinished;
			if (testFinished != null)
			{
				testFinished(this, new TestEventArgs(testId, this.EventState));
			}
		}

		public const string LastExpectedRedirectionParameterName = "LastExpectedRedirection";

		public const string CafeErrorPageValidationRulesParameterName = "CafeErrorPageValidationRules";

		internal static readonly HttpStatusCode[] OkStatusCode = new HttpStatusCode[]
		{
			HttpStatusCode.OK
		};

		internal static readonly HttpStatusCode[] OkOrRedirectionStatusCode = new HttpStatusCode[]
		{
			HttpStatusCode.OK,
			HttpStatusCode.Found,
			HttpStatusCode.MovedPermanently
		};

		internal static readonly HttpStatusCode[] AllHttpStatusCodes;

		private string userAgent = "Mozilla/4.0 (compatible; MSIE 9.0; Windows NT 6.1; MSEXCHMON; MONITORINGWEBCLIENT)";

		private Stack<TestId> testStack = new Stack<TestId>();

		private SslValidationOptions sslValidationOptions = SslValidationOptions.BasicCertificateValidation;

		private readonly Dictionary<string, string> persistentHeaders = new Dictionary<string, string>();

		private ExCookieContainer cookieContainer = new ExCookieContainer();

		private IRequestAdapter requestAdapter;

		private IExceptionAnalyzer exceptionAnalyzer;

		private IResponseTracker responseTracker;

		private Dictionary<RequestTarget, int> retryCountMapping = new Dictionary<RequestTarget, int>();

		private List<DynamicHeader> dynamicHeaders = new List<DynamicHeader>();
	}
}
