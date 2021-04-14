using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class ProxyChannelSettings : PushNotificationSettingsBase
	{
		public ProxyChannelSettings(string appId, string serviceUri, string organization, DateTime? lastUpdated, int publishRetryMax, int publishRetryDelay, int publishStepTimeout, int backOffTime) : base(appId)
		{
			this.serviceUriString = serviceUri;
			this.LastUpdated = lastUpdated;
			this.Organization = organization;
			this.PublishRetryMax = publishRetryMax;
			this.PublishRetryDelay = publishRetryDelay;
			this.PublishStepTimeout = publishStepTimeout;
			this.BackOffTimeInSeconds = backOffTime;
		}

		public Uri ServiceUri
		{
			get
			{
				if (!base.IsSuitable)
				{
					throw new InvalidOperationException("ServiceUri can only be accessed if the instance is suitable");
				}
				return this.serviceUri;
			}
		}

		public DateTime? LastUpdated { get; private set; }

		public string Organization { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		public int PublishRetryMax { get; private set; }

		public int PublishRetryDelay { get; private set; }

		public int PublishStepTimeout { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			if (string.IsNullOrWhiteSpace(this.serviceUriString))
			{
				errors.Add(Strings.ValidationErrorEmptyString("ServiceUri"));
			}
			else
			{
				try
				{
					this.serviceUri = new Uri(this.serviceUriString, UriKind.Absolute);
				}
				catch (UriFormatException ex)
				{
					errors.Add(Strings.ValidationErrorInvalidUri("ServiceUri", this.serviceUriString, ex.Message));
				}
			}
			if (string.IsNullOrWhiteSpace(this.Organization))
			{
				errors.Add(Strings.ValidationErrorEmptyString("Organization"));
			}
			else if (!SmtpAddress.IsValidDomain(this.Organization))
			{
				errors.Add(DataStrings.InvalidSmtpDomainName(this.Organization));
			}
			if (this.PublishRetryMax < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("PublishRetryMax", this.PublishRetryMax));
			}
			if (this.PublishRetryDelay < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("PublishRetryDelay", this.PublishRetryDelay));
			}
			if (this.PublishStepTimeout < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("PublishStepTimeout", this.PublishStepTimeout));
			}
			if (this.BackOffTimeInSeconds < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("BackOffTimeInSeconds", this.BackOffTimeInSeconds));
			}
		}

		public const int DefaultPublishRetryMax = 3;

		public const int DefaultPublishRetryDelay = 1500;

		public const int DefaultBackOffTimeInSeconds = 600;

		public const int DefaultPublishStepTimeout = 500;

		private readonly string serviceUriString;

		private Uri serviceUri;
	}
}
