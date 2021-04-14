using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class PlatformNotSupportedException : NotSupportedException
	{
		[__DynamicallyInvokable]
		public PlatformNotSupportedException() : base(Environment.GetResourceString("Arg_PlatformNotSupported"))
		{
			base.SetErrorCode(-2146233031);
		}

		[__DynamicallyInvokable]
		public PlatformNotSupportedException(string message) : base(message)
		{
			base.SetErrorCode(-2146233031);
		}

		[__DynamicallyInvokable]
		public PlatformNotSupportedException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233031);
		}

		protected PlatformNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
