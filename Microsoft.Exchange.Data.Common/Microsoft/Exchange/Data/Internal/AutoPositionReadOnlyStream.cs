using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Internal
{
	internal sealed class AutoPositionReadOnlyStream : Stream, ICloneableStream
	{
		public AutoPositionReadOnlyStream(Stream wrapped, bool ownsStream)
		{
			this.storage = new ReadableDataStorageOnStream(wrapped, ownsStream);
			this.position = wrapped.Position;
		}

		private AutoPositionReadOnlyStream(AutoPositionReadOnlyStream original)
		{
			original.storage.AddRef();
			this.storage = original.storage;
			this.position = original.position;
		}

		public override bool CanRead
		{
			get
			{
				return this.storage != null;
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
				return this.storage != null;
			}
		}

		public override long Length
		{
			get
			{
				if (this.storage == null)
				{
					throw new ObjectDisposedException("AutoPositionReadOnlyStream");
				}
				return this.storage.Length;
			}
		}

		public override long Position
		{
			get
			{
				if (this.storage == null)
				{
					throw new ObjectDisposedException("AutoPositionReadOnlyStream");
				}
				return this.position;
			}
			set
			{
				if (this.storage == null)
				{
					throw new ObjectDisposedException("AutoPositionReadOnlyStream");
				}
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException("value", SharedStrings.CannotSeekBeforeBeginning);
				}
				this.position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("AutoPositionReadOnlyStream");
			}
			int num = this.storage.Read(this.position, buffer, offset, count);
			this.position += (long)num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("AutoPositionReadOnlyStream");
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
				throw new ArgumentOutOfRangeException("offset", SharedStrings.CannotSeekBeforeBeginning);
			}
			this.position = offset;
			return this.position;
		}

		public Stream Clone()
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("AutoPositionReadOnlyStream");
			}
			return new AutoPositionReadOnlyStream(this);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.storage != null)
			{
				this.storage.Release();
				this.storage = null;
			}
			base.Dispose(disposing);
		}

		private ReadableDataStorage storage;

		private long position;
	}
}
