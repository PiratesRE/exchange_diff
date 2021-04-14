using System;

namespace Microsoft.Exchange.Transport
{
	internal class Exch50Exception : ApplicationException
	{
		public Exch50Exception(string message) : base(message)
		{
		}

		public Exch50Exception(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
