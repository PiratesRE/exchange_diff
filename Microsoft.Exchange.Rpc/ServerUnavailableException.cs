using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class ServerUnavailableException : RpcException
	{
		public ServerUnavailableException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ServerUnavailableException(string message) : base(message)
		{
			base.HResult = 1722;
		}
	}
}
