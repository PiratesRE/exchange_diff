using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class InvalidCastException : SystemException
	{
		[__DynamicallyInvokable]
		public InvalidCastException() : base(Environment.GetResourceString("Arg_InvalidCastException"))
		{
			base.SetErrorCode(-2147467262);
		}

		[__DynamicallyInvokable]
		public InvalidCastException(string message) : base(message)
		{
			base.SetErrorCode(-2147467262);
		}

		[__DynamicallyInvokable]
		public InvalidCastException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147467262);
		}

		protected InvalidCastException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[__DynamicallyInvokable]
		public InvalidCastException(string message, int errorCode) : base(message)
		{
			base.SetErrorCode(errorCode);
		}
	}
}
