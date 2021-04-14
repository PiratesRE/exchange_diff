using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Internal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal struct BitString
	{
		internal BitString(byte[] buffer, int offset, int length)
		{
			this.bytes = new byte[length];
			Buffer.BlockCopy(buffer, offset, this.bytes, 0, length);
		}

		internal BitString(byte[] buffer, int offset, int length, bool last)
		{
			this.bytes = new byte[last ? length : (length + 1)];
			Buffer.BlockCopy(buffer, offset, this.bytes, last ? 0 : 1, length);
			if (!last)
			{
				this.bytes[0] = byte.MaxValue;
			}
		}

		public int Length
		{
			get
			{
				if (this.bytes != null)
				{
					return this.bytes.Length;
				}
				return 0;
			}
		}

		public int LengthBits
		{
			get
			{
				if (this.bytes == null)
				{
					return 0;
				}
				if (this.bytes.Length != 0)
				{
					return this.bytes.Length * 8 - (int)this.bytes[0];
				}
				return 0;
			}
		}

		internal void AddSegment(byte[] buffer, int offset, int length, bool last)
		{
			byte[] dst = new byte[this.bytes.Length + (last ? (length - 1) : length)];
			Buffer.BlockCopy(this.bytes, 0, dst, 0, this.bytes.Length);
			Buffer.BlockCopy(buffer, last ? (offset + 1) : offset, dst, this.bytes.Length, last ? (length - 1) : length);
			this.bytes = dst;
			if (last)
			{
				this.bytes[0] = buffer[0];
			}
		}

		private byte[] bytes;
	}
}
