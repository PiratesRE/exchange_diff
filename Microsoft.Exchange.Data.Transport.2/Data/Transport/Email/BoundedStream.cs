using System;
using System.IO;

namespace Microsoft.Exchange.Data.Transport.Email
{
	internal class BoundedStream : Stream
	{
		public BoundedStream(Stream stream, long offset, long size)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (offset < 0L)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (size < 0L)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			this.internalStream = stream;
			this.expectedSize = size;
			this.startOffset = offset;
			if (!this.internalStream.CanRead)
			{
				throw new ArgumentException();
			}
			if (this.internalStream.CanSeek && this.internalStream.Position != offset)
			{
				this.internalStream.Position = offset;
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		protected override void Dispose(bool disposing)
		{
			if (!this.isDisposed)
			{
				this.isDisposed = true;
				this.InternalDispose(disposing);
			}
			base.Dispose(disposing);
		}

		public new void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void InternalDispose(bool disposing)
		{
			if (disposing && this.internalStream != null)
			{
				this.internalStream.Dispose();
				this.internalStream = null;
			}
		}

		private void CheckDisposed(string methodName)
		{
			if (this.isDisposed)
			{
				throw new ObjectDisposedException(this.ToString());
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.CheckDisposed("Seek");
			if (!this.CanSeek)
			{
				throw new NotSupportedException();
			}
			long num = this.Position;
			switch (origin)
			{
			case SeekOrigin.Begin:
				num = offset;
				break;
			case SeekOrigin.Current:
				num += offset;
				break;
			case SeekOrigin.End:
				num = this.expectedSize - offset;
				break;
			default:
				throw new ArgumentOutOfRangeException("origin");
			}
			this.Position = num;
			return num;
		}

		public override void Flush()
		{
			this.CheckDisposed("Flush");
			this.internalStream.Flush();
		}

		public override long Position
		{
			get
			{
				return this.bytesRead;
			}
			set
			{
				this.CheckDisposed("Position::set");
				if (!this.CanSeek)
				{
					throw new NotSupportedException();
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.bytesRead = value;
				long num = Math.Min(this.expectedSize, value);
				this.internalStream.Position = this.startOffset + num;
			}
		}

		public override long Length
		{
			get
			{
				return this.expectedSize;
			}
		}

		public override bool CanSeek
		{
			get
			{
				this.CheckDisposed("CanSeek::get");
				return this.internalStream.CanSeek;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.CheckDisposed("Read");
			int num = Math.Min(count, (int)(this.expectedSize - this.bytesRead));
			if (num > 0)
			{
				int num2 = this.internalStream.Read(buffer, offset, num);
				this.bytesRead += (long)num2;
				return num2;
			}
			return 0;
		}

		public override int ReadByte()
		{
			this.CheckDisposed("ReadByte");
			if (this.bytesRead >= this.expectedSize)
			{
				return -1;
			}
			int num = this.internalStream.ReadByte();
			if (num != -1)
			{
				this.bytesRead += 1L;
			}
			return num;
		}

		private readonly long expectedSize;

		private long bytesRead;

		private readonly long startOffset;

		private Stream internalStream;

		private bool isDisposed;
	}
}
