using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class BasePerfFailureAuxiliaryBlock : AuxiliaryBlock
	{
		protected BasePerfFailureAuxiliaryBlock(byte blockVersion, AuxiliaryBlockTypes blockType, ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, ushort blockRequestId, uint blockTimeSinceRequest, uint blockTimeToFailRequest, uint blockResultCode, byte blockRequestOperation) : base(blockVersion, blockType)
		{
			if (blockType != AuxiliaryBlockTypes.PerfFailure && blockType != AuxiliaryBlockTypes.PerfBgFailure && blockType != AuxiliaryBlockTypes.PerfFgFailure)
			{
				throw new ArgumentException("Type must be PerfFailure, PerfBgFailure or PerfFgFailure", "blockType");
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
			this.blockTimeToFailRequest = blockTimeToFailRequest;
			this.blockResultCode = blockResultCode;
			this.blockRequestOperation = blockRequestOperation;
		}

		internal BasePerfFailureAuxiliaryBlock(Reader reader) : base(reader)
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
			this.blockTimeToFailRequest = reader.ReadUInt32();
			this.blockResultCode = reader.ReadUInt32();
			this.blockRequestOperation = reader.ReadByte();
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

		public uint BlockTimeToFailRequest
		{
			get
			{
				return this.blockTimeToFailRequest;
			}
		}

		public uint BlockResultCode
		{
			get
			{
				return this.blockResultCode;
			}
		}

		public byte BlockRequestOperation
		{
			get
			{
				return this.blockRequestOperation;
			}
		}

		protected internal override void ReportClientPerformance(IClientPerformanceDataSink sink)
		{
			sink.ReportEvent(new ClientTimeStampedEventArgs(this.blockTimeSinceRequest, ClientPerformanceEventType.RpcAttempted));
			sink.ReportEvent(new ClientFailureEventArgs(this.blockTimeSinceRequest, ClientPerformanceEventType.RpcFailed, this.blockResultCode));
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
			writer.WriteUInt32(this.blockTimeToFailRequest);
			writer.WriteUInt32(this.blockResultCode);
			writer.WriteByte(this.blockRequestOperation);
			writer.WriteByte(0);
			writer.WriteInt16(0);
		}

		private readonly ushort blockProcessId;

		private readonly ushort blockClientId;

		private readonly ushort blockServerId;

		private readonly ushort blockSessionId;

		private readonly ushort blockRequestId;

		private readonly uint blockTimeSinceRequest;

		private readonly uint blockTimeToFailRequest;

		private readonly uint blockResultCode;

		private readonly byte blockRequestOperation;
	}
}
