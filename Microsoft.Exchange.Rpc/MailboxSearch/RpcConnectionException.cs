using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	[Serializable]
	internal class RpcConnectionException : RpcException
	{
		public RpcConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RpcConnectionException(string message, int hr) : base(message, hr)
		{
		}

		public RpcConnectionException(string message) : base(message)
		{
		}
	}
}
