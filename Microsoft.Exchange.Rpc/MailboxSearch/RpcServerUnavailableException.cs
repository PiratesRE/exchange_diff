using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	[Serializable]
	internal class RpcServerUnavailableException : RpcConnectionException
	{
		public RpcServerUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RpcServerUnavailableException(string message) : base(message, 1722)
		{
		}
	}
}
