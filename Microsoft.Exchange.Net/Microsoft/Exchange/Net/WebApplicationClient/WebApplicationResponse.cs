using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net.WebApplicationClient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class WebApplicationResponse
	{
		public virtual void SetResponse(HttpWebResponse response)
		{
			this.StatusCode = response.StatusCode;
		}

		public HttpStatusCode StatusCode { get; private set; }
	}
}
