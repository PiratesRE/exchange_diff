using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsDatabaseScope : SettingsServerScope
	{
		public SettingsDatabaseScope()
		{
		}

		public SettingsDatabaseScope(Guid? guidMatch) : base(guidMatch)
		{
		}

		public SettingsDatabaseScope(string nameMatch, string minVersion, string maxVersion) : base(nameMatch, minVersion, maxVersion)
		{
		}

		internal override int DefaultPriority
		{
			get
			{
				return 300;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			List<QueryFilter> list = new List<QueryFilter>();
			Guid? guidMatch = base.Restriction.GuidMatch;
			if (guidMatch != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, SettingsScopeFilterSchema.DatabaseGuid, guidMatch.Value));
			}
			if (base.Restriction.MinServerVersion != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, SettingsScopeFilterSchema.DatabaseVersion, base.Restriction.MinServerVersion));
			}
			if (base.Restriction.MaxServerVersion != null)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.LessThanOrEqual, SettingsScopeFilterSchema.DatabaseVersion, base.Restriction.MaxServerVersion));
			}
			if (!string.IsNullOrEmpty(base.Restriction.NameMatch))
			{
				list.Add(new TextFilter(SettingsScopeFilterSchema.DatabaseName, base.Restriction.NameMatch, MatchOptions.WildcardString, MatchFlags.IgnoreCase));
			}
			return QueryFilter.AndTogether(list.ToArray()) ?? QueryFilter.False;
		}
	}
}
