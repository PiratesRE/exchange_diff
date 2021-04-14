using System;

namespace System.Diagnostics.Tracing
{
	internal struct ManifestEnvelope
	{
		public const int MaxChunkSize = 65280;

		public ManifestEnvelope.ManifestFormats Format;

		public byte MajorVersion;

		public byte MinorVersion;

		public byte Magic;

		public ushort TotalChunks;

		public ushort ChunkNumber;

		public enum ManifestFormats : byte
		{
			SimpleXmlFormat = 1
		}
	}
}
