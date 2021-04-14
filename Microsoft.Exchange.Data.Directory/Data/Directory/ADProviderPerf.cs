using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Data.Directory.Cache;
using Microsoft.Exchange.Data.Directory.GlobalLocatorService;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Data.Directory
{
	internal static class ADProviderPerf
	{
		public static void PrepareDCCountersForRefresh()
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				Globals.InitializeUnknownPerfCounterInstance();
			}
			NativeMethods.DsaccessPerfDCPrepareForRefresh();
		}

		public static void FinalizeDCCountersRefresh()
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				Globals.InitializeUnknownPerfCounterInstance();
			}
			NativeMethods.DsaccessPerfDCFinalizeRefresh();
		}

		public static void AddDCInstance(string serverName)
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				Globals.InitializeUnknownPerfCounterInstance();
			}
			NativeMethods.DsaccessPerfDCAddToList(serverName);
		}

		public static void UpdateProcessCounter(Counter counter, UpdateType updateType, uint value)
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				Globals.InitializeUnknownPerfCounterInstance();
			}
			NativeMethods.DsaccessPerfCounterUpdate(76U, (uint)counter, (uint)updateType, value, null);
		}

		public static void UpdateProcessTimeSearchPercentileCounter(uint value)
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				Globals.InitializeUnknownPerfCounterInstance();
			}
			ADProviderPerf.perProcessPercentileADLatency.AddValue((long)((ulong)value));
			uint value2 = (uint)ADProviderPerf.perProcessPercentileADLatency.PercentileQuery(90.0);
			ADProviderPerf.UpdateProcessCounter(Counter.ProcessTimeSearchNinetiethPercentile, UpdateType.Add, value2);
			ADProviderPerf.UpdateProcessCounter(Counter.ProcessTimeSearchNinetiethPercentileBase, UpdateType.Add, 1U);
			value2 = (uint)ADProviderPerf.perProcessPercentileADLatency.PercentileQuery(95.0);
			ADProviderPerf.UpdateProcessCounter(Counter.ProcessTimeSearchNinetyFifthPercentile, UpdateType.Add, value2);
			ADProviderPerf.UpdateProcessCounter(Counter.ProcessTimeSearchNinetyFifthPercentileBase, UpdateType.Add, 1U);
			value2 = (uint)ADProviderPerf.perProcessPercentileADLatency.PercentileQuery(99.0);
			ADProviderPerf.UpdateProcessCounter(Counter.ProcessTimeSearchNinetyNinethPercentile, UpdateType.Add, value2);
			ADProviderPerf.UpdateProcessCounter(Counter.ProcessTimeSearchNinetyNinethPercentileBase, UpdateType.Add, 1U);
		}

		public static void UpdateDCCounter(string dcName, Counter counter, UpdateType updateType, uint value)
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				Globals.InitializeUnknownPerfCounterInstance();
			}
			NativeMethods.DsaccessPerfCounterUpdate(146U, (uint)counter, (uint)updateType, value, dcName);
		}

		public static void UpdateGlobalCounter(Counter counter, UpdateType updateType, uint value)
		{
			if (Globals.InstanceType == InstanceType.NotInitialized)
			{
				Globals.InitializeUnknownPerfCounterInstance();
			}
			NativeMethods.DsaccessPerfCounterUpdate(268U, (uint)counter, (uint)updateType, value, null);
		}

		public static void UpdateGlsCallLatency(string apiName, bool isRead, int latencyMsec, bool success)
		{
			GlsProcessPerformanceCountersInstance value = ADProviderPerf.processGlsCounters.Value;
			GlsPerformanceCounters.AverageOverallLatency.IncrementBy((long)latencyMsec);
			GlsPerformanceCounters.AverageOverallLatencyBase.Increment();
			if (isRead)
			{
				GlsPerformanceCounters.AverageReadLatency.IncrementBy((long)latencyMsec);
				GlsPerformanceCounters.AverageReadLatencyBase.Increment();
				if (value != null)
				{
					value.AverageReadLatency.IncrementBy((long)latencyMsec);
					value.AverageReadLatencyBase.Increment();
				}
			}
			else
			{
				GlsPerformanceCounters.AverageWriteLatency.IncrementBy((long)latencyMsec);
				GlsPerformanceCounters.AverageWriteLatencyBase.Increment();
				if (value != null)
				{
					value.AverageWriteLatency.IncrementBy((long)latencyMsec);
					value.AverageWriteLatencyBase.Increment();
				}
			}
			if (value != null)
			{
				value.AverageOverallLatency.IncrementBy((long)latencyMsec);
				value.AverageOverallLatencyBase.Increment();
				ADProviderPerf.perProcessPercentileGlsLatency.AddValue((long)latencyMsec);
				uint num = (uint)ADProviderPerf.perProcessPercentileGlsLatency.PercentileQuery(95.0);
				value.NinetyFifthPercentileLatency.IncrementBy((long)((ulong)num));
				value.NinetyFifthPercentileLatencyBase.Increment();
				num = (uint)ADProviderPerf.perProcessPercentileGlsLatency.PercentileQuery(99.0);
				value.NinetyNinthPercentileLatency.IncrementBy((long)((ulong)num));
				value.NinetyNinthPercentileLatencyBase.Increment();
				lock (ADProviderPerf.slidingTotalLockRoot)
				{
					if (success)
					{
						ADProviderPerf.successesPerMinute.AddValue(1L);
					}
					else
					{
						ADProviderPerf.failuresPerMinute.AddValue(1L);
					}
				}
				ADProviderPerf.InitializeTimerIfRequired();
			}
			switch (apiName)
			{
			case "FindTenant":
				GlsApiPerformanceCounters.FindTenantAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.FindTenantAverageOverallLatencyBase.Increment();
				return;
			case "FindDomain":
			case "FindDomains":
				GlsApiPerformanceCounters.FindDomainAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.FindDomainAverageOverallLatencyBase.Increment();
				return;
			case "FindUser":
				GlsApiPerformanceCounters.FindUserAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.FindUserAverageOverallLatencyBase.Increment();
				return;
			case "SaveTenant":
				GlsApiPerformanceCounters.SaveTenantAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.SaveTenantAverageOverallLatencyBase.Increment();
				return;
			case "SaveDomain":
				GlsApiPerformanceCounters.SaveDomainAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.SaveDomainAverageOverallLatencyBase.Increment();
				return;
			case "SaveUser":
				GlsApiPerformanceCounters.SaveUserAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.SaveUserAverageOverallLatencyBase.Increment();
				return;
			case "DeleteTenant":
				GlsApiPerformanceCounters.DeleteTenantAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.DeleteTenantAverageOverallLatencyBase.Increment();
				return;
			case "DeleteDomain":
				GlsApiPerformanceCounters.DeleteDomainAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.DeleteDomainAverageOverallLatencyBase.Increment();
				return;
			case "DeleteUser":
				GlsApiPerformanceCounters.DeleteUserAverageOverallLatency.IncrementBy((long)latencyMsec);
				GlsApiPerformanceCounters.DeleteUserAverageOverallLatencyBase.Increment();
				return;
			}
			throw new ArgumentException("Unknown API " + apiName);
		}

		private static void UpdatePerMinuteCounters(object state)
		{
			GlsProcessPerformanceCountersInstance value = ADProviderPerf.processGlsCounters.Value;
			if (value != null)
			{
				lock (ADProviderPerf.slidingTotalLockRoot)
				{
					value.SuccessfulCallsPerMinute.RawValue = ADProviderPerf.successesPerMinute.Sum;
					value.FailedCallsPerMinute.RawValue = ADProviderPerf.failuresPerMinute.Sum;
					value.NotFoundCallsPerMinute.RawValue = ADProviderPerf.notFoundsPerMinute.Sum;
					value.CacheHitsRatioPerMinute.RawValue = (long)ADProviderPerf.cacheHitsPercentageForLastMinute.GetSlidingPercentage();
					value.AcceptedDomainLookupCacheHitsRatioPerMinute.RawValue = (long)ADProviderPerf.acceptedDomainLookupCacheHitsPercentageForLastMinute.GetSlidingPercentage();
					value.ExternalDirectoryOrganizationIdCacheHitsRatioPerMinute.RawValue = (long)ADProviderPerf.externalDirOrgIdCacheHitsPercentageForLastMinute.GetSlidingPercentage();
					value.MSAUserNetIdCacheHitsRatioPerMinute.RawValue = (long)ADProviderPerf.msaUserNetIdLookupCacheHitsPercentageForLastMinute.GetSlidingPercentage();
				}
			}
			if (ADProviderPerf.directoryCacheHitCounter.IsInitialized && ADProviderPerf.directoryCacheHitCounter.Value != null)
			{
				MSExchangeDirectoryCacheServiceCounters.CacheHit.RawValue = (long)ADProviderPerf.directoryCacheHitCounter.Value.GetSlidingPercentage();
			}
			if (ADProviderPerf.directoryAcceptedDomainCacheHitCounter.IsInitialized && ADProviderPerf.directoryAcceptedDomainCacheHitCounter.Value != null)
			{
				MSExchangeDirectoryCacheServiceCounters.AcceptedDomainHit.RawValue = (long)ADProviderPerf.directoryAcceptedDomainCacheHitCounter.Value.GetSlidingPercentage();
			}
			if (ADProviderPerf.directoryADRawEntryCacheHitCounter.IsInitialized && ADProviderPerf.directoryADRawEntryCacheHitCounter.Value != null)
			{
				MSExchangeDirectoryCacheServiceCounters.ADRawEntryCacheHit.RawValue = (long)ADProviderPerf.directoryADRawEntryCacheHitCounter.Value.GetSlidingPercentage();
			}
			if (ADProviderPerf.directoryADRawEntryPropertiesMisMatchCounter.IsInitialized && ADProviderPerf.directoryADRawEntryPropertiesMisMatchCounter.Value != null)
			{
				MSExchangeDirectoryCacheServiceCounters.ADRawEntryPropertiesMismatchLastMinute.RawValue = (long)ADProviderPerf.directoryADRawEntryPropertiesMisMatchCounter.Value.GetSlidingPercentage();
			}
			if (ADProviderPerf.directoryConfigUnitCacheHitCounter.IsInitialized && ADProviderPerf.directoryConfigUnitCacheHitCounter.Value != null)
			{
				MSExchangeDirectoryCacheServiceCounters.ConfigurationUnitHit.RawValue = (long)ADProviderPerf.directoryConfigUnitCacheHitCounter.Value.GetSlidingPercentage();
			}
			if (ADProviderPerf.directoryRecipientCacheHitCounter.IsInitialized && ADProviderPerf.directoryRecipientCacheHitCounter.Value != null)
			{
				MSExchangeDirectoryCacheServiceCounters.RecipientHit.RawValue = (long)ADProviderPerf.directoryRecipientCacheHitCounter.Value.GetSlidingPercentage();
			}
			if (ADProviderPerf.adDriverCacheHitCounter.IsInitialized && ADProviderPerf.adDriverCacheHitCounter.Value != null)
			{
				MSExchangeADAccessCacheCountersInstance value2 = ADProviderPerf.processADDriverCacheCounters.Value;
				if (value2 != null)
				{
					value2.CacheHit.RawValue = (long)ADProviderPerf.adDriverCacheHitCounter.Value.GetSlidingPercentage();
				}
			}
		}

		public static void IncrementNotFoundCounter()
		{
			lock (ADProviderPerf.slidingTotalLockRoot)
			{
				ADProviderPerf.notFoundsPerMinute.AddValue(1L);
			}
		}

		public static void UpdateGlsCacheHitRatio(GlsLookupKey glsLookupKey, bool cacheHit)
		{
			long numerator = cacheHit ? 1L : 0L;
			ADProviderPerf.cacheHitsPercentageForLastMinute.Add(numerator, 1L);
			switch (glsLookupKey)
			{
			case GlsLookupKey.ExternalDirectoryObjectId:
				ADProviderPerf.externalDirOrgIdCacheHitsPercentageForLastMinute.Add(numerator, 1L);
				return;
			case GlsLookupKey.AcceptedDomain:
				ADProviderPerf.acceptedDomainLookupCacheHitsPercentageForLastMinute.Add(numerator, 1L);
				return;
			case GlsLookupKey.MSAUserNetID:
				ADProviderPerf.msaUserNetIdLookupCacheHitsPercentageForLastMinute.Add(numerator, 1L);
				return;
			default:
				return;
			}
		}

		public static void UpdateDirectoryCacheHitRatio(bool cacheHit, ObjectType objectType)
		{
			ADProviderPerf.directoryCacheHitCounter.Value.AddDenominator(1L);
			if (cacheHit)
			{
				ADProviderPerf.directoryCacheHitCounter.Value.AddNumerator(1L);
			}
			if (objectType <= ObjectType.ActiveSyncMiniRecipient)
			{
				if (objectType <= ObjectType.MiniRecipient)
				{
					switch (objectType)
					{
					case ObjectType.ExchangeConfigurationUnit:
						ADProviderPerf.directoryConfigUnitCacheHitCounter.Value.AddDenominator(1L);
						if (cacheHit)
						{
							ADProviderPerf.directoryConfigUnitCacheHitCounter.Value.AddNumerator(1L);
							goto IL_15D;
						}
						goto IL_15D;
					case ObjectType.Recipient:
						break;
					case ObjectType.ExchangeConfigurationUnit | ObjectType.Recipient:
						goto IL_15D;
					case ObjectType.AcceptedDomain:
						ADProviderPerf.directoryAcceptedDomainCacheHitCounter.Value.AddDenominator(1L);
						if (cacheHit)
						{
							ADProviderPerf.directoryAcceptedDomainCacheHitCounter.Value.AddNumerator(1L);
							goto IL_15D;
						}
						goto IL_15D;
					default:
						if (objectType != ObjectType.MiniRecipient)
						{
							goto IL_15D;
						}
						break;
					}
				}
				else if (objectType != ObjectType.TransportMiniRecipient && objectType != ObjectType.OWAMiniRecipient && objectType != ObjectType.ActiveSyncMiniRecipient)
				{
					goto IL_15D;
				}
			}
			else if (objectType <= ObjectType.StorageMiniRecipient)
			{
				if (objectType != ObjectType.ADRawEntry)
				{
					if (objectType != ObjectType.StorageMiniRecipient)
					{
						goto IL_15D;
					}
				}
				else
				{
					ADProviderPerf.directoryADRawEntryCacheHitCounter.Value.AddDenominator(1L);
					if (cacheHit)
					{
						ADProviderPerf.directoryADRawEntryCacheHitCounter.Value.AddNumerator(1L);
						goto IL_15D;
					}
					goto IL_15D;
				}
			}
			else if (objectType != ObjectType.LoadBalancingMiniRecipient && objectType != ObjectType.MiniRecipientWithTokenGroups && objectType != ObjectType.FrontEndMiniRecipient)
			{
				goto IL_15D;
			}
			ADProviderPerf.directoryRecipientCacheHitCounter.Value.AddDenominator(1L);
			if (cacheHit)
			{
				ADProviderPerf.directoryRecipientCacheHitCounter.Value.AddNumerator(1L);
			}
			IL_15D:
			ADProviderPerf.InitializeTimerIfRequired();
		}

		public static void UpdateDirectoryADRawCachePropertiesMismatchRate(bool mismatch)
		{
			ADProviderPerf.directoryADRawEntryPropertiesMisMatchCounter.Value.AddDenominator(1L);
			if (mismatch)
			{
				ADProviderPerf.directoryADRawEntryPropertiesMisMatchCounter.Value.AddNumerator(1L);
			}
			ADProviderPerf.InitializeTimerIfRequired();
		}

		public static void UpdateADDriverCacheHitRate(bool cacheHit)
		{
			ADProviderPerf.adDriverCacheHitCounter.Value.AddDenominator(1L);
			if (cacheHit)
			{
				ADProviderPerf.adDriverCacheHitCounter.Value.AddNumerator(1L);
			}
			MSExchangeADAccessCacheCountersInstance value = ADProviderPerf.processADDriverCacheCounters.Value;
			if (value != null)
			{
				value.NumberOfCacheRequests.Increment();
			}
			ADProviderPerf.InitializeTimerIfRequired();
		}

		private static void InitializeTimerIfRequired()
		{
			if (ADProviderPerf.updateTimer == null)
			{
				lock (ADProviderPerf.guardedTimerLockRoot)
				{
					if (ADProviderPerf.updateTimer == null)
					{
						ADProviderPerf.updateTimer = new GuardedTimer(new TimerCallback(ADProviderPerf.UpdatePerMinuteCounters), null, ADProviderPerf.TenSeconds, ADProviderPerf.TenSeconds);
					}
				}
			}
		}

		private static GlsProcessPerformanceCountersInstance CreateGlsProcessPerfCountersInstance()
		{
			GlsProcessPerformanceCountersInstance result = null;
			string text = null;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				text = string.Format("{0} {1}{2}{3}", new object[]
				{
					currentProcess.ProcessName,
					Globals.ProcessAppName ?? string.Empty,
					string.IsNullOrEmpty(Globals.ProcessAppName) ? string.Empty : " ",
					Globals.ProcessId
				});
			}
			try
			{
				result = GlsProcessPerformanceCounters.GetInstance(text);
			}
			catch (InvalidOperationException arg)
			{
				ExTraceGlobals.PerfCountersTracer.TraceError<string, InvalidOperationException>(0L, "Get GlsProcessPerformanceCountersInstance {0} failed due to: {1}", text, arg);
			}
			return result;
		}

		private static MSExchangeADAccessCacheCountersInstance CreateADDriverCacheProcessPerfCountersInstance()
		{
			MSExchangeADAccessCacheCountersInstance result = null;
			try
			{
				result = MSExchangeADAccessCacheCounters.GetInstance(Globals.ProcessNameAppName);
			}
			catch (InvalidOperationException arg)
			{
				ExTraceGlobals.PerfCountersTracer.TraceError<string, InvalidOperationException>(0L, "Get MSExchangeADAccessCacheCountersInstance {0} failed due to: {1}", Globals.ProcessNameAppName, arg);
			}
			return result;
		}

		private const double LatencyPercentile90 = 90.0;

		private const double LatencyPercentile95 = 95.0;

		private const double LatencyPercentile99 = 99.0;

		private static readonly PercentileCounter perProcessPercentileADLatency = new PercentileCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0), 10L, 10000L);

		private static readonly PercentileCounter perProcessPercentileGlsLatency = new PercentileCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(30.0), 10L, 10000L);

		private static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1.0);

		private static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10.0);

		private static readonly object slidingTotalLockRoot = new object();

		private static readonly SlidingTotalCounter successesPerMinute = new SlidingTotalCounter(ADProviderPerf.OneMinute, ADProviderPerf.TenSeconds);

		private static readonly SlidingTotalCounter failuresPerMinute = new SlidingTotalCounter(ADProviderPerf.OneMinute, ADProviderPerf.TenSeconds);

		private static readonly SlidingTotalCounter notFoundsPerMinute = new SlidingTotalCounter(ADProviderPerf.OneMinute, ADProviderPerf.TenSeconds);

		private static readonly SlidingPercentageCounter cacheHitsPercentageForLastMinute = new SlidingPercentageCounter(ADProviderPerf.OneMinute, ADProviderPerf.TenSeconds);

		private static readonly SlidingPercentageCounter acceptedDomainLookupCacheHitsPercentageForLastMinute = new SlidingPercentageCounter(ADProviderPerf.OneMinute, ADProviderPerf.TenSeconds);

		private static readonly SlidingPercentageCounter externalDirOrgIdCacheHitsPercentageForLastMinute = new SlidingPercentageCounter(ADProviderPerf.OneMinute, ADProviderPerf.TenSeconds);

		private static readonly SlidingPercentageCounter msaUserNetIdLookupCacheHitsPercentageForLastMinute = new SlidingPercentageCounter(ADProviderPerf.OneMinute, ADProviderPerf.TenSeconds);

		private static readonly LazilyInitialized<GlsProcessPerformanceCountersInstance> processGlsCounters = new LazilyInitialized<GlsProcessPerformanceCountersInstance>(() => ADProviderPerf.CreateGlsProcessPerfCountersInstance());

		private static readonly LazilyInitialized<MSExchangeADAccessCacheCountersInstance> processADDriverCacheCounters = new LazilyInitialized<MSExchangeADAccessCacheCountersInstance>(() => ADProviderPerf.CreateADDriverCacheProcessPerfCountersInstance());

		private static readonly LazilyInitialized<SlidingPercentageCounter> directoryCacheHitCounter = new LazilyInitialized<SlidingPercentageCounter>(() => new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0)));

		private static readonly LazilyInitialized<SlidingPercentageCounter> directoryAcceptedDomainCacheHitCounter = new LazilyInitialized<SlidingPercentageCounter>(() => new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0)));

		private static readonly LazilyInitialized<SlidingPercentageCounter> directoryConfigUnitCacheHitCounter = new LazilyInitialized<SlidingPercentageCounter>(() => new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0)));

		private static readonly LazilyInitialized<SlidingPercentageCounter> directoryRecipientCacheHitCounter = new LazilyInitialized<SlidingPercentageCounter>(() => new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0)));

		private static readonly LazilyInitialized<SlidingPercentageCounter> directoryADRawEntryCacheHitCounter = new LazilyInitialized<SlidingPercentageCounter>(() => new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0)));

		private static readonly LazilyInitialized<SlidingPercentageCounter> directoryADRawEntryPropertiesMisMatchCounter = new LazilyInitialized<SlidingPercentageCounter>(() => new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0)));

		private static readonly LazilyInitialized<SlidingPercentageCounter> adDriverCacheHitCounter = new LazilyInitialized<SlidingPercentageCounter>(() => new SlidingPercentageCounter(TimeSpan.FromMinutes(5.0), TimeSpan.FromSeconds(15.0)));

		private static readonly object guardedTimerLockRoot = new object();

		private static GuardedTimer updateTimer;
	}
}
