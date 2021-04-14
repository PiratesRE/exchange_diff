using System;
using System.Runtime.Serialization;

namespace System
{
	public class ExArgumentOutOfRangeException : ArgumentOutOfRangeException
	{
		public ExArgumentOutOfRangeException()
		{
		}

		public ExArgumentOutOfRangeException(string message) : base(message)
		{
		}

		public ExArgumentOutOfRangeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ExArgumentOutOfRangeException(string message, string paramName) : base(message, paramName)
		{
		}

		public ExArgumentOutOfRangeException(string paramName, object actualValue, string message) : base(paramName, actualValue, message)
		{
		}

		protected ExArgumentOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
