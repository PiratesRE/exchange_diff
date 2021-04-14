using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal static class ReplayRpcErrorCode
	{
		[return: MarshalAs(UnmanagedType.U1)]
		public static bool IsRpcConnectionError(int errorCode)
		{
			return errorCode == 1722 || errorCode == 1753 || errorCode == 1717 || errorCode == 1723 || errorCode == 1726 || errorCode == 1727;
		}

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool IsRpcTimeoutError(int errorCode)
		{
			return errorCode == 1818;
		}
	}
}
