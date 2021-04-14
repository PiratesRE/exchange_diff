using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.HttpProxy.Common
{
	internal class PerfCounters
	{
		public static HttpProxyCountersInstance HttpProxyCountersInstance
		{
			get
			{
				return PerfCounters.counters.Member;
			}
		}

		public static HttpProxyCacheCountersInstance HttpProxyCacheCountersInstance
		{
			get
			{
				return PerfCounters.cacheCounters.Member;
			}
		}

		public static bool RoutingLatenciesEnabled
		{
			get
			{
				return PerfCounters.RoutingLatencyPerfCountersEnabledAppSettingEntry.Value;
			}
		}

		public static bool RoutingErrorsEnabled
		{
			get
			{
				return PerfCounters.RoutingErrorPerfCountersEnabledAppSettingEntry.Value;
			}
		}

		public static HttpProxyPerSiteCountersInstance GetHttpProxyPerSiteCountersInstance(string siteName)
		{
			if (string.IsNullOrEmpty(siteName))
			{
				siteName = "Unknown";
			}
			string text = HttpProxyGlobals.ProtocolType.ToString() + ";" + siteName;
			ExTraceGlobals.VerboseTracer.TraceDebug<string, bool>(0L, "[PerfCounters::GetHttpProxyPerSiteCountersInstance]: InstanceName = {0}, NeedsInit = {1}", text, !PerfCounters.httpProxyPerSiteInitializedInstances.Contains(text));
			HttpProxyPerSiteCountersInstance instance = HttpProxyPerSiteCounters.GetInstance(text);
			if (!PerfCounters.httpProxyPerSiteInitializedInstances.Contains(text))
			{
				lock (PerfCounters.httpProxyPerSiteInitializedInstances)
				{
					if (!PerfCounters.httpProxyPerSiteInitializedInstances.Contains(text))
					{
						PerfCounters.InitMaps<HttpProxyPerSiteCountersInstance>(instance);
						PerfCounters.httpProxyPerSiteInitializedInstances.Add(text);
					}
				}
			}
			return instance;
		}

		public static void UpdateMovingAveragePerformanceCounter(ExPerformanceCounter performanceCounter, long newValue)
		{
			if (PerfCounters.latencycounterToRunningAverageFloatMap.ContainsKey(performanceCounter))
			{
				RunningAverageFloat runningAverageFloat = PerfCounters.latencycounterToRunningAverageFloatMap[performanceCounter];
				long rawValue = (long)runningAverageFloat.Update((float)newValue);
				performanceCounter.RawValue = rawValue;
			}
		}

		public static void UpdateMovingPercentagePerformanceCounter(ExPerformanceCounter performanceCounter)
		{
			if (PerfCounters.latencycounterToRunningPercentageMap.ContainsKey(performanceCounter))
			{
				RunningPercentage runningPercentage = PerfCounters.latencycounterToRunningPercentageMap[performanceCounter];
				long rawValue = runningPercentage.Update();
				performanceCounter.RawValue = rawValue;
			}
		}

		public static void IncrementMovingPercentagePerformanceCounterBase(ExPerformanceCounter performanceCounter)
		{
			if (PerfCounters.latencycounterToRunningPercentageMap.ContainsKey(performanceCounter))
			{
				RunningPercentage runningPercentage = PerfCounters.latencycounterToRunningPercentageMap[performanceCounter];
				long rawValue = runningPercentage.IncrementBase();
				performanceCounter.RawValue = rawValue;
			}
		}

		internal static void UpdateHttpProxyPerArrayCounters()
		{
			ClientAccessArray localServerClientAccessArray = Server.GetLocalServerClientAccessArray();
			if (localServerClientAccessArray == null)
			{
				PerfCounters.ResetHttpProxyPerArrayCounters(null);
				return;
			}
			HttpProxyPerArrayCountersInstance instance = HttpProxyPerArrayCounters.GetInstance(localServerClientAccessArray.Name);
			if (instance != null)
			{
				PerfCounters.ResetHttpProxyPerArrayCounters(localServerClientAccessArray.Name);
				instance.TotalServersInArray.RawValue = (long)Math.Max(1, localServerClientAccessArray.ServerCount);
			}
		}

		private static void ResetHttpProxyPerArrayCounters(string excludeInstanceName)
		{
			foreach (string text in HttpProxyPerArrayCounters.GetInstanceNames())
			{
				if (!string.Equals(text, excludeInstanceName, StringComparison.OrdinalIgnoreCase))
				{
					HttpProxyPerArrayCounters.ResetInstance(text);
				}
			}
		}

		private static void InitMaps<T>(T instance)
		{
			foreach (FieldInfo fieldInfo in typeof(T).GetFields())
			{
				ExPerformanceCounter exPerformanceCounter = (ExPerformanceCounter)fieldInfo.GetValue(instance);
				exPerformanceCounter.RawValue = 0L;
				if (fieldInfo.Name.StartsWith("MovingAverage") && !PerfCounters.latencycounterToRunningAverageFloatMap.ContainsKey(exPerformanceCounter))
				{
					PerfCounters.latencycounterToRunningAverageFloatMap.Add(exPerformanceCounter, new RunningAverageFloat(200));
				}
				if (fieldInfo.Name.StartsWith("MovingPercentage") && !PerfCounters.latencycounterToRunningPercentageMap.ContainsKey(exPerformanceCounter))
				{
					PerfCounters.latencycounterToRunningPercentageMap.Add(exPerformanceCounter, new RunningPercentage(200));
				}
			}
		}

		[Conditional("DEBUG")]
		private static void AssertOnMissingCounterMap(ExPerformanceCounter performanceCounter, string type)
		{
			throw new ArgumentException(string.Format(type + " counter mapped instance not found : {0}({1})", performanceCounter.CounterName, performanceCounter.InstanceName));
		}

		private const int MovingWindowNumberOfSamples = 200;

		private const string UnknownInstance = "Unknown";

		private static readonly BoolAppSettingsEntry RoutingLatencyPerfCountersEnabledAppSettingEntry = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RoutingLatencyPerfCountersEnabled"), HttpProxyGlobals.IsPartnerHostedOnly || VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.ConfigurePerformanceCounters.Enabled, ExTraceGlobals.VerboseTracer);

		private static readonly BoolAppSettingsEntry RoutingErrorPerfCountersEnabledAppSettingEntry = new BoolAppSettingsEntry(HttpProxySettings.Prefix("RoutingErrorPerfCountersEnabled"), HttpProxyGlobals.IsPartnerHostedOnly || VariantConfiguration.GetSnapshot(MachineSettingsContext.Local, null, null).Cafe.ConfigurePerformanceCounters.Enabled, ExTraceGlobals.VerboseTracer);

		private static Dictionary<ExPerformanceCounter, RunningAverageFloat> latencycounterToRunningAverageFloatMap = new Dictionary<ExPerformanceCounter, RunningAverageFloat>();

		private static Dictionary<ExPerformanceCounter, RunningPercentage> latencycounterToRunningPercentageMap = new Dictionary<ExPerformanceCounter, RunningPercentage>();

		private static HashSet<string> httpProxyPerSiteInitializedInstances = new HashSet<string>();

		private static LazyMember<HttpProxyCountersInstance> counters = new LazyMember<HttpProxyCountersInstance>(delegate()
		{
			HttpProxyCountersInstance instance = HttpProxyCounters.GetInstance(HttpProxyGlobals.ProtocolType.ToString());
			PerfCounters.InitMaps<HttpProxyCountersInstance>(instance);
			return instance;
		});

		private static LazyMember<HttpProxyCacheCountersInstance> cacheCounters = new LazyMember<HttpProxyCacheCountersInstance>(delegate()
		{
			HttpProxyCacheCountersInstance instance = HttpProxyCacheCounters.GetInstance(HttpProxyGlobals.ProtocolType.ToString());
			PerfCounters.InitMaps<HttpProxyCacheCountersInstance>(instance);
			return instance;
		});
	}
}
