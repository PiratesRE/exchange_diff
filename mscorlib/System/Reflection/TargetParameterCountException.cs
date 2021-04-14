using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Reflection
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class TargetParameterCountException : ApplicationException
	{
		[__DynamicallyInvokable]
		public TargetParameterCountException() : base(Environment.GetResourceString("Arg_TargetParameterCountException"))
		{
			base.SetErrorCode(-2147352562);
		}

		[__DynamicallyInvokable]
		public TargetParameterCountException(string message) : base(message)
		{
			base.SetErrorCode(-2147352562);
		}

		[__DynamicallyInvokable]
		public TargetParameterCountException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2147352562);
		}

		internal TargetParameterCountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
