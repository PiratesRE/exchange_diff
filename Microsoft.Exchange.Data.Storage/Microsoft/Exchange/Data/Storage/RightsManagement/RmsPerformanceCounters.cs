using System;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsPerformanceCounters : IWSManagerPerfCounters
	{
		public void Initialize()
		{
			foreach (ExPerformanceCounter exPerformanceCounter in RmsPerfCounters.AllCounters)
			{
				exPerformanceCounter.RawValue = 0L;
			}
		}

		public void CertifySuccessful(long elapsedMilliseconds)
		{
			RmsPerfCounters.TotalSuccessfulCertify.Increment();
			RmsPerfCounters.TotalSuccessfulCertifyTime.IncrementBy(elapsedMilliseconds);
		}

		public void CertifyFailed()
		{
			RmsPerfCounters.TotalFailedCertify.Increment();
		}

		public void GetClientLicensorCertSuccessful(long elapsedMilliseconds)
		{
			RmsPerfCounters.TotalSuccessfulGetClientLicensorCert.Increment();
			RmsPerfCounters.TotalSuccessfulGetClientLicensorCertTime.IncrementBy(elapsedMilliseconds);
		}

		public void GetClientLicensorCertFailed()
		{
			RmsPerfCounters.TotalFailedGetClientLicensorCert.Increment();
		}

		public void AcquireLicenseSuccessful(long elapsedMilliseconds)
		{
			RmsPerfCounters.TotalSuccessfulAcquireLicense.Increment();
			RmsPerfCounters.TotalSuccessfulAcquireLicenseTime.IncrementBy(elapsedMilliseconds);
		}

		public void AcquireLicenseFailed()
		{
			RmsPerfCounters.TotalFailedAcquireLicense.Increment();
		}

		public void AcquirePreLicenseSuccessful(long elapsedMilliseconds)
		{
			RmsPerfCounters.TotalSuccessfulAcquirePreLicense.Increment();
			RmsPerfCounters.TotalSuccessfulAcquirePreLicenseTime.IncrementBy(elapsedMilliseconds);
		}

		public void AcquirePreLicenseFailed()
		{
			RmsPerfCounters.TotalFailedAcquirePreLicense.Increment();
		}

		public void AcquireTemplatesSuccessful(long elapsedMilliseconds)
		{
			RmsPerfCounters.TotalSuccessfulAcquireTemplates.Increment();
			RmsPerfCounters.TotalSuccessfulAcquireTemplatesTime.IncrementBy(elapsedMilliseconds);
		}

		public void AcquireTemplatesFailed()
		{
			RmsPerfCounters.TotalFailedAcquireTemplates.Increment();
		}

		public void FindServiceLocationsSuccessful(long elapsedMilliseconds)
		{
			RmsPerfCounters.TotalSuccessfulFindServiceLocations.Increment();
			RmsPerfCounters.TotalSuccessfulFindServiceLocationsTime.IncrementBy(elapsedMilliseconds);
		}

		public void FindServiceLocationsFailed()
		{
			RmsPerfCounters.TotalFailedFindServiceLocations.Increment();
		}

		public void WCFCertifySuccessful()
		{
			RmsPerfCounters.TotalExternalSuccessfulCertify.Increment();
		}

		public void WCFCertifyFailed()
		{
			RmsPerfCounters.TotalExternalFailedCertify.Increment();
		}

		public void WCFAcquireServerLicenseSuccessful()
		{
			RmsPerfCounters.TotalExternalSuccessfulAcquireLicense.Increment();
		}

		public void WCFAcquireServerLicenseFailed()
		{
			RmsPerfCounters.TotalExternalFailedAcquireLicense.Increment();
		}

		public readonly RmsPerformanceCounters.ServerInfoMapPerformanceCounters ServerInfoMapPerfCounters = new RmsPerformanceCounters.ServerInfoMapPerformanceCounters();

		public readonly RmsPerformanceCounters.LicenseStorePerformanceCounters LicenseStorePerfCounters = new RmsPerformanceCounters.LicenseStorePerformanceCounters();

		public sealed class ServerInfoMapPerformanceCounters : IMruDictionaryPerfCounters
		{
			public void CacheHit()
			{
				RmsPerfCounters.TotalRmsServerInfoCacheHit.Increment();
			}

			public void CacheMiss()
			{
				RmsPerfCounters.TotalRmsServerInfoCacheMiss.Increment();
			}

			public void CacheAdd(bool overwrite, bool remove)
			{
				if (overwrite)
				{
					return;
				}
				if (remove)
				{
					RmsPerfCounters.TotalRmsServerInfoCacheRemove.Increment();
					RmsPerfCounters.TotalRmsServerInfoCacheAdd.Increment();
					return;
				}
				RmsPerfCounters.TotalRmsServerInfoCacheAdd.Increment();
			}

			public void CacheRemove()
			{
				RmsPerfCounters.TotalRmsServerInfoCacheRemove.Increment();
			}

			public void FileRead(int count)
			{
			}

			public void FileWrite(int count)
			{
			}
		}

		public sealed class LicenseStorePerformanceCounters : IMruDictionaryPerfCounters, ICachePerformanceCounters
		{
			public void CacheHit()
			{
				RmsPerfCounters.TotalLicenseStoreL2CacheHit.Increment();
			}

			public void CacheMiss()
			{
				RmsPerfCounters.TotalLicenseStoreL2CacheMiss.Increment();
			}

			public void CacheAdd(bool overwrite, bool remove)
			{
				if (overwrite)
				{
					return;
				}
				if (remove)
				{
					RmsPerfCounters.TotalLicenseStoreCacheRemove.Increment();
					RmsPerfCounters.TotalLicenseStoreCacheAdd.Increment();
					return;
				}
				RmsPerfCounters.TotalLicenseStoreCacheAdd.Increment();
			}

			public void CacheRemove()
			{
				RmsPerfCounters.TotalLicenseStoreCacheRemove.Increment();
			}

			public void FileRead(int count)
			{
				RmsPerfCounters.TotalLicenseStoreFileRead.IncrementBy((long)count);
			}

			public void FileWrite(int count)
			{
				RmsPerfCounters.TotalLicenseStoreFileWrite.IncrementBy((long)count);
			}

			public void Accessed(AccessStatus accessStatus)
			{
				if (accessStatus == AccessStatus.Hit)
				{
					RmsPerfCounters.TotalLicenseStoreL1CacheHit.Increment();
					return;
				}
				RmsPerfCounters.TotalLicenseStoreL1CacheMiss.Increment();
			}

			public void SizeUpdated(long cacheSize)
			{
			}
		}
	}
}
