using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationSharedMailboxComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationSharedMailboxComponent() : base("SharedMailbox")
		{
			base.Add(new VariantConfigurationSection("SharedMailbox.settings.ini", "SharedMailboxSentItemCopy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("SharedMailbox.settings.ini", "SharedMailboxSentItemsRoutingAgent", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("SharedMailbox.settings.ini", "SharedMailboxSentItemsDeliveryAgent", typeof(IFeature), false));
		}

		public VariantConfigurationSection SharedMailboxSentItemCopy
		{
			get
			{
				return base["SharedMailboxSentItemCopy"];
			}
		}

		public VariantConfigurationSection SharedMailboxSentItemsRoutingAgent
		{
			get
			{
				return base["SharedMailboxSentItemsRoutingAgent"];
			}
		}

		public VariantConfigurationSection SharedMailboxSentItemsDeliveryAgent
		{
			get
			{
				return base["SharedMailboxSentItemsDeliveryAgent"];
			}
		}
	}
}
