using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfGcSuccessAuxiliaryBlock : BasePerfGcSuccessAuxiliaryBlock
	{
		public PerfGcSuccessAuxiliaryBlock(ushort blockClientId, ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(1, AuxiliaryBlockTypes.PerfGcSuccess, 0, blockClientId, blockServerId, blockSessionId, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestOperation)
		{
		}

		public PerfGcSuccessAuxiliaryBlock(ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(2, AuxiliaryBlockTypes.PerfGcSuccess, blockProcessId, blockClientId, blockServerId, blockSessionId, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestOperation)
		{
		}

		internal PerfGcSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}
	}
}
