using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfFgMdbSuccessAuxiliaryBlock : BasePerfMdbSuccessAuxiliaryBlock
	{
		public PerfFgMdbSuccessAuxiliaryBlock(ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest) : base(1, AuxiliaryBlockTypes.PerfFgMdbSuccess, 0, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToCompleteRequest)
		{
		}

		public PerfFgMdbSuccessAuxiliaryBlock(ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest) : base(2, AuxiliaryBlockTypes.PerfFgMdbSuccess, blockProcessId, blockClientId, blockServerId, blockSessionId, blockRequestId, blockTimeSinceRequest, blockTimeToCompleteRequest)
		{
		}

		internal PerfFgMdbSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}

		protected internal override void ReportClientPerformance(IClientPerformanceDataSink sink)
		{
			sink.ReportEvent(new ClientPerformanceEventArgs(ClientPerformanceEventType.ForegroundRpcSucceeded));
			base.ReportClientPerformance(sink);
		}
	}
}
