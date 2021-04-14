using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	[Serializable]
	internal class RpcNoEndPointException : RpcConnectionException
	{
		public RpcNoEndPointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RpcNoEndPointException(string message) : base(message, 1753)
		{
		}
	}
}
