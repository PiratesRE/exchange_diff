using System;

namespace Microsoft.Exchange.Rpc.AdminRpc
{
	internal abstract class Admin50RpcServer : AdminRpcServerBase
	{
		public Admin50RpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = AdminRpcServerBase.Admin50IntfHandle;
	}
}
