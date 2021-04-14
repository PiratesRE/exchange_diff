using System;
using System.Globalization;
using System.Text;

namespace Microsoft.Exchange.InfoWorker.Common
{
	internal sealed class ByteArray
	{
		public ByteArray(byte[] bytes)
		{
			this.bytes = bytes;
		}

		public override string ToString()
		{
			if (this.bytes == null)
			{
				return "<null>";
			}
			if (this.bytes.Length > 0)
			{
				StringBuilder stringBuilder = new StringBuilder(this.bytes.Length * 2);
				for (int i = 0; i < this.bytes.Length; i++)
				{
					stringBuilder.Append(this.bytes[i].ToString("X2"));
				}
				return stringBuilder.ToString();
			}
			return "<empty>";
		}

		internal byte[] Bytes
		{
			get
			{
				return this.bytes;
			}
		}

		internal static ByteArray Parse(string hexString)
		{
			if (hexString == null)
			{
				throw new ArgumentNullException("hexString");
			}
			if (hexString.Length == 0 || hexString.Length % 2 != 0)
			{
				throw new ArgumentException("Invalid hex encoded string", "hexString");
			}
			byte[] array = new byte[hexString.Length / 2];
			for (int i = 0; i < hexString.Length; i += 2)
			{
				string s = hexString.Substring(i, 2);
				byte b;
				if (!byte.TryParse(s, NumberStyles.HexNumber, null, out b))
				{
					throw new ArgumentException("Invalid hex encoded string", "hexString");
				}
				array[i / 2] = b;
			}
			return new ByteArray(array);
		}

		private byte[] bytes;
	}
}
