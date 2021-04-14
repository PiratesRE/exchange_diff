using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class InvalidOperationException : SystemException
	{
		[__DynamicallyInvokable]
		public InvalidOperationException() : base(Environment.GetResourceString("Arg_InvalidOperationException"))
		{
			base.SetErrorCode(-2146233079);
		}

		[__DynamicallyInvokable]
		public InvalidOperationException(string message) : base(message)
		{
			base.SetErrorCode(-2146233079);
		}

		[__DynamicallyInvokable]
		public InvalidOperationException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233079);
		}

		protected InvalidOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
