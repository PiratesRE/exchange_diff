using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public sealed class StackOverflowException : SystemException
	{
		public StackOverflowException() : base(Environment.GetResourceString("Arg_StackOverflowException"))
		{
			base.SetErrorCode(-2147023895);
		}

		public StackOverflowException(string message) : base(message)
		{
			base.SetErrorCode(-2147023895);
		}

		public StackOverflowException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147023895);
		}

		internal StackOverflowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
