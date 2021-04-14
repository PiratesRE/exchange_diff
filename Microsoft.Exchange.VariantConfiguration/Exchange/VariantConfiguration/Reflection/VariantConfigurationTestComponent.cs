using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationTestComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationTestComponent() : base("Test")
		{
			base.Add(new VariantConfigurationSection("Test.settings.ini", "TestSettingsEnterprise", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Test.settings.ini", "TestSettings", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Test.settings.ini", "TestSettingsOn", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Test.settings.ini", "TestSettingsOff", typeof(IFeature), false));
		}

		public VariantConfigurationSection TestSettingsEnterprise
		{
			get
			{
				return base["TestSettingsEnterprise"];
			}
		}

		public VariantConfigurationSection TestSettings
		{
			get
			{
				return base["TestSettings"];
			}
		}

		public VariantConfigurationSection TestSettingsOn
		{
			get
			{
				return base["TestSettingsOn"];
			}
		}

		public VariantConfigurationSection TestSettingsOff
		{
			get
			{
				return base["TestSettingsOff"];
			}
		}
	}
}
