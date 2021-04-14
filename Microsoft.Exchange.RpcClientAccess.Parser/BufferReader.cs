using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class BufferReader : Reader
	{
		internal BufferReader(ArraySegment<byte> arraySegment)
		{
			this.arraySegment = arraySegment;
		}

		public static TResult Parse<TResult>(ArraySegment<byte> arraySegment, Func<Reader, TResult> parser)
		{
			TResult result;
			using (BufferReader bufferReader = new BufferReader(arraySegment))
			{
				result = parser(bufferReader);
			}
			return result;
		}

		public BufferReader SubReader(int count)
		{
			return new BufferReader(this.arraySegment.SubSegment(this.bufferRelativePosition, count));
		}

		private int BufferAbsolutePosition
		{
			get
			{
				return this.arraySegment.Offset + this.bufferRelativePosition;
			}
		}

		public override long Length
		{
			get
			{
				return (long)this.arraySegment.Count;
			}
		}

		public override long Position
		{
			get
			{
				return (long)this.bufferRelativePosition;
			}
			set
			{
				if (value < 0L || value > (long)this.arraySegment.Count)
				{
					throw new BufferParseException(string.Format("Invalid position. Position requested = {0}. Size of underlying buffer = {1}", value, this.arraySegment.Count));
				}
				this.bufferRelativePosition = (int)value;
			}
		}

		protected override byte InternalReadByte()
		{
			byte result = this.arraySegment.Array[this.BufferAbsolutePosition];
			this.bufferRelativePosition++;
			return result;
		}

		protected override double InternalReadDouble()
		{
			double result = BitConverter.ToDouble(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 8;
			return result;
		}

		protected override short InternalReadInt16()
		{
			short result = BitConverter.ToInt16(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 2;
			return result;
		}

		protected override ushort InternalReadUInt16()
		{
			ushort result = BitConverter.ToUInt16(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 2;
			return result;
		}

		protected override int InternalReadInt32()
		{
			int result = BitConverter.ToInt32(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 4;
			return result;
		}

		protected override uint InternalReadUInt32()
		{
			uint result = BitConverter.ToUInt32(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 4;
			return result;
		}

		protected override long InternalReadInt64()
		{
			long result = BitConverter.ToInt64(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 8;
			return result;
		}

		protected override ulong InternalReadUInt64()
		{
			ulong result = BitConverter.ToUInt64(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 8;
			return result;
		}

		protected override float InternalReadSingle()
		{
			float result = BitConverter.ToSingle(this.arraySegment.Array, this.BufferAbsolutePosition);
			this.bufferRelativePosition += 4;
			return result;
		}

		protected override ArraySegment<byte> InternalReadArraySegment(uint count)
		{
			ArraySegment<byte> result = this.arraySegment.SubSegment(this.bufferRelativePosition, (int)count);
			this.bufferRelativePosition += (int)count;
			return result;
		}

		protected override ArraySegment<byte> InternalReadArraySegmentForString(int maxCount)
		{
			int count = Math.Min(maxCount, this.arraySegment.Count - this.bufferRelativePosition);
			return this.InternalReadArraySegment((uint)count);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<BufferReader>(this);
		}

		private readonly ArraySegment<byte> arraySegment;

		private int bufferRelativePosition;
	}
}
