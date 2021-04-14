using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class ProxyPublisherSettings : PushNotificationPublisherSettings
	{
		public ProxyPublisherSettings(string appId, bool enabled, string hubName, Version minimumVersion, Version maximumVersion, int queueSize, int numberOfChannels, int addTimeout, ProxyChannelSettings channelSettings) : base(appId, enabled, minimumVersion, maximumVersion, queueSize, numberOfChannels, addTimeout)
		{
			ArgumentValidator.ThrowIfNull("channelSettings", channelSettings);
			this.HubName = hubName;
			this.ChannelSettings = channelSettings;
		}

		public string HubName { get; private set; }

		public ProxyChannelSettings ChannelSettings { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (!this.ChannelSettings.IsValid)
			{
				errors.AddRange(this.ChannelSettings.ValidationErrors);
			}
		}

		protected override bool RunSuitabilityCheck()
		{
			bool flag = base.RunSuitabilityCheck();
			return this.ChannelSettings.IsSuitable && flag;
		}

		public const bool DefaultEnabled = false;

		public const Version DefaultMinimumVersion = null;

		public const Version DefaultMaximumVersion = null;

		public const int DefaultQueueSize = 10000;

		public const int DefaultNumberOfChannels = 1;

		public const int DefaultAddTimeout = 15;
	}
}
