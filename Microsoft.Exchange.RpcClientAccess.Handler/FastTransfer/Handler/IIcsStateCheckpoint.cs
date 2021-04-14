using System;
using Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Handler
{
	internal interface IIcsStateCheckpoint
	{
		IFastTransferProcessor<FastTransferDownloadContext> CreateIcsStateCheckpointFastTransferObject();
	}
}
