using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class UMRpcClient : RpcClientBase
	{
		public UMRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] GetUmActiveCalls([MarshalAs(UnmanagedType.U1)] bool isDialPlan, string dialPlan, [MarshalAs(UnmanagedType.U1)] bool isIpGateway, string ipGateway)
		{
			IntPtr intPtr = IntPtr.Zero;
			IntPtr intPtr2 = IntPtr.Zero;
			byte[] result = null;
			byte* ptr = null;
			try
			{
				intPtr = Marshal.StringToHGlobalUni((!(dialPlan == null)) ? dialPlan : string.Empty);
				intPtr2 = Marshal.StringToHGlobalUni((!(ipGateway == null)) ? ipGateway : string.Empty);
				int num = isIpGateway ? 1 : 0;
				int num2 = isDialPlan ? 1 : 0;
				int cBytes;
				<Module>.cli_GetUmActiveCalls(base.BindingHandle, num2, (ushort*)intPtr.ToPointer(), num, (ushort*)intPtr2.ToPointer(), &cBytes, &ptr);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "GetUmActiveCalls");
			}
			finally
			{
				if (IntPtr.Zero != intPtr)
				{
					Marshal.FreeHGlobal(intPtr);
				}
				if (IntPtr.Zero != intPtr2)
				{
					Marshal.FreeHGlobal(intPtr2);
				}
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}
	}
}
