using System;
using System.Runtime.Serialization;

namespace System
{
	public class ExInvalidCastException : InvalidCastException
	{
		public ExInvalidCastException()
		{
		}

		public ExInvalidCastException(string message) : base(message)
		{
		}

		public ExInvalidCastException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ExInvalidCastException(string message, int errorCode) : base(message, errorCode)
		{
		}

		protected ExInvalidCastException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
