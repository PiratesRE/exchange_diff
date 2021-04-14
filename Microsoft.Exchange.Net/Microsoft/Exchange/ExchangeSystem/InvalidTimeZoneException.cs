using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	[Serializable]
	internal class InvalidTimeZoneException : ExArgumentException
	{
		public InvalidTimeZoneException(string message) : base(message)
		{
		}

		public InvalidTimeZoneException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
