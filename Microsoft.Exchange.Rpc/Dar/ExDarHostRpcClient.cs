using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.Dar
{
	internal class ExDarHostRpcClient : RpcClientBase
	{
		public ExDarHostRpcClient(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe byte[] SendHostRequest(int version, int type, byte[] inputParameterBytes)
		{
			int num = 0;
			byte* ptr = <Module>.MToUBytesClient(inputParameterBytes, &num);
			int num2 = 0;
			byte* ptr2 = null;
			byte[] result = null;
			try
			{
				int num3 = <Module>.cli_SendHostRequest(base.BindingHandle, version, type, num, ptr, &num2, &ptr2);
				if (num2 > 0)
				{
					result = <Module>.UToMBytes(num2, ptr2);
				}
			}
			catch when (endfilter(true))
			{
				RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "cli_SendHostRequest");
			}
			finally
			{
				if (ptr != null)
				{
					<Module>.MIDL_user_free((void*)ptr);
				}
				if (ptr2 != null)
				{
					<Module>.MIDL_user_free((void*)ptr2);
				}
			}
			return result;
		}
	}
}
