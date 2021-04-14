using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class OutOfMemoryException : SystemException
	{
		[__DynamicallyInvokable]
		public OutOfMemoryException() : base(Exception.GetMessageFromNativeResources(Exception.ExceptionMessageKind.OutOfMemory))
		{
			base.SetErrorCode(-2147024882);
		}

		[__DynamicallyInvokable]
		public OutOfMemoryException(string message) : base(message)
		{
			base.SetErrorCode(-2147024882);
		}

		[__DynamicallyInvokable]
		public OutOfMemoryException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147024882);
		}

		protected OutOfMemoryException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
