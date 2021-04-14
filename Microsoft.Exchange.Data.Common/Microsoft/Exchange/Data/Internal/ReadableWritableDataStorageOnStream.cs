using System;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal class ReadableWritableDataStorageOnStream : ReadableWritableDataStorage
	{
		public ReadableWritableDataStorageOnStream(Stream stream, bool ownsStream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			this.stream = stream;
			this.ownsStream = ownsStream;
		}

		public override long Length
		{
			get
			{
				base.ThrowIfDisposed();
				return this.stream.Length;
			}
		}

		public override int Read(long position, byte[] buffer, int offset, int count)
		{
			base.ThrowIfDisposed();
			int result = 0;
			if (this.isReadOnly)
			{
				this.readOnlySemaphore.Wait();
				try
				{
					return this.InternalRead(position, buffer, offset, count);
				}
				finally
				{
					this.readOnlySemaphore.Release();
				}
			}
			result = this.InternalRead(position, buffer, offset, count);
			return result;
		}

		public override void Write(long position, byte[] buffer, int offset, int count)
		{
			base.ThrowIfDisposed();
			if (this.isReadOnly)
			{
				throw new InvalidOperationException("Write to read-only DataStorage");
			}
			this.stream.Position = position;
			this.stream.Write(buffer, offset, count);
		}

		public override void SetLength(long length)
		{
			base.ThrowIfDisposed();
			if (this.isReadOnly)
			{
				throw new InvalidOperationException("Write to read-only DataStorage");
			}
			this.stream.SetLength(length);
		}

		protected override void Dispose(bool disposing)
		{
			if (!base.IsDisposed)
			{
				if (disposing && this.ownsStream)
				{
					this.stream.Dispose();
				}
				this.stream = null;
			}
			base.Dispose(disposing);
		}

		private int InternalRead(long position, byte[] buffer, int offset, int count)
		{
			this.stream.Position = position;
			return this.stream.Read(buffer, offset, count);
		}

		protected Stream stream;

		protected bool ownsStream;
	}
}
