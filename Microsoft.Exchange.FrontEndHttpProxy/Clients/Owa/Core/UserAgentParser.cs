using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal static class UserAgentParser
	{
		internal static void Parse(string userAgent, out string application, out UserAgentParser.UserAgentVersion version, out string platform)
		{
			ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "Globals.ParseUserAgent. user-agent = {0}", (userAgent != null) ? userAgent : "<null>");
			application = string.Empty;
			version = default(UserAgentParser.UserAgentVersion);
			platform = string.Empty;
			if (userAgent == null || userAgent.Length == 0)
			{
				return;
			}
			int num = int.MinValue;
			int i;
			for (i = 0; i < UserAgentParser.clientApplication.Length; i++)
			{
				if (-1 != (num = userAgent.IndexOf(UserAgentParser.clientApplication[i], StringComparison.OrdinalIgnoreCase)))
				{
					if (string.Equals(UserAgentParser.clientApplication[i], "Safari", StringComparison.OrdinalIgnoreCase))
					{
						if (-1 != userAgent.IndexOf("Chrome", StringComparison.OrdinalIgnoreCase))
						{
							goto IL_AF;
						}
					}
					else if (string.Equals(UserAgentParser.clientApplication[i], UserAgentParser.ie11ApplicationName, StringComparison.OrdinalIgnoreCase) && -1 == userAgent.IndexOf(UserAgentParser.ie11ExtraCheck, StringComparison.OrdinalIgnoreCase))
					{
						goto IL_AF;
					}
					application = UserAgentParser.clientApplication[i];
					break;
				}
				IL_AF:;
			}
			if (i == UserAgentParser.clientApplication.Length)
			{
				return;
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "Parsed out application = {0}", application);
			int num2 = -1;
			if (string.Equals(application, "MSIE", StringComparison.Ordinal) || string.Equals(application, "Firefox", StringComparison.Ordinal) || string.Equals(application, "Chrome", StringComparison.Ordinal))
			{
				num += application.Length + 1;
			}
			else if (string.Equals(application, "Safari", StringComparison.Ordinal))
			{
				string text = "Version/";
				num = userAgent.IndexOf(text) + text.Length;
			}
			else
			{
				if (!string.Equals(application, UserAgentParser.ie11ApplicationName, StringComparison.Ordinal))
				{
					return;
				}
				num += application.Length;
			}
			int j;
			for (j = num; j < userAgent.Length; j++)
			{
				if (!char.IsDigit(userAgent, j) && userAgent[j] != '.')
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
				ExTraceGlobals.VerboseTracer.TraceDebug(0L, "Unable to parse browser version.  Could not find semicolon");
				return;
			}
			string text2 = userAgent.Substring(num, num2 - num);
			try
			{
				version = new UserAgentParser.UserAgentVersion(text2);
			}
			catch (ArgumentException)
			{
				ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "TryParse failed, unable to parse browser version = {0}", text2);
				return;
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "Parsed out version = {0}", version.ToString());
			for (i = 0; i < UserAgentParser.clientPlatform.Length; i++)
			{
				if (-1 != userAgent.IndexOf(UserAgentParser.clientPlatform[i], StringComparison.OrdinalIgnoreCase))
				{
					platform = UserAgentParser.clientPlatform[i];
					break;
				}
			}
			ExTraceGlobals.VerboseTracer.TraceDebug<string>(0L, "Parsed out platform = {0}", platform);
		}

		internal static bool IsMonitoringRequest(string userAgent)
		{
			return !string.IsNullOrEmpty(userAgent) && userAgent.IndexOf("MSEXCHMON", StringComparison.OrdinalIgnoreCase) != -1;
		}

		private static string ie11ExtraCheck = "Trident";

		private static string ie11ApplicationName = "rv:";

		private static string[] clientApplication = new string[]
		{
			"Opera",
			"Netscape",
			"MSIE",
			"Safari",
			"Firefox",
			"Chrome",
			"rv:"
		};

		private static string[] clientPlatform = new string[]
		{
			"Windows NT",
			"Windows 98; Win 9x 4.90",
			"Windows 2000",
			"Macintosh",
			"Linux"
		};

		internal struct UserAgentVersion : IComparable<UserAgentParser.UserAgentVersion>
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
				for (;;)
				{
					num = version.IndexOf('.', num + 1);
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
						goto IL_68;
					}
				}
				throw new ArgumentException("The version parameter is not a valid User Agent Version");
				IL_68:
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

			public int CompareTo(UserAgentParser.UserAgentVersion userAgentVersionComparand)
			{
				int num = (this.Minor.ToString().Length > userAgentVersionComparand.Minor.ToString().Length) ? this.Minor.ToString().Length : userAgentVersionComparand.Minor.ToString().Length;
				int num2 = (this.Major.ToString().Length > userAgentVersionComparand.Major.ToString().Length) ? this.Major.ToString().Length : userAgentVersionComparand.Major.ToString().Length;
				int num3 = this.Minor + (int)Math.Pow(10.0, (double)num) * this.Major + (int)Math.Pow(10.0, (double)(num2 + num)) * this.Build;
				num = userAgentVersionComparand.Minor.ToString().Length;
				int num4 = userAgentVersionComparand.Minor + (int)Math.Pow(10.0, (double)num) * userAgentVersionComparand.Major + (int)Math.Pow(10.0, (double)(num2 + num)) * userAgentVersionComparand.Build;
				return num3 - num4;
			}

			private const string FormatToString = "{0}.{1}.{2}";

			private int build;

			private int major;

			private int minor;
		}
	}
}
