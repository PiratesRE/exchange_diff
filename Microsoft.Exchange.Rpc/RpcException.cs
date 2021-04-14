using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class RpcException : Exception
	{
		public int ErrorCode
		{
			get
			{
				return base.HResult;
			}
		}

		public RpcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public RpcException(string message, int hr, Exception innerException) : base(message, innerException)
		{
			base.HResult = hr;
		}

		public RpcException(string message, int hr) : base(message)
		{
			base.HResult = hr;
		}

		public RpcException(string message) : base(message)
		{
		}
	}
}
