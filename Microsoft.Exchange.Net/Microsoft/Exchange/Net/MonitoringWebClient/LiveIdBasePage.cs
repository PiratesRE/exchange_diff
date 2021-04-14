using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class LiveIdBasePage
	{
		public string PostUrl { get; private set; }

		public Uri PostUri { get; protected set; }

		public Dictionary<string, string> HiddenFields { get; protected set; }

		public string HiddenFieldsString
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string text in this.HiddenFields.Keys)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append("&");
					}
					stringBuilder.AppendFormat("{0}={1}", HttpUtility.UrlEncode(text), HttpUtility.UrlEncode(this.HiddenFields[text]));
				}
				return stringBuilder.ToString();
			}
		}

		protected static string GetRedirectionLocation(HttpWebResponseWrapper response)
		{
			string result = null;
			if (response != null)
			{
				result = response.Headers["Location"];
			}
			return result;
		}

		protected void SetPostUrl(Uri uri, HttpWebRequestWrapper request)
		{
			this.PostUrl = uri.ToString();
			this.PostUri = uri;
		}

		protected void SetPostUrl(string newValue, HttpWebRequestWrapper request)
		{
			this.PostUrl = newValue;
			Uri postUri;
			if (!Uri.TryCreate(newValue, UriKind.Absolute, out postUri))
			{
				postUri = new Uri(string.Format("https://{0}{1}", request.RequestUri.Host, newValue));
			}
			this.PostUri = postUri;
		}
	}
}
