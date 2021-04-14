using System;

namespace Microsoft.Exchange.Rpc.AdminRpc
{
	internal abstract class Admin20RpcServer : AdminRpcServerBase
	{
		public Admin20RpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = AdminRpcServerBase.Admin20IntfHandle;
	}
}
