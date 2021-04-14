using System;
using System.IO;

namespace Microsoft.Exchange.Net
{
	internal class DotStuffingStream : Stream
	{
		public DotStuffingStream(Stream stream) : this(stream, false)
		{
		}

		public DotStuffingStream(Stream stream, bool rejectBareLinefeeds)
		{
			this.stream = stream;
			this.rejectBareLinefeeds = rejectBareLinefeeds;
		}

		public override bool CanRead
		{
			get
			{
				return true;
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
				return false;
			}
		}

		public override bool CanTimeout
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
				return this.position;
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public override int ReadTimeout
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

		public override int WriteTimeout
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

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int size, AsyncCallback callback, object state)
		{
			throw new NotSupportedException();
		}

		public override int EndRead(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void EndWrite(IAsyncResult asyncResult)
		{
			throw new NotSupportedException();
		}

		public override void Flush()
		{
			throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int size)
		{
			if (this.state == DotStuffingStream.State.EndOfData)
			{
				return 0;
			}
			int num = (size - 6) * 3 / 4;
			int num2 = offset + size - num;
			int bytesFilled = this.stream.Read(buffer, num2, num);
			return this.DotStuffBuffer(buffer, offset, size, num2, bytesFilled);
		}

		public override int ReadByte()
		{
			throw new NotSupportedException();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Write(byte[] buffer, int offset, int size)
		{
			throw new NotSupportedException();
		}

		public override void WriteByte(byte value)
		{
			throw new NotSupportedException();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && this.stream != null)
			{
				this.stream.Flush();
				this.stream.Dispose();
				this.stream = null;
			}
			base.Dispose(disposing);
		}

		private int DotStuffBuffer(byte[] buffer, int offset, int size, int readOffset, int bytesFilled)
		{
			int num = 0;
			for (int i = readOffset; i < readOffset + bytesFilled; i++)
			{
				byte b = buffer[i];
				buffer[offset + num++] = b;
				switch (this.state)
				{
				case DotStuffingStream.State.BeginningOfLine:
					if (b == 46)
					{
						buffer[offset + num++] = 46;
						this.state = DotStuffingStream.State.Data;
					}
					else if (b == 13)
					{
						this.state = DotStuffingStream.State.CarriageReturnSeen;
					}
					else
					{
						this.state = DotStuffingStream.State.Data;
					}
					break;
				case DotStuffingStream.State.Data:
					if (b == 13)
					{
						this.state = DotStuffingStream.State.CarriageReturnSeen;
					}
					else if (b == 10 && this.rejectBareLinefeeds)
					{
						throw new BareLinefeedException();
					}
					break;
				case DotStuffingStream.State.CarriageReturnSeen:
					if (b == 10)
					{
						this.state = DotStuffingStream.State.BeginningOfLine;
					}
					else if (b == 13)
					{
						this.state = DotStuffingStream.State.CarriageReturnSeen;
					}
					else
					{
						this.state = DotStuffingStream.State.Data;
					}
					break;
				default:
					throw new InvalidOperationException();
				}
			}
			if (this.stream.Position == this.stream.Length)
			{
				int num2;
				switch (this.state)
				{
				case DotStuffingStream.State.BeginningOfLine:
					num2 = 2;
					break;
				case DotStuffingStream.State.Data:
					num2 = 0;
					break;
				case DotStuffingStream.State.CarriageReturnSeen:
					num2 = 1;
					break;
				default:
					throw new InvalidOperationException();
				}
				this.state = DotStuffingStream.State.EndOfData;
				int num3 = DotStuffingStream.EodSequence.Length - num2;
				Buffer.BlockCopy(DotStuffingStream.EodSequence, num2, buffer, offset + num, num3);
				num += num3;
			}
			this.position += (long)num;
			return num;
		}

		private static readonly byte[] EodSequence = new byte[]
		{
			13,
			10,
			46,
			13,
			10
		};

		private Stream stream;

		private DotStuffingStream.State state;

		private long position;

		private bool rejectBareLinefeeds;

		private enum State
		{
			BeginningOfLine,
			Data,
			CarriageReturnSeen,
			EndOfData
		}
	}
}
