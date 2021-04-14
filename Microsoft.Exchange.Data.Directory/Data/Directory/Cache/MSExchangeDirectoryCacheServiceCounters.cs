using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Directory.Cache
{
	internal static class MSExchangeDirectoryCacheServiceCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (MSExchangeDirectoryCacheServiceCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in MSExchangeDirectoryCacheServiceCounters.AllCounters)
			{
				try
				{
					element.Add(new XElement(ExPerformanceCounter.GetEncodedName(exPerformanceCounter.CounterName), exPerformanceCounter.NextValue()));
				}
				catch (XmlException ex)
				{
					XElement content = new XElement("Error", ex.Message);
					element.Add(content);
				}
			}
		}

		public const string CategoryName = "MSExchange Directory Cache Service";

		public static readonly ExPerformanceCounter CacheHitRatio = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Hit Ratio", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CacheHitRatioBase = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Hit Ratio Base", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter CacheHit = new ExPerformanceCounter("MSExchange Directory Cache Service", "Percentage of Directory cache hits for the last minute", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter RateOfCacheReadRequest = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Read Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfCacheReadRequests = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Read Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeDirectoryCacheServiceCounters.RateOfCacheReadRequest
		});

		private static readonly ExPerformanceCounter RateOfCacheInsertionRequest = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Insertion Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfCacheInsertionRequests = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Insertion Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeDirectoryCacheServiceCounters.RateOfCacheInsertionRequest
		});

		private static readonly ExPerformanceCounter RateOfCacheRemovalRequest = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Removal Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter NumberOfCacheRemovalRequests = new ExPerformanceCounter("MSExchange Directory Cache Service", "Cache Removal Requests", string.Empty, null, new ExPerformanceCounter[]
		{
			MSExchangeDirectoryCacheServiceCounters.RateOfCacheRemovalRequest
		});

		public static readonly ExPerformanceCounter AcceptedDomainHit = new ExPerformanceCounter("MSExchange Directory Cache Service", "Percentage of AcceptedDomain Cache Hits", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ConfigurationUnitHit = new ExPerformanceCounter("MSExchange Directory Cache Service", "Percentage of ConfigurationUnit Cache Hit", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter RecipientHit = new ExPerformanceCounter("MSExchange Directory Cache Service", "Percentage of Recipient Cache Hit", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ADRawEntryCacheHit = new ExPerformanceCounter("MSExchange Directory Cache Service", "Percentage of ADRawEntry Cache Hit", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter ADRawEntryPropertiesMismatchLastMinute = new ExPerformanceCounter("MSExchange Directory Cache Service", "Percentage of ADRawEntry Cache Properties Mismatch", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			MSExchangeDirectoryCacheServiceCounters.CacheHitRatio,
			MSExchangeDirectoryCacheServiceCounters.CacheHitRatioBase,
			MSExchangeDirectoryCacheServiceCounters.CacheHit,
			MSExchangeDirectoryCacheServiceCounters.NumberOfCacheReadRequests,
			MSExchangeDirectoryCacheServiceCounters.NumberOfCacheInsertionRequests,
			MSExchangeDirectoryCacheServiceCounters.NumberOfCacheRemovalRequests,
			MSExchangeDirectoryCacheServiceCounters.AcceptedDomainHit,
			MSExchangeDirectoryCacheServiceCounters.ConfigurationUnitHit,
			MSExchangeDirectoryCacheServiceCounters.RecipientHit,
			MSExchangeDirectoryCacheServiceCounters.ADRawEntryCacheHit,
			MSExchangeDirectoryCacheServiceCounters.ADRawEntryPropertiesMismatchLastMinute
		};
	}
}
