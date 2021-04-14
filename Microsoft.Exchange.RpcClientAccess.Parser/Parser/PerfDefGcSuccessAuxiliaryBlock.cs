using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfDefGcSuccessAuxiliaryBlock : BasePerfDefGcSuccessAuxiliaryBlock
	{
		public PerfDefGcSuccessAuxiliaryBlock(ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(AuxiliaryBlockTypes.PerfDefGcSuccess, blockServerId, blockSessionId, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestOperation)
		{
		}

		internal PerfDefGcSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}
	}
}
