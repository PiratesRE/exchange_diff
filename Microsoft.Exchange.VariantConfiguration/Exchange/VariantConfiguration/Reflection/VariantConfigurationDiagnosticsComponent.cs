using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationDiagnosticsComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationDiagnosticsComponent() : base("Diagnostics")
		{
			base.Add(new VariantConfigurationSection("Diagnostics.settings.ini", "TraceToHeadersLogger", typeof(IFeature), false));
		}

		public VariantConfigurationSection TraceToHeadersLogger
		{
			get
			{
				return base["TraceToHeadersLogger"];
			}
		}
	}
}
