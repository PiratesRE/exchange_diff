using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;

namespace Microsoft.Exchange.Diagnostics.Service.Common
{
	public class DiagnosticsConfiguration
	{
		public DiagnosticsConfiguration()
		{
			string exePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Microsoft.Exchange.Diagnostics.Service.exe");
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(exePath);
			if (!configuration.HasFile)
			{
				throw new FileNotFoundException("No configuration file exists for service.");
			}
			this.serviceConfiguration = (configuration.GetSection("ServiceConfiguration") as ServiceConfiguration);
			this.jobSection = (configuration.GetSection("JobSection") as JobSection);
		}

		public ServiceConfiguration ServiceConfiguration
		{
			get
			{
				return this.serviceConfiguration;
			}
		}

		public JobSection JobSection
		{
			get
			{
				return this.jobSection;
			}
		}

		public static bool GetIsDatacenterMode()
		{
			object obj = null;
			string value = null;
			if (CommonUtils.TryGetRegistryValue(DiagnosticsConfiguration.DiagnosticsRegistryKey, "LogFolderPath", null, out obj))
			{
				value = obj.ToString();
			}
			return !string.IsNullOrEmpty(value) || DiagnosticsConfiguration.GetIsInDogfoodDomain(".EXCHANGE.CORP.MICROSOFT.COM");
		}

		private static bool GetIsInDogfoodDomain(string domainSuffix)
		{
			bool result = false;
			string hostName = Dns.GetHostName();
			if (!string.IsNullOrEmpty(hostName))
			{
				IPHostEntry hostEntry = Dns.GetHostEntry(hostName);
				if (hostEntry != null && !string.IsNullOrEmpty(hostEntry.HostName))
				{
					result = hostEntry.HostName.EndsWith(domainSuffix, StringComparison.OrdinalIgnoreCase);
				}
			}
			return result;
		}

		private const string DiagnosticsLogFolderRegistryValue = "LogFolderPath";

		public static readonly string DiagnosticsRegistryKey = "HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ExchangeServer\\v15\\Diagnostics";

		private readonly ServiceConfiguration serviceConfiguration;

		private readonly JobSection jobSection;
	}
}
