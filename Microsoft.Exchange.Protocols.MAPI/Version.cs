using System;

namespace Microsoft.Exchange.Protocols.MAPI
{
	public struct Version
	{
		public Version(ulong version)
		{
			this.version = version;
		}

		public Version(ushort productMajor, ushort productMinor, ushort buildMajor, ushort buildMinor)
		{
			this.version = ((ulong)productMajor << 48 | (ulong)productMinor << 32 | (ulong)buildMajor << 16 | (ulong)buildMinor);
		}

		public static bool operator >(Version left, Version right)
		{
			return left.version > right.version;
		}

		public static bool operator <(Version left, Version right)
		{
			return left.version < right.version;
		}

		public ulong Value
		{
			get
			{
				return this.version;
			}
		}

		public ushort ProductMajor
		{
			get
			{
				return (ushort)(this.version >> 48 & 65535UL);
			}
		}

		public ushort ProductMinor
		{
			get
			{
				return (ushort)(this.version >> 32 & 65535UL);
			}
		}

		public ushort BuildMajor
		{
			get
			{
				return (ushort)(this.version >> 16 & 65535UL);
			}
		}

		public ushort BuildMinor
		{
			get
			{
				return (ushort)(this.version & 65535UL);
			}
		}

		private readonly ulong version;

		public static Version Minimum = new Version(0UL);

		internal static readonly Version Exchange15MinVersion = new Version(15, 0, 0, 0);

		internal static readonly Version SupportsTableRowDeletedExtendedVersion = new Version(15, 0, 861, 0);
	}
}
