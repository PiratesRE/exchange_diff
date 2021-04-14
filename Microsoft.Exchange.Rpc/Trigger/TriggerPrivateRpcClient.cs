using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Trigger
{
	internal class TriggerPrivateRpcClient : RpcClientBase
	{
		public TriggerPrivateRpcClient(string machineName)
		{
			this.m_machineName = machineName;
			base..ctor(machineName, null, null, AuthenticationService.Negotiate, <Module>.cli_TriggerPrivateRPC_v6_0_c_ifspec, true);
		}

		public TriggerPrivateRpcClient()
		{
			this.m_machineName = null;
			base..ctor(null, null, null, AuthenticationService.Negotiate, <Module>.cli_TriggerPrivateRPC_v6_0_c_ifspec, true);
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void RunOabGenTask(string oabDN)
		{
			IntPtr hglobal = Marshal.StringToHGlobalUni(oabDN);
			try
			{
				int num = <Module>.cli_ScRunOffLineABTask(base.BindingHandle, (ushort*)hglobal.ToPointer());
				if (num != null)
				{
					Marshal.ThrowExceptionForHR(<Module>.HrFromSc(num));
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "RunOabGenTask");
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
			}
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe void CreateExchangeOabFolder()
		{
			IntPtr hglobal = Marshal.StringToHGlobalUni(this.m_machineName);
			try
			{
				int num = <Module>.cli_ScCreateExchangeOabFolder(base.BindingHandle, (ushort*)hglobal.ToPointer());
				if (num != null)
				{
					Marshal.ThrowExceptionForHR(<Module>.HrFromSc(num));
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "CreateExchangeOabFolder");
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
			}
		}

		private string m_machineName;
	}
}
