using System;
using System.Net;

namespace Microsoft.Exchange.Services.OData
{
	public class ErrorResponse
	{
		public HttpStatusCode HttpStatusCode { get; set; }

		public ErrorDetails ErrorDetails { get; set; }
	}
}
