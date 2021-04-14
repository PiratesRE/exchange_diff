using System;

namespace Microsoft.Exchange.Rpc.UM
{
	internal abstract class UMServerPingRpcServerBase : RpcServerBase
	{
		public abstract int Ping(Guid dialPlanGuid, ref bool availableToTakeCalls);

		public UMServerPingRpcServerBase()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IUMServerHealth_v1_0_s_ifspec;
	}
}
