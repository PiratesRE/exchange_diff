using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class MemberAccessException : SystemException
	{
		[__DynamicallyInvokable]
		public MemberAccessException() : base(Environment.GetResourceString("Arg_AccessException"))
		{
			base.SetErrorCode(-2146233062);
		}

		[__DynamicallyInvokable]
		public MemberAccessException(string message) : base(message)
		{
			base.SetErrorCode(-2146233062);
		}

		[__DynamicallyInvokable]
		public MemberAccessException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233062);
		}

		protected MemberAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
