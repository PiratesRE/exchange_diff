using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfMdbSuccessAuxiliaryBlock : BasePerfMdbSuccessAuxiliaryBlock
	{
		public PerfMdbSuccessAuxiliaryBlock(ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest) : base(1, AuxiliaryBlockTypes.PerfMdbSuccess, 0, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToCompleteRequest)
		{
		}

		public PerfMdbSuccessAuxiliaryBlock(ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest) : base(2, AuxiliaryBlockTypes.PerfMdbSuccess, blockProcessId, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToCompleteRequest)
		{
		}

		internal PerfMdbSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}
	}
}
