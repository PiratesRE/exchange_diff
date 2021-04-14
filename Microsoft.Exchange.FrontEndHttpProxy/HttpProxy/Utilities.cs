using System;
using System.Globalization;
using Microsoft.Exchange.Clients.Owa.Core;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.HttpProxy.Common;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	public static class Utilities
	{
		public static bool IsPartnerHostedOnly
		{
			get
			{
				return HttpProxyGlobals.IsPartnerHostedOnly;
			}
		}

		public static BrowserType GetBrowserType(string userAgent)
		{
			if (userAgent == null)
			{
				return BrowserType.Other;
			}
			string a = null;
			string text = null;
			UserAgentParser.UserAgentVersion userAgentVersion;
			UserAgentParser.Parse(userAgent, out a, out userAgentVersion, out text);
			if (string.Equals(a, "MSIE", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.IE;
			}
			if (string.Equals(a, "Opera", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Opera;
			}
			if (string.Equals(a, "Safari", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Safari;
			}
			if (string.Equals(a, "Firefox", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Firefox;
			}
			if (string.Equals(a, "Chrome", StringComparison.OrdinalIgnoreCase))
			{
				return BrowserType.Chrome;
			}
			return BrowserType.Other;
		}

		public static bool IsViet()
		{
			CultureInfo userCulture = Culture.GetUserCulture();
			return Utilities.IsViet(userCulture);
		}

		public static bool IsViet(CultureInfo userCulture)
		{
			if (userCulture == null)
			{
				throw new ArgumentNullException("userCulture");
			}
			return userCulture.LCID == 1066;
		}

		internal static SidAndAttributesType[] SidStringAndAttributesConverter(SidStringAndAttributes[] sidStringAndAttributesArray)
		{
			if (sidStringAndAttributesArray == null)
			{
				return null;
			}
			SidAndAttributesType[] array = new SidAndAttributesType[sidStringAndAttributesArray.Length];
			for (int i = 0; i < sidStringAndAttributesArray.Length; i++)
			{
				array[i] = new SidAndAttributesType
				{
					SecurityIdentifier = sidStringAndAttributesArray[i].SecurityIdentifier,
					Attributes = sidStringAndAttributesArray[i].Attributes
				};
			}
			return array;
		}

		internal static string FormatServerVersion(int serverVersion)
		{
			ServerVersion serverVersion2 = new ServerVersion(serverVersion);
			return string.Format(CultureInfo.InvariantCulture, "{0:d}.{1:d2}.{2:d4}.{3:d3}", new object[]
			{
				serverVersion2.Major,
				serverVersion2.Minor,
				serverVersion2.Build,
				serverVersion2.Revision
			});
		}

		internal static string NormalizeExchClientVer(string version)
		{
			if (string.IsNullOrWhiteSpace(version))
			{
				return version;
			}
			string[] array = version.Split(new char[]
			{
				'.'
			});
			return string.Join(".", new string[]
			{
				array[0],
				(array.Length > 1) ? array[1] : "0",
				(array.Length > 2) ? array[2] : "1",
				(array.Length > 3) ? array[3] : "0"
			});
		}

		internal static string GetTruncatedString(string inputString, int maxLength)
		{
			if (string.IsNullOrEmpty(inputString) || maxLength <= 0)
			{
				return inputString;
			}
			if (inputString.Length <= maxLength)
			{
				return inputString;
			}
			return inputString.Substring(0, maxLength);
		}

		internal static bool TryParseDBMountedOnServerHeader(string headerValue, out Guid mdbGuid, out Fqdn serverFqdn, out int serverVersion)
		{
			mdbGuid = default(Guid);
			serverFqdn = null;
			serverVersion = 0;
			if (string.IsNullOrEmpty(headerValue))
			{
				return false;
			}
			string[] array = headerValue.Split(new char[]
			{
				'~'
			});
			return array.Length == 3 && Guid.TryParse(array[0], out mdbGuid) && Fqdn.TryParse(array[1], out serverFqdn) && int.TryParse(array[2], out serverVersion) && serverVersion != 0;
		}

		internal static bool TryGetSiteNameFromServerFqdn(string serverFqdn, out string siteName)
		{
			siteName = string.Empty;
			if (string.IsNullOrEmpty(serverFqdn))
			{
				throw new ArgumentNullException("serverFqdn");
			}
			string[] array = serverFqdn.Split(new char[]
			{
				'.'
			});
			if ((Utilities.IsPartnerHostedOnly || VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.SiteNameFromServerFqdnTranslation.Enabled) && array[0].Length > 5)
			{
				siteName = array[0].Substring(0, array[0].Length - 5);
				return true;
			}
			siteName = array[0];
			return true;
		}

		internal static string GetForestFqdnFromServerFqdn(string serverFqdn)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("serverFqdn", serverFqdn);
			int num = serverFqdn.IndexOf('.');
			return serverFqdn.Substring(num + 1, serverFqdn.Length - num - 1);
		}

		internal static ServerVersion ConvertToServerVersion(string version)
		{
			if (string.IsNullOrEmpty(version))
			{
				return null;
			}
			Version version2 = Version.Parse(version);
			return new ServerVersion(version2.Major, version2.Minor, version2.Build, version2.Revision);
		}
	}
}
