using System;
using Microsoft.Exchange.Diagnostics.FaultInjection;

namespace Microsoft.Exchange.Diagnostics.Components.Search
{
	public static class ExTraceGlobals
	{
		public static Trace GeneralTracer
		{
			get
			{
				if (ExTraceGlobals.generalTracer == null)
				{
					ExTraceGlobals.generalTracer = new Trace(ExTraceGlobals.componentGuid, 0);
				}
				return ExTraceGlobals.generalTracer;
			}
		}

		public static Trace CatchUpNotificationCrawlerTracer
		{
			get
			{
				if (ExTraceGlobals.catchUpNotificationCrawlerTracer == null)
				{
					ExTraceGlobals.catchUpNotificationCrawlerTracer = new Trace(ExTraceGlobals.componentGuid, 1);
				}
				return ExTraceGlobals.catchUpNotificationCrawlerTracer;
			}
		}

		public static Trace ChunkSourceFunctionsTracer
		{
			get
			{
				if (ExTraceGlobals.chunkSourceFunctionsTracer == null)
				{
					ExTraceGlobals.chunkSourceFunctionsTracer = new Trace(ExTraceGlobals.componentGuid, 2);
				}
				return ExTraceGlobals.chunkSourceFunctionsTracer;
			}
		}

		public static Trace CrawlerTracer
		{
			get
			{
				if (ExTraceGlobals.crawlerTracer == null)
				{
					ExTraceGlobals.crawlerTracer = new Trace(ExTraceGlobals.componentGuid, 3);
				}
				return ExTraceGlobals.crawlerTracer;
			}
		}

		public static Trace CSrchProjectTracer
		{
			get
			{
				if (ExTraceGlobals.cSrchProjectTracer == null)
				{
					ExTraceGlobals.cSrchProjectTracer = new Trace(ExTraceGlobals.componentGuid, 4);
				}
				return ExTraceGlobals.cSrchProjectTracer;
			}
		}

		public static Trace DataSourceFunctionsTracer
		{
			get
			{
				if (ExTraceGlobals.dataSourceFunctionsTracer == null)
				{
					ExTraceGlobals.dataSourceFunctionsTracer = new Trace(ExTraceGlobals.componentGuid, 5);
				}
				return ExTraceGlobals.dataSourceFunctionsTracer;
			}
		}

		public static Trace DriverTracer
		{
			get
			{
				if (ExTraceGlobals.driverTracer == null)
				{
					ExTraceGlobals.driverTracer = new Trace(ExTraceGlobals.componentGuid, 6);
				}
				return ExTraceGlobals.driverTracer;
			}
		}

		public static Trace FilterEnumeratorTracer
		{
			get
			{
				if (ExTraceGlobals.filterEnumeratorTracer == null)
				{
					ExTraceGlobals.filterEnumeratorTracer = new Trace(ExTraceGlobals.componentGuid, 7);
				}
				return ExTraceGlobals.filterEnumeratorTracer;
			}
		}

		public static Trace FTEAdminComInteropTracer
		{
			get
			{
				if (ExTraceGlobals.fTEAdminComInteropTracer == null)
				{
					ExTraceGlobals.fTEAdminComInteropTracer = new Trace(ExTraceGlobals.componentGuid, 8);
				}
				return ExTraceGlobals.fTEAdminComInteropTracer;
			}
		}

		public static Trace IndexablePropertyCacheTracer
		{
			get
			{
				if (ExTraceGlobals.indexablePropertyCacheTracer == null)
				{
					ExTraceGlobals.indexablePropertyCacheTracer = new Trace(ExTraceGlobals.componentGuid, 9);
				}
				return ExTraceGlobals.indexablePropertyCacheTracer;
			}
		}

		public static Trace MapiIteratorTracer
		{
			get
			{
				if (ExTraceGlobals.mapiIteratorTracer == null)
				{
					ExTraceGlobals.mapiIteratorTracer = new Trace(ExTraceGlobals.componentGuid, 10);
				}
				return ExTraceGlobals.mapiIteratorTracer;
			}
		}

		public static Trace NotificationProcessingTracer
		{
			get
			{
				if (ExTraceGlobals.notificationProcessingTracer == null)
				{
					ExTraceGlobals.notificationProcessingTracer = new Trace(ExTraceGlobals.componentGuid, 11);
				}
				return ExTraceGlobals.notificationProcessingTracer;
			}
		}

		public static Trace NotificationQueueTracer
		{
			get
			{
				if (ExTraceGlobals.notificationQueueTracer == null)
				{
					ExTraceGlobals.notificationQueueTracer = new Trace(ExTraceGlobals.componentGuid, 12);
				}
				return ExTraceGlobals.notificationQueueTracer;
			}
		}

