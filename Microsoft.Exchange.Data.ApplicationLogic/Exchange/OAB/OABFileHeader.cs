using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class OABFileHeader
	{
		public int Version { get; set; }

		public uint CRC { get; set; }

		public int RecordCount { get; set; }

		public static OABFileHeader ReadFrom(BinaryReader reader)
		{
			return new OABFileHeader
			{
				Version = (int)reader.ReadUInt32("Version"),
				CRC = reader.ReadUInt32("CRC"),
				RecordCount = (int)reader.ReadUInt32("RecordCount")
			};
		}

		public void WriteTo(BinaryWriter writer)
		{
			writer.Write((uint)this.Version);
			writer.Write(this.CRC);
			writer.Write((uint)this.RecordCount);
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(100);
			stringBuilder.Append("Version: ");
			stringBuilder.AppendLine(this.Version.ToString());
			stringBuilder.Append("CRC: ");
			stringBuilder.AppendLine(this.CRC.ToString("X8"));
			stringBuilder.Append("RecordCount: ");
			stringBuilder.AppendLine(this.RecordCount.ToString());
			return stringBuilder.ToString();
		}

		public static readonly int DefaultVersion = 32;
	}
}
