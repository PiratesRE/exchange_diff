using System;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public abstract class WebSession
	{
		protected WebSession(Uri loginUrl, NetworkCredential credentials)
		{
			if (null == loginUrl)
			{
				throw new ArgumentNullException("loginUrl");
			}
			if (credentials == null)
			{
				throw new ArgumentNullException("credentials");
			}
			if (!loginUrl.IsAbsoluteUri)
			{
				throw new ArgumentOutOfRangeException("loginUrl");
			}
			this.ServiceAuthority = loginUrl;
			this.Credentials = credentials;
			this.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0)";
			this.SessionCookies = new WebSessionCookieContainer(this);
		}

		public NetworkCredential Credentials { get; private set; }

		public Uri ServiceAuthority { get; protected set; }

		public string UserAgent { get; set; }

		protected CookieContainer SessionCookies { get; set; }

		public bool TrustAnySSLCertificate { get; set; }

		public void AddCookie(Cookie cookie)
		{
			lock (this.SessionCookies)
			{
				this.SessionCookies.Add(this.ServiceAuthority, cookie);
			}
		}

		public CookieCollection GetCookies(Uri uri)
		{
			CookieCollection cookies;
			lock (this.SessionCookies)
			{
				cookies = this.SessionCookies.GetCookies(uri);
			}
			return cookies;
		}

		public abstract void Initialize();

		protected abstract void Authenticate(HttpWebRequest request);

		public T Get<T>(Uri requestUri, Func<HttpWebResponse, T> processResponse)
		{
			HttpWebRequest request = this.CreateRequest(requestUri, "GET");
			return this.Send<T>(request, null, processResponse);
		}

		public void Get<T>(Uri requestUri, Func<HttpWebResponse, T> processResponse, Action<T> onSuccess, Action<Exception> onFailure)
		{
			HttpWebRequest request = this.CreateRequest(requestUri, "GET");
			this.Send<T>(request, null, processResponse, onSuccess, onFailure);
		}

		public T Post<T>(Uri requestUri, RequestBody requestBody, Func<HttpWebResponse, T> processResponse)
		{
			HttpWebRequest request = this.CreateRequest(requestUri, "POST");
			return this.Send<T>(request, requestBody, processResponse);
		}

		public void Post<T>(Uri requestUri, RequestBody requestBody, Func<HttpWebResponse, T> processResponse, Action<T> onSuccess, Action<Exception> onFailure)
		{
			HttpWebRequest request = this.CreateRequest(requestUri, "POST");
			this.Send<T>(request, requestBody, processResponse, onSuccess, onFailure);
		}

		protected HttpWebRequest CreateRequest(Uri requestUri, string method)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUri);
			httpWebRequest.Method = method;
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.UserAgent = this.UserAgent;
			httpWebRequest.Accept = "*/*";
			httpWebRequest.CachePolicy = WebSession.DefaultCachePolicy;
			return httpWebRequest;
		}

		public T Send<T>(HttpWebRequest request, RequestBody requestBody, Func<HttpWebResponse, T> processResponse)
		{
			return this.EndSend<T>(this.BeginSend(request, requestBody, null, null), processResponse);
		}

		public void Send<T>(HttpWebRequest request, RequestBody requestBody, Func<HttpWebResponse, T> processResponse, Action<T> onSuccess, Action<Exception> onFailure)
		{
			if (onSuccess == null)
			{
				throw new ArgumentNullException("onSuccess");
			}
			if (onFailure == null)
			{
				throw new ArgumentNullException("onFailure");
			}
			this.BeginSend(request, requestBody, delegate(IAsyncResult asyncResults)
			{
				T obj;
				try
				{
					obj = this.EndSend<T>(asyncResults, processResponse);
				}
				catch (Exception obj2)
				{
					onFailure(obj2);
					return;
				}
				onSuccess(obj);
			}, null);
		}

		public IAsyncResult BeginSend(HttpWebRequest request, RequestBody requestBody, AsyncCallback callback, object asyncState)
		{
			if (request == null)
			{
				throw new ArgumentNullException("request");
			}
			this.OnSendingRequest(request);
			this.Authenticate(request);
			this.SetupCertificateValidation(request);
			return new SendRequestOperation(request, requestBody, callback, asyncState);
		}

		public T EndSend<T>(IAsyncResult results, Func<HttpWebResponse, T> processResponse)
		{
			if (processResponse == null)
			{
				throw new ArgumentNullException("processResponse");
			}
			T result;
			using (HttpWebResponse response = this.GetResponse(results))
			{
				result = processResponse(response);
			}
			return result;
		}

		private HttpWebResponse GetResponse(IAsyncResult results)
		{
			SendRequestOperation sendRequestOperation = (SendRequestOperation)results;
			sendRequestOperation.AsyncWaitHandle.WaitOne();
			if (sendRequestOperation.Response != null)
			{
				lock (this.SessionCookies)
				{
					foreach (object obj in sendRequestOperation.Response.Cookies)
					{
						Cookie cookie = (Cookie)obj;
						try
						{
							cookie.Domain = cookie.Domain.TrimStart(new char[]
							{
								'.'
							});
							this.SessionCookies.Add(cookie);
						}
						catch (CookieException)
						{
						}
						try
						{
							cookie.Domain = '.' + cookie.Domain;
							this.SessionCookies.Add(cookie);
						}
						catch (CookieException)
						{
						}
					}
				}
			}
			this.UpdatePerformanceCounters(new RequestPerformance(sendRequestOperation));
			if (sendRequestOperation.Exception != null)
			{
				if (sendRequestOperation.Exception is WebException)
				{
					this.OnRequestException(sendRequestOperation.Request, sendRequestOperation.Exception as WebException);
				}
				throw sendRequestOperation.Exception;
			}
			this.OnResponseReceived(sendRequestOperation.Request, sendRequestOperation.Response);
			return sendRequestOperation.Response;
		}

		protected internal void SetupCertificateValidation(HttpWebRequest request)
		{
			lock (this.SessionCookies)
			{
				WebSessionCookieContainer webSessionCookieContainer = new WebSessionCookieContainer(this);
				if (request.CookieContainer != null)
				{
					webSessionCookieContainer.Add(request.CookieContainer.GetCookies(request.RequestUri));
				}
				webSessionCookieContainer.Add(this.SessionCookies.GetCookies(request.RequestUri));
				request.CookieContainer = webSessionCookieContainer;
			}
			CertificateValidationManager.SetComponentId(request, "WebSession");
		}

		public event EventHandler<HttpWebRequestEventArgs> SendingRequest;

		protected virtual void OnSendingRequest(HttpWebRequest request)
		{
			EventHandler<HttpWebRequestEventArgs> sendingRequest = this.SendingRequest;
			if (sendingRequest != null)
			{
				sendingRequest(this, new HttpWebRequestEventArgs(request));
			}
		}

		public event EventHandler<HttpWebResponseEventArgs> ResponseReceived;

		protected virtual void OnResponseReceived(HttpWebRequest request, HttpWebResponse response)
		{
			EventHandler<HttpWebResponseEventArgs> responseReceived = this.ResponseReceived;
			if (responseReceived != null)
			{
				responseReceived(this, new HttpWebResponseEventArgs(request, response));
			}
		}

		public event EventHandler<WebExceptionEventArgs> RequestException;

		protected virtual void OnRequestException(HttpWebRequest request, WebException exception)
		{
			EventHandler<WebExceptionEventArgs> requestException = this.RequestException;
			if (requestException != null)
			{
				requestException(this, new WebExceptionEventArgs(request, exception));
			}
		}

		protected virtual void UpdatePerformanceCounters(RequestPerformance requestPerformance)
		{
		}

		static WebSession()
		{
			CertificateValidationManager.RegisterCallback("WebSession", new RemoteCertificateValidationCallback(WebSession.ServerCertificateValidator));
		}

		private static WebSession FromRequest(HttpWebRequest request)
		{
			WebSessionCookieContainer webSessionCookieContainer = (WebSessionCookieContainer)request.CookieContainer;
			return webSessionCookieContainer.WebSession;
		}

		private static bool ServerCertificateValidator(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			WebSession webSession = WebSession.FromRequest((HttpWebRequest)sender);
			return webSession.TrustAnySSLCertificate || sslPolicyErrors == SslPolicyErrors.None;
		}

		public static HttpWebResponse GetResponseFromException(Exception exception)
		{
			WebException ex = exception as WebException;
			if (ex != null)
			{
				return ex.Response as HttpWebResponse;
			}
			return null;
		}

		private const string CertificateValidationComponentId = "WebSession";

		private const string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 7.0)";

		private const string DefaultAccept = "*/*";

		private static readonly RequestCachePolicy DefaultCachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
	}
}
