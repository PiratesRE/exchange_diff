using System;
using System.DirectoryServices;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	internal sealed class AdsUtils
	{
		public static bool GetAdsServiceExists(int ldapPort)
		{
			bool result = false;
			try
			{
				result = DirectoryEntry.Exists(AdsUtils.GetRootPath(ldapPort));
			}
			catch (COMException ex)
			{
				if (-2147016646 == ex.ErrorCode)
				{
					return false;
				}
				throw;
			}
			return result;
		}

		public static DirectoryEntry GetRootDirectoryEntry(int ldapPort)
		{
			return new DirectoryEntry
			{
				Path = AdsUtils.GetRootPath(ldapPort)
			};
		}

		private static string GetRootPath(int ldapPort)
		{
			return string.Format("{0}:{1}/{2}", "LDAP://localhost", ldapPort, "RootDse");
		}

		public const string HostSpecificAdsPathRoot = "LDAP://localhost";

		public const string DseAdsPath = "RootDse";

		public const string CommonNamePropertyName = "cn";

		public const string RootDseSchemaNamingContextPropertyName = "SchemaNamingContext";

		public const string RootDseConfigNamingContextPropertyName = "ConfigurationNamingContext";

		public const string PredefSchemaNamingContextMacro = "#schemaNamingContext";

		public const string PredefConfigNamingContextMacro = "#configurationNamingContext";
	}
}
