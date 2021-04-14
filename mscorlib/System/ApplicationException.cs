using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class ApplicationException : Exception
	{
		public ApplicationException() : base(Environment.GetResourceString("Arg_ApplicationException"))
		{
			base.SetErrorCode(-2146232832);
		}

		public ApplicationException(string message) : base(message)
		{
			base.SetErrorCode(-2146232832);
		}

		public ApplicationException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146232832);
		}

		protected ApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
