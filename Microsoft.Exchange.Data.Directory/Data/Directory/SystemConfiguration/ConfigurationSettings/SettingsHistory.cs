using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public sealed class SettingsHistory : XMLSerializableBase
	{
		public SettingsHistory()
		{
		}

		public SettingsHistory(string name)
		{
			this.Name = name;
			this.Entries = new List<SettingsGroup>();
		}

		[XmlAttribute(AttributeName = "Nm")]
		public string Name { get; set; }

		[XmlElement(ElementName = "StsG")]
		public List<SettingsGroup> Entries { get; set; }

		public void AddSettingsToHistory(SettingsGroup group)
		{
			this.Entries.Add(group);
			this.ResizeHistorySettings(10);
		}

		public void ResizeHistorySettings(int maxSize)
		{
			if (this.Entries.Count > maxSize)
			{
				this.Entries.RemoveRange(0, this.Entries.Count - maxSize);
			}
		}

		public override string ToString()
		{
			return DirectoryStrings.ConfigurationSettingsHistorySummary(this.Name, this.Entries.Count);
		}

		private const int MaximumEntryCount = 10;
	}
}
