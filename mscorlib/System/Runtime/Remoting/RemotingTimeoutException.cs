using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	[Serializable]
	public class RemotingTimeoutException : RemotingException
	{
		public RemotingTimeoutException() : base(RemotingTimeoutException._nullMessage)
		{
		}

		public RemotingTimeoutException(string message) : base(message)
		{
			base.SetErrorCode(-2146233077);
		}

		public RemotingTimeoutException(string message, Exception InnerException) : base(message, InnerException)
		{
			base.SetErrorCode(-2146233077);
		}

		internal RemotingTimeoutException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static string _nullMessage = Environment.GetResourceString("Remoting_Default");
	}
}
