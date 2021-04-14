using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	public abstract class ProtocolAddress : IComparable, IComparable<ProtocolAddress>
	{
		protected ProtocolAddress(Protocol protocol, string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentNullException("address");
			}
			this.protocol = protocol;
			this.address = address;
		}

		public Protocol ProtocolType
		{
			get
			{
				return this.protocol;
			}
		}

		public string AddressString
		{
			get
			{
				return this.address;
			}
		}

		public sealed override string ToString()
		{
			return string.Format("{0}{1}{2}", this.protocol.ProtocolName, ':', this.address);
		}

		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this == obj as ProtocolAddress;
		}

		public static bool operator ==(ProtocolAddress a, ProtocolAddress b)
		{
			return a == b || (a != null && b != null && a.protocol.Equals(b.protocol) && a.address.Equals(b.address, StringComparison.OrdinalIgnoreCase));
		}

		public static bool operator !=(ProtocolAddress a, ProtocolAddress b)
		{
			return !(a == b);
		}

		public int CompareTo(object other)
		{
			if (other == null)
			{
				return 1;
			}
			if (!(other is ProtocolAddress))
			{
				throw new ArgumentException(DataStrings.InvalidTypeArgumentException("other", other.GetType(), typeof(ProtocolAddress)), "other");
			}
			return this.CompareTo((ProtocolAddress)other);
		}

		public int CompareTo(ProtocolAddress other)
		{
			if (other == null)
			{
				return 1;
			}
			return StringComparer.OrdinalIgnoreCase.Compare(this.ToString(), other.ToString());
		}

		public bool Equals(ProtocolAddress other)
		{
			return this == other;
		}

		public const char Separator = ':';

		private Protocol protocol;

		private string address;
	}
}
