using System;

namespace Microsoft.Exchange.Rpc.MailboxSearch
{
	internal class RpcExceptionHelper
	{
		public static void ThrowRpcException(int status, string message)
		{
			if (status == 1717)
			{
				throw new RpcUnknownInterfaceException(message);
			}
			if (status == 1722)
			{
				throw new RpcServerUnavailableException(message);
			}
			if (status == 1723)
			{
				throw new RpcServerTooBusyException(message);
			}
			if (status == 1727)
			{
				throw new RpcFailedException(message);
			}
			if (status != 1753)
			{
				throw new RpcException(message, status);
			}
			throw new RpcNoEndPointException(message);
		}
	}
}
