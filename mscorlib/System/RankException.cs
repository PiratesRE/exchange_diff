using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class RankException : SystemException
	{
		[__DynamicallyInvokable]
		public RankException() : base(Environment.GetResourceString("Arg_RankException"))
		{
			base.SetErrorCode(-2146233065);
		}

		[__DynamicallyInvokable]
		public RankException(string message) : base(message)
		{
			base.SetErrorCode(-2146233065);
		}

		[__DynamicallyInvokable]
		public RankException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233065);
		}

		protected RankException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
