using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Transport.Agent.InterceptorAgent
{
	internal class InterceptorCountersGroup
	{
		internal InterceptorCountersGroup(InterceptorAgentRuleBehavior actions)
		{
			foreach (object obj in Enum.GetValues(typeof(InterceptorAgentRuleBehavior)))
			{
				InterceptorAgentRuleBehavior interceptorAgentRuleBehavior = (InterceptorAgentRuleBehavior)obj;
				if (interceptorAgentRuleBehavior != InterceptorAgentRuleBehavior.NoOp && actions.HasFlag(interceptorAgentRuleBehavior) && InterceptorCountersGroup.ActionToPerfCounterNameMap.ContainsKey(interceptorAgentRuleBehavior))
				{
					this.AddCounterOfName(1073741824 | (int)interceptorAgentRuleBehavior, InterceptorCountersGroup.ActionToPerfCounterNameMap[interceptorAgentRuleBehavior]);
				}
			}
			InterceptorCountersGroup.TrackInstance(this);
		}

		internal InterceptorCountersGroup(InterceptorAgentEvent events)
		{
			foreach (object obj in Enum.GetValues(typeof(InterceptorAgentEvent)))
			{
				InterceptorAgentEvent interceptorAgentEvent = (InterceptorAgentEvent)obj;
				if (interceptorAgentEvent != InterceptorAgentEvent.Invalid && events.HasFlag(interceptorAgentEvent))
				{
					this.AddCounterOfName(268435456 | (int)interceptorAgentEvent, string.Format("{0} {1} Messages", "Evaluated", interceptorAgentEvent));
					this.AddCounterOfName(536870912 | (int)interceptorAgentEvent, string.Format("{0} {1} Messages", "Matched", interceptorAgentEvent));
				}
			}
			InterceptorCountersGroup.TrackInstance(this);
		}

		internal void StopTracking()
		{
			lock (InterceptorCountersGroup.InstancesSyncObject)
			{
				if (InterceptorCountersGroup.Instances.Contains(this))
				{
					InterceptorCountersGroup.Instances.Remove(this);
					if (InterceptorCountersGroup.Instances.Count == 0)
					{
						InterceptorCountersGroup.averageUpdateTimer.Pause();
					}
				}
			}
		}

		internal void Increment(InterceptorAgentRuleBehavior action)
		{
			this.IncrementCounterOfHash(1073741824 | (int)action);
		}

		internal void Increment(InterceptorAgentEvent evt, bool matched)
		{
			this.IncrementCounterOfHash((matched ? 536870912 : 268435456) | (int)evt);
		}

		internal void UpdateAverage()
		{
			foreach (KeyValuePair<int, ICountAndRatePairCounter> keyValuePair in this.ruleCounters)
			{
				keyValuePair.Value.UpdateAverage();
			}
		}

		internal void GetDiagnosticInfo(XElement parent)
		{
			foreach (KeyValuePair<int, ICountAndRatePairCounter> keyValuePair in this.ruleCounters)
			{
				keyValuePair.Value.GetDiagnosticInfo(parent);
			}
		}

		private static void TrackInstance(InterceptorCountersGroup interceptorCountersGroup)
		{
			lock (InterceptorCountersGroup.InstancesSyncObject)
			{
				InterceptorCountersGroup.Instances.Add(interceptorCountersGroup);
				if (InterceptorCountersGroup.averageUpdateTimer == null)
				{
					InterceptorCountersGroup.averageUpdateTimer = new GuardedTimer(new TimerCallback(InterceptorCountersGroup.AverageUpdater), null, 0, 5000);
				}
				else
				{
					InterceptorCountersGroup.averageUpdateTimer.Continue();
				}
			}
		}

		private static void AverageUpdater(object state)
		{
			bool flag = false;
			try
			{
				flag = Monitor.TryEnter(InterceptorCountersGroup.InstancesSyncObject);
				if (flag)
				{
					foreach (KeyValuePair<int, ICountAndRatePairCounter> keyValuePair in InterceptorCountersGroup.TotalCounters)
					{
						keyValuePair.Value.UpdateAverage();
					}
					foreach (InterceptorCountersGroup interceptorCountersGroup in InterceptorCountersGroup.Instances)
					{
						interceptorCountersGroup.UpdateAverage();
					}
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(InterceptorCountersGroup.InstancesSyncObject);
				}
			}
		}

		private void AddCounterOfName(int hashKey, string counterName)
		{
			ICountAndRatePairCounter countAndRatePairCounter = null;
			if (Util.PerfCountersTotalInstance != null && !InterceptorCountersGroup.TotalCounters.TryGetValue(hashKey, out countAndRatePairCounter))
			{
				ExPerformanceCounter counterOfName = Util.PerfCountersTotalInstance.GetCounterOfName(counterName);
				ExPerformanceCounter counterOfName2 = Util.PerfCountersTotalInstance.GetCounterOfName(counterName + " Rate");
				countAndRatePairCounter = new CountAndRatePairWindowsCounter(counterOfName, counterOfName2, InterceptorCountersGroup.TrackingLength, InterceptorCountersGroup.RateDuration, null);
				lock (InterceptorCountersGroup.InstancesSyncObject)
				{
					InterceptorCountersGroup.TotalCounters.Add(hashKey, countAndRatePairCounter);
				}
			}
			string averageCounterName = counterName + " Rate";
			this.ruleCounters.Add(hashKey, new CountAndRatePairMemoryCounter(counterName, averageCounterName, InterceptorCountersGroup.TrackingLength, InterceptorCountersGroup.RateDuration, countAndRatePairCounter));
		}

		private void IncrementCounterOfHash(int hashKey)
		{
			ICountAndRatePairCounter countAndRatePairCounter;
			if (this.ruleCounters.TryGetValue(hashKey, out countAndRatePairCounter))
			{
				countAndRatePairCounter.AddValue(1L);
			}
		}

		private const int InterceptorRateCountersUpdatePeriod = 5000;

		private const int EvaluatedHashMask = 268435456;

		private const int MatchedHashMask = 536870912;

		private const int ActionHashMask = 1073741824;

		private static readonly TimeSpan TrackingLength = TimeSpan.FromMinutes(30.0);

		private static readonly TimeSpan RateDuration = TimeSpan.FromMinutes(1.0);

		private static readonly Dictionary<InterceptorAgentRuleBehavior, string> ActionToPerfCounterNameMap = new Dictionary<InterceptorAgentRuleBehavior, string>
		{
			{
				InterceptorAgentRuleBehavior.Archive,
				"Messages Archived"
			},
			{
				InterceptorAgentRuleBehavior.ArchiveHeaders,
				"Message Headers Archived"
			},
			{
				InterceptorAgentRuleBehavior.Delay,
				"Messages Delayed"
			},
			{
				InterceptorAgentRuleBehavior.PermanentReject,
				"Messages Permanently Rejected"
			},
			{
				InterceptorAgentRuleBehavior.TransientReject,
				"Messages Transiently Rejected"
			},
			{
				InterceptorAgentRuleBehavior.Drop,
				"Messages Dropped"
			},
			{
				InterceptorAgentRuleBehavior.Defer,
				"Messages Deferred"
			}
		};

		private static readonly Dictionary<int, ICountAndRatePairCounter> TotalCounters = new Dictionary<int, ICountAndRatePairCounter>();

		private static readonly object InstancesSyncObject = new object();

		private static readonly List<InterceptorCountersGroup> Instances = new List<InterceptorCountersGroup>();

		private static GuardedTimer averageUpdateTimer;

		private readonly Dictionary<int, ICountAndRatePairCounter> ruleCounters = new Dictionary<int, ICountAndRatePairCounter>();
	}
}
