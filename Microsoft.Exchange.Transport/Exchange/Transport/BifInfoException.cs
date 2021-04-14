using System;

namespace Microsoft.Exchange.Transport
{
	internal class BifInfoException : ApplicationException
	{
		public BifInfoException(string message) : base(message)
		{
		}

		public BifInfoException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
