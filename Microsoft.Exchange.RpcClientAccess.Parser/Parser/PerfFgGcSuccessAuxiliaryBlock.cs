using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfFgGcSuccessAuxiliaryBlock : BasePerfGcSuccessAuxiliaryBlock
	{
		public PerfFgGcSuccessAuxiliaryBlock(ushort blockClientId, ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(1, AuxiliaryBlockTypes.PerfFgGcSuccess, 0, blockClientId, blockServerId, blockSessionId, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestOperation)
		{
		}

		public PerfFgGcSuccessAuxiliaryBlock(ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(2, AuxiliaryBlockTypes.PerfFgGcSuccess, blockProcessId, blockClientId, blockServerId, blockSessionId, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestOperation)
		{
		}

		internal PerfFgGcSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}
	}
}
