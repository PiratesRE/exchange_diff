using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class BufferWriter : Writer
	{
		internal BufferWriter(ArraySegment<byte> buffer)
		{
			Util.ThrowOnNullArgument(buffer, "buffer");
			this.buffer = buffer;
			this.bufferRelativePosition = 0U;
		}

		internal BufferWriter(byte[] buffer) : this(new ArraySegment<byte>(buffer))
		{
		}

		internal BufferWriter(int size) : this(new byte[size])
		{
		}

		public override long Position
		{
			get
			{
				return (long)((ulong)this.bufferRelativePosition);
			}
			set
			{
				if (value < 0L || value > (long)this.buffer.Count)
				{
					throw new ArgumentOutOfRangeException(string.Format("Cannot move to position {0}. It is out of range for our buffer of size {1}", value, this.buffer.Count));
				}
				this.bufferRelativePosition = (uint)value;
			}
		}

		public uint AvailableSpace
		{
			get
			{
				return (uint)(this.buffer.Count - (int)this.bufferRelativePosition);
			}
		}

		private int BufferAbsolutePosition
		{
			get
			{
				return (int)((ulong)this.bufferRelativePosition + (ulong)((long)this.buffer.Offset));
			}
		}

		public static byte[] Serialize(BufferWriter.SerializeDelegate serializeDelegate)
		{
			uint num = 0U;
			using (CountWriter countWriter = new CountWriter())
			{
				serializeDelegate(countWriter);
				num = (uint)countWriter.Position;
			}
			byte[] result = new byte[num];
			using (Writer writer = new BufferWriter(result))
			{
				serializeDelegate(writer);
			}
			return result;
		}

		public int CopyFrom(Stream source, int count)
		{
			Util.ThrowOnNullArgument(source, "source");
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException("count");
			}
			this.CheckValidRange((uint)count);
			int num = source.Read(this.buffer.Array, this.BufferAbsolutePosition, count);
			this.bufferRelativePosition += (uint)num;
			return num;
		}

		public BufferWriter SubWriter()
		{
			return new BufferWriter(this.buffer.SubSegment((int)this.bufferRelativePosition, (int)((long)this.buffer.Count - (long)((ulong)this.bufferRelativePosition))));
		}

		public override void WriteByte(byte value)
		{
			this.CheckValidRange(1U);
			this.buffer.Array[this.BufferAbsolutePosition] = value;
			this.bufferRelativePosition += 1U;
		}

		public override void WriteDouble(double value)
		{
			this.CheckValidRange(8U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 8U;
		}

		public override void WriteSingle(float value)
		{
			this.CheckValidRange(4U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 4U;
		}

		public override void WriteGuid(Guid value)
		{
			this.CheckValidRange(16U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 16U;
		}

		public override void WriteInt32(int value)
		{
			this.CheckValidRange(4U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 4U;
		}

		public override void WriteInt64(long value)
		{
			this.CheckValidRange(8U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 8U;
		}

		public override void WriteInt16(short value)
		{
			this.CheckValidRange(2U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 2U;
		}

		public override void WriteUInt32(uint value)
		{
			this.CheckValidRange(4U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 4U;
		}

		public override void WriteUInt64(ulong value)
		{
			this.CheckValidRange(8U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 8U;
		}

		public override void WriteUInt16(ushort value)
		{
			this.CheckValidRange(2U);
			ExBitConverter.Write(value, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 2U;
		}

		public override void SkipArraySegment(ArraySegment<byte> buffer)
		{
			if (BufferWriter.verifySkippedArraySegment)
			{
				if (buffer.Array != this.buffer.Array)
				{
					throw new ArgumentException("The ArraySegment being skipped is not the current buffer in the Writer.");
				}
				if (buffer.Offset != this.BufferAbsolutePosition)
				{
					throw new ArgumentException("The ArraySegment being skipped is not in the current Writer's buffer position.");
				}
			}
			this.CheckValidRange((uint)buffer.Count);
			this.bufferRelativePosition += (uint)buffer.Count;
		}

		internal static void VerifySkippedArraySegment(bool verify)
		{
			BufferWriter.verifySkippedArraySegment = verify;
		}

		protected override void InternalWriteBytes(byte[] value, int offset, int count)
		{
			this.CheckValidRange((uint)count);
			Buffer.BlockCopy(value, offset, this.buffer.Array, this.BufferAbsolutePosition, count);
			this.bufferRelativePosition += (uint)count;
		}

		protected override void InternalWriteString(string value, int length, Encoding encoding)
		{
			this.CheckValidRange((uint)length);
			encoding.GetBytes(value, 0, value.Length, this.buffer.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += (uint)length;
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BufferWriter>(this);
		}

		private void ThrowInvalidRange()
		{
			throw new BufferTooSmallException();
		}

		private void CheckValidRange(uint size)
		{
			if (size > this.AvailableSpace)
			{
				this.ThrowInvalidRange();
			}
		}

		private static bool verifySkippedArraySegment = true;

		private readonly ArraySegment<byte> buffer;

		private uint bufferRelativePosition;

		public delegate void SerializeDelegate(Writer writer);
	}
}
