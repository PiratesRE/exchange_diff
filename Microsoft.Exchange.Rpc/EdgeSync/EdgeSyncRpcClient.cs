using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.EdgeSync
{
	internal class EdgeSyncRpcClient : RpcClientBase
	{
		public EdgeSyncRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe StartResults StartSyncNow(string targetServer, [MarshalAs(UnmanagedType.U1)] bool forceFullSync, [MarshalAs(UnmanagedType.U1)] bool forceUpdateCookie)
		{
			int result = -1;
			ushort* ptr = null;
			try
			{
				if (targetServer != null)
				{
					ptr = <Module>.StringToUnmanaged(targetServer);
				}
				try
				{
					int num = forceUpdateCookie ? 1 : 0;
					int num2 = forceFullSync ? 1 : 0;
					int num3 = <Module>.cli_StartSyncNow(base.BindingHandle, ptr, num2, num, &result);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_StartSyncNow");
				}
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return (StartResults)result;
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] GetSyncNowResult(ref GetResultResults continueFlag)
		{
			byte* ptr = null;
			byte[] result = null;
			try
			{
				int num2;
				int cBytes;
				try
				{
					int num = <Module>.cli_GetSyncNowResult(base.BindingHandle, &num2, &cBytes, &ptr);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_GetSyncNowResult");
				}
				result = <Module>.UToMBytes(cBytes, ptr);
				continueFlag = (GetResultResults)num2;
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}
	}
}
