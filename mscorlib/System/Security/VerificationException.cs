using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class VerificationException : SystemException
	{
		[__DynamicallyInvokable]
		public VerificationException() : base(Environment.GetResourceString("Verification_Exception"))
		{
			base.SetErrorCode(-2146233075);
		}

		[__DynamicallyInvokable]
		public VerificationException(string message) : base(message)
		{
			base.SetErrorCode(-2146233075);
		}

		[__DynamicallyInvokable]
		public VerificationException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233075);
		}

		protected VerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
