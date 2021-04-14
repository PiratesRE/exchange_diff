using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationMessageTrackingComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationMessageTrackingComponent() : base("MessageTracking")
		{
			base.Add(new VariantConfigurationSection("MessageTracking.settings.ini", "StatsLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MessageTracking.settings.ini", "QueueViewerDiagnostics", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MessageTracking.settings.ini", "AllowDebugMode", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("MessageTracking.settings.ini", "UseBackEndLocator", typeof(IFeature), false));
		}

		public VariantConfigurationSection StatsLogging
		{
			get
			{
				return base["StatsLogging"];
			}
		}

		public VariantConfigurationSection QueueViewerDiagnostics
		{
			get
			{
				return base["QueueViewerDiagnostics"];
			}
		}

		public VariantConfigurationSection AllowDebugMode
		{
			get
			{
				return base["AllowDebugMode"];
			}
		}

		public VariantConfigurationSection UseBackEndLocator
		{
			get
			{
				return base["UseBackEndLocator"];
			}
		}
	}
}
