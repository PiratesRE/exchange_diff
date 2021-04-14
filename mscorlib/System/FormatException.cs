using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class FormatException : SystemException
	{
		[__DynamicallyInvokable]
		public FormatException() : base(Environment.GetResourceString("Arg_FormatException"))
		{
			base.SetErrorCode(-2146233033);
		}

		[__DynamicallyInvokable]
		public FormatException(string message) : base(message)
		{
			base.SetErrorCode(-2146233033);
		}

		[__DynamicallyInvokable]
		public FormatException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233033);
		}

		protected FormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
