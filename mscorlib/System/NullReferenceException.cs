using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class NullReferenceException : SystemException
	{
		[__DynamicallyInvokable]
		public NullReferenceException() : base(Environment.GetResourceString("Arg_NullReferenceException"))
		{
			base.SetErrorCode(-2147467261);
		}

		[__DynamicallyInvokable]
		public NullReferenceException(string message) : base(message)
		{
			base.SetErrorCode(-2147467261);
		}

		[__DynamicallyInvokable]
		public NullReferenceException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147467261);
		}

		protected NullReferenceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
