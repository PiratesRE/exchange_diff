using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationSharedCacheComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationSharedCacheComponent() : base("SharedCache")
		{
			base.Add(new VariantConfigurationSection("SharedCache.settings.ini", "UsePersistenceForCafe", typeof(IFeature), false));
		}

		public VariantConfigurationSection UsePersistenceForCafe
		{
			get
			{
				return base["UsePersistenceForCafe"];
			}
		}
	}
}
