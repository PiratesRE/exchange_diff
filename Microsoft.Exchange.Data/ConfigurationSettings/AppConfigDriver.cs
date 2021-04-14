using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.ConfigurationSettings
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AppConfigDriver : ConfigDriverBase
	{
		public AppConfigDriver(IConfigSchema schema) : this(schema, new TimeSpan?(ConfigDriverBase.DefaultErrorThresholdInterval))
		{
		}

		public AppConfigDriver(IConfigSchema schema, TimeSpan? errorThresholdInterval) : base(schema, errorThresholdInterval)
		{
			this.section = null;
		}

		protected string ConfigFilePath { get; set; }

		protected ConfigurationSection Section
		{
			get
			{
				ConfigurationSection result;
				lock (this)
				{
					if (this.section == null)
					{
						this.RunConfigurationOperation(2, delegate
						{
							Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
							this.section = configuration.GetSection(base.Schema.SectionName);
						});
					}
					result = this.section;
				}
				return result;
			}
		}

		private FileSystemWatcher ConfigWatcher { get; set; }

		public override bool TryGetBoxedSetting(ISettingsContext context, string settingName, Type settingType, out object settingValue)
		{
			ConfigurationSection configurationSection = this.Section;
			if (configurationSection != null)
			{
				ExchangeConfigurationSection exchangeConfigurationSection = configurationSection as ExchangeConfigurationSection;
				if (exchangeConfigurationSection != null)
				{
					object propertyValue = exchangeConfigurationSection.GetPropertyValue(settingName);
					if (propertyValue != exchangeConfigurationSection.GetConfigurationProperty(settingName, null).DefaultValue)
					{
						settingValue = propertyValue;
						return true;
					}
				}
				else
				{
					AppSettingsSection appSettingsSection = configurationSection as AppSettingsSection;
					KeyValueConfigurationElement keyValueConfigurationElement = appSettingsSection.Settings[settingName];
					if (keyValueConfigurationElement != null)
					{
						settingValue = base.ParseAndValidateConfigValue(settingName, keyValueConfigurationElement.Value, settingType);
						return true;
					}
				}
			}
			settingValue = null;
			return false;
		}

		public override XElement GetDiagnosticInfo(string argument)
		{
			XElement diagnosticInfo = base.GetDiagnosticInfo(argument);
			diagnosticInfo.Add(new XElement("description", "App config contains DEFAULTS.  They may be overrided by updating app.config directly but it's not recommended"));
			ConfigurationSection configurationSection = this.Section;
			if (configurationSection != null)
			{
				XElement xelement = new XElement(base.Schema.SectionName);
				ExchangeConfigurationSection exchangeConfigurationSection = configurationSection as ExchangeConfigurationSection;
				if (exchangeConfigurationSection != null)
				{
					using (IEnumerator<string> enumerator = exchangeConfigurationSection.Settings.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							string text = enumerator.Current;
							xelement.Add(new XElement(text, exchangeConfigurationSection.GetPropertyValue(text)));
						}
						goto IL_FC;
					}
				}
				AppSettingsSection appSettingsSection = configurationSection as AppSettingsSection;
				foreach (object obj in appSettingsSection.Settings)
				{
					KeyValueConfigurationElement keyValueConfigurationElement = (KeyValueConfigurationElement)obj;
					xelement.Add(new XElement(keyValueConfigurationElement.Key, keyValueConfigurationElement.Value));
				}
				IL_FC:
				diagnosticInfo.Add(xelement);
			}
			return diagnosticInfo;
		}

		public override void Initialize()
		{
			if (base.IsInitialized)
			{
				return;
			}
			this.ConfigFilePath = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None).FilePath;
			this.InitializeFileWatcher();
			base.IsInitialized = true;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<AppConfigDriver>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.ConfigWatcher != null)
				{
					this.ConfigWatcher.Dispose();
				}
				this.ConfigWatcher = null;
			}
		}

		private void RunConfigurationOperation(int maxSeconds, Action operation)
		{
			for (int i = 1; i > 0; i <<= 1)
			{
				try
				{
					operation();
					base.HandleLoadSuccess();
					break;
				}
				catch (Exception ex)
				{
					if (!(ex is IOException) && !(ex is ConfigurationErrorsException))
					{
						throw;
					}
					if (2 * i > maxSeconds)
					{
						this.HandleLoadError(new ConfigurationSettingsAppSettingsException(ex));
					}
					Thread.Sleep(TimeSpan.FromSeconds((double)i));
				}
			}
		}

		private void InitializeFileWatcher()
		{
			this.ConfigWatcher = new FileSystemWatcher();
			this.ConfigWatcher.Path = Path.GetDirectoryName(this.ConfigFilePath);
			string fileName = Path.GetFileName(this.ConfigFilePath);
			this.ConfigWatcher.Filter = fileName;
			this.ConfigWatcher.NotifyFilter = NotifyFilters.LastWrite;
			this.ConfigWatcher.Changed += this.FileWatcherChanged;
			this.ConfigWatcher.EnableRaisingEvents = true;
		}

		private void FileWatcherChanged(object sender, FileSystemEventArgs e)
		{
			if (!e.FullPath.EndsWith(this.ConfigFilePath, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			this.RunConfigurationOperation(120, delegate
			{
				ConfigurationManager.RefreshSection(base.Schema.SectionName);
				lock (this)
				{
					this.section = null;
				}
				base.LastUpdated = DateTime.UtcNow;
			});
		}

		private const string Description = "App config contains DEFAULTS.  They may be overrided by updating app.config directly but it's not recommended";

		private ConfigurationSection section;
	}
}
