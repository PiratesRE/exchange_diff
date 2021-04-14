using System;
using System.Runtime.Serialization;

namespace System
{
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class InsufficientExecutionStackException : SystemException
	{
		[__DynamicallyInvokable]
		public InsufficientExecutionStackException() : base(Environment.GetResourceString("Arg_InsufficientExecutionStackException"))
		{
			base.SetErrorCode(-2146232968);
		}

		[__DynamicallyInvokable]
		public InsufficientExecutionStackException(string message) : base(message)
		{
			base.SetErrorCode(-2146232968);
		}

		[__DynamicallyInvokable]
		public InsufficientExecutionStackException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146232968);
		}

		private InsufficientExecutionStackException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
