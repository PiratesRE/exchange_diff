using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class SafeArrayTypeMismatchException : SystemException
	{
		[__DynamicallyInvokable]
		public SafeArrayTypeMismatchException() : base(Environment.GetResourceString("Arg_SafeArrayTypeMismatchException"))
		{
			base.SetErrorCode(-2146233037);
		}

		[__DynamicallyInvokable]
		public SafeArrayTypeMismatchException(string message) : base(message)
		{
			base.SetErrorCode(-2146233037);
		}

		[__DynamicallyInvokable]
		public SafeArrayTypeMismatchException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233037);
		}

		protected SafeArrayTypeMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
