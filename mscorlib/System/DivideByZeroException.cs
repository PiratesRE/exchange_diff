using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class DivideByZeroException : ArithmeticException
	{
		[__DynamicallyInvokable]
		public DivideByZeroException() : base(Environment.GetResourceString("Arg_DivideByZero"))
		{
			base.SetErrorCode(-2147352558);
		}

		[__DynamicallyInvokable]
		public DivideByZeroException(string message) : base(message)
		{
			base.SetErrorCode(-2147352558);
		}

		[__DynamicallyInvokable]
		public DivideByZeroException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147352558);
		}

		protected DivideByZeroException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
