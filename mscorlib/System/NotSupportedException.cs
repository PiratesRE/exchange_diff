using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class NotSupportedException : SystemException
	{
		[__DynamicallyInvokable]
		public NotSupportedException() : base(Environment.GetResourceString("Arg_NotSupportedException"))
		{
			base.SetErrorCode(-2146233067);
		}

		[__DynamicallyInvokable]
		public NotSupportedException(string message) : base(message)
		{
			base.SetErrorCode(-2146233067);
		}

		[__DynamicallyInvokable]
		public NotSupportedException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233067);
		}

		protected NotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
