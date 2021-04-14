using System;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Conversion;

namespace Microsoft.Exchange.Data
{
	[CLSCompliant(true)]
	[Serializable]
	public struct IPvxAddress : IEquatable<IPvxAddress>, IComparable<IPvxAddress>
	{
		[CLSCompliant(false)]
		public IPvxAddress(IPvxAddress other)
		{
			this.highBytes = other.highBytes;
			this.lowBytes = other.lowBytes;
		}

		[CLSCompliant(false)]
		public IPvxAddress(ulong high, ulong low)
		{
			this.highBytes = high;
			this.lowBytes = low;
		}

		public IPvxAddress(IPAddress address)
		{
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				byte[] addressBytes = address.GetAddressBytes();
				int network = BitConverter.ToInt32(addressBytes, 0);
				this.lowBytes = (ulong)((long)IPAddress.NetworkToHostOrder(network) & (long)((ulong)-1));
				this.highBytes = 0UL;
				this.lowBytes |= 281470681743360UL;
				return;
			}
			if (address.AddressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentException(string.Format("Unsupported address family {0}", (address == null) ? string.Empty : address.AddressFamily.ToString()), "address");
			}
			byte[] addressBytes2 = address.GetAddressBytes();
			long network2 = BitConverter.ToInt64(addressBytes2, 0);
			this.highBytes = (ulong)IPAddress.NetworkToHostOrder(network2);
			network2 = BitConverter.ToInt64(addressBytes2, 8);
			this.lowBytes = (ulong)IPAddress.NetworkToHostOrder(network2);
			if (this.IsIPv4Compatible)
			{
				this.lowBytes |= 281470681743360UL;
				return;
			}
		}

		public IPvxAddress(byte[] address)
		{
			if (address.Length == 4)
			{
				int network = BitConverter.ToInt32(address, 0);
				this.lowBytes = (ulong)((long)IPAddress.NetworkToHostOrder(network));
				this.highBytes = 0UL;
				this.lowBytes |= 281470681743360UL;
				return;
			}
			if (address.Length != 16)
			{
				throw new ArgumentException("Bad IP address", "address");
			}
			long network2 = BitConverter.ToInt64(address, 0);
			this.highBytes = (ulong)IPAddress.NetworkToHostOrder(network2);
			network2 = BitConverter.ToInt64(address, 8);
			this.lowBytes = (ulong)IPAddress.NetworkToHostOrder(network2);
			if (this.IsIPv4Compatible)
			{
				this.lowBytes |= 281470681743360UL;
				return;
			}
		}

		public IPvxAddress(long newAddress)
		{
			if (newAddress < 0L || newAddress > (long)((ulong)-1))
			{
				throw new ArgumentOutOfRangeException("newAddress", newAddress, "Unexpected IPv4 address.");
			}
			this.lowBytes = (ulong)IPAddress.NetworkToHostOrder((int)newAddress);
			this.lowBytes |= 281470681743360UL;
			this.highBytes = 0UL;
		}

