using System;
using Microsoft.Exchange.UM.Rpc;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal interface IVersionedRpcTarget : IRpcTarget
	{
		UMRpcResponse ExecuteRequest(UMVersionedRpcRequest request);
	}
}
