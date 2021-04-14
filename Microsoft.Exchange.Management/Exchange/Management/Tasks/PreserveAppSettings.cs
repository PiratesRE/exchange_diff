using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Preserve", "AppSettings")]
	[LocDescription(Strings.IDs.PreserveAppSettingsTask)]
	public sealed class PreserveAppSettings : Task
	{
		[Parameter(Mandatory = true)]
		public string RoleInstallPath
		{
			get
			{
				return this.roleInstallPath;
			}
			set
			{
				this.roleInstallPath = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ConfigFileName
		{
			get
			{
				return this.configFileName;
			}
			set
			{
				this.configFileName = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			string text = Path.Combine(this.roleInstallPath, this.configFileName);
			string sourceFileName = text + ".template";
			if (!File.Exists(text))
			{
				File.Copy(sourceFileName, text);
				return;
			}
			string destFileName = text + ".old";
			string exePath = Path.Combine(this.roleInstallPath, Path.GetFileNameWithoutExtension(text));
			Configuration configuration = ConfigurationManager.OpenExeConfiguration(exePath);
			foreach (string key in PreserveAppSettings.preservedSettingNames)
			{
				if (configuration.AppSettings.Settings[key] != null)
				{
					this.settings[key] = configuration.AppSettings.Settings[key].Value;
				}
			}
			File.Copy(text, destFileName, true);
			File.Copy(sourceFileName, text, true);
			Configuration configuration2 = ConfigurationManager.OpenExeConfiguration(exePath);
			foreach (KeyValuePair<string, string> keyValuePair in this.settings)
			{
				if (!string.IsNullOrEmpty(keyValuePair.Value))
				{
					configuration2.AppSettings.Settings[keyValuePair.Key].Value = keyValuePair.Value;
				}
			}
			configuration2.Save();
		}

		private static string[] preservedSettingNames = new string[]
		{
			"MaxIoThreadsPerCPU",
			"ConnectionCacheSize",
			"TemporaryStoragePath",
			"ThrottlingTimeoutInSeconds",
			"MaxConnectionRatePerMinute"
		};

		private string roleInstallPath;

		private string configFileName;

		private Dictionary<string, string> settings = new Dictionary<string, string>(PreserveAppSettings.preservedSettingNames.Length);
	}
}
