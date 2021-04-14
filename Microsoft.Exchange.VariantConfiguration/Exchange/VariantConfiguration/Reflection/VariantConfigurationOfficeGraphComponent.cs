using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationOfficeGraphComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationOfficeGraphComponent() : base("OfficeGraph")
		{
			base.Add(new VariantConfigurationSection("OfficeGraph.settings.ini", "OfficeGraphGenerateSignals", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("OfficeGraph.settings.ini", "OfficeGraphAgent", typeof(IFeature), false));
		}

		public VariantConfigurationSection OfficeGraphGenerateSignals
		{
			get
			{
				return base["OfficeGraphGenerateSignals"];
			}
		}

		public VariantConfigurationSection OfficeGraphAgent
		{
			get
			{
				return base["OfficeGraphAgent"];
			}
		}
	}
}
