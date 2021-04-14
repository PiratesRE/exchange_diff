using System;
using System.Collections.Generic;
using System.Net;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class CookieAwareWebRequestCreator : IWebRequestCreate
	{
		public CookieAwareWebRequestCreator()
		{
			string text = TestIntegration.Instance.RoutingCookie;
			if (string.IsNullOrEmpty(text))
			{
				text = "exchangecookie";
			}
			else if (text.Equals("Disabled", StringComparison.InvariantCultureIgnoreCase))
			{
				text = null;
			}
			if (text != null)
			{
				this.routingCookie = new Cookie(text, Guid.NewGuid().ToString("N"));
			}
		}

		public bool IsDisabled
		{
			get
			{
				return this.routingCookie == null;
			}
		}

		public WebRequest Create(Uri uri)
		{
			uri == null;
			string input = uri.AbsolutePath.Remove(0, 1);
			MRSProxyRequestContext mrsproxyRequestContext = null;
			Guid id;
			if (Guid.TryParse(input, out id))
			{
				mrsproxyRequestContext = MRSProxyRequestContext.Find(id);
			}
			if (mrsproxyRequestContext == null)
			{
				return (HttpWebRequest)WebRequest.CreateDefault(uri);
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.CreateDefault(mrsproxyRequestContext.EndpointUri);
			if (!this.IsDisabled)
			{
				httpWebRequest.CookieContainer = new CookieContainer();
				httpWebRequest.CookieContainer.Add(httpWebRequest.RequestUri, this.routingCookie);
				if (mrsproxyRequestContext.BackendCookie != null)
				{
					httpWebRequest.CookieContainer.Add(httpWebRequest.RequestUri, mrsproxyRequestContext.BackendCookie);
				}
			}
			foreach (KeyValuePair<string, string> keyValuePair in mrsproxyRequestContext.HttpHeaders)
			{
				httpWebRequest.Headers.Add(keyValuePair.Key, keyValuePair.Value);
			}
			return httpWebRequest;
		}

		private const string DefaultCookieName = "exchangecookie";

		private Cookie routingCookie;
	}
}
