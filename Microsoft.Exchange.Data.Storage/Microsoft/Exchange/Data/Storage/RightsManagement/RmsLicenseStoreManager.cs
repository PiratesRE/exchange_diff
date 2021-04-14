using System;
using System.IO;
using System.Security;
using System.Xml;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RmsLicenseStoreManager
	{
		public RmsLicenseStoreManager(string path, int maxCount, RmsPerformanceCounters perfCounters)
		{
			ICachePerformanceCounters cachePerformanceCounters = null;
			if (perfCounters == null)
			{
				this.perfCounters = NoopMruDictionaryPerfCounters.Instance;
			}
			else
			{
				this.perfCounters = perfCounters.LicenseStorePerfCounters;
				cachePerformanceCounters = perfCounters.LicenseStorePerfCounters;
			}
			this.licenseMap = new RmsLicenseStoreInfoMap(path, maxCount, this.perfCounters);
			this.cache = new Cache<MultiValueKey, TenantLicensePair>(RmsClientManager.AppSettings.RacClcCacheSizeInBytes, RmsClientManager.AppSettings.RacClcCacheExpirationInterval, TimeSpan.Zero, null, cachePerformanceCounters);
			this.cache.OnRemoved += RmsLicenseStoreManager.CacheOnRemoved;
		}

		public int Count
		{
			get
			{
				return this.licenseMap.Count;
			}
		}

		public void Clear()
		{
			if (this.cache != null)
			{
				this.cache.Clear();
			}
		}

		public bool WriteToStore(Guid tenantId, Uri url, XmlNode[] rac, byte version)
		{
			if (null == url)
			{
				throw new ArgumentNullException("url");
			}
			if (rac == null || rac.Length < 1 || rac[0] == null)
			{
				throw new ArgumentNullException("rac");
			}
			return this.WriteToStore(tenantId, url, rac, null, version);
		}

		public bool WriteToStore(Guid tenantId, XmlNode[] rac, XmlNode[] clc, byte version)
		{
			if (rac == null || rac.Length < 1 || rac[0] == null)
			{
				throw new ArgumentNullException("rac");
			}
			if (clc == null || clc.Length < 1 || clc[0] == null)
			{
				throw new ArgumentNullException("clc");
			}
			return this.WriteToStore(tenantId, RmsLicenseStoreInfo.DefaultUri, rac, clc, version);
		}

		public bool ReadFromStore(Guid tenantId, Uri serviceLocation, Uri publishLocation, byte version, out TenantLicensePair tenantLicenses)
		{
			if (!this.ReadFromStore(tenantId, RmsLicenseStoreInfo.DefaultUri, version, out tenantLicenses))
			{
				return false;
			}
			RmsLicenseStoreManager.Tracer.TraceDebug<Uri, Uri, int>(0L, "Verifying ServiceLocation {0}, PublishLocation {1} and Version {2}", serviceLocation, publishLocation, (int)version);
			if (tenantLicenses.HasConfigurationChanged(serviceLocation, publishLocation, version))
			{
				MultiValueKey key = new MultiValueKey(new object[]
				{
					tenantId,
					RmsLicenseStoreInfo.DefaultUri
				});
				RmsLicenseStoreManager.Tracer.TraceDebug<Guid>(0L, "Failed to match the RAC/CLC distribution point to the tenant's current configuration. Deleting the current entry. TenantId: {0}", tenantId);
				this.licenseMap.Remove(key);
				this.cache.Remove(key);
				return false;
			}
			RmsLicenseStoreManager.Tracer.TraceDebug(0L, "Distribution points and version not changed");
			return true;
		}

		public bool ReadFromStore(Guid tenantId, Uri url, byte version, out TenantLicensePair tenantLicenses)
		{
			if (null == url)
			{
				throw new ArgumentNullException("url");
			}
			MultiValueKey multiValueKey = new MultiValueKey(new object[]
			{
				tenantId,
				url
			});
			RmsLicenseStoreInfo rmsLicenseStoreInfo;
			if (this.ReadFromCache(multiValueKey, out tenantLicenses))
			{
				this.licenseMap.TryGet(multiValueKey, out rmsLicenseStoreInfo);
				return true;
			}
			if (!this.licenseMap.TryGet(multiValueKey, out rmsLicenseStoreInfo))
			{
				RmsLicenseStoreManager.Tracer.TraceDebug<MultiValueKey>(0L, "License Store Manager doesn't have entry in map for key ({0}) or certs are expired.", multiValueKey);
				return false;
			}
			string text = null;
			string text2 = null;
			try
			{
				text = DrmClientUtils.GetCertFromLicenseStore(rmsLicenseStoreInfo.RacFileName);
				if (!string.IsNullOrEmpty(rmsLicenseStoreInfo.ClcFileName))
				{
					text2 = DrmClientUtils.GetCertFromLicenseStore(rmsLicenseStoreInfo.ClcFileName);
					this.perfCounters.FileRead(2);
				}
				else
				{
					this.perfCounters.FileRead(1);
				}
			}
			catch (IOException arg)
			{
				RmsLicenseStoreManager.Tracer.TraceError<MultiValueKey, IOException>(0L, "License Store Manager failed to read certs from store for key ({0}). IOException - {1}.", multiValueKey, arg);
				return false;
			}
			catch (UnauthorizedAccessException arg2)
			{
				RmsLicenseStoreManager.Tracer.TraceError<MultiValueKey, UnauthorizedAccessException>(0L, "License Store Manager failed to read certs from store for key ({0}). UnauthorizedAccessException - {1}.", multiValueKey, arg2);
				return false;
			}
			catch (SecurityException arg3)
			{
				RmsLicenseStoreManager.Tracer.TraceError<MultiValueKey, SecurityException>(0L, "License Store Manager failed to read certs from store for key ({0}). SecurityException - {1}.", multiValueKey, arg3);
				return false;
			}
			XmlNode[] rac;
			XmlNode[] array;
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2) && RMUtil.TryConvertAppendedCertsToXmlNodeArray(text, out rac) && RMUtil.TryConvertAppendedCertsToXmlNodeArray(text2, out array))
			{
				tenantLicenses = new TenantLicensePair(tenantId, rac, array[0], null, DrmClientUtils.ConvertXmlNodeArrayToCertificateChain(array), rmsLicenseStoreInfo.RacExpire, rmsLicenseStoreInfo.ClcExpire, rmsLicenseStoreInfo.Version, RmsClientManager.EnvironmentHandle, RmsClientManager.LibraryHandle);
				this.cache.TryAdd(multiValueKey, tenantLicenses);
				return true;
			}
			if (!string.IsNullOrEmpty(text) && RMUtil.TryConvertAppendedCertsToXmlNodeArray(text, out rac) && version == rmsLicenseStoreInfo.Version)
			{
				tenantLicenses = new TenantLicensePair(tenantId, rac, null, null, null, rmsLicenseStoreInfo.RacExpire, rmsLicenseStoreInfo.ClcExpire, rmsLicenseStoreInfo.Version, RmsClientManager.EnvironmentHandle, RmsClientManager.LibraryHandle);
				this.cache.TryAdd(multiValueKey, tenantLicenses);
				return true;
			}
			RmsLicenseStoreManager.Tracer.TraceError<MultiValueKey>(0L, "License Store Manager failed to read certs from store for key ({0}) - certs are empty or invalid.", multiValueKey);
			this.licenseMap.Remove(multiValueKey);
			return false;
		}

		private static void CacheOnRemoved(object sender, OnRemovedEventArgs<MultiValueKey, TenantLicensePair> e)
		{
			TenantLicensePair value = e.Value;
			if (value != null)
			{
				value.Release();
			}
		}

		private bool WriteToStore(Guid tenantId, Uri url, XmlNode[] rac, XmlNode[] clc, byte version)
		{
			bool flag = clc == null;
			DateTime dateTime = RMUtil.GetRacExpirationTime(rac[0]);
			DateTime dateTime2 = flag ? DateTime.MaxValue : RMUtil.GetClcExpirationTime(clc[0]);
			if (RmsClientManager.AppSettings.RacClcStoreExpirationInterval.TotalMinutes != 0.0)
			{
				DateTime dateTime3 = DateTime.UtcNow.Add(RmsClientManager.AppSettings.RacClcStoreExpirationInterval);
				dateTime = ((dateTime3 > dateTime) ? dateTime : dateTime3);
				dateTime2 = ((dateTime3 > dateTime2) ? dateTime2 : dateTime3);
			}
			string text = DrmClientUtils.ConvertXmlNodeArrayToCertificateChain(rac);
			string text2 = flag ? null : DrmClientUtils.ConvertXmlNodeArrayToCertificateChain(clc);
			string text3 = null;
			string text4 = null;
			try
			{
				text3 = DrmClientUtils.AddCertToLicenseStore(text, true);
				if (flag)
				{
					this.perfCounters.FileWrite(1);
				}
				else
				{
					text4 = DrmClientUtils.AddCertToLicenseStore(text2, false);
					this.perfCounters.FileWrite(2);
				}
			}
			catch (IOException arg)
			{
				RmsLicenseStoreManager.Tracer.TraceError<Guid, Uri, IOException>(0L, "License Store Manager failed to write RAC-CLC to store for tenant ({0}) and URL ({1}). IOException - {2}.", tenantId, url, arg);
				return false;
			}
			catch (UnauthorizedAccessException arg2)
			{
				RmsLicenseStoreManager.Tracer.TraceError<Guid, Uri, UnauthorizedAccessException>(0L, "License Store Manager failed to write RAC-CLC to store for tenant ({0}) and URL ({1}). UnauthorizedAccessException - {2}.", tenantId, url, arg2);
				return false;
			}
			if (string.IsNullOrEmpty(text3) || (!flag && string.IsNullOrEmpty(text4)))
			{
				RmsLicenseStoreManager.Tracer.TraceError<Guid, Uri>(0L, "License Store Manager failed to write RAC-CLC to store for tenant ({0}) and URL ({1}).", tenantId, url);
				return false;
			}
			RmsLicenseStoreInfo rmsLicenseStoreInfo = new RmsLicenseStoreInfo(tenantId, url, text3, text4, dateTime, dateTime2, version);
			this.licenseMap.Add(rmsLicenseStoreInfo);
			this.cache.TryAdd(new MultiValueKey(new object[]
			{
				rmsLicenseStoreInfo.TenantId,
				rmsLicenseStoreInfo.Url
			}), new TenantLicensePair(rmsLicenseStoreInfo.TenantId, rac, (clc != null) ? clc[0] : null, text, text2, rmsLicenseStoreInfo.RacExpire, rmsLicenseStoreInfo.ClcExpire, version, RmsClientManager.EnvironmentHandle, RmsClientManager.LibraryHandle));
			return true;
		}

		private bool ReadFromCache(MultiValueKey key, out TenantLicensePair tenantLicenses)
		{
			if (this.cache.TryGetValue(key, out tenantLicenses) && tenantLicenses != null)
			{
				DateTime utcNow = DateTime.UtcNow;
				if (!(tenantLicenses.RacExpire < utcNow) && !(tenantLicenses.ClcExpire < utcNow))
				{
					return true;
				}
				this.cache.Remove(key);
				tenantLicenses = null;
			}
			RmsLicenseStoreManager.Tracer.TraceDebug<MultiValueKey>(0L, "License Store Manager failed to get certs from cache for key {0}.", key);
			return false;
		}

		private static readonly Trace Tracer = ExTraceGlobals.RightsManagementTracer;

		private RmsLicenseStoreInfoMap licenseMap;

		private Cache<MultiValueKey, TenantLicensePair> cache;

		private IMruDictionaryPerfCounters perfCounters;
	}
}
