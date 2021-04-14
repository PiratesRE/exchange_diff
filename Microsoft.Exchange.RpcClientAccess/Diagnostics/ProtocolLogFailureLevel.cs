using System;

namespace Microsoft.Exchange.RpcClientAccess.Diagnostics
{
	internal enum ProtocolLogFailureLevel
	{
		RopHandler,
		RpcDispatch,
		RpcEndPoint,
		Watson,
		WebServiceEndPoint,
		MapiHttpEndPoint
	}
}
