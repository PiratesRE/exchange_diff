using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class ArrayTypeMismatchException : SystemException
	{
		[__DynamicallyInvokable]
		public ArrayTypeMismatchException() : base(Environment.GetResourceString("Arg_ArrayTypeMismatchException"))
		{
			base.SetErrorCode(-2146233085);
		}

		[__DynamicallyInvokable]
		public ArrayTypeMismatchException(string message) : base(message)
		{
			base.SetErrorCode(-2146233085);
		}

		[__DynamicallyInvokable]
		public ArrayTypeMismatchException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233085);
		}

		protected ArrayTypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
