using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Rpc.Cluster
{
	internal class ReplayRpcException : RpcException
	{
		public ReplayRpcException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public ReplayRpcException(string message, int hr) : base(message, hr)
		{
		}

		public ReplayRpcException(string message) : base(message)
		{
		}

		public static bool DowngradeOOMErrors
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return ReplayRpcException.downgradeOOM;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				ReplayRpcException.downgradeOOM = value;
			}
		}

		public static void ThrowRpcException(int status, string routineName)
		{
			UncaughtClrOverRpcBarrierException.ThrowIfNecessary(status, routineName);
			if (status == 14 && ReplayRpcException.DowngradeOOMErrors)
			{
				throw new ReplayRpcException(string.Format("Error 0x{0:x} mapped to 0x{1:x} RPC_S_OUT_OF_RESOURCES from {2}", 14, 1721, routineName), 1721);
			}
			throw new ReplayRpcException(string.Format("Error 0x{0:x} ({2}) from {1}", status, routineName, new Win32Exception(status).Message), status);
		}

		private static bool downgradeOOM = true;
	}
}
