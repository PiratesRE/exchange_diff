using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class BasePerfMdbSuccessAuxiliaryBlock : AuxiliaryBlock
	{
		protected BasePerfMdbSuccessAuxiliaryBlock(byte blockVersion, AuxiliaryBlockTypes blockType, ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest) : base(blockVersion, blockType)
		{
			if (blockType != AuxiliaryBlockTypes.PerfMdbSuccess && blockType != AuxiliaryBlockTypes.PerfBgMdbSuccess && blockType != AuxiliaryBlockTypes.PerfFgMdbSuccess)
			{
				throw new ArgumentException("Type must be either PerfMdbSuccess, PerfBgMdbSuccess or PerfFgMdbSuccess", "blockType");
			}
			if (blockVersion == 2)
			{
				this.blockProcessId = blockProcessId;
			}
			this.blockClientId = blockClientId;
			this.blockServerId = blockServerId;
			this.blockSessionId = blockSessionId;
			this.blockRequestId = blockRequestId;
			this.blockTimeSinceRequest = blockTimeSinceRequest;
			this.blockTimeToCompleteRequest = blockTimeToCompleteRequest;
		}

		protected BasePerfMdbSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
			if (base.Version == 2)
			{
				this.blockProcessId = reader.ReadUInt16();
			}
			this.blockClientId = reader.ReadUInt16();
			this.blockServerId = reader.ReadUInt16();
			this.blockSessionId = reader.ReadUInt16();
			this.blockRequestId = reader.ReadUInt16();
			if (base.Version == 2)
			{
				reader.ReadInt16();
			}
			this.blockTimeSinceRequest = reader.ReadUInt32();
			this.blockTimeToCompleteRequest = reader.ReadUInt32();
		}

		public ushort BlockProcessId
		{
			get
			{
				return this.blockProcessId;
			}
		}

		public ushort BlockClientId
		{
			get
			{
				return this.blockClientId;
			}
		}

		public ushort BlockServerId
		{
			get
			{
				return this.blockServerId;
			}
		}

		public ushort BlockSessionId
		{
			get
			{
				return this.blockSessionId;
			}
		}

		public ushort BlockRequestId
		{
			get
			{
				return this.blockRequestId;
			}
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
			if (base.Version == 2)
			{
				writer.WriteUInt16(this.blockProcessId);
			}
			writer.WriteUInt16(this.blockClientId);
			writer.WriteUInt16(this.blockServerId);
			writer.WriteUInt16(this.blockSessionId);
			writer.WriteUInt16(this.blockRequestId);
			if (base.Version == 2)
			{
				writer.WriteInt16(0);
			}
			writer.WriteUInt32(this.blockTimeSinceRequest);
			writer.WriteUInt32(this.blockTimeToCompleteRequest);
		}

		private readonly ushort blockProcessId;

		private readonly ushort blockClientId;

		private readonly ushort blockServerId;

		private readonly ushort blockSessionId;

		private readonly ushort blockRequestId;

		private readonly uint blockTimeSinceRequest;

		private readonly uint blockTimeToCompleteRequest;
	}
}
