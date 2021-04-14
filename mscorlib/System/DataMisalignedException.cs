using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public sealed class DataMisalignedException : SystemException
	{
		[__DynamicallyInvokable]
		public DataMisalignedException() : base(Environment.GetResourceString("Arg_DataMisalignedException"))
		{
			base.SetErrorCode(-2146233023);
		}

		[__DynamicallyInvokable]
		public DataMisalignedException(string message) : base(message)
		{
			base.SetErrorCode(-2146233023);
		}

		[__DynamicallyInvokable]
		public DataMisalignedException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233023);
		}

		internal DataMisalignedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
