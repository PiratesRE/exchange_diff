using System;
using Microsoft.Exchange.Data.ConfigurationSettings;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings
{
	[Serializable]
	public class SettingsForestScope : SettingsScope
	{
		internal override int DefaultPriority
		{
			get
			{
				return 100;
			}
		}

		internal override QueryFilter ConstructScopeFilter(IConfigSchema schema)
		{
			return QueryFilter.True;
		}

		public override string ToString()
		{
			return "Forest";
		}

		internal override void Validate(IConfigSchema schema)
		{
			if (base.Restriction != null)
			{
				throw new ConfigurationSettingsRestrictionNotExpectedException(base.GetType().Name);
			}
		}
	}
}
