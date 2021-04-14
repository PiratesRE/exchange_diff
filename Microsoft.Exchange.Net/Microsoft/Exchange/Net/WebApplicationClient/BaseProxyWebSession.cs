using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Cache;
using System.Net.Security;
using System.Web;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	internal abstract class BaseProxyWebSession : AuthenticateWebSession
	{
		public BaseProxyWebSession(Uri serviceUrl) : base(serviceUrl, CredentialCache.DefaultNetworkCredentials)
		{
		}

		protected HashSet<string> AllowedRequestCookies
		{
			get
			{
				return this.allowedRequestCookies;
			}
		}

		protected HashSet<string> AllowedResponseCookies
		{
			get
			{
				return this.allowedResponseCookies;
			}
		}

		protected HashSet<string> BlockedRequestHeaders
		{
			get
			{
				return this.blockedRequestHeaders;
			}
		}

		protected HashSet<string> BlockedResposeHeaders
		{
			get
			{
				return this.blockedResposeHeaders;
			}
		}

		protected virtual HttpWebRequest CreateProxyRequest(HttpContext context)
		{
			HttpRequest request = context.Request;
			Uri uri = new Uri(base.ServiceAuthority, request.Url.PathAndQuery);
			HttpWebRequest httpWebRequest = base.CreateRequest(uri, request.RequestType);
			for (int i = 0; i < request.Headers.Count; i++)
			{
				string text = request.Headers.Keys[i];
				if (!WebHeaderCollection.IsRestricted(text, false) && !this.blockedRequestHeaders.Contains(text))
				{
					httpWebRequest.Headers[text] = request.Headers[text];
				}
			}
			httpWebRequest.UserAgent = request.UserAgent;
			string text2 = request.Headers["referer"];
			if (text2 != null)
			{
				httpWebRequest.Referer = text2;
			}
			text2 = request.Headers["accept"];
			if (text2 != null)
			{
				httpWebRequest.Accept = text2;
			}
			httpWebRequest.CookieContainer = new CookieContainer();
			foreach (string name in this.AllowedRequestCookies)
			{
				HttpCookie httpCookie = context.Request.Cookies[name];
				if (httpCookie != null)
				{
					Cookie cookie = new Cookie(httpCookie.Name, httpCookie.Value)
					{
						Domain = uri.Host,
						Expires = httpCookie.Expires,
						HttpOnly = httpCookie.HttpOnly,
						Path = httpCookie.Path,
						Secure = httpCookie.Secure
					};
					httpWebRequest.CookieContainer.Add(cookie);
				}
			}
			httpWebRequest.AllowAutoRedirect = false;
			httpWebRequest.AutomaticDecompression = (DecompressionMethods.GZip | DecompressionMethods.Deflate);
			httpWebRequest.AuthenticationLevel = AuthenticationLevel.None;
			httpWebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
			httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;
			httpWebRequest.PreAuthenticate = true;
			return httpWebRequest;
		}

		protected override void Authenticate(HttpWebRequest request)
		{
			request.Credentials = base.Credentials.GetCredential(base.ServiceAuthority, "Kerberos");
		}

		protected virtual RequestBody CreateProxyRequestBody(HttpContext context)
		{
			HttpRequest request = context.Request;
			if (request.ContentLength > 0)
			{
				if (request.InputStream.CanSeek && request.InputStream.Position > 0L)
				{
					request.InputStream.Position = 0L;
				}
				return new StreamBody(request.InputStream, request.ContentType);
			}
			return null;
		}

		public void SendProxyRequest(HttpContext context, Action onSuccess, Action<HttpContext, HttpWebResponse, Exception> onFailure)
		{
			HttpWebRequest proxyRequest = this.CreateProxyRequest(context);
			RequestBody requestBody = this.CreateProxyRequestBody(context);
			this.OnSendingProxyRequest(context, proxyRequest);
			base.Send<BaseProxyWebSession>(proxyRequest, requestBody, delegate(HttpWebResponse proxyResponse)
			{
				this.ProcessProxyResponse(context, proxyResponse);
				return this;
			}, delegate(BaseProxyWebSession responseData)
			{
				this.ReportProxySucceeded(context, proxyRequest, onSuccess);
			}, delegate(Exception exception)
			{
				this.HandleProxyFailure(context, proxyRequest, exception, onSuccess, onFailure);
			});
		}

		private void HandleProxyFailure(HttpContext context, HttpWebRequest proxyRequest, Exception exception, Action onSuccess, Action<HttpContext, HttpWebResponse, Exception> onFailure)
		{
			HttpWebResponse responseFromException = WebSession.GetResponseFromException(exception);
			if (responseFromException != null && this.ShouldProcessFailedResponse((WebException)exception))
			{
				try
				{
					this.ProcessProxyResponse(context, responseFromException);
				}
				catch (Exception exception2)
				{
					this.ReportProxyFailure(context, proxyRequest, responseFromException, exception2, onFailure);
					return;
				}
				this.ReportProxySucceeded(context, proxyRequest, onSuccess);
				return;
			}
			this.ReportProxyFailure(context, proxyRequest, responseFromException, exception, onFailure);
		}

		protected virtual bool ShouldProcessFailedResponse(WebException exception)
		{
			switch (exception.GetTroubleshootingID())
			{
			case WebExceptionTroubleshootingID.TrustFailure:
			case WebExceptionTroubleshootingID.ServiceUnavailable:
				return false;
			default:
				return true;
			}
		}

		private void ReportProxySucceeded(HttpContext context, HttpWebRequest proxyRequest, Action onSuccess)
		{
			try
			{
				this.OnProxyRequestSucceeded(context, proxyRequest);
			}
			finally
			{
				onSuccess();
			}
		}

		private void ReportProxyFailure(HttpContext context, HttpWebRequest proxyRequest, HttpWebResponse response, Exception exception, Action<HttpContext, HttpWebResponse, Exception> onFailure)
		{
			try
			{
				this.OnProxyRequestFailed(context, proxyRequest, response, exception);
			}
			finally
			{
				onFailure(context, response, exception);
			}
		}

		protected abstract void OnSendingProxyRequest(HttpContext context, HttpWebRequest request);

		protected abstract void OnProxyRequestSucceeded(HttpContext context, HttpWebRequest request);

		protected abstract void OnProxyRequestFailed(HttpContext context, HttpWebRequest request, HttpWebResponse response, Exception exception);

		protected virtual void ProcessProxyResponse(HttpContext context, HttpWebResponse proxyResponse)
		{
			HttpResponse response = context.Response;
			response.TrySkipIisCustomErrors = true;
			response.ContentType = proxyResponse.ContentType;
			response.StatusCode = (int)proxyResponse.StatusCode;
			for (int i = 0; i < proxyResponse.Headers.Count; i++)
			{
				string text = proxyResponse.Headers.Keys[i];
				if (!WebHeaderCollection.IsRestricted(text, true) && !this.blockedResposeHeaders.Contains(text))
				{
					response.AddHeader(text, proxyResponse.Headers[text]);
				}
			}
			foreach (string name in this.AllowedResponseCookies)
			{
				Cookie cookie = proxyResponse.Cookies[name];
				if (cookie != null)
				{
					HttpCookie cookie2 = new HttpCookie(cookie.Name, cookie.Value)
					{
						Expires = cookie.Expires,
						HttpOnly = cookie.HttpOnly,
						Path = cookie.Path,
						Secure = cookie.Secure
					};
					context.Response.Cookies.Add(cookie2);
				}
			}
			using (Stream responseStream = proxyResponse.GetResponseStream())
			{
				if (responseStream != null)
				{
					responseStream.CopyTo(response.OutputStream);
				}
			}
		}

		private HashSet<string> allowedRequestCookies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private HashSet<string> allowedResponseCookies = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

		private HashSet<string> blockedRequestHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"accept-encoding",
			"cookie",
			"authorization",
			"proxy-authorization"
		};

		private HashSet<string> blockedResposeHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"set-cookie",
			"server",
			"x-powered-by",
			"x-aspnet-version",
			"www-authenticate"
		};
	}
}
