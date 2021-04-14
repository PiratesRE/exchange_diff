using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class DriverFactory : IDriverFactory
	{
		public IRopDriver CreateIRopDriver(IConnectionHandler connectionHandler, IConnectionInformation connectionInformation)
		{
			return new RopDriver(connectionHandler, connectionInformation);
		}
	}
}
