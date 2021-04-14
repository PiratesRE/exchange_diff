using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	[Serializable]
	internal class RpcFailedException : RpcException
	{
		public RpcFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RpcFailedException(string message) : base(message, 1727)
		{
		}
	}
}
