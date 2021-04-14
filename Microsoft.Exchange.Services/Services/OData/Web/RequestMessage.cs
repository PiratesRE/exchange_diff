using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class RequestMessage : IODataRequestMessage
	{
		public RequestMessage(HttpContext httpContext)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			this.HttpContext = httpContext;
			this.Method = this.HttpContext.Request.HttpMethod;
			this.Url = this.HttpContext.Request.Url;
			this.headers = new List<KeyValuePair<string, string>>();
			foreach (string text in this.HttpContext.Request.Headers.AllKeys)
			{
				this.headers.Add(new KeyValuePair<string, string>(text, this.HttpContext.Request.Headers[text]));
			}
		}

		public HttpContext HttpContext { get; private set; }

		public string GetHeader(string headerName)
		{
			return this.HttpContext.Request.Headers[headerName];
		}

		public void SetHeader(string headerName, string headerValue)
		{
			this.HttpContext.Request.Headers[headerName] = headerValue;
		}

		public Stream GetStream()
		{
			return this.HttpContext.Request.InputStream;
		}

		public IEnumerable<KeyValuePair<string, string>> Headers
		{
			get
			{
				return this.headers;
			}
		}

		public Uri Url { get; set; }

		public string Method { get; set; }

		private List<KeyValuePair<string, string>> headers;
	}
}
