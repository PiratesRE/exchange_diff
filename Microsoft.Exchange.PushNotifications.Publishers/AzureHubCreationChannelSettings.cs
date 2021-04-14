using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureHubCreationChannelSettings : PushNotificationSettingsBase
	{
		public AzureHubCreationChannelSettings(string appId, string serviceAcsUriTemplate, string serviceScopeUriTemplate, string serviceUriTemplate, string acsUserName, string acsUserPassword, bool isAzureKeyEncrypted, int requestTimeout, int requestStepTimeout, int authenticateRetryDelay, int maxHubCacheSize, int backOffTimeInSeconds) : base(appId)
		{
			this.acsServiceUriTemplate = serviceAcsUriTemplate;
			this.serviceScopeUriTemplate = serviceScopeUriTemplate;
			this.serviceUriTemplate = serviceUriTemplate;
			this.AcsUserName = acsUserName;
			this.acsUserPasswordString = acsUserPassword;
			this.isPasswordEncrypted = isAzureKeyEncrypted;
			this.RequestTimeout = requestTimeout;
			this.RequestStepTimeout = requestStepTimeout;
			this.AuthenticateRetryDelay = authenticateRetryDelay;
			this.MaxHubCreatedCacheSize = maxHubCacheSize;
			this.BackOffTimeInSeconds = backOffTimeInSeconds;
		}

		public AcsUriTemplate AcsUriTemplate
		{
			get
			{
				if (!base.IsSuitable)
				{
					throw new InvalidOperationException("UriTemplate can only be accessed if the instance is suitable");
				}
				return this.acsUriTemplate;
			}
		}

		public AzureUriTemplate UriTemplate
		{
			get
			{
				if (!base.IsSuitable)
				{
					throw new InvalidOperationException("UriTemplate can only be accessed if the instance is suitable");
				}
				return this.uriTemplate;
			}
		}

		public string AcsUserName { get; private set; }

		public SecureString AcsUserPassword
		{
			get
			{
				if (!base.IsSuitable)
				{
					throw new InvalidOperationException("AcsUserPassword can only be accessed if the instance is suitable");
				}
				return this.acsUserPassword;
			}
		}

		public int RequestTimeout { get; private set; }

		public int RequestStepTimeout { get; private set; }

		public int AuthenticateRetryDelay { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		public int MaxHubCreatedCacheSize { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			if (!PushNotificationCannedApp.AzureHubCreation.Name.Equals(base.AppId))
			{
				errors.Add(Strings.ValidationErrorHubCreationAppId(base.AppId, PushNotificationCannedApp.AzureHubCreation.Name));
			}
			if (string.IsNullOrWhiteSpace(this.acsServiceUriTemplate))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AcsServiceUriTemplate"));
			}
			else if (string.IsNullOrWhiteSpace(this.serviceScopeUriTemplate))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AcsServiceScopeUriTemplate"));
			}
			else
			{
				try
				{
					this.acsUriTemplate = new AcsUriTemplate(this.acsServiceUriTemplate, this.serviceScopeUriTemplate);
				}
				catch (ArgumentException ex)
				{
					errors.Add(Strings.ValidationErrorInvalidUri("ServiceAcsUriTemplate", this.acsServiceUriTemplate, ex.Message));
				}
			}
			if (string.IsNullOrWhiteSpace(this.serviceUriTemplate))
			{
				errors.Add(Strings.ValidationErrorEmptyString("ServiceUriTemplate"));
			}
			else
			{
				try
				{
					this.uriTemplate = AzureUriTemplate.CreateUriTemplate(this.serviceUriTemplate, null);
				}
				catch (ArgumentException ex2)
				{
					errors.Add(Strings.ValidationErrorInvalidUri("ServiceUriTemplate", this.serviceUriTemplate, ex2.Message));
				}
			}
			if (string.IsNullOrWhiteSpace(this.AcsUserName))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AcsKeyName"));
			}
			if (string.IsNullOrWhiteSpace(this.acsUserPasswordString))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AcsKeyValue"));
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
				SecureString secureString = this.isPasswordEncrypted ? PushNotificationDataProtector.Default.Decrypt(this.acsUserPasswordString) : this.acsUserPasswordString.AsSecureString();
				if (secureString == null)
				{
					throw new PushNotificationConfigurationException(Strings.ValidationErrorEmptyString("AcsUserPassword"));
				}
				this.acsUserPassword = secureString;
			}
			catch (PushNotificationConfigurationException exception)
			{
				string text = exception.ToTraceString();
				PushNotificationsCrimsonEvents.PushNotificationPublisherConfigurationError.Log<string, string, string>(base.AppId, string.Empty, text);
				ExTraceGlobals.PublisherManagerTracer.TraceError<string, string>((long)this.GetHashCode(), "[AzureHubCreationChannelSettings:RunSuitabilityCheck] Channel configuration for '{0}' has suitability errors: {1}", base.AppId, text);
				result = false;
			}
			return result;
		}

		public const int DefaultRequestTimeout = 60000;

		public const int DefaultRequestStepTimeout = 500;

		public const int DefaultBackOffTimeInSeconds = 600;

		public const int DefaultHubCacheSize = 10000;

		public const int DefaultAuthenticateRetryDelay = 1500;

		public const bool DefaultIsAcsPasswordEncrypted = true;

		public const string DefaultUriTemplate = "https://{0}-{1}.servicebus.windows.net/exo/{2}{3}";

		public const string DefaultAcsUriTemplate = "https://{0}-{1}-sb.accesscontrol.windows.net/";

		public const string DefaultAcsScopeUriTemplate = "http://{0}-{1}.servicebus.windows.net/exo/";

		private readonly string acsServiceUriTemplate;

		private readonly string serviceScopeUriTemplate;

		private readonly string serviceUriTemplate;

		private readonly string acsUserPasswordString;

		private readonly bool isPasswordEncrypted;

		private AcsUriTemplate acsUriTemplate;

		private AzureUriTemplate uriTemplate;

		private SecureString acsUserPassword;
	}
}
