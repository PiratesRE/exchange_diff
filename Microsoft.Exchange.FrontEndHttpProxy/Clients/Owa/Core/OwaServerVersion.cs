using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class OwaServerVersion
	{
		private OwaServerVersion(int major, int minor, int build, int dot)
		{
			this.major = major;
			this.minor = minor;
			this.build = build;
			this.dot = dot;
		}

		public int Major
		{
			get
			{
				return this.major;
			}
		}

		public int Minor
		{
			get
			{
				return this.minor;
			}
		}

		public int Build
		{
			get
			{
				return this.build;
			}
		}

		public int Dot
		{
			get
			{
				return this.dot;
			}
		}

		public static OwaServerVersion CreateFromVersionNumber(int versionNumber)
		{
			int num = versionNumber & 32767;
			int num2 = versionNumber >> 16 & 63;
			int num3 = versionNumber >> 22 & 63;
			int num4 = 0;
			return new OwaServerVersion(num3, num2, num, num4);
		}

		public static bool IsE14SP1OrGreater(int versionNumber)
		{
			OwaServerVersion owaServerVersion = OwaServerVersion.CreateFromVersionNumber(versionNumber);
			return owaServerVersion.Major >= 14 && owaServerVersion.Minor >= 1;
		}

		public static OwaServerVersion CreateFromVersionString(string versionString)
		{
			if (versionString == null)
			{
				throw new ArgumentNullException("versionString");
			}
			return OwaServerVersion.TryValidateVersionString(versionString);
		}

		public static int Compare(OwaServerVersion a, OwaServerVersion b)
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
						num = a.Dot - b.Dot;
					}
				}
			}
			return num;
		}

		public static bool IsEqualMajorVersion(int a, int b)
		{
			return ((a ^ b) & 264241152) == 0;
		}

		public override string ToString()
		{
			if (this.versionString == null)
			{
				string arg = string.Concat(new object[]
				{
					this.major,
					".",
					this.minor,
					".",
					this.build
				});
				if (this.dot != -1)
				{
					arg = arg + "." + this.dot;
				}
				this.versionString = arg;
			}
			return this.versionString;
		}

		private static OwaServerVersion TryValidateVersionString(string versionString)
		{
			if (string.IsNullOrEmpty(versionString))
			{
				return null;
			}
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			bool flag = false;
			int nextVersionPart = OwaServerVersion.GetNextVersionPart(versionString, 0, 2, out num, out flag);
			if (nextVersionPart == -1 || flag)
			{
				return null;
			}
			nextVersionPart = OwaServerVersion.GetNextVersionPart(versionString, nextVersionPart, 2, out num2, out flag);
			if (nextVersionPart == -1 || flag)
			{
				return null;
			}
			nextVersionPart = OwaServerVersion.GetNextVersionPart(versionString, nextVersionPart, 4, out num3, out flag);
			if (nextVersionPart == -1)
			{
				return null;
			}
			if (!flag)
			{
				nextVersionPart = OwaServerVersion.GetNextVersionPart(versionString, nextVersionPart, 4, out num4, out flag);
				if (nextVersionPart == -1 || !flag)
				{
					return null;
				}
			}
			return new OwaServerVersion(num, num2, num3, num4);
		}

		private static int GetNextVersionPart(string versionString, int start, int maximumLength, out int part, out bool foundEnd)
		{
			bool flag = false;
			int num = start;
			part = 0;
			foundEnd = false;
			StringBuilder stringBuilder = new StringBuilder(maximumLength);
			for (;;)
			{
				if (num == versionString.Length)
				{
					if (stringBuilder.Length == 0)
					{
						break;
					}
					flag = true;
					foundEnd = true;
				}
				else
				{
					char c = versionString[num];
					if (c == '.')
					{
						flag = true;
					}
					else
					{
						if (!char.IsDigit(c))
						{
							return -1;
						}
						if (maximumLength == stringBuilder.Length)
						{
							return -1;
						}
						stringBuilder.Append(c);
					}
				}
				num++;
				if (flag)
				{
					goto Block_6;
				}
			}
			return -1;
			Block_6:
			part = int.Parse(stringBuilder.ToString());
			return num;
		}

		private readonly int major;

		private readonly int minor;

		private readonly int build;

		private readonly int dot;

		private string versionString;

		public class ServerVersionComparer : IEqualityComparer<OwaServerVersion>
		{
			public bool Equals(OwaServerVersion a, OwaServerVersion b)
			{
				return this.GetHashCode(a) == this.GetHashCode(b);
			}

			public int GetHashCode(OwaServerVersion owaServerVersion)
			{
				if (owaServerVersion == null)
				{
					throw new ArgumentNullException("owaServerVersion");
				}
				return (owaServerVersion.major << 26 & -67108864) | (owaServerVersion.minor << 20 & 66060288) | (owaServerVersion.build << 5 & 1048544) | (owaServerVersion.dot & 31);
			}
		}
	}
}
