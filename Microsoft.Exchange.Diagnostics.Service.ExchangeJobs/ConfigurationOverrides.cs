using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Exchange.Diagnostics.Service.Common;
using Microsoft.ExLogAnalyzer;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs
{
	public class ConfigurationOverrides : ConfigurationOverrides
	{
		protected override Dictionary<string, string> Read()
		{
			DateTime utcNow = DateTime.UtcNow;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Version obj = new Version();
			try
			{
				Assembly executingAssembly = Assembly.GetExecutingAssembly();
				obj = executingAssembly.GetName().Version;
				AssemblyFileVersionAttribute[] array = executingAssembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true) as AssemblyFileVersionAttribute[];
				if (array != null && array.Length > 0)
				{
					obj = new Version(array[0].Version);
				}
			}
			catch (Exception ex)
			{
				Log.LogErrorMessage("ConfigurationOverrides: Exception '{0}' while reading version numbers.", new object[]
				{
					ex
				});
			}
			try
			{
				string adpath = ConfigurationOverrides.GetADPath();
				using (DirectorySearcher directorySearcher = new DirectorySearcher(new DirectoryEntry(adpath), "(objectClass=msExchMonitoringOverride)", ConfigurationOverrides.OverrideProperties, SearchScope.OneLevel))
				{
					using (SearchResultCollection searchResultCollection = directorySearcher.FindAll())
					{
						foreach (object obj2 in searchResultCollection)
						{
							SearchResult searchResult = (SearchResult)obj2;
							DirectoryEntry directoryEntry = searchResult.GetDirectoryEntry();
							string property = ConfigurationOverrides.GetProperty(directoryEntry, "msExchConfigurationXML");
							MatchCollection matchCollection = ConfigurationOverrides.AddKeyRegex.Matches(property);
							string key;
							string value;
							if (matchCollection.Count == 1)
							{
								key = matchCollection[0].Groups[1].Value;
								value = matchCollection[0].Groups[2].Value;
							}
							else
							{
								key = ConfigurationOverrides.GetProperty(directoryEntry, "name");
								value = property;
							}
							string property2 = ConfigurationOverrides.GetProperty(directoryEntry, "msExchADCGlobalNames");
							DateTime t;
							if (!DateTime.TryParse(property2, out t) || !(utcNow > t))
							{
								string property3 = ConfigurationOverrides.GetProperty(directoryEntry, "msExchMonitoringOverrideApplyVersion");
								Version version;
								if (!ConfigurationOverrides.TryParseVersion(property3, out version) || version.Equals(obj))
								{
									dictionary[key] = value;
								}
							}
						}
					}
				}
			}
			catch (COMException ex2)
			{
				if (ex2.ErrorCode != -2147016656)
				{
					Log.LogErrorMessage("ConfigurationOverrides: Exception '{0}' while reading AD configuration overrides.", new object[]
					{
						ex2
					});
				}
			}
			catch (Exception ex3)
			{
				Log.LogErrorMessage("ConfigurationOverrides: Exception '{0}' while reading AD configuration overrides.", new object[]
				{
					ex3
				});
			}
			return dictionary;
		}

		private static string GetProperty(DirectoryEntry entry, string propertyName)
		{
			PropertyValueCollection propertyValueCollection = entry.Properties[propertyName];
			if (propertyValueCollection == null || propertyValueCollection.Count != 1)
			{
				return null;
			}
			return propertyValueCollection[0].ToString();
		}

		private static string GetADPath()
		{
			string path = "LDAP://rootDSE";
			string result;
			using (DirectoryEntry directoryEntry = new DirectoryEntry(path))
			{
				directoryEntry.AuthenticationType = AuthenticationTypes.ReadonlyServer;
				string str = directoryEntry.Properties["configurationNamingContext"][0].ToString();
				string str2 = "CN=Microsoft Exchange,CN=Services," + str;
				using (DirectoryEntry directoryEntry2 = new DirectoryEntry("LDAP://" + str2))
				{
					string path2;
					using (DirectorySearcher directorySearcher = new DirectorySearcher(directoryEntry2, "(objectClass=msExchOrganizationContainer)", null, SearchScope.OneLevel))
					{
						SearchResult searchResult = directorySearcher.FindOne();
						path2 = searchResult.Path;
					}
					result = "LDAP://CN=Configuration,CN=EDS,CN=Monitor,CN=Overrides,CN=Monitoring Settings," + path2.Substring("LDAP://".Length);
				}
			}
			return result;
		}

		private static bool TryParseVersion(string versionString, out Version version)
		{
			if (versionString == null)
			{
				version = new Version(0, 0, 0, 0);
				return false;
			}
			Match match = ConfigurationOverrides.VersionRegex.Match(versionString);
			if (match != null)
			{
				int major = ConfigurationOverrides.ParseInt(match.Groups[1].Value);
				int minor = ConfigurationOverrides.ParseInt(match.Groups[2].Value);
				int num = ConfigurationOverrides.ParseInt(match.Groups[3].Value);
				int revision = ConfigurationOverrides.ParseInt(match.Groups[4].Value);
				if (num > 30000)
				{
					num -= 30000;
				}
				version = new Version(major, minor, num, revision);
				return true;
			}
			version = new Version(0, 0, 0, 0);
			return false;
		}

		private static int ParseInt(string value)
		{
			if (!string.IsNullOrEmpty(value))
			{
				return int.Parse(value);
			}
			return 0;
		}

		private const int LegacyBuildOffset = 30000;

		private const string PropertyName = "name";

		private const string PropertyValue = "msExchConfigurationXML";

		private const string ExpirationTime = "msExchADCGlobalNames";

		private const string ApplyVersion = "msExchMonitoringOverrideApplyVersion";

		private static readonly string[] OverrideProperties = new string[]
		{
			"name",
			"msExchConfigurationXML",
			"msExchADCGlobalNames",
			"msExchMonitoringOverrideApplyVersion"
		};

		private static readonly Regex VersionRegex = new Regex("^Version (\\d+)(?:\\.(\\d+))? \\(Build (\\d+)(?:\\.(\\d+))?", RegexOptions.Compiled);

		private static readonly Regex AddKeyRegex = new Regex("^\\s*<add\\s*key=[\"'](.*?)[\"']\\s*value=[\"'](.*?)[\"']\\s*/>\\s*$", RegexOptions.Compiled);
	}
}
