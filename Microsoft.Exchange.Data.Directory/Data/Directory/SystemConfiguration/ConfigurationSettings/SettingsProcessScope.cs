using System;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsProcessScope : SettingsScope
	{
		public SettingsProcessScope()
		{
		}

		public SettingsProcessScope(string nameMatch) : base(nameMatch, null, null)
		{
		}

		internal override int DefaultPriority
		{
			get
			{
				return 250;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			if (!string.IsNullOrEmpty(base.Restriction.NameMatch))
			{
				return new TextFilter(SettingsScopeFilterSchema.ProcessName, base.Restriction.NameMatch, MatchOptions.WildcardString, MatchFlags.IgnoreCase);
			}
			return QueryFilter.False;
		}

		internal override void Validate(IConfigSchema schema)
		{
			if (base.Restriction == null)
			{
				throw new ConfigurationSettingsRestrictionExpectedException(base.GetType().Name);
			}
			if (!string.IsNullOrEmpty(base.Restriction.SubType))
			{
				throw new ConfigurationSettingsRestrictionExtraProperty(base.GetType().Name, "SubType");
			}
			if (!string.IsNullOrEmpty(base.Restriction.MinVersion))
			{
				throw new ConfigurationSettingsRestrictionExtraProperty(base.GetType().Name, "MinVersion");
			}
			if (!string.IsNullOrEmpty(base.Restriction.MaxVersion))
			{
				throw new ConfigurationSettingsRestrictionExtraProperty(base.GetType().Name, "MaxVersion");
			}
			if (string.IsNullOrEmpty(base.Restriction.NameMatch))
			{
				throw new ConfigurationSettingsRestrictionExpectedException(base.GetType().Name);
			}
		}
	}
}
