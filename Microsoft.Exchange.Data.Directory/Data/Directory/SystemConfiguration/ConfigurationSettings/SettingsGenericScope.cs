using System;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsGenericScope : SettingsScope
	{
		public SettingsGenericScope()
		{
		}

		public SettingsGenericScope(string scopeType, string nameMatch) : base(scopeType, nameMatch)
		{
		}

		internal override int DefaultPriority
		{
			get
			{
				return 600;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			if (!string.IsNullOrEmpty(base.Restriction.SubType))
			{
				SettingsScopeFilterSchema schemaInstance = SettingsScopeFilterSchema.GetSchemaInstance(schema);
				PropertyDefinition propertyDefinition = schemaInstance.LookupSchemaProperty(base.Restriction.SubType);
				if (propertyDefinition != null)
				{
					return new TextFilter(propertyDefinition, base.Restriction.NameMatch, MatchOptions.WildcardString, MatchFlags.IgnoreCase);
				}
			}
			return QueryFilter.False;
		}

		internal override void Validate(IConfigSchema schema)
		{
			if (base.Restriction == null)
			{
				throw new ConfigurationSettingsRestrictionExpectedException(base.GetType().Name);
			}
			if (string.IsNullOrEmpty(base.Restriction.SubType))
			{
				throw new ConfigurationSettingsRestrictionMissingProperty(base.GetType().Name, "SubType");
			}
			if (!string.IsNullOrEmpty(base.Restriction.MinVersion))
			{
				throw new ConfigurationSettingsRestrictionExtraProperty(base.GetType().Name, "MinVersion");
			}
			if (!string.IsNullOrEmpty(base.Restriction.MaxVersion))
			{
				throw new ConfigurationSettingsRestrictionExtraProperty(base.GetType().Name, "MaxVersion");
			}
			if (schema != null)
			{
				schema.ParseAndValidateScopeValue(base.Restriction.SubType, base.Restriction.NameMatch);
			}
		}
	}
}
