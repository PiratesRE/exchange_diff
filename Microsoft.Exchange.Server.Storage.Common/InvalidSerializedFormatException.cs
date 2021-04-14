using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class InvalidSerializedFormatException : Exception
	{
		public InvalidSerializedFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public InvalidSerializedFormatException(string message) : base(message)
		{
		}
	}
}
