using System;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationNotificationBrokerServiceComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationNotificationBrokerServiceComponent() : base("NotificationBrokerService")
		{
			base.Add(new VariantConfigurationSection("NotificationBrokerService.settings.ini", "Service", typeof(IFeature), false));
		}

		public VariantConfigurationSection Service
		{
			get
			{
				return base["Service"];
			}
		}
	}
}
