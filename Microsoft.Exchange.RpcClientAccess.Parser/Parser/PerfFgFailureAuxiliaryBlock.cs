using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfFgFailureAuxiliaryBlock : BasePerfFailureAuxiliaryBlock
	{
		public PerfFgFailureAuxiliaryBlock(ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToFailRequest, uint blockResultCode, byte blockRequestOperation) : base(1, AuxiliaryBlockTypes.PerfFgFailure, 0, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToFailRequest, blockResultCode, blockRequestOperation)
		{
		}

		public PerfFgFailureAuxiliaryBlock(ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToFailRequest, uint blockResultCode, byte blockRequestOperation) : base(2, AuxiliaryBlockTypes.PerfFgFailure, blockProcessId, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToFailRequest, blockResultCode, blockRequestOperation)
		{
		}

		internal PerfFgFailureAuxiliaryBlock(Reader reader) : base(reader)
		{
		}

		protected internal override void ReportClientPerformance(IClientPerformanceDataSink sink)
		{
			sink.ReportEvent(new ClientPerformanceEventArgs(ClientPerformanceEventType.ForegroundRpcFailed));
			base.ReportClientPerformance(sink);
		}
	}
}
