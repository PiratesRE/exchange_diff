using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsServerScope : SettingsScope
	{
		public SettingsServerScope()
		{
		}

		public SettingsServerScope(Guid? guidMatch) : base(guidMatch)
		{
		}

		public SettingsServerScope(string nameMatch, string minVersion, string maxVersion) : base(nameMatch, minVersion, maxVersion)
		{
		}

		internal override int DefaultPriority
		{
			get
			{
				return 200;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			Guid? guidMatch = base.Restriction.GuidMatch;
			if (guidMatch != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, SettingsScopeFilterSchema.ServerGuid, guidMatch.Value));
			}
			if (base.Restriction.MinServerVersion != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, SettingsScopeFilterSchema.ServerVersion, base.Restriction.MinServerVersion));
			}
			if (base.Restriction.MaxServerVersion != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, SettingsScopeFilterSchema.ServerVersion, base.Restriction.MaxServerVersion));
			}
			if (!string.IsNullOrEmpty(base.Restriction.NameMatch))
			{
				list.Add(new TextFilter(SettingsScopeFilterSchema.ServerName, base.Restriction.NameMatch, MatchOptions.WildcardString, MatchFlags.IgnoreCase));
			}
			return QueryFilter.AndTogether(list.ToArray()) ?? QueryFilter.False;
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
			bool flag = false;
			if (!string.IsNullOrEmpty(base.Restriction.MinVersion))
			{
				SettingsScopeRestriction.ValidateAsServerVersion(base.Restriction.MinVersion);
				flag = true;
			}
			if (!string.IsNullOrEmpty(base.Restriction.MaxVersion))
			{
				SettingsScopeRestriction.ValidateAsServerVersion(base.Restriction.MaxVersion);
				flag = true;
			}
			if (!string.IsNullOrEmpty(base.Restriction.NameMatch))
			{
				SettingsScopeRestriction.ValidateNameMatch(base.Restriction.NameMatch);
				flag = true;
			}
			if (base.Restriction.GuidMatch != null)
			{
				flag = true;
			}
			if (!flag)
			{
				throw new ConfigurationSettingsRestrictionExpectedException(base.GetType().Name);
			}
		}
	}
}
