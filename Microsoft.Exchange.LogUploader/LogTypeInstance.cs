using System;
using System.Configuration;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogTypeInstance : ConfigurationElement
	{
		[ConfigurationProperty("dir", IsRequired = true)]
		public string Dir
		{
			get
			{
				return (string)base["dir"];
			}
		}

		[ConfigurationProperty("schema", IsRequired = true)]
		public LogSchemaType Schema
		{
			get
			{
				return (LogSchemaType)base["schema"];
			}
		}

		[ConfigurationProperty("prefix", IsRequired = true)]
		public string Prefix
		{
			get
			{
				return (string)base["prefix"];
			}
		}

		[ConfigurationProperty("cfgName", IsRequired = true)]
		public string ConfigName
		{
			get
			{
				return (string)base["cfgName"];
			}
		}

		[ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
		public bool Enabled
		{
			get
			{
				return (bool)base["enabled"];
			}
			internal set
			{
				base["enabled"] = value;
			}
		}

		[ConfigurationProperty("LogManagerPluginName", IsRequired = false)]
		public string LogManagerPlugin
		{
			get
			{
				return (string)base["LogManagerPluginName"];
			}
		}

		[ConfigurationProperty("watermarkDir", IsRequired = false)]
		public string WatermarkDirCfg
		{
			get
			{
				return (string)base["watermarkDir"];
			}
		}

		[ConfigurationProperty("instanceName", IsRequired = false, DefaultValue = null)]
		public string InstanceName
		{
			get
			{
				return (string)base["instanceName"];
			}
		}

		[ConfigurationProperty("outputDir", IsRequired = false, DefaultValue = null)]
		public string OutputDirectory
		{
			get
			{
				return (string)base["outputDir"];
			}
		}

		public string Instance
		{
			get
			{
				string text;
				if (string.IsNullOrWhiteSpace(this.InstanceName))
				{
					text = this.Prefix;
				}
				else
				{
					text = this.InstanceName;
				}
				foreach (char c in text)
				{
					if (!char.IsLetterOrDigit(c))
					{
						throw new ArgumentOutOfRangeException("instance name is restricted to be a~z A~Z only");
					}
				}
				return text;
			}
		}

		public string WatermarkFileDirectory
		{
			get
			{
				if (string.IsNullOrWhiteSpace(this.WatermarkDirCfg))
				{
					return this.Dir;
				}
				return this.WatermarkDirCfg;
			}
		}

		public const string DirectoryKey = "dir";

		public const string SchemaKey = "schema";

		public const string PrefixKey = "prefix";

		public const string ConfigurationKey = "cfgName";

		public const string EnabledKey = "enabled";
	}
}
