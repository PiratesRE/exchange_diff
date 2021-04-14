using System;
using System.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;
using Microsoft.Office.CompliancePolicy.PolicySync;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Hygiene.Data
{
	internal static class EventLogger
	{
		public static ExEventLog Logger
		{
			get
			{
				return EventLogger.logger;
			}
		}

		public static void LogRetry(string database, string correlationId, int retryCount, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_WebstoreDataProviderRetry, new object[]
			{
				database + ":" + correlationId,
				retryCount,
				exception
			});
		}

		public static void LogMaxRetry(string database, string correlationId, int maximumRetryCount, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_WebstoreDataProviderMaxRetry, new object[]
			{
				database + ":" + correlationId,
				maximumRetryCount,
				exception
			});
			string notificationReason = string.Format("The Webstore Data Provider for query '{0}' reached the maximum retry limit: {1} time(s). Error: {2}", database + ":" + correlationId, maximumRetryCount, exception);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoWebstoreDataProvider.MaxRetry." + database, null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static void LogSlowResponse(string database, string correlationId, TimeSpan slowResponseThreshold, TimeSpan elapsedTime)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_WebstoreDataProviderSlowResponse, new object[]
			{
				database + ":" + correlationId,
				slowResponseThreshold,
				elapsedTime
			});
		}

		public static void LogFatalError(string database, string correlationId, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_WebstoreDataProviderCallFailure, new object[]
			{
				database + ":" + correlationId,
				exception
			});
		}

		public static void LogNoConnectionAvailException(Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_WebstoreDataProviderConnectionError, new object[]
			{
				exception
			});
			string notificationReason = string.Format("The Webstore Data provider is encountered a no connection exception and failing over to the database in other Datacenter: {0}.", exception);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoWebstoreDataProvider.CrossDatacenterFailover", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static void LogMetadataRefreshException(Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_WebstoreDataProviderMetadataRefreshError, new object[]
			{
				exception
			});
		}

		public static void LogCorruptDataIgnored(Type queryType, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_WebstoreDataProviderCorruptDataIgnored, new object[]
			{
				queryType.Name,
				exception
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "WebstoreDataProvider.CorruptDataIgnored", null, string.Format("The Webstore Data Provider ignored corrupt data processing a query for {0} that triggered the following exception: {1}", queryType.Name, exception), ResultSeverityLevel.Error, false);
		}

		public static void LogCacheProviderRetry(string cacheName, int retryCount, Exception exception)
		{
			EventLogger.LogPeriodicEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheDataProviderRetry, exception.Message, new object[]
			{
				retryCount,
				exception,
				cacheName
			});
		}

		public static void LogCacheProviderMaxRetry(string cacheName, int maximumRetryCount, Exception exception, bool transientError = false)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheDataProviderMaxRetry, new object[]
			{
				maximumRetryCount,
				exception,
				cacheName
			});
			if (transientError)
			{
				string notificationReason = string.Format("The Cache Data Provider reached the maximum retry limit for transient error: {0} time(s) for cache {2}. Error: {1}", maximumRetryCount, exception, cacheName);
				EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoCacheDataProvider.MaxRetry.Transient", null, notificationReason, ResultSeverityLevel.Warning, false);
				return;
			}
			string notificationReason2 = string.Format("The Cache Data Provider reached the maximum retry limit: {0} time(s) for cache {2}. Error: {1}", maximumRetryCount, exception, cacheName);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoCacheDataProvider.MaxRetry", null, notificationReason2, ResultSeverityLevel.Warning, false);
		}

		public static void LogCacheProviderSlowResponse(string cacheName, TimeSpan slowResponseThreshold, TimeSpan elapsedTime)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheDataProviderSlowResponse, new object[]
			{
				slowResponseThreshold,
				elapsedTime,
				cacheName
			});
		}

		public static void LogCacheProviderUnhandledException(string cacheName, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheDataProviderCallFailure, new object[]
			{
				exception,
				cacheName
			});
			string notificationReason = string.Format("The Cache Data Provider fatally failed for cache {1} with error: {0}.", exception, cacheName);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoCacheDataProvider.UnhandledException", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static void LogCompositeDataProviderDatabaseFailover(string objectType, Exception exception)
		{
			EventLogger.LogPeriodicEvent(FfoHygineDataProviderEventLogConstants.Tuple_CompositeDataProviderDatabaseFailover, exception.Message, new object[]
			{
				objectType,
				exception
			});
			string notificationReason = string.Format("The Composite Data provider is encountered a permanent cache exception and failing over to the Database for type {0}: {1}.", objectType, exception);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoCacheDataProvider.DatabaseFailover", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static void LogCompositeDataProviderCacheFailover(string objectType, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CompositeDataProviderCacheFailover, new object[]
			{
				objectType,
				exception
			});
			string notificationReason = string.Format("The Composite Data provider is encountered a permanent DAL exception and failing over to the Cache for type {0}: {1}.", objectType, exception);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoCacheDataProvider.CacheFailover", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static void LogCacheSerializerPackObjectFallback(string cacheName, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheSerializerPackObjectFallback, new object[]
			{
				cacheName,
				exception
			});
		}

		public static void LogCacheSerializerPackObjectFailure(string cacheName, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheSerializerPackObjectFailure, new object[]
			{
				cacheName,
				exception
			});
		}

		public static void LogCacheSerializerUnpackObjectFailure(string cacheName, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheSerializerUnpackObjectFailure, new object[]
			{
				cacheName,
				exception
			});
		}

		public static void LogCompositeDataProviderCacheUnhealthy(string cacheName)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CompositeDataProviderCacheUnhealthy, new object[]
			{
				cacheName
			});
			string notificationReason = string.Format("The Cache Data Provider determined priming info to be unhealthy for type {0}.", cacheName);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoCacheDataProvider.GetPrimingInfoUnhealthy", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static void LogCacheDataProviderGetPrimingInfoFailure(string cacheName, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_CacheDataProviderGetPrimingInfoFailure, new object[]
			{
				cacheName,
				exception
			});
			string notificationReason = string.Format("The Cache Data Provider unable to determine priming info for type {0}. Error: {1}.", cacheName, exception);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoCacheDataProvider.GetPrimingInfoFailure", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static void LogDomainCacheTrackingError(params object[] messageArgs)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_DomainCacheTrackingError, messageArgs);
		}

		public static void LogBloomFilterDataProviderLoadedNewFile(Type dataType, string newFilePath)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_BloomFilterDataProviderLoadedNewFile, new object[]
			{
				dataType.Name,
				newFilePath
			});
		}

		public static void LogBloomFilterDataProviderNoNewFileFound(Type dataType, DateTime lastUpdateTime)
		{
			EventLogger.LogPeriodicEvent(FfoHygineDataProviderEventLogConstants.Tuple_BloomFilterDataProviderNoNewFileFound, dataType.Name, new object[]
			{
				dataType.Name,
				lastUpdateTime
			});
			if (DateTime.UtcNow - lastUpdateTime > EventLogger.newFileTimeSpan)
			{
				EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "CompositeDataProvider.NoNewBloomFileForExtendedTime", null, string.Format("The CompositeDataProvider has not been able to swap to a new {0} bloom filter file. The last file was loaded at {1}", dataType.Name, lastUpdateTime), ResultSeverityLevel.Error, false);
			}
		}

		public static void LogBloomFilterDataProviderFailureLoadingFile(Type dataType, Exception ex)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_BloomFilterDataProviderFailureLoadingFile, new object[]
			{
				dataType.Name,
				ex
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "CompositeDataProvider.ErrorLoadingNewBloomFile", null, string.Format("The CompositeDataProvider encountered an error swapping to a new {0} bloom filter file: {1}", dataType.Name, ex), ResultSeverityLevel.Error, false);
		}

		public static void LogStaleTracerTokenDetected(Type dataType, string expectedTracer)
		{
			EventLogger.LogPeriodicEvent(FfoHygineDataProviderEventLogConstants.Tuple_BloomFilterDataProviderStaleTracerTokenDetected, dataType.Name + expectedTracer, new object[]
			{
				dataType.Name,
				expectedTracer
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "CompositeDataProvider.StaleTracerTokenDetected", null, string.Format("The CompositeDataProvider encountered a stale tracer token in the {0} bloom filter file. Expected to find key '{1}'.", dataType.Name, expectedTracer), ResultSeverityLevel.Error, false);
		}

		public static void LogInMemoryCachePrimingComplete(string cache, int configCount, int tenantCount)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_InMemoryCachePrimingComplete, new object[]
			{
				cache,
				configCount,
				tenantCount
			});
		}

		public static void LogInMemoryCacheTransientErrorEncountered(string cache, TransientDALException transientError)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_InMemoryCacheTransientErrorEncountered, new object[]
			{
				cache,
				transientError
			});
		}

		public static void LogInMemoryCacheFatalErrorEncounteredDuringPriming(string cache, Exception fatalError)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_InMemoryCachePrimingComplete, new object[]
			{
				cache,
				fatalError
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "InMemoryCache.FatalErrorDuringPriming", null, string.Format("The in-memory cache for {0} data encountered a fatal error during priming of tenant data: {1}", cache, fatalError), ResultSeverityLevel.Error, false);
		}

		public static void LogInMemoryCacheFatalErrorEncounteredDuringRefresh(string cache, object itemKey, Exception fatalError)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_InMemoryCacheFatalErrorEncounteredDuringRefresh, new object[]
			{
				cache,
				itemKey,
				fatalError
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "InMemoryCache.FatalErrorDuringRefresh", null, string.Format("The in-memory cache for {0} data encountered a fatal error during refresh of the item with key {1}: {2}", cache, itemKey, fatalError), ResultSeverityLevel.Error, false);
		}

		public static void LogConnectorPrimingIterationBeginning()
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_InMemoryConnectorCachePrimingIterationBeginning, new object[0]);
		}

		public static void LogConnectorPrimingIterationComplete()
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_InMemoryConnectorCachePrimingIterationComplete, new object[0]);
		}

		public static void LogPolicySyncWebserviceInitialized()
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_PolicySyncWebserviceInitialized, new object[0]);
		}

		public static void LogPolicySyncWebserviceTransientException(string operation, Workload workload, string objectType, SyncCallerContext callerContext, Exception transientException)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_PolicySyncTransientException, new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				transientException
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "PolicySyncWS.TransientException", null, string.Format("The PolicySync Webservice encountered a transient error (operation: {0}, workload: {1}, type: {2}, context: {3}): {4}", new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				transientException
			}), ResultSeverityLevel.Error, false);
		}

		public static void LogPolicySyncWebservicePermanentException(string operation, Workload workload, string objectType, SyncCallerContext callerContext, Exception permanentException)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_PolicySyncPermanentException, new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				permanentException
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "PolicySyncWS.PermanentException", null, string.Format("The PolicySync Webservice encountered a permanent error (operation: {0}, workload: {1}, type: {2}, context: {3}): {4}", new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				permanentException
			}), ResultSeverityLevel.Error, false);
		}

		public static void LogPolicySyncWebserviceUnhandledException(string operation, Workload workload, string objectType, SyncCallerContext callerContext, Exception unhandledException)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_PolicySyncUnhandledException, new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				unhandledException
			});
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "PolicySyncWS.UnhandledException", null, string.Format("The PolicySync Webservice encountered an unhandled error (operation: {0}, workload: {1}, type: {2}, context: {3}): {4}", new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				unhandledException
			}), ResultSeverityLevel.Error, false);
		}

		public static void LogPolicySyncWebserviceTenantNotFound(string operation, Workload workload, string objectType, SyncCallerContext callerContext, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_PolicySyncTenantNotFound, new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				exception
			});
		}

		public static void LogPolicySyncWebserviceGlsError(string operation, Workload workload, string objectType, SyncCallerContext callerContext, Exception exception)
		{
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "PolicySyncWS.GlsError", null, string.Format("The PolicySync Webservice encountered a Gls error (operation: {0}, workload: {1}, type: {2}, context: {3}): {4}", new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				exception
			}), ResultSeverityLevel.Error, false);
		}

		public static void LogPolicySyncWebserviceUnauthorizedAccess(string operation, Workload workload, string objectType, SyncCallerContext callerContext, Exception exception)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_PolicySyncUnauthorizedAccess, new object[]
			{
				operation,
				workload,
				objectType,
				callerContext,
				exception
			});
		}

		public static void LogPartitionMapDatabaseReadError(Exception ex, string localFileName)
		{
			EventLogger.LogEvent(FfoHygineDataProviderEventLogConstants.Tuple_ErrorReadingPartitionMapFromDB, new object[]
			{
				ex.ToString(),
				localFileName
			});
			string notificationReason = string.Format("Error {0} reading Partition Map from DB. Reading Partition Map from Local File {1}", ex, localFileName);
			EventNotificationItem.Publish(ExchangeComponent.Dal.Name, "FfoWebstoreDataProvider.PartitionMapDBRead", null, notificationReason, ResultSeverityLevel.Error, false);
		}

		public static string LogPeriodicalKey(int minute)
		{
			DateTime utcNow = DateTime.UtcNow;
			return string.Format("{0}-{1} {2}:{3}", new object[]
			{
				utcNow.Day,
				utcNow.Month,
				utcNow.Hour,
				utcNow.Minute / minute
			});
		}

		private static void LogEvent(ExEventLog.EventTuple tuple, params object[] messageArgs)
		{
			EventLogger.LogPeriodicEvent(tuple, null, messageArgs);
		}

		private static void LogPeriodicEvent(ExEventLog.EventTuple tuple, string periodicKey, params object[] messageArgs)
		{
			EventLogger.logger.LogEvent(tuple, periodicKey, messageArgs.Select(delegate(object arg)
			{
				if (!(arg is Exception))
				{
					return arg;
				}
				return EventLogger.TrimException((Exception)arg);
			}).ToArray<object>());
		}

		private static object TrimException(Exception exception)
		{
			return new string(exception.ToString().Take(31766).ToArray<char>());
		}

		public const string LogAlways = null;

		public const int LogEveryXMinutes = 1;

		private static TimeSpan newFileTimeSpan = TimeSpan.FromHours(18.0);

		private static Guid componentGuid = Guid.Parse("{4B65DA35-2EAC-4452-B7B7-375D986BCA91}");

		private static ExEventLog logger = new ExEventLog(EventLogger.componentGuid, "FfoDataService");
	}
}
