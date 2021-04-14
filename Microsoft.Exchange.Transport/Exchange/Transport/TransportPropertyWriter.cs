using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport
{
	internal class TransportPropertyWriter
	{
		public TransportPropertyWriter()
		{
			this.buffer = new List<byte>();
		}

		public int Length
		{
			get
			{
				return this.buffer.Count;
			}
		}

		public void Add(Guid range, uint id, byte[] value)
		{
			this.buffer.AddRange(range.ToByteArray());
			this.buffer.AddRange(BitConverter.GetBytes(id));
			this.buffer.AddRange(BitConverter.GetBytes(value.Length));
			this.buffer.AddRange(value);
		}

		public byte[] GetBytes()
		{
			return this.buffer.ToArray();
		}

		private List<byte> buffer;
	}
}
