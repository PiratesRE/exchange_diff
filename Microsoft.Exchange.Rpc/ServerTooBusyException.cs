using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class ServerTooBusyException : RpcException
	{
		public ServerTooBusyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ServerTooBusyException(string message) : base(message)
		{
			base.HResult = 1723;
		}
	}
}
