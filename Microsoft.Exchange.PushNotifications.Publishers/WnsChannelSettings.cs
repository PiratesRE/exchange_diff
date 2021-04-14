using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class WnsChannelSettings : PushNotificationSettingsBase
	{
		public WnsChannelSettings(string appId, string appSid, string appSecret, bool isAppSecretEncrypted, string authenticationUri, int requestTimeout, int requestStepTimeout, int authenticateRetryDelay, int authenticateRetryMax, int backOffTimeInSeconds) : base(appId)
		{
			this.AppSid = appSid;
			this.appSecretString = appSecret;
			this.authenticationUriString = authenticationUri;
			this.isAppSecretEncrypted = isAppSecretEncrypted;
			this.RequestTimeout = requestTimeout;
			this.RequestStepTimeout = requestStepTimeout;
			this.AuthenticateRetryDelay = authenticateRetryDelay;
			this.AuthenticateRetryMax = authenticateRetryMax;
			this.BackOffTimeInSeconds = backOffTimeInSeconds;
		}

		public string AppSid { get; private set; }

		public SecureString AppSecret
		{
			get
			{
				if (!base.IsSuitable)
				{
					throw new InvalidOperationException("AppSecret can only be accessed if the instance is suitable");
				}
				return this.appSecret;
			}
		}

		public Uri AuthenticationUri
		{
			get
			{
				if (!base.IsValid)
				{
					throw new InvalidOperationException("AuthenticationUri can only be accessed if the instance is valid");
				}
				return this.authenticationUri;
			}
		}

		public int RequestTimeout { get; private set; }

		public int RequestStepTimeout { get; private set; }

		public int AuthenticateRetryDelay { get; private set; }

		public int AuthenticateRetryMax { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			base.RunValidationCheck(errors);
			if (string.IsNullOrWhiteSpace(this.AppSid))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AppSid"));
			}
			if (string.IsNullOrWhiteSpace(this.authenticationUriString))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AuthenticationUri"));
			}
			else
			{
				try
				{
					this.authenticationUri = new Uri(this.authenticationUriString, UriKind.Absolute);
				}
				catch (UriFormatException ex)
				{
					errors.Add(Strings.ValidationErrorInvalidUri("AuthenticationUri", this.authenticationUriString, ex.Message));
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
			if (this.AuthenticateRetryDelay < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("AuthenticateRetryDelay", this.AuthenticateRetryDelay));
			}
			if (this.AuthenticateRetryMax < 0)
			{
				errors.Add(Strings.ValidationErrorNonNegativeInteger("AuthenticateRetryMax", this.AuthenticateRetryMax));
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
				this.appSecret = (this.isAppSecretEncrypted ? PushNotificationDataProtector.Default.Decrypt(this.appSecretString) : this.appSecretString.AsSecureString());
				if (this.appSecret == null)
				{
					throw new PushNotificationConfigurationException(Strings.ValidationErrorEmptyString("AppSecret"));
				}
			}
			catch (PushNotificationConfigurationException exception)
			{
				string text = exception.ToTraceString();
				PushNotificationsCrimsonEvents.PushNotificationPublisherConfigurationError.Log<string, string, string>(base.AppId, string.Empty, text);
				ExTraceGlobals.PublisherManagerTracer.TraceError<string, string>((long)this.GetHashCode(), "[WnsChannelSettings:RunSuitabilityCheck] Channel configuration for '{0}' has suitability errors: {1}", base.AppId, text);
				result = false;
			}
			return result;
		}

		public const string DefaultAuthenticationUri = "https://login.live.com/accesstoken.srf";

		public const bool DefaultIsAppSecretEncrypted = true;

		public const int DefaultRequestTimeout = 60000;

		public const int DefaultRequestStepTimeout = 500;

		public const int DefaultAuthenticateRetryDelay = 1500;

		public const int DefaultAuthenticateRetryMax = 2;

		public const int DefaultBackOffTimeInSeconds = 600;

		private readonly string authenticationUriString;

		private readonly string appSecretString;

		private readonly bool isAppSecretEncrypted;

		private Uri authenticationUri;

		private SecureString appSecret;
	}
}