		public static implicit operator IPAddress(IPvxAddress address)
		{
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				long newAddress = (long)((ulong)IPAddress.HostToNetworkOrder((int)address.lowBytes));
				return new IPAddress(newAddress);
			}
			return new IPAddress(address.GetBytes(), 0L);
		}

		public static explicit operator byte(IPvxAddress address)
		{
			return (byte)address.lowBytes;
		}

		[CLSCompliant(false)]
		public static explicit operator ushort(IPvxAddress address)
		{
			return (ushort)address.lowBytes;
		}

		[CLSCompliant(false)]
		public static explicit operator uint(IPvxAddress address)
		{
			return (uint)address.lowBytes;
		}

		[CLSCompliant(false)]
		public static explicit operator ulong(IPvxAddress address)
		{
			return address.lowBytes;
		}

		public bool IsIPv4Mapped
		{
			get
			{
				return (this.lowBytes & 18446744069414584320UL) == 281470681743360UL && this.highBytes == 0UL;
			}
		}

		public bool IsIPv4Compatible
		{
			get
			{
				return this.highBytes == 0UL && this.lowBytes != 1UL && (this.lowBytes & 18446744069414584320UL) == 0UL && (this.lowBytes & (ulong)-65536) != 0UL;
			}
		}

		public bool IsIPv6SiteLocal
		{
			get
			{
				return (this.highBytes & 18428729675200069632UL) == 18356672081162141696UL;
			}
		}

		public bool IsIPv6LinkLocal
		{
			get
			{
				return (this.highBytes & 18428729675200069632UL) == 18410715276690587648UL;
			}
		}

		public AddressFamily AddressFamily
		{
			get
			{
				if (!this.IsIPv4Mapped && !this.IsIPv4Compatible)
				{
					return AddressFamily.InterNetworkV6;
				}
				return AddressFamily.InterNetwork;
			}
		}

		public bool IsBroadcast
		{
			get
			{
				return this.AddressFamily != AddressFamily.InterNetworkV6 && this.Equals(IPvxAddress.IPv4Broadcast);
			}
		}

		public bool IsMulticast
		{
			get
			{
				if (this.AddressFamily == AddressFamily.InterNetworkV6)
				{
					return (this.highBytes & 18374686479671623680UL) == 18374686479671623680UL;
				}
				return (this.lowBytes & (ulong)-268435456) == (ulong)-536870912;
			}
		}

		internal ulong LowBytes
		{
			get
			{
				return this.lowBytes;
			}
		}

		internal ulong HighBytes
		{
			get
			{
				return this.highBytes;
			}
		}

		public static bool TryParse(string ipString, out IPvxAddress address)
		{
			IPAddress address2;
			if (!IPAddress.TryParse(ipString, out address2))
			{
				address = IPvxAddress.Zero;
				return false;
			}
			address = new IPvxAddress(address2);
			return true;
		}

		public static IPvxAddress Parse(string ipString)
		{
			IPAddress address = IPAddress.Parse(ipString);
			return new IPvxAddress(address);
		}

		public static bool operator ==(IPvxAddress v1, IPvxAddress comparand)
		{
			return v1.Equals(comparand);
		}

		public static bool operator !=(IPvxAddress v1, IPvxAddress comparand)
		{
			return !v1.Equals(comparand);
		}

		public static bool operator <(IPvxAddress v1, IPvxAddress comparand)
		{
			return IPvxAddress.Compare(v1, comparand) == -1;
		}

		public static bool operator <=(IPvxAddress v1, IPvxAddress comparand)
		{
			return IPvxAddress.Compare(v1, comparand) != 1;
		}

		public static bool operator >(IPvxAddress v1, IPvxAddress comparand)
		{
			return IPvxAddress.Compare(v1, comparand) == 1;
		}

		public static bool operator >=(IPvxAddress v1, IPvxAddress comparand)
		{
			return IPvxAddress.Compare(v1, comparand) != -1;
		}

		[CLSCompliant(false)]
		public static IPvxAddress operator +(IPvxAddress v1, ulong operand)
		{
			ulong num = v1.highBytes;
			ulong num2 = v1.lowBytes + operand;
			if (num2 < v1.lowBytes)
			{
				if (num == 18446744073709551615UL)
				{
					throw new OverflowException();
				}
				num += 1UL;
			}
			return new IPvxAddress(num, num2);
		}

		[CLSCompliant(false)]
		public static IPvxAddress operator -(IPvxAddress v1, IPvxAddress v2)
		{
			ulong num = v1.highBytes - v2.highBytes;
			ulong num2 = v1.lowBytes - v2.lowBytes;
			if (num > v1.highBytes)
			{
				throw new OverflowException();
			}
			if (num2 > v1.lowBytes)
			{
				if (num == 0UL)
				{
					throw new OverflowException();
				}
				num -= 1UL;
			}
			return new IPvxAddress(num, num2);
		}

		public static IPvxAddress operator ^(IPvxAddress v1, IPvxAddress mask)
		{
			return v1.Xor(mask);
		}

		[CLSCompliant(false)]
		public static IPvxAddress operator ^(IPvxAddress v1, ulong mask)
		{
			return v1.Xor((long)mask);
		}

		public static IPvxAddress operator ^(IPvxAddress v1, long mask)
		{
			return v1.Xor(mask);
		}

		public static IPvxAddress operator &(IPvxAddress v1, IPvxAddress mask)
		{
			return v1.BitwiseAnd(mask);
		}

		[CLSCompliant(false)]
		public static IPvxAddress operator &(IPvxAddress v1, ulong mask)
		{
			return v1.BitwiseAnd((long)mask);
		}

		public static IPvxAddress operator &(IPvxAddress v1, long mask)
		{
			return v1.BitwiseAnd(mask);
		}

		public static IPvxAddress operator |(IPvxAddress v1, IPvxAddress mask)
		{
			return v1.BitwiseOr(mask);
		}

		[CLSCompliant(false)]
		public static IPvxAddress operator |(IPvxAddress v1, ulong mask)
		{
			return v1.BitwiseOr((long)mask);
		}

		public static IPvxAddress operator |(IPvxAddress v1, long mask)
		{
			return v1.BitwiseOr(mask);
		}

		public static IPvxAddress operator ~(IPvxAddress v1)
		{
			return v1.OnesComplement();
		}

		public static IPvxAddress operator <<(IPvxAddress v1, int shift)
		{
			return v1.LeftShift(shift);
		}

		public static IPvxAddress operator >>(IPvxAddress v1, int shift)
		{
			return v1.RightShift(shift);
		}

		public IPvxAddress Xor(IPvxAddress mask)
		{
			return new IPvxAddress(this.highBytes ^ mask.highBytes, this.lowBytes ^ mask.lowBytes);
		}

		public IPvxAddress Xor(long mask)
		{
			return new IPvxAddress(this.highBytes, this.lowBytes ^ (ulong)mask);
		}

		public IPvxAddress BitwiseAnd(IPvxAddress mask)
		{
			return new IPvxAddress(this.highBytes & mask.highBytes, this.lowBytes & mask.lowBytes);
		}

		public IPvxAddress BitwiseAnd(long mask)
		{
			return new IPvxAddress(0UL, this.lowBytes & (ulong)mask);
		}

		public IPvxAddress BitwiseOr(IPvxAddress mask)
		{
			return new IPvxAddress(this.highBytes | mask.highBytes, this.lowBytes | mask.lowBytes);
		}

		public IPvxAddress BitwiseOr(long mask)
		{
			return new IPvxAddress(this.highBytes, this.lowBytes | (ulong)mask);
		}

		public IPvxAddress OnesComplement()
		{
			return new IPvxAddress(~this.highBytes, ~this.lowBytes);
		}

		public IPvxAddress LeftShift(int shift)
		{
			if (shift == 0)
			{
				return this;
			}
			if (shift >= 128)
			{
				return IPvxAddress.Zero;
			}
			if (shift == 64)
			{
				return new IPvxAddress(this.lowBytes, 0UL);
			}
			if (shift > 64)
			{
				return new IPvxAddress(this.lowBytes << shift - 64, 0UL);
			}
			ulong num = this.lowBytes >> 64 - shift;
			return new IPvxAddress(this.highBytes << shift | num, this.lowBytes << shift);
		}

		public IPvxAddress RightShift(int shift)
		{
			if (shift == 0)
			{
				return this;
			}
			if (shift >= 128)
			{
				return IPvxAddress.Zero;
			}
			if (shift == 64)
			{
				return new IPvxAddress(0UL, this.highBytes);
			}
			if (shift > 64)
			{
				return new IPvxAddress(0UL, this.highBytes >> shift - 64);
			}
			ulong num = this.highBytes << 64 - shift;
			return new IPvxAddress(this.highBytes >> shift, num | this.lowBytes >> shift);
		}

		public byte[] GetBytes()
		{
			byte[] array = new byte[16];
			long value = IPAddress.HostToNetworkOrder((long)this.highBytes);
			ExBitConverter.Write(value, array, 0);
			value = IPAddress.HostToNetworkOrder((long)this.lowBytes);
			ExBitConverter.Write(value, array, 8);
			return array;
		}

		public override bool Equals(object comparand)
		{
			if (comparand is IPvxAddress)
			{
				return this.Equals((IPvxAddress)comparand);
			}
			IPAddress ipaddress = comparand as IPAddress;
			return ipaddress != null && this.Equals(ipaddress);
		}

		public bool Equals(IPAddress comparand)
		{
			return this.Equals(new IPvxAddress(comparand));
		}

		public bool Equals(IPvxAddress comparand)
		{
			return this.highBytes == comparand.highBytes && this.lowBytes == comparand.lowBytes;
		}

		public static bool Equals(IPvxAddress v1, IPvxAddress v2)
		{
			return v1.Equals(v2);
		}

		public static int Compare(IPvxAddress v1, IPvxAddress v2)
		{
			return v1.CompareTo(v2);
		}

		public int CompareTo(object value)
		{
			if (value == null)
			{
				return 1;
			}
			if (value is IPvxAddress)
			{
				return this.CompareTo((IPvxAddress)value);
			}
			IPAddress ipaddress = value as IPAddress;
			if (ipaddress != null)
			{
				return this.CompareTo(ipaddress);
			}
			throw new ArgumentException("Argument must be IP address");
		}

		public int CompareTo(IPAddress value)
		{
			return this.CompareTo(new IPvxAddress(value));
		}

		public int CompareTo(IPvxAddress value)
		{
			if (this.highBytes > value.highBytes)
			{
				return 1;
			}
			if (this.highBytes < value.highBytes)
			{
				return -1;
			}
			if (this.lowBytes > value.lowBytes)
			{
				return 1;
			}
			if (this.lowBytes < value.lowBytes)
			{
				return -1;
			}
			return 0;
		}

		public override int GetHashCode()
		{
			return (int)(this.highBytes & (ulong)-1) ^ (int)(this.highBytes >> 16 & (ulong)-1) ^ (int)(this.lowBytes & (ulong)-1) ^ (int)(this.lowBytes >> 16 & (ulong)-1);
		}

		public override string ToString()
		{
			IPAddress ipaddress = this;
			return ipaddress.ToString();
		}

		private const ulong IPv4MappedMaskLow = 281470681743360UL;

		public static readonly IPvxAddress IPv4Loopback = new IPvxAddress(0UL, 281472812449793UL);

		public static readonly IPvxAddress IPv6Loopback = new IPvxAddress(0UL, 1UL);

		public static readonly IPvxAddress None = new IPvxAddress(0UL, 0UL);

		public static readonly IPvxAddress Zero = IPvxAddress.None;

		internal static readonly IPvxAddress IPv4MappedMask = new IPvxAddress(0UL, 281470681743360UL);

		public static readonly IPvxAddress IPv4Broadcast = new IPvxAddress(0UL, 281474976710655UL);

		private ulong highBytes;

		private ulong lowBytes;
	}
}
