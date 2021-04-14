using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Forefront.Reporting.Security;

namespace Microsoft.Forefront.Monitoring.ActiveMonitoring
{
	public class HttpRequest : IHttpRequest
	{
		public HttpRequest()
		{
			this.userAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Active Monitoring)";
			this.acceptLanguage = "en-us";
			this.cookies = new CookieContainer();
		}

		public string AcceptLanguage
		{
			get
			{
				return this.acceptLanguage;
			}
			set
			{
				this.acceptLanguage = value;
			}
		}

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

		public ProcessCookies ProcessCookies
		{
			get
			{
				return this.processCookies;
			}
			set
			{
				this.processCookies = value;
			}
		}

		public ServerResponse SendGetRequest(string uri, bool sslValidation, string componentId, bool allowRedirects = true, int timeout = 0, string authenticationType = null, string authenticationUser = null, string authenticationPassword = null, Dictionary<string, string> properties = null)
		{
			HttpWebRequest httpWebRequest = this.CreateRequest(uri, allowRedirects, timeout);
			httpWebRequest.Method = "GET";
			if (!sslValidation)
			{
				CertificateValidationManager.SetComponentId(httpWebRequest, componentId);
			}
			AuthenticatorSettings settings = new AuthenticatorSettings(authenticationUser, authenticationPassword, properties);
			HttpRequest.AuthenticateHttpWebRequest(authenticationType, httpWebRequest, settings);
			return this.GetServerResponse(httpWebRequest);
		}

		public ServerResponse SendPostRequest(string uri, bool allowRedirects, bool getHiddenInputValues, ref PostData postData, string contentType, string formName = null, int timeout = 0)
		{
			if (getHiddenInputValues)
			{
				ServerResponse serverResponse = this.SendGetRequest(uri, true, string.Empty, allowRedirects, timeout, null, null, null, null);
				string tagInnerHtml = HtmlUtility.GetTagInnerHtml("form", serverResponse.Text, formName);
				if (!string.IsNullOrEmpty(tagInnerHtml))
				{
					Dictionary<string, string> hiddenFormInputs = HtmlUtility.GetHiddenFormInputs(tagInnerHtml);
					if (hiddenFormInputs.Count > 0)
					{
						if (postData == null)
						{
							postData = new PostData(hiddenFormInputs);
						}
						else
						{
							postData.AddRange(hiddenFormInputs);
						}
					}
				}
				if (allowRedirects)
				{
					uri = serverResponse.Uri.AbsoluteUri;
				}
			}
			HttpWebRequest httpWebRequest = this.CreateRequest(uri, allowRedirects, timeout);
			httpWebRequest.Method = "POST";
			httpWebRequest.ContentType = contentType;
			if (postData != null && postData.Count > 0)
			{
				using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
				{
					string value = postData.ToString(contentType.Equals("application/x-www-form-urlencoded", StringComparison.InvariantCultureIgnoreCase));
					streamWriter.Write(value);
					streamWriter.Close();
					goto IL_E4;
				}
			}
			httpWebRequest.ContentLength = 0L;
			IL_E4:
			return this.GetServerResponse(httpWebRequest);
		}

		private static void AuthenticateHttpWebRequest(string authenticationType, HttpWebRequest request, AuthenticatorSettings settings)
		{
			if (!string.IsNullOrEmpty(authenticationType))
			{
				Type type = Type.GetType(authenticationType);
				if (null == type)
				{
					throw new ArgumentException("Invalid HttpAuthenticationType is specified");
				}
				IAuthenticator authenticator = (IAuthenticator)Activator.CreateInstance(type);
				authenticator.PrepareRequest(request, settings);
			}
		}

		private static HttpWebResponse GetHttpWebResponse(HttpWebRequest request)
		{
			HttpWebResponse httpWebResponse;
			try
			{
				httpWebResponse = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException ex)
			{
				httpWebResponse = (ex.Response as HttpWebResponse);
				if (httpWebResponse == null)
				{
					throw;
				}
			}
			return httpWebResponse;
		}

		private HttpWebRequest CreateRequest(string uri, bool allowRedirects, int timeout)
		{
			HttpWebRequest httpWebRequest = WebRequest.Create(uri) as HttpWebRequest;
			httpWebRequest.CookieContainer = this.cookies;
			httpWebRequest.UserAgent = this.userAgent;
			httpWebRequest.AllowAutoRedirect = allowRedirects;
			httpWebRequest.Headers.Add("Accept-Language", this.acceptLanguage);
			httpWebRequest.KeepAlive = true;
			if (timeout > 0)
			{
				httpWebRequest.Timeout = timeout;
			}
			return httpWebRequest;
		}

		private void PersistCookies(CookieContainer cookieContainer, Uri uri)
		{
			switch (this.processCookies)
			{
			case ProcessCookies.None:
				this.cookies = new CookieContainer();
				return;
			case ProcessCookies.SessionOnly:
			{
				this.cookies = new CookieContainer();
				if (cookieContainer.Count <= 0)
				{
					return;
				}
				CookieCollection cookieCollection = cookieContainer.GetCookies(uri);
				using (IEnumerator enumerator = cookieCollection.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						Cookie cookie = (Cookie)obj;
						if (cookie.Expires < DateTime.UtcNow)
						{
							this.cookies.Add(cookie);
						}
					}
					return;
				}
				break;
			}
			}
			this.cookies = cookieContainer;
		}

		private ServerResponse GetServerResponse(HttpWebRequest request)
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			ServerResponse result;
			using (HttpWebResponse httpWebResponse = HttpRequest.GetHttpWebResponse(request))
			{
				this.PersistCookies(request.CookieContainer, request.Address);
				string text = string.Empty;
				using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
				{
					text = streamReader.ReadToEnd();
				}
				result = new ServerResponse(httpWebResponse.ResponseUri, httpWebResponse.StatusCode, httpWebResponse.ContentType, stopwatch.Elapsed, text, httpWebResponse.Headers.ToString());
			}
			return result;
		}

		private string userAgent;

		private string acceptLanguage;

		private ProcessCookies processCookies;

		private CookieContainer cookies;
	}
}
