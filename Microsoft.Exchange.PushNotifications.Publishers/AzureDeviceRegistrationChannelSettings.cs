using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureDeviceRegistrationChannelSettings : PushNotificationSettingsBase
	{
		public AzureDeviceRegistrationChannelSettings(string appId, int requestTimeout, int requestStepTimeout, int maxDevicesRegisteredCacheSize, int backOffTimeInSeconds) : base(appId)
		{
			this.RequestTimeout = requestTimeout;
			this.RequestStepTimeout = requestStepTimeout;
			this.MaxDevicesRegistrationCacheSize = maxDevicesRegisteredCacheSize;
			this.BackOffTimeInSeconds = backOffTimeInSeconds;
		}

		public int RequestTimeout { get; private set; }

		public int RequestStepTimeout { get; private set; }

		public int MaxDevicesRegistrationCacheSize { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			if (!PushNotificationCannedApp.AzureDeviceRegistration.Name.Equals(base.AppId))
			{
				errors.Add(Strings.ValidationErrorDeviceRegistrationAppId(base.AppId, PushNotificationCannedApp.AzureDeviceRegistration.Name));
			}
			if (this.RequestTimeout < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("RequestTimeout", this.RequestTimeout));
			}
			if (this.RequestStepTimeout < 0 || this.RequestStepTimeout > this.RequestTimeout)
			{
				errors.Add(Strings.ValidationErrorRangeInteger("RequestStepTimeout", 0, this.RequestTimeout, this.RequestStepTimeout));
			}
			if (this.MaxDevicesRegistrationCacheSize < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("MaxDevicesRegistrationCacheSize", this.MaxDevicesRegistrationCacheSize));
			}
			if (this.BackOffTimeInSeconds < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("BackOffTimeInSeconds", this.BackOffTimeInSeconds));
			}
		}

		protected override bool RunSuitabilityCheck()
		{
			return base.RunSuitabilityCheck();
		}

		public const int DefaultRequestTimeout = 60000;

		public const int DefaultRequestStepTimeout = 500;

		public const int DefaultBackOffTimeInSeconds = 600;

		public const int DefaultDevicesRegistrationCacheSize = 10000;
	}
}
