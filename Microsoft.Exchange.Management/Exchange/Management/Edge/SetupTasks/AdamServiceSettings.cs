using System;
using System.IO;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.Edge.SetupTasks
{
	internal sealed class AdamServiceSettings
	{
		public AdamServiceSettings(string instanceName, string dataFilesPath, string logFilesPath, int ldapPort, int sslPort)
		{
			this.instanceName = instanceName;
			this.dataFilesPath = dataFilesPath;
			this.logFilesPath = logFilesPath;
			this.ldapPort = ldapPort;
			this.sslPort = sslPort;
		}

		private AdamServiceSettings()
		{
		}

		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
		}

		public string DataFilesPath
		{
			get
			{
				return this.dataFilesPath;
			}
		}

		public string LogFilesPath
		{
			get
			{
				return this.logFilesPath;
			}
		}

		public int LdapPort
		{
			get
			{
				return this.ldapPort;
			}
		}

		public int SslPort
		{
			get
			{
				return this.sslPort;
			}
		}

		public static bool GetSettingsExist(string instanceName)
		{
			bool result;
			using (RegistryKey registryKey = AdamServiceSettings.GetAdamServiceSubKey(instanceName).Open())
			{
				result = (null != registryKey);
			}
			return result;
		}

		public static AdamServiceSettings GetFromRegistry(string instanceName)
		{
			AdamServiceSettings result;
			using (RegistryKey registryKey = AdamServiceSettings.GetAdamServiceSubKey(instanceName).Open())
			{
				string text = registryKey.GetValue("DataFilesPath") as string;
				string text2 = registryKey.GetValue("LogFilesPath") as string;
				int num = (int)registryKey.GetValue("LdapPort");
				int num2 = (int)registryKey.GetValue("SslPort");
				result = new AdamServiceSettings(instanceName, text, text2, num, num2);
			}
			return result;
		}

		public static void DeleteFromRegistry(string instanceName)
		{
			RegistrySubKey adamServiceSubKey = AdamServiceSettings.GetAdamServiceSubKey(instanceName);
			adamServiceSubKey.DeleteTreeIfExist();
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings"))
			{
				if (registryKey != null && registryKey.SubKeyCount == 0)
				{
					Registry.LocalMachine.DeleteSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings");
				}
			}
		}

		public void SaveToRegistry()
		{
			RegistrySubKey adamServiceSubKey = AdamServiceSettings.GetAdamServiceSubKey(this.InstanceName);
			adamServiceSubKey.DeleteTreeIfExist();
			using (RegistryKey registryKey = adamServiceSubKey.Create())
			{
				registryKey.SetValue("DataFilesPath", this.DataFilesPath);
				registryKey.SetValue("LogFilesPath", this.LogFilesPath);
				registryKey.SetValue("LdapPort", this.LdapPort);
				registryKey.SetValue("SslPort", this.SslPort);
			}
		}

		private static RegistrySubKey GetAdamServiceSubKey(string instanceName)
		{
			return new RegistrySubKey(Registry.LocalMachine, Path.Combine("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings", instanceName));
		}

		public const string ExchangeGatewayRoleSettingsRegPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole";

		public const string AdamServiceSettingsKey = "AdamSettings";

		public const string AdamSettingsRegKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings";

		private const string DataFilesPathRegValueName = "DataFilesPath";

		private const string LogFilesPathRegValueName = "LogFilesPath";

		private const string LdapPortRegValueName = "LdapPort";

		private const string SslPortRegValueName = "SslPort";

		private readonly string instanceName;

		private readonly string dataFilesPath;

		private readonly string logFilesPath;

		private readonly int ldapPort;

		private readonly int sslPort;
	}
}
