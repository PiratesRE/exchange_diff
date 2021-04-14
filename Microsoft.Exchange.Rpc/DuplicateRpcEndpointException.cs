using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class DuplicateRpcEndpointException : RpcException
	{
		public DuplicateRpcEndpointException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public DuplicateRpcEndpointException(string message) : base(message)
		{
			base.HResult = 1740;
		}
	}
}
