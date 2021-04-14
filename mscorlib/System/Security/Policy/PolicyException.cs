using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Security.Policy
{
	[ComVisible(true)]
	[Serializable]
	public class PolicyException : SystemException
	{
		public PolicyException() : base(Environment.GetResourceString("Policy_Default"))
		{
			base.HResult = -2146233322;
		}

		public PolicyException(string message) : base(message)
		{
			base.HResult = -2146233322;
		}

		public PolicyException(string message, Exception exception) : base(message, exception)
		{
			base.HResult = -2146233322;
		}

		protected PolicyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		internal PolicyException(string message, int hresult) : base(message)
		{
			base.HResult = hresult;
		}

		internal PolicyException(string message, int hresult, Exception exception) : base(message, exception)
		{
			base.HResult = hresult;
		}
	}
}
