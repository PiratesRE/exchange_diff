using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class UnhealthyTargetFilter<K>
	{
		public UnhealthyTargetFilter(UnhealthyTargetFilterConfiguration configuration, IEqualityComparer<K> comparer, Func<ExDateTime> currentTimeGetter)
		{
			this.tracer.TraceFunction(0L, "UnhealthyTargetFilter");
			if (configuration == null)
			{
				throw new ArgumentNullException("configuration");
			}
			this.healthyServerMap = new SynchronizedDictionary<K, UnhealthyTargetFilter<K>.HealthyTargetServerEntry>(100, comparer);
			this.unhealthyServerMap = new SynchronizedDictionary<K, UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry>(100, comparer);
			this.serverDelayMap = new SynchronizedDictionary<K, TimeSpan>(100, comparer);
			if (currentTimeGetter == null)
			{
				currentTimeGetter = (() => ExDateTime.UtcNow);
			}
			this.getCurrentTime = currentTimeGetter;
			this.configuration = configuration;
		}

		public bool Enabled
		{
			get
			{
				return this.configuration.Enabled;
			}
		}

		public void CleanupExpiredEntries()
		{
			this.tracer.TraceFunction(0L, "CleanupExpiredEntries");
			ExDateTime currTime = this.getCurrentTime();
			this.healthyServerMap.RemoveAll((UnhealthyTargetFilter<K>.HealthyTargetServerEntry entry) => entry.IsExpired(currTime));
			this.unhealthyServerMap.RemoveAll((UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry entry) => entry.IsExpired(currTime, this.configuration));
		}

		public bool TryMarkTargetUnhealthyIfNoConnectionOpen(K key, out ExDateTime nextRetryTime, out int currentConnectionCount, out int currentFailureCount)
		{
			this.tracer.TraceFunction<string>(0L, "TryMarkTargetUnhealthyIfNoConnectionOpen arg:{0}", (key != null) ? key.ToString() : "null");
			if (!this.configuration.Enabled)
			{
				currentConnectionCount = int.MinValue;
				nextRetryTime = ExDateTime.MinValue;
				currentFailureCount = 0;
				return false;
			}
			UnhealthyTargetFilter<K>.HealthyTargetServerEntry healthyTargetServerEntry;
			if (!this.healthyServerMap.TryGetValue(key, out healthyTargetServerEntry) || healthyTargetServerEntry.IsPlaceHolder)
			{
				UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry unhealthyTargetServerEntry = this.unhealthyServerMap.AddIfNotExists(key, (K target) => new UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry());
				unhealthyTargetServerEntry.IncrementFailureCount(this.getCurrentTime(), this.configuration);
				ExDateTime retryTime = unhealthyTargetServerEntry.GetRetryTime(this.configuration);
				this.tracer.TraceDebug<K, int, ExDateTime>(0L, "TryMarkTargetUnhealthyIfNoConnectionOpen key:{0} Failure Count:{1} Next Retry Time:{2}", key, unhealthyTargetServerEntry.FailureCount, retryTime);
				currentConnectionCount = 0;
				nextRetryTime = retryTime;
				currentFailureCount = unhealthyTargetServerEntry.FailureCount;
				return true;
			}
			UnhealthyTargetFilter<K>.HealthyTargetServerEntry healthyTargetServerEntry2 = healthyTargetServerEntry;
			currentConnectionCount = healthyTargetServerEntry2.ActiveConnectionsCount;
			currentFailureCount = 0;
			nextRetryTime = this.getCurrentTime();
			healthyTargetServerEntry.SetLastAccessed(nextRetryTime);
			this.tracer.Information<K, int>(0L, "TryMarkTargetUnhealthyIfNoConnectionOpen key:{0} is not marked as unhealthy because it has {1} connection opened", key, currentConnectionCount);
			return false;
		}

		public bool TryMarkTargetInConnectingState(K key)
		{
			bool foundUnhealthyTarget = false;
			bool markedInConnectingState = false;
			this.unhealthyServerMap.ForKey(key, delegate(K targetkey, UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry unhealthyTargetServerEntry)
			{
				if (unhealthyTargetServerEntry != null)
				{
					foundUnhealthyTarget = true;
					markedInConnectingState = unhealthyTargetServerEntry.TryMarkTargetInConnectingState();
				}
			});
			return !foundUnhealthyTarget || markedInConnectingState;
		}

		public void UnMarkTargetInConnectingState(K key)
		{
			this.unhealthyServerMap.ForKey(key, delegate(K targetkey, UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry unhealthyTargetServerEntry)
			{
				if (unhealthyTargetServerEntry != null)
				{
					unhealthyTargetServerEntry.UnMarkTargetInConnectingState();
				}
			});
		}

		public void IncrementConnectionCountToTarget(K key)
		{
			this.tracer.TraceFunction<string>(0L, "IncrementConnectionCountToTarget arg:{0}", (key != null) ? key.ToString() : "null");
			if (!this.configuration.Enabled)
			{
				return;
			}
			UnhealthyTargetFilter<K>.HealthyTargetServerEntry healthyTargetServerEntry = this.healthyServerMap.AddIfNotExists(key, (K target) => new UnhealthyTargetFilter<K>.HealthyTargetServerEntry());
			healthyTargetServerEntry.IncrementActiveConnectionsCount(this.getCurrentTime());
			this.tracer.Information<K, int>(0L, "IncrementConnectionCountToTarget key:{0} New Connection Count {1}.", key, healthyTargetServerEntry.ActiveConnectionsCount);
			this.unhealthyServerMap.Remove(key);
		}

		public void DecrementConnectionCountToTarget(K key)
		{
			this.tracer.TraceFunction<string>(0L, "DecrementConnectionCountToTarget arg:{0}", (key != null) ? key.ToString() : "null");
			if (!this.configuration.Enabled)
			{
				return;
			}
			UnhealthyTargetFilter<K>.HealthyTargetServerEntry healthyTargetServerEntry;
			if (!this.healthyServerMap.TryGetValue(key, out healthyTargetServerEntry) || healthyTargetServerEntry.IsPlaceHolder)
			{
				throw new InvalidOperationException("connection to healthy server can't be negative.");
			}
			UnhealthyTargetFilter<K>.HealthyTargetServerEntry healthyTargetServerEntry2 = healthyTargetServerEntry;
			healthyTargetServerEntry2.DecrementActiveConnectionsCount(this.getCurrentTime());
			this.tracer.Information<K, int>(0L, "DecrementConnectionCountToTarget key:{0} New Connection Count {1}.", key, healthyTargetServerEntry2.ActiveConnectionsCount);
		}

		public List<K> FilterOutUnhealthyTargets(List<K> listToBeFiltered, out bool allUnhealthyEntries)
		{
			return this.FilterOutUnhealthyTargets<K>(listToBeFiltered, (K key) => key, out allUnhealthyEntries);
		}

		public T[] FilterOutUnhealthyTargets<T>(T[] targetsToBeFiltered, Func<T, K> objectToKeyMapper)
		{
			if (targetsToBeFiltered == null || targetsToBeFiltered.Length < 1)
			{
				return targetsToBeFiltered;
			}
			List<T> list = new List<T>(targetsToBeFiltered);
			bool flag;
			List<T> list2 = this.FilterOutUnhealthyTargets<T>(list, objectToKeyMapper, out flag);
			if (list.Count == list2.Count)
			{
				return targetsToBeFiltered;
			}
			return list2.ToArray();
		}

		public List<T> FilterOutUnhealthyTargets<T>(List<T> targetHostToBeFiltered, Func<T, K> objectToKeyMapper, out bool allUnhealthyEntries)
		{
			if (!this.configuration.Enabled || targetHostToBeFiltered == null || targetHostToBeFiltered.Count == 0)
			{
				allUnhealthyEntries = false;
				return targetHostToBeFiltered;
			}
			Dictionary<K, List<T>> keyToTargetHostMap = new Dictionary<K, List<T>>(targetHostToBeFiltered.Count);
			foreach (T t in targetHostToBeFiltered)
			{
				List<T> list = null;
				K key2 = objectToKeyMapper(t);
				if (!keyToTargetHostMap.TryGetValue(key2, out list))
				{
					list = new List<T>(1);
					keyToTargetHostMap.Add(key2, list);
				}
				list.Add(t);
			}
			List<T> healthyServers = null;
			ExDateTime currTime = this.getCurrentTime();
			this.unhealthyServerMap.ForEachKey(keyToTargetHostMap.Keys, null, delegate(K key, UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry unhealthyServerEntry)
			{
				if (unhealthyServerEntry == null || unhealthyServerEntry.ShouldRetryUnhealthyServer(currTime, this.configuration))
				{
					if (healthyServers == null)
					{
						healthyServers = new List<T>(targetHostToBeFiltered.Count);
					}
					healthyServers.AddRange(keyToTargetHostMap[key]);
				}
			});
			allUnhealthyEntries = (healthyServers == null);
			if (healthyServers == null || healthyServers.Count == targetHostToBeFiltered.Count)
			{
				healthyServers = targetHostToBeFiltered;
			}
			return healthyServers;
		}

		public bool IsUnhealthy(K target)
		{
			UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry unhealthyTargetServerEntry;
			return this.unhealthyServerMap.TryGetValue(target, out unhealthyTargetServerEntry) && !unhealthyTargetServerEntry.ShouldRetryUnhealthyServer(this.getCurrentTime(), this.configuration);
		}

		internal void AddDiagnosticInfoTo(XElement unhealthyTargetFiltererElement, bool showVerbose)
		{
			this.tracer.TraceFunction<bool>(0L, "AddDiagnosticInfoTo verbose:{0}", showVerbose);
			if (unhealthyTargetFiltererElement == null)
			{
				throw new ArgumentNullException("unhealthyTargetFiltererElement");
			}
			unhealthyTargetFiltererElement.Add(new XElement("UnhealthyTargetsCount", this.unhealthyServerMap.Count));
			if (showVerbose)
			{
				XElement unhealthyTargetsElement = new XElement("UnhealthyTargets");
				ExDateTime currTime = this.getCurrentTime();
				this.unhealthyServerMap.ForEach(null, delegate(K server, UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry unhealthyServer)
				{
					XElement xelement = new XElement("Server", server.ToString());
					xelement.Add(new object[]
					{
						new XElement("FailureCount", unhealthyServer.FailureCount),
						new XElement("LastRetry", unhealthyServer.LastRetryTime),
						new XElement("Retryable", unhealthyServer.ShouldRetryUnhealthyServer(currTime, this.configuration)),
						new XElement("NextRetryScheduled", unhealthyServer.GetRetryTime(this.configuration))
					});
					unhealthyTargetsElement.Add(xelement);
				});
				XElement healthyTargetsElement = new XElement("HealthyTargets");
				this.healthyServerMap.ForEach(null, delegate(K server, UnhealthyTargetFilter<K>.HealthyTargetServerEntry healthyServer)
				{
					XElement xelement = new XElement("Server", server.ToString());
					xelement.Add(new XElement("ActiveConnectionsCount", healthyServer.ActiveConnectionsCount));
					healthyTargetsElement.Add(xelement);
				});
				this.serverDelayMap.ForEach(null, delegate(K server, TimeSpan delay)
				{
					XElement xelement = new XElement("Server", server.ToString());
					xelement.Add(new XElement("Delay", delay));
					healthyTargetsElement.Add(xelement);
				});
				unhealthyTargetFiltererElement.Add(healthyTargetsElement);
				unhealthyTargetFiltererElement.Add(unhealthyTargetsElement);
			}
		}

		public void UpdateServerLatency(K key, TimeSpan timeSpan)
		{
			this.serverDelayMap[key] = timeSpan;
		}

		public TimeSpan GetServerLatency(K smtpTarget)
		{
			TimeSpan result;
			if (this.serverDelayMap.TryGetValue(smtpTarget, out result))
			{
				return result;
			}
			return TimeSpan.Zero;
		}

		private const int DictionaryInitialCapacity = 100;

		private Func<ExDateTime> getCurrentTime;

		private UnhealthyTargetFilterConfiguration configuration;

		private SynchronizedDictionary<K, UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry> unhealthyServerMap;

		private SynchronizedDictionary<K, UnhealthyTargetFilter<K>.HealthyTargetServerEntry> healthyServerMap;

		private SynchronizedDictionary<K, TimeSpan> serverDelayMap;

		private Trace tracer = ExTraceGlobals.RoutingTracer;

		private sealed class UnhealthyTargetServerEntry
		{
			public int FailureCount
			{
				get
				{
					return this.failureCount;
				}
			}

			public ExDateTime LastRetryTime
			{
				get
				{
					return this.lastRetryTime;
				}
			}

			public bool IsExpired(ExDateTime currTime, UnhealthyTargetFilterConfiguration configuration)
			{
				return this.inConnectingState == 0 && this.GetRetryTime(configuration) + UnhealthyTargetFilter<K>.UnhealthyTargetServerEntry.ExpiryInterval <= currTime;
			}

			public void IncrementFailureCount(ExDateTime currTime, UnhealthyTargetFilterConfiguration configuration)
			{
				int num = this.failureCount + 1;
				ExDateTime retryTime = this.GetRetryTime(configuration);
				if (retryTime < currTime)
				{
					this.lastRetryTime = currTime;
					this.failureCount = num;
				}
				this.inConnectingState = 0;
			}

			public bool ShouldRetryUnhealthyServer(ExDateTime currTime, UnhealthyTargetFilterConfiguration configuration)
			{
				return currTime >= this.GetRetryTime(configuration) && this.inConnectingState == 0;
			}

			public ExDateTime GetRetryTime(UnhealthyTargetFilterConfiguration configuration)
			{
				if (this.failureCount == 0)
				{
					return ExDateTime.MinValue;
				}
				TimeSpan t;
				if (this.failureCount <= configuration.GlitchRetryCount)
				{
					t = configuration.GlitchRetryInterval;
				}
				else if (this.failureCount <= configuration.GlitchRetryCount + configuration.TransientFailureRetryCount)
				{
					t = configuration.TransientFailureRetryInterval;
				}
				else
				{
					t = configuration.OutboundConnectionFailureRetryInterval;
				}
				return this.lastRetryTime + t;
			}

			public bool TryMarkTargetInConnectingState()
			{
				return Interlocked.CompareExchange(ref this.inConnectingState, 1, 0) == 0;
			}

			public void UnMarkTargetInConnectingState()
			{
				this.inConnectingState = 0;
			}

			private static readonly TimeSpan ExpiryInterval = TimeSpan.FromMinutes(10.0);

			private int failureCount;

			private ExDateTime lastRetryTime;

			private int inConnectingState;
		}

		private sealed class HealthyTargetServerEntry
		{
			public bool IsPlaceHolder
			{
				get
				{
					return this.ActiveConnectionsCount == 0;
				}
			}

			public int ActiveConnectionsCount
			{
				get
				{
					return this.activeConnectionsCount;
				}
			}

			public void SetLastAccessed(ExDateTime currTime)
			{
				this.lastAccessed = currTime;
			}

			public bool IsExpired(ExDateTime currTime)
			{
				return this.ActiveConnectionsCount == 0 && this.lastAccessed + UnhealthyTargetFilter<K>.HealthyTargetServerEntry.expiryInterval <= currTime;
			}

			public int IncrementActiveConnectionsCount(ExDateTime currTime)
			{
				this.lastAccessed = currTime;
				return Interlocked.Increment(ref this.activeConnectionsCount);
			}

			public int DecrementActiveConnectionsCount(ExDateTime currTime)
			{
				this.lastAccessed = currTime;
				return Interlocked.Decrement(ref this.activeConnectionsCount);
			}

			private static readonly TimeSpan expiryInterval = TimeSpan.FromMinutes(1.0);

			private int activeConnectionsCount;

			private ExDateTime lastAccessed;
		}
	}
}
