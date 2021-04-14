using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OABCompressedBlock
	{
		public CompressionBlockFlags Flags { get; set; }

		public int CompressedLength { get; set; }

		public int UncompressedLength { get; set; }

		public uint CRC { get; set; }

		public byte[] Data { get; set; }

		public static OABCompressedBlock ReadFrom(BinaryReader reader)
		{
			OABCompressedBlock oabcompressedBlock = new OABCompressedBlock();
			oabcompressedBlock.Flags = (CompressionBlockFlags)reader.ReadUInt32("Flags");
			oabcompressedBlock.CompressedLength = (int)reader.ReadUInt32("CompressedLength");
			oabcompressedBlock.UncompressedLength = (int)reader.ReadUInt32("UncompressedLength");
			oabcompressedBlock.CRC = reader.ReadUInt32("CRC");
			oabcompressedBlock.Data = reader.ReadBytes(oabcompressedBlock.CompressedLength, "Data");
			return oabcompressedBlock;
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write((uint)this.Flags);
			writer.Write((uint)this.CompressedLength);
			writer.Write((uint)this.UncompressedLength);
			writer.Write(this.CRC);
			writer.Write(this.Data);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Flags: ");
			stringBuilder.AppendLine(this.Flags.ToString());
			stringBuilder.Append("CompressedLength: ");
			stringBuilder.AppendLine(this.CompressedLength.ToString());
			stringBuilder.Append("UncompressedLength: ");
			stringBuilder.AppendLine(this.UncompressedLength.ToString());
			stringBuilder.Append("CRC: ");
			stringBuilder.AppendLine(this.CRC.ToString("X8"));
			return stringBuilder.ToString();
		}
	}
}
