using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class RegistryConfigurationProvider : IPushNotificationPublisherConfigurationProvider
	{
		public IEnumerable<IPushNotificationRawSettings> LoadSettings(bool ignoreErrors = true)
		{
			RegistrySession registrySession = new RegistrySession(ignoreErrors);
			return registrySession.Find<RegistryPushNotificationApp>(RegistryPushNotificationApp.Schema.RootFolder);
		}
	}
}
