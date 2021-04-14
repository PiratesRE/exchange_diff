using System;
using System.IO;
using Microsoft.Exchange.CtsResources;
using Microsoft.Exchange.Data.Internal;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	public class EncoderStream : Stream, ICloneableStream
	{
		public EncoderStream(Stream stream, ByteEncoder encoder, EncoderStreamAccess access)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (encoder == null)
			{
				throw new ArgumentNullException("encoder");
			}
			if (access == EncoderStreamAccess.Read)
			{
				if (!stream.CanRead)
				{
					throw new NotSupportedException(EncodersStrings.EncStrCannotRead);
				}
			}
			else if (!stream.CanWrite)
			{
				throw new NotSupportedException(EncodersStrings.EncStrCannotWrite);
			}
			this.stream = stream;
			this.encoder = encoder;
			this.access = access;
			this.ownsStream = true;
			this.length = long.MaxValue;
			this.buffer = new byte[4096];
		}

		internal EncoderStream(Stream stream, ByteEncoder encoder, EncoderStreamAccess access, bool ownsStream)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (encoder == null)
			{
				throw new ArgumentNullException("encoder");
			}
			if (access == EncoderStreamAccess.Read)
			{
				if (!stream.CanRead)
				{
					throw new NotSupportedException(EncodersStrings.EncStrCannotRead);
				}
			}
			else if (!stream.CanWrite)
			{
				throw new NotSupportedException(EncodersStrings.EncStrCannotWrite);
			}
			this.stream = stream;
			this.encoder = encoder;
			this.access = access;
			this.ownsStream = ownsStream;
			this.length = long.MaxValue;
			this.buffer = new byte[4096];
		}

		public sealed override bool CanRead
		{
			get
			{
				return this.access == EncoderStreamAccess.Read && this.IsOpen;
			}
		}

		public sealed override bool CanWrite
		{
			get
			{
				return EncoderStreamAccess.Write == this.access && this.IsOpen;
			}
		}

		public sealed override bool CanSeek
		{
			get
			{
				return this.access == EncoderStreamAccess.Read && this.IsOpen && this.stream.CanSeek;
			}
		}

		public sealed override long Length
		{
			get
			{
				this.AssertOpen();
				if (this.access == EncoderStreamAccess.Read)
				{
					if (9223372036854775807L == this.length)
					{
						Stream stream = this.Clone();
						long num = this.position;
						byte[] array = new byte[4096];
						long num2;
						do
						{
							num2 = (long)stream.Read(array, 0, 4096);
							num += num2;
						}
						while (num2 > 0L);
						this.length = num;
					}
					return this.length;
				}
				return this.position;
			}
		}

		public sealed override long Position
		{
			get
			{
				this.AssertOpen();
				return this.position;
			}
			set
			{
				this.AssertOpen();
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		public Stream BaseStream
		{
			get
			{
				this.AssertOpen();
				return this.stream;
			}
		}

		public ByteEncoder ByteEncoder
		{
			get
			{
				this.AssertOpen();
				return this.encoder;
			}
		}

		private bool IsOpen
		{
			get
			{
				return null != this.stream;
			}
		}

		public Stream Clone()
		{
			this.AssertOpen();
			if (EncoderStreamAccess.Write == this.access)
			{
				throw new NotSupportedException(EncodersStrings.EncStrCannotCloneWriteableStream);
			}
			ICloneableStream cloneableStream = this.stream as ICloneableStream;
			if (cloneableStream == null && this.stream.CanSeek)
			{
				this.stream = new AutoPositionReadOnlyStream(this.stream, this.ownsStream);
				this.ownsStream = true;
				cloneableStream = (this.stream as ICloneableStream);
			}
			if (cloneableStream != null)
			{
				EncoderStream encoderStream = base.MemberwiseClone() as EncoderStream;
				encoderStream.buffer = (this.buffer.Clone() as byte[]);
				encoderStream.stream = cloneableStream.Clone();
				encoderStream.encoder = this.encoder.Clone();
				return encoderStream;
			}
			throw new NotSupportedException(EncodersStrings.EncStrCannotCloneChildStream(this.stream.GetType().ToString()));
		}

		public sealed override int Read(byte[] array, int offset, int count)
		{
			this.AssertOpen();
			if (!this.CanRead)
			{
				throw new NotSupportedException(EncodersStrings.EncStrCannotRead);
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (offset + count > array.Length)
			{
				throw new ArgumentOutOfRangeException("offset, count", EncodersStrings.EncStrLengthExceeded(offset + count, array.Length));
			}
			if (0 > offset || 0 > count)
			{
				throw new ArgumentOutOfRangeException((offset < 0) ? "offset" : "count");
			}
			int num = 0;
			while (!this.endOfFile && count != 0)
			{
				if (this.bufferCount == 0)
				{
					this.bufferPos = 0;
					this.bufferCount = this.stream.Read(this.buffer, 0, 4096);
				}
				int num2;
				int num3;
				bool flag;
				this.encoder.Convert(this.buffer, this.bufferPos, this.bufferCount, array, offset, count, this.bufferCount == 0, out num2, out num3, out flag);
				if (this.bufferCount == 0 && flag)
				{
					this.endOfFile = true;
				}
				count -= num3;
				offset += num3;
				num += num3;
				this.position += (long)num3;
				this.bufferPos += num2;
				this.bufferCount -= num2;
			}
			return num;
		}

		public sealed override void Write(byte[] array, int offset, int count)
		{
			this.AssertOpen();
			if (!this.CanWrite)
			{
				throw new NotSupportedException(EncodersStrings.EncStrCannotWrite);
			}
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (count + offset > array.Length)
			{
				throw new ArgumentException(EncodersStrings.EncStrLengthExceeded(offset + count, array.Length), "array");
			}
			if (0 > offset || 0 > count)
			{
				throw new ArgumentOutOfRangeException((offset < 0) ? "offset" : "count");
			}
			while (count != 0)
			{
				int num;
				int count2;
				bool flag;
				this.encoder.Convert(array, offset, count, this.buffer, 0, this.buffer.Length, false, out num, out count2, out flag);
				count -= num;
				offset += num;
				this.position += (long)num;
				this.stream.Write(this.buffer, 0, count2);
			}
		}

		public sealed override long Seek(long offset, SeekOrigin origin)
		{
			this.AssertOpen();
			if (!this.CanSeek)
			{
				throw new NotSupportedException(EncodersStrings.EncStrCannotSeek);
			}
			if (origin == SeekOrigin.Current)
			{
				offset += this.position;
			}
			else if (origin == SeekOrigin.End)
			{
				if (this.length == 9223372036854775807L && offset == 0L)
				{
					offset = long.MaxValue;
				}
				else
				{
					offset += this.Length;
				}
			}
			if (offset < 0L)
			{
				throw new ArgumentOutOfRangeException("offset");
			}
			if (offset < this.position)
			{
				this.bufferPos = 0;
				this.bufferCount = 0;
				this.endOfFile = false;
				this.encoder.Reset();
				this.stream.Seek(0L, SeekOrigin.Begin);
				this.position = 0L;
			}
			if (offset > this.position)
			{
				long num = offset - this.position;
				byte[] array = new byte[4096];
				while (num > 0L)
				{
					int num2 = (int)Math.Min(num, 4096L);
					num2 = this.Read(array, 0, num2);
					if (num2 == 0)
					{
						if (this.length == 9223372036854775807L)
						{
							this.length = this.position;
						}
						offset = this.position;
						break;
					}
					num -= (long)num2;
				}
			}
			return offset;
		}

		public sealed override void SetLength(long value)
		{
			this.AssertOpen();
			throw new NotSupportedException();
		}

		public sealed override void Flush()
		{
			this.AssertOpen();
			if (!this.CanWrite)
			{
				throw new NotSupportedException(EncodersStrings.EncStrCannotWrite);
			}
			this.FlushEncoder(false);
			this.stream.Flush();
		}

		private void FlushEncoder(bool done)
		{
			bool flag = false;
			int num2;
			do
			{
				int num;
				this.encoder.Convert(null, 0, 0, this.buffer, 0, this.buffer.Length, done, out num, out num2, out flag);
				this.stream.Write(this.buffer, 0, num2);
			}
			while (0 < num2 && !flag);
		}

		private void AssertOpen()
		{
			if (!this.IsOpen)
			{
				throw new ObjectDisposedException("EncoderStream");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.IsOpen)
			{
				if (this.CanWrite)
				{
					this.FlushEncoder(true);
				}
				if (this.stream != null)
				{
					this.stream.Dispose();
					this.stream = null;
				}
				if (this.encoder != null)
				{
					this.encoder.Dispose();
					this.encoder = null;
				}
			}
			base.Dispose(disposing);
		}

		private const int BlockSize = 4096;

		private Stream stream;

		private EncoderStreamAccess access;

		private bool ownsStream;

		private bool endOfFile;

		private long length;

		private long position;

		private ByteEncoder encoder;

		private byte[] buffer;

		private int bufferPos;

		private int bufferCount;
	}
}
