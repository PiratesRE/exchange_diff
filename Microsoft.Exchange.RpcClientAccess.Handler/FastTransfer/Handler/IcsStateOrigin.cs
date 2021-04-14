using System;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal enum IcsStateOrigin
	{
		None,
		ClientInitial,
		ServerIncremental,
		ServerFinal
	}
}
