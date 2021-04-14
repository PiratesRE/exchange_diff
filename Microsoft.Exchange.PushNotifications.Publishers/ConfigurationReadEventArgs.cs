using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ConfigurationReadEventArgs : EventArgs
	{
		public ConfigurationReadEventArgs(PushNotificationPublisherConfiguration configuration)
		{
			ArgumentValidator.ThrowIfNull("configuration", configuration);
			this.Configuration = configuration;
		}

		public PushNotificationPublisherConfiguration Configuration { get; private set; }
	}
}
