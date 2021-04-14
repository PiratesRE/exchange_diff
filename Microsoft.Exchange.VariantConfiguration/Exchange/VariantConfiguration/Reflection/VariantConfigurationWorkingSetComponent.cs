using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationWorkingSetComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationWorkingSetComponent() : base("WorkingSet")
		{
			base.Add(new VariantConfigurationSection("WorkingSet.settings.ini", "WorkingSetAgent", typeof(IFeature), false));
		}

		public VariantConfigurationSection WorkingSetAgent
		{
			get
			{
				return base["WorkingSetAgent"];
			}
		}
	}
}
