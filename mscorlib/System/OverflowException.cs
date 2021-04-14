using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class OverflowException : ArithmeticException
	{
		[__DynamicallyInvokable]
		public OverflowException() : base(Environment.GetResourceString("Arg_OverflowException"))
		{
			base.SetErrorCode(-2146233066);
		}

		[__DynamicallyInvokable]
		public OverflowException(string message) : base(message)
		{
			base.SetErrorCode(-2146233066);
		}

		[__DynamicallyInvokable]
		public OverflowException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233066);
		}

		protected OverflowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
