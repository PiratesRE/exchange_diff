using System;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsUserScope : SettingsScope
	{
		public SettingsUserScope()
		{
		}

		public SettingsUserScope(Guid? guidMatch) : base(guidMatch)
		{
		}

		internal override int DefaultPriority
		{
			get
			{
				return 500;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			Guid? guidMatch = base.Restriction.GuidMatch;
			if (guidMatch != null)
			{
				return new ComparisonFilter(ComparisonOperator.Equal, SettingsScopeFilterSchema.MailboxGuid, guidMatch.Value);
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
			if (base.Restriction.GuidMatch == null)
			{
				throw new ConfigurationSettingsRestrictionExpectedException(base.GetType().Name);
			}
		}
	}
}
