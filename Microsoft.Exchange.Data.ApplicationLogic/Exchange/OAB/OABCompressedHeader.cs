using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OABCompressedHeader
	{
		public int MaxVersion { get; set; }

		public int MinVersion { get; set; }

		public int MaximumCompressionBlockSize { get; set; }

		public uint UncompressedFileSize { get; set; }

		public static OABCompressedHeader ReadFrom(BinaryReader reader)
		{
			return new OABCompressedHeader
			{
				MaxVersion = (int)reader.ReadUInt32("MaxVersion"),
				MinVersion = (int)reader.ReadUInt32("MinVersion"),
				MaximumCompressionBlockSize = (int)reader.ReadUInt32("MaximumCompressionBlockSize"),
				UncompressedFileSize = reader.ReadUInt32("UncompressedFileSize")
			};
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write((uint)this.MaxVersion);
			writer.Write((uint)this.MinVersion);
			writer.Write((uint)this.MaximumCompressionBlockSize);
			writer.Write(this.UncompressedFileSize);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("MaxVersion: ");
			stringBuilder.AppendLine(this.MaxVersion.ToString());
			stringBuilder.Append("MinVersion: ");
			stringBuilder.AppendLine(this.MinVersion.ToString());
			stringBuilder.Append("MaximumCompressionBlockSize: ");
			stringBuilder.AppendLine(this.MaximumCompressionBlockSize.ToString());
			stringBuilder.Append("UncompressedFileSize: ");
			stringBuilder.AppendLine(this.UncompressedFileSize.ToString());
			return stringBuilder.ToString();
		}

		public static readonly int DefaultMaxVersion = 3;

		public static readonly int DefaultMinVersion = 1;
	}
}
