using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public abstract class WebApplication
	{
		protected WebApplication(string virtualDirectory, WebSession webSession)
		{
			virtualDirectory = (virtualDirectory.EndsWith("/") ? virtualDirectory : (virtualDirectory + '/'));
			Uri baseUri = new Uri(webSession.ServiceAuthority, "/");
			this.BaseUri = new Uri(baseUri, virtualDirectory);
			this.webSession = webSession;
		}

		public Uri BaseUri { get; private set; }

		public Cookie GetCookie(string name)
		{
			return this.GetCookies()[name];
		}

		public CookieCollection GetCookies()
		{
			return this.webSession.GetCookies(this.BaseUri);
		}

		public void Get<T>(string relativeUrl, Action<T> successCallback, Action<Exception> failedCallback) where T : WebApplicationResponse, new()
		{
			Uri requestUri = new Uri(this.BaseUri, relativeUrl);
			this.webSession.Get<T>(requestUri, new Func<HttpWebResponse, T>(WebApplication.GetPage<T>), successCallback, failedCallback);
		}

		public T Get<T>(string relativeUrl) where T : WebApplicationResponse, new()
		{
			Uri requestUri = new Uri(this.BaseUri, relativeUrl);
			return this.webSession.Get<T>(requestUri, new Func<HttpWebResponse, T>(WebApplication.GetPage<T>));
		}

		public void Post<T>(string relativeUrl, RequestBody requestBody, Action<T> successCallback, Action<Exception> failedCallback) where T : WebApplicationResponse, new()
		{
			Uri requestUri = new Uri(this.BaseUri, relativeUrl);
			this.webSession.Post<T>(requestUri, requestBody, new Func<HttpWebResponse, T>(WebApplication.GetPage<T>), successCallback, failedCallback);
		}

		public T Post<T>(string relativeUrl, RequestBody requestBody) where T : WebApplicationResponse, new()
		{
			Uri requestUri = new Uri(this.BaseUri, relativeUrl);
			return this.webSession.Post<T>(requestUri, requestBody, new Func<HttpWebResponse, T>(WebApplication.GetPage<T>));
		}

		private static T GetPage<T>(HttpWebResponse response) where T : WebApplicationResponse, new()
		{
			T result = Activator.CreateInstance<T>();
			result.SetResponse(response);
			return result;
		}

		public abstract bool ValidateLogin();

		private WebSession webSession;
	}
}
