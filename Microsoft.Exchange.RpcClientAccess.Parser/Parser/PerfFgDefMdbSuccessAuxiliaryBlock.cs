using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfFgDefMdbSuccessAuxiliaryBlock : BasePerfDefMdbSuccessAuxiliaryBlock
	{
		public PerfFgDefMdbSuccessAuxiliaryBlock(uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, ushort blockRequestId) : base(AuxiliaryBlockTypes.PerfFgDefMdbSuccess, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestId)
		{
		}

		internal PerfFgDefMdbSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}

		protected internal override void ReportClientPerformance(IClientPerformanceDataSink sink)
		{
			sink.ReportEvent(new ClientPerformanceEventArgs(ClientPerformanceEventType.ForegroundRpcSucceeded));
			base.ReportClientPerformance(sink);
		}
	}
}
