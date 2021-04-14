using System;

namespace Microsoft.Exchange.MapiHttp
{
	public sealed class MapiHttpVersion : IComparable
	{
		public MapiHttpVersion(ushort major, ushort minor, ushort buildMajor, ushort buildMinor)
		{
			this.Major = major;
			this.Minor = minor;
			this.BuildMajor = buildMajor;
			this.BuildMinor = buildMinor;
		}

		public ushort Major { get; private set; }

		public ushort Minor { get; private set; }

		public ushort BuildMajor { get; private set; }

		public ushort BuildMinor { get; private set; }

		public static bool TryParse(string versionString, out MapiHttpVersion version)
		{
			version = null;
			string[] array = versionString.Split(new char[]
			{
				'.'
			});
			if (array.Length != 4)
			{
				return false;
			}
			ushort[] array2 = new ushort[4];
			for (int i = 0; i < array.Length; i++)
			{
				if (!ushort.TryParse(array[i], out array2[i]))
				{
					return false;
				}
			}
			version = new MapiHttpVersion(array2[0], array2[1], array2[2], array2[3]);
			return true;
		}

		public static bool operator ==(MapiHttpVersion v1, MapiHttpVersion v2)
		{
			return object.ReferenceEquals(v1, v2) || (v1 != null && v2 != null && v1.CompareTo(v2) == 0);
		}

		public static bool operator !=(MapiHttpVersion v1, MapiHttpVersion v2)
		{
			return !object.ReferenceEquals(v1, v2) && (v1 == null || v2 == null || v1.CompareTo(v2) != 0);
		}

		public static bool operator <(MapiHttpVersion v1, MapiHttpVersion v2)
		{
			return v1.CompareTo(v2) < 0;
		}

		public static bool operator >(MapiHttpVersion v1, MapiHttpVersion v2)
		{
			return v1.CompareTo(v2) > 0;
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				this.Major,
				this.Minor,
				this.BuildMajor,
				this.BuildMinor
			});
		}

		public ushort[] ToQuartet()
		{
			return new ushort[]
			{
				this.Major,
				this.Minor,
				this.BuildMajor,
				this.BuildMinor
			};
		}

		public override int GetHashCode()
		{
			return (int)this.Major << 24 | (int)this.Minor << 16 | (int)this.BuildMajor << 8 | (int)this.BuildMinor;
		}

		public long GetComparableValue()
		{
			return (long)((int)this.Major << 16 | (int)this.Minor | (int)this.BuildMajor << 16 | (int)this.BuildMinor);
		}

		public override bool Equals(object obj)
		{
			return obj is MapiHttpVersion && this.CompareTo(obj) == 0;
		}

		public int CompareTo(object obj)
		{
			if (!(obj is MapiHttpVersion))
			{
				throw new ArgumentException("A MapiHttpVersion object is required for comparison.");
			}
			long comparableValue = this.GetComparableValue();
			long comparableValue2 = ((MapiHttpVersion)obj).GetComparableValue();
			if (comparableValue < comparableValue2)
			{
				return -1;
			}
			if (comparableValue > comparableValue2)
			{
				return 1;
			}
			return 0;
		}

		public static readonly MapiHttpVersion Version14 = new MapiHttpVersion(14, 0, 0, 0);

		public static readonly MapiHttpVersion Version15 = new MapiHttpVersion(15, 0, 0, 0);
	}
}
