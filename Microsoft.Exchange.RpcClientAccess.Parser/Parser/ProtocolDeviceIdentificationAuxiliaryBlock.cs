using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class ProtocolDeviceIdentificationAuxiliaryBlock : AuxiliaryBlock
	{
		internal ProtocolDeviceIdentificationAuxiliaryBlock(string manufacturer, string model, string serialNumber, string deviceVersion, string firmwareVersion) : base(1, AuxiliaryBlockTypes.ProtocolDeviceIdentification)
		{
			this.manufacturer = manufacturer;
			this.model = model;
			this.serialNumber = serialNumber;
			this.deviceVersion = deviceVersion;
			this.firmwareVersion = firmwareVersion;
		}

		internal ProtocolDeviceIdentificationAuxiliaryBlock(Reader reader) : base(reader)
		{
			ushort offset = reader.ReadUInt16();
			ushort offset2 = reader.ReadUInt16();
			ushort offset3 = reader.ReadUInt16();
			ushort offset4 = reader.ReadUInt16();
			ushort offset5 = reader.ReadUInt16();
			this.manufacturer = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset);
			this.model = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset2);
			this.serialNumber = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset3);
			this.deviceVersion = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset4);
			this.firmwareVersion = AuxiliaryBlock.ReadUnicodeStringAtPosition(reader, offset5);
		}

		public string Manufacturer
		{
			get
			{
				return this.manufacturer;
			}
		}

		public string Model
		{
			get
			{
				return this.model;
			}
		}

		public string SerialNumber
		{
			get
			{
				return this.serialNumber;
			}
		}

		public string DeviceVersion
		{
			get
			{
				return this.deviceVersion;
			}
		}

		public string FirmwareVersion
		{
			get
			{
				return this.firmwareVersion;
			}
		}

		protected override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			long position = writer.Position;
			writer.WriteUInt16(0);
			long position2 = writer.Position;
			writer.WriteUInt16(0);
			long position3 = writer.Position;
			writer.WriteUInt16(0);
			long position4 = writer.Position;
			writer.WriteUInt16(0);
			long position5 = writer.Position;
			writer.WriteUInt16(0);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.manufacturer, position);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.model, position2);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.serialNumber, position3);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.deviceVersion, position4);
			AuxiliaryBlock.WriteUnicodeStringAndUpdateOffset(writer, this.firmwareVersion, position5);
		}

		private readonly string manufacturer;

		private readonly string model;

		private readonly string serialNumber;

		private readonly string deviceVersion;

		private readonly string firmwareVersion;
	}
}
