using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Common.Cache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Threading;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpOutSessionCache : IDisposable
	{
		public SmtpOutSessionCache(int maxEntriesForOutboundProxy, int maxEntriesForNonOutboundProxy, TimeSpan smtpConnectionTimeout, TimeSpan smtpConnectionInactivityTimeout, ICachePerformanceCounters perfCountersForOutboundProxy, ICachePerformanceCounters perfCountersForNonOutboundProxy)
		{
			ArgumentValidator.ThrowIfNegative("maxEntriesForOutboundProxy", maxEntriesForOutboundProxy);
			ArgumentValidator.ThrowIfNegative("maxEntriesForNonOutboundProxy", maxEntriesForNonOutboundProxy);
			if (smtpConnectionTimeout.TotalSeconds < 5.0)
			{
				throw new ArgumentOutOfRangeException("smtpConnectionTimeOut", smtpConnectionTimeout, "SMTP Connection Timeout must be greater than or equal to 5 seconds");
			}
			if (smtpConnectionInactivityTimeout.TotalSeconds < 5.0)
			{
				throw new ArgumentOutOfRangeException("smtpConnectionInactivityTimeOut", smtpConnectionInactivityTimeout, "SMTP Connection Inactivity Timeout must be greater than or equal to 5 seconds");
			}
			this.cachePerfCountersForOutboundProxy = perfCountersForOutboundProxy;
			this.cachePerfCountersForNonOutboundProxy = perfCountersForNonOutboundProxy;
			this.maxCacheEntriesForOutboundProxy = maxEntriesForOutboundProxy;
			this.maxCacheEntriesForNonOutboundProxy = maxEntriesForNonOutboundProxy;
			this.currentCacheEntriesForNonOutboundProxy = 0;
			this.currentCacheEntriesForOutboundProxy = 0;
			this.mruListForNonOutboundProxy = new LinkedList<SmtpOutSessionCache.CacheItemWrapper>();
			this.mruListForOutboundProxy = new LinkedList<SmtpOutSessionCache.CacheItemWrapper>();
			this.connectionTimeout = smtpConnectionTimeout;
			this.connectionInactivityTimeout = smtpConnectionInactivityTimeout;
			this.cacheExpirationCheckInterval = ((smtpConnectionInactivityTimeout < smtpConnectionTimeout) ? smtpConnectionInactivityTimeout : smtpConnectionTimeout);
			this.sessionCache = new Dictionary<MultiValueKey, LinkedList<SmtpOutSessionCache.CacheItemWrapper>>();
			this.expiryTimer = new GuardedTimer(new TimerCallback(this.HandleExpiry), null, this.cacheExpirationCheckInterval, this.cacheExpirationCheckInterval);
		}

		private int GetMaxEntriesByNextHop(NextHopSolutionKey nextHopKey)
		{
			if (SmtpOutSessionCache.OutboundFrontendCacheKey.Equals(nextHopKey))
			{
				return this.maxCacheEntriesForOutboundProxy;
			}
			return this.maxCacheEntriesForNonOutboundProxy;
		}

		private ICachePerformanceCounters GetCachePerfCountersByNextHop(NextHopSolutionKey nextHopKey)
		{
			if (SmtpOutSessionCache.OutboundFrontendCacheKey.Equals(nextHopKey))
			{
				return this.cachePerfCountersForOutboundProxy;
			}
			return this.cachePerfCountersForNonOutboundProxy;
		}

		private int GetCurrentEntriesByNextHop(NextHopSolutionKey nextHopKey)
		{
			if (SmtpOutSessionCache.OutboundFrontendCacheKey.Equals(nextHopKey))
			{
				return this.currentCacheEntriesForOutboundProxy;
			}
			return this.currentCacheEntriesForNonOutboundProxy;
		}

		private void SetCurrentEntriesByNextHop(NextHopSolutionKey nextHopKey, int currentEntries)
		{
			if (SmtpOutSessionCache.OutboundFrontendCacheKey.Equals(nextHopKey))
			{
				this.currentCacheEntriesForOutboundProxy = currentEntries;
				this.cachePerfCountersForOutboundProxy.SizeUpdated((long)currentEntries);
				return;
			}
			this.currentCacheEntriesForNonOutboundProxy = currentEntries;
			this.cachePerfCountersForNonOutboundProxy.SizeUpdated((long)currentEntries);
		}

		private void AddToMRUList(SmtpOutSessionCache.CacheItemWrapper cacheItem)
		{
			NextHopSolutionKey other = (NextHopSolutionKey)cacheItem.ItemKey.GetKey(0);
			if (SmtpOutSessionCache.OutboundFrontendCacheKey.Equals(other))
			{
				this.mruListForOutboundProxy.AddFirst(cacheItem);
				return;
			}
			this.mruListForNonOutboundProxy.AddFirst(cacheItem);
		}

		private void RemoveFromMRUList(SmtpOutSessionCache.CacheItemWrapper cacheItem)
		{
			NextHopSolutionKey other = (NextHopSolutionKey)cacheItem.ItemKey.GetKey(0);
			if (SmtpOutSessionCache.OutboundFrontendCacheKey.Equals(other))
			{
				this.mruListForOutboundProxy.Remove(cacheItem);
				return;
			}
			this.mruListForNonOutboundProxy.Remove(cacheItem);
		}

		private SmtpOutSessionCache.CacheItemWrapper GetLastNodeFromMRUList(NextHopSolutionKey nextHopKey)
		{
			if (SmtpOutSessionCache.OutboundFrontendCacheKey.Equals(nextHopKey))
			{
				return this.mruListForOutboundProxy.Last.Value;
			}
			return this.mruListForNonOutboundProxy.Last.Value;
		}

		public bool TryAdd(NextHopSolutionKey nextHopKey, IPEndPoint remoteEndPoint, SmtpOutSession connection)
		{
			if (nextHopKey.IsEmpty)
			{
				throw new ArgumentException("NextHopKey is Empty", "remoteEndPoint");
			}
			ArgumentValidator.ThrowIfNull("remoteEndPoint", remoteEndPoint);
			ArgumentValidator.ThrowIfNull("connection", connection);
			int maxEntriesByNextHop = this.GetMaxEntriesByNextHop(nextHopKey);
			if (maxEntriesByNextHop == 0)
			{
				return false;
			}
			TimeSpan t = DateTime.UtcNow - connection.SessionStartTime;
			if (t >= this.connectionTimeout || connection.Disconnected)
			{
				return false;
			}
			lock (this.syncObject)
			{
				MultiValueKey multiValueKey = new MultiValueKey(new object[]
				{
					nextHopKey,
					remoteEndPoint
				});
				LinkedListNode<SmtpOutSessionCache.CacheItemWrapper> linkedListNode = new LinkedListNode<SmtpOutSessionCache.CacheItemWrapper>(new SmtpOutSessionCache.CacheItemWrapper(multiValueKey, connection));
				int currentEntriesByNextHop = this.GetCurrentEntriesByNextHop(nextHopKey);
				if (currentEntriesByNextHop + 1 > maxEntriesByNextHop)
				{
					SmtpOutSessionCache.CacheItemWrapper lastNodeFromMRUList = this.GetLastNodeFromMRUList(nextHopKey);
					this.Remove(lastNodeFromMRUList, CacheItemRemovedReason.Scavenged);
				}
				LinkedList<SmtpOutSessionCache.CacheItemWrapper> linkedList;
				if (!this.sessionCache.TryGetValue(multiValueKey, out linkedList))
				{
					linkedList = new LinkedList<SmtpOutSessionCache.CacheItemWrapper>();
					this.sessionCache.Add(multiValueKey, linkedList);
				}
				linkedList.AddFirst(linkedListNode);
				this.SetCurrentEntriesByNextHop(nextHopKey, this.GetCurrentEntriesByNextHop(nextHopKey) + 1);
				this.AddToMRUList(linkedListNode.Value);
				connection.SetNextStateForCachedSessionAndLogInfo(this.GetCurrentEntriesByNextHop(nextHopKey));
			}
			return true;
		}

		public bool TryGetValue(NextHopSolutionKey nextHopKey, IPEndPoint remoteEndPoint, out SmtpOutSession cachedConnection, out string logMessage)
		{
			cachedConnection = null;
			logMessage = null;
			int maxEntriesByNextHop = this.GetMaxEntriesByNextHop(nextHopKey);
			ICachePerformanceCounters cachePerfCountersByNextHop = this.GetCachePerfCountersByNextHop(nextHopKey);
			if (maxEntriesByNextHop == 0)
			{
				return false;
			}
			bool result;
			lock (this.syncObject)
			{
				MultiValueKey key = new MultiValueKey(new object[]
				{
					nextHopKey,
					remoteEndPoint
				});
				LinkedList<SmtpOutSessionCache.CacheItemWrapper> linkedList;
				if (!this.sessionCache.TryGetValue(key, out linkedList) || linkedList == null || linkedList.Count == 0)
				{
					cachePerfCountersByNextHop.Accessed(AccessStatus.Miss);
					logMessage = string.Format("Cache Miss. Current Cache Size {0}", this.GetCurrentEntriesByNextHop(nextHopKey));
					result = false;
				}
				else
				{
					SmtpOutSessionCache.CacheItemWrapper cacheItemWrapper = null;
					foreach (SmtpOutSessionCache.CacheItemWrapper cacheItemWrapper2 in linkedList)
					{
						TimeSpan t = DateTime.UtcNow - cacheItemWrapper2.CacheItem.SessionStartTime;
						TimeSpan t2 = DateTime.UtcNow - cacheItemWrapper2.TimeCached;
						if (!cacheItemWrapper2.CacheItem.Disconnected && t < this.connectionTimeout && t2 < this.connectionInactivityTimeout)
						{
							cacheItemWrapper = cacheItemWrapper2;
							break;
						}
					}
					if (cacheItemWrapper != null)
					{
						this.Remove(cacheItemWrapper, CacheItemRemovedReason.Removed);
						logMessage = string.Format("Cache Hit. Current Cache Size {0}", this.GetCurrentEntriesByNextHop(nextHopKey));
						cachedConnection = cacheItemWrapper.CacheItem;
						cachePerfCountersByNextHop.Accessed(AccessStatus.Hit);
						result = true;
					}
					else
					{
						logMessage = string.Format("Cache Miss. Current Cache Size {0}", this.GetCurrentEntriesByNextHop(nextHopKey));
						cachePerfCountersByNextHop.Accessed(AccessStatus.Miss);
						result = false;
					}
				}
			}
			return result;
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.expiryTimer.Dispose(true);
				this.disposed = true;
			}
		}

		public void RemoveAll(ConnectionCacheRemovalType removeType)
		{
			if (this.maxCacheEntriesForNonOutboundProxy == 0 && this.maxCacheEntriesForOutboundProxy == 0)
			{
				return;
			}
			Dictionary<MultiValueKey, LinkedList<SmtpOutSessionCache.CacheItemWrapper>> dictionary;
			lock (this.syncObject)
			{
				dictionary = this.sessionCache;
				this.sessionCache = new Dictionary<MultiValueKey, LinkedList<SmtpOutSessionCache.CacheItemWrapper>>();
				this.mruListForNonOutboundProxy = new LinkedList<SmtpOutSessionCache.CacheItemWrapper>();
				if (removeType == ConnectionCacheRemovalType.ConfigChange)
				{
					foreach (KeyValuePair<MultiValueKey, LinkedList<SmtpOutSessionCache.CacheItemWrapper>> keyValuePair in dictionary)
					{
						object key = keyValuePair.Key.GetKey(0);
						if (((NextHopSolutionKey)key).Equals(SmtpOutSessionCache.OutboundFrontendCacheKey) && !this.sessionCache.ContainsKey(keyValuePair.Key))
						{
							this.sessionCache.Add(keyValuePair.Key, keyValuePair.Value);
						}
					}
					using (Dictionary<MultiValueKey, LinkedList<SmtpOutSessionCache.CacheItemWrapper>>.Enumerator enumerator2 = this.sessionCache.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							KeyValuePair<MultiValueKey, LinkedList<SmtpOutSessionCache.CacheItemWrapper>> keyValuePair2 = enumerator2.Current;
							dictionary.Remove(keyValuePair2.Key);
						}
						goto IL_121;
					}
				}
				this.currentCacheEntriesForOutboundProxy = 0;
				this.cachePerfCountersForOutboundProxy.SizeUpdated(0L);
				this.mruListForOutboundProxy = new LinkedList<SmtpOutSessionCache.CacheItemWrapper>();
				IL_121:
				this.currentCacheEntriesForNonOutboundProxy = 0;
				this.cachePerfCountersForNonOutboundProxy.SizeUpdated(0L);
			}
			foreach (LinkedList<SmtpOutSessionCache.CacheItemWrapper> linkedList in dictionary.Values)
			{
				foreach (SmtpOutSessionCache.CacheItemWrapper cacheItemWrapper in linkedList)
				{
					if (removeType == ConnectionCacheRemovalType.ConfigChange)
					{
						cacheItemWrapper.CacheItem.SetNextStateToQuit();
					}
					else
					{
						cacheItemWrapper.CacheItem.RemoveConnection();
					}
				}
			}
			dictionary.Clear();
		}

		private void Remove(SmtpOutSessionCache.CacheItemWrapper itemTobeRemoved, CacheItemRemovedReason reason)
		{
			LinkedList<SmtpOutSessionCache.CacheItemWrapper> connectionList;
			if (this.sessionCache.TryGetValue(itemTobeRemoved.ItemKey, out connectionList))
			{
				this.Remove(connectionList, itemTobeRemoved, reason);
			}
		}

		private void Remove(LinkedList<SmtpOutSessionCache.CacheItemWrapper> connectionList, SmtpOutSessionCache.CacheItemWrapper itemTobeRemoved, CacheItemRemovedReason reason)
		{
			if (reason == CacheItemRemovedReason.Scavenged || reason == CacheItemRemovedReason.Expired)
			{
				if (itemTobeRemoved.CacheItem.LogSession != null)
				{
					itemTobeRemoved.CacheItem.LogSession.LogInformation(ProtocolLoggingLevel.Verbose, null, "Connection being removed from Cache. Reason : {0}", new object[]
					{
						reason
					});
				}
				itemTobeRemoved.CacheItem.SetNextStateToQuit();
			}
			connectionList.Remove(itemTobeRemoved);
			if (connectionList.Count == 0)
			{
				this.sessionCache.Remove(itemTobeRemoved.ItemKey);
			}
			this.RemoveFromMRUList(itemTobeRemoved);
			this.SetCurrentEntriesByNextHop((NextHopSolutionKey)itemTobeRemoved.ItemKey.GetKey(0), this.GetCurrentEntriesByNextHop((NextHopSolutionKey)itemTobeRemoved.ItemKey.GetKey(0)) - 1);
		}

		private void HandleExpiry(object state)
		{
			DateTime utcNow = DateTime.UtcNow;
			lock (this.syncObject)
			{
				List<SmtpOutSessionCache.CacheItemWrapper> list = new List<SmtpOutSessionCache.CacheItemWrapper>();
				foreach (LinkedList<SmtpOutSessionCache.CacheItemWrapper> linkedList in this.sessionCache.Values)
				{
					foreach (SmtpOutSessionCache.CacheItemWrapper cacheItemWrapper in linkedList)
					{
						TimeSpan t = utcNow - cacheItemWrapper.CacheItem.SessionStartTime;
						TimeSpan t2 = utcNow - cacheItemWrapper.TimeCached;
						if (t >= this.connectionTimeout || t2 >= this.connectionInactivityTimeout || cacheItemWrapper.CacheItem.Disconnected)
						{
							list.Add(cacheItemWrapper);
						}
					}
				}
				if (list != null && list.Count != 0)
				{
					foreach (SmtpOutSessionCache.CacheItemWrapper cacheItemWrapper2 in list)
					{
						if (!cacheItemWrapper2.CacheItem.Disconnected)
						{
							this.Remove(cacheItemWrapper2, CacheItemRemovedReason.Expired);
						}
						else
						{
							this.Remove(cacheItemWrapper2, CacheItemRemovedReason.Removed);
						}
					}
					list.Clear();
				}
			}
		}

		public void AddDiagnosticInfoTo(XElement cacheElement, bool verbose)
		{
			cacheElement.SetAttributeValue("currentCacheEntriesForOutboundProxy", this.currentCacheEntriesForOutboundProxy);
			cacheElement.SetAttributeValue("currentCacheEntriesForNonOutboundProxy", this.currentCacheEntriesForNonOutboundProxy);
		}

		// Note: this type is marked as 'beforefieldinit'.
		static SmtpOutSessionCache()
		{
			byte[] outboundFrontendIPAddressCacheKey = new byte[4];
			SmtpOutSessionCache.OutboundFrontendIPAddressCacheKey = outboundFrontendIPAddressCacheKey;
			SmtpOutSessionCache.OutboundFrontendPortCacheKey = 25;
			SmtpOutSessionCache.OutboundFrontendIPEndpointCacheKey = new IPEndPoint(new IPAddress(SmtpOutSessionCache.OutboundFrontendIPAddressCacheKey), SmtpOutSessionCache.OutboundFrontendPortCacheKey);
		}

		public static readonly NextHopSolutionKey OutboundFrontendCacheKey = new NextHopSolutionKey(NextHopType.Empty, "Cached:Frontend", Guid.Empty);

		private static readonly byte[] OutboundFrontendIPAddressCacheKey;

		private static readonly int OutboundFrontendPortCacheKey;

		public static readonly IPEndPoint OutboundFrontendIPEndpointCacheKey;

		private readonly TimeSpan cacheExpirationCheckInterval;

		private readonly TimeSpan connectionTimeout;

		private readonly TimeSpan connectionInactivityTimeout;

		private readonly ICachePerformanceCounters cachePerfCountersForOutboundProxy;

		private readonly ICachePerformanceCounters cachePerfCountersForNonOutboundProxy;

		private readonly int maxCacheEntriesForNonOutboundProxy;

		private readonly int maxCacheEntriesForOutboundProxy;

		private int currentCacheEntriesForNonOutboundProxy;

		private int currentCacheEntriesForOutboundProxy;

		private GuardedTimer expiryTimer;

		private bool disposed;

		private object syncObject = new object();

		private Dictionary<MultiValueKey, LinkedList<SmtpOutSessionCache.CacheItemWrapper>> sessionCache;

		private LinkedList<SmtpOutSessionCache.CacheItemWrapper> mruListForNonOutboundProxy;

		private LinkedList<SmtpOutSessionCache.CacheItemWrapper> mruListForOutboundProxy;

		private class CacheItemWrapper
		{
			public CacheItemWrapper(MultiValueKey itemKey, SmtpOutSession cacheItem)
			{
				this.cacheItem = cacheItem;
				this.itemKey = itemKey;
				this.timeCached = DateTime.UtcNow;
			}

			public MultiValueKey ItemKey
			{
				get
				{
					return this.itemKey;
				}
			}

			public SmtpOutSession CacheItem
			{
				get
				{
					return this.cacheItem;
				}
			}

			public DateTime TimeCached
			{
				get
				{
					return this.timeCached;
				}
			}

			private readonly DateTime timeCached;

			private SmtpOutSession cacheItem;

			private MultiValueKey itemKey;
		}

		internal class ConnectionCachePerfCounters : DefaultCachePerformanceCounters
		{
			public ConnectionCachePerfCounters(ProcessTransportRole transportRole, string cachePerfCounterInstance)
			{
				ArgumentValidator.ThrowIfNull("cachePerfCounterInstance", cachePerfCounterInstance);
				if (!ProcessTransportRole.Edge.Equals(transportRole) && !ProcessTransportRole.Hub.Equals(transportRole) && !ProcessTransportRole.FrontEnd.Equals(transportRole) && !ProcessTransportRole.MailboxDelivery.Equals(transportRole) && !ProcessTransportRole.MailboxSubmission.Equals(transportRole))
				{
					throw new ArgumentNotSupportedException("transportRole", "Supplied Transport Role is not supported for these performance counters. [" + transportRole.ToString() + "]");
				}
				try
				{
					SmtpConnectionCachePerfCounters.SetCategoryName(SmtpOutSessionCache.ConnectionCachePerfCounters.perfCounterCategoryMap[transportRole]);
					this.perfCounters = SmtpConnectionCachePerfCounters.GetInstance(cachePerfCounterInstance);
					if (this.perfCounters != null)
					{
						base.InitializeCounters(this.perfCounters.Requests, this.perfCounters.HitRatio, this.perfCounters.HitRatio_Base, this.perfCounters.CacheSize);
					}
				}
				catch (InvalidOperationException ex)
				{
					ExTraceGlobals.GeneralTracer.TraceError<string, InvalidOperationException>(0L, "Failed to initialize performance counters for component '{0}': {1}", cachePerfCounterInstance, ex);
					SmtpCommand.EventLogger.LogEvent(TransportEventLogConstants.Tuple_PerfCountersLoadFailure, null, new object[]
					{
						"SmtpOutSessionCache",
						cachePerfCounterInstance,
						ex.ToString()
					});
					this.perfCounters = null;
				}
			}

			private const string EventTag = "SmtpOutSessionCache";

			private static readonly IDictionary<ProcessTransportRole, string> perfCounterCategoryMap = new Dictionary<ProcessTransportRole, string>
			{
				{
					ProcessTransportRole.Edge,
					"MSExchangeTransport Smtp Connection Cache"
				},
				{
					ProcessTransportRole.Hub,
					"MSExchangeTransport Smtp Connection Cache"
				},
				{
					ProcessTransportRole.FrontEnd,
					"MSExchangeFrontEndTransport Smtp Connection Cache"
				},
				{
					ProcessTransportRole.MailboxDelivery,
					"MSExchange Delivery Smtp Connection Cache"
				},
				{
					ProcessTransportRole.MailboxSubmission,
					"MSExchange Submission Smtp Connection Cache"
				}
			};

			private SmtpConnectionCachePerfCountersInstance perfCounters;
		}
	}
}
