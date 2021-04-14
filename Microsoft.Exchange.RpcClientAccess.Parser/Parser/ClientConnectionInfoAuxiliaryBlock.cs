using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ClientConnectionInfoAuxiliaryBlock : AuxiliaryBlock
	{
		public ClientConnectionInfoAuxiliaryBlock(Guid connectionGuid, uint connectionAttempts, uint connectionFlags, string connectionContextInfo) : base(1, AuxiliaryBlockTypes.ClientConnectionInfo)
		{
			this.connectionGuid = connectionGuid;
			this.connectionAttempts = connectionAttempts;
			this.connectionFlags = connectionFlags;
			this.connectionContextInfo = connectionContextInfo;
		}

		internal ClientConnectionInfoAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.connectionGuid = reader.ReadGuid();
			ushort offset = reader.ReadUInt16();
			reader.ReadUInt16();
			this.connectionAttempts = reader.ReadUInt32();
			this.connectionFlags = reader.ReadUInt32();
			this.connectionContextInfo = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset);
		}

		public Guid ConnectionGuid
		{
			get
			{
				return this.connectionGuid;
			}
		}

		public string ConnectionContextInfo
		{
			get
			{
				return this.connectionContextInfo;
			}
		}

		public uint ConnectionAttempts
		{
			get
			{
				return this.connectionAttempts;
			}
		}

		public uint ConnectionFlags
		{
			get
			{
				return this.connectionFlags;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteGuid(this.connectionGuid);
			long position = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt16(0);
			writer.WriteUInt32(this.connectionAttempts);
			writer.WriteUInt32(this.connectionFlags);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.connectionContextInfo, position);
		}

		private readonly string connectionContextInfo;

		private readonly Guid connectionGuid;

		private readonly uint connectionAttempts;

		private readonly uint connectionFlags;
	}
}
