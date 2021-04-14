using System;
using System.Collections.Specialized;
using System.Net;
using System.ServiceModel.Web;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class OutgoingWebResponseContextWrapper : IOutgoingWebResponseContext
	{
		public OutgoingWebResponseContextWrapper(OutgoingWebResponseContext response)
		{
			if (response == null)
			{
				throw new ArgumentNullException("response");
			}
			this.response = response;
		}

		public HttpStatusCode StatusCode
		{
			get
			{
				return this.response.StatusCode;
			}
			set
			{
				this.response.StatusCode = value;
			}
		}

		public string ETag
		{
			set
			{
				this.response.ETag = value;
			}
		}

		public string Expires
		{
			set
			{
				this.response.Headers[HttpResponseHeader.Expires] = value;
			}
		}

		public string ContentType
		{
			set
			{
				this.response.ContentType = value;
			}
		}

		public NameValueCollection Headers
		{
			get
			{
				return this.response.Headers;
			}
		}

		public void SetHeader(string name, string value)
		{
			this.response.Headers[name] = value;
		}

		public bool SuppressContent
		{
			get
			{
				return this.response.SuppressEntityBody;
			}
			set
			{
				this.response.SuppressEntityBody = value;
			}
		}

		private OutgoingWebResponseContext response;
	}
}
