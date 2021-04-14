using System;
using System.IO;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal struct ByteBuffer
	{
		public int Length
		{
			get
			{
				return this.buffer.Length;
			}
		}

		public ByteBuffer(int size)
		{
			this.buffer = new byte[size];
			this.offset = 0;
		}

		public void SkipBytes(int count)
		{
			this.offset += count;
		}

		public uint ReadUInt32()
		{
			return (uint)((int)this.buffer[this.offset++] | ((int)this.buffer[this.offset++] | ((int)this.buffer[this.offset++] | (int)this.buffer[this.offset++] << 8) << 8) << 8);
		}

		public ushort ReadUInt16()
		{
			return (ushort)((int)this.buffer[this.offset++] | (int)this.buffer[this.offset++] << 8);
		}

		public void WriteUInt32(uint value)
		{
			this.buffer[this.offset++] = (byte)value;
			this.buffer[this.offset++] = (byte)(value >> 8);
			this.buffer[this.offset++] = (byte)(value >> 16);
			this.buffer[this.offset++] = (byte)(value >> 24);
		}

		public void WriteUInt16(ushort value)
		{
			this.buffer[this.offset++] = (byte)value;
			this.buffer[this.offset++] = (byte)(value >> 8);
		}

		public void WriteContentsTo(Stream writer)
		{
			writer.Write(this.buffer, 0, this.buffer.Length);
		}

		public int ReadContentsFrom(Stream reader)
		{
			return reader.Read(this.buffer, 0, this.buffer.Length);
		}

		private byte[] buffer;

		private int offset;
	}
}
