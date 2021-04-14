using System;
using Microsoft.Exchange.TextProcessing.Boomerang;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationBoomerangComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationBoomerangComponent() : base("Boomerang")
		{
			base.Add(new VariantConfigurationSection("Boomerang.settings.ini", "BoomerangSettings", typeof(IBoomerangSettings), false));
			base.Add(new VariantConfigurationSection("Boomerang.settings.ini", "BoomerangMessageId", typeof(IFeature), false));
		}

		public VariantConfigurationSection BoomerangSettings
		{
			get
			{
				return base["BoomerangSettings"];
			}
		}

		public VariantConfigurationSection BoomerangMessageId
		{
			get
			{
				return base["BoomerangMessageId"];
			}
		}
	}
}
