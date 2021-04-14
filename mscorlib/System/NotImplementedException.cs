using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class NotImplementedException : SystemException
	{
		[__DynamicallyInvokable]
		public NotImplementedException() : base(Environment.GetResourceString("Arg_NotImplementedException"))
		{
			base.SetErrorCode(-2147467263);
		}

		[__DynamicallyInvokable]
		public NotImplementedException(string message) : base(message)
		{
			base.SetErrorCode(-2147467263);
		}

		[__DynamicallyInvokable]
		public NotImplementedException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2147467263);
		}

		protected NotImplementedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
