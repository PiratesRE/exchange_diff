using System;

namespace Microsoft.Exchange.Rpc.AdminRpc
{
	internal abstract class Admin40RpcServer : AdminRpcServerBase
	{
		public Admin40RpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = AdminRpcServerBase.Admin40IntfHandle;
	}
}
