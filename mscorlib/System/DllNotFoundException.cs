using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class DllNotFoundException : TypeLoadException
	{
		[__DynamicallyInvokable]
		public DllNotFoundException() : base(Environment.GetResourceString("Arg_DllNotFoundException"))
		{
			base.SetErrorCode(-2146233052);
		}

		[__DynamicallyInvokable]
		public DllNotFoundException(string message) : base(message)
		{
			base.SetErrorCode(-2146233052);
		}

		[__DynamicallyInvokable]
		public DllNotFoundException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233052);
		}

		protected DllNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
