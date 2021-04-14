using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class ExchangeSettings : InternalExchangeSettings
	{
		public MultiValuedProperty<SettingsGroup> Groups
		{
			get
			{
				if (base.Settings == null)
				{
					return null;
				}
				return new MultiValuedProperty<SettingsGroup>(true, null, base.Settings.Values.ToArray<SettingsGroup>());
			}
		}

		public MultiValuedProperty<string> GroupNames
		{
			get
			{
				if (base.Settings == null)
				{
					return null;
				}
				return new MultiValuedProperty<string>(true, null, base.Settings.Keys.ToArray<string>());
			}
		}

		public override XMLSerializableDictionary<string, SettingsHistory> History
		{
			get
			{
				return (XMLSerializableDictionary<string, SettingsHistory>)this[ExchangeSettingsSchema.History];
			}
		}

		public string DiagnosticInfo { get; set; }

		public KeyValuePair<string, object> EffectiveSetting { get; set; }

		public override string ToString()
		{
			List<SettingsGroup> list = new List<SettingsGroup>(base.Settings.Values);
			list.Sort((SettingsGroup x, SettingsGroup y) => y.Priority.CompareTo(x.Priority));
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SettingsGroup settingsGroup in list)
			{
				stringBuilder.AppendLine(settingsGroup.ToString());
			}
			return stringBuilder.ToString();
		}

		private static ExchangeSettingsSchema schema = ObjectSchema.GetInstance<ExchangeSettingsSchema>();
	}
}
