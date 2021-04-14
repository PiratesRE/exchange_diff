using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OABDecompressStream : Stream
	{
		public OABDecompressStream(Stream stream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanRead)
			{
				throw new ArgumentException("stream must allow read");
			}
			this.stream = new NoCloseStream(stream);
			this.reader = new BinaryReader(this.stream);
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override long Length
		{
			get
			{
				return this.stream.Length;
			}
		}

		public override long Position
		{
			get
			{
				return this.position;
			}
			set
			{
				throw new InvalidOperationException("set_Position");
			}
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new InvalidOperationException("Seek'");
		}

		public override void SetLength(long value)
		{
			throw new InvalidOperationException("SetLength'");
		}

		public override void Close()
		{
			this.reader.Dispose();
			if (this.decompressor != null)
			{
				this.decompressor.Dispose();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException("Write'");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.stage == 0)
			{
				this.ReadHeader();
				this.stage = 1;
			}
			int num = 0;
			if (this.stage == 1)
			{
				for (;;)
				{
					int num2 = this.buffer.Dequeue(count, buffer, offset);
					count -= num2;
					offset += num2;
					num += num2;
					if (count == 0)
					{
						goto IL_6F;
					}
					if (this.uncompressedLength == this.header.UncompressedFileSize)
					{
						break;
					}
					this.buffer.Enqueue(this.ReadBlock());
				}
				this.stage = 2;
			}
			IL_6F:
			this.position += (long)num;
			return num;
		}

		private void ReadHeader()
		{
			long arg = this.reader.BaseStream.Position;
			this.header = OABCompressedHeader.ReadFrom(this.reader);
			OABDecompressStream.Tracer.TraceDebug<long, OABCompressedHeader>((long)this.GetHashCode(), "OABDecompressStream: read OABCompressedHeader at position {0}:\r\n{1}", arg, this.header);
			if (this.header.MaxVersion != OABCompressedHeader.DefaultMaxVersion)
			{
				throw new InvalidDataException(string.Format("MaxVersion: expected={0}, actual={1}", OABCompressedHeader.DefaultMaxVersion, this.header.MaxVersion));
			}
			if (this.header.MinVersion != OABCompressedHeader.DefaultMinVersion)
			{
				throw new InvalidDataException(string.Format("MinVersion: expected={0}, actual={1}", OABCompressedHeader.DefaultMinVersion, this.header.MinVersion));
			}
			this.decompressor = new DataCompression(this.header.MaximumCompressionBlockSize, this.header.MaximumCompressionBlockSize);
			this.buffer = new ByteQueue(this.header.MaximumCompressionBlockSize);
		}

		private byte[] ReadBlock()
		{
			long num = this.reader.BaseStream.Position;
			OABCompressedBlock oabcompressedBlock = OABCompressedBlock.ReadFrom(this.reader);
			OABDecompressStream.Tracer.TraceDebug<long, OABCompressedBlock>((long)this.GetHashCode(), "OABDecompressStream: read OABCompressedBlock at position {0}:\r\n{1}", num, oabcompressedBlock);
			if (oabcompressedBlock.CompressedLength > this.header.MaximumCompressionBlockSize)
			{
				throw new InvalidDataException(string.Format("Compressed block starting at position {0}: data is larger than header stated. MaximumCompressionBlockSize={1}, CompressedLength={2}", num, this.header.MaximumCompressionBlockSize, oabcompressedBlock.CompressedLength));
			}
			if (oabcompressedBlock.UncompressedLength > this.header.MaximumCompressionBlockSize)
			{
				throw new InvalidDataException(string.Format("Compressed block starting at position {0}: data is larger than header stated. MaximumCompressionBlockSize={1}, UncompressedLength={2}", num, this.header.MaximumCompressionBlockSize, oabcompressedBlock.UncompressedLength));
			}
			byte[] array;
			if (oabcompressedBlock.Flags == CompressionBlockFlags.Compressed)
			{
				try
				{
					array = this.decompressor.Decompress(oabcompressedBlock.Data, oabcompressedBlock.UncompressedLength);
					goto IL_FF;
				}
				catch (Win32Exception innerException)
				{
					throw new InvalidDataException(string.Format("Compressed block starting at position {0}: unable to decompress data", num), innerException);
				}
			}
			array = oabcompressedBlock.Data;
			IL_FF:
			uint num2 = OABCRC.ComputeCRC(OABCRC.DefaultSeed, array);
			if (num2 != oabcompressedBlock.CRC)
			{
				throw new InvalidDataException(string.Format("Compressed block starting at position {0}: invalid CRC. Expected: {1:X8}, actual: {2:X8}", num, num2, oabcompressedBlock.CRC));
			}
			this.uncompressedLength += (uint)array.Length;
			if (this.uncompressedLength > this.header.UncompressedFileSize)
			{
				throw new InvalidDataException(string.Format("Compressed block starting at position {0}: decompressed data so far is already longer than header stated. Header size: {1}, so far: {2}.", num, this.header.UncompressedFileSize, this.uncompressedLength));
			}
			return array;
		}

		private static readonly Trace Tracer = ExTraceGlobals.DataTracer;

		private readonly Stream stream;

		private readonly BinaryReader reader;

		private DataCompression decompressor;

		private OABCompressedHeader header;

		private uint uncompressedLength;

		private long position;

		private int stage;

		private ByteQueue buffer;
	}
}
