using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationGlobalComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationGlobalComponent() : base("Global")
		{
			base.Add(new VariantConfigurationSection("Global.settings.ini", "GlobalCriminalCompliance", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Global.settings.ini", "WindowsLiveID", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Global.settings.ini", "DistributedKeyManagement", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Global.settings.ini", "MultiTenancy", typeof(IFeature), false));
		}

		public VariantConfigurationSection GlobalCriminalCompliance
		{
			get
			{
				return base["GlobalCriminalCompliance"];
			}
		}

		public VariantConfigurationSection WindowsLiveID
		{
			get
			{
				return base["WindowsLiveID"];
			}
		}

		public VariantConfigurationSection DistributedKeyManagement
		{
			get
			{
				return base["DistributedKeyManagement"];
			}
		}

		public VariantConfigurationSection MultiTenancy
		{
			get
			{
				return base["MultiTenancy"];
			}
		}
	}
}
