using System;
using System.IO;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Internal
{
	internal class AppendStreamOnDataStorage : StreamOnDataStorage
	{
		public AppendStreamOnDataStorage(ReadableWritableDataStorage storage)
		{
			storage.AddRef();
			this.storage = storage;
			this.position = storage.Length;
		}

		public override DataStorage Storage
		{
			get
			{
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
				return this.position;
			}
		}

		public ReadableWritableDataStorage ReadableWritableStorage
		{
			get
			{
				return this.storage;
			}
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
				return this.storage != null;
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
				throw new NotSupportedException();
			}
		}

		public override long Position
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("AppendStreamOnDataStorage");
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
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			if (this.storage == null)
			{
				throw new ObjectDisposedException("AppendStreamOnDataStorage");
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
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
