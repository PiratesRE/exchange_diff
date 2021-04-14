using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Threading
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class SynchronizationLockException : SystemException
	{
		[__DynamicallyInvokable]
		public SynchronizationLockException() : base(Environment.GetResourceString("Arg_SynchronizationLockException"))
		{
			base.SetErrorCode(-2146233064);
		}

		[__DynamicallyInvokable]
		public SynchronizationLockException(string message) : base(message)
		{
			base.SetErrorCode(-2146233064);
		}

		[__DynamicallyInvokable]
		public SynchronizationLockException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2146233064);
		}

		protected SynchronizationLockException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
