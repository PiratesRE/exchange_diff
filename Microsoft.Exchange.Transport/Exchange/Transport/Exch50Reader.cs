using System;
using System.Net;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Transport
{
	internal class Exch50Reader
	{
		public Exch50Reader(byte[] data, int start, int length)
		{
			this.data = data;
			this.current = start;
			this.end = start + length;
		}

		public bool ReadNextBlob()
		{
			if (this.current >= this.end)
			{
				return false;
			}
			this.blobLength = this.ReadNetworkInt32();
			if (this.blobLength < 0 || this.blobLength > this.end - this.current)
			{
				throw new Exch50Exception("invalid blob length");
			}
			this.blobOffset = this.current;
			this.current += this.blobLength + 3 >> 2 << 2;
			return true;
		}

		public MdbefPropertyCollection GetMdbefProperties()
		{
			if (this.blobLength == 0)
			{
				return null;
			}
			return MdbefPropertyCollection.Create(this.data, this.blobOffset, this.blobLength);
		}

		private int ReadNetworkInt32()
		{
			if (4 > this.end - this.current)
			{
				throw new Exch50Exception("unexpected end of data");
			}
			int result = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(this.data, this.current));
			this.current += 4;
			return result;
		}

		private byte[] data;

		private int current;

		private int end;

		private int blobOffset;

		private int blobLength;
	}
}
