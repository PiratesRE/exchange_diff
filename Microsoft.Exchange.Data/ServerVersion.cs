using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	[Serializable]
	public sealed class ServerVersion : IComparable, IComparable<ServerVersion>
	{
		public ServerVersion(int major, int minor, int build, int revision)
		{
			this.version = new Version(major, minor, build, revision);
		}

		public ServerVersion(int major, int minor, int build, int revision, string filePatchLevelDescription) : this(major, minor, build, revision)
		{
			this.filePatchLevelDescription = filePatchLevelDescription;
		}

		public ServerVersion(int versionNumber) : this(versionNumber >> 22 & 63, versionNumber >> 16 & 63, versionNumber & 32767, 0, string.Empty)
		{
		}

		public ServerVersion(long versionNumber) : this((int)(versionNumber >> 48 & 65535L), (int)(versionNumber >> 32 & 65535L), (int)(versionNumber >> 16 & 65535L), (int)(versionNumber & 65535L), string.Empty)
		{
		}

		public int Major
		{
			get
			{
				return this.version.Major;
			}
		}

		public int Minor
		{
			get
			{
				return this.version.Minor;
			}
		}

		public int Build
		{
			get
			{
				return this.version.Build;
			}
		}

		public int Revision
		{
			get
			{
				return this.version.Revision;
			}
		}

		public string FilePatchLevelDescription
		{
			get
			{
				return this.filePatchLevelDescription;
			}
		}

		internal static ServerVersion InstalledVersion
		{
			get
			{
				if (ServerVersion.installedVersion == null)
				{
					ServerVersion.installedVersion = ExchangeSetupContext.InstalledVersion;
				}
				return ServerVersion.installedVersion;
			}
		}

		public static ServerVersion ParseFromSerialNumber(string serialNumber)
		{
			ServerVersion result;
			if (!ServerVersion.TryParseFromSerialNumber(serialNumber, out result))
			{
				throw new FormatException(DataStrings.ErrorSerialNumberFormatError(serialNumber));
			}
			return result;
		}

		public static bool TryParseFromSerialNumber(string serialNumber, out ServerVersion serverVersion)
		{
			serverVersion = null;
			if (serialNumber == null)
			{
				return false;
			}
			string text = null;
			string text2 = null;
			Match match = Regex.Match(serialNumber, "build (\\d+(\\.\\d+)*)", RegexOptions.IgnoreCase);
			if (match.Success && match.Groups.Count > 1)
			{
				text = match.Groups[1].Value;
				match = Regex.Match(serialNumber, "\\(build (\\d+(\\.\\d+)*): ([^\\)]+)\\)", RegexOptions.IgnoreCase);
				if (match.Success && match.Groups.Count > 3)
				{
					text2 = match.Groups[3].Value;
				}
			}
			match = Regex.Match(serialNumber, "version (\\d+\\.\\d+)", RegexOptions.IgnoreCase);
			if (match.Success && match.Groups.Count > 1)
			{
				string str = match.Groups[1].Value;
				if (text != null)
				{
					str = str + "." + text;
				}
				Version version = new Version(str);
				int num = version.Build;
				if (version.Major >= 8)
				{
					if (num >= 30000)
					{
						num = version.Build - 30000;
					}
					else if (num >= 10000)
					{
						num = version.Build - 10000;
					}
				}
				try
				{
					serverVersion = new ServerVersion(version.Major, version.Minor, num, version.Revision, text2);
					return true;
				}
				catch (ArgumentException)
				{
					return false;
				}
				return false;
			}
			return false;
		}

		public string ToString(bool addSerialNumberOffset)
		{
			int num = this.Build + ((addSerialNumberOffset && this.Major >= 8) ? 30000 : 0);
			if (!string.IsNullOrEmpty(this.FilePatchLevelDescription))
			{
				return string.Format("Version {0}.{1} (Build {2}.{3}: {4})", new object[]
				{
					this.Major,
					this.Minor,
					num,
					this.Revision,
					this.FilePatchLevelDescription
				});
			}
			return string.Format("Version {0}.{1} (Build {2}.{3})", new object[]
			{
				this.Major,
				this.Minor,
				num,
				this.Revision
			});
		}

		public override string ToString()
		{
			return this.ToString(false);
		}

		public int ToInt()
		{
			return (this.Build & 32767) | (this.Minor & 63) << 16 | (this.Major & 63) << 22 | 1879080960;
		}

		public long ToLong()
		{
			ulong num = (ulong)((long)this.Major & 65535L);
			num <<= 48;
			ulong num2 = (ulong)((long)this.Minor & 65535L);
			num2 <<= 32;
			ulong num3 = (ulong)((long)this.Build & 65535L);
			num3 <<= 16;
			ulong num4 = (ulong)((long)this.Revision & 65535L);
			return (long)(num | num2 | num3 | num4);
		}

		public bool Equals(ServerVersion other)
		{
			return other != null && this.version == other.version && string.Compare(this.FilePatchLevelDescription, other.FilePatchLevelDescription) == 0;
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as ServerVersion);
		}

		public override int GetHashCode()
		{
			int num = this.version.GetHashCode();
			if (!string.IsNullOrEmpty(this.FilePatchLevelDescription))
			{
				num ^= this.FilePatchLevelDescription.GetHashCode();
			}
			return num;
		}

		public static bool operator ==(ServerVersion left, ServerVersion right)
		{
			return object.Equals(left, right);
		}

		public static bool operator !=(ServerVersion left, ServerVersion right)
		{
			return !(left == right);
		}

		public static implicit operator Version(ServerVersion sVersion)
		{
			return sVersion.version;
		}

		public static implicit operator ServerVersion(Version version)
		{
			return new ServerVersion(version.Major, version.Minor, version.Build, version.Revision);
		}

		public static int Compare(ServerVersion a, ServerVersion b)
		{
			if (a == null)
			{
				throw new ArgumentNullException("a");
			}
			if (b == null)
			{
				throw new ArgumentNullException("b");
			}
			int num = a.Major - b.Major;
			if (num == 0)
			{
				num = a.Minor - b.Minor;
				if (num == 0)
				{
					num = a.Build - b.Build;
					if (num == 0)
					{
						num = a.Revision - b.Revision;
					}
				}
			}
			return num;
		}

		int IComparable.CompareTo(object obj)
		{
			if (obj == null)
			{
				return 1;
			}
			if (!(obj is ServerVersion))
			{
				throw new ArgumentException(DataStrings.InvalidTypeArgumentException("obj", obj.GetType(), typeof(ServerVersion)));
			}
			return ServerVersion.Compare(this, (ServerVersion)obj);
		}

		int IComparable<ServerVersion>.CompareTo(ServerVersion other)
		{
			if (null == other)
			{
				return 1;
			}
			return ServerVersion.Compare(this, other);
		}

		private const string ShortVersionFormat = "Version {0}.{1} (Build {2}.{3})";

		private const string LongVersionFormat = "Version {0}.{1} (Build {2}.{3}: {4})";

		private Version version;

		private string filePatchLevelDescription;

		private static ServerVersion installedVersion;
	}
}
