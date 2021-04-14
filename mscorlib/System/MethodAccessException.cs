using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class MethodAccessException : MemberAccessException
	{
		[__DynamicallyInvokable]
		public MethodAccessException() : base(Environment.GetResourceString("Arg_MethodAccessException"))
		{
			base.SetErrorCode(-2146233072);
		}

		[__DynamicallyInvokable]
		public MethodAccessException(string message) : base(message)
		{
			base.SetErrorCode(-2146233072);
		}

		[__DynamicallyInvokable]
		public MethodAccessException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233072);
		}

		protected MethodAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
