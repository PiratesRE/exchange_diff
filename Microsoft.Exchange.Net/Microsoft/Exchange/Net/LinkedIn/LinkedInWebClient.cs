using System;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.LinkedIn
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class LinkedInWebClient : ILinkedInWebClient
	{
		public LinkedInWebClient(LinkedInAppConfig config, ITracer tracer)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("tracer", tracer);
			this.appConfig = config;
			this.oauth = new LinkedInOAuth(tracer);
		}

		public void SubscribeDownloadCompletedEvent(EventHandler<DownloadCompleteEventArgs> eventHandler)
		{
			this.downloadCompleted = (EventHandler<DownloadCompleteEventArgs>)Delegate.Remove(this.downloadCompleted, eventHandler);
			this.downloadCompleted = (EventHandler<DownloadCompleteEventArgs>)Delegate.Combine(this.downloadCompleted, eventHandler);
		}

		public void Abort(IAsyncResult result)
		{
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)result;
			LinkedInWebClient.GetOperationState getOperationState = (LinkedInWebClient.GetOperationState)lazyAsyncResult.AsyncObject;
			if (getOperationState.HttpWebRequest != null)
			{
				getOperationState.HttpWebRequest.Abort();
			}
		}

		public LinkedInResponse AuthenticateApplication(string url, string authenticationHeader, TimeSpan requestTimeout, IWebProxy proxy)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = "application/x-www-form-urlencoded";
			httpWebRequest.ContentLength = 0L;
			httpWebRequest.Timeout = (int)requestTimeout.TotalMilliseconds;
			httpWebRequest.Headers["Authorization"] = authenticationHeader;
			if (proxy != null)
			{
				httpWebRequest.Proxy = proxy;
			}
			LinkedInResponse result;
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					string body = streamReader.ReadToEnd();
					result = new LinkedInResponse
					{
						Code = httpWebResponse.StatusCode,
						Body = body
					};
				}
			}
			return result;
		}

		public LinkedInPerson GetProfile(string accessToken, string accessTokenSecret, string fields)
		{
			if (string.IsNullOrEmpty(accessToken))
			{
				throw new ArgumentNullException("accessToken");
			}
			if (string.IsNullOrEmpty(accessTokenSecret))
			{
				throw new ArgumentNullException("accessTokenSecret");
			}
			if (string.IsNullOrEmpty(fields))
			{
				throw new ArgumentNullException("fields");
			}
			string url = string.Format("{0}:({1})", this.appConfig.ProfileEndpoint, fields);
			string authorizationHeader = this.oauth.GetAuthorizationHeader(url, "GET", null, accessToken, accessTokenSecret, this.appConfig.AppId, this.appConfig.AppSecret);
			return (LinkedInPerson)this.GetOperation(url, authorizationHeader, this.appConfig.WebRequestTimeout, this.appConfig.WebProxy, new Func<HttpWebResponse, object>(this.GetObjectsFromResponse<LinkedInPerson>));
		}

		public HttpStatusCode RemoveApplicationPermissions(string accessToken, string accessSecret)
		{
			string authorizationHeader = this.oauth.GetAuthorizationHeader(this.appConfig.RemoveAppEndpoint, "GET", null, accessToken, accessSecret, this.appConfig.AppId, this.appConfig.AppSecret);
			return (HttpStatusCode)this.GetOperation(this.appConfig.RemoveAppEndpoint, authorizationHeader, this.appConfig.WebRequestTimeout, this.appConfig.WebProxy, (HttpWebResponse httpWebResponse) => httpWebResponse.StatusCode);
		}

		public IAsyncResult BeginGetLinkedInConnections(string url, string authorizationHeader, TimeSpan requestTimeout, IWebProxy proxy, AsyncCallback callback, object callbackState)
		{
			return this.BeginGetOperation(url, authorizationHeader, requestTimeout, proxy, callback, callbackState, new Func<HttpWebResponse, object>(this.GetObjectsFromResponse<LinkedInConnections>));
		}

		public LinkedInConnections EndGetLinkedInConnections(IAsyncResult ar)
		{
			return (LinkedInConnections)this.EndGetOperation(ar);
		}

		private object GetOperation(string url, string authorizationHeader, TimeSpan requestTimeout, IWebProxy proxy, Func<HttpWebResponse, object> responseProcessor)
		{
			IAsyncResult ar = this.BeginGetOperation(url, authorizationHeader, requestTimeout, proxy, null, null, responseProcessor);
			return this.EndGetOperation(ar);
		}

		private IAsyncResult BeginGetOperation(string url, string authorizationHeader, TimeSpan requestTimeout, IWebProxy proxy, AsyncCallback callback, object callbackState, Func<HttpWebResponse, object> responseProcessor)
		{
			HttpWebRequest httpWebRequest = LinkedInWebClient.GetHttpWebRequest(url, authorizationHeader, requestTimeout, proxy);
			LinkedInWebClient.GetOperationState worker = new LinkedInWebClient.GetOperationState(httpWebRequest, responseProcessor);
			LazyAsyncResult lazyAsyncResult = new LazyAsyncResult(worker, callbackState, callback);
			httpWebRequest.BeginGetResponse(new AsyncCallback(this.OnGetOperationsCompleted), lazyAsyncResult);
			return lazyAsyncResult;
		}

		private void OnGetOperationsCompleted(IAsyncResult asyncResult)
		{
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)asyncResult.AsyncState;
			LinkedInWebClient.GetOperationState getOperationState = (LinkedInWebClient.GetOperationState)lazyAsyncResult.AsyncObject;
			object value = null;
			try
			{
				using (HttpWebResponse httpWebResponse = (HttpWebResponse)getOperationState.HttpWebRequest.EndGetResponse(asyncResult))
				{
					Func<HttpWebResponse, object> responseProcessor = getOperationState.ResponseProcessor;
					if (responseProcessor != null)
					{
						value = responseProcessor(httpWebResponse);
					}
				}
			}
			catch (Exception ex)
			{
				value = ex;
			}
			lazyAsyncResult.InvokeCallback(value);
		}

		private object EndGetOperation(IAsyncResult ar)
		{
			LazyAsyncResult lazyAsyncResult = (LazyAsyncResult)ar;
			lazyAsyncResult.InternalWaitForCompletion();
			if (lazyAsyncResult.EndCalled)
			{
				throw new InvalidOperationException("EndGetOperation is called more than once for the same IAsyncResult object.");
			}
			lazyAsyncResult.EndCalled = true;
			Exception ex = lazyAsyncResult.Result as Exception;
			if (ex != null)
			{
				throw ex;
			}
			return lazyAsyncResult.Result;
		}

		private T GetObjectsFromResponse<T>(HttpWebResponse httpWebResponse) where T : class
		{
			DataContractJsonSerializer dataContractJsonSerializer = new DataContractJsonSerializer(typeof(T));
			T result;
			using (Stream responseStream = httpWebResponse.GetResponseStream())
			{
				if (this.downloadCompleted != null && httpWebResponse.ContentLength > 0L)
				{
					this.downloadCompleted(this, new DownloadCompleteEventArgs(httpWebResponse.ContentLength));
				}
				result = (dataContractJsonSerializer.ReadObject(responseStream) as T);
			}
			return result;
		}

		private static HttpWebRequest GetHttpWebRequest(string url, string authorizationHeader, TimeSpan requestTimeout, IWebProxy proxy)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			httpWebRequest.Timeout = (int)requestTimeout.TotalMilliseconds;
			httpWebRequest.Headers["x-li-format"] = "json";
			httpWebRequest.Headers["Authorization"] = authorizationHeader;
			if (proxy != null)
			{
				httpWebRequest.Proxy = proxy;
			}
			return httpWebRequest;
		}

		private EventHandler<DownloadCompleteEventArgs> downloadCompleted;

		private readonly LinkedInAppConfig appConfig;

		private readonly LinkedInOAuth oauth;

		private class GetOperationState
		{
			public GetOperationState(HttpWebRequest request, Func<HttpWebResponse, object> responseProcessor)
			{
				this.HttpWebRequest = request;
				this.ResponseProcessor = responseProcessor;
			}

			public HttpWebRequest HttpWebRequest { get; private set; }

			public Func<HttpWebResponse, object> ResponseProcessor { get; private set; }
		}
	}
}
