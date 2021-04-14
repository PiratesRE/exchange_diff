using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.EdgeSync
{
	internal abstract class EdgeSyncRpcServer : RpcServerBase
	{
		public abstract StartResults StartSyncNow(string targetServer, [MarshalAs(UnmanagedType.U1)] bool forceFullSync, [MarshalAs(UnmanagedType.U1)] bool forceUpdateCookie);

		public abstract byte[] GetSyncNowResult(ref GetResultResults continueFlag);

		public EdgeSyncRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.ISyncNow_v1_0_s_ifspec;
	}
}
