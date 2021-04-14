using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class UnauthorizedAccessException : SystemException
	{
		[__DynamicallyInvokable]
		public UnauthorizedAccessException() : base(Environment.GetResourceString("Arg_UnauthorizedAccessException"))
		{
			base.SetErrorCode(-2147024891);
		}

		[__DynamicallyInvokable]
		public UnauthorizedAccessException(string message) : base(message)
		{
			base.SetErrorCode(-2147024891);
		}

		[__DynamicallyInvokable]
		public UnauthorizedAccessException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2147024891);
		}

		protected UnauthorizedAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
