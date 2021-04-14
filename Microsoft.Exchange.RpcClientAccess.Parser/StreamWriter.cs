using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class StreamWriter : Writer
	{
		internal StreamWriter(Stream stream)
		{
			Util.ThrowOnNullArgument(stream, "stream");
			this.writer = new BinaryWriter(stream, Encoding.Unicode);
		}

		public override long Position
		{
			get
			{
				return this.writer.BaseStream.Position;
			}
			set
			{
				this.writer.BaseStream.Position = value;
			}
		}

		public override void WriteByte(byte value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		protected override void InternalWriteBytes(byte[] value, int offset, int count)
		{
			try
			{
				this.writer.Write(value, offset, count);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteDouble(double value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteSingle(float value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteGuid(Guid value)
		{
			if (this.charBytes == null)
			{
				this.charBytes = new byte[256];
			}
			int count = ExBitConverter.Write(value, this.charBytes, 0);
			base.WriteBytes(this.charBytes, 0, count);
		}

		public override void WriteInt32(int value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteInt64(long value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteInt16(short value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteUInt32(uint value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteUInt64(ulong value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void WriteUInt16(ushort value)
		{
			try
			{
				this.writer.Write(value);
			}
			catch (IOException)
			{
				throw new BufferTooSmallException();
			}
			catch (NotSupportedException)
			{
				throw new BufferTooSmallException();
			}
		}

		public override void SkipArraySegment(ArraySegment<byte> buffer)
		{
			throw new NotSupportedException("StreamWriter does not support SkipArraySegment.");
		}

		protected override void InternalWriteString(string value, int length, Encoding encoding)
		{
			base.CheckDisposed();
			if (this.charBytes == null)
			{
				this.charBytes = new byte[256];
			}
			if (length <= 256)
			{
				encoding.GetBytes(value, 0, value.Length, this.charBytes, 0);
				base.WriteBytes(this.charBytes, 0, length);
				return;
			}
			this.WriteLargeBufferString(value, encoding);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<StreamWriter>(this);
		}

		protected override void InternalDispose()
		{
			if (this.writer != null)
			{
				((IDisposable)this.writer).Dispose();
			}
			base.InternalDispose();
		}

		private unsafe void WriteLargeBufferString(string value, Encoding encoding)
		{
			int num = 0;
			int i = value.Length;
			Encoder encoder = encoding.GetEncoder();
			int num2 = 256 / encoding.GetMaxByteCount(1);
			while (i > 0)
			{
				int num3 = (i > num2) ? num2 : i;
				int bytes;
				fixed (char* ptr = value)
				{
					fixed (byte* ptr2 = this.charBytes)
					{
						bytes = encoder.GetBytes(ptr + num, num3, ptr2, 256, num3 == i);
					}
				}
				base.WriteBytes(this.charBytes, 0, bytes);
				num += num3;
				i -= num3;
			}
		}

		private const int LargeByteBufferSize = 256;

		private readonly BinaryWriter writer;

		private byte[] charBytes;
	}
}
