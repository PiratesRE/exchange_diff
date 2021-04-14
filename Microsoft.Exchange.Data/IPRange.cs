using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[CLSCompliant(true)]
	[Serializable]
	public class IPRange : IComparable<IPRange>, IComparable
	{
		private IPRange()
		{
		}

		protected IPRange(IPAddress networkAddress, short cidrLength)
		{
			short num = cidrLength;
			if (!this.IsValidCIDRLength(cidrLength, networkAddress.AddressFamily))
			{
				throw new ArgumentException(this.exceptionMessage);
			}
			if (networkAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				num += 96;
			}
			IPvxAddress pvxAddress = new IPvxAddress(networkAddress);
			if (num == 128)
			{
				this.SetAddresses(pvxAddress, pvxAddress);
				this.format = IPRange.Format.SingleAddress;
			}
			else
			{
				if (!this.SetAddressesFromCIDR(pvxAddress, cidrLength))
				{
					throw new ArgumentException(this.exceptionMessage);
				}
				this.format = IPRange.Format.CIDR;
			}
			if (!this.ValidateRange())
			{
				throw new ArgumentOutOfRangeException(this.exceptionMessage);
			}
		}

		protected IPRange(IPAddress startAddress, IPAddress endAddress)
		{
			this.SetAddresses(new IPvxAddress(startAddress), new IPvxAddress(endAddress));
			this.format = IPRange.Format.LoHi;
			if (!this.ValidateAddressFamily())
			{
				throw new FormatException(this.exceptionMessage);
			}
			if (!this.ValidateRange())
			{
				throw new ArgumentOutOfRangeException(this.exceptionMessage);
			}
		}

		public IPRange(IPvxAddress startAddress, IPvxAddress endAddress, IPRange.Format format)
		{
			this.SetAddresses(startAddress, endAddress);
			if (!this.ValidateAddressFamily())
			{
				throw new FormatException(this.exceptionMessage);
			}
			if (!this.ValidateRange())
			{
				throw new ArgumentOutOfRangeException(this.exceptionMessage);
			}
			if (!this.IsValidFormat(format))
			{
				throw new FormatException(this.exceptionMessage);
			}
			this.format = format;
		}

		protected void SetAddresses(IPvxAddress start, IPvxAddress end)
		{
			this.startAddress = start;
			this.endAddress = end;
		}

		private bool ValidateRange()
		{
			if (this.startAddress > this.endAddress)
			{
				this.exceptionMessage = DataStrings.InvalidIPRange(this.startAddress.ToString(), this.endAddress.ToString());
				this.SetAddresses(IPvxAddress.Zero, IPvxAddress.Zero);
				this.format = IPRange.Format.Invalid;
				return false;
			}
			return true;
		}

		private bool ValidateAddressFamily()
		{
			if (this.startAddress.AddressFamily != this.endAddress.AddressFamily)
			{
				this.exceptionMessage = DataStrings.AddressFamilyMismatch;
				this.SetAddresses(IPvxAddress.Zero, IPvxAddress.Zero);
				this.format = IPRange.Format.Invalid;
				return false;
			}
			return true;
		}

		public static bool operator ==(IPRange value1, IPRange value2)
		{
			return value1 == value2 || (value1 != null && value2 != null && value1.startAddress == value2.startAddress && value1.endAddress == value2.endAddress);
		}

		public static bool operator !=(IPRange value1, IPRange value2)
		{
			return !(value1 == value2);
		}

		public bool Contains(IPAddress ipAddress)
		{
			IPvxAddress ipAddress2 = new IPvxAddress(ipAddress);
			return this.Contains(ipAddress2);
		}

		public bool Contains(IPvxAddress ipAddress)
		{
			return ipAddress <= this.endAddress && ipAddress >= this.startAddress;
		}

		public bool Overlaps(IPRange ipRange)
		{
			if (ipRange == null)
			{
				throw new ArgumentNullException("ipRange");
			}
			if (this.RangeFormat == IPRange.Format.Invalid || ipRange.RangeFormat == IPRange.Format.Invalid)
			{
				throw new ArgumentException("One of the RangeFormats is invalid");
			}
			if (this.LowerBound.AddressFamily != ipRange.LowerBound.AddressFamily)
			{
				return false;
			}
			IPRange.Format rangeFormat = this.RangeFormat;
			bool result;
			if (rangeFormat == IPRange.Format.SingleAddress)
			{
				if (ipRange.RangeFormat == IPRange.Format.SingleAddress)
				{
					result = (this.LowerBound == ipRange.LowerBound);
				}
				else
				{
					result = ipRange.Contains(this.LowerBound);
				}
			}
			else if (ipRange.RangeFormat == IPRange.Format.SingleAddress)
			{
				result = this.Contains(ipRange.LowerBound);
			}
			else
			{
				result = (this.Contains(ipRange.LowerBound) || this.Contains(ipRange.UpperBound) || ipRange.Contains(this.LowerBound) || ipRange.Contains(this.UpperBound));
			}
			return result;
		}

		public bool PartiallyOverlaps(IPRange other)
		{
			ArgumentValidator.ThrowIfNull("other", other);
			return (this.LowerBound > other.LowerBound && this.LowerBound <= other.UpperBound && this.UpperBound > other.UpperBound) || (this.LowerBound < other.LowerBound && this.UpperBound >= other.LowerBound && this.UpperBound < other.UpperBound);
		}

		public static IPRange CreateSingleAddress(IPAddress address)
		{
			return new IPRange(address, (address.AddressFamily == AddressFamily.InterNetwork) ? 32 : 128);
		}

		public static IPRange CreateIPAndCIDR(IPAddress address, short cidrLength)
		{
			return new IPRange(address, cidrLength);
		}

		public static IPRange CreateIPRange(IPAddress startAddress, IPAddress endAddress)
		{
			return new IPRange(startAddress, endAddress);
		}

		public static IPRange CreateIPAndMask(IPAddress address, IPAddress mask)
		{
			return IPRange.Parse(address.ToString() + "(" + mask.ToString() + ")");
		}

		public IPvxAddress LowerBound
		{
			get
			{
				return this.startAddress;
			}
		}

		public IPvxAddress UpperBound
		{
			get
			{
				return this.endAddress;
			}
		}

		public IPAddress Netmask
		{
			get
			{
				if (this.format != IPRange.Format.CIDR && this.format != IPRange.Format.Netmask)
				{
					throw new InvalidOperationException("Netmask is only valid if the IPRange was constructed using a netmask or with CIDR notation.");
				}
				if (this.startAddress.AddressFamily == AddressFamily.InterNetwork)
				{
					return (this.startAddress ^ this.endAddress).Xor(IPvxAddress.IPv4Broadcast);
				}
				return ~(this.startAddress ^ this.endAddress);
			}
		}

		public short CIDRLength
		{
			get
			{
				if (this.format != IPRange.Format.CIDR)
				{
					throw new InvalidOperationException("CIDRLength is only valid if the IPRange was constructed as a CIDR");
				}
				IPvxAddress pvxAddress = this.startAddress ^ this.endAddress;
				int num = 0;
				bool flag;
				do
				{
					flag = false;
					byte b = (byte)pvxAddress;
					if (b <= 15)
					{
						switch (b)
						{
						case 0:
						case 2:
							break;
						case 1:
							num++;
							break;
						case 3:
							num += 2;
							break;
						default:
							if (b != 7)
							{
								if (b == 15)
								{
									num += 4;
								}
							}
							else
							{
								num += 3;
							}
							break;
						}
					}
					else if (b <= 63)
					{
						if (b != 31)
						{
							if (b == 63)
							{
								num += 6;
							}
						}
						else
						{
							num += 5;
						}
					}
					else if (b != 127)
					{
						if (b == 255)
						{
							num += 8;
							flag = true;
						}
					}
					else
					{
						num += 7;
					}
					pvxAddress >>= 8;
				}
				while (flag);
				int num2 = (this.startAddress.AddressFamily == AddressFamily.InterNetwork) ? 32 : 128;
				return (short)(num2 - num);
			}
		}

		public IPRange.Format RangeFormat
		{
			get
			{
				return this.format;
			}
		}

		public IPvxAddress Size
		{
			get
			{
				IPvxAddress result;
				try
				{
					result = this.endAddress - this.startAddress + 1UL;
				}
				catch (OverflowException)
				{
					result = new IPvxAddress(ulong.MaxValue, ulong.MaxValue);
				}
				return result;
			}
		}

		public string Expression
		{
			get
			{
				if (!string.IsNullOrEmpty(this.originalExpression))
				{
					return this.originalExpression;
				}
				return this.ToString();
			}
		}

		public static IPRange Parse(string expression)
		{
			IPRange iprange;
			if (IPRange.InternalTryParse(expression, out iprange))
			{
				return iprange;
			}
			if (string.IsNullOrEmpty(iprange.exceptionMessage))
			{
				throw new FormatException(DataStrings.InvalidIPRangeFormat(expression));
			}
			throw new FormatException(iprange.exceptionMessage);
		}

		public static bool TryParse(string expression, out IPRange range)
		{
			return IPRange.InternalTryParse(expression, out range);
		}

		public override string ToString()
		{
			switch (this.format)
			{
			case IPRange.Format.CIDR:
				return this.startAddress.ToString() + "/" + this.CIDRLength.ToString();
			case IPRange.Format.Netmask:
				return this.startAddress.ToString() + "(" + this.Netmask.ToString() + ")";
			case IPRange.Format.LoHi:
				return this.startAddress.ToString() + "-" + this.endAddress.ToString();
			case IPRange.Format.SingleAddress:
				return this.startAddress.ToString();
			default:
				return null;
			}
		}

		public override int GetHashCode()
		{
			return this.startAddress.GetHashCode() ^ this.endAddress.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			IPRange iprange = obj as IPRange;
			return !(iprange == null) && this.Equals(iprange);
		}

		public bool Equals(IPRange other)
		{
			return this.startAddress.Equals(other.startAddress) && this.endAddress.Equals(other.endAddress);
		}

		public int CompareTo(IPRange x)
		{
			if (x == null)
			{
				return 1;
			}
			int num = this.LowerBound.CompareTo(x.LowerBound);
			if (num != 0)
			{
				return num;
			}
			return this.UpperBound.CompareTo(x.UpperBound);
		}

		private static bool InternalTryParse(string expression, out IPRange range)
		{
			range = new IPRange();
			range.originalExpression = expression;
			char[] anyOf = new char[]
			{
				'-',
				'/',
				'('
			};
			int num = expression.IndexOfAny(anyOf);
			if (num <= 0)
			{
				return IPRange.InternalTryParseSingleIPAddress(expression, ref range);
			}
			if (expression[num] == '/')
			{
				int num2 = expression.IndexOfAny(anyOf, num + 1);
				if (num2 > 0)
				{
					num = num2;
				}
			}
			bool flag = false;
			char c = expression[num];
			if (c != '(')
			{
				switch (c)
				{
				case '-':
					flag = IPRange.InternalTryParseLoHi(expression, num, ref range);
					break;
				case '/':
					flag = IPRange.InternalTryParseCIDR(expression, num, ref range);
					break;
				}
			}
			else
			{
				flag = IPRange.InternalTryParseNetmask(expression, num, ref range);
			}
			if (!flag)
			{
				range.SetAddresses(IPvxAddress.Zero, IPvxAddress.Zero);
				range.format = IPRange.Format.Invalid;
				return false;
			}
			return range.ValidateRange();
		}

		private static bool InternalTryParseLoHi(string expression, int sep, ref IPRange range)
		{
			range.format = IPRange.Format.Invalid;
			int length = expression.Length;
			if (sep == 0 || expression[sep] != '-')
			{
				return false;
			}
			IPvxAddress start;
			if (!IPvxAddress.TryParse(expression.Substring(0, sep), out start))
			{
				range.exceptionMessage = DataStrings.InvalidIPAddressFormat(expression.Substring(0, sep));
				return false;
			}
			IPvxAddress end;
			if (!IPvxAddress.TryParse(expression.Substring(sep + 1, length - sep - 1), out end))
			{
				range.exceptionMessage = DataStrings.InvalidIPAddressFormat(expression.Substring(sep + 1, length - sep - 1));
				return false;
			}
			range.SetAddresses(start, end);
			if (!range.ValidateAddressFamily())
			{
				return false;
			}
			range.format = IPRange.Format.LoHi;
			return true;
		}

		private static bool InternalTryParseCIDR(string expression, int sep, ref IPRange range)
		{
			range.format = IPRange.Format.Invalid;
			int length = expression.Length;
			if (sep == 0 || expression[sep] != '/')
			{
				return false;
			}
			IPvxAddress baseAddress;
			if (!IPvxAddress.TryParse(expression.Substring(0, sep), out baseAddress))
			{
				range.exceptionMessage = DataStrings.InvalidIPAddressFormat(expression.Substring(0, sep));
				return false;
			}
			short cidrLength = 0;
			if (!short.TryParse(expression.Substring(sep + 1, length - sep - 1), out cidrLength))
			{
				range.exceptionMessage = DataStrings.InvalidCIDRLength(expression.Substring(sep + 1, length - sep - 1));
				return false;
			}
			if (!range.IsValidCIDRLength(cidrLength, baseAddress.AddressFamily))
			{
				return false;
			}
			if (!range.SetAddressesFromCIDR(baseAddress, cidrLength))
			{
				return false;
			}
			range.format = IPRange.Format.CIDR;
			return true;
		}

		private static bool InternalTryParseSingleIPAddress(string expression, ref IPRange range)
		{
			range.format = IPRange.Format.Invalid;
			IPvxAddress pvxAddress;
			if (!IPvxAddress.TryParse(expression, out pvxAddress))
			{
				range.exceptionMessage = DataStrings.InvalidIPAddressFormat(expression);
				return false;
			}
			range.SetAddresses(pvxAddress, pvxAddress);
			range.format = IPRange.Format.SingleAddress;
			return true;
		}

		private static bool InternalTryParseNetmask(string expression, int sep, ref IPRange range)
		{
			range.format = IPRange.Format.Invalid;
			int length = expression.Length;
			if (sep == 0 || expression[sep] != '(' || expression[length - 1] != ')')
			{
				return false;
			}
			IPAddress ipaddress;
			if (!IPAddress.TryParse(expression.Substring(0, sep), out ipaddress))
			{
				range.exceptionMessage = DataStrings.InvalidIPAddressFormat(expression.Substring(0, sep));
				return false;
			}
			IPAddress ipaddress2;
			if (!IPAddress.TryParse(expression.Substring(sep + 1, length - sep - 2), out ipaddress2))
			{
				range.exceptionMessage = DataStrings.InvalidIPAddressFormat(expression.Substring(sep + 1, length - sep - 2));
				return false;
			}
			if (ipaddress.AddressFamily != ipaddress2.AddressFamily)
			{
				range.exceptionMessage = DataStrings.StartingAddressAndMaskAddressFamilyMismatch;
				return false;
			}
			int i = (ipaddress.AddressFamily == AddressFamily.InterNetwork) ? 32 : 128;
			IPvxAddress pvxAddress = new IPvxAddress(ipaddress2);
			while (i > 0 && (byte)pvxAddress == 0)
			{
				i -= 8;
				pvxAddress >>= 8;
			}
			if (i == 0)
			{
				if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
				{
					range.SetAddresses(IPvxAddress.IPv4MappedMask, IPvxAddress.IPv4Broadcast);
				}
				else
				{
					range.SetAddresses(IPvxAddress.Zero, new IPvxAddress(ulong.MaxValue, ulong.MaxValue));
				}
				range.format = IPRange.Format.Netmask;
				return true;
			}
			short num = 0;
			byte b = (byte)pvxAddress;
			if (b <= 224)
			{
				if (b == 128)
				{
					num += 1;
					goto IL_1EB;
				}
				if (b == 192)
				{
					num += 2;
					goto IL_1EB;
				}
				if (b == 224)
				{
					num += 3;
					goto IL_1EB;
				}
			}
			else
			{
				if (b == 240)
				{
					num += 4;
					goto IL_1EB;
				}
				if (b == 248)
				{
					num += 5;
					goto IL_1EB;
				}
				switch (b)
				{
				case 252:
					num += 6;
					goto IL_1EB;
				case 254:
					num += 7;
					goto IL_1EB;
				case 255:
					num += 8;
					goto IL_1EB;
				}
			}
			range.exceptionMessage = DataStrings.InvalidIPAddressMask(ipaddress2.ToString());
			return false;
			IL_1EB:
			i -= 8;
			pvxAddress >>= 8;
			while (i > 0)
			{
				if ((byte)pvxAddress != 255)
				{
					range.exceptionMessage = DataStrings.InvalidIPAddressMask(ipaddress2.ToString());
					return false;
				}
				i -= 8;
				pvxAddress >>= 8;
				num += 8;
			}
			if (!range.SetAddressesFromCIDR(new IPvxAddress(ipaddress), num))
			{
				return false;
			}
			range.format = IPRange.Format.Netmask;
			return true;
		}

		private bool SetAddressesFromCIDR(IPvxAddress baseAddress, short cidrLength)
		{
			if (!this.IsValidCIDRLength(cidrLength, baseAddress.AddressFamily))
			{
				return false;
			}
			if (baseAddress.AddressFamily == AddressFamily.InterNetwork)
			{
				cidrLength += 96;
			}
			IPvxAddress pvxAddress = ~IPvxAddress.Zero >> (int)cidrLength;
			IPvxAddress pvxAddress2 = baseAddress;
			pvxAddress2 |= pvxAddress;
			baseAddress &= ~pvxAddress;
			this.SetAddresses(baseAddress, pvxAddress2);
			return true;
		}

		private bool IsValidCIDRLength(short cidrLength, AddressFamily addressFamily)
		{
			if (cidrLength < 0)
			{
				this.exceptionMessage = DataStrings.InvalidCIDRLength(cidrLength.ToString(CultureInfo.InvariantCulture));
				return false;
			}
			if (addressFamily == AddressFamily.InterNetwork)
			{
				if (cidrLength > 32)
				{
					this.exceptionMessage = DataStrings.InvalidCIDRLengthIPv4(cidrLength);
					return false;
				}
			}
			else if (cidrLength > 128)
			{
				this.exceptionMessage = DataStrings.InvalidCIDRLengthIPv6(cidrLength);
				return false;
			}
			return true;
		}

		private bool IsValidFormat(IPRange.Format format)
		{
			if (format <= IPRange.Format.Invalid || format > IPRange.Format.SingleAddress)
			{
				this.exceptionMessage = DataStrings.InvalidNotationFormat;
				return false;
			}
			return true;
		}

		int IComparable.CompareTo(object obj)
		{
			IPRange iprange = obj as IPRange;
			if (iprange == null)
			{
				throw new ArgumentException("object is not a IPRange");
			}
			return this.CompareTo(iprange);
		}

		public const string AllowedCharacters = "[0-9A-Fa-fxX./()-:]";

		private IPvxAddress startAddress;

		private IPvxAddress endAddress;

		private IPRange.Format format;

		private string exceptionMessage;

		private string originalExpression;

		public enum Format
		{
			Invalid,
			CIDR,
			Netmask,
			LoHi,
			SingleAddress
		}
	}
}
