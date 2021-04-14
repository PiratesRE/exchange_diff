using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security;

namespace System
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public class ArgumentNullException : ArgumentException
	{
		[__DynamicallyInvokable]
		public ArgumentNullException() : base(Environment.GetResourceString("ArgumentNull_Generic"))
		{
			base.SetErrorCode(-2147467261);
		}

		[__DynamicallyInvokable]
		public ArgumentNullException(string paramName) : base(Environment.GetResourceString("ArgumentNull_Generic"), paramName)
		{
			base.SetErrorCode(-2147467261);
		}

		[__DynamicallyInvokable]
		public ArgumentNullException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147467261);
		}

		[__DynamicallyInvokable]
		public ArgumentNullException(string paramName, string message) : base(message, paramName)
		{
			base.SetErrorCode(-2147467261);
		}

		[SecurityCritical]
		protected ArgumentNullException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
