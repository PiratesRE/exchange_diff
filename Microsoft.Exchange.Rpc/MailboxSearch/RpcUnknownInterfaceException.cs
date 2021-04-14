using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	[Serializable]
	internal class RpcUnknownInterfaceException : RpcConnectionException
	{
		public RpcUnknownInterfaceException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RpcUnknownInterfaceException(string message) : base(message, 1717)
		{
		}
	}
}
