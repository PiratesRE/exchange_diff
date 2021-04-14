using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Common
{
	[Serializable]
	public class InvalidSerializedFormatException : Exception
	{
		public InvalidSerializedFormatException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public InvalidSerializedFormatException(string message) : base(message)
		{
		}

		public InvalidSerializedFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
