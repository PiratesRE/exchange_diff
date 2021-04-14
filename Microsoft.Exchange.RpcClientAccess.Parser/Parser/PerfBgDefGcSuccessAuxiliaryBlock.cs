using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfBgDefGcSuccessAuxiliaryBlock : BasePerfDefGcSuccessAuxiliaryBlock
	{
		public PerfBgDefGcSuccessAuxiliaryBlock(ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(AuxiliaryBlockTypes.PerfBgDefGcSuccess, blockServerId, blockSessionId, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestOperation)
		{
		}

		internal PerfBgDefGcSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}
	}
}
