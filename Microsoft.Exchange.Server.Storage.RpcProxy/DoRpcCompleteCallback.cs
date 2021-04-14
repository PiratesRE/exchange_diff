using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal delegate void DoRpcCompleteCallback(ErrorCode result, uint flags, ArraySegment<byte> response, ArraySegment<byte> auxOut);
}
