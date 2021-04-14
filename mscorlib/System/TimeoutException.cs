using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class TimeoutException : SystemException
	{
		[__DynamicallyInvokable]
		public TimeoutException() : base(Environment.GetResourceString("Arg_TimeoutException"))
		{
			base.SetErrorCode(-2146233083);
		}

		[__DynamicallyInvokable]
		public TimeoutException(string message) : base(message)
		{
			base.SetErrorCode(-2146233083);
		}

		[__DynamicallyInvokable]
		public TimeoutException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233083);
		}

		protected TimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
