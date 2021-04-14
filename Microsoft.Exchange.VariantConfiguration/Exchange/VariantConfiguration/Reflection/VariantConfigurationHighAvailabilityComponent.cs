using System;
using Microsoft.Exchange.Cluster.Shared;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationHighAvailabilityComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationHighAvailabilityComponent() : base("HighAvailability")
		{
			base.Add(new VariantConfigurationSection("HighAvailability.settings.ini", "ActiveManager", typeof(IActiveManagerSettings), false));
		}

		public VariantConfigurationSection ActiveManager
		{
			get
			{
				return base["ActiveManager"];
			}
		}
	}
}
