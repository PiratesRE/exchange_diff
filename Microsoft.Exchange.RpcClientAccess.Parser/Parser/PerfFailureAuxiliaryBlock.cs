using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfFailureAuxiliaryBlock : BasePerfFailureAuxiliaryBlock
	{
		public PerfFailureAuxiliaryBlock(ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToFailRequest, uint blockResultCode, byte blockRequestOperation) : base(1, AuxiliaryBlockTypes.PerfFailure, 0, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToFailRequest, blockResultCode, blockRequestOperation)
		{
		}

		public PerfFailureAuxiliaryBlock(ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToFailRequest, uint blockResultCode, byte blockRequestOperation) : base(2, AuxiliaryBlockTypes.PerfFailure, blockProcessId, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToFailRequest, blockResultCode, blockRequestOperation)
		{
		}

		internal PerfFailureAuxiliaryBlock(Reader reader) : base(reader)
		{
		}
	}
}
