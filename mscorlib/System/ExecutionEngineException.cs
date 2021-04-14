using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[Obsolete("This type previously indicated an unspecified fatal error in the runtime. The runtime no longer raises this exception so this type is obsolete.")]
	[ComVisible(true)]
	[Serializable]
	public sealed class ExecutionEngineException : SystemException
	{
		public ExecutionEngineException() : base(Environment.GetResourceString("Arg_ExecutionEngineException"))
		{
			base.SetErrorCode(-2146233082);
		}

		public ExecutionEngineException(string message) : base(message)
		{
			base.SetErrorCode(-2146233082);
		}

		public ExecutionEngineException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233082);
		}

		internal ExecutionEngineException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
