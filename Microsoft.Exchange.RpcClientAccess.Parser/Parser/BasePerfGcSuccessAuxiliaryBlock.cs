using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class BasePerfGcSuccessAuxiliaryBlock : AuxiliaryBlock
	{
		protected BasePerfGcSuccessAuxiliaryBlock(byte blockVersion, AuxiliaryBlockTypes blockType, ushort blockProcessId, ushort blockClientId, ushort blockServerId, ushort blockSessionId, uint blockTimeSinceRequest, uint blockTimeToCompleteRequest, byte blockRequestOperation) : base(blockVersion, blockType)
		{
			if (blockType != AuxiliaryBlockTypes.PerfGcSuccess && blockType != AuxiliaryBlockTypes.PerfBgGcSuccess && blockType != AuxiliaryBlockTypes.PerfFgGcSuccess)
			{
				throw new ArgumentException("Type must be either PerfGcSuccess, PerfBgGcSuccess or PerfFgGcSuccess", "blockType");
			}
			if (blockVersion == 2)
			{
				this.blockProcessId = blockProcessId;
			}
			this.blockClientId = blockClientId;
			this.blockServerId = blockServerId;
			this.blockSessionId = blockSessionId;
			this.blockTimeSinceRequest = blockTimeSinceRequest;
			this.blockTimeToCompleteRequest = blockTimeToCompleteRequest;
			this.blockRequestOperation = blockRequestOperation;
		}

		protected BasePerfGcSuccessAuxiliaryBlock(Reader reader) : base(reader)
		{
			if (base.Version == 2)
			{
				this.blockProcessId = reader.ReadUInt16();
			}
			this.blockClientId = reader.ReadUInt16();
			this.blockServerId = reader.ReadUInt16();
			this.blockSessionId = reader.ReadUInt16();
			if (base.Version == 1)
			{
				reader.ReadInt16();
			}
			this.blockTimeSinceRequest = reader.ReadUInt32();
			this.blockTimeToCompleteRequest = reader.ReadUInt32();
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
			if (base.Version == 2)
			{
				writer.WriteUInt16(this.blockProcessId);
			}
			writer.WriteUInt16(this.blockClientId);
			writer.WriteUInt16(this.blockServerId);
			writer.WriteUInt16(this.blockSessionId);
			if (base.Version == 1)
			{
				writer.WriteInt16(0);
			}
			writer.WriteUInt32(this.blockTimeSinceRequest);
			writer.WriteUInt32(this.blockTimeToCompleteRequest);
			writer.WriteByte(this.blockRequestOperation);
			writer.WriteByte(0);
			writer.WriteInt16(0);
		}

		private readonly ushort blockProcessId;

		private readonly ushort blockClientId;

		private readonly ushort blockServerId;

		private readonly ushort blockSessionId;

		private readonly uint blockTimeSinceRequest;

		private readonly uint blockTimeToCompleteRequest;

		private readonly byte blockRequestOperation;
	}
}
