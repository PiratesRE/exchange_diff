using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	[Serializable]
	public class ExchangeObjectVersion : IComparable, IComparable<ExchangeObjectVersion>
	{
		public ExchangeObjectVersion(long encodedVersion)
		{
			this.Major = (byte)(encodedVersion >> 50 & 255L);
			this.Minor = (byte)(encodedVersion >> 42 & 255L);
			this.ExchangeBuild = new ExchangeBuild(encodedVersion);
		}

		public ExchangeObjectVersion(byte major, byte minor, byte majorBuild, byte minorBuild, ushort build, ushort buildRevision) : this(major, minor, new ExchangeBuild(majorBuild, minorBuild, build, buildRevision))
		{
		}

		public ExchangeObjectVersion(byte major, byte minor, ExchangeBuild exchangeBuild)
		{
			this.Major = major;
			this.Minor = minor;
			this.ExchangeBuild = exchangeBuild;
		}

		public static ExchangeObjectVersion Current
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public ExchangeObjectVersion NextMajorVersion
		{
			get
			{
				return new ExchangeObjectVersion(this.Major + 1, 0, 0, 0, 0, 0);
			}
		}

		public ExchangeObjectVersion NextMinorVersion
		{
			get
			{
				return new ExchangeObjectVersion(this.Major, this.Minor + 1, 0, 0, 0, 0);
			}
		}

		private int MajorMinor
		{
			get
			{
				return (int)this.Major << 8 | (int)this.Minor;
			}
		}

		public static bool operator ==(ExchangeObjectVersion objA, ExchangeObjectVersion objB)
		{
			return object.Equals(objA, objB);
		}

		public static bool operator !=(ExchangeObjectVersion objA, ExchangeObjectVersion objB)
		{
			return !object.Equals(objA, objB);
		}

		public static ExchangeObjectVersion Parse(string input)
		{
			if (input == null || string.IsNullOrEmpty(input = input.Trim()))
			{
				throw new ArgumentException(DataStrings.EmptyExchangeObjectVersion, "input");
			}
			long encodedVersion = 0L;
			if (long.TryParse(input, out encodedVersion))
			{
				return new ExchangeObjectVersion(encodedVersion);
			}
			if (ExchangeObjectVersion.parsingRegex == null)
			{
				lock (ExchangeObjectVersion.parsingRegexInitializationLock)
				{
					if (ExchangeObjectVersion.parsingRegex == null)
					{
						ExchangeObjectVersion.parsingRegex = new Regex("^(?<major>\\d{1,3})\\.(?<minor>\\d{1,3})\\s*\\((?<exbuild>.+)\\)$", RegexOptions.ExplicitCapture | RegexOptions.Compiled);
					}
				}
			}
			Match match = ExchangeObjectVersion.parsingRegex.Match(input);
			if (!match.Success)
			{
				throw new ArgumentException(DataStrings.InvalidFormatExchangeObjectVersion);
			}
			byte major = 0;
			if (!byte.TryParse(match.Groups["major"].Value, out major))
			{
				throw new ArgumentException(DataStrings.InvalidFormatExchangeObjectVersion);
			}
			byte minor = 0;
			if (!byte.TryParse(match.Groups["minor"].Value, out minor))
			{
				throw new ArgumentException(DataStrings.InvalidFormatExchangeObjectVersion);
			}
			ExchangeBuild exchangeBuild = ExchangeBuild.Parse(match.Groups["exbuild"].Value);
			return new ExchangeObjectVersion(major, minor, exchangeBuild.Major, exchangeBuild.Minor, exchangeBuild.Build, exchangeBuild.BuildRevision);
		}

		public long ToInt64()
		{
			return (long)((ulong)this.Major << 50 | (ulong)this.Minor << 42 | (ulong)this.ExchangeBuild.ToInt64());
		}

		public override bool Equals(object other)
		{
			return other is ExchangeObjectVersion && this.ToInt64() == ((ExchangeObjectVersion)other).ToInt64();
		}

		public override int GetHashCode()
		{
			return this.ToInt64().GetHashCode();
		}

		public bool IsOlderThan(ExchangeObjectVersion other)
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}
			return this.MajorMinor < other.MajorMinor;
		}

		public bool IsNewerThan(ExchangeObjectVersion other)
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}
			return this.MajorMinor > other.MajorMinor;
		}

		public bool IsSameVersion(ExchangeObjectVersion other)
		{
			if (null == other)
			{
				throw new ArgumentNullException("other");
			}
			return this.MajorMinor == other.MajorMinor;
		}

		public override string ToString()
		{
			return string.Format("{0}.{1} ({2})", this.Major, this.Minor, this.ExchangeBuild.ToString());
		}

		public int CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is ExchangeObjectVersion))
			{
				throw new ArgumentException(DataStrings.InvalidTypeArgumentException("obj", obj.GetType(), typeof(ExchangeObjectVersion)));
			}
			return this.CompareTo((ExchangeObjectVersion)obj);
		}

		public int CompareTo(ExchangeObjectVersion other)
		{
			if (null == other)
			{
				return 1;
			}
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

		private const string GroupNameMajorInRegex = "major";

		private const string GroupNameMinorInRegex = "minor";

		private const string GroupNameExBuildInRegex = "exbuild";

		private const string ParsingRegexPattern = "^(?<major>\\d{1,3})\\.(?<minor>\\d{1,3})\\s*\\((?<exbuild>.+)\\)$";

		public static readonly ExchangeObjectVersion Exchange2003 = new ExchangeObjectVersion(0, 0, 6, 5, 6500, 0);

		public static readonly ExchangeObjectVersion Exchange2007 = new ExchangeObjectVersion(0, 1, 8, 0, 535, 0);

		public static readonly ExchangeObjectVersion Exchange2010 = new ExchangeObjectVersion(0, 10, 14, 0, 100, 0);

		public static readonly ExchangeObjectVersion Exchange2012 = new ExchangeObjectVersion(0, 20, 15, 0, 0, 0);

		public static readonly ExchangeObjectVersion Maximum = new ExchangeObjectVersion(0, byte.MaxValue, byte.MaxValue, 0, 0, 0);

		public readonly byte Major;

		public readonly byte Minor;

		public readonly ExchangeBuild ExchangeBuild;

		private static Regex parsingRegex;

		private static object parsingRegexInitializationLock = new object();
	}
}
