using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.IO
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class EndOfStreamException : IOException
	{
		[__DynamicallyInvokable]
		public EndOfStreamException() : base(Environment.GetResourceString("Arg_EndOfStreamException"))
		{
			base.SetErrorCode(-2147024858);
		}

		[__DynamicallyInvokable]
		public EndOfStreamException(string message) : base(message)
		{
			base.SetErrorCode(-2147024858);
		}

		[__DynamicallyInvokable]
		public EndOfStreamException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147024858);
		}

		protected EndOfStreamException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
