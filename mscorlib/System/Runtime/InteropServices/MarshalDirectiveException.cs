using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class MarshalDirectiveException : SystemException
	{
		[__DynamicallyInvokable]
		public MarshalDirectiveException() : base(Environment.GetResourceString("Arg_MarshalDirectiveException"))
		{
			base.SetErrorCode(-2146233035);
		}

		[__DynamicallyInvokable]
		public MarshalDirectiveException(string message) : base(message)
		{
			base.SetErrorCode(-2146233035);
		}

		[__DynamicallyInvokable]
		public MarshalDirectiveException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233035);
		}

		protected MarshalDirectiveException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
