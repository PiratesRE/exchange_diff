using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class BasePerfDefGcSuccessAuxiliaryBlock : AuxiliaryBlock
	{
		protected BasePerfDefGcSuccessAuxiliaryBlock(AuxiliaryBlockTypes blockType, ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(1, blockType)
		{
			if (blockType != AuxiliaryBlockTypes.PerfDefGcSuccess && blockType != AuxiliaryBlockTypes.PerfBgDefGcSuccess && blockType != AuxiliaryBlockTypes.PerfFgDefGcSuccess)
			{
				throw new ArgumentException("Type must be either PerfDefGcSuccess, PerfBgDefGcSuccess or PerfFgDefGcSuccess", "blockType");
			}
			this.blockServerId = blockServerId;
			this.blockSessionId = blockSessionId;
			this.blockTimeSinceRequest = blockTimeSinceRequest;
			this.blockTimeToCompleteRequest = blockTimeToCompleteRequest;
			this.blockRequestOperation = blockRequestOperation;
		}

		internal BasePerfDefGcSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.blockServerId = reader.ReadUInt16();
			this.blockSessionId = reader.ReadUInt16();
			this.blockTimeSinceRequest = reader.ReadUInt32();
			this.blockTimeToCompleteRequest = reader.ReadUInt32();
			this.blockRequestOperation = reader.ReadByte();
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

		public byte BlockRequestOperation
		{
			get
			{
				return this.blockRequestOperation;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt16(this.blockServerId);
			writer.WriteUInt16(this.blockSessionId);
			writer.WriteUInt32(this.blockTimeSinceRequest);
			writer.WriteUInt32(this.blockTimeToCompleteRequest);
			writer.WriteByte(this.blockRequestOperation);
			writer.WriteByte(0);
			writer.WriteInt16(0);
		}

		private readonly ushort blockServerId;

		private readonly ushort blockSessionId;

		private readonly uint blockTimeSinceRequest;

		private readonly uint blockTimeToCompleteRequest;

		private readonly byte blockRequestOperation;
	}
}
