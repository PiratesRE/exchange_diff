using System;

namespace Microsoft.Exchange.Transport
{
	internal class TransportPropertyException : ApplicationException
	{
		public TransportPropertyException(string message) : base(message)
		{
		}

		public TransportPropertyException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
