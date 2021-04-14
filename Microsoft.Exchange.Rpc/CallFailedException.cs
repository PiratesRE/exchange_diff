using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class CallFailedException : RpcException
	{
		public CallFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public CallFailedException(string message) : base(message)
		{
			base.HResult = 1726;
		}
	}
}
