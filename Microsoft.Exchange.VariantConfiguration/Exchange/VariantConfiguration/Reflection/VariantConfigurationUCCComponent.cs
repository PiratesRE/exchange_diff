using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationUCCComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationUCCComponent() : base("UCC")
		{
			base.Add(new VariantConfigurationSection("UCC.settings.ini", "UCC", typeof(IFeature), false));
		}

		public VariantConfigurationSection UCC
		{
			get
			{
				return base["UCC"];
			}
		}
	}
}
