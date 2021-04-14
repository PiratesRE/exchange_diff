using System;
using System.IO;

namespace Microsoft.Exchange.UnifiedContent
{
	public class StreamOnStream : Stream
	{
		public StreamOnStream(Stream stream, long start, long length)
		{
			this.baseStream = stream;
			this.start = Math.Min(start, stream.Length);
			this.length = Math.Min(length, stream.Length - this.start);
			this.position = 0L;
		}

		public override bool CanRead
		{
			get
			{
				return this.baseStream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.baseStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.baseStream.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return this.length;
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
				this.position = value;
			}
		}

		public override void Flush()
		{
			this.baseStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			count = Math.Min(count, (int)(this.length - this.position));
			long num = this.baseStream.Position;
			this.baseStream.Position = this.position + this.start;
			int result = this.baseStream.Read(buffer, offset, count);
			this.position = this.baseStream.Position - this.start;
			this.baseStream.Position = num;
			return result;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.position = offset;
				break;
			case SeekOrigin.Current:
				this.position += offset;
				break;
			case SeekOrigin.End:
				this.position = this.length - offset;
				break;
			default:
				throw new ArgumentException("Invalid seek origin");
			}
			if (this.position < 0L)
			{
				throw new IOException("Cannot seek before beginning of stream.");
			}
			return this.position;
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count > (int)(this.length - this.position))
			{
				throw new NotSupportedException();
			}
			long num = this.baseStream.Position;
			this.baseStream.Position = this.position + this.start;
			this.baseStream.Write(buffer, offset, count);
			this.position = this.baseStream.Position - this.start;
			this.baseStream.Position = num;
		}

		private readonly long start;

		private readonly long length;

		private readonly Stream baseStream;

		private long position;
	}
}
