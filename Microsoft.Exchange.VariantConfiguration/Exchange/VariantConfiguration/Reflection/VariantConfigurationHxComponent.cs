using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationHxComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationHxComponent() : base("Hx")
		{
			base.Add(new VariantConfigurationSection("Hx.settings.ini", "SmartSyncWebSockets", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Hx.settings.ini", "EnforceDevicePolicy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Hx.settings.ini", "Irm", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Hx.settings.ini", "ServiceAvailable", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Hx.settings.ini", "ClientSettingsPane", typeof(IFeature), false));
		}

		public VariantConfigurationSection SmartSyncWebSockets
		{
			get
			{
				return base["SmartSyncWebSockets"];
			}
		}

		public VariantConfigurationSection EnforceDevicePolicy
		{
			get
			{
				return base["EnforceDevicePolicy"];
			}
		}

		public VariantConfigurationSection Irm
		{
			get
			{
				return base["Irm"];
			}
		}

		public VariantConfigurationSection ServiceAvailable
		{
			get
			{
				return base["ServiceAvailable"];
			}
		}

		public VariantConfigurationSection ClientSettingsPane
		{
			get
			{
				return base["ClientSettingsPane"];
			}
		}
	}
}
