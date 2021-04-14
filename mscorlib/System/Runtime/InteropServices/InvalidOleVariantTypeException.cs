using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class InvalidOleVariantTypeException : SystemException
	{
		[__DynamicallyInvokable]
		public InvalidOleVariantTypeException() : base(Environment.GetResourceString("Arg_InvalidOleVariantTypeException"))
		{
			base.SetErrorCode(-2146233039);
		}

		[__DynamicallyInvokable]
		public InvalidOleVariantTypeException(string message) : base(message)
		{
			base.SetErrorCode(-2146233039);
		}

		[__DynamicallyInvokable]
		public InvalidOleVariantTypeException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2146233039);
		}

		protected InvalidOleVariantTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
