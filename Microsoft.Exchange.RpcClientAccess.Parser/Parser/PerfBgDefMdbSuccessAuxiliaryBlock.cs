using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfBgDefMdbSuccessAuxiliaryBlock : BasePerfDefMdbSuccessAuxiliaryBlock
	{
		public PerfBgDefMdbSuccessAuxiliaryBlock(uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, ushort blockRequestId) : base(AuxiliaryBlockTypes.PerfBgDefMdbSuccess, blockTimeSinceRequest, blockTimeToCompleteRequest, blockRequestId)
		{
		}

		internal PerfBgDefMdbSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
		}

		protected internal override void ReportClientPerformance(IClientPerformanceDataSink sink)
		{
			sink.ReportEvent(new ClientPerformanceEventArgs(ClientPerformanceEventType.BackgroundRpcSucceeded));
			base.ReportClientPerformance(sink);
		}
	}
}
