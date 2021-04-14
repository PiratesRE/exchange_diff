using System;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Autodiscover.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class UserAgentHelper
	{
		internal static bool IsWindowsClient(string userAgentString)
		{
			return UserAgentHelper.ValidateClientSoftwareVersions(userAgentString, new UserAgentHelper.WindowsVersionNumberPredicate(UserAgentHelper.AcceptAnyWindowsVersion), new UserAgentHelper.OfficeVersionNumberPredicate(UserAgentHelper.AcceptAnyOfficeVersion));
		}

		internal static bool IsClientWin7OrGreater(string userAgentString)
		{
			return UserAgentHelper.ValidateClientSoftwareVersions(userAgentString, (int windowsVersionMajor, int windowsVersionMinor) => windowsVersionMajor > 6 || (windowsVersionMajor == 6 && windowsVersionMinor >= 1), new UserAgentHelper.OfficeVersionNumberPredicate(UserAgentHelper.AcceptAnyOfficeVersion));
		}

		internal static bool TryGetOfficeVersion(string userAgentString, out Version officeVersion)
		{
			officeVersion = null;
			int major = 0;
			int minor = 0;
			int build = 0;
			int num = 0;
			int num2 = 0;
			if (!string.IsNullOrEmpty(userAgentString) && UserAgentHelper.TryParseUserAgent(userAgentString, out major, out minor, out build, out num, out num2))
			{
				officeVersion = new Version(major, minor, build, 0);
				return true;
			}
			return false;
		}

		public static bool ValidateClientSoftwareVersions(string userAgentString, UserAgentHelper.WindowsVersionNumberPredicate windowsVersionValidator, UserAgentHelper.OfficeVersionNumberPredicate officeVersionValidator)
		{
			if (!string.IsNullOrEmpty(userAgentString))
			{
				int majorVersion = 0;
				int minorVersion = 0;
				int buildNumber = 0;
				int majorVersion2 = 0;
				int minorVersion2 = 0;
				bool flag = UserAgentHelper.TryParseUserAgent(userAgentString, out majorVersion, out minorVersion, out buildNumber, out majorVersion2, out minorVersion2);
				return flag && windowsVersionValidator(majorVersion2, minorVersion2) && officeVersionValidator(majorVersion, minorVersion, buildNumber);
			}
			return false;
		}

		private static bool TryParseUserAgent(string userAgentString, out int officeVersionMajor, out int officeVersionMinor, out int productBuildMajor, out int windowsVersionMajor, out int windowsVersionMinor)
		{
			officeVersionMajor = 0;
			officeVersionMinor = 0;
			productBuildMajor = 0;
			windowsVersionMajor = 0;
			windowsVersionMinor = 0;
			Match match = Regex.Match(userAgentString, "Microsoft Office/(\\d+)\\.(\\d+) \\(Windows NT (\\d+)\\.(\\d+)\\D*\\d+\\.\\d+\\.(\\d+).*\\)");
			if (match.Success)
			{
				try
				{
					officeVersionMajor = int.Parse(match.Groups[1].Value);
					officeVersionMinor = int.Parse(match.Groups[2].Value);
					windowsVersionMajor = int.Parse(match.Groups[3].Value);
					windowsVersionMinor = int.Parse(match.Groups[4].Value);
					productBuildMajor = int.Parse(match.Groups[5].Value);
					return true;
				}
				catch (FormatException)
				{
					return false;
				}
				catch (OverflowException)
				{
					return false;
				}
				return false;
			}
			return false;
		}

		private static bool AcceptAnyWindowsVersion(int unusedMajor, int unusedMinor)
		{
			return true;
		}

		private static bool AcceptAnyOfficeVersion(int unusedMajor, int unusedMinor, int unusedBuildNumber)
		{
			return true;
		}

		internal delegate bool WindowsVersionNumberPredicate(int majorVersion, int minorVersion);

		internal delegate bool OfficeVersionNumberPredicate(int majorVersion, int minorVersion, int buildNumber);
	}
}
