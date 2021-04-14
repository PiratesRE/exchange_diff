using System;
using Microsoft.Exchange.Data.Mime.Internal;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public struct SmtpAddress : IEquatable<SmtpAddress>, IComparable<SmtpAddress>
	{
		public SmtpAddress(string address)
		{
			this.address = (string.IsNullOrEmpty(address) ? null : address);
		}

		public SmtpAddress(byte[] address)
		{
			this.address = ((address != null && address.Length != 0) ? MimeInternalHelpers.BytesToString(address, true) : null);
		}

		public SmtpAddress(string local, string domain)
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
			if (local != SmtpAddress.NullReversePath.address)
			{
				throw new ArgumentNullException("domain");
			}
			this = SmtpAddress.NullReversePath;
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

		public string Local
		{
			get
			{
				if (this.address == null)
				{
					return null;
				}
				int num;
				if (MimeInternalHelpers.IsValidSmtpAddress(this.address, true, out num, MimeInternalHelpers.IsEaiEnabled()))
				{
					return this.address.Substring(0, num - 1);
				}
				if (this.address == SmtpAddress.NullReversePath.address)
				{
					return this.address;
				}
				return null;
			}
		}

		public string Domain
		{
			get
			{
				if (this.address == null)
				{
					return null;
				}
				int startIndex;
				if (!MimeInternalHelpers.IsValidSmtpAddress(this.address, true, out startIndex, MimeInternalHelpers.IsEaiEnabled()))
				{
					return null;
				}
				return this.address.Substring(startIndex);
			}
		}

		public string Address
		{
			get
			{
				return this.address;
			}
		}

		public bool IsUTF8
		{
			get
			{
				return SmtpAddress.IsUTF8Address(this.address);
			}
		}

		public bool IsValidAddress
		{
			get
			{
				int num;
				return this.address != null && (MimeInternalHelpers.IsValidSmtpAddress(this.address, true, out num, MimeInternalHelpers.IsEaiEnabled()) || this.address == SmtpAddress.NullReversePath.address);
			}
		}

		public static bool IsValidSmtpAddress(string address)
		{
			if (address == null)
			{
				throw new ArgumentNullException("address");
			}
			int domainStart;
			return (MimeInternalHelpers.IsValidSmtpAddress(address, true, out domainStart, MimeInternalHelpers.IsEaiEnabled()) && !SmtpAddress.IsDomainIPLiteral(address, domainStart)) || address == SmtpAddress.NullReversePath.address;
		}

		public static bool IsUTF8Address(string address)
		{
			return !MimeInternalHelpers.IsPureASCII(address);
		}

		public static bool IsValidDomain(string domain)
		{
			if (domain == null)
			{
				throw new ArgumentNullException("domain");
			}
			return MimeInternalHelpers.IsValidDomain(domain, 0, true, MimeInternalHelpers.IsEaiEnabled()) && !SmtpAddress.IsDomainIPLiteral(domain);
		}

		public static SmtpAddress Parse(string address)
		{
			SmtpAddress result = new SmtpAddress(address);
			if (!result.IsValidAddress)
			{
				throw new FormatException(DataStrings.InvalidSmtpAddress(address));
			}
			return result;
		}

		public static bool operator ==(SmtpAddress value1, SmtpAddress value2)
		{
			return value1.Equals(value2);
		}

		public static bool operator !=(SmtpAddress value1, SmtpAddress value2)
		{
			return !(value1 == value2);
		}

		public static explicit operator string(SmtpAddress address)
		{
			return address.address ?? string.Empty;
		}

		public static explicit operator SmtpAddress(string address)
		{
			return new SmtpAddress(address);
		}

		public override int GetHashCode()
		{
			if (this.address == null)
			{
				return 0;
			}
			return StringComparer.OrdinalIgnoreCase.GetHashCode(this.address);
		}

		public int CompareTo(SmtpAddress address)
		{
			return StringComparer.OrdinalIgnoreCase.Compare(this.address, address.address);
		}

		public int CompareTo(object address)
		{
			if (address is SmtpAddress)
			{
				return this.CompareTo((SmtpAddress)address);
			}
			string text = address as string;
			if (text != null)
			{
				return string.Compare(this.address, text, StringComparison.OrdinalIgnoreCase);
			}
			if (this.address != null)
			{
				return 1;
			}
			return 0;
		}

		public byte[] GetBytes()
		{
			if (this.address == null)
			{
				return null;
			}
			return MimeInternalHelpers.StringToBytes(this.address, MimeInternalHelpers.IsEaiEnabled());
		}

		public int GetBytes(byte[] array, int offset)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (0 > offset)
			{
				throw new ArgumentOutOfRangeException("offset");
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
				throw new ArgumentException(DataStrings.InsufficientSpace);
			}
			return MimeInternalHelpers.StringToBytes(this.address, 0, length, array, offset, MimeInternalHelpers.IsEaiEnabled());
		}

		public override bool Equals(object address)
		{
			if (address is SmtpAddress)
			{
				return this.Equals((SmtpAddress)address);
			}
			string text = address as string;
			if (text != null)
			{
				return string.Equals(this.address, text, StringComparison.OrdinalIgnoreCase);
			}
			return this.address == null;
		}

		public bool Equals(SmtpAddress address)
		{
			return string.Equals(this.address, address.address, StringComparison.OrdinalIgnoreCase);
		}

		public override string ToString()
		{
			return this.address ?? string.Empty;
		}

		private static bool IsDomainIPLiteral(string domain)
		{
			return !string.IsNullOrEmpty(domain) && domain[0] == '[' && domain[domain.Length - 1] == ']';
		}

		private static bool IsDomainIPLiteral(string address, int domainStart)
		{
			return !string.IsNullOrEmpty(address) && domainStart >= 0 && domainStart < address.Length && address[domainStart] == '[' && address[address.Length - 1] == ']';
		}

		public const int MaxLength = 571;

		public const int MaxEmailNameLength = 315;

		public static readonly SmtpAddress Empty = default(SmtpAddress);

		public static readonly SmtpAddress NullReversePath = new SmtpAddress("<>");

		private readonly string address;
	}
}
