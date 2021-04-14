using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class BasePerfDefMdbSuccessAuxiliaryBlock : AuxiliaryBlock
	{
		protected BasePerfDefMdbSuccessAuxiliaryBlock(AuxiliaryBlockTypes blockType, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, ushort blockRequestId) : base(1, blockType)
		{
			if (blockType != AuxiliaryBlockTypes.PerfDefMdbSuccess && blockType != AuxiliaryBlockTypes.PerfBgDefMdbSuccess && blockType != AuxiliaryBlockTypes.PerfFgDefMdbSuccess)
			{
				throw new ArgumentException("Type must be PerfDefMdbSuccess, PerfBgDefMdbSuccess or PerfFgDefMdbSuccess", "blockType");
			}
			this.blockTimeSinceRequest = blockTimeSinceRequest;
			this.blockTimeToCompleteRequest = blockTimeToCompleteRequest;
			this.blockRequestId = blockRequestId;
		}

		internal BasePerfDefMdbSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.blockTimeSinceRequest = reader.ReadUInt32();
			this.blockTimeToCompleteRequest = reader.ReadUInt32();
			this.blockRequestId = reader.ReadUInt16();
		}

		public uint BlockTimeSinceRequest
		{
			get
			{
				return this.blockTimeSinceRequest;
			}
		}

		public uint BlockTimeToCompleteRequest
		{
			get
			{
				return this.blockTimeToCompleteRequest;
			}
		}

		public ushort BlockRequestId
		{
			get
			{
				return this.blockRequestId;
			}
		}

		protected internal override void ReportClientPerformance(IClientPerformanceDataSink sink)
		{
			sink.ReportEvent(new ClientTimeStampedEventArgs(this.blockTimeSinceRequest, ClientPerformanceEventType.RpcAttempted));
			sink.ReportEvent(new ClientPerformanceEventArgs(ClientPerformanceEventType.RpcSucceeded));
			sink.ReportLatency(TimeSpan.FromMilliseconds(this.blockTimeToCompleteRequest));
			base.ReportClientPerformance(sink);
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.blockTimeSinceRequest);
			writer.WriteUInt32(this.blockTimeToCompleteRequest);
			writer.WriteUInt16(this.blockRequestId);
			writer.WriteInt16(0);
		}

		private readonly uint blockTimeSinceRequest;

		private readonly uint blockTimeToCompleteRequest;

		private readonly ushort blockRequestId;
	}
}
