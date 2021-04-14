using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationPublisherManagerBuilder
	{
		public PushNotificationPublisherManagerBuilder(List<PushNotificationPlatform> platforms)
		{
			ArgumentValidator.ThrowIfNull("platforms", platforms);
			this.Platforms = platforms;
		}

		private List<PushNotificationPlatform> Platforms { get; set; }

		public PushNotificationPublisherManager Build(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager, AzureHubEventHandler hubEventHandler = null)
		{
			Dictionary<PushNotificationPlatform, PushNotificationPublisherFactory> dictionary = new Dictionary<PushNotificationPlatform, PushNotificationPublisherFactory>();
			foreach (PushNotificationPlatform pushNotificationPlatform in this.Platforms)
			{
				PushNotificationPublisherFactory pushNotificationPublisherFactory;
				switch (pushNotificationPlatform)
				{
				case PushNotificationPlatform.APNS:
					pushNotificationPublisherFactory = this.CreateApnsFactory(configuration, throttlingManager);
					break;
				case PushNotificationPlatform.PendingGet:
					pushNotificationPublisherFactory = this.CreatePendingGetFactory(configuration, throttlingManager);
					break;
				case PushNotificationPlatform.WNS:
					pushNotificationPublisherFactory = this.CreateWnsFactory(configuration, throttlingManager);
					break;
				case PushNotificationPlatform.Proxy:
					pushNotificationPublisherFactory = this.CreateProxyFactory(configuration, throttlingManager);
					break;
				case PushNotificationPlatform.GCM:
					pushNotificationPublisherFactory = this.CreateGcmFactory(configuration, throttlingManager);
					break;
				case PushNotificationPlatform.WebApp:
					pushNotificationPublisherFactory = this.CreateWebAppFactory(configuration, throttlingManager);
					break;
				case PushNotificationPlatform.Azure:
					pushNotificationPublisherFactory = this.CreateAzureFactory(configuration, throttlingManager, hubEventHandler);
					break;
				case PushNotificationPlatform.AzureHubCreation:
					pushNotificationPublisherFactory = this.CreateAzureHubFactory(configuration, throttlingManager, hubEventHandler);
					break;
				case PushNotificationPlatform.AzureChallengeRequest:
					pushNotificationPublisherFactory = this.CreateAzureChallengeRequestFactory(configuration, throttlingManager, hubEventHandler);
					break;
				case PushNotificationPlatform.AzureDeviceRegistration:
					pushNotificationPublisherFactory = this.CreateAzureDeviceRegistrationFactory(configuration, throttlingManager, hubEventHandler);
					break;
				default:
					throw new InvalidOperationException("Unrecognized platform: " + pushNotificationPlatform.ToString());
				}
				dictionary.Add(pushNotificationPublisherFactory.Platform, pushNotificationPublisherFactory);
			}
			PushNotificationPublisherManager pushNotificationPublisherManager = configuration.CreatePublisherManager(dictionary);
			if (hubEventHandler != null)
			{
				hubEventHandler.PublisherManager = pushNotificationPublisherManager;
			}
			return pushNotificationPublisherManager;
		}

		private PushNotificationPublisherFactory CreateApnsFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager)
		{
			RegistrySession registrySession = new RegistrySession(true);
			ApnsFeedbackManagerSettings settings = registrySession.Read<ApnsFeedbackManagerSettings>();
			ApnsFeedbackManager feedbackProvider = new ApnsFeedbackManager(settings);
			ApnsNotificationFactory item = new ApnsNotificationFactory();
			List<IPushNotificationMapping<ApnsNotification>> mappings = new List<IPushNotificationMapping<ApnsNotification>>
			{
				item
			};
			return new ApnsPublisherFactory(feedbackProvider, throttlingManager, mappings);
		}

		private PushNotificationPublisherFactory CreatePendingGetFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager)
		{
			PendingGetNotificationFactory item = new PendingGetNotificationFactory();
			List<IPushNotificationMapping<PendingGetNotification>> mappings = new List<IPushNotificationMapping<PendingGetNotification>>
			{
				item
			};
			IPendingGetConnectionCache instance = PendingGetConnectionCache.Instance;
			return new PendingGetPublisherFactory(instance, mappings);
		}

		private PushNotificationPublisherFactory CreateWnsFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager)
		{
			WnsNotificationFactory item = new WnsNotificationFactory();
			WnsOutlookNotificationMapping item2 = new WnsOutlookNotificationMapping();
			List<IPushNotificationMapping<WnsNotification>> mappings = new List<IPushNotificationMapping<WnsNotification>>
			{
				item,
				item2
			};
			return new WnsPublisherFactory(throttlingManager, mappings);
		}

		private PushNotificationPublisherFactory CreateGcmFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager)
		{
			GcmNotificationFactory item = new GcmNotificationFactory();
			List<IPushNotificationMapping<GcmNotification>> mappings = new List<IPushNotificationMapping<GcmNotification>>
			{
				item
			};
			return new GcmPublisherFactory(throttlingManager, mappings);
		}

		private PushNotificationPublisherFactory CreateWebAppFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager)
		{
			WebAppNotificationFactory item = new WebAppNotificationFactory();
			List<IPushNotificationMapping<WebAppNotification>> mappings = new List<IPushNotificationMapping<WebAppNotification>>
			{
				item
			};
			return new WebAppPublisherFactory(mappings);
		}

		private PushNotificationPublisherFactory CreateAzureFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager, AzureHubEventHandler hubEventHandler)
		{
			AzureNotificationFactory item = new AzureNotificationFactory();
			AzureOutlookNotificationMapping item2 = new AzureOutlookNotificationMapping();
			List<IPushNotificationMapping<AzureNotification>> mappings = new List<IPushNotificationMapping<AzureNotification>>
			{
				item,
				item2
			};
			return new AzurePublisherFactory(mappings);
		}

		private PushNotificationPublisherFactory CreateAzureHubFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager, AzureHubEventHandler hubEventHandler)
		{
			AzureHubDefinitionMapping item = new AzureHubDefinitionMapping(configuration);
			List<IPushNotificationMapping<AzureHubCreationNotification>> mappings = new List<IPushNotificationMapping<AzureHubCreationNotification>>
			{
				item
			};
			return new AzureHubCreationPublisherFactory(mappings);
		}

		private PushNotificationPublisherFactory CreateAzureChallengeRequestFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager, AzureHubEventHandler hubEventHandler)
		{
			AzureChallengeRequestInfoMapping item = new AzureChallengeRequestInfoMapping(configuration);
			List<IPushNotificationMapping<AzureChallengeRequestNotification>> mappings = new List<IPushNotificationMapping<AzureChallengeRequestNotification>>
			{
				item
			};
			return new AzureChallengeRequestPublisherFactory(mappings, hubEventHandler);
		}

		private PushNotificationPublisherFactory CreateAzureDeviceRegistrationFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager, AzureHubEventHandler hubEventHandler)
		{
			AzureDeviceRegistrationInfoMapping item = new AzureDeviceRegistrationInfoMapping(configuration);
			List<IPushNotificationMapping<AzureDeviceRegistrationNotification>> mappings = new List<IPushNotificationMapping<AzureDeviceRegistrationNotification>>
			{
				item
			};
			return new AzureDeviceRegistrationPublisherFactory(mappings, hubEventHandler);
		}

		private PushNotificationPublisherFactory CreateProxyFactory(PushNotificationPublisherConfiguration configuration, IThrottlingManager throttlingManager)
		{
			return new ProxyPublisherFactory();
		}
	}
}
