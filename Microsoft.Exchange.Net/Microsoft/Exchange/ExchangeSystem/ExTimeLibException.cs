using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	[Serializable]
	public class ExTimeLibException : InvalidOperationException
	{
		public ExTimeLibException(string message) : base(message)
		{
		}

		public ExTimeLibException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
