using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System
{
	[ComVisible(true)]
	[Serializable]
	public class AccessViolationException : SystemException
	{
		public AccessViolationException() : base(Environment.GetResourceString("Arg_AccessViolationException"))
		{
			base.SetErrorCode(-2147467261);
		}

		public AccessViolationException(string message) : base(message)
		{
			base.SetErrorCode(-2147467261);
		}

		public AccessViolationException(string message, Exception innerException) : base(message, innerException)
		{
			base.SetErrorCode(-2147467261);
		}

		protected AccessViolationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private IntPtr _ip;

		private IntPtr _target;

		private int _accessType;
	}
}
