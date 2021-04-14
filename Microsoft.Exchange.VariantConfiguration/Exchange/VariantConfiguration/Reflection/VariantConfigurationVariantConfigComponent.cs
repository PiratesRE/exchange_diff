using System;
using Microsoft.Exchange.VariantConfiguration.Settings;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationVariantConfigComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationVariantConfigComponent() : base("VariantConfig")
		{
			base.Add(new VariantConfigurationSection("VariantConfig.settings.ini", "Microsoft", typeof(IOrganizationNameSettings), false));
			base.Add(new VariantConfigurationSection("VariantConfig.settings.ini", "InternalAccess", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("VariantConfig.settings.ini", "SettingOverrideSync", typeof(IOverrideSyncSettings), false));
		}

		public VariantConfigurationSection Microsoft
		{
			get
			{
				return base["Microsoft"];
			}
		}

		public VariantConfigurationSection InternalAccess
		{
			get
			{
				return base["InternalAccess"];
			}
		}

		public VariantConfigurationSection SettingOverrideSync
		{
			get
			{
				return base["SettingOverrideSync"];
			}
		}
	}
}
