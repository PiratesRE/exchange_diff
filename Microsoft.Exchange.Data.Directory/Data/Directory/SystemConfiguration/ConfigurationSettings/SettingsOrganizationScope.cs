using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsOrganizationScope : SettingsDatabaseScope
	{
		public SettingsOrganizationScope()
		{
		}

		public SettingsOrganizationScope(string nameMatch, string minVersion, string maxVersion) : base(nameMatch, minVersion, maxVersion)
		{
		}

		internal override int DefaultPriority
		{
			get
			{
				return 400;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			if (base.Restriction.MinExchangeVersion != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, SettingsScopeFilterSchema.OrganizationVersion, base.Restriction.MinExchangeVersion));
			}
			if (base.Restriction.MaxExchangeVersion != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, SettingsScopeFilterSchema.OrganizationVersion, base.Restriction.MaxExchangeVersion));
			}
			if (!string.IsNullOrEmpty(base.Restriction.NameMatch))
			{
				list.Add(new TextFilter(SettingsScopeFilterSchema.OrganizationName, base.Restriction.NameMatch, MatchOptions.WildcardString, MatchFlags.IgnoreCase));
			}
			return QueryFilter.AndTogether(list.ToArray()) ?? QueryFilter.False;
		}
	}
}
