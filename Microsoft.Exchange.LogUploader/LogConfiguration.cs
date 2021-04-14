using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogConfiguration : ConfigurationSection
	{
		[ConfigurationProperty("Environments", IsRequired = true)]
		public ProcessingEnvironmentCollection Environments
		{
			get
			{
				return (ProcessingEnvironmentCollection)base["Environments"];
			}
		}

		[ConfigurationProperty("ConfigSettings", IsRequired = true)]
		public ConfigCollection ConfigSettings
		{
			get
			{
				return (ConfigCollection)base["ConfigSettings"];
			}
		}

		[ConfigurationProperty("LogManagerPlugin", IsRequired = true)]
		public LogManagerPluginCollection LogProcessingSchemas
		{
			get
			{
				return (LogManagerPluginCollection)base["LogManagerPlugin"];
			}
		}

		public const string EnvironmentsKey = "Environments";

		public const string ConfigSettingsKey = "ConfigSettings";
	}
}
