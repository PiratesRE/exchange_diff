using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationTest2Component : VariantConfigurationComponent
	{
		internal VariantConfigurationTest2Component() : base("Test2")
		{
			base.Add(new VariantConfigurationSection("Test2.settings.ini", "Test2SettingsOn", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Test2.settings.ini", "Test2Settings", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Test2.settings.ini", "Test2SettingsEnterprise", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Test2.settings.ini", "Test2SettingsOff", typeof(IFeature), false));
		}

		public VariantConfigurationSection Test2SettingsOn
		{
			get
			{
				return base["Test2SettingsOn"];
			}
		}

		public VariantConfigurationSection Test2Settings
		{
			get
			{
				return base["Test2Settings"];
			}
		}

		public VariantConfigurationSection Test2SettingsEnterprise
		{
			get
			{
				return base["Test2SettingsEnterprise"];
			}
		}

		public VariantConfigurationSection Test2SettingsOff
		{
			get
			{
				return base["Test2SettingsOff"];
			}
		}
	}
}
