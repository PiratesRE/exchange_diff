using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal struct BufDeserializer
	{
		public BufDeserializer(byte[] workingBuf, int startOffset)
		{
			this.serializeIx = startOffset;
			this.buf = workingBuf;
		}

		public void Reset(byte[] workingBuf, int startOffset)
		{
			this.serializeIx = startOffset;
			this.buf = workingBuf;
		}

		public long ExtractInt64()
		{
			long result = BitConverter.ToInt64(this.buf, this.serializeIx);
			this.serializeIx += 8;
			return result;
		}

		public ulong ExtractUInt64()
		{
			ulong result = BitConverter.ToUInt64(this.buf, this.serializeIx);
			this.serializeIx += 8;
			return result;
		}

		public int ExtractInt32()
		{
			int result = BitConverter.ToInt32(this.buf, this.serializeIx);
			this.serializeIx += 4;
			return result;
		}

		public uint ExtractUInt32()
		{
			uint result = BitConverter.ToUInt32(this.buf, this.serializeIx);
			this.serializeIx += 4;
			return result;
		}

		public ushort ExtractUInt16()
		{
			ushort result = BitConverter.ToUInt16(this.buf, this.serializeIx);
			this.serializeIx += 2;
			return result;
		}

		public DateTime ExtractDateTime()
		{
			long dateData = this.ExtractInt64();
			return DateTime.FromBinary(dateData);
		}

		private int serializeIx;

		private byte[] buf;
	}
}
