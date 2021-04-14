using System;

namespace Microsoft.Exchange.Diagnostics
{
	public struct PushNotificationsTags
	{
		public const int ExchangeToOwa = 0;

		public const int NotificationFormat = 1;

		public const int PublisherManager = 2;

		public const int StorageNotificationSubscription = 3;

		public const int PushNotificationAssistant = 4;

		public const int ApnsPublisher = 5;

		public const int WnsPublisher = 6;

		public const int GcmPublisher = 7;

		public const int ProxyPublisher = 8;

		public const int PendingGetPublisher = 9;

		public const int PushNotificationService = 10;

		public const int PushNotificationClient = 11;

		public const int WebAppPublisher = 12;

		public const int AzurePublisher = 13;

		public const int AzureHubCreationPublisher = 14;

		public const int AzureChallengeRequestPublisher = 15;

		public const int AzureDeviceRegistrationPublisher = 16;

		public static Guid guid = new Guid("5af2f275-ee7b-466b-8ba6-b317da1f7800");
	}
}
