using System;
using System.IO;
using System.Threading;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class BufferedStream : Stream
	{
		public BufferedStream(Stream parentStream) : this(parentStream, DataStream.JetChunkSize)
		{
		}

		public BufferedStream(Stream parentStream, int bufferSize)
		{
			this.parentStream = parentStream;
			this.bufferedStream = new BufferedStream(parentStream, bufferSize);
			this.identity = Interlocked.Increment(ref BufferedStream.uniqueId);
		}

		public override bool CanRead
		{
			get
			{
				return this.bufferedStream.CanRead;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return this.bufferedStream.CanWrite;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return this.bufferedStream.CanSeek;
			}
		}

		public override long Length
		{
			get
			{
				return this.bufferedStream.Length;
			}
		}

		public Stream ContainedStream
		{
			get
			{
				return this.parentStream;
			}
		}

		public override long Position
		{
			get
			{
				return this.bufferedStream.Position;
			}
			set
			{
				if (value == this.bufferedStream.Position)
				{
					return;
				}
				this.bufferedStream.Position = value;
			}
		}

		internal int Identity
		{
			get
			{
				return this.identity;
			}
		}

		public override void Flush()
		{
			this.bufferedStream.Flush();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return this.bufferedStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return this.bufferedStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			if (this.Length != value)
			{
				this.bufferedStream.SetLength(value);
			}
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			this.bufferedStream.Write(buffer, offset, count);
		}

		public void WrapStream(Stream newStream)
		{
			if (this.bufferedStream != null)
			{
				this.bufferedStream.Close();
			}
			this.bufferedStream = new BufferedStream(newStream, DataStream.JetChunkSize);
			this.parentStream = newStream;
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && this.bufferedStream != null)
				{
					this.bufferedStream.Close();
					this.bufferedStream = null;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private static int uniqueId;

		private readonly int identity;

		private BufferedStream bufferedStream;

		private Stream parentStream;
	}
}
