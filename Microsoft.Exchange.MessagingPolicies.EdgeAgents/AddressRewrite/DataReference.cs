using System;
using System.Text;

namespace Microsoft.Exchange.MessagingPolicies.AddressRewrite
{
	internal struct DataReference
	{
		internal DataReference(byte[] buffer, int offset, int length)
		{
			this.buffer = buffer;
			this.offset = offset;
			this.length = length;
		}

		internal DataReference(string data)
		{
			this.buffer = Encoding.ASCII.GetBytes(data);
			this.offset = 0;
			this.length = this.buffer.Length;
		}

		public override string ToString()
		{
			return Encoding.ASCII.GetString(this.buffer, this.offset, this.length);
		}

		public int CompareTo(DataReference other)
		{
			int num = Math.Min(this.length, other.length);
			int num2 = this.offset;
			int num3 = other.offset;
			for (int i = 0; i < num; i++)
			{
				char c = char.ToLower((char)this.buffer[num2 + i]);
				char c2 = char.ToLower((char)other.buffer[num3 + i]);
				int num4 = (int)(c - c2);
				if (num4 != 0)
				{
					return num4;
				}
			}
			return this.length - other.length;
		}

		private byte[] buffer;

		private int offset;

		private int length;
	}
}
