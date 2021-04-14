using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal interface IDriverFactory
	{
		IRopDriver CreateIRopDriver(IConnectionHandler connectionHandler, IConnectionInformation connectionInformation);
	}
}
