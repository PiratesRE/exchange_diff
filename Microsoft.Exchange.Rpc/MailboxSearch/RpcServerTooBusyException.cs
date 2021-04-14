using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	[Serializable]
	internal class RpcServerTooBusyException : RpcException
	{
		public RpcServerTooBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RpcServerTooBusyException(string message) : base(message, 1723)
		{
		}
	}
}
