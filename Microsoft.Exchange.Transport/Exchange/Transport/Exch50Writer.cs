using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Transport
{
	internal class Exch50Writer
	{
		public Exch50Writer()
		{
			this.buffer = new List<byte>();
		}

		public void Add(MdbefPropertyCollection properties)
		{
			if (properties.Count == 0)
			{
				this.buffer.AddRange(Exch50Writer.Placeholder);
				return;
			}
			byte[] bytes = properties.GetBytes();
			this.buffer.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bytes.Length)));
			this.buffer.AddRange(bytes);
			while (this.buffer.Count % 4 != 0)
			{
				this.buffer.Add(0);
			}
		}

		public byte[] GetBytes()
		{
			return this.buffer.ToArray();
		}

		// Note: this type is marked as 'beforefieldinit'.
		static Exch50Writer()
		{
			byte[] placeholder = new byte[4];
			Exch50Writer.Placeholder = placeholder;
		}

		private static readonly byte[] Placeholder;

		private List<byte> buffer;
	}
}
