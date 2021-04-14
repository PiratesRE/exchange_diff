using System;

namespace Microsoft.Exchange.Diagnostics.Components.PushNotifications
{
	public static class ExTraceGlobals
	{
		public static Trace ExchangeToOwaTracer
		{
			get
			{
				if (ExTraceGlobals.exchangeToOwaTracer == null)
				{
					ExTraceGlobals.exchangeToOwaTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.exchangeToOwaTracer;
			}
		}

		public static Trace NotificationFormatTracer
		{
			get
			{
				if (ExTraceGlobals.notificationFormatTracer == null)
				{
					ExTraceGlobals.notificationFormatTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.notificationFormatTracer;
			}
		}

		public static Trace PublisherManagerTracer
		{
			get
			{
				if (ExTraceGlobals.publisherManagerTracer == null)
				{
					ExTraceGlobals.publisherManagerTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.publisherManagerTracer;
			}
		}

		public static Trace StorageNotificationSubscriptionTracer
		{
			get
			{
				if (ExTraceGlobals.storageNotificationSubscriptionTracer == null)
				{
					ExTraceGlobals.storageNotificationSubscriptionTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.storageNotificationSubscriptionTracer;
			}
		}

		public static Trace PushNotificationAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.pushNotificationAssistantTracer == null)
				{
					ExTraceGlobals.pushNotificationAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.pushNotificationAssistantTracer;
			}
		}

		public static Trace ApnsPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.apnsPublisherTracer == null)
				{
					ExTraceGlobals.apnsPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.apnsPublisherTracer;
			}
		}

		public static Trace WnsPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.wnsPublisherTracer == null)
				{
					ExTraceGlobals.wnsPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.wnsPublisherTracer;
			}
		}

		public static Trace GcmPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.gcmPublisherTracer == null)
				{
					ExTraceGlobals.gcmPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.gcmPublisherTracer;
			}
		}

		public static Trace ProxyPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.proxyPublisherTracer == null)
				{
					ExTraceGlobals.proxyPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.proxyPublisherTracer;
			}
		}

		public static Trace PendingGetPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.pendingGetPublisherTracer == null)
				{
					ExTraceGlobals.pendingGetPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.pendingGetPublisherTracer;
			}
		}

		public static Trace PushNotificationServiceTracer
		{
			get
			{
				if (ExTraceGlobals.pushNotificationServiceTracer == null)
				{
					ExTraceGlobals.pushNotificationServiceTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.pushNotificationServiceTracer;
			}
		}

		public static Trace PushNotificationClientTracer
		{
			get
			{
				if (ExTraceGlobals.pushNotificationClientTracer == null)
				{
					ExTraceGlobals.pushNotificationClientTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.pushNotificationClientTracer;
			}
		}

		public static Trace WebAppPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.webAppPublisherTracer == null)
				{
					ExTraceGlobals.webAppPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.webAppPublisherTracer;
			}
		}

		public static Trace AzurePublisherTracer
		{
			get
			{
				if (ExTraceGlobals.azurePublisherTracer == null)
				{
					ExTraceGlobals.azurePublisherTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.azurePublisherTracer;
			}
		}

		public static Trace AzureHubCreationPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.azureHubCreationPublisherTracer == null)
				{
					ExTraceGlobals.azureHubCreationPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.azureHubCreationPublisherTracer;
			}
		}

		public static Trace AzureChallengeRequestPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.azureChallengeRequestPublisherTracer == null)
				{
					ExTraceGlobals.azureChallengeRequestPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.azureChallengeRequestPublisherTracer;
			}
		}

		public static Trace AzureDeviceRegistrationPublisherTracer
		{
			get
			{
				if (ExTraceGlobals.azureDeviceRegistrationPublisherTracer == null)
				{
					ExTraceGlobals.azureDeviceRegistrationPublisherTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.azureDeviceRegistrationPublisherTracer;
			}
		}

		private static Guid componentGuid = new Guid("5af2f275-ee7b-466b-8ba6-b317da1f7800");

		private static Trace exchangeToOwaTracer = null;

		private static Trace notificationFormatTracer = null;

		private static Trace publisherManagerTracer = null;

		private static Trace storageNotificationSubscriptionTracer = null;

		private static Trace pushNotificationAssistantTracer = null;

		private static Trace apnsPublisherTracer = null;

		private static Trace wnsPublisherTracer = null;

		private static Trace gcmPublisherTracer = null;

		private static Trace proxyPublisherTracer = null;

		private static Trace pendingGetPublisherTracer = null;

		private static Trace pushNotificationServiceTracer = null;

		private static Trace pushNotificationClientTracer = null;

		private static Trace webAppPublisherTracer = null;

		private static Trace azurePublisherTracer = null;

		private static Trace azureHubCreationPublisherTracer = null;

		private static Trace azureChallengeRequestPublisherTracer = null;

		private static Trace azureDeviceRegistrationPublisherTracer = null;
	}
}
