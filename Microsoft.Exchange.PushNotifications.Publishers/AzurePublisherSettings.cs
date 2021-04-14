using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class AzurePublisherSettings : PushNotificationPublisherSettings
	{
		public AzurePublisherSettings(string appId, bool enabled, Version minimumVersion, Version maximumVersion, int queueSize, int numberOfChannels, int addTimeout, bool isMultifactorRegistrationEnabled, AzureChannelSettings channelSettings) : base(appId, enabled, minimumVersion, maximumVersion, queueSize, numberOfChannels, addTimeout)
		{
			ArgumentValidator.ThrowIfNull("channelSettings", channelSettings);
			this.ChannelSettings = channelSettings;
			this.IsMultifactorRegistrationEnabled = isMultifactorRegistrationEnabled;
		}

		public AzureChannelSettings ChannelSettings { get; private set; }

		public bool IsMultifactorRegistrationEnabled { get; private set; }

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

		public const bool DefaultEnabled = true;

		public const Version DefaultMinimumVersion = null;

		public const Version DefaultMaximumVersion = null;

		public const int DefaultQueueSize = 10000;

		public const int DefaultNumberOfChannels = 1;

		public const int DefaultAddTimeout = 15;

		public const bool DefaultMultifactorRegistrationEnabled = false;
	}
}
