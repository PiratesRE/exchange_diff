using System;

namespace Microsoft.Exchange.Rpc
{
	public abstract class RpcClientHelper
	{
		public static object Invoke(RpcClientHelper.InvokeRpc invokeRpc)
		{
			for (int i = 0; i < 3; i++)
			{
				try
				{
					return invokeRpc();
				}
				catch (RpcException)
				{
					if (i == 2)
					{
						throw;
					}
				}
			}
			return null;
		}

		public RpcClientHelper()
		{
		}

		private const int RetryCount = 3;

		public delegate object InvokeRpc();
	}
}
