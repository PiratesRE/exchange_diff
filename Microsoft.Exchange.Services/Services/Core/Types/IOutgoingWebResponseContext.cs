using System;
using System.Collections.Specialized;
using System.Net;

namespace Microsoft.Exchange.Services.Core.Types
{
	public interface IOutgoingWebResponseContext
	{
		HttpStatusCode StatusCode { get; set; }

		string ETag { set; }

		string Expires { set; }

		string ContentType { set; }

		NameValueCollection Headers { get; }

		bool SuppressContent { get; set; }
	}
}
