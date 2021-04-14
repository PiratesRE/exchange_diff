using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	[Serializable]
	public class RemotingException : SystemException
	{
		public RemotingException() : base(RemotingException._nullMessage)
		{
			base.SetErrorCode(-2146233077);
		}

		public RemotingException(string message) : base(message)
		{
			base.SetErrorCode(-2146233077);
		}

		public RemotingException(string message, Exception InnerException) : base(message, InnerException)
		{
			base.SetErrorCode(-2146233077);
		}

		protected RemotingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static string _nullMessage = Environment.GetResourceString("Remoting_Default");
	}
}
