using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	[Serializable]
	public class ServerException : SystemException
	{
		public ServerException() : base(ServerException._nullMessage)
		{
			base.SetErrorCode(-2146233074);
		}

		public ServerException(string message) : base(message)
		{
			base.SetErrorCode(-2146233074);
		}

		public ServerException(string message, Exception InnerException) : base(message, InnerException)
		{
			base.SetErrorCode(-2146233074);
		}

		internal ServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		private static string _nullMessage = Environment.GetResourceString("Remoting_Default");
	}
}