		public static Trace NotificationWatcherTracer
		{
			get
			{
				if (ExTraceGlobals.notificationWatcherTracer == null)
				{
					ExTraceGlobals.notificationWatcherTracer = new Trace(ExTraceGlobals.componentGuid, 13);
				}
				return ExTraceGlobals.notificationWatcherTracer;
			}
		}

		public static Trace PHFunctionsTracer
		{
			get
			{
				if (ExTraceGlobals.pHFunctionsTracer == null)
				{
					ExTraceGlobals.pHFunctionsTracer = new Trace(ExTraceGlobals.componentGuid, 14);
				}
				return ExTraceGlobals.pHFunctionsTracer;
			}
		}

		public static Trace RetryEngineTracer
		{
			get
			{
				if (ExTraceGlobals.retryEngineTracer == null)
				{
					ExTraceGlobals.retryEngineTracer = new Trace(ExTraceGlobals.componentGuid, 16);
				}
				return ExTraceGlobals.retryEngineTracer;
			}
		}

		public static Trace ThrottleControllerTracer
		{
			get
			{
				if (ExTraceGlobals.throttleControllerTracer == null)
				{
					ExTraceGlobals.throttleControllerTracer = new Trace(ExTraceGlobals.componentGuid, 17);
				}
				return ExTraceGlobals.throttleControllerTracer;
			}
		}

		public static Trace PropertyStoreCacheTracer
		{
			get
			{
				if (ExTraceGlobals.propertyStoreCacheTracer == null)
				{
					ExTraceGlobals.propertyStoreCacheTracer = new Trace(ExTraceGlobals.componentGuid, 18);
				}
				return ExTraceGlobals.propertyStoreCacheTracer;
			}
		}

		public static Trace ActiveManagerTracer
		{
			get
			{
				if (ExTraceGlobals.activeManagerTracer == null)
				{
					ExTraceGlobals.activeManagerTracer = new Trace(ExTraceGlobals.componentGuid, 19);
				}
				return ExTraceGlobals.activeManagerTracer;
			}
		}

		public static Trace CatalogHealthTracer
		{
			get
			{
				if (ExTraceGlobals.catalogHealthTracer == null)
				{
					ExTraceGlobals.catalogHealthTracer = new Trace(ExTraceGlobals.componentGuid, 20);
				}
				return ExTraceGlobals.catalogHealthTracer;
			}
		}

		public static Trace SearchCatalogClientTracer
		{
			get
			{
				if (ExTraceGlobals.searchCatalogClientTracer == null)
				{
					ExTraceGlobals.searchCatalogClientTracer = new Trace(ExTraceGlobals.componentGuid, 21);
				}
				return ExTraceGlobals.searchCatalogClientTracer;
			}
		}

		public static Trace SearchCatalogServerTracer
		{
			get
			{
				if (ExTraceGlobals.searchCatalogServerTracer == null)
				{
					ExTraceGlobals.searchCatalogServerTracer = new Trace(ExTraceGlobals.componentGuid, 22);
				}
				return ExTraceGlobals.searchCatalogServerTracer;
			}
		}

