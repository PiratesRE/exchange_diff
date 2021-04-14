using System;
using System.Diagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data
{
	public static class FfoHygineDataProviderEventLogConstants
	{
		public const string EventSource = "FfoDataService";

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceEndpointConfigLoaded = new ExEventLog.EventTuple(1073742824U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceEndpointConfigLoadFailed = new ExEventLog.EventTuple(3221488619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceInvalidServiceTag = new ExEventLog.EventTuple(3221488622U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceRetry = new ExEventLog.EventTuple(2147484658U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceMaxRetry = new ExEventLog.EventTuple(3221226483U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceSlowResponse = new ExEventLog.EventTuple(2147484660U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceFailure = new ExEventLog.EventTuple(3221226485U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceOperationNotAllowed = new ExEventLog.EventTuple(3221488630U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceTypeNotAllowed = new ExEventLog.EventTuple(3221488631U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceFailedToExtractTenantId = new ExEventLog.EventTuple(2147746808U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebServiceUnhandledExceptionDeterminingTenantRegion = new ExEventLog.EventTuple(3221488633U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PolicySyncWebserviceInitialized = new ExEventLog.EventTuple(1073742924U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PolicySyncTransientException = new ExEventLog.EventTuple(2147484749U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PolicySyncPermanentException = new ExEventLog.EventTuple(3221226574U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PolicySyncUnhandledException = new ExEventLog.EventTuple(3221226575U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PolicySyncTenantNotFound = new ExEventLog.EventTuple(2147484752U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_PolicySyncUnauthorizedAccess = new ExEventLog.EventTuple(2147484753U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderRetry = new ExEventLog.EventTuple(2147485148U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderMaxRetry = new ExEventLog.EventTuple(3221226973U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderCallFailure = new ExEventLog.EventTuple(3221226974U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderSlowResponse = new ExEventLog.EventTuple(2147485151U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderFindPageStoredProcMissing = new ExEventLog.EventTuple(2147485152U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CachingSprocCriticalError = new ExEventLog.EventTuple(3221226977U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderConnectionError = new ExEventLog.EventTuple(3221226978U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderMetadataRefreshError = new ExEventLog.EventTuple(3221226979U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_WebstoreDataProviderCorruptDataIgnored = new ExEventLog.EventTuple(3221226980U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CacheDataProviderRetry = new ExEventLog.EventTuple(2147485249U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CacheDataProviderMaxRetry = new ExEventLog.EventTuple(2147485250U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CacheDataProviderCallFailure = new ExEventLog.EventTuple(3221227075U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CacheDataProviderSlowResponse = new ExEventLog.EventTuple(2147485252U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CompositeDataProviderDatabaseFailover = new ExEventLog.EventTuple(3221227077U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CompositeDataProviderCacheFailover = new ExEventLog.EventTuple(3221227078U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CacheSerializerPackObjectFallback = new ExEventLog.EventTuple(3221227079U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CacheSerializerPackObjectFailure = new ExEventLog.EventTuple(3221227080U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_CacheSerializerUnpackObjectFailure = new ExEventLog.EventTuple(3221227081U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CacheDataProviderGetPrimingInfoFailure = new ExEventLog.EventTuple(3221227082U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_CompositeDataProviderCacheUnhealthy = new ExEventLog.EventTuple(3221227083U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BloomFilterDataProviderLoadedNewFile = new ExEventLog.EventTuple(1073743524U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_BloomFilterDataProviderNoNewFileFound = new ExEventLog.EventTuple(1073743525U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_BloomFilterDataProviderFailureLoadingFile = new ExEventLog.EventTuple(3221227174U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_BloomFilterDataProviderStaleTracerTokenDetected = new ExEventLog.EventTuple(2147485351U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InMemoryCachePrimingComplete = new ExEventLog.EventTuple(1073743625U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InMemoryCacheTransientErrorEncountered = new ExEventLog.EventTuple(2147485450U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InMemoryCacheFatalErrorEncounteredDuringPriming = new ExEventLog.EventTuple(3221227275U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_InMemoryCacheFatalErrorEncounteredDuringRefresh = new ExEventLog.EventTuple(3221227276U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InMemoryConnectorCachePrimingIterationBeginning = new ExEventLog.EventTuple(1073743629U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_InMemoryConnectorCachePrimingIterationComplete = new ExEventLog.EventTuple(1073743630U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DalWSServerUnknowError = new ExEventLog.EventTuple(3221489617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PerfmonInstanceErrorFatal = new ExEventLog.EventTuple(3221490617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DalWebServiceStarted = new ExEventLog.EventTuple(1073746824U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_DalWebServiceStopped = new ExEventLog.EventTuple(1073746825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StoreProcReturnedSuccessfully = new ExEventLog.EventTuple(1073748828U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_StoreProcInvoked = new ExEventLog.EventTuple(1073748827U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DalExceptionThrown = new ExEventLog.EventTuple(2147490650U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnsupportedFFOAPICalled = new ExEventLog.EventTuple(1073748825U, 1, EventLogEntryType.Information, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_AuditUserIdentityMissing = new ExEventLog.EventTuple(2147490653U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnsupportedQueryFilter = new ExEventLog.EventTuple(2147490654U, 1, EventLogEntryType.Warning, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_DomainCacheTrackingError = new ExEventLog.EventTuple(3221232479U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_TransientExceptionWhenQueryGLS = new ExEventLog.EventTuple(3221495617U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_UnknownExceptionWhenQueryGLS = new ExEventLog.EventTuple(3221495618U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogOneTime")]
		public static readonly ExEventLog.EventTuple Tuple_FailedToRetrieveRegionTagWhenQueryGLS = new ExEventLog.EventTuple(3221495619U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogOneTime);

		[EventLogPeriod(Period = "LogPeriodic")]
		public static readonly ExEventLog.EventTuple Tuple_PermanentExceptionWhenQueryGLS = new ExEventLog.EventTuple(3221495620U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogPeriodic);

		[EventLogPeriod(Period = "LogAlways")]
		public static readonly ExEventLog.EventTuple Tuple_ErrorReadingPartitionMapFromDB = new ExEventLog.EventTuple(3221495621U, 1, EventLogEntryType.Error, ExEventLog.EventLevel.Lowest, ExEventLog.EventPeriod.LogAlways);

		private enum Category : short
		{
			General = 1
		}

		internal enum Message : uint
		{
			WebServiceEndpointConfigLoaded = 1073742824U,
			WebServiceEndpointConfigLoadFailed = 3221488619U,
			WebServiceInvalidServiceTag = 3221488622U,
			WebServiceRetry = 2147484658U,
			WebServiceMaxRetry = 3221226483U,
			WebServiceSlowResponse = 2147484660U,
			WebServiceFailure = 3221226485U,
			WebServiceOperationNotAllowed = 3221488630U,
			WebServiceTypeNotAllowed,
			WebServiceFailedToExtractTenantId = 2147746808U,
			WebServiceUnhandledExceptionDeterminingTenantRegion = 3221488633U,
			PolicySyncWebserviceInitialized = 1073742924U,
			PolicySyncTransientException = 2147484749U,
			PolicySyncPermanentException = 3221226574U,
			PolicySyncUnhandledException,
			PolicySyncTenantNotFound = 2147484752U,
			PolicySyncUnauthorizedAccess,
			WebstoreDataProviderRetry = 2147485148U,
			WebstoreDataProviderMaxRetry = 3221226973U,
			WebstoreDataProviderCallFailure,
			WebstoreDataProviderSlowResponse = 2147485151U,
			WebstoreDataProviderFindPageStoredProcMissing,
			CachingSprocCriticalError = 3221226977U,
			WebstoreDataProviderConnectionError,
			WebstoreDataProviderMetadataRefreshError,
			WebstoreDataProviderCorruptDataIgnored,
			CacheDataProviderRetry = 2147485249U,
			CacheDataProviderMaxRetry,
			CacheDataProviderCallFailure = 3221227075U,
			CacheDataProviderSlowResponse = 2147485252U,
			CompositeDataProviderDatabaseFailover = 3221227077U,
			CompositeDataProviderCacheFailover,
			CacheSerializerPackObjectFallback,
			CacheSerializerPackObjectFailure,
			CacheSerializerUnpackObjectFailure,
			CacheDataProviderGetPrimingInfoFailure,
			CompositeDataProviderCacheUnhealthy,
			BloomFilterDataProviderLoadedNewFile = 1073743524U,
			BloomFilterDataProviderNoNewFileFound,
			BloomFilterDataProviderFailureLoadingFile = 3221227174U,
			BloomFilterDataProviderStaleTracerTokenDetected = 2147485351U,
			InMemoryCachePrimingComplete = 1073743625U,
			InMemoryCacheTransientErrorEncountered = 2147485450U,
			InMemoryCacheFatalErrorEncounteredDuringPriming = 3221227275U,
			InMemoryCacheFatalErrorEncounteredDuringRefresh,
			InMemoryConnectorCachePrimingIterationBeginning = 1073743629U,
			InMemoryConnectorCachePrimingIterationComplete,
			DalWSServerUnknowError = 3221489617U,
			PerfmonInstanceErrorFatal = 3221490617U,
			DalWebServiceStarted = 1073746824U,
			DalWebServiceStopped,
			StoreProcReturnedSuccessfully = 1073748828U,
			StoreProcInvoked = 1073748827U,
			DalExceptionThrown = 2147490650U,
			UnsupportedFFOAPICalled = 1073748825U,
			AuditUserIdentityMissing = 2147490653U,
			UnsupportedQueryFilter,
			DomainCacheTrackingError = 3221232479U,
			TransientExceptionWhenQueryGLS = 3221495617U,
			UnknownExceptionWhenQueryGLS,
			FailedToRetrieveRegionTagWhenQueryGLS,
			PermanentExceptionWhenQueryGLS,
			ErrorReadingPartitionMapFromDB
		}
	}
}
