using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class PathTooLongException : IOException
	{
		[__DynamicallyInvokable]
		public PathTooLongException() : base(Environment.GetResourceString("IO.PathTooLong"))
		{
			base.SetErrorCode(-2147024690);
		}

		[__DynamicallyInvokable]
		public PathTooLongException(string message) : base(message)
		{
			base.SetErrorCode(-2147024690);
		}

		[__DynamicallyInvokable]
		public PathTooLongException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147024690);
		}

		protected PathTooLongException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
