using System;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal sealed class CountWriter : Writer
	{
		internal CountWriter()
		{
		}

		public override long Position
		{
			get
			{
				return (long)((ulong)this.currentPosition);
			}
			set
			{
				if (value < 0L)
				{
					throw new ArgumentOutOfRangeException(string.Format("Cannot move to position {0}.", value));
				}
				this.currentPosition = (uint)value;
			}
		}

		public override void WriteByte(byte value)
		{
			this.AdvancePosition(1U);
		}

		protected override void InternalWriteBytes(byte[] value, int offset, int count)
		{
			this.AdvancePosition((uint)count);
		}

		public override void WriteDouble(double value)
		{
			this.AdvancePosition(8U);
		}

		public override void WriteSingle(float value)
		{
			this.AdvancePosition(4U);
		}

		public override void WriteGuid(Guid value)
		{
			this.AdvancePosition(16U);
		}

		public override void WriteInt32(int value)
		{
			this.AdvancePosition(4U);
		}

		public override void WriteInt64(long value)
		{
			this.AdvancePosition(8U);
		}

		public override void WriteInt16(short value)
		{
			this.AdvancePosition(2U);
		}

		public override void WriteUInt32(uint value)
		{
			this.AdvancePosition(4U);
		}

		public override void WriteUInt64(ulong value)
		{
			this.AdvancePosition(8U);
		}

		public override void WriteUInt16(ushort value)
		{
			this.AdvancePosition(2U);
		}

		public override void SkipArraySegment(ArraySegment<byte> buffer)
		{
			this.AdvancePosition((uint)buffer.Count);
		}

		protected override void InternalWriteString(string value, int length, Encoding encoding)
		{
			this.AdvancePosition((uint)length);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<CountWriter>(this);
		}

		private void AdvancePosition(uint count)
		{
			this.currentPosition += count;
		}

		private uint currentPosition;
	}
}
