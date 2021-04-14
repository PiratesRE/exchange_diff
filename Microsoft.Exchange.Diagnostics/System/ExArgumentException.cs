using System;
using System.Runtime.Serialization;

namespace System
{
	public class ExArgumentException : ArgumentException
	{
		public ExArgumentException()
		{
		}

		public ExArgumentException(string message) : base(message)
		{
		}

		public ExArgumentException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ExArgumentException(string message, string paramName) : base(message, paramName)
		{
		}

		public ExArgumentException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
		{
		}

		protected ExArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
