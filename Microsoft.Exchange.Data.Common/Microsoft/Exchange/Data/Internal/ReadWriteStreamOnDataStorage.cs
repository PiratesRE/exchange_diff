using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Internal
{
	internal class ReadWriteStreamOnDataStorage : StreamOnDataStorage, ICloneableStream
	{
		internal ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage)
		{
			storage.AddRef();
			this.storage = storage;
		}

		private ReadWriteStreamOnDataStorage(ReadableWritableDataStorage storage, long position)
		{
			storage.AddRef();
			this.storage = storage;
			this.position = position;
		}

		public override DataStorage Storage
		{
			get
			{
				if (this.storage == null)
				{
					throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
				}
				return this.storage;
			}
		}

		public override long Start
		{
			get
			{
				return 0L;
			}
		}

		public override long End
		{
			get
			{
				return long.MaxValue;
			}
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
				return this.storage != null;
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
					throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
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
					throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
				}
				return this.position;
			}
			set
			{
				if (this.storage == null)
				{
					throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
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
				throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
			}
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
			int num = this.storage.Read(this.position, buffer, offset, count);
			this.position += (long)num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
			}
			if (buffer == null)
			{
				throw new ArgumentNullException("buffer");
			}
			if (offset > buffer.Length || offset < 0)
			{
				throw new ArgumentOutOfRangeException("offset", SharedStrings.OffsetOutOfRange);
			}
			if (count > buffer.Length || count < 0)
			{
				throw new ArgumentOutOfRangeException("count", SharedStrings.CountOutOfRange);
			}
			if (count + offset > buffer.Length)
			{
				throw new ArgumentOutOfRangeException("count", SharedStrings.CountTooLarge);
			}
			this.storage.Write(this.position, buffer, offset, count);
			this.position += (long)count;
		}

		public override void SetLength(long value)
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
			}
			if (value < 0L)
			{
				throw new ArgumentOutOfRangeException("value", SharedStrings.CannotSetNegativelength);
			}
			this.storage.SetLength(value);
		}

		public override void Flush()
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
			}
			switch (origin)
			{
			case SeekOrigin.Begin:
				this.position = offset;
				break;
			case SeekOrigin.Current:
				offset = this.position + offset;
				break;
			case SeekOrigin.End:
				offset = this.storage.Length + offset;
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

		Stream ICloneableStream.Clone()
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("ReadWriteStreamOnDataStorage");
			}
			return new ReadWriteStreamOnDataStorage(this.storage, this.position);
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

		private ReadableWritableDataStorage storage;

		private long position;
	}
}
