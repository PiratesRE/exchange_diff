using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.Clients;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class UserAgentParser
	{
		private UserAgentParser()
		{
		}

		internal static void Parse(string userAgent, out string application, out UserAgentParser.UserAgentVersion version, out string platform)
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug<string>(0L, "Globals.ParseUserAgent. user-agent = {0}", (userAgent != null) ? userAgent : "<null>");
			application = string.Empty;
			version = default(UserAgentParser.UserAgentVersion);
			platform = string.Empty;
			if (userAgent == null || userAgent.Length == 0)
			{
				return;
			}
			int num = int.MinValue;
			int i;
			for (i = 0; i < UserAgentParser.clientPlatform.Length; i++)
			{
				if (-1 != (num = userAgent.IndexOf(UserAgentParser.clientPlatform[i], StringComparison.OrdinalIgnoreCase)))
				{
					platform = UserAgentParser.clientPlatform[i];
					break;
				}
			}
			for (i = 0; i < UserAgentParser.clientApplication.Length; i++)
			{
				if (-1 != (num = userAgent.IndexOf(UserAgentParser.clientApplication[i], StringComparison.OrdinalIgnoreCase)) && (!string.Equals(UserAgentParser.clientApplication[i], "Safari", StringComparison.OrdinalIgnoreCase) || -1 == userAgent.IndexOf("Chrome", StringComparison.OrdinalIgnoreCase)))
				{
					application = UserAgentParser.clientApplication[i];
					break;
				}
			}
			if (i == UserAgentParser.clientApplication.Length)
			{
				return;
			}
			ExTraceGlobals.CoreDataTracer.TraceDebug<string>(0L, "Parsed out application = {0}", application);
			int num2 = -1;
			if (string.Equals(application, "MSIE", StringComparison.Ordinal) || string.Equals(application, "Firefox", StringComparison.Ordinal) || string.Equals(application, "Chrome", StringComparison.Ordinal))
			{
				num += application.Length + 1;
			}
			else if (string.Equals("iPhone", platform) || string.Equals("iPad", platform))
			{
				string text = "OS ";
				num = userAgent.IndexOf(text) + text.Length;
			}
			else if (string.Equals("Android", platform))
			{
				string text2 = "Android ";
				num = userAgent.IndexOf(text2) + text2.Length;
			}
			else
			{
				if (!string.Equals(application, "Safari", StringComparison.Ordinal))
				{
					platform = string.Empty;
					return;
				}
				string text3 = "Version/";
				num = userAgent.IndexOf(text3) + text3.Length;
			}
			int j;
			for (j = num; j < userAgent.Length; j++)
			{
				if (!char.IsDigit(userAgent, j) && userAgent[j] != '.' && userAgent[j] != '_')
				{
					num2 = j;
					break;
				}
			}
			if (num2 == -1)
			{
				num2 = userAgent.Length;
			}
			if (j == num)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Unable to parse browser version.  Could not find semicolon");
				return;
			}
			string text4 = userAgent.Substring(num, num2 - num);
			try
			{
				version = new UserAgentParser.UserAgentVersion(text4);
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "TryParse failed, unable to parse browser version = {0}", text4);
				return;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Parsed out version = {0}", version.ToString());
			ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Parsed out platform = {0}", platform);
		}

		private static string[] clientApplication = new string[]
		{
			"Opera",
			"Netscape",
			"MSIE",
			"Safari",
			"Firefox",
			"Chrome",
			"Mobile"
		};

		private static string[] clientPlatform = new string[]
		{
			"Windows NT",
			"Windows 98; Win 9x 4.90",
			"Windows 2000",
			"iPhone",
			"iPad",
			"Android",
			"Linux",
			"Macintosh"
		};

		internal struct UserAgentVersion : IComparable<UserAgentParser.UserAgentVersion>
		{
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
						goto IL_85;
					}
				}
				throw new ArgumentException("The version parameter is not a valid User Agent Version");
				IL_85:
				this.build = array2[0];
				this.major = array2[1];
				this.minor = array2[2];
			}

			public override string ToString()
			{
				return string.Format("{0}.{1}.{2}", this.Build, this.Major, this.Minor);
			}

			public int CompareTo(UserAgentParser.UserAgentVersion b)
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
}
