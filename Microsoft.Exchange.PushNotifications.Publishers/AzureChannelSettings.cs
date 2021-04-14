using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal sealed class AzureChannelSettings : PushNotificationSettingsBase
	{
		public AzureChannelSettings(string appId, string serviceUriTemplate, string azureKeyName, string azureKey, bool isAzureKeyEncrypted, string registrationTemplate, bool isRegistrationEnabled, string partitionName, int requestTimeout, int requestStepTimeout, int maxDevicesRegisteredCacheSize, int backOffTimeInSeconds) : base(appId)
		{
			this.serviceUriTemplate = serviceUriTemplate;
			this.azureKeyName = azureKeyName;
			this.azureStringKey = azureKey;
			this.isAzureKeyEncrypted = isAzureKeyEncrypted;
			this.RegistrationTemplate = registrationTemplate;
			this.IsRegistrationEnabled = isRegistrationEnabled;
			this.PartitionName = partitionName;
			this.RequestTimeout = requestTimeout;
			this.RequestStepTimeout = requestStepTimeout;
			this.MaxDevicesRegistrationCacheSize = maxDevicesRegisteredCacheSize;
			this.BackOffTimeInSeconds = backOffTimeInSeconds;
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

		public IAzureSasTokenProvider AzureSasTokenProvider
		{
			get
			{
				if (!base.IsSuitable)
				{
					throw new InvalidOperationException("AzureKey can only be accessed if the instance is suitable");
				}
				return this.azureTokenProvider;
			}
		}

		public string RegistrationTemplate { get; private set; }

		public bool IsRegistrationEnabled { get; private set; }

		public string PartitionName { get; private set; }

		public int RequestTimeout { get; private set; }

		public int RequestStepTimeout { get; private set; }

		public int MaxDevicesRegistrationCacheSize { get; private set; }

		public int BackOffTimeInSeconds { get; private set; }

		protected override void RunValidationCheck(List<LocalizedString> errors)
		{
			if (string.IsNullOrWhiteSpace(this.serviceUriTemplate))
			{
				errors.Add(Strings.ValidationErrorEmptyString("ServiceUriTemplate"));
			}
			else
			{
				try
				{
					this.uriTemplate = AzureUriTemplate.CreateUriTemplate(this.serviceUriTemplate, this.PartitionName);
				}
				catch (ArgumentException ex)
				{
					errors.Add(Strings.ValidationErrorInvalidUri("ServiceUriTemplate", this.serviceUriTemplate, ex.Message));
				}
			}
			if (string.IsNullOrWhiteSpace(this.azureKeyName))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AzureKeyName"));
			}
			if (string.IsNullOrWhiteSpace(this.azureStringKey))
			{
				errors.Add(Strings.ValidationErrorEmptyString("AzureKeyValue"));
			}
			if (this.IsRegistrationEnabled && string.IsNullOrWhiteSpace(this.RegistrationTemplate))
			{
				errors.Add(Strings.ValidationErrorEmptyString("RegistrationTemplate"));
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
				SecureString secureString = this.isAzureKeyEncrypted ? PushNotificationDataProtector.Default.Decrypt(this.azureStringKey) : this.azureStringKey.AsSecureString();
				if (secureString == null)
				{
					throw new PushNotificationConfigurationException(Strings.ValidationErrorEmptyString("AzureKey"));
				}
				if (secureString.AsUnsecureString().Contains("{"))
				{
					try
					{
						AzureSasToken azureSasToken = JsonConverter.Deserialize<AzureSasToken>(secureString.AsUnsecureString(), null);
						if (azureSasToken == null || !azureSasToken.IsValid())
						{
							throw new PushNotificationConfigurationException(Strings.ValidationErrorInvalidSasToken(azureSasToken.ToNullableString(null)));
						}
						this.azureTokenProvider = azureSasToken;
						goto IL_AD;
					}
					catch (SerializationException)
					{
						throw new PushNotificationConfigurationException(Strings.ValidationErrorInvalidAuthenticationKey);
					}
				}
				this.azureTokenProvider = new AzureSasKey(this.azureKeyName, secureString, null);
				IL_AD:;
			}
			catch (PushNotificationConfigurationException exception)
			{
				string text = exception.ToTraceString();
				PushNotificationsCrimsonEvents.PushNotificationPublisherConfigurationError.Log<string, string, string>(base.AppId, string.Empty, text);
				ExTraceGlobals.PublisherManagerTracer.TraceError<string, string>((long)this.GetHashCode(), "[AzureChannelSettings:RunSuitabilityCheck] Channel configuration for '{0}' has suitability errors: {1}", base.AppId, text);
				result = false;
			}
			return result;
		}

		public const int DefaultRequestTimeout = 60000;

		public const int DefaultRequestStepTimeout = 500;

		public const int DefaultBackOffTimeInSeconds = 600;

		public const bool DefaultIsSasKeyEncrypted = true;

		public const bool DefaultIsRegistrationEnabled = false;

		public const string DefaultUriTemplate = "https://{0}-{1}.servicebus.windows.net/exo/{2}/{3}";

		public const int DefaultDevicesRegistrationCacheSize = 10000;

		public const bool DefaultIsDefaultPartitionName = false;

		private readonly string serviceUriTemplate;

		private readonly string azureKeyName;

		private readonly string azureStringKey;

		private readonly bool isAzureKeyEncrypted;

		private AzureUriTemplate uriTemplate;

		private IAzureSasTokenProvider azureTokenProvider;
	}
}