		public static Trace MailboxDeletionTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxDeletionTracer == null)
				{
					ExTraceGlobals.mailboxDeletionTracer = new Trace(ExTraceGlobals.componentGuid, 23);
				}
				return ExTraceGlobals.mailboxDeletionTracer;
			}
		}

		public static Trace PropertyStoreTracer
		{
			get
			{
				if (ExTraceGlobals.propertyStoreTracer == null)
				{
					ExTraceGlobals.propertyStoreTracer = new Trace(ExTraceGlobals.componentGuid, 24);
				}
				return ExTraceGlobals.propertyStoreTracer;
			}
		}

		public static Trace StoreMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.storeMonitorTracer == null)
				{
					ExTraceGlobals.storeMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 25);
				}
				return ExTraceGlobals.storeMonitorTracer;
			}
		}

		public static FaultInjectionTrace FaultInjectionTracer
		{
			get
			{
				if (ExTraceGlobals.faultInjectionTracer == null)
				{
					ExTraceGlobals.faultInjectionTracer = new FaultInjectionTrace(ExTraceGlobals.componentGuid, 26);
				}
				return ExTraceGlobals.faultInjectionTracer;
			}
		}

		public static Trace MailboxIndexingHelperTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxIndexingHelperTracer == null)
				{
					ExTraceGlobals.mailboxIndexingHelperTracer = new Trace(ExTraceGlobals.componentGuid, 27);
				}
				return ExTraceGlobals.mailboxIndexingHelperTracer;
			}
		}

		public static Trace CatalogStateTracer
		{
			get
			{
				if (ExTraceGlobals.catalogStateTracer == null)
				{
					ExTraceGlobals.catalogStateTracer = new Trace(ExTraceGlobals.componentGuid, 28);
				}
				return ExTraceGlobals.catalogStateTracer;
			}
		}

		public static Trace FileExtensionCacheTracer
		{
			get
			{
				if (ExTraceGlobals.fileExtensionCacheTracer == null)
				{
					ExTraceGlobals.fileExtensionCacheTracer = new Trace(ExTraceGlobals.componentGuid, 29);
				}
				return ExTraceGlobals.fileExtensionCacheTracer;
			}
		}

		public static Trace MsFteSqlMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.msFteSqlMonitorTracer == null)
				{
					ExTraceGlobals.msFteSqlMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 30);
				}
				return ExTraceGlobals.msFteSqlMonitorTracer;
			}
		}

		public static Trace ServerConnectionsTracer
		{
			get
			{
				if (ExTraceGlobals.serverConnectionsTracer == null)
				{
					ExTraceGlobals.serverConnectionsTracer = new Trace(ExTraceGlobals.componentGuid, 31);
				}
				return ExTraceGlobals.serverConnectionsTracer;
			}
		}

		public static Trace LogonCacheTracer
		{
			get
			{
				if (ExTraceGlobals.logonCacheTracer == null)
				{
					ExTraceGlobals.logonCacheTracer = new Trace(ExTraceGlobals.componentGuid, 32);
				}
				return ExTraceGlobals.logonCacheTracer;
			}
		}

		public static Trace LogonTracer
		{
			get
			{
				if (ExTraceGlobals.logonTracer == null)
				{
					ExTraceGlobals.logonTracer = new Trace(ExTraceGlobals.componentGuid, 33);
				}
				return ExTraceGlobals.logonTracer;
			}
		}

		public static Trace CatalogReconcilerTracer
		{
			get
			{
				if (ExTraceGlobals.catalogReconcilerTracer == null)
				{
					ExTraceGlobals.catalogReconcilerTracer = new Trace(ExTraceGlobals.componentGuid, 34);
				}
				return ExTraceGlobals.catalogReconcilerTracer;
			}
		}

		public static Trace CatalogReconcileResultTracer
		{
			get
			{
				if (ExTraceGlobals.catalogReconcileResultTracer == null)
				{
					ExTraceGlobals.catalogReconcileResultTracer = new Trace(ExTraceGlobals.componentGuid, 35);
				}
				return ExTraceGlobals.catalogReconcileResultTracer;
			}
		}

		public static Trace AllCatalogReconcilerTracer
		{
			get
			{
				if (ExTraceGlobals.allCatalogReconcilerTracer == null)
				{
					ExTraceGlobals.allCatalogReconcilerTracer = new Trace(ExTraceGlobals.componentGuid, 36);
				}
				return ExTraceGlobals.allCatalogReconcilerTracer;
			}
		}

		public static Trace MailboxReconcileResultTracer
		{
			get
			{
				if (ExTraceGlobals.mailboxReconcileResultTracer == null)
				{
					ExTraceGlobals.mailboxReconcileResultTracer = new Trace(ExTraceGlobals.componentGuid, 37);
				}
				return ExTraceGlobals.mailboxReconcileResultTracer;
			}
		}

		public static Trace NewFilterMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.newFilterMonitorTracer == null)
				{
					ExTraceGlobals.newFilterMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 38);
				}
				return ExTraceGlobals.newFilterMonitorTracer;
			}
		}

		public static Trace InMemoryDefaultTracer
		{
			get
			{
				if (ExTraceGlobals.inMemoryDefaultTracer == null)
				{
					ExTraceGlobals.inMemoryDefaultTracer = new Trace(ExTraceGlobals.componentGuid, 39);
				}
				return ExTraceGlobals.inMemoryDefaultTracer;
			}
		}

		public static Trace TestExchangeSearchTracer
		{
			get
			{
				if (ExTraceGlobals.testExchangeSearchTracer == null)
				{
					ExTraceGlobals.testExchangeSearchTracer = new Trace(ExTraceGlobals.componentGuid, 40);
				}
				return ExTraceGlobals.testExchangeSearchTracer;
			}
		}

		public static Trace BatchThrottlerTracer
		{
			get
			{
				if (ExTraceGlobals.batchThrottlerTracer == null)
				{
					ExTraceGlobals.batchThrottlerTracer = new Trace(ExTraceGlobals.componentGuid, 41);
				}
				return ExTraceGlobals.batchThrottlerTracer;
			}
		}

		public static Trace ThrottleParametersTracer
		{
			get
			{
				if (ExTraceGlobals.throttleParametersTracer == null)
				{
					ExTraceGlobals.throttleParametersTracer = new Trace(ExTraceGlobals.componentGuid, 42);
				}
				return ExTraceGlobals.throttleParametersTracer;
			}
		}

		public static Trace ThrottleDataProviderTracer
		{
			get
			{
				if (ExTraceGlobals.throttleDataProviderTracer == null)
				{
					ExTraceGlobals.throttleDataProviderTracer = new Trace(ExTraceGlobals.componentGuid, 43);
				}
				return ExTraceGlobals.throttleDataProviderTracer;
			}
		}

		public static Trace RegistryParameterTracer
		{
			get
			{
				if (ExTraceGlobals.registryParameterTracer == null)
				{
					ExTraceGlobals.registryParameterTracer = new Trace(ExTraceGlobals.componentGuid, 44);
				}
				return ExTraceGlobals.registryParameterTracer;
			}
		}

		public static Trace LatencySamplerTracer
		{
			get
			{
				if (ExTraceGlobals.latencySamplerTracer == null)
				{
					ExTraceGlobals.latencySamplerTracer = new Trace(ExTraceGlobals.componentGuid, 45);
				}
				return ExTraceGlobals.latencySamplerTracer;
			}
		}

		public static Trace MovingAverageTracer
		{
			get
			{
				if (ExTraceGlobals.movingAverageTracer == null)
				{
					ExTraceGlobals.movingAverageTracer = new Trace(ExTraceGlobals.componentGuid, 46);
				}
				return ExTraceGlobals.movingAverageTracer;
			}
		}

		public static Trace CoreComponentTracer
		{
			get
			{
				if (ExTraceGlobals.coreComponentTracer == null)
				{
					ExTraceGlobals.coreComponentTracer = new Trace(ExTraceGlobals.componentGuid, 50);
				}
				return ExTraceGlobals.coreComponentTracer;
			}
		}

		public static Trace CoreComponentRegistryTracer
		{
			get
			{
				if (ExTraceGlobals.coreComponentRegistryTracer == null)
				{
					ExTraceGlobals.coreComponentRegistryTracer = new Trace(ExTraceGlobals.componentGuid, 51);
				}
				return ExTraceGlobals.coreComponentRegistryTracer;
			}
		}

		public static Trace CoreGeneralTracer
		{
			get
			{
				if (ExTraceGlobals.coreGeneralTracer == null)
				{
					ExTraceGlobals.coreGeneralTracer = new Trace(ExTraceGlobals.componentGuid, 52);
				}
				return ExTraceGlobals.coreGeneralTracer;
			}
		}

		public static Trace FastFeederTracer
		{
			get
			{
				if (ExTraceGlobals.fastFeederTracer == null)
				{
					ExTraceGlobals.fastFeederTracer = new Trace(ExTraceGlobals.componentGuid, 53);
				}
				return ExTraceGlobals.fastFeederTracer;
			}
		}

		public static Trace MdbNotificationsFeederTracer
		{
			get
			{
				if (ExTraceGlobals.mdbNotificationsFeederTracer == null)
				{
					ExTraceGlobals.mdbNotificationsFeederTracer = new Trace(ExTraceGlobals.componentGuid, 54);
				}
				return ExTraceGlobals.mdbNotificationsFeederTracer;
			}
		}

		public static Trace ServiceTracer
		{
			get
			{
				if (ExTraceGlobals.serviceTracer == null)
				{
					ExTraceGlobals.serviceTracer = new Trace(ExTraceGlobals.componentGuid, 55);
				}
				return ExTraceGlobals.serviceTracer;
			}
		}

		public static Trace EngineTracer
		{
			get
			{
				if (ExTraceGlobals.engineTracer == null)
				{
					ExTraceGlobals.engineTracer = new Trace(ExTraceGlobals.componentGuid, 56);
				}
				return ExTraceGlobals.engineTracer;
			}
		}

		public static Trace MdbFeedingControllerTracer
		{
			get
			{
				if (ExTraceGlobals.mdbFeedingControllerTracer == null)
				{
					ExTraceGlobals.mdbFeedingControllerTracer = new Trace(ExTraceGlobals.componentGuid, 57);
				}
				return ExTraceGlobals.mdbFeedingControllerTracer;
			}
		}

		public static Trace IndexManagementTracer
		{
			get
			{
				if (ExTraceGlobals.indexManagementTracer == null)
				{
					ExTraceGlobals.indexManagementTracer = new Trace(ExTraceGlobals.componentGuid, 58);
				}
				return ExTraceGlobals.indexManagementTracer;
			}
		}

		public static Trace CoreFailureMonitorTracer
		{
			get
			{
				if (ExTraceGlobals.coreFailureMonitorTracer == null)
				{
					ExTraceGlobals.coreFailureMonitorTracer = new Trace(ExTraceGlobals.componentGuid, 59);
				}
				return ExTraceGlobals.coreFailureMonitorTracer;
			}
		}

		public static Trace MdbCrawlerFeederTracer
		{
			get
			{
				if (ExTraceGlobals.mdbCrawlerFeederTracer == null)
				{
					ExTraceGlobals.mdbCrawlerFeederTracer = new Trace(ExTraceGlobals.componentGuid, 60);
				}
				return ExTraceGlobals.mdbCrawlerFeederTracer;
			}
		}

		public static Trace MdbDocumentAdapterTracer
		{
			get
			{
				if (ExTraceGlobals.mdbDocumentAdapterTracer == null)
				{
					ExTraceGlobals.mdbDocumentAdapterTracer = new Trace(ExTraceGlobals.componentGuid, 61);
				}
				return ExTraceGlobals.mdbDocumentAdapterTracer;
			}
		}

		public static Trace CoreDocumentModelTracer
		{
			get
			{
				if (ExTraceGlobals.coreDocumentModelTracer == null)
				{
					ExTraceGlobals.coreDocumentModelTracer = new Trace(ExTraceGlobals.componentGuid, 62);
				}
				return ExTraceGlobals.coreDocumentModelTracer;
			}
		}

		public static Trace PipelineLoaderTracer
		{
			get
			{
				if (ExTraceGlobals.pipelineLoaderTracer == null)
				{
					ExTraceGlobals.pipelineLoaderTracer = new Trace(ExTraceGlobals.componentGuid, 63);
				}
				return ExTraceGlobals.pipelineLoaderTracer;
			}
		}

		public static Trace CorePipelineTracer
		{
			get
			{
				if (ExTraceGlobals.corePipelineTracer == null)
				{
					ExTraceGlobals.corePipelineTracer = new Trace(ExTraceGlobals.componentGuid, 64);
				}
				return ExTraceGlobals.corePipelineTracer;
			}
		}

		public static Trace QueueManagerTracer
		{
			get
			{
				if (ExTraceGlobals.queueManagerTracer == null)
				{
					ExTraceGlobals.queueManagerTracer = new Trace(ExTraceGlobals.componentGuid, 65);
				}
				return ExTraceGlobals.queueManagerTracer;
			}
		}

		public static Trace CrawlerWatermarkManagerTracer
		{
			get
			{
				if (ExTraceGlobals.crawlerWatermarkManagerTracer == null)
				{
					ExTraceGlobals.crawlerWatermarkManagerTracer = new Trace(ExTraceGlobals.componentGuid, 66);
				}
				return ExTraceGlobals.crawlerWatermarkManagerTracer;
			}
		}

		public static Trace FailedItemStorageTracer
		{
			get
			{
				if (ExTraceGlobals.failedItemStorageTracer == null)
				{
					ExTraceGlobals.failedItemStorageTracer = new Trace(ExTraceGlobals.componentGuid, 67);
				}
				return ExTraceGlobals.failedItemStorageTracer;
			}
		}

		public static Trace MdbWatcherTracer
		{
			get
			{
				if (ExTraceGlobals.mdbWatcherTracer == null)
				{
					ExTraceGlobals.mdbWatcherTracer = new Trace(ExTraceGlobals.componentGuid, 68);
				}
				return ExTraceGlobals.mdbWatcherTracer;
			}
		}

		public static Trace MdbRetryFeederTracer
		{
			get
			{
				if (ExTraceGlobals.mdbRetryFeederTracer == null)
				{
					ExTraceGlobals.mdbRetryFeederTracer = new Trace(ExTraceGlobals.componentGuid, 69);
				}
				return ExTraceGlobals.mdbRetryFeederTracer;
			}
		}

		public static Trace MdbSessionCacheTracer
		{
			get
			{
				if (ExTraceGlobals.mdbSessionCacheTracer == null)
				{
					ExTraceGlobals.mdbSessionCacheTracer = new Trace(ExTraceGlobals.componentGuid, 70);
				}
				return ExTraceGlobals.mdbSessionCacheTracer;
			}
		}

		public static Trace RetrieverOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.retrieverOperatorTracer == null)
				{
					ExTraceGlobals.retrieverOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 71);
				}
				return ExTraceGlobals.retrieverOperatorTracer;
			}
		}

		public static Trace StreamManagerTracer
		{
			get
			{
				if (ExTraceGlobals.streamManagerTracer == null)
				{
					ExTraceGlobals.streamManagerTracer = new Trace(ExTraceGlobals.componentGuid, 72);
				}
				return ExTraceGlobals.streamManagerTracer;
			}
		}

		public static Trace StreamChannelTracer
		{
			get
			{
				if (ExTraceGlobals.streamChannelTracer == null)
				{
					ExTraceGlobals.streamChannelTracer = new Trace(ExTraceGlobals.componentGuid, 73);
				}
				return ExTraceGlobals.streamChannelTracer;
			}
		}

		public static Trace AnnotationTokenTracer
		{
			get
			{
				if (ExTraceGlobals.annotationTokenTracer == null)
				{
					ExTraceGlobals.annotationTokenTracer = new Trace(ExTraceGlobals.componentGuid, 74);
				}
				return ExTraceGlobals.annotationTokenTracer;
			}
		}

		public static Trace TransportOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.transportOperatorTracer == null)
				{
					ExTraceGlobals.transportOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 75);
				}
				return ExTraceGlobals.transportOperatorTracer;
			}
		}

		public static Trace IndexRoutingAgentTracer
		{
			get
			{
				if (ExTraceGlobals.indexRoutingAgentTracer == null)
				{
					ExTraceGlobals.indexRoutingAgentTracer = new Trace(ExTraceGlobals.componentGuid, 76);
				}
				return ExTraceGlobals.indexRoutingAgentTracer;
			}
		}

		public static Trace IndexDeliveryAgentTracer
		{
			get
			{
				if (ExTraceGlobals.indexDeliveryAgentTracer == null)
				{
					ExTraceGlobals.indexDeliveryAgentTracer = new Trace(ExTraceGlobals.componentGuid, 77);
				}
				return ExTraceGlobals.indexDeliveryAgentTracer;
			}
		}

		public static Trace TransportFlowFeederTracer
		{
			get
			{
				if (ExTraceGlobals.transportFlowFeederTracer == null)
				{
					ExTraceGlobals.transportFlowFeederTracer = new Trace(ExTraceGlobals.componentGuid, 78);
				}
				return ExTraceGlobals.transportFlowFeederTracer;
			}
		}

		public static Trace QueryExecutorTracer
		{
			get
			{
				if (ExTraceGlobals.queryExecutorTracer == null)
				{
					ExTraceGlobals.queryExecutorTracer = new Trace(ExTraceGlobals.componentGuid, 79);
				}
				return ExTraceGlobals.queryExecutorTracer;
			}
		}

		public static Trace ErrorOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.errorOperatorTracer == null)
				{
					ExTraceGlobals.errorOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 80);
				}
				return ExTraceGlobals.errorOperatorTracer;
			}
		}

		public static Trace NotificationsWatermarkManagerTracer
		{
			get
			{
				if (ExTraceGlobals.notificationsWatermarkManagerTracer == null)
				{
					ExTraceGlobals.notificationsWatermarkManagerTracer = new Trace(ExTraceGlobals.componentGuid, 81);
				}
				return ExTraceGlobals.notificationsWatermarkManagerTracer;
			}
		}

		public static Trace IndexStatusStoreTracer
		{
			get
			{
				if (ExTraceGlobals.indexStatusStoreTracer == null)
				{
					ExTraceGlobals.indexStatusStoreTracer = new Trace(ExTraceGlobals.componentGuid, 82);
				}
				return ExTraceGlobals.indexStatusStoreTracer;
			}
		}

		public static Trace IndexStatusProviderTracer
		{
			get
			{
				if (ExTraceGlobals.indexStatusProviderTracer == null)
				{
					ExTraceGlobals.indexStatusProviderTracer = new Trace(ExTraceGlobals.componentGuid, 83);
				}
				return ExTraceGlobals.indexStatusProviderTracer;
			}
		}

		public static Trace FastIoExtensionTracer
		{
			get
			{
				if (ExTraceGlobals.fastIoExtensionTracer == null)
				{
					ExTraceGlobals.fastIoExtensionTracer = new Trace(ExTraceGlobals.componentGuid, 84);
				}
				return ExTraceGlobals.fastIoExtensionTracer;
			}
		}

		public static Trace XSOMailboxSessionTracer
		{
			get
			{
				if (ExTraceGlobals.xSOMailboxSessionTracer == null)
				{
					ExTraceGlobals.xSOMailboxSessionTracer = new Trace(ExTraceGlobals.componentGuid, 85);
				}
				return ExTraceGlobals.xSOMailboxSessionTracer;
			}
		}

		public static Trace PostDocParserOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.postDocParserOperatorTracer == null)
				{
					ExTraceGlobals.postDocParserOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 86);
				}
				return ExTraceGlobals.postDocParserOperatorTracer;
			}
		}

		public static Trace RecordManagerOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.recordManagerOperatorTracer == null)
				{
					ExTraceGlobals.recordManagerOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 87);
				}
				return ExTraceGlobals.recordManagerOperatorTracer;
			}
		}

		public static Trace OperatorDiagnosticsTracer
		{
			get
			{
				if (ExTraceGlobals.operatorDiagnosticsTracer == null)
				{
					ExTraceGlobals.operatorDiagnosticsTracer = new Trace(ExTraceGlobals.componentGuid, 88);
				}
				return ExTraceGlobals.operatorDiagnosticsTracer;
			}
		}

		public static Trace SearchRpcClientTracer
		{
			get
			{
				if (ExTraceGlobals.searchRpcClientTracer == null)
				{
					ExTraceGlobals.searchRpcClientTracer = new Trace(ExTraceGlobals.componentGuid, 89);
				}
				return ExTraceGlobals.searchRpcClientTracer;
			}
		}

		public static Trace SearchRpcServerTracer
		{
			get
			{
				if (ExTraceGlobals.searchRpcServerTracer == null)
				{
					ExTraceGlobals.searchRpcServerTracer = new Trace(ExTraceGlobals.componentGuid, 90);
				}
				return ExTraceGlobals.searchRpcServerTracer;
			}
		}

		public static Trace DocumentTrackerOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.documentTrackerOperatorTracer == null)
				{
					ExTraceGlobals.documentTrackerOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 91);
				}
				return ExTraceGlobals.documentTrackerOperatorTracer;
			}
		}

		public static Trace ErrorBypassOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.errorBypassOperatorTracer == null)
				{
					ExTraceGlobals.errorBypassOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 92);
				}
				return ExTraceGlobals.errorBypassOperatorTracer;
			}
		}

		public static Trace FeederThrottlingTracer
		{
			get
			{
				if (ExTraceGlobals.feederThrottlingTracer == null)
				{
					ExTraceGlobals.feederThrottlingTracer = new Trace(ExTraceGlobals.componentGuid, 93);
				}
				return ExTraceGlobals.feederThrottlingTracer;
			}
		}

		public static Trace WatermarkStorageTracer
		{
			get
			{
				if (ExTraceGlobals.watermarkStorageTracer == null)
				{
					ExTraceGlobals.watermarkStorageTracer = new Trace(ExTraceGlobals.componentGuid, 94);
				}
				return ExTraceGlobals.watermarkStorageTracer;
			}
		}

		public static Trace DiagnosticOperatorTracer
		{
			get
			{
				if (ExTraceGlobals.diagnosticOperatorTracer == null)
				{
					ExTraceGlobals.diagnosticOperatorTracer = new Trace(ExTraceGlobals.componentGuid, 95);
				}
				return ExTraceGlobals.diagnosticOperatorTracer;
			}
		}

		public static Trace InstantSearchTracer
		{
			get
			{
				if (ExTraceGlobals.instantSearchTracer == null)
				{
					ExTraceGlobals.instantSearchTracer = new Trace(ExTraceGlobals.componentGuid, 96);
				}
				return ExTraceGlobals.instantSearchTracer;
			}
		}

		public static Trace TopNManagementClientTracer
		{
			get
			{
				if (ExTraceGlobals.topNManagementClientTracer == null)
				{
					ExTraceGlobals.topNManagementClientTracer = new Trace(ExTraceGlobals.componentGuid, 97);
				}
				return ExTraceGlobals.topNManagementClientTracer;
			}
		}

		public static Trace SearchDictionaryTracer
		{
			get
			{
				if (ExTraceGlobals.searchDictionaryTracer == null)
				{
					ExTraceGlobals.searchDictionaryTracer = new Trace(ExTraceGlobals.componentGuid, 98);
				}
				return ExTraceGlobals.searchDictionaryTracer;
			}
		}

		private static Guid componentGuid = new Guid("c3ea5adf-c135-45e7-9dff-e1dc3bd67123");

		private static Trace generalTracer = null;

		private static Trace catchUpNotificationCrawlerTracer = null;

		private static Trace chunkSourceFunctionsTracer = null;

		private static Trace crawlerTracer = null;

		private static Trace cSrchProjectTracer = null;

		private static Trace dataSourceFunctionsTracer = null;

		private static Trace driverTracer = null;

		private static Trace filterEnumeratorTracer = null;

		private static Trace fTEAdminComInteropTracer = null;

		private static Trace indexablePropertyCacheTracer = null;

		private static Trace mapiIteratorTracer = null;

		private static Trace notificationProcessingTracer = null;

		private static Trace notificationQueueTracer = null;

		private static Trace notificationWatcherTracer = null;

		private static Trace pHFunctionsTracer = null;

		private static Trace retryEngineTracer = null;

		private static Trace throttleControllerTracer = null;

		private static Trace propertyStoreCacheTracer = null;

		private static Trace activeManagerTracer = null;

		private static Trace catalogHealthTracer = null;

		private static Trace searchCatalogClientTracer = null;

		private static Trace searchCatalogServerTracer = null;

		private static Trace mailboxDeletionTracer = null;

		private static Trace propertyStoreTracer = null;

		private static Trace storeMonitorTracer = null;

		private static FaultInjectionTrace faultInjectionTracer = null;

		private static Trace mailboxIndexingHelperTracer = null;

		private static Trace catalogStateTracer = null;

		private static Trace fileExtensionCacheTracer = null;

		private static Trace msFteSqlMonitorTracer = null;

		private static Trace serverConnectionsTracer = null;

		private static Trace logonCacheTracer = null;

		private static Trace logonTracer = null;

		private static Trace catalogReconcilerTracer = null;

		private static Trace catalogReconcileResultTracer = null;

		private static Trace allCatalogReconcilerTracer = null;

		private static Trace mailboxReconcileResultTracer = null;

		private static Trace newFilterMonitorTracer = null;

		private static Trace inMemoryDefaultTracer = null;

		private static Trace testExchangeSearchTracer = null;

		private static Trace batchThrottlerTracer = null;

		private static Trace throttleParametersTracer = null;

		private static Trace throttleDataProviderTracer = null;

		private static Trace registryParameterTracer = null;

		private static Trace latencySamplerTracer = null;

		private static Trace movingAverageTracer = null;

		private static Trace coreComponentTracer = null;

		private static Trace coreComponentRegistryTracer = null;

		private static Trace coreGeneralTracer = null;

		private static Trace fastFeederTracer = null;

		private static Trace mdbNotificationsFeederTracer = null;

		private static Trace serviceTracer = null;

		private static Trace engineTracer = null;

		private static Trace mdbFeedingControllerTracer = null;

		private static Trace indexManagementTracer = null;

		private static Trace coreFailureMonitorTracer = null;

		private static Trace mdbCrawlerFeederTracer = null;

		private static Trace mdbDocumentAdapterTracer = null;

		private static Trace coreDocumentModelTracer = null;

		private static Trace pipelineLoaderTracer = null;

		private static Trace corePipelineTracer = null;

		private static Trace queueManagerTracer = null;

		private static Trace crawlerWatermarkManagerTracer = null;

		private static Trace failedItemStorageTracer = null;

		private static Trace mdbWatcherTracer = null;

		private static Trace mdbRetryFeederTracer = null;

		private static Trace mdbSessionCacheTracer = null;

		private static Trace retrieverOperatorTracer = null;

		private static Trace streamManagerTracer = null;

		private static Trace streamChannelTracer = null;

		private static Trace annotationTokenTracer = null;

		private static Trace transportOperatorTracer = null;

		private static Trace indexRoutingAgentTracer = null;

		private static Trace indexDeliveryAgentTracer = null;

		private static Trace transportFlowFeederTracer = null;

		private static Trace queryExecutorTracer = null;

		private static Trace errorOperatorTracer = null;

		private static Trace notificationsWatermarkManagerTracer = null;

		private static Trace indexStatusStoreTracer = null;

		private static Trace indexStatusProviderTracer = null;

		private static Trace fastIoExtensionTracer = null;

		private static Trace xSOMailboxSessionTracer = null;

		private static Trace postDocParserOperatorTracer = null;

		private static Trace recordManagerOperatorTracer = null;

		private static Trace operatorDiagnosticsTracer = null;

		private static Trace searchRpcClientTracer = null;

		private static Trace searchRpcServerTracer = null;

		private static Trace documentTrackerOperatorTracer = null;

		private static Trace errorBypassOperatorTracer = null;

		private static Trace feederThrottlingTracer = null;

		private static Trace watermarkStorageTracer = null;

		private static Trace diagnosticOperatorTracer = null;

		private static Trace instantSearchTracer = null;

		private static Trace topNManagementClientTracer = null;

		private static Trace searchDictionaryTracer = null;
	}
}
