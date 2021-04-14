using System;
using System.Runtime.Serialization;

namespace System
{
	public class ExObjectDisposedException : ObjectDisposedException
	{
		public ExObjectDisposedException(string objectName) : base(objectName)
		{
		}

		public ExObjectDisposedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public ExObjectDisposedException(string objectName, string message) : base(objectName, message)
		{
		}

		protected ExObjectDisposedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
