using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public sealed class SettingsXml : XMLSerializableBase
	{
		[XmlElement(ElementName = "Sts")]
		public SettingsGroupDictionary Settings { get; set; }

		[XmlElement(ElementName = "Hst")]
		public SettingsHistoryGroup History { get; set; }

		public static SettingsXml Create()
		{
			return new SettingsXml
			{
				Settings = new SettingsGroupDictionary(),
				History = new SettingsHistoryGroup()
			};
		}

		public void AddSettingsGroup(SettingsGroup settingsGroup)
		{
			if (this.Settings.ContainsKey(settingsGroup.Name))
			{
				throw new ConfigurationSettingsGroupExistsException(settingsGroup.Name);
			}
			this.Settings[settingsGroup.Name] = settingsGroup;
		}

		public void UpdateSettingsGroup(SettingsGroup settingsGroup)
		{
			if (!this.Settings.ContainsKey(settingsGroup.Name))
			{
				throw new ConfigurationSettingsGroupNotFoundException(settingsGroup.Name);
			}
			this.AddSettingsToHistory(settingsGroup.Name);
			settingsGroup.LastModified = DateTime.UtcNow;
			this.Settings[settingsGroup.Name] = settingsGroup;
		}

		public void RemoveSettingsGroup(SettingsGroup settingsGroup, bool addHistory = true)
		{
			if (!this.Settings.ContainsKey(settingsGroup.Name))
			{
				throw new ConfigurationSettingsGroupNotFoundException(settingsGroup.Name);
			}
			if (addHistory)
			{
				this.AddSettingsToHistory(settingsGroup.Name);
			}
			this.Settings.Remove(settingsGroup.Name);
		}

		public void ResizeHistorySettings(string name, int maxSize)
		{
			if (!this.Settings.ContainsKey(name))
			{
				throw new ConfigurationSettingsGroupNotFoundException(name);
			}
			this.History.ResizeHistorySettings(name, maxSize);
		}

		private void AddSettingsToHistory(string name)
		{
			if (!this.Settings.ContainsKey(name))
			{
				throw new ConfigurationSettingsGroupNotFoundException(name);
			}
			this.History.AddSettingsToHistory(this.Settings[name]);
		}
	}
}
