using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.OAB;

namespace Microsoft.Exchange.OAB
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class OABCompressStream : Stream
	{
		public OABCompressStream(Stream stream, int maximumCompressionBlockSize)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.CanSeek)
			{
				throw new ArgumentException("stream must allow seek");
			}
			if (!stream.CanWrite)
			{
				throw new ArgumentException("stream must allow write");
			}
			this.stream = stream;
			this.maximumCompressionBlockSize = maximumCompressionBlockSize;
			this.writer = new BinaryWriter(new NoCloseStream(stream));
			this.compressor = new DataCompression(maximumCompressionBlockSize, maximumCompressionBlockSize);
			this.buffer = new ByteQueue(maximumCompressionBlockSize * 2);
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
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
				return this.stream.Position;
			}
			set
			{
				throw new InvalidOperationException("set Position");
			}
		}

		public override void Flush()
		{
			this.stream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new InvalidOperationException("Read'");
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
			try
			{
				while (this.buffer.Count > 0)
				{
					byte[] data = this.buffer.Dequeue(this.maximumCompressionBlockSize);
					this.WriteData(data);
				}
				this.stream.Seek(0L, SeekOrigin.Begin);
				this.WriteHeader(this.uncompressedLength);
			}
			finally
			{
				this.writer.Dispose();
				this.compressor.Dispose();
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			while (count > 0 || this.buffer.Count >= this.maximumCompressionBlockSize)
			{
				if (this.buffer.Count >= this.maximumCompressionBlockSize)
				{
					byte[] data = this.buffer.Dequeue(this.maximumCompressionBlockSize);
					this.WriteData(data);
				}
				if (count > 0)
				{
					int num = Math.Min(this.maximumCompressionBlockSize, count);
					this.buffer.Enqueue(num, buffer, offset);
					count -= num;
					offset += num;
				}
			}
		}

		private void WriteData(byte[] data)
		{
			if (!this.header)
			{
				this.WriteHeader(0U);
				this.header = true;
			}
			this.uncompressedLength += (uint)data.Length;
			byte[] compressedData;
			if (this.compressor.TryCompress(data, out compressedData))
			{
				this.WriteBlock(CompressionBlockFlags.Compressed, data, compressedData);
				return;
			}
			this.WriteBlock(CompressionBlockFlags.NotCompressed, data, data);
		}

		private void WriteHeader(uint uncompressedFileSize)
		{
			OABCompressedHeader oabcompressedHeader = new OABCompressedHeader
			{
				MaxVersion = OABCompressedHeader.DefaultMaxVersion,
				MinVersion = OABCompressedHeader.DefaultMinVersion,
				MaximumCompressionBlockSize = this.maximumCompressionBlockSize,
				UncompressedFileSize = uncompressedFileSize
			};
			OABCompressStream.Tracer.TraceDebug<long, OABCompressedHeader>((long)this.GetHashCode(), "OABCompressStream: writing header at stream position {0}:\n\r{1}", this.stream.Position, oabcompressedHeader);
			oabcompressedHeader.WriteTo(this.writer);
		}

		private void WriteBlock(CompressionBlockFlags compressionBlockFlags, byte[] uncompressedData, byte[] compressedData)
		{
			OABCompressedBlock oabcompressedBlock = new OABCompressedBlock
			{
				Flags = compressionBlockFlags,
				CompressedLength = compressedData.Length,
				UncompressedLength = uncompressedData.Length,
				CRC = OABCRC.ComputeCRC(OABCRC.DefaultSeed, uncompressedData),
				Data = compressedData
			};
			OABCompressStream.Tracer.TraceDebug<long, OABCompressedBlock>((long)this.GetHashCode(), "OABCompressStream: writing block at stream position {0}:\n\r{1}", this.stream.Position, oabcompressedBlock);
			oabcompressedBlock.WriteTo(this.writer);
		}

		private static readonly Trace Tracer = ExTraceGlobals.DataTracer;

		private readonly Stream stream;

		private readonly int maximumCompressionBlockSize;

		private readonly BinaryWriter writer;

		private readonly DataCompression compressor;

		private readonly ByteQueue buffer;

		private uint uncompressedLength;

		private bool header;
	}
}
