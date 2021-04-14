using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class InvalidComObjectException : SystemException
	{
		[__DynamicallyInvokable]
		public InvalidComObjectException() : base(Environment.GetResourceString("Arg_InvalidComObjectException"))
		{
			base.SetErrorCode(-2146233049);
		}

		[__DynamicallyInvokable]
		public InvalidComObjectException(string message) : base(message)
		{
			base.SetErrorCode(-2146233049);
		}

		[__DynamicallyInvokable]
		public InvalidComObjectException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233049);
		}

		protected InvalidComObjectException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
