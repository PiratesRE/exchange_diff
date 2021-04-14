using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class UMServerPingRpcClientBase : RpcClientBase
	{
		public UMServerPingRpcClientBase(string machineName) : base(machineName)
		{
		}

		[HandleProcessCorruptedStateExceptions]
		public unsafe virtual void Ping(Guid dialPlanGuid, ref bool availableToTakeCalls)
		{
			int num = 0;
			try
			{
				int num2;
				try
				{
					_GUID guid = <Module>.ToGUID(ref dialPlanGuid);
					num = <Module>.cli_Ping(base.BindingHandle, guid, &num2);
				}
				catch when (endfilter(true))
				{
					RpcClientBase.ThrowRpcException(Marshal.GetExceptionCode(), "Ping");
				}
				if (num < 0)
				{
					RpcClientBase.ThrowRpcException(num, "Ping");
				}
				int num3 = (num2 == 1) ? 1 : 0;
				availableToTakeCalls = (num3 != 0);
			}
			finally
			{
			}
		}
	}
}
