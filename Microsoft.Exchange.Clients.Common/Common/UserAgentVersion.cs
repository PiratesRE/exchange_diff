using System;
using System.Globalization;

namespace Microsoft.Exchange.Clients.Common
{
	public class UserAgentVersion : IComparable<UserAgentVersion>
	{
		public UserAgentVersion(int buildVersion, int majorVersion, int minorVersion)
		{
			this.build = buildVersion;
			this.major = majorVersion;
			this.minor = minorVersion;
		}

		public UserAgentVersion(string version)
		{
			int[] array = new int[3];
			int[] array2 = array;
			int num = -1;
			int num2 = 0;
			int num3 = 0;
			char c = '.';
			char c2 = '_';
			char value;
			if (version.IndexOf(c2) >= 0)
			{
				value = c2;
			}
			else
			{
				value = c;
			}
			for (;;)
			{
				num = version.IndexOf(value, num + 1);
				if (num == -1)
				{
					num = version.Length;
				}
				if (!int.TryParse(version.Substring(num3, num - num3), NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out array2[num2]))
				{
					break;
				}
				num2++;
				num3 = num + 1;
				if (num2 >= array2.Length || num >= version.Length)
				{
					goto IL_8B;
				}
			}
			throw new ArgumentException("The version parameter is not a valid User Agent Version");
			IL_8B:
			this.build = array2[0];
			this.major = array2[1];
			this.minor = array2[2];
		}

		public int Build
		{
			get
			{
				return this.build;
			}
			set
			{
				this.build = value;
			}
		}

		public int Major
		{
			get
			{
				return this.major;
			}
			set
			{
				this.major = value;
			}
		}

		public int Minor
		{
			get
			{
				return this.minor;
			}
			set
			{
				this.minor = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}", this.Build, this.Major, this.Minor);
		}

		public int CompareTo(UserAgentVersion b)
		{
			int num = (this.Minor.ToString().Length > b.Minor.ToString().Length) ? this.Minor.ToString().Length : b.Minor.ToString().Length;
			int num2 = (this.Major.ToString().Length > b.Major.ToString().Length) ? this.Major.ToString().Length : b.Major.ToString().Length;
			int num3 = this.Minor + (int)Math.Pow(10.0, (double)num) * this.Major + (int)Math.Pow(10.0, (double)(num2 + num)) * this.Build;
			num = b.Minor.ToString().Length;
			int num4 = b.Minor + (int)Math.Pow(10.0, (double)num) * b.Major + (int)Math.Pow(10.0, (double)(num2 + num)) * b.Build;
			return num3 - num4;
		}

		private const string FormatToString = "{0}.{1}.{2}";

		private int build;

		private int major;

		private int minor;
	}
}
