using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class RpcServiceException : Exception
	{
		protected RpcServiceException(string message, int status, Exception innerException) : base(message, innerException)
		{
			this.RpcStatus = status;
		}

		public int RpcStatus { get; private set; }

		public static uint GetHResultFromStatusCode(int statusCode)
		{
			return (uint)(statusCode + -2147024896);
		}

		public const int RPC_S_OUT_OF_MEMORY = 14;

		public const int RPC_S_INVALID_ARG = 87;

		public const int RPC_S_INVALID_BINDING = 1702;

		public const int RPC_S_SERVER_UNAVAILABLE = 1722;

		public const int RPC_S_SERVER_TOO_BUSY = 1723;

		public const int RPC_S_CALL_FAILED = 1726;

		public const int RPC_S_CALL_FAILED_DNE = 1727;
	}
}
