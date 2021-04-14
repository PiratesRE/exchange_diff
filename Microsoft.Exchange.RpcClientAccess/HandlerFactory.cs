using System;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal delegate IConnectionHandler HandlerFactory(IConnection connection);
}
