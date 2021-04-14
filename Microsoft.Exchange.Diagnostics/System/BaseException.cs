using System;
using System.Runtime.Serialization;

namespace System
{
	public class BaseException : Exception
	{
		public BaseException()
		{
		}

		public BaseException(string message) : base(message, null)
		{
		}

		public BaseException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
