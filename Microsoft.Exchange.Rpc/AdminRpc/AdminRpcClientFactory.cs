using System;

namespace Microsoft.Exchange.Rpc.AdminRpc
{
	public class AdminRpcClientFactory
	{
		public static object CreateLocal()
		{
			return new AdminRpcClient("localhost", null);
		}
	}
}
