using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class ArithmeticException : SystemException
	{
		[__DynamicallyInvokable]
		public ArithmeticException() : base(Environment.GetResourceString("Arg_ArithmeticException"))
		{
			base.SetErrorCode(-2147024362);
		}

		[__DynamicallyInvokable]
		public ArithmeticException(string message) : base(message)
		{
			base.SetErrorCode(-2147024362);
		}

		[__DynamicallyInvokable]
		public ArithmeticException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147024362);
		}

		protected ArithmeticException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
