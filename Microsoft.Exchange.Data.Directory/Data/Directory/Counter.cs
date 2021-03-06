using System;

namespace Microsoft.Exchange.Data.Directory
{
	internal enum Counter
	{
		CacheObj,
		CacheHitsRate = 2,
		CacheMissesRate = 4,
		CacheExpiriesUserRate = 6,
		CacheExpiriesConfigRate = 8,
		CacheInsertsUserRate = 10,
		CacheInsertsConfigRate = 12,
		CacheLdapSearchesRate = 14,
		CacheAsyncNotifies = 16,
		CacheAsyncReads = 18,
		CacheAsyncSearches = 20,
		CacheTotalUserEntries = 22,
		CacheTotalConfigEntries = 24,
		CacheUserDnEntries = 26,
		CacheConfigDnEntries = 28,
		CacheUserSrchEntries = 30,
		CacheConfigSrchEntries = 32,
		CacheUserNfdnEntries = 34,
		CacheConfigNfdnEntries = 36,
		CacheUserNfguidEntries = 38,
		CacheConfigNfguidEntries = 40,
		CacheSizeTotalUserEntries = 42,
		CacheSizeTotalConfigEntries = 44,
		CacheSizeUserDnEntries = 46,
		CacheSizeConfigDnEntries = 48,
		CacheSizeUserSrchEntries = 50,
		CacheSizeConfigSrchEntries = 52,
		CacheSizeUserNfdnEntries = 54,
		CacheSizeConfigNfdnEntries = 56,
		CacheSizeUserNfguidEntries = 58,
		CacheSizeConfigNfguidEntries = 60,
		CacheHitsTotal = 62,
		CacheMissesTotal = 64,
		CacheExpiriesUserTotal = 66,
		CacheExpiriesConfigTotal = 68,
		CacheInsertsUserTotal = 70,
		CacheInsertsConfigTotal = 72,
		CacheLdapSearchesTotal = 74,
		ProcessObj = 76,
		ProcessProcessid = 78,
		ProcessRateRead = 80,
		ProcessRateSearch = 82,
		ProcessRateWrite = 84,
		ProcessRatePaged = 86,
		ProcessRateVlv = 88,
		ProcessRateNotFoundConfigReads = 90,
		ProcessRateLongRunningOperations = 92,
		ProcessRateTimeouts = 94,
		ProcessRateNotificationsReceived = 96,
		ProcessRateNotificationsReported = 98,
		ProcessRateCriticalValidationFailures = 100,
		ProcessRateNonCriticalValidationFailures = 102,
		ProcessRateIgnoredValidationFailures = 104,
		ProcessOpenConnectionsDC = 106,
		ProcessOpenConnectionsGC = 108,
		ProcessOutstandingRequests = 110,
		ProcessTopologyVersion = 112,
		ProcessTimeRead = 114,
		ProcessTimeReadBase = 116,
		ProcessTimeSearch = 118,
		ProcessTimeSearchBase = 120,
		ProcessTimeSearchNinetiethPercentile = 122,
		ProcessTimeSearchNinetiethPercentileBase = 124,
		ProcessTimeSearchNinetyFifthPercentile = 126,
		ProcessTimeSearchNinetyFifthPercentileBase = 128,
		ProcessTimeSearchNinetyNinethPercentile = 130,
		ProcessTimeSearchNinetyNinethPercentileBase = 132,
		ProcessTimeSearchOnDC = 134,
		ProcessTimeSearchOnDCBase = 136,
		ProcessCostSearch = 138,
		ProcessCostSearchBase = 140,
		ProcessTimeWrite = 142,
		ProcessTimeWriteBase = 144,
		DCObj = 146,
		DCRateRead = 148,
		DCRateSearch = 150,
		DCRateTimeouts = 152,
		DCRateTimelimitExceeded = 154,
		DCRateFatalErrors = 156,
		DCRateDisconnects = 158,
		DCRateSearchFailures = 160,
		DCRateModificationError = 162,
		DCRateBindFailures = 164,
		DCRateLongRunningOperations = 166,
		DCRatePaged = 168,
		DCRateVlv = 170,
		DCOutstandingRequests = 172,
		DCTimeNetlogon = 174,
		DCTimeGethostbyname = 176,
		DCAvgTimeKerberos = 178,
		DCAvgTimeConnection = 180,
		DCLocalSite = 182,
		DCStateReachability = 184,
		DCStateSynchronized = 186,
		DCStateGCCapable = 188,
		DCStateIsPdc = 190,
		DCStateSaclRight = 192,
		DCStateCriticalData = 194,
		DCStateNetlogon = 196,
		DCStateOsversion = 198,
		DCTimeRead = 200,
		DCTimeReadBase = 202,
		DCTimeSearch = 204,
		DCTimeSearchBase = 206,
		LocalDCObj = 208,
		LocalDCRateRead = 210,
		LocalDCRateSearch = 212,
		LocalDCRateTimeouts = 214,
		LocalDCRateTimelimitExceeded = 216,
		LocalDCRateFatalErrors = 218,
		LocalDCRateDisconnects = 220,
		LocalDCRateSearchFailures = 222,
		LocalDCRateModificationError = 224,
		LocalDCRateBindFailures = 226,
		LocalDCRateLongRunningOperations = 228,
		LocalDCRatePaged = 230,
		LocalDCRateVlv = 232,
		LocalDCOutstandingRequests = 234,
		LocalDCTimeNetlogon = 236,
		LocalDCTimeGethostbyname = 238,
		LocalDCAvgTimeKerberos = 240,
		LocalDCAvgTimeConnection = 242,
		LocalDCStateReachability = 244,
		LocalDCStateSynchronized = 246,
		LocalDCStateGCCapable = 248,
		LocalDCStateIsPdc = 250,
		LocalDCStateSaclRight = 252,
		LocalDCStateCriticalData = 254,
		LocalDCStateNetlogon = 256,
		LocalDCStateOsversion = 258,
		LocalDCTimeRead = 260,
		LocalDCTimeReadBase = 262,
		LocalDCTimeSearch = 264,
		LocalDCTimeSearchBase = 266,
		GlobalObj = 268,
		GlobalTimeDiscovery = 270,
		GlobalTimeDnsquery = 272,
		GlobalDCInSite = 274,
		GlobalGCInSite = 276,
		GlobalDCOutOfSite = 278,
		GlobalGCOutOfSite = 280,
		CacheShortObjEnd = 60,
		CacheShortObjNum = 30,
		CacheLongObjEnd = 74,
		CacheLongObjNum = 7,
		CacheObjNum = 37,
		ProcessShortObjEnd = 112,
		ProcessShortObjNum = 18,
		ProcessSearchObjEnd = 144,
		ProcessSearchObjNum = 8,
		ProcessObjNum = 34,
		DCShortObjEnd = 198,
		DCShortObjNum = 26,
		LocalDCShortObjNum = 25,
		DCSearchObjEnd = 206,
		DCSearchObjNum = 2,
		DCObjNum = 30,
		LocalDCObjNum = 29,
		GlobalObjEnd = 280,
		GlobalObjNum = 6
	}
}
