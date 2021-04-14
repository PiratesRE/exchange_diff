using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal interface IPushNotificationPublisherConfigurationProvider
	{
		IEnumerable<IPushNotificationRawSettings> LoadSettings(bool ignoreErrors = true);
	}
}
