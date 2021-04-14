using System;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class RmsPerfCounters
	{
		public static void GetPerfCounterInfo(XElement element)
		{
			if (RmsPerfCounters.AllCounters == null)
			{
				return;
			}
			foreach (ExPerformanceCounter exPerformanceCounter in RmsPerfCounters.AllCounters)
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

		public const string CategoryName = "MSExchange Rights Management";

		private static readonly ExPerformanceCounter RateOfSuccessfulCertify = new ExPerformanceCounter("MSExchange Rights Management", "Successful Certify()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageSuccessfulCertifyTime = new ExPerformanceCounter("MSExchange Rights Management", "Average time for successful Certify()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulCertifyTime = new ExPerformanceCounter("MSExchange Rights Management", "Total time for successful Certify()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.AverageSuccessfulCertifyTime
		});

		private static readonly ExPerformanceCounter AverageSuccessfulCertifyTimeBase = new ExPerformanceCounter("MSExchange Rights Management", "Base of Average time for successful Certify()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulCertify = new ExPerformanceCounter("MSExchange Rights Management", "Total successful Certify()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfSuccessfulCertify,
			RmsPerfCounters.AverageSuccessfulCertifyTimeBase
		});

		private static readonly ExPerformanceCounter RateOfFailedCertify = new ExPerformanceCounter("MSExchange Rights Management", "Failed Certify()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedCertify = new ExPerformanceCounter("MSExchange Rights Management", "Total failed Certify()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfFailedCertify
		});

		private static readonly ExPerformanceCounter RateOfSuccessfulGetClientLicensorCert = new ExPerformanceCounter("MSExchange Rights Management", "Successful GetClientLicensorCert()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageSuccessfulGetClientLicensorCertTime = new ExPerformanceCounter("MSExchange Rights Management", "Average time for successful GetClientLicensorCert()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulGetClientLicensorCertTime = new ExPerformanceCounter("MSExchange Rights Management", "Total time for successful GetClientLicensorCert()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.AverageSuccessfulGetClientLicensorCertTime
		});

		private static readonly ExPerformanceCounter AverageSuccessfulGetClientLicensorCertTimeBase = new ExPerformanceCounter("MSExchange Rights Management", "Base of Average time for successful GetClientLicensorCert()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulGetClientLicensorCert = new ExPerformanceCounter("MSExchange Rights Management", "Total successful GetClientLicensorCert()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfSuccessfulGetClientLicensorCert,
			RmsPerfCounters.AverageSuccessfulGetClientLicensorCertTimeBase
		});

		private static readonly ExPerformanceCounter RateOfFailedGetClientLicensorCert = new ExPerformanceCounter("MSExchange Rights Management", "Failed GetClientLicensorCert()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedGetClientLicensorCert = new ExPerformanceCounter("MSExchange Rights Management", "Total failed GetClientLicensorCert()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfFailedGetClientLicensorCert
		});

		private static readonly ExPerformanceCounter RateOfSuccessfulAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Successful AcquireLicense()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageSuccessfulAcquireLicenseTime = new ExPerformanceCounter("MSExchange Rights Management", "Average time for successful AcquireLicense()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulAcquireLicenseTime = new ExPerformanceCounter("MSExchange Rights Management", "Total time for successful AcquireLicense()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.AverageSuccessfulAcquireLicenseTime
		});

		private static readonly ExPerformanceCounter AverageSuccessfulAcquireLicenseTimeBase = new ExPerformanceCounter("MSExchange Rights Management", "Base of Average time for successful AcquireLicense()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Total successful AcquireLicense()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfSuccessfulAcquireLicense,
			RmsPerfCounters.AverageSuccessfulAcquireLicenseTimeBase
		});

		private static readonly ExPerformanceCounter RateOfFailedAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Failed AcquireLicense()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Total failed AcquireLicense()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfFailedAcquireLicense
		});

		private static readonly ExPerformanceCounter RateOfSuccessfulAcquirePreLicense = new ExPerformanceCounter("MSExchange Rights Management", "Successful AcquirePreLicense()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageSuccessfulAcquirePreLicenseTime = new ExPerformanceCounter("MSExchange Rights Management", "Average time for successful AcquirePreLicense()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulAcquirePreLicenseTime = new ExPerformanceCounter("MSExchange Rights Management", "Total time for successful AcquirePreLicense()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.AverageSuccessfulAcquirePreLicenseTime
		});

		private static readonly ExPerformanceCounter AverageSuccessfulAcquirePreLicenseTimeBase = new ExPerformanceCounter("MSExchange Rights Management", "Base of Average time for successful AcquirePreLicense()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulAcquirePreLicense = new ExPerformanceCounter("MSExchange Rights Management", "Total successful AcquirePreLicense()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfSuccessfulAcquirePreLicense,
			RmsPerfCounters.AverageSuccessfulAcquirePreLicenseTimeBase
		});

		private static readonly ExPerformanceCounter RateOfFailedAcquirePreLicense = new ExPerformanceCounter("MSExchange Rights Management", "Failed AcquirePreLicense()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedAcquirePreLicense = new ExPerformanceCounter("MSExchange Rights Management", "Total failed AcquirePreLicense()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfFailedAcquirePreLicense
		});

		private static readonly ExPerformanceCounter RateOfSuccessfulFindServiceLocations = new ExPerformanceCounter("MSExchange Rights Management", "Successful FindServiceLocations()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageSuccessfulFindServiceLocationsTime = new ExPerformanceCounter("MSExchange Rights Management", "Average time for successful FindServiceLocations()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulFindServiceLocationsTime = new ExPerformanceCounter("MSExchange Rights Management", "Total time for successful FindServiceLocations()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.AverageSuccessfulFindServiceLocationsTime
		});

		private static readonly ExPerformanceCounter AverageSuccessfulFindServiceLocationsTimeBase = new ExPerformanceCounter("MSExchange Rights Management", "Base of Average time for successful FindServiceLocations()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulFindServiceLocations = new ExPerformanceCounter("MSExchange Rights Management", "Total successful FindServiceLocations()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfSuccessfulFindServiceLocations,
			RmsPerfCounters.AverageSuccessfulFindServiceLocationsTimeBase
		});

		private static readonly ExPerformanceCounter RateOfFailedFindServiceLocations = new ExPerformanceCounter("MSExchange Rights Management", "Failed FindServiceLocations()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedFindServiceLocations = new ExPerformanceCounter("MSExchange Rights Management", "Total failed FindServiceLocations()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfFailedFindServiceLocations
		});

		private static readonly ExPerformanceCounter RateOfSuccessfulAcquireTemplates = new ExPerformanceCounter("MSExchange Rights Management", "Successful AcquireTemplates()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		private static readonly ExPerformanceCounter AverageSuccessfulAcquireTemplatesTime = new ExPerformanceCounter("MSExchange Rights Management", "Average time for successful AcquireTemplates()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulAcquireTemplatesTime = new ExPerformanceCounter("MSExchange Rights Management", "Total time for successful AcquireTemplates()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.AverageSuccessfulAcquireTemplatesTime
		});

		private static readonly ExPerformanceCounter AverageSuccessfulAcquireTemplatesTimeBase = new ExPerformanceCounter("MSExchange Rights Management", "Base of Average time for successful AcquireTemplates()", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalSuccessfulAcquireTemplates = new ExPerformanceCounter("MSExchange Rights Management", "Total successful AcquireTemplates()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfSuccessfulAcquireTemplates,
			RmsPerfCounters.AverageSuccessfulAcquireTemplatesTimeBase
		});

		private static readonly ExPerformanceCounter RateOfFailedAcquireTemplates = new ExPerformanceCounter("MSExchange Rights Management", "Failed AcquireTemplates()/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalFailedAcquireTemplates = new ExPerformanceCounter("MSExchange Rights Management", "Total failed AcquireTemplates()", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfFailedAcquireTemplates
		});

		private static readonly ExPerformanceCounter RateOfRmsServerInfoCacheHit = new ExPerformanceCounter("MSExchange Rights Management", "Cache-hit of RMS Server Info/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRmsServerInfoCacheHit = new ExPerformanceCounter("MSExchange Rights Management", "Total cache-hit of RMS Server Info", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfRmsServerInfoCacheHit
		});

		private static readonly ExPerformanceCounter RateOfRmsServerInfoCacheMiss = new ExPerformanceCounter("MSExchange Rights Management", "Cache-miss of RMS Server Info/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRmsServerInfoCacheMiss = new ExPerformanceCounter("MSExchange Rights Management", "Total cache-miss of RMS Server Info", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfRmsServerInfoCacheMiss
		});

		private static readonly ExPerformanceCounter RateOfRmsServerInfoCacheAdd = new ExPerformanceCounter("MSExchange Rights Management", "Entries added into RMS Server Info cache/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRmsServerInfoCacheAdd = new ExPerformanceCounter("MSExchange Rights Management", "Total entries added into RMS Server Info cache.", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfRmsServerInfoCacheAdd
		});

		private static readonly ExPerformanceCounter RateOfRmsServerInfoCacheRemove = new ExPerformanceCounter("MSExchange Rights Management", "Entries removed from RMS Server Info cache/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalRmsServerInfoCacheRemove = new ExPerformanceCounter("MSExchange Rights Management", "Total entries removed from RMS Server Info cache", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfRmsServerInfoCacheRemove
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreL1CacheHit = new ExPerformanceCounter("MSExchange Rights Management", "L1 Cache-Hit of RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreL1CacheHit = new ExPerformanceCounter("MSExchange Rights Management", "Total L1 Cache-Hit of Rms License Store", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreL1CacheHit
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreL1CacheMiss = new ExPerformanceCounter("MSExchange Rights Management", "L1 Cache-Miss of RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreL1CacheMiss = new ExPerformanceCounter("MSExchange Rights Management", "Total L1 Cache-Miss of RMS License Store", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreL1CacheMiss
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreL2CacheHit = new ExPerformanceCounter("MSExchange Rights Management", "L2 Cache-Hit of RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreL2CacheHit = new ExPerformanceCounter("MSExchange Rights Management", "Total L2 Cache-Hit of RMS License Store", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreL2CacheHit
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreL2CacheMiss = new ExPerformanceCounter("MSExchange Rights Management", "L2 Cache-Miss of RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreL2CacheMiss = new ExPerformanceCounter("MSExchange Rights Management", "Total L2 Cache-Miss of RMS License Store", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreL2CacheMiss
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreCacheAdd = new ExPerformanceCounter("MSExchange Rights Management", "Entries added into RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreCacheAdd = new ExPerformanceCounter("MSExchange Rights Management", "Total entries added into RMS License Store", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreCacheAdd
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreCacheRemove = new ExPerformanceCounter("MSExchange Rights Management", "Entries removed from RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreCacheRemove = new ExPerformanceCounter("MSExchange Rights Management", "Total entries removed from RMS License Store", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreCacheRemove
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreFileRead = new ExPerformanceCounter("MSExchange Rights Management", "Files read by RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreFileRead = new ExPerformanceCounter("MSExchange Rights Management", "Total files read by RMS License Store from Disk", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreFileRead
		});

		private static readonly ExPerformanceCounter RateOfLicenseStoreFileWrite = new ExPerformanceCounter("MSExchange Rights Management", "Files written by RMS License Store/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalLicenseStoreFileWrite = new ExPerformanceCounter("MSExchange Rights Management", "Total files written by RMS License Store to Disk", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfLicenseStoreFileWrite
		});

		private static readonly ExPerformanceCounter RateOfExternalSuccessfulCertify = new ExPerformanceCounter("MSExchange Rights Management", "Successful External Certification Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExternalSuccessfulCertify = new ExPerformanceCounter("MSExchange Rights Management", "Total external successful certification requests", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfExternalSuccessfulCertify
		});

		private static readonly ExPerformanceCounter RateOfExternalFailedCertify = new ExPerformanceCounter("MSExchange Rights Management", "Failed External Certification Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExternalFailedCertify = new ExPerformanceCounter("MSExchange Rights Management", "Total external failed certification requests", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfExternalFailedCertify
		});

		private static readonly ExPerformanceCounter RateOfExternalSuccessfulAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Successful External AcquireLicense Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExternalSuccessfulAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Total external successful AcquireLicense requests", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfExternalSuccessfulAcquireLicense
		});

		private static readonly ExPerformanceCounter RateOfExternalFailedAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Failed External AcquireLicense Requests/sec", string.Empty, null, new ExPerformanceCounter[0]);

		public static readonly ExPerformanceCounter TotalExternalFailedAcquireLicense = new ExPerformanceCounter("MSExchange Rights Management", "Total external failed AcquireLicense requests", string.Empty, null, new ExPerformanceCounter[]
		{
			RmsPerfCounters.RateOfExternalFailedAcquireLicense
		});

		public static readonly ExPerformanceCounter[] AllCounters = new ExPerformanceCounter[]
		{
			RmsPerfCounters.TotalSuccessfulCertify,
			RmsPerfCounters.TotalSuccessfulCertifyTime,
			RmsPerfCounters.TotalFailedCertify,
			RmsPerfCounters.TotalSuccessfulGetClientLicensorCert,
			RmsPerfCounters.TotalSuccessfulGetClientLicensorCertTime,
			RmsPerfCounters.TotalFailedGetClientLicensorCert,
			RmsPerfCounters.TotalSuccessfulAcquireLicense,
			RmsPerfCounters.TotalSuccessfulAcquireLicenseTime,
			RmsPerfCounters.TotalFailedAcquireLicense,
			RmsPerfCounters.TotalSuccessfulAcquirePreLicense,
			RmsPerfCounters.TotalSuccessfulAcquirePreLicenseTime,
			RmsPerfCounters.TotalFailedAcquirePreLicense,
			RmsPerfCounters.TotalSuccessfulFindServiceLocations,
			RmsPerfCounters.TotalSuccessfulFindServiceLocationsTime,
			RmsPerfCounters.TotalFailedFindServiceLocations,
			RmsPerfCounters.TotalSuccessfulAcquireTemplates,
			RmsPerfCounters.TotalSuccessfulAcquireTemplatesTime,
			RmsPerfCounters.TotalFailedAcquireTemplates,
			RmsPerfCounters.TotalRmsServerInfoCacheHit,
			RmsPerfCounters.TotalRmsServerInfoCacheMiss,
			RmsPerfCounters.TotalRmsServerInfoCacheAdd,
			RmsPerfCounters.TotalRmsServerInfoCacheRemove,
			RmsPerfCounters.TotalLicenseStoreL1CacheHit,
			RmsPerfCounters.TotalLicenseStoreL1CacheMiss,
			RmsPerfCounters.TotalLicenseStoreL2CacheHit,
			RmsPerfCounters.TotalLicenseStoreL2CacheMiss,
			RmsPerfCounters.TotalLicenseStoreCacheAdd,
			RmsPerfCounters.TotalLicenseStoreCacheRemove,
			RmsPerfCounters.TotalLicenseStoreFileRead,
			RmsPerfCounters.TotalLicenseStoreFileWrite,
			RmsPerfCounters.TotalExternalSuccessfulCertify,
			RmsPerfCounters.TotalExternalFailedCertify,
			RmsPerfCounters.TotalExternalSuccessfulAcquireLicense,
			RmsPerfCounters.TotalExternalFailedAcquireLicense
		};
	}
}
