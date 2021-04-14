using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class InvalidProgramException : SystemException
	{
		[__DynamicallyInvokable]
		public InvalidProgramException() : base(Environment.GetResourceString("InvalidProgram_Default"))
		{
			base.SetErrorCode(-2146233030);
		}

		[__DynamicallyInvokable]
		public InvalidProgramException(string message) : base(message)
		{
			base.SetErrorCode(-2146233030);
		}

		[__DynamicallyInvokable]
		public InvalidProgramException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233030);
		}

		internal InvalidProgramException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
