using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal interface IRpcServiceManager
	{
		void StopService();

		void AddTcpPort(string port);

		void AddHttpPort(string port);

		void EnableLrpc();

		void AddServer(Action starter, Action stopper);
	}
}
