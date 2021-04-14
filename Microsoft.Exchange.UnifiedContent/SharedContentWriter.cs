using System;
using System.IO;
using System.Text;

namespace Microsoft.Exchange.UnifiedContent
{
	internal class SharedContentWriter : BinaryWriter
	{
		internal SharedContentWriter(Stream stream) : base(stream, Encoding.Unicode)
		{
		}

		public static long ComputeLength(string str)
		{
			return (long)(8 + str.Length * 2 + 2);
		}

		public static long ComputeLength(Stream stream)
		{
			return 8L + stream.Length;
		}

		public override void Write(string str)
		{
			this.Write(((long)str.Length + 1L) * 2L);
			this.Write(str.ToCharArray());
			this.Write('\0');
		}

		public override void Write(byte[] buffer)
		{
			this.Write(buffer.Length);
			base.Write(buffer);
		}

		internal void Write(Stream stream)
		{
			this.Write(stream.Length);
			stream.Position = 0L;
			stream.CopyTo(this.BaseStream);
		}

		internal void Write(UnifiedContentSerializer.EntryId id)
		{
			this.Write((uint)id);
		}

		internal void Write(UnifiedContentSerializer.PropertyId id)
		{
			this.Write((uint)id);
		}

		internal void ValidateAtEndOfEntry()
		{
			if (this.BaseStream.Position != this.BaseStream.Length)
			{
				throw new FormatException("Shared Content Entry invalid");
			}
		}

		protected override void Dispose(bool disposing)
		{
			this.BaseStream.Flush();
		}

		public const int EntryIdSize = 4;

		public const int PropertyIdSize = 4;

		public const int SharedEntryPosSize = 8;

		public const int RawEntryPosSize = 8;

		public const int ExtractedEntryPosSize = 8;

		public const int StreamLengthSize = 8;
	}
}
