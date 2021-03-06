using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WebAppChannelSettings : PushNotificationSettingsBase
	{
		public WebAppChannelSettings(string appId, int requestTimeout, int requestStepTimeout, int backOffTimeInSeconds) : base(appId)
		{
			this.RequestTimeout = requestTimeout;
			this.RequestStepTimeout = requestStepTimeout;
			this.BackOffTimeInSeconds = backOffTimeInSeconds;
		}

		public int RequestTimeout { get; private set; }

		public int RequestStepTimeout { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (this.RequestTimeout < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("RequestTimeout", this.RequestTimeout));
			}
			if (this.RequestStepTimeout < 0 || this.RequestStepTimeout > this.RequestTimeout)
			{
				errors.Add(Strings.ValidationErrorRangeInteger("RequestStepTimeout", 0, this.RequestTimeout, this.RequestStepTimeout));
			}
			if (this.BackOffTimeInSeconds < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("BackOffTimeInSeconds", this.BackOffTimeInSeconds));
			}
		}

		public const int DefaultRequestTimeout = 60000;

		public const int DefaultRequestStepTimeout = 500;

		public const int DefaultBackOffTimeInSeconds = 300;
	}
}
