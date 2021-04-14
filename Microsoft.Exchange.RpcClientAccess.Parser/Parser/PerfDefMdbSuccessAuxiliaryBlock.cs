using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfDefMdbSuccessAuxiliaryBlock : BasePerfDefMdbSuccessAuxiliaryBlock
	{
		public PerfDefMdbSuccessAuxiliaryBlock(uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, ushort blockRequestId) : base(AuxiliaryBlockTypes.PerfDefMdbSuccess, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestId)
		{
		}

		internal PerfDefMdbSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}
	}
}
