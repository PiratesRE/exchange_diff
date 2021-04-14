using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class CallCancelledException : RpcException
	{
		public CallCancelledException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public CallCancelledException(string message) : base(message)
		{
			base.HResult = 1818;
		}
	}
}
