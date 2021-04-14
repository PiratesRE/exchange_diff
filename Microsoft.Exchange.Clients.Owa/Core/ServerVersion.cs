using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public class ServerVersion
	{
		private ServerVersion(int major, int minor, int build, int dot)
		{
			this.major = major;
			this.minor = minor;
			this.build = build;
			this.dot = dot;
		}

		public static ServerVersion CreateFromVersionNumber(int versionNumber)
		{
			int num = versionNumber & 32767;
			int num2 = versionNumber >> 16 & 63;
			int num3 = versionNumber >> 22 & 63;
			int num4 = 0;
			return new ServerVersion(num3, num2, num, num4);
		}

		public static bool IsE14SP1OrGreater(int versionNumber)
		{
			ServerVersion serverVersion = ServerVersion.CreateFromVersionNumber(versionNumber);
			return serverVersion.Major >= 14 && serverVersion.Minor >= 1;
		}

		public static ServerVersion CreateFromVersionString(string versionString)
		{
			if (versionString == null)
			{
				throw new ArgumentNullException("versionString");
			}
			return ServerVersion.TryValidateVersionString(versionString);
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

		private static ServerVersion TryValidateVersionString(string versionString)
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
			int nextVersionPart = ServerVersion.GetNextVersionPart(versionString, 0, 2, out num, out flag);
			if (nextVersionPart == -1 || flag)
			{
				return null;
			}
			nextVersionPart = ServerVersion.GetNextVersionPart(versionString, nextVersionPart, 2, out num2, out flag);
			if (nextVersionPart == -1 || flag)
			{
				return null;
			}
			nextVersionPart = ServerVersion.GetNextVersionPart(versionString, nextVersionPart, 4, out num3, out flag);
			if (nextVersionPart == -1)
			{
				return null;
			}
			if (!flag)
			{
				nextVersionPart = ServerVersion.GetNextVersionPart(versionString, nextVersionPart, 4, out num4, out flag);
				if (nextVersionPart == -1 || !flag)
				{
					return null;
				}
			}
			return new ServerVersion(num, num2, num3, num4);
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

		private int major;

		private int minor;

		private int build;

		private int dot;

		private string versionString;

		public class ServerVersionComparer : IEqualityComparer<ServerVersion>
		{
			public bool Equals(ServerVersion a, ServerVersion b)
			{
				return this.GetHashCode(a) == this.GetHashCode(b);
			}

			public int GetHashCode(ServerVersion a)
			{
				if (a == null)
				{
					throw new ArgumentNullException("a");
				}
				return (a.major << 26 & -67108864) | (a.minor << 20 & 66060288) | (a.build << 5 & 1048544) | (a.dot & 31);
			}
		}
	}
}
