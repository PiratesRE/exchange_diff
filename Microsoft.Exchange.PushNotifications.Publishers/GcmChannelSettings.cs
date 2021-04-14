using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class GcmChannelSettings : PushNotificationSettingsBase
	{
		public GcmChannelSettings(string appId, string senderId, string senderAuthToken, bool isAuthTokenEncrypted, string serviceUri, int requestTimeout, int requestStepTimeout, int backOffTimeInSeconds) : base(appId)
		{
			this.SenderId = senderId;
			this.authTokenString = senderAuthToken;
			this.serviceUriString = serviceUri;
			this.isAuthTokenEncrypted = isAuthTokenEncrypted;
			this.RequestTimeout = requestTimeout;
			this.RequestStepTimeout = requestStepTimeout;
			this.BackOffTimeInSeconds = backOffTimeInSeconds;
		}

		public string SenderId { get; private set; }

		public SecureString SenderAuthToken
		{
			get
			{
				if (!base.IsSuitable)
				{
					throw new InvalidOperationException("SenderAuthToken can only be accessed if the instance is suitable");
				}
				return this.authToken;
			}
		}

		public Uri ServiceUri
		{
			get
			{
				if (!base.IsValid)
				{
					throw new InvalidOperationException("ServiceUri can only be accessed if the instance is valid");
				}
				return this.serviceUri;
			}
		}

		public int RequestTimeout { get; private set; }

		public int RequestStepTimeout { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (string.IsNullOrWhiteSpace(this.SenderId))
			{
				errors.Add(Strings.ValidationErrorEmptyString("SenderId"));
			}
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

		protected override bool RunSuitabilityCheck()
		{
			bool result = base.RunSuitabilityCheck();
			try
			{
				this.authToken = (this.isAuthTokenEncrypted ? PushNotificationDataProtector.Default.Decrypt(this.authTokenString) : this.authTokenString.AsSecureString());
				if (this.authToken == null)
				{
					throw new PushNotificationConfigurationException(Strings.ValidationErrorEmptyString("SenderAuthToken"));
				}
			}
			catch (PushNotificationConfigurationException exception)
			{
				string text = exception.ToTraceString();
				PushNotificationsCrimsonEvents.PushNotificationPublisherConfigurationError.Log<string, string, string>(base.AppId, string.Empty, text);
				ExTraceGlobals.PublisherManagerTracer.TraceError<string, string>((long)this.GetHashCode(), "[GcmChannelSettings:RunSuitabilityCheck] Channel configuration for '{0}' has suitability errors: {1}", base.AppId, text);
				result = false;
			}
			return result;
		}

		public const string DefaultServiceUri = "https://android.googleapis.com/gcm/send";

		public const bool DefaultIsAuthTokenEncrypted = true;

		public const int DefaultRequestTimeout = 60000;

		public const int DefaultRequestStepTimeout = 500;

		public const int DefaultBackOffTimeInSeconds = 600;

		private readonly string serviceUriString;

		private readonly string authTokenString;

		private readonly bool isAuthTokenEncrypted;

		private Uri serviceUri;

		private SecureString authToken;
	}
}
