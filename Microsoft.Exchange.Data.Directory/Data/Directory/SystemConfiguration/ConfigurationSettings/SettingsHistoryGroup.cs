using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public sealed class SettingsHistoryGroup : XMLSerializableCompressed<XMLSerializableDictionary<string, SettingsHistory>>
	{
		protected override PropertyDefinition XmlRawProperty
		{
			get
			{
				return InternalExchangeSettingsSchema.ConfigurationXMLRaw;
			}
		}

		public void ResizeHistorySettings(string name, int maxSize)
		{
			if (base.Value == null || !base.Value.ContainsKey(name))
			{
				return;
			}
			base.Value[name].ResizeHistorySettings(maxSize);
		}

		public void AddSettingsToHistory(SettingsGroup group)
		{
			if (base.Value == null)
			{
				base.Value = new XMLSerializableDictionary<string, SettingsHistory>();
			}
			if (!base.Value.ContainsKey(group.Name))
			{
				SettingsHistory value = new SettingsHistory(group.Name);
				base.Value[group.Name] = value;
			}
			base.Value[group.Name].AddSettingsToHistory(group);
		}
	}
}
