using System;
using System.Runtime.InteropServices;

namespace System.Runtime.Serialization
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class SerializationException : SystemException
	{
		[__DynamicallyInvokable]
		public SerializationException() : base(SerializationException._nullMessage)
		{
			base.SetErrorCode(-2146233076);
		}

		[__DynamicallyInvokable]
		public SerializationException(string message) : base(message)
		{
			base.SetErrorCode(-2146233076);
		}

		[__DynamicallyInvokable]
		public SerializationException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233076);
		}

		protected SerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static string _nullMessage = Environment.GetResourceString("Arg_SerializationException");
	}
}
