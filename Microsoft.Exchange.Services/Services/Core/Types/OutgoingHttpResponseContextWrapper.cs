using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Web;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class OutgoingHttpResponseContextWrapper : IOutgoingWebResponseContext
	{
		public OutgoingHttpResponseContextWrapper(HttpResponse response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			this.response = response;
			this.response.TrySkipIisCustomErrors = true;
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return (HttpStatusCode)this.response.StatusCode;
			}
			set
			{
				this.response.StatusCode = (int)value;
			}
		}

		public string ETag
		{
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					this.response.Cache.SetETag(value);
				}
			}
		}

		public string Expires
		{
			set
			{
				this.response.Headers["Expires"] = value;
			}
		}

		public string ContentType
		{
			set
			{
				this.response.ContentType = value;
			}
		}

		public long ContentLength
		{
			set
			{
				this.response.Headers["Content-Length"] = value.ToString(CultureInfo.InvariantCulture);
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return this.response.Headers;
			}
		}

		public bool SuppressContent
		{
			get
			{
				return this.response.SuppressContent;
			}
			set
			{
				this.response.SuppressContent = value;
			}
		}

		private HttpResponse response;
	}
}
