using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class FieldAccessException : MemberAccessException
	{
		[__DynamicallyInvokable]
		public FieldAccessException() : base(Environment.GetResourceString("Arg_FieldAccessException"))
		{
			base.SetErrorCode(-2146233081);
		}

		[__DynamicallyInvokable]
		public FieldAccessException(string message) : base(message)
		{
			base.SetErrorCode(-2146233081);
		}

		[__DynamicallyInvokable]
		public FieldAccessException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233081);
		}

		protected FieldAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
