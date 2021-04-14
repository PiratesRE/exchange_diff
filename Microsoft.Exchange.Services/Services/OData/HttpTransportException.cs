using System;

namespace Microsoft.Exchange.Services.OData
{
	internal class HttpTransportException : Exception
	{
		public HttpTransportException(Exception innerException) : base(innerException.Message, innerException)
		{
		}
	}
}
