using System;
using System.IO;

namespace Microsoft.Exchange.Transport
{
	internal class SynchronizedStream : Stream
	{
		public SynchronizedStream(Stream wrapped)
		{
			this.stream = wrapped;
		}

		public override bool CanRead
		{
			get
			{
				return this.stream != null && this.stream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.stream != null && this.stream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.stream != null && this.stream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				if (this.stream == null)
				{
					throw new ObjectDisposedException("AutoPositionStream");
				}
				long length;
				lock (this.stream)
				{
					length = this.stream.Length;
				}
				return length;
			}
		}

		public override long Position
		{
			get
			{
				if (this.stream == null)
				{
					throw new ObjectDisposedException("AutoPositionStream");
				}
				return this.position;
			}
			set
			{
				if (this.stream == null)
				{
					throw new ObjectDisposedException("AutoPositionStream");
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", "Cannot Seek before the beginning");
				}
				this.position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.stream == null)
			{
				throw new ObjectDisposedException("AutoPositionStream");
			}
			int num = 0;
			lock (this.stream)
			{
				this.stream.Position = this.position;
				num = this.stream.Read(buffer, offset, count);
			}
			this.position += (long)num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.stream == null)
			{
				throw new ObjectDisposedException("AutoPositionStream");
			}
			lock (this.stream)
			{
				this.stream.Position = this.position;
				this.stream.Write(buffer, offset, count);
			}
			this.position += (long)count;
		}

		public override void SetLength(long value)
		{
			if (this.stream == null)
			{
				throw new ObjectDisposedException("AutoPositionStream");
			}
			lock (this.stream)
			{
				this.stream.SetLength(value);
			}
			if (this.position > value)
			{
				this.position = value;
			}
		}

		public override void Flush()
		{
			if (this.stream == null)
			{
				throw new ObjectDisposedException("AutoPositionStream");
			}
			lock (this.stream)
			{
				this.stream.Flush();
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.stream == null)
			{
				throw new ObjectDisposedException("AutoPositionStream");
			}
			switch (origin)
			{
			case SeekOrigin.Begin:
				break;
			case SeekOrigin.Current:
				offset += this.position;
				break;
			case SeekOrigin.End:
				offset += this.Length;
				break;
			default:
				throw new ArgumentException("origin");
			}
			if (0L > offset)
			{
				throw new ArgumentOutOfRangeException("offset", "Cannot Seek before the beginning");
			}
			this.position = offset;
			return this.position;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.stream = null;
			}
			base.Dispose(disposing);
		}

		private long position;

		private Stream stream;
	}
}
