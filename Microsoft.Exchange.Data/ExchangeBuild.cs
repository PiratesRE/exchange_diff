using System;
using System.ComponentModel;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	[Serializable]
	public struct ExchangeBuild : IFormattable, IComparable, IComparable<ExchangeBuild>
	{
		public ExchangeBuild(byte major, byte minor, ushort build, ushort buildRevision)
		{
			this.Major = major;
			this.Minor = minor;
			this.Build = build;
			this.BuildRevision = (buildRevision & 1023);
		}

		public ExchangeBuild(long encodedBuildNumber)
		{
			this.Major = (byte)(encodedBuildNumber >> 34 & 255L);
			this.Minor = (byte)(encodedBuildNumber >> 26 & 255L);
			this.Build = (ushort)(encodedBuildNumber >> 10 & 65535L);
			this.BuildRevision = (ushort)(encodedBuildNumber & 1023L);
		}

		public static ExchangeBuild Parse(string versionString)
		{
			if (string.IsNullOrEmpty(versionString))
			{
				throw new ArgumentException(DataStrings.EmptyExchangeBuild, "ExchangeBuild");
			}
			string[] array = versionString.Split(new char[]
			{
				'.'
			});
			if (4 != array.Length)
			{
				throw new ArgumentException(DataStrings.FormatExchangeBuildWrong, "ExchangeBuild");
			}
			byte major;
			if (!byte.TryParse(array[0], out major))
			{
				throw new ArgumentException(DataStrings.FormatExchangeBuildWrong, "ExchangeBuild");
			}
			byte minor;
			if (!byte.TryParse(array[1], out minor))
			{
				throw new ArgumentException(DataStrings.FormatExchangeBuildWrong, "ExchangeBuild");
			}
			ushort build;
			if (!ushort.TryParse(array[2], out build))
			{
				throw new ArgumentException(DataStrings.FormatExchangeBuildWrong, "ExchangeBuild");
			}
			ushort buildRevision;
			if (!ushort.TryParse(array[3], out buildRevision))
			{
				throw new ArgumentException(DataStrings.FormatExchangeBuildWrong, "ExchangeBuild");
			}
			return new ExchangeBuild(major, minor, build, buildRevision);
		}

		public static bool operator ==(ExchangeBuild objA, ExchangeBuild objB)
		{
			return object.Equals(objA, objB);
		}

		public static bool operator !=(ExchangeBuild objA, ExchangeBuild objB)
		{
			return !object.Equals(objA, objB);
		}

		public static bool operator >(ExchangeBuild objA, ExchangeBuild objB)
		{
			return objA.CompareTo(objB) > 0;
		}

		public static bool operator >=(ExchangeBuild objA, ExchangeBuild objB)
		{
			return objA.CompareTo(objB) >= 0;
		}

		public static bool operator <(ExchangeBuild objA, ExchangeBuild objB)
		{
			return objA.CompareTo(objB) < 0;
		}

		public static bool operator <=(ExchangeBuild objA, ExchangeBuild objB)
		{
			return objA.CompareTo(objB) <= 0;
		}

		public long ToInt64()
		{
			return (long)((ulong)this.Major << 34 | (ulong)this.Minor << 26 | (ulong)this.Build << 10 | (ulong)this.BuildRevision);
		}

		public int ToExchange2003FormatInt32()
		{
			return ((int)Math.Min(this.Major, 15) << 28 | (int)Math.Min(this.Minor, 15) << 24 | (int)Math.Min(this.BuildRevision, 15) << 20 | (int)this.Build) + 30000;
		}

		public override bool Equals(object obj)
		{
			return obj is ExchangeBuild && this.Equals((ExchangeBuild)obj);
		}

		public bool Equals(ExchangeBuild other)
		{
			return this.ToInt64() == other.ToInt64();
		}

		public override int GetHashCode()
		{
			return this.ToExchange2003FormatInt32();
		}

		public override string ToString()
		{
			return this.ToString(null, null);
		}

		public string ToString(string format, IFormatProvider fp)
		{
			string text = null;
			if (format == "N")
			{
				if (this.Major < 8)
				{
					text = DataStrings.Exchange2003.ToString();
				}
				if (this.Major == 8 && this.Minor == 0 && this.Build < 900)
				{
					text = DataStrings.Exchange2007.ToString();
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					this.Major,
					this.Minor,
					this.Build,
					this.BuildRevision
				});
			}
			return text;
		}

		public bool IsOlderThan(ExchangeBuild other)
		{
			return this.Major < other.Major || (this.Major == other.Major && this.Minor < other.Minor);
		}

		public bool IsNewerThan(ExchangeBuild other)
		{
			return this.Major > other.Major || (this.Major == other.Major && this.Minor > other.Minor);
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is ExchangeBuild))
			{
				throw new ArgumentException(DataStrings.InvalidTypeArgumentException("obj", obj.GetType(), typeof(ExchangeBuild)));
			}
			return this.CompareTo((ExchangeBuild)obj);
		}

		public int CompareTo(ExchangeBuild other)
		{
			long num = this.ToInt64() - other.ToInt64();
			if (0L == num)
			{
				return 0;
			}
			if (0L <= num)
			{
				return 1;
			}
			return -1;
		}

		public const ushort MaxBuildRevision = 1023;

		internal const int LegacyBuildOffset = 30000;

		public readonly byte Major;

		public readonly byte Minor;

		public readonly ushort Build;

		public readonly ushort BuildRevision;
	}
}
