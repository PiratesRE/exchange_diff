using System;
using System.Runtime.Serialization;

namespace System
{
	public class ExInvalidOperationException : InvalidOperationException
	{
		public ExInvalidOperationException()
		{
		}

		public ExInvalidOperationException(string message) : base(message)
		{
		}

		public ExInvalidOperationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ExInvalidOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
