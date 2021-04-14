using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class RequestAdapter : IRequestAdapter
	{
		public RequestAdapter() : this(CancellationToken.None, null)
		{
		}

		public RequestAdapter(CancellationToken cancellationToken, Dictionary<string, List<NamedVip>> staticMapping)
		{
			if (cancellationToken.CanBeCanceled)
			{
				cancellationToken.Register(new Action(this.CancellationRequested));
			}
			this.nameResolver = new NameResolver(staticMapping);
		}

		public TimeSpan RequestTimeout
		{
			get
			{
				return this.requestTimeout;
			}
			set
			{
				this.requestTimeout = value;
			}
		}

		public string ConnectionGroupName
		{
			get
			{
				return this.connectionGroupName;
			}
		}

		public IResponseTracker ResponseTracker
		{
			get
			{
				return this.responseTracker;
			}
			set
			{
				this.responseTracker = value;
			}
		}

		public IAsyncResult BeginGetResponse(HttpWebRequestWrapper requestWrapper, ExCookieContainer cookieContainer, SslValidationOptions sslValidationOptions, AuthenticationData? authenticationData, int retryCount, AsyncCallback callback, Dictionary<string, object> asyncState)
		{
			return this.BeginGetResponse(requestWrapper, cookieContainer, sslValidationOptions, authenticationData, retryCount, callback, asyncState, null);
		}

		private IAsyncResult BeginGetResponse(HttpWebRequestWrapper requestWrapper, ExCookieContainer cookieContainer, SslValidationOptions sslValidationOptions, AuthenticationData? authenticationData, int retryCount, AsyncCallback callback, Dictionary<string, object> asyncState, LazyAsyncResult asyncResult)
		{
			if (asyncState == null)
			{
				asyncState = new Dictionary<string, object>();
			}
			if (asyncResult == null)
			{
				asyncResult = new LazyAsyncResult(callback, asyncState);
			}
			try
			{
				if (this.cancellationRequested)
				{
					throw new TaskCanceledException(MonitoringWebClientStrings.ScenarioCanceled);
				}
				HttpWebRequest httpWebRequest = this.CreateHttpWebRequest(requestWrapper, cookieContainer, authenticationData, sslValidationOptions);
				asyncState["RequestWrapper"] = requestWrapper;
				asyncState["HttpWebRequest"] = httpWebRequest;
				asyncState["CookieContainer"] = cookieContainer;
				asyncState["SslValidationOptions"] = sslValidationOptions;
				asyncState["AuthenticationData"] = authenticationData;
				asyncState["AsyncCallback"] = callback;
				asyncState["MaxRetryCount"] = retryCount;
				asyncState["GetResponseAsyncResult"] = asyncResult;
				TimeSpan timeSpan = this.RequestTimeout;
				object obj;
				if (asyncState.TryGetValue("RequestTimeout", out obj))
				{
					timeSpan = ((obj is TimeSpan) ? ((TimeSpan)obj) : timeSpan);
				}
				RequestState requestState = new RequestState(new TimerCallback(this.RequestTimeoutCallback), asyncState, (int)timeSpan.TotalMilliseconds, false);
				asyncState["RequestState"] = requestState;
				requestState.StartTimer();
				lock (this.requestStates)
				{
					this.requestStates.Add(requestState);
				}
				requestWrapper.SentTime = ExDateTime.Now;
				if (requestWrapper.RequestBody != null)
				{
					ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Getting request stream for: {0}", requestWrapper.RequestUri);
					httpWebRequest.BeginGetRequestStream(new AsyncCallback(this.GetRequestStreamCompleted), asyncState);
				}
				else
				{
					ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Sending request: {0}", requestWrapper.RequestUri);
					httpWebRequest.BeginGetResponse(new AsyncCallback(this.GetResponseCompleted), asyncState);
				}
			}
			catch (Exception exception)
			{
				asyncResult.Complete(null, exception);
			}
			return asyncResult;
		}

		public HttpWebResponseWrapper EndGetResponse(IAsyncResult result)
		{
			LazyAsyncResult lazyAsyncResult = result as LazyAsyncResult;
			if (lazyAsyncResult.Exception != null)
			{
				throw lazyAsyncResult.Exception as Exception;
			}
			return lazyAsyncResult.ResultObject as HttpWebResponseWrapper;
		}

		public void CloseConnections()
		{
			foreach (ServicePoint servicePoint in this.accessedServicePoints.Keys)
			{
				servicePoint.CloseConnectionGroup(this.connectionGroupName);
			}
			this.connectionGroupName = Guid.NewGuid().ToString();
			this.accessedServicePoints = new ConcurrentDictionary<ServicePoint, ServicePoint>();
		}

		private void CancellationRequested()
		{
			ExTraceGlobals.MonitoringWebClientTracer.TraceDebug((long)this.GetHashCode(), "Cancellation of the probe run has been requested");
			this.cancellationRequested = true;
			lock (this.requestStates)
			{
				try
				{
					foreach (RequestState requestState in this.requestStates)
					{
						requestState.Cancel();
					}
				}
				catch (InvalidOperationException arg)
				{
					ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<InvalidOperationException>((long)this.GetHashCode(), "InvalidOperationException was thrown while iterating through the request states object. The exception will be ignored. Exception: {0}", arg);
				}
			}
		}

		private void RequestTimeoutCallback(object asyncObj)
		{
			Dictionary<string, object> dictionary = asyncObj as Dictionary<string, object>;
			HttpWebRequest httpWebRequest = dictionary["HttpWebRequest"] as HttpWebRequest;
			RequestState requestState = dictionary["RequestState"] as RequestState;
			ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<int>((long)this.GetHashCode(), "Request timed out. Request state: {0}", requestState.State);
			if (requestState.TryTransitionFromExecutingToTimedOut())
			{
				lock (this.requestStates)
				{
					if (this.requestStates.Contains(requestState))
					{
						this.requestStates.Remove(requestState);
					}
				}
				ExTraceGlobals.MonitoringWebClientTracer.TraceDebug((long)this.GetHashCode(), "Calling Request.Abort");
				httpWebRequest.Abort();
			}
		}

		private void GetRequestStreamCompleted(IAsyncResult asyncResult)
		{
			Dictionary<string, object> dictionary = asyncResult.AsyncState as Dictionary<string, object>;
			HttpWebRequest httpWebRequest = dictionary["HttpWebRequest"] as HttpWebRequest;
			HttpWebRequestWrapper httpWebRequestWrapper = dictionary["RequestWrapper"] as HttpWebRequestWrapper;
			LazyAsyncResult lazyAsyncResult = dictionary["GetResponseAsyncResult"] as LazyAsyncResult;
			ExCookieContainer cookieContainer = dictionary["CookieContainer"] as ExCookieContainer;
			SslValidationOptions sslValidationOptions = (SslValidationOptions)dictionary["SslValidationOptions"];
			AuthenticationData? authenticationData = dictionary["AuthenticationData"] as AuthenticationData?;
			int num = (int)dictionary["MaxRetryCount"];
			AsyncCallback callback = dictionary["AsyncCallback"] as AsyncCallback;
			RequestState requestState = dictionary["RequestState"] as RequestState;
			ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "GetRequestStreamCompleted callback called for {0}", httpWebRequestWrapper.RequestUri);
			HttpWebResponse httpWebResponse = null;
			Exception ex = null;
			try
			{
				using (Stream stream = httpWebRequest.EndGetRequestStream(asyncResult))
				{
					httpWebRequestWrapper.RequestBody.Write(stream);
				}
			}
			catch (WebException ex2)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceError<WebException>((long)this.GetHashCode(), "WebException thrown: {0}", ex2);
				WebExceptionStatus status = ex2.Status;
				if (status == WebExceptionStatus.RequestCanceled && requestState.IsCancelled)
				{
					ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Request timed out: {0}", httpWebRequestWrapper.RequestUri);
				}
				HttpWebResponseWrapper response = null;
				httpWebResponse = (HttpWebResponse)ex2.Response;
				if (httpWebResponse != null)
				{
					response = HttpWebResponseWrapper.Create(httpWebRequestWrapper, httpWebResponse);
				}
				HttpWebResponseWrapperException ex3 = new HttpWebResponseWrapperException(ex2.Message, httpWebRequestWrapper, response, status);
				ex = ex3;
			}
			catch (Exception ex4)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception thrown: {0}", ex4);
				ex = ex4;
			}
			finally
			{
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
			}
			if (ex == null)
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Sending request: {0}", httpWebRequestWrapper.RequestUri);
				httpWebRequest.BeginGetResponse(new AsyncCallback(this.GetResponseCompleted), dictionary);
				return;
			}
			if (this.ShouldRetry(ex, httpWebRequestWrapper, dictionary, num))
			{
				if (this.responseTracker != null)
				{
					this.responseTracker.TrackFailedTcpConnection(httpWebRequestWrapper, ex);
				}
				this.BeginGetResponse(httpWebRequestWrapper, cookieContainer, sslValidationOptions, authenticationData, num, callback, dictionary, lazyAsyncResult);
				return;
			}
			lazyAsyncResult.Complete(null, ex);
		}

		private void GetResponseCompleted(IAsyncResult asyncResult)
		{
			Dictionary<string, object> dictionary = asyncResult.AsyncState as Dictionary<string, object>;
			RequestState requestState = dictionary["RequestState"] as RequestState;
			requestState.TryTransitionFromExecutingToFinished();
			lock (this.requestStates)
			{
				if (this.requestStates.Contains(requestState))
				{
					this.requestStates.Remove(requestState);
				}
			}
			HttpWebRequest httpWebRequest = dictionary["HttpWebRequest"] as HttpWebRequest;
			HttpWebRequestWrapper httpWebRequestWrapper = dictionary["RequestWrapper"] as HttpWebRequestWrapper;
			LazyAsyncResult lazyAsyncResult = dictionary["GetResponseAsyncResult"] as LazyAsyncResult;
			ExCookieContainer exCookieContainer = dictionary["CookieContainer"] as ExCookieContainer;
			SslValidationOptions sslValidationOptions = (SslValidationOptions)dictionary["SslValidationOptions"];
			AuthenticationData? authenticationData = dictionary["AuthenticationData"] as AuthenticationData?;
			int num = (int)dictionary["MaxRetryCount"];
			AsyncCallback callback = dictionary["AsyncCallback"] as AsyncCallback;
			try
			{
				ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "GetResponseCompleted callback called for {0}", httpWebRequestWrapper.RequestUri);
				HttpWebResponseWrapper httpWebResponseWrapper = null;
				HttpWebResponse httpWebResponse = null;
				Exception ex = null;
				try
				{
					httpWebResponse = (httpWebRequest.EndGetResponse(asyncResult) as HttpWebResponse);
					httpWebResponseWrapper = HttpWebResponseWrapper.Create(httpWebRequestWrapper, httpWebResponse);
				}
				catch (WebException ex2)
				{
					ExTraceGlobals.MonitoringWebClientTracer.TraceError<WebException>((long)this.GetHashCode(), "WebException thrown: {0}", ex2);
					WebExceptionStatus status = ex2.Status;
					if (status == WebExceptionStatus.RequestCanceled && requestState.IsCancelled)
					{
						ExTraceGlobals.MonitoringWebClientTracer.TraceDebug<Uri>((long)this.GetHashCode(), "Request timed out: {0}", httpWebRequestWrapper.RequestUri);
					}
					httpWebResponseWrapper = null;
					httpWebResponse = (HttpWebResponse)ex2.Response;
					if (httpWebResponse != null)
					{
						httpWebResponseWrapper = HttpWebResponseWrapper.Create(httpWebRequestWrapper, httpWebResponse);
					}
					else
					{
						HttpWebResponseWrapperException ex3 = new HttpWebResponseWrapperException(ex2.Message, httpWebRequestWrapper, httpWebResponseWrapper, status);
						ex = ex3;
					}
				}
				catch (Exception ex4)
				{
					ExTraceGlobals.MonitoringWebClientTracer.TraceError<Exception>((long)this.GetHashCode(), "Exception thrown: {0}", ex4);
					ex = ex4;
				}
				finally
				{
					if (httpWebResponse != null)
					{
						exCookieContainer.StoreCookies(httpWebResponse);
						httpWebResponse.Close();
					}
				}
				if (ex != null)
				{
					if (this.ShouldRetry(ex, httpWebRequestWrapper, dictionary, num))
					{
						if (this.responseTracker != null)
						{
							this.responseTracker.TrackFailedTcpConnection(httpWebRequestWrapper, ex);
						}
						this.BeginGetResponse(httpWebRequestWrapper, exCookieContainer, sslValidationOptions, authenticationData, num, callback, dictionary, lazyAsyncResult);
					}
					else
					{
						lazyAsyncResult.Complete(null, ex);
					}
				}
				else if (this.ShouldRetryBasedOnStatusCode(httpWebResponseWrapper, dictionary))
				{
					this.BeginGetResponse(httpWebRequestWrapper, exCookieContainer, sslValidationOptions, authenticationData, num, callback, dictionary, lazyAsyncResult);
				}
				else
				{
					lazyAsyncResult.Complete(httpWebResponseWrapper, null);
				}
			}
			catch (Exception exception)
			{
				lazyAsyncResult.Complete(null, exception);
			}
		}

		private bool ShouldRetry(Exception exception, HttpWebRequestWrapper request, Dictionary<string, object> asyncState, int maxRetryCount)
		{
			HttpWebResponseWrapperException httpWebResponseWrapperException = exception as HttpWebResponseWrapperException;
			return this.ShouldRetryWithDnsRoundRobin(exception, request) || this.ShouldRetryBasedOnRequestType(asyncState, maxRetryCount) || this.ShouldRetryBasedOnError(httpWebResponseWrapperException, request, asyncState);
		}

		private bool ShouldRetryBasedOnRequestType(Dictionary<string, object> asyncState, int maxRetryCount)
		{
			return RequestAdapter.IncrementRetryCount(asyncState, maxRetryCount);
		}

		private bool ShouldRetryBasedOnError(HttpWebResponseWrapperException httpWebResponseWrapperException, HttpWebRequestWrapper request, Dictionary<string, object> asyncState)
		{
			return httpWebResponseWrapperException != null && httpWebResponseWrapperException.Status != null && RequestAdapter.RetriableErrors.ContainsKey(httpWebResponseWrapperException.Status.Value) && RequestAdapter.IncrementRetryCount(asyncState, RequestAdapter.RetriableErrors[httpWebResponseWrapperException.Status.Value]);
		}

		private bool ShouldRetryBasedOnStatusCode(HttpWebResponseWrapper response, Dictionary<string, object> asyncState)
		{
			return response != null && RequestAdapter.RetriableStatusCodes.ContainsKey(response.StatusCode) && RequestAdapter.IncrementRetryCount(asyncState, RequestAdapter.RetriableStatusCodes[response.StatusCode]);
		}

		private bool ShouldRetryWithDnsRoundRobin(Exception exception, HttpWebRequestWrapper request)
		{
			if (!(exception is SocketException))
			{
				HttpWebResponseWrapperException ex = exception as HttpWebResponseWrapperException;
				if (ex == null || ex.Status == null || !BaseExceptionAnalyzer.IsNetworkRelatedError(ex.Status.Value))
				{
					return false;
				}
			}
			return this.nameResolver.ShouldRetryWithDnsRoundRobin(request.RequestUri.Host);
		}

		private static bool IncrementRetryCount(Dictionary<string, object> asyncState, int maxRetryCount)
		{
			int num = 0;
			if (asyncState.ContainsKey("CurrentRetryCount"))
			{
				num = (int)asyncState["CurrentRetryCount"];
			}
			if (num < maxRetryCount)
			{
				asyncState["CurrentRetryCount"] = num + 1;
				return true;
			}
			return false;
		}

		private HttpWebRequest CreateHttpWebRequest(HttpWebRequestWrapper requestWrapper, ExCookieContainer cookieContainer, AuthenticationData? authenticationData, SslValidationOptions sslValidationOptions)
		{
			try
			{
				requestWrapper.RequestIpAddressUri = this.nameResolver.ResolveUri(requestWrapper);
			}
			finally
			{
				if (this.responseTracker != null)
				{
					this.responseTracker.TrackResolvedRequest(requestWrapper);
				}
			}
			requestWrapper.Headers["Cookie"] = cookieContainer.GetCookieHeader(requestWrapper.RequestUri);
			HttpWebRequest httpWebRequest = requestWrapper.CreateHttpWebRequest();
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.CachePolicy = RequestAdapter.DefaultCachePolicy;
			httpWebRequest.KeepAlive = true;
			if (authenticationData != null)
			{
				httpWebRequest.UseDefaultCredentials = authenticationData.Value.UseDefaultCredentials;
				if (!httpWebRequest.UseDefaultCredentials)
				{
					httpWebRequest.Credentials = authenticationData.Value.Credentials;
				}
				httpWebRequest.PreAuthenticate = true;
				httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
			}
			SslValidationHelper.SetComponentId(sslValidationOptions, httpWebRequest);
			httpWebRequest.ConnectionGroupName = this.connectionGroupName;
			requestWrapper.ConnectionGroupName = this.connectionGroupName;
			ServicePoint servicePoint = ServicePointManager.FindServicePoint(httpWebRequest.RequestUri, httpWebRequest.Proxy);
			if (this.accessedServicePoints.TryAdd(servicePoint, servicePoint) && servicePoint.ConnectionLimit < 500)
			{
				servicePoint.ConnectionLimit = 500;
			}
			return httpWebRequest;
		}

		private const int ConnectionLimit = 500;

		internal const string RequestTimeoutParameterName = "RequestTimeout";

		private static readonly RequestCachePolicy DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

		private static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(30.0);

		private static readonly Dictionary<WebExceptionStatus, int> RetriableErrors = BaseExceptionAnalyzer.GetRetriableErrors();

		private static readonly Dictionary<HttpStatusCode, int> RetriableStatusCodes = BaseExceptionAnalyzer.GetStatusCodes();

		private TimeSpan requestTimeout = RequestAdapter.DefaultRequestTimeout;

		private IResponseTracker responseTracker;

		private bool cancellationRequested;

		private string connectionGroupName = Guid.NewGuid().ToString();

		private ConcurrentDictionary<ServicePoint, ServicePoint> accessedServicePoints = new ConcurrentDictionary<ServicePoint, ServicePoint>();

		private List<RequestState> requestStates = new List<RequestState>();

		private NameResolver nameResolver;
	}
}
