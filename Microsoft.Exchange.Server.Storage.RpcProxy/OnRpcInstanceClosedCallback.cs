using System;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal delegate void OnRpcInstanceClosedCallback(Guid instanceId, int generation);
}
