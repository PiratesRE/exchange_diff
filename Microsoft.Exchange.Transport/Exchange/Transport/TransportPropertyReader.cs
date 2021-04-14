using System;

namespace Microsoft.Exchange.Transport
{
	internal class TransportPropertyReader
	{
		public TransportPropertyReader(byte[] data, int start, int length)
		{
			this.data = data;
			this.current = start;
			this.end = start + length;
		}

		public Guid Range
		{
			get
			{
				return this.range;
			}
		}

		public uint Id
		{
			get
			{
				return this.id;
			}
		}

		public byte[] Value
		{
			get
			{
				return this.value;
			}
		}

		public bool ReadNextProperty()
		{
			if (this.current == this.end)
			{
				return false;
			}
			this.range = this.ReadGuid();
			this.id = this.ReadUInt32();
			int num = this.ReadInt32();
			if (num < 0)
			{
				throw new TransportPropertyException("invalid property length");
			}
			this.value = this.Read(num);
			return true;
		}

		private byte[] Read(int length)
		{
			if (length > this.end - this.current)
			{
				throw new TransportPropertyException("unexpected end of data");
			}
			byte[] array = new byte[length];
			Buffer.BlockCopy(this.data, this.current, array, 0, length);
			this.current += length;
			return array;
		}

		private Guid ReadGuid()
		{
			byte[] b = this.Read(16);
			return new Guid(b);
		}

		private uint ReadUInt32()
		{
			if (4 > this.end - this.current)
			{
				throw new TransportPropertyException("unexpected end of data");
			}
			uint result = BitConverter.ToUInt32(this.data, this.current);
			this.current += 4;
			return result;
		}

		private int ReadInt32()
		{
			if (4 > this.end - this.current)
			{
				throw new TransportPropertyException("unexpected end of data");
			}
			int result = BitConverter.ToInt32(this.data, this.current);
			this.current += 4;
			return result;
		}

		private byte[] data;

		private int current;

		private int end;

		private Guid range;

		private uint id;

		private byte[] value;
	}
}
