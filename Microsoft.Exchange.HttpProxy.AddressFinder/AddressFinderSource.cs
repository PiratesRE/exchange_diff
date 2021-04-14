using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.HttpProxy.AddressFinder
{
	internal sealed class AddressFinderSource
	{
		public AddressFinderSource(IDictionary items, NameValueCollection headers, NameValueCollection queryString, Uri url, string applicationPath, string filePath, HttpCookieCollection cookies)
		{
			ArgumentValidator.ThrowIfNull("items", items);
			ArgumentValidator.ThrowIfNull("headers", headers);
			ArgumentValidator.ThrowIfNull("queryString", queryString);
			ArgumentValidator.ThrowIfNull("url", url);
			ArgumentValidator.ThrowIfNull("applicationPath", applicationPath);
			ArgumentValidator.ThrowIfNull("filePath", filePath);
			ArgumentValidator.ThrowIfNull("cookies", cookies);
			this.Items = items;
			this.Headers = headers;
			this.QueryString = queryString;
			this.Url = url;
			this.ApplicationPath = applicationPath;
			this.FilePath = filePath;
			this.Cookies = cookies;
		}

		public IDictionary Items { get; private set; }

		public NameValueCollection Headers { get; private set; }

		public NameValueCollection QueryString { get; private set; }

		public Uri Url { get; private set; }

		public string ApplicationPath { get; private set; }

		public string FilePath { get; private set; }

		public HttpCookieCollection Cookies { get; private set; }
	}
}
