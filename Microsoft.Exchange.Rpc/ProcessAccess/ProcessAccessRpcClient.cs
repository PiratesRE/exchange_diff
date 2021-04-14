using System;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.ProcessAccess
{
	internal class ProcessAccessRpcClient : RpcClientBase
	{
		public ProcessAccessRpcClient(string machineName, NetworkCredential nc) : base(machineName, nc)
		{
		}

		public ProcessAccessRpcClient(string machineName, ValueType clientObjectGuid) : base(machineName, clientObjectGuid)
		{
		}

		public ProcessAccessRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] RunProcessCommand(byte[] inBytes)
		{
			byte[] result = null;
			IntPtr hglobal = IntPtr.Zero;
			byte* ptr = null;
			int cBytes = 0;
			try
			{
				bool flag = false;
				do
				{
					try
					{
						int num = 0;
						hglobal = <Module>.MToUBytes(inBytes, &num);
						<Module>.cli_RunProcessCommand(base.BindingHandle, num, (byte*)hglobal.ToPointer(), &cBytes, &ptr);
						flag = false;
					}
					catch when (endfilter(true))
					{
						int exceptionCode = Marshal.GetExceptionCode();
						if (1727 == exceptionCode)
						{
							flag = (!flag || flag);
						}
						<Module>.Microsoft.Exchange.Rpc.ThrowRpcExceptionWithEEInfo(exceptionCode, "RunProcessCommand");
					}
				}
				while (flag);
				result = <Module>.UToMBytes(cBytes, ptr);
			}
			finally
			{
				Marshal.FreeHGlobal(hglobal);
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
			}
			return result;
		}

		public static int RpcServerTooBusy = 1723;
	}
}
