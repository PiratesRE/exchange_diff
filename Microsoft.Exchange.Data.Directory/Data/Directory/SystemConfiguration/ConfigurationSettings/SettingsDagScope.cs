using System;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsDagScope : SettingsServerScope
	{
		public SettingsDagScope()
		{
		}

		public SettingsDagScope(Guid? guidMatch) : base(guidMatch)
		{
		}

		internal override int DefaultPriority
		{
			get
			{
				return 150;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			Guid? guidMatch = base.Restriction.GuidMatch;
			if (guidMatch != null)
			{
				return new ComparisonFilter(ComparisonOperator.Equal, SettingsScopeFilterSchema.DagOrServerGuid, guidMatch.Value);
			}
			return QueryFilter.False;
		}
	}
}
