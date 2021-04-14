using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class HttpResponseTransportException : HttpTransportException
	{
		public HttpResponseTransportException(Exception innerException) : base(innerException)
		{
		}
	}
}
