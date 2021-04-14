using System;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal class StringBlock : Block<byte>
	{
		public StringBlock() : base(StringBlock.BlockSize)
		{
		}

		internal int AppendUnsafe(string data)
		{
			int written = this.written;
			for (int i = 0; i < data.Length; i++)
			{
				this.buffer[this.written + i] = (byte)data[i];
			}
			this.buffer[this.written + data.Length] = 0;
			this.free -= data.Length + 1;
			this.written += data.Length + 1;
			return written;
		}

		internal void ReadStringUnsafe(int offset, ref byte[] outBuffer, out int bytesRead)
		{
			int num = 0;
			while (this.buffer[offset + num] != 0)
			{
				outBuffer[num] = this.buffer[offset + num];
				num++;
			}
			bytesRead = num;
		}

		internal int FindOffsetPreviousString(int offset)
		{
			offset -= 2;
			while (offset >= 0 && base[offset] != 0)
			{
				offset--;
			}
			return offset + 1;
		}

		internal int FindOffsetNextString(int offset)
		{
			while (base[offset] != 0)
			{
				offset++;
			}
			return offset + 1;
		}

		internal void GetDataReference(int offset, out byte[] buffer, out int length)
		{
			buffer = this.buffer;
			length = 0;
			while (this.buffer[offset + length] != 0)
			{
				length++;
			}
		}

		internal static int BlockSize = 131072;
	}
}
