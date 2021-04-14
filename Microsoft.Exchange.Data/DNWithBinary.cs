using System;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class DNWithBinary
	{
		public string DistinguishedName
		{
			get
			{
				return this.dn;
			}
			set
			{
				this.dn = value;
			}
		}

		public byte[] Binary
		{
			get
			{
				return this.binary;
			}
			set
			{
				this.binary = value;
			}
		}

		private DNWithBinary()
		{
		}

		public DNWithBinary(string dn, byte[] binary)
		{
			this.dn = dn;
			this.binary = binary;
		}

		public static DNWithBinary Parse(string expression)
		{
			DNWithBinary result = null;
			if (!DNWithBinary.TryParse(expression, out result))
			{
				throw new FormatException(DataStrings.DNWithBinaryFormatError(expression));
			}
			return result;
		}

		public static bool TryParse(string expression, out DNWithBinary dnWithBinary)
		{
			dnWithBinary = null;
			int num = -1;
			int i = num + 1;
			num = expression.IndexOf(':', i);
			if (num != 1 || expression[0] != 'B')
			{
				return false;
			}
			i = num + 1;
			num = expression.IndexOf(':', i);
			if (num == -1)
			{
				return false;
			}
			int num2 = 0;
			while (i < num)
			{
				char c = expression[i];
				if (c < '0' || c > '9')
				{
					return false;
				}
				num2 = 10 * num2 + (int)(c - '0');
				i++;
			}
			if ((num2 & 1) != 0)
			{
				return false;
			}
			i = num + 1;
			num = expression.IndexOf(':', i);
			if (num == -1 || num2 != num - i)
			{
				return false;
			}
			byte[] array = new byte[num2 / 2];
			int num3 = 0;
			while (i + 1 < num)
			{
				byte b = 0;
				char c2 = expression[i];
				if (c2 >= '0' && c2 <= '9')
				{
					b |= (byte)(c2 - '0');
				}
				else if (c2 >= 'A' && c2 <= 'F')
				{
					b |= (byte)(c2 - 'A' + '\n');
				}
				else
				{
					if (c2 < 'a' || c2 > 'f')
					{
						return false;
					}
					b |= (byte)(c2 - 'a' + '\n');
				}
				b = (byte)(b << 4);
				c2 = expression[i + 1];
				if (c2 >= '0' && c2 <= '9')
				{
					b |= (byte)(c2 - '0');
				}
				else if (c2 >= 'A' && c2 <= 'F')
				{
					b |= (byte)(c2 - 'A' + '\n');
				}
				else
				{
					if (c2 < 'a' || c2 > 'f')
					{
						return false;
					}
					b |= (byte)(c2 - 'a' + '\n');
				}
				array[num3] = b;
				num3++;
				i += 2;
			}
			i = num + 1;
			if (string.CompareOrdinal(expression, i, "<GUID=", 0, "<GUID=".Length) == 0)
			{
				int num4 = expression.IndexOf(';', i);
				if (num4 < 0)
				{
					return false;
				}
				i = num4 + 1;
				if (string.CompareOrdinal(expression, i, "<SID=", 0, "<SID=".Length) == 0)
				{
					num4 = expression.IndexOf(';', i);
					if (num4 < 0)
					{
						return false;
					}
					i = num4 + 1;
				}
			}
			string text = expression.Substring(i);
			dnWithBinary = new DNWithBinary(text, array);
			return true;
		}

		public override string ToString()
		{
			string arg = HexConverter.ByteArrayToHexString(this.binary);
			return string.Format("B:{0}:{1}:{2}", this.binary.Length * 2, arg, this.dn);
		}

		public static bool operator ==(DNWithBinary value1, DNWithBinary value2)
		{
			if (value1 == value2)
			{
				return true;
			}
			if (value1 == null || value2 == null)
			{
				return false;
			}
			if (!value1.dn.Equals(value2.dn, StringComparison.OrdinalIgnoreCase))
			{
				return false;
			}
			if (value1.binary.Length != value2.binary.Length)
			{
				return false;
			}
			for (int i = 0; i < value1.binary.Length; i++)
			{
				if (value1.binary[i] != value2.binary[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool operator !=(DNWithBinary value1, DNWithBinary value2)
		{
			return !(value1 == value2);
		}

		public override int GetHashCode()
		{
			return this.dn.GetHashCode() ^ this.binary.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			DNWithBinary dnwithBinary = obj as DNWithBinary;
			return !(dnwithBinary == null) && this.Equals(dnwithBinary);
		}

		public bool Equals(DNWithBinary other)
		{
			return this == other;
		}

		private const string GuidDNPrefix = "<GUID=";

		private const string SidDNPrefix = "<SID=";

		private string dn;

		private byte[] binary;
	}
}
