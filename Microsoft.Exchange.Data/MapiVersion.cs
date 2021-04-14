using System;

namespace Microsoft.Exchange.Data
{
	internal struct MapiVersion : IEquatable<MapiVersion>, IComparable<MapiVersion>
	{
		public MapiVersion(ushort productMajor, ushort productMinor, ushort buildMajor, ushort buildMinor)
		{
			this.value = MapiVersion.EnsureConvertible(productMajor, productMinor, buildMajor, buildMinor);
		}

		public MapiVersion(ushort s0, ushort s1, ushort s2)
		{
			ushort productMajor;
			ushort productMinor;
			ushort buildMajor;
			if ((s1 & 32768) != 0)
			{
				productMajor = (ushort)(s0 >> 8);
				productMinor = (s0 & 255);
				buildMajor = (s1 & 32767);
			}
			else
			{
				productMajor = s0;
				productMinor = 0;
				buildMajor = s1;
			}
			this.value = MapiVersion.EnsureConvertible(productMajor, productMinor, buildMajor, s2);
		}

		private MapiVersion(ulong internalValue)
		{
			this.value = internalValue;
		}

		public static MapiVersion Parse(string wireFormat)
		{
			string[] array = wireFormat.Split(new char[]
			{
				'.'
			});
			if (array.Length != 3)
			{
				throw new FormatException("Version specification should have 3 parts");
			}
			ushort[] array2 = new ushort[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (!ushort.TryParse(array[i], out array2[i]))
				{
					throw new ArgumentOutOfRangeException("wireFormat", array[i], "Version number part should be between 0 and 65535");
				}
			}
			return new MapiVersion(MapiVersion.EnsureConvertible(array2[0], 0, array2[1], array2[2]));
		}

		internal ulong Value
		{
			get
			{
				return this.value;
			}
		}

		public static bool operator ==(MapiVersion left, MapiVersion right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(MapiVersion left, MapiVersion right)
		{
			return !left.Equals(right);
		}

		public static bool operator <(MapiVersion left, MapiVersion right)
		{
			return left.value < right.value;
		}

		public static bool operator <=(MapiVersion left, MapiVersion right)
		{
			return left.value <= right.value;
		}

		public static bool operator >=(MapiVersion left, MapiVersion right)
		{
			return left.value >= right.value;
		}

		public static bool operator >(MapiVersion left, MapiVersion right)
		{
			return left.value > right.value;
		}

		public override string ToString()
		{
			return MapiVersion.ConvertToQuartetString(this.value);
		}

		public override bool Equals(object obj)
		{
			return obj is MapiVersion && this.Equals((MapiVersion)obj);
		}

		public override int GetHashCode()
		{
			return this.value.GetHashCode();
		}

		public ushort[] ToTriplet()
		{
			ushort[] array = this.ToQuartet();
			return new ushort[]
			{
				(ushort)((int)array[0] << 8 | (int)array[1]),
				array[2] | 32768,
				array[3]
			};
		}

		public ushort[] ToQuartet()
		{
			return MapiVersion.ToQuartet(this.value);
		}

		internal static string ConvertToQuartetString(ulong versionValue)
		{
			ushort[] array = MapiVersion.ToQuartet(versionValue);
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				array[0],
				array[1],
				array[2],
				array[3]
			});
		}

		private static ushort[] ToQuartet(ulong value)
		{
			return new ushort[]
			{
				(ushort)(value >> 48),
				(ushort)(value >> 32 & 65535UL),
				(ushort)(value >> 16 & 65535UL),
				(ushort)(value & 65535UL)
			};
		}

		private static ulong EnsureConvertible(ushort productMajor, ushort productMinor, ushort buildMajor, ushort buildMinor)
		{
			if (productMajor < 128 && productMinor < 256 && buildMajor < 32768)
			{
				return (ulong)productMajor << 48 | (ulong)productMinor << 32 | (ulong)buildMajor << 16 | (ulong)buildMinor;
			}
			throw new ArgumentOutOfRangeException("value", "One or more of the version parts is out of range. Constraints are: productMajor < 128, productMinor < 256, buildMajor < 32768");
		}

		public bool Equals(MapiVersion other)
		{
			return this.value == other.value;
		}

		public int CompareTo(MapiVersion other)
		{
			return this.value.CompareTo(other.Value);
		}

		private const ulong VerificationMask = 18410995654303154176UL;

		public static MapiVersion Min = new MapiVersion(0, 0, 0, 0);

		public static MapiVersion Max = new MapiVersion(127, 255, 32767, ushort.MaxValue);

		public static MapiVersion Outlook11 = new MapiVersion(11, 0, 0, 0);

		public static MapiVersion Outlook12 = new MapiVersion(12, 0, 0, 0);

		public static MapiVersion Outlook14 = new MapiVersion(14, 0, 0, 0);

		public static MapiVersion Outlook15 = new MapiVersion(15, 0, 0, 0);

		public static MapiVersion MRS14SP1 = new MapiVersion(14, 1, 180, 1);

		private readonly ulong value;
	}
}
