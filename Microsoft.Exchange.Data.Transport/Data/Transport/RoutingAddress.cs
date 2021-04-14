using System;
using Microsoft.Exchange.Data.Internal;
using Microsoft.Exchange.Data.Mime;

namespace Microsoft.Exchange.Data.Transport
{
	[Serializable]
	public struct RoutingAddress : IEquatable<RoutingAddress>, IComparable<RoutingAddress>
	{
		public RoutingAddress(string address)
		{
			this.address = (string.IsNullOrEmpty(address) ? null : address);
		}

		internal RoutingAddress(byte[] address)
		{
			this.address = ((address != null && address.Length != 0) ? ByteString.BytesToString(address, true) : null);
		}

		public RoutingAddress(string local, string domain)
		{
			if (local == null)
			{
				throw new ArgumentNullException("local");
			}
			if (domain != null)
			{
				this.address = local + "@" + domain;
				return;
			}
			if (local != RoutingAddress.NullReversePath.address)
			{
				throw new ArgumentNullException("domain");
			}
			this = RoutingAddress.NullReversePath;
		}

		public int Length
		{
			get
			{
				if (this.address != null)
				{
					return this.address.Length;
				}
				return 0;
			}
		}

		public string LocalPart
		{
			get
			{
				if (this.address == null)
				{
					return null;
				}
				int num;
				if (MimeAddressParser.IsValidSmtpAddress(this.address, true, out num, RoutingAddress.IsEaiEnabled()))
				{
					return this.address.Substring(0, num - 1);
				}
				if (this.address == RoutingAddress.NullReversePath.address)
				{
					return this.address;
				}
				return null;
			}
		}

		public string DomainPart
		{
			get
			{
				if (this.address == null)
				{
					return null;
				}
				int startIndex;
				if (!MimeAddressParser.IsValidSmtpAddress(this.address, true, out startIndex, RoutingAddress.IsEaiEnabled()))
				{
					return null;
				}
				return this.address.Substring(startIndex);
			}
		}

		public bool IsValid
		{
			get
			{
				return this.address != null && RoutingAddress.IsValidAddress(this.address);
			}
		}

		public bool IsUTF8
		{
			get
			{
				return RoutingAddress.IsUTF8Address(this.address);
			}
		}

		public static bool IsValidAddress(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			int domainStart;
			return (MimeAddressParser.IsValidSmtpAddress(address, true, out domainStart, RoutingAddress.IsEaiEnabled()) && !RoutingAddress.IsValidDomainIPLiteral(address, domainStart)) || address == RoutingAddress.NullReversePath.address;
		}

		public static bool IsUTF8Address(string address)
		{
			return !MimeString.IsPureASCII(address);
		}

		internal static bool IsValidDomain(string domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			return MimeAddressParser.IsValidDomain(domain, 0, true, RoutingAddress.IsEaiEnabled()) && !RoutingAddress.IsValidDomainIPLiteral(domain);
		}

		public static RoutingAddress Parse(string address)
		{
			RoutingAddress routingAddress = new RoutingAddress(address);
			if (!routingAddress.IsValid)
			{
				throw new FormatException(string.Format("The specified address is an invalid SMTP address - {0}", routingAddress));
			}
			return routingAddress;
		}

		public static bool IsEmpty(RoutingAddress address)
		{
			return RoutingAddress.Empty == address;
		}

		public static bool operator ==(RoutingAddress value1, RoutingAddress value2)
		{
			return value1.Equals(value2);
		}

		public static bool operator !=(RoutingAddress value1, RoutingAddress value2)
		{
			return !(value1 == value2);
		}

		public static explicit operator string(RoutingAddress address)
		{
			return address.address ?? string.Empty;
		}

		public static explicit operator RoutingAddress(string address)
		{
			return new RoutingAddress(address);
		}

		internal static bool IsDomainIPLiteral(string domain)
		{
			return !string.IsNullOrEmpty(domain) && (domain[0] == '[' && domain[domain.Length - 1] == ']') && MimeAddressParser.IsValidDomain(domain, 0, true, false);
		}

		public override int GetHashCode()
		{
			if (this.address == null)
			{
				return 0;
			}
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this.address);
		}

		public int CompareTo(RoutingAddress address)
		{
			return StringComparer.OrdinalIgnoreCase.Compare(this.address, address.address);
		}

		public int CompareTo(object address)
		{
			if (!(address is RoutingAddress))
			{
				throw new ArgumentException("Argument is not a RoutingAddress", "address");
			}
			return this.CompareTo((RoutingAddress)address);
		}

		internal byte[] GetBytes()
		{
			if (this.address == null)
			{
				return null;
			}
			return ByteString.StringToBytes(this.address, RoutingAddress.IsEaiEnabled());
		}

		internal int GetBytes(byte[] array, int offset)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (0 > offset)
			{
				throw new ArgumentOutOfRangeException("offset", string.Format("offset value must be non-negative - {0}", offset));
			}
			if (this.address == null)
			{
				return 0;
			}
			int length = this.Length;
			if (length == 0)
			{
				return 0;
			}
			if (length > array.Length - offset)
			{
				throw new ArgumentException(string.Format("Insufficient array space to store the values - required {0}, actual {1}", length, array.Length - offset));
			}
			return ByteString.StringToBytes(this.address, 0, length, array, offset, RoutingAddress.IsEaiEnabled());
		}

		public override bool Equals(object obj)
		{
			if (!(obj is RoutingAddress))
			{
				throw new ArgumentException("Argument is not a RoutingAddress", "obj");
			}
			return this.Equals((RoutingAddress)obj);
		}

		public bool Equals(RoutingAddress obj)
		{
			return string.Equals(this.address, obj.address, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return this.address ?? string.Empty;
		}

		private static bool IsValidDomainIPLiteral(string domain)
		{
			return !string.IsNullOrEmpty(domain) && domain[0] == '[' && domain[domain.Length - 1] == ']';
		}

		private static bool IsValidDomainIPLiteral(string address, int domainStart)
		{
			return !string.IsNullOrEmpty(address) && domainStart >= 0 && domainStart < address.Length && address[domainStart] == '[' && address[address.Length - 1] == ']';
		}

		private static bool IsEaiEnabled()
		{
			bool result;
			try
			{
				result = InternalConfiguration.IsEaiEnabled();
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		public static readonly RoutingAddress Empty = default(RoutingAddress);

		public static readonly RoutingAddress NullReversePath = new RoutingAddress("<>");

		internal static readonly RoutingAddress PostMasterAddress = new RoutingAddress("postmaster");

		private readonly string address;
	}
}
