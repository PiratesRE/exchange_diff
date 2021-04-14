using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class IndexOutOfRangeException : SystemException
	{
		[__DynamicallyInvokable]
		public IndexOutOfRangeException() : base(Environment.GetResourceString("Arg_IndexOutOfRangeException"))
		{
			base.SetErrorCode(-2146233080);
		}

		[__DynamicallyInvokable]
		public IndexOutOfRangeException(string message) : base(message)
		{
			base.SetErrorCode(-2146233080);
		}

		[__DynamicallyInvokable]
		public IndexOutOfRangeException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233080);
		}

		internal IndexOutOfRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
