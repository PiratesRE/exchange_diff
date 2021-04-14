using System;
using System.Runtime.Serialization;

namespace System.Runtime.InteropServices
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class SEHException : ExternalException
	{
		[__DynamicallyInvokable]
		public SEHException()
		{
			base.SetErrorCode(-2147467259);
		}

		[__DynamicallyInvokable]
		public SEHException(string message) : base(message)
		{
			base.SetErrorCode(-2147467259);
		}

		[__DynamicallyInvokable]
		public SEHException(string message, Exception inner) : base(message, inner)
		{
			base.SetErrorCode(-2147467259);
		}

		protected SEHException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		[__DynamicallyInvokable]
		public virtual bool CanResume()
		{
			return false;
		}
	}
}
