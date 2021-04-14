using System;
using System.IO;

namespace Microsoft.Exchange.Data.Internal
{
	internal class ReadableDataStorageOnStream : ReadableDataStorage
	{
		public ReadableDataStorageOnStream(Stream stream, bool ownsStream)
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

		private Stream stream;

		private bool ownsStream;
	}
}
