using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationMailboxPlansComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationMailboxPlansComponent() : base("MailboxPlans")
		{
			base.Add(new VariantConfigurationSection("MailboxPlans.settings.ini", "CloneLimitedSetOfMailboxPlanProperties", typeof(IFeature), false));
		}

		public VariantConfigurationSection CloneLimitedSetOfMailboxPlanProperties
		{
			get
			{
				return base["CloneLimitedSetOfMailboxPlanProperties"];
			}
		}
	}
}
