using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Collections.Generic
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class KeyNotFoundException : SystemException, ISerializable
	{
		[__DynamicallyInvokable]
		public KeyNotFoundException() : base(Environment.GetResourceString("Arg_KeyNotFound"))
		{
			base.SetErrorCode(-2146232969);
		}

		[__DynamicallyInvokable]
		public KeyNotFoundException(string message) : base(message)
		{
			base.SetErrorCode(-2146232969);
		}

		[__DynamicallyInvokable]
		public KeyNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146232969);
		}

		protected KeyNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
