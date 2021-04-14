using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class InvalidHandleException : RpcException
	{
		public InvalidHandleException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public InvalidHandleException(string message) : base(message)
		{
			base.HResult = 6;
		}
	}
}
