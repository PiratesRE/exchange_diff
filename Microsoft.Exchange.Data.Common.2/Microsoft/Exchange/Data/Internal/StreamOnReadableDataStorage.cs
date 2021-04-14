using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Internal
{
	internal class StreamOnReadableDataStorage : StreamOnDataStorage, ICloneableStream
	{
		public StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end)
		{
			if (baseStorage != null)
			{
				baseStorage.AddRef();
				this.baseStorage = baseStorage;
			}
			this.start = start;
			this.end = end;
		}

		private StreamOnReadableDataStorage(ReadableDataStorage baseStorage, long start, long end, long position)
		{
			if (baseStorage != null)
			{
				baseStorage.AddRef();
				this.baseStorage = baseStorage;
			}
			this.start = start;
			this.end = end;
			this.position = position;
		}

		public override DataStorage Storage
		{
			get
			{
				this.ThrowIfDisposed();
				return this.baseStorage;
			}
		}

		public override long Start
		{
			get
			{
				this.ThrowIfDisposed();
				return this.start;
			}
		}

		public override long End
		{
			get
			{
				this.ThrowIfDisposed();
				return this.end;
			}
		}

		public override bool CanRead
		{
			get
			{
				return !this.disposed;
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
				return !this.disposed;
			}
		}

		public override long Length
		{
			get
			{
				this.ThrowIfDisposed();
				if (this.end != 9223372036854775807L)
				{
					return this.end - this.start;
				}
				return this.baseStorage.Length - this.start;
			}
		}

		public override long Position
		{
			get
			{
				this.ThrowIfDisposed();
				return this.position;
			}
			set
			{
				this.ThrowIfDisposed();
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", SharedStrings.CannotSeekBeforeBeginning);
				}
				this.position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			this.ThrowIfDisposed();
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", SharedStrings.OffsetOutOfRange);
			}
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count", SharedStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", SharedStrings.CountTooLarge);
			}
			int num = 0;
			if ((this.end == 9223372036854775807L || this.position < this.end - this.start) && count != 0)
			{
				if (this.end != 9223372036854775807L && (long)count > this.end - this.start - this.position)
				{
					count = (int)(this.end - this.start - this.position);
				}
				int num2;
				do
				{
					num2 = this.baseStorage.Read(this.start + this.position, buffer, offset, count);
					count -= num2;
					offset += num2;
					this.position += (long)num2;
					num += num2;
				}
				while (count != 0 && num2 != 0);
			}
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			this.ThrowIfDisposed();
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
				throw new ArgumentException("Invalid Origin enumeration value", "origin");
			}
			if (offset < 0L)
			{
				throw new ArgumentOutOfRangeException("offset", SharedStrings.CannotSeekBeforeBeginning);
			}
			this.position = offset;
			return this.position;
		}

		public Stream Clone()
		{
			this.ThrowIfDisposed();
			return new StreamOnReadableDataStorage(this.baseStorage, this.start, this.end, this.position);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.baseStorage != null)
			{
				this.baseStorage.Release();
				this.baseStorage = null;
			}
			this.disposed = true;
			base.Dispose(disposing);
		}

		private void ThrowIfDisposed()
		{
			if (this.disposed)
			{
				throw new ObjectDisposedException("StreamOnReadableDataStorage");
			}
		}

		private ReadableDataStorage baseStorage;

		private long start;

		private long end;

		private long position;

		private bool disposed;
	}
}
