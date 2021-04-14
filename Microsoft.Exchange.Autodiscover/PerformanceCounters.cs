using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Autodiscover
{
	internal static class PerformanceCounters
	{
		public static void Initialize()
		{
			try
			{
				foreach (ExPerformanceCounter exPerformanceCounter in AutodiscoverPerformanceCounters.AllCounters)
				{
					exPerformanceCounter.RawValue = 0L;
				}
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Autodiscover.ConfigurePerformanceCounters.Enabled)
				{
					AutodiscoverDatacenterPerformanceCounters.RemoveAllInstances();
					AutodiscoverDatacenterPerformanceCounters.ResetInstance(string.Empty);
					string text = ConfigurationManager.AppSettings["TrustedClientsForInstanceBasedPerfCounters"];
					if (!string.IsNullOrEmpty(text))
					{
						PerformanceCounters.trustedClientsList.AddRange(text.Split(new string[]
						{
							";"
						}, StringSplitOptions.RemoveEmptyEntries));
					}
					string text2 = ConfigurationManager.AppSettings["InstanceBasedPerfCounterTimeWindowInterval"];
					if (!string.IsNullOrEmpty(text2))
					{
						double.TryParse(text2, out PerformanceCounters.timerForPerTimeWindowCountersInterval);
					}
					PerformanceCounters.timerForPerTimeWindowCounters.Elapsed += PerformanceCounters.UpdatePerTimeWindowCounters;
					PerformanceCounters.timerForPerTimeWindowCounters.Interval = PerformanceCounters.timerForPerTimeWindowCountersInterval;
					PerformanceCounters.timerForPerTimeWindowCounters.Enabled = true;
				}
				AutodiscoverPerformanceCounters.PID.RawValue = (long)Process.GetCurrentProcess().Id;
				PerformanceCounters.performanceCountersInitialized = true;
			}
			catch (InvalidOperationException ex)
			{
				PerformanceCounters.performanceCountersInitialized = false;
				Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnCorePerfCounterInitializationFailed, Common.PeriodicKey, new object[]
				{
					ex.Message,
					ex.StackTrace
				});
			}
		}

		public static void UpdatePerTimeWindowCounters(object source, ElapsedEventArgs e)
		{
			lock (PerformanceCounters.perClientInstanceTotalPerWindowCounters)
			{
				PerformanceCounters.perClientInstanceTotalPerWindowCounters.Keys.ToList<string>().ForEach(delegate(string key)
				{
					long num = PerformanceCounters.perClientInstanceTotalCountersLastWindowValue[key];
					long rawValue = PerformanceCounters.perClientInstanceTotalCounters[key].RawValue;
					PerformanceCounters.perClientInstanceTotalPerWindowCounters[key].RawValue = rawValue - num;
					PerformanceCounters.perClientInstanceTotalCountersLastWindowValue[key] = rawValue;
				});
			}
		}

		public static void UpdateTotalRequests(bool error)
		{
			PerformanceCounters.SafeUpdatePerfCounter(delegate
			{
				AutodiscoverPerformanceCounters.TotalRequests.Increment();
				if (error)
				{
					AutodiscoverPerformanceCounters.TotalErrorResponses.Increment();
				}
			});
		}

		public static void UpdatePartnerTokenRequests(string userAgent)
		{
			PerformanceCounters.IncrementPerClientInstanceForCounter(userAgent, AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalPartnerTokenRequests, AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalPartnerTokenRequestsPerTimeWindow);
			PerformanceCounters.SafeUpdatePerfCounter(delegate
			{
				AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalPartnerTokenRequests.Increment();
			});
		}

		public static void UpdatePartnerTokenRequestsFailed(string userAgent)
		{
			PerformanceCounters.IncrementPerClientInstanceForCounter(userAgent, AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalPartnerTokenRequestsFailed, AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalPartnerTokenRequestsFailedPerTimeWindow);
			PerformanceCounters.SafeUpdatePerfCounter(delegate
			{
				AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalPartnerTokenRequestsFailed.Increment();
			});
		}

		public static void UpdateCertAuthRequestsFailed(string userAgent)
		{
			PerformanceCounters.IncrementPerClientInstanceForCounter(userAgent, AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalCertAuthRequestsFailed, AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalCertAuthRequestsFailedPerTimeWindow);
			PerformanceCounters.SafeUpdatePerfCounter(delegate
			{
				AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalCertAuthRequestsFailed.Increment();
			});
		}

		public static void UpdateAveragePartnerInfoQueryTime(long latency)
		{
			PerformanceCounters.SafeUpdatePerfCounter(delegate
			{
				AutodiscoverDatacenterPerformanceCounters.TotalInstance.AveragePartnerInfoQueryTime.RawValue = (long)PerformanceCounters.averagePartnerInfoQueryTime.Update((float)latency);
			});
		}

		public static void UpdateRequestsReceivedWithPartnerToken()
		{
			PerformanceCounters.SafeUpdatePerfCounter(delegate
			{
				AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalRequestsReceivedWithPartnerToken.Increment();
			});
		}

		public static void UpdateUnauthorizedRequestsReceivedWithPartnerToken()
		{
			PerformanceCounters.SafeUpdatePerfCounter(delegate
			{
				AutodiscoverDatacenterPerformanceCounters.TotalInstance.TotalUnauthorizedRequestsReceivedWithPartnerToken.Increment();
			});
		}

		private static void SafeUpdatePerfCounter(Action updateAction)
		{
			if (PerformanceCounters.performanceCountersInitialized)
			{
				try
				{
					updateAction();
				}
				catch (InvalidOperationException ex)
				{
					Common.EventLog.LogEvent(AutodiscoverEventLogConstants.Tuple_WarnCorePerfCounterIncrementFailed, Common.PeriodicKey, new object[]
					{
						ex.Message,
						ex.StackTrace
					});
				}
			}
		}

		private static void IncrementPerClientInstanceForCounter(string userAgent, ExPerformanceCounter counter, ExPerformanceCounter associatedWindowCounter)
		{
			if (!string.IsNullOrEmpty(userAgent))
			{
				string client = userAgent.Split(new char[]
				{
					'/'
				})[0];
				if (!string.IsNullOrEmpty(client) && PerformanceCounters.trustedClientsList.Contains(client.ToLower()))
				{
					PerformanceCounters.EnsurePerClientPerfCounterInstancesExist(client, counter, associatedWindowCounter);
					PerformanceCounters.SafeUpdatePerfCounter(delegate
					{
						PerformanceCounters.perClientInstanceTotalCounters[client + "_" + counter.CounterName].Increment();
					});
				}
			}
		}

		private static void EnsurePerClientPerfCounterInstancesExist(string client, ExPerformanceCounter counter, ExPerformanceCounter associatedWindowCounter)
		{
			string key = client + "_" + counter.CounterName;
			if (!PerformanceCounters.perClientInstanceTotalCounters.ContainsKey(key))
			{
				lock (PerformanceCounters.perClientInstanceTotalCounters)
				{
					if (!PerformanceCounters.perClientInstanceTotalCounters.ContainsKey(key))
					{
						lock (PerformanceCounters.perClientInstanceTotalPerWindowCounters)
						{
							PerformanceCounters.perClientInstanceTotalCounters.Add(key, new ExPerformanceCounter("MSExchangeAutodiscover:Datacenter", counter.CounterName, client, null, new ExPerformanceCounter[0]));
							PerformanceCounters.perClientInstanceTotalCounters[key].RawValue = 0L;
							if (associatedWindowCounter != null)
							{
								PerformanceCounters.perClientInstanceTotalPerWindowCounters.Add(key, new ExPerformanceCounter("MSExchangeAutodiscover:Datacenter", associatedWindowCounter.CounterName, client, null, new ExPerformanceCounter[0]));
								PerformanceCounters.perClientInstanceTotalPerWindowCounters[key].RawValue = 0L;
								PerformanceCounters.perClientInstanceTotalCountersLastWindowValue.Add(key, 0L);
							}
						}
					}
				}
			}
		}

		private const string TrustedClientsForInstanceBasedPerfCountersKey = "TrustedClientsForInstanceBasedPerfCounters";

		private const string InstanceBasedPerfCounterTimeWindowInterval = "InstanceBasedPerfCounterTimeWindowInterval";

		private static bool performanceCountersInitialized;

		private static RunningAverageFloat averagePartnerInfoQueryTime = new RunningAverageFloat(25);

		private static Dictionary<string, ExPerformanceCounter> perClientInstanceTotalCounters = new Dictionary<string, ExPerformanceCounter>(10);

		private static Dictionary<string, ExPerformanceCounter> perClientInstanceTotalPerWindowCounters = new Dictionary<string, ExPerformanceCounter>(10);

		private static Dictionary<string, long> perClientInstanceTotalCountersLastWindowValue = new Dictionary<string, long>(10);

		private static List<string> trustedClientsList = new List<string>(5);

		private static Timer timerForPerTimeWindowCounters = new Timer();

		private static double timerForPerTimeWindowCountersInterval = 1000.0;
	}
}
