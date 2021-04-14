using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class SafeArrayRankMismatchException : SystemException
	{
		[__DynamicallyInvokable]
		public SafeArrayRankMismatchException() : base(Environment.GetResourceString("Arg_SafeArrayRankMismatchException"))
		{
			base.SetErrorCode(-2146233032);
		}

		[__DynamicallyInvokable]
		public SafeArrayRankMismatchException(string message) : base(message)
		{
			base.SetErrorCode(-2146233032);
		}

		[__DynamicallyInvokable]
		public SafeArrayRankMismatchException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233032);
		}

		protected SafeArrayRankMismatchException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
