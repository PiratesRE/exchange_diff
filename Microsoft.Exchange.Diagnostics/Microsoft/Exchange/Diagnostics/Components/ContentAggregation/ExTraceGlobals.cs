using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.ContentAggregation
{
	public static class ExTraceGlobals
	{
		public static Trace CommonTracer
		{
			get
			{
				if (ExTraceGlobals.commonTracer == null)
				{
					ExTraceGlobals.commonTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.commonTracer;
			}
		}

		public static Trace Pop3ClientTracer
		{
			get
			{
				if (ExTraceGlobals.pop3ClientTracer == null)
				{
					ExTraceGlobals.pop3ClientTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.pop3ClientTracer;
			}
		}

		public static Trace DeltaSyncStorageProviderTracer
		{
			get
			{
				if (ExTraceGlobals.deltaSyncStorageProviderTracer == null)
				{
					ExTraceGlobals.deltaSyncStorageProviderTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.deltaSyncStorageProviderTracer;
			}
		}

		public static Trace FeedParserTracer
		{
			get
			{
				if (ExTraceGlobals.feedParserTracer == null)
				{
					ExTraceGlobals.feedParserTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.feedParserTracer;
			}
		}

		public static Trace ContentGenerationTracer
		{
			get
			{
				if (ExTraceGlobals.contentGenerationTracer == null)
				{
					ExTraceGlobals.contentGenerationTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.contentGenerationTracer;
			}
		}

		public static Trace SubscriptionSubmissionRpcTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionSubmissionRpcTracer == null)
				{
					ExTraceGlobals.subscriptionSubmissionRpcTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.subscriptionSubmissionRpcTracer;
			}
		}

		public static Trace RssServerLockTracer
		{
			get
			{
				if (ExTraceGlobals.rssServerLockTracer == null)
				{
					ExTraceGlobals.rssServerLockTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.rssServerLockTracer;
			}
		}

		public static Trace SubscriptionSubmitTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionSubmitTracer == null)
				{
					ExTraceGlobals.subscriptionSubmitTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.subscriptionSubmitTracer;
			}
		}

		public static Trace SubscriptionSubmissionServerTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionSubmissionServerTracer == null)
				{
					ExTraceGlobals.subscriptionSubmissionServerTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.subscriptionSubmissionServerTracer;
			}
		}

		public static Trace SchedulerTracer
		{
			get
			{
				if (ExTraceGlobals.schedulerTracer == null)
				{
					ExTraceGlobals.schedulerTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.schedulerTracer;
			}
		}

		public static Trace SubscriptionManagerTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionManagerTracer == null)
				{
					ExTraceGlobals.subscriptionManagerTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.subscriptionManagerTracer;
			}
		}

		public static Trace WebFeedProtocolHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.webFeedProtocolHandlerTracer == null)
				{
					ExTraceGlobals.webFeedProtocolHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.webFeedProtocolHandlerTracer;
			}
		}

		public static Trace DeliveryAgentTracer
		{
			get
			{
				if (ExTraceGlobals.deliveryAgentTracer == null)
				{
					ExTraceGlobals.deliveryAgentTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.deliveryAgentTracer;
			}
		}

		public static Trace HtmlFixerTracer
		{
			get
			{
				if (ExTraceGlobals.htmlFixerTracer == null)
				{
					ExTraceGlobals.htmlFixerTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.htmlFixerTracer;
			}
		}

		public static Trace Pop3ProtocolHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.pop3ProtocolHandlerTracer == null)
				{
					ExTraceGlobals.pop3ProtocolHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.pop3ProtocolHandlerTracer;
			}
		}

		public static Trace Pop3StorageProviderTracer
		{
			get
			{
				if (ExTraceGlobals.pop3StorageProviderTracer == null)
				{
					ExTraceGlobals.pop3StorageProviderTracer = new Trace(ExTraceGlobals.componentGuid, 15);
				}
				return ExTraceGlobals.pop3StorageProviderTracer;
			}
		}

		public static Trace SubscriptionTaskTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionTaskTracer == null)
				{
					ExTraceGlobals.subscriptionTaskTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.subscriptionTaskTracer;
			}
		}

		public static Trace SyncEngineTracer
		{
			get
			{
				if (ExTraceGlobals.syncEngineTracer == null)
				{
					ExTraceGlobals.syncEngineTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.syncEngineTracer;
			}
		}

		public static Trace TransportSyncStorageProviderTracer
		{
			get
			{
				if (ExTraceGlobals.transportSyncStorageProviderTracer == null)
				{
					ExTraceGlobals.transportSyncStorageProviderTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.transportSyncStorageProviderTracer;
			}
		}

		public static Trace StateStorageTracer
		{
			get
			{
				if (ExTraceGlobals.stateStorageTracer == null)
				{
					ExTraceGlobals.stateStorageTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.stateStorageTracer;
			}
		}

		public static Trace SyncLogTracer
		{
			get
			{
				if (ExTraceGlobals.syncLogTracer == null)
				{
					ExTraceGlobals.syncLogTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.syncLogTracer;
			}
		}

		public static Trace XSOSyncStorageProviderTracer
		{
			get
			{
				if (ExTraceGlobals.xSOSyncStorageProviderTracer == null)
				{
					ExTraceGlobals.xSOSyncStorageProviderTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.xSOSyncStorageProviderTracer;
			}
		}

		public static Trace SubscriptionEventbasedAssistantTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionEventbasedAssistantTracer == null)
				{
					ExTraceGlobals.subscriptionEventbasedAssistantTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.subscriptionEventbasedAssistantTracer;
			}
		}

		public static Trace CacheManagerTracer
		{
			get
			{
				if (ExTraceGlobals.cacheManagerTracer == null)
				{
					ExTraceGlobals.cacheManagerTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.cacheManagerTracer;
			}
		}

		public static Trace CacheManagerLookupTracer
		{
			get
			{
				if (ExTraceGlobals.cacheManagerLookupTracer == null)
				{
					ExTraceGlobals.cacheManagerLookupTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.cacheManagerLookupTracer;
			}
		}

		public static Trace TokenManagerTracer
		{
			get
			{
				if (ExTraceGlobals.tokenManagerTracer == null)
				{
					ExTraceGlobals.tokenManagerTracer = new Trace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.tokenManagerTracer;
			}
		}

		public static Trace SubscriptionCompletionServerTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionCompletionServerTracer == null)
				{
					ExTraceGlobals.subscriptionCompletionServerTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.subscriptionCompletionServerTracer;
			}
		}

		public static Trace SubscriptionCompletionClientTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionCompletionClientTracer == null)
				{
					ExTraceGlobals.subscriptionCompletionClientTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.subscriptionCompletionClientTracer;
			}
		}

		public static Trace EventLogTracer
		{
			get
			{
				if (ExTraceGlobals.eventLogTracer == null)
				{
					ExTraceGlobals.eventLogTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.eventLogTracer;
			}
		}

		public static Trace ProtocolHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.protocolHandlerTracer == null)
				{
					ExTraceGlobals.protocolHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.protocolHandlerTracer;
			}
		}

		public static Trace AggregationComponentTracer
		{
			get
			{
				if (ExTraceGlobals.aggregationComponentTracer == null)
				{
					ExTraceGlobals.aggregationComponentTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.aggregationComponentTracer;
			}
		}

		public static Trace IMAPSyncStorageProviderTracer
		{
			get
			{
				if (ExTraceGlobals.iMAPSyncStorageProviderTracer == null)
				{
					ExTraceGlobals.iMAPSyncStorageProviderTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.iMAPSyncStorageProviderTracer;
			}
		}

		public static Trace IMAPClientTracer
		{
			get
			{
				if (ExTraceGlobals.iMAPClientTracer == null)
				{
					ExTraceGlobals.iMAPClientTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.iMAPClientTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace DavClientTracer
		{
			get
			{
				if (ExTraceGlobals.davClientTracer == null)
				{
					ExTraceGlobals.davClientTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.davClientTracer;
			}
		}

		public static Trace DavSyncStorageProviderTracer
		{
			get
			{
				if (ExTraceGlobals.davSyncStorageProviderTracer == null)
				{
					ExTraceGlobals.davSyncStorageProviderTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.davSyncStorageProviderTracer;
			}
		}

		public static Trace SyncPoisonHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.syncPoisonHandlerTracer == null)
				{
					ExTraceGlobals.syncPoisonHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.syncPoisonHandlerTracer;
			}
		}

		public static Trace NativeSyncStorageProviderTracer
		{
			get
			{
				if (ExTraceGlobals.nativeSyncStorageProviderTracer == null)
				{
					ExTraceGlobals.nativeSyncStorageProviderTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.nativeSyncStorageProviderTracer;
			}
		}

		public static Trace SendAsTracer
		{
			get
			{
				if (ExTraceGlobals.sendAsTracer == null)
				{
					ExTraceGlobals.sendAsTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.sendAsTracer;
			}
		}

		public static Trace StatefulHubPickerTracer
		{
			get
			{
				if (ExTraceGlobals.statefulHubPickerTracer == null)
				{
					ExTraceGlobals.statefulHubPickerTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.statefulHubPickerTracer;
			}
		}

		public static Trace RemoteAccountPolicyTracer
		{
			get
			{
				if (ExTraceGlobals.remoteAccountPolicyTracer == null)
				{
					ExTraceGlobals.remoteAccountPolicyTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.remoteAccountPolicyTracer;
			}
		}

		public static Trace DataAccessLayerTracer
		{
			get
			{
				if (ExTraceGlobals.dataAccessLayerTracer == null)
				{
					ExTraceGlobals.dataAccessLayerTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.dataAccessLayerTracer;
			}
		}

		public static Trace SystemMailboxSessionPoolTracer
		{
			get
			{
				if (ExTraceGlobals.systemMailboxSessionPoolTracer == null)
				{
					ExTraceGlobals.systemMailboxSessionPoolTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.systemMailboxSessionPoolTracer;
			}
		}

		public static Trace SubscriptionCacheMessageTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionCacheMessageTracer == null)
				{
					ExTraceGlobals.subscriptionCacheMessageTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.subscriptionCacheMessageTracer;
			}
		}

		public static Trace SubscriptionQueueTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionQueueTracer == null)
				{
					ExTraceGlobals.subscriptionQueueTracer = new Trace(ExTraceGlobals.componentGuid, 45);
				}
				return ExTraceGlobals.subscriptionQueueTracer;
			}
		}

		public static Trace SubscriptionCacheRpcServerTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionCacheRpcServerTracer == null)
				{
					ExTraceGlobals.subscriptionCacheRpcServerTracer = new Trace(ExTraceGlobals.componentGuid, 46);
				}
				return ExTraceGlobals.subscriptionCacheRpcServerTracer;
			}
		}

		public static Trace ContentAggregationConfigTracer
		{
			get
			{
				if (ExTraceGlobals.contentAggregationConfigTracer == null)
				{
					ExTraceGlobals.contentAggregationConfigTracer = new Trace(ExTraceGlobals.componentGuid, 47);
				}
				return ExTraceGlobals.contentAggregationConfigTracer;
			}
		}

		public static Trace AggregationConfigurationTracer
		{
			get
			{
				if (ExTraceGlobals.aggregationConfigurationTracer == null)
				{
					ExTraceGlobals.aggregationConfigurationTracer = new Trace(ExTraceGlobals.componentGuid, 48);
				}
				return ExTraceGlobals.aggregationConfigurationTracer;
			}
		}

		public static Trace SubscriptionAgentManagerTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionAgentManagerTracer == null)
				{
					ExTraceGlobals.subscriptionAgentManagerTracer = new Trace(ExTraceGlobals.componentGuid, 49);
				}
				return ExTraceGlobals.subscriptionAgentManagerTracer;
			}
		}

		public static Trace SyncHealthLogManagerTracer
		{
			get
			{
				if (ExTraceGlobals.syncHealthLogManagerTracer == null)
				{
					ExTraceGlobals.syncHealthLogManagerTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.syncHealthLogManagerTracer;
			}
		}

		public static Trace TransportSyncManagerSvcTracer
		{
			get
			{
				if (ExTraceGlobals.transportSyncManagerSvcTracer == null)
				{
					ExTraceGlobals.transportSyncManagerSvcTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.transportSyncManagerSvcTracer;
			}
		}

		public static Trace GlobalDatabaseHandlerTracer
		{
			get
			{
				if (ExTraceGlobals.globalDatabaseHandlerTracer == null)
				{
					ExTraceGlobals.globalDatabaseHandlerTracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.globalDatabaseHandlerTracer;
			}
		}

		public static Trace DatabaseManagerTracer
		{
			get
			{
				if (ExTraceGlobals.databaseManagerTracer == null)
				{
					ExTraceGlobals.databaseManagerTracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.databaseManagerTracer;
			}
		}

		public static Trace MailboxManagerTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxManagerTracer == null)
				{
					ExTraceGlobals.mailboxManagerTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.mailboxManagerTracer;
			}
		}

		public static Trace MailboxTableManagerTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxTableManagerTracer == null)
				{
					ExTraceGlobals.mailboxTableManagerTracer = new Trace(ExTraceGlobals.componentGuid, 55);
				}
				return ExTraceGlobals.mailboxTableManagerTracer;
			}
		}

		public static Trace SubscriptionNotificationServerTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionNotificationServerTracer == null)
				{
					ExTraceGlobals.subscriptionNotificationServerTracer = new Trace(ExTraceGlobals.componentGuid, 56);
				}
				return ExTraceGlobals.subscriptionNotificationServerTracer;
			}
		}

		public static Trace SubscriptionNotificationClientTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionNotificationClientTracer == null)
				{
					ExTraceGlobals.subscriptionNotificationClientTracer = new Trace(ExTraceGlobals.componentGuid, 57);
				}
				return ExTraceGlobals.subscriptionNotificationClientTracer;
			}
		}

		public static Trace FacebookProviderTracer
		{
			get
			{
				if (ExTraceGlobals.facebookProviderTracer == null)
				{
					ExTraceGlobals.facebookProviderTracer = new Trace(ExTraceGlobals.componentGuid, 58);
				}
				return ExTraceGlobals.facebookProviderTracer;
			}
		}

		public static Trace LinkedInProviderTracer
		{
			get
			{
				if (ExTraceGlobals.linkedInProviderTracer == null)
				{
					ExTraceGlobals.linkedInProviderTracer = new Trace(ExTraceGlobals.componentGuid, 59);
				}
				return ExTraceGlobals.linkedInProviderTracer;
			}
		}

		public static Trace SubscriptionRemoveTracer
		{
			get
			{
				if (ExTraceGlobals.subscriptionRemoveTracer == null)
				{
					ExTraceGlobals.subscriptionRemoveTracer = new Trace(ExTraceGlobals.componentGuid, 60);
				}
				return ExTraceGlobals.subscriptionRemoveTracer;
			}
		}

		private static Guid componentGuid = new Guid("B29C4959-0C49-4bfa-BDDD-9B6E961420AC");

		private static Trace commonTracer = null;

		private static Trace pop3ClientTracer = null;

		private static Trace deltaSyncStorageProviderTracer = null;

		private static Trace feedParserTracer = null;

		private static Trace contentGenerationTracer = null;

		private static Trace subscriptionSubmissionRpcTracer = null;

		private static Trace rssServerLockTracer = null;

		private static Trace subscriptionSubmitTracer = null;

		private static Trace subscriptionSubmissionServerTracer = null;

		private static Trace schedulerTracer = null;

		private static Trace subscriptionManagerTracer = null;

		private static Trace webFeedProtocolHandlerTracer = null;

		private static Trace deliveryAgentTracer = null;

		private static Trace htmlFixerTracer = null;

		private static Trace pop3ProtocolHandlerTracer = null;

		private static Trace pop3StorageProviderTracer = null;

		private static Trace subscriptionTaskTracer = null;

		private static Trace syncEngineTracer = null;

		private static Trace transportSyncStorageProviderTracer = null;

		private static Trace stateStorageTracer = null;

		private static Trace syncLogTracer = null;

		private static Trace xSOSyncStorageProviderTracer = null;

		private static Trace subscriptionEventbasedAssistantTracer = null;

		private static Trace cacheManagerTracer = null;

		private static Trace cacheManagerLookupTracer = null;

		private static Trace tokenManagerTracer = null;

		private static Trace subscriptionCompletionServerTracer = null;

		private static Trace subscriptionCompletionClientTracer = null;

		private static Trace eventLogTracer = null;

		private static Trace protocolHandlerTracer = null;

		private static Trace aggregationComponentTracer = null;

		private static Trace iMAPSyncStorageProviderTracer = null;

		private static Trace iMAPClientTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace davClientTracer = null;

		private static Trace davSyncStorageProviderTracer = null;

		private static Trace syncPoisonHandlerTracer = null;

		private static Trace nativeSyncStorageProviderTracer = null;

		private static Trace sendAsTracer = null;

		private static Trace statefulHubPickerTracer = null;

		private static Trace remoteAccountPolicyTracer = null;

		private static Trace dataAccessLayerTracer = null;

		private static Trace systemMailboxSessionPoolTracer = null;

		private static Trace subscriptionCacheMessageTracer = null;

		private static Trace subscriptionQueueTracer = null;

		private static Trace subscriptionCacheRpcServerTracer = null;

		private static Trace contentAggregationConfigTracer = null;

		private static Trace aggregationConfigurationTracer = null;

		private static Trace subscriptionAgentManagerTracer = null;

		private static Trace syncHealthLogManagerTracer = null;

		private static Trace transportSyncManagerSvcTracer = null;

		private static Trace globalDatabaseHandlerTracer = null;

		private static Trace databaseManagerTracer = null;

		private static Trace mailboxManagerTracer = null;

		private static Trace mailboxTableManagerTracer = null;

		private static Trace subscriptionNotificationServerTracer = null;

		private static Trace subscriptionNotificationClientTracer = null;

		private static Trace facebookProviderTracer = null;

		private static Trace linkedInProviderTracer = null;

		private static Trace subscriptionRemoveTracer = null;
	}
}
