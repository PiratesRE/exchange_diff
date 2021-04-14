using System;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Microsoft.Exchange.PushNotifications.Client
{
	internal class PushNotificationsProxyPoolFactory
	{
		public static PushNotificationsProxyPool<IPublisherServiceContract> CreatePublisherProxyPool()
		{
			ChannelFactory<IPublisherServiceContract> channelFactory = new ChannelFactory<IPublisherServiceContract>(PushNotificationsProxyPoolFactory.namedPipeBinding, new EndpointAddress("net.pipe://localhost/PushNotifications/service.svc"));
			return new PushNotificationsProxyPool<IPublisherServiceContract>("PushNotifications-Publishing", Environment.MachineName, channelFactory, true);
		}

		public static PushNotificationsProxyPool<IOutlookPublisherServiceContract> CreateOutlookPublisherProxyPool()
		{
			ChannelFactory<IOutlookPublisherServiceContract> channelFactory = new ChannelFactory<IOutlookPublisherServiceContract>(PushNotificationsProxyPoolFactory.namedPipeBinding, new EndpointAddress("net.pipe://localhost/PushNotifications/service.svc"));
			return new PushNotificationsProxyPool<IOutlookPublisherServiceContract>("Outlook-PushNotifications-Publishing", Environment.MachineName, channelFactory, true);
		}

		public static PushNotificationsProxyPool<IAzureHubCreationServiceContract> CreateAzureHubCreationProxyPool()
		{
			ChannelFactory<IAzureHubCreationServiceContract> channelFactory = new ChannelFactory<IAzureHubCreationServiceContract>(PushNotificationsProxyPoolFactory.namedPipeBinding, new EndpointAddress("net.pipe://localhost/PushNotifications/service.svc"));
			return new PushNotificationsProxyPool<IAzureHubCreationServiceContract>("Azure-Hub-Creation", Environment.MachineName, channelFactory, true);
		}

		public static PushNotificationsProxyPool<IAzureChallengeRequestServiceContract> CreateAzureChallengeRequestProxyPool()
		{
			ChannelFactory<IAzureChallengeRequestServiceContract> channelFactory = new ChannelFactory<IAzureChallengeRequestServiceContract>(PushNotificationsProxyPoolFactory.namedPipeBinding, new EndpointAddress("net.pipe://localhost/PushNotifications/service.svc"));
			return new PushNotificationsProxyPool<IAzureChallengeRequestServiceContract>("Azure-Challenge-Request", Environment.MachineName, channelFactory, true);
		}

		public static PushNotificationsProxyPool<IAzureDeviceRegistrationServiceContract> CreateAzureDeviceRegistrationProxyPool()
		{
			ChannelFactory<IAzureDeviceRegistrationServiceContract> channelFactory = new ChannelFactory<IAzureDeviceRegistrationServiceContract>(PushNotificationsProxyPoolFactory.namedPipeBinding, new EndpointAddress("net.pipe://localhost/PushNotifications/service.svc"));
			return new PushNotificationsProxyPool<IAzureDeviceRegistrationServiceContract>("Azure-Device-Registration", Environment.MachineName, channelFactory, true);
		}

		public static PushNotificationsProxyPool<ILocalUserNotificationPublisherServiceContract> CreateLocalUserNotificationPublisherProxyPool()
		{
			ChannelFactory<ILocalUserNotificationPublisherServiceContract> channelFactory = new ChannelFactory<ILocalUserNotificationPublisherServiceContract>(PushNotificationsProxyPoolFactory.namedPipeBinding, new EndpointAddress("net.pipe://localhost/PushNotifications/service.svc"));
			return new PushNotificationsProxyPool<ILocalUserNotificationPublisherServiceContract>("LocalUserNotification-Publishing", Environment.MachineName, channelFactory, true);
		}

		public const string PublishingEndpointName = "PushNotifications-Publishing";

		public const string OutlookPublishingEndpointName = "Outlook-PushNotifications-Publishing";

		public const string AzureHubCreationEndpointName = "Azure-Hub-Creation";

		public const string AzureChallengeRequestEndpointName = "Azure-Challenge-Request";

		public const string AzureDeviceRegistrationEndpointName = "Azure-Device-Registration";

		public const string LocalUserNotificationPublishingEndpointName = "LocalUserNotification-Publishing";

		private static Binding namedPipeBinding = new NetNamedPipeBinding();
	}
}
