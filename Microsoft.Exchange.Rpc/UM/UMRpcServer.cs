using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.UM
{
	internal abstract class UMRpcServer : RpcServerBase
	{
		public abstract byte[] GetUmActiveCalls([MarshalAs(UnmanagedType.U1)] bool isDialPlan, string dialPlan, [MarshalAs(UnmanagedType.U1)] bool isIpGateway, string ipGateway);

		public UMRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IUM_v2_0_s_ifspec;
	}
}
