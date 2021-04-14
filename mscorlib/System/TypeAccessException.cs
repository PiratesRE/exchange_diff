using System;
using System.Runtime.Serialization;

namespace System
{
	[__DynamicallyInvokable]
	[Serializable]
	public class TypeAccessException : TypeLoadException
	{
		[__DynamicallyInvokable]
		public TypeAccessException() : base(Environment.GetResourceString("Arg_TypeAccessException"))
		{
			base.SetErrorCode(-2146233021);
		}

		[__DynamicallyInvokable]
		public TypeAccessException(string message) : base(message)
		{
			base.SetErrorCode(-2146233021);
		}

		[__DynamicallyInvokable]
		public TypeAccessException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233021);
		}

		protected TypeAccessException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			base.SetErrorCode(-2146233021);
		}
	}
}
