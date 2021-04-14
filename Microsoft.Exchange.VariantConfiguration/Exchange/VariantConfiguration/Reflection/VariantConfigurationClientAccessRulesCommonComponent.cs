using System;
using Microsoft.Exchange.Flighting;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationClientAccessRulesCommonComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationClientAccessRulesCommonComponent() : base("ClientAccessRulesCommon")
		{
			base.Add(new VariantConfigurationSection("ClientAccessRulesCommon.settings.ini", "ImplicitAllowLocalClientAccessRulesEnabled", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("ClientAccessRulesCommon.settings.ini", "ClientAccessRulesCacheExpiryTime", typeof(ICacheExpiryTimeInMinutes), false));
		}

		public VariantConfigurationSection ImplicitAllowLocalClientAccessRulesEnabled
		{
			get
			{
				return base["ImplicitAllowLocalClientAccessRulesEnabled"];
			}
		}

		public VariantConfigurationSection ClientAccessRulesCacheExpiryTime
		{
			get
			{
				return base["ClientAccessRulesCacheExpiryTime"];
			}
		}
	}
}
