using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc
{
	[Serializable]
	internal class FailRpcException : Exception
	{
		public int ErrorCode
		{
			get
			{
				return base.HResult;
			}
		}

		public FailRpcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public FailRpcException(string message, int hr, Exception innerException) : base(message, innerException)
		{
			base.HResult = hr;
		}

		public FailRpcException(string message, int hr) : base(message)
		{
			base.HResult = hr;
		}

		public FailRpcException(string message) : base(message)
		{
		}
	}
}
