using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class PerfClientInfoAuxiliaryBlock : AuxiliaryBlock
	{
		internal PerfClientInfoAuxiliaryBlock(uint blockAdapterSpeed, ushort blockClientId, string blockMachineName, string blockUserName, ArraySegment<byte> blockClientIp, ArraySegment<byte> blockClientIpMask, string blockAdapterName, ArraySegment<byte> blockMacAddress, ClientMode blockClientMode) : base(1, AuxiliaryBlockTypes.PerfClientInfo)
		{
			this.adapterSpeed = blockAdapterSpeed;
			this.clientId = blockClientId;
			this.machineName = blockMachineName;
			this.userName = blockUserName;
			this.clientIp = blockClientIp;
			this.clientIpMask = blockClientIpMask;
			this.adapterName = blockAdapterName;
			this.macAddress = blockMacAddress;
			this.clientMode = blockClientMode;
		}

		internal PerfClientInfoAuxiliaryBlock(Reader reader) : base(reader)
		{
			this.adapterSpeed = reader.ReadUInt32();
			this.clientId = reader.ReadUInt16();
			ushort offset = reader.ReadUInt16();
			ushort offset2 = reader.ReadUInt16();
			ushort count = reader.ReadUInt16();
			ushort offset3 = reader.ReadUInt16();
			ushort count2 = reader.ReadUInt16();
			ushort offset4 = reader.ReadUInt16();
			ushort offset5 = reader.ReadUInt16();
			ushort count3 = reader.ReadUInt16();
			ushort offset6 = reader.ReadUInt16();
			this.clientMode = (ClientMode)reader.ReadUInt16();
			this.machineName = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset);
			this.userName = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset2);
			this.clientIp = AuxiliaryBlock.ReadBytesAtPosition(reader, offset3, count);
			this.clientIpMask = AuxiliaryBlock.ReadBytesAtPosition(reader, offset4, count2);
			this.adapterName = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset5);
			this.macAddress = AuxiliaryBlock.ReadBytesAtPosition(reader, offset6, count3);
		}

		public ClientMode ClientMode
		{
			get
			{
				return this.clientMode;
			}
		}

		public string MachineName
		{
			get
			{
				return this.machineName;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32(this.adapterSpeed);
			writer.WriteUInt16(this.clientId);
			long position = writer.Position;
			writer.WriteUInt16(0);
			long position2 = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt16((ushort)this.clientIp.Count);
			long position3 = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt16((ushort)this.clientIpMask.Count);
			long position4 = writer.Position;
			writer.WriteUInt16(0);
			long position5 = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt16((ushort)this.macAddress.Count);
			long position6 = writer.Position;
			writer.WriteUInt16(0);
			writer.WriteUInt16((ushort)this.clientMode);
			writer.WriteUInt16(0);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.machineName, position);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.userName, position2);
			AuxiliaryBlock.WriteBytesAndUpdateOffset(writer, this.clientIp, position3);
			AuxiliaryBlock.WriteBytesAndUpdateOffset(writer, this.clientIpMask, position4);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.adapterName, position5);
			AuxiliaryBlock.WriteBytesAndUpdateOffset(writer, this.macAddress, position6);
		}

		private readonly uint adapterSpeed;

		private readonly ushort clientId;

		private readonly string machineName;

		private readonly string userName;

		private readonly ArraySegment<byte> clientIp;

		private readonly ArraySegment<byte> clientIpMask;

		private readonly string adapterName;

		private readonly ArraySegment<byte> macAddress;

		private readonly ClientMode clientMode;
	}
}
