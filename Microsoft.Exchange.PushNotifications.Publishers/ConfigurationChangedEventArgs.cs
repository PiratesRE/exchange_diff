using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ConfigurationChangedEventArgs : EventArgs
	{
		public ConfigurationChangedEventArgs(PushNotificationPublisherConfiguration updatedConfiguration)
		{
			ArgumentValidator.ThrowIfNull("updatedConfiguration", updatedConfiguration);
			this.UpdatedConfiguration = updatedConfiguration;
		}

		public PushNotificationPublisherConfiguration UpdatedConfiguration { get; private set; }
	}
}
