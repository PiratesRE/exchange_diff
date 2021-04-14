using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Microsoft.Exchange.Diagnostics;
using Microsoft.OData.Core;

namespace Microsoft.Exchange.Services.OData.Web
{
	internal class ResponseMessage : IODataResponseMessage
	{
		public ResponseMessage(HttpContext httpContext)
		{
			ArgumentValidator.ThrowIfNull("httpContext", httpContext);
			this.HttpContext = httpContext;
			this.headers = new List<KeyValuePair<string, string>>();
			foreach (string text in this.HttpContext.Response.Headers.AllKeys)
			{
				this.headers.Add(new KeyValuePair<string, string>(text, this.HttpContext.Response.Headers[text]));
			}
		}

		public HttpContext HttpContext { get; private set; }

		public IEnumerable<KeyValuePair<string, string>> Headers
		{
			get
			{
				return this.headers;
			}
		}

		public Stream GetStream()
		{
			return this.HttpContext.Response.OutputStream;
		}

		public int StatusCode
		{
			get
			{
				return this.HttpContext.Response.StatusCode;
			}
			set
			{
				this.HttpContext.Response.StatusCode = value;
			}
		}

		public string GetHeader(string headerName)
		{
			return this.HttpContext.Response.Headers[headerName];
		}

		public void SetHeader(string headerName, string headerValue)
		{
			this.HttpContext.Response.Headers[headerName] = headerValue;
			if (string.Equals(headerName, "content-type", StringComparison.OrdinalIgnoreCase))
			{
				this.HttpContext.Response.ContentType = headerValue;
			}
		}

		private List<KeyValuePair<string, string>> headers;
	}
}
