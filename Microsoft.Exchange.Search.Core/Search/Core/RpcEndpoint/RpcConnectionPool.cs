using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Search;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Core.RpcEndpoint
{
	internal class RpcConnectionPool
	{
		private RpcConnectionPool()
		{
			RpcConnectionPool.tracingContext = this.GetHashCode();
			this.searchConfig = SearchConfig.Instance;
			RpcConnectionPool.maxCacheSize = this.searchConfig.DocumentTrackingMaxClientCacheSize;
			this.rpcClientCache = new ExactTimeoutCache<int, SearchServiceRpcClient>(new RemoveItemDelegate<int, SearchServiceRpcClient>(RpcConnectionPool.RemoveFromCacheCallback), null, null, RpcConnectionPool.maxCacheSize, true, CacheFullBehavior.ExpireExisting);
		}

		internal static SearchServiceRpcClient GetSearchRpcClient()
		{
			SearchServiceRpcClient searchServiceRpcClient;
			lock (RpcConnectionPool.lockObject)
			{
				if (!RpcConnectionPool.TryGetRpcClientFromCache(out searchServiceRpcClient))
				{
					RpcConnectionPool.tracer.TraceDebug((long)RpcConnectionPool.tracingContext, "RpcConnectionPool: Creating new RpcClient.");
					searchServiceRpcClient = new SearchServiceRpcClient("localhost");
				}
			}
			if (searchServiceRpcClient == null)
			{
				throw new InvalidOperationException("RPC client pool returned null client.");
			}
			return searchServiceRpcClient;
		}

		internal static void ReturnSearchRpcClientToCache(ref SearchServiceRpcClient rpcClient, bool discard)
		{
			if (rpcClient == null)
			{
				throw new ArgumentNullException("rpcClient");
			}
			bool flag = false;
			try
			{
				if (!discard)
				{
					lock (RpcConnectionPool.lockObject)
					{
						if (RpcConnectionPool.instance.rpcClientCache.Count < RpcConnectionPool.maxCacheSize)
						{
							int key = Interlocked.Increment(ref RpcConnectionPool.globalCacheId);
							flag = RpcConnectionPool.instance.rpcClientCache.TryAddSliding(key, rpcClient, RpcConnectionPool.instance.searchConfig.DocumentTrackingClientCacheTimeout);
						}
						if (flag)
						{
							RpcConnectionPool.tracer.TraceDebug<int>((long)RpcConnectionPool.tracingContext, "RpcConnectionPool: RpcClient returned to the cache. Current Cache size: {0}", RpcConnectionPool.instance.rpcClientCache.Count);
						}
						else
						{
							RpcConnectionPool.tracer.TraceDebug<int>((long)RpcConnectionPool.tracingContext, "RpcConnectionPool: RpcClient discarded - Cache Full: {0}", RpcConnectionPool.instance.rpcClientCache.Count);
						}
						goto IL_D8;
					}
				}
				RpcConnectionPool.tracer.TraceDebug((long)RpcConnectionPool.tracingContext, "RpcConnectionPool: RpcClient discarded - User Instructed.");
				IL_D8:;
			}
			finally
			{
				if (!flag)
				{
					rpcClient.Dispose();
					rpcClient = null;
				}
			}
		}

		internal static void RegisterCaller()
		{
			Interlocked.Increment(ref RpcConnectionPool.currentReferenceCount);
		}

		internal static void UnRegisterCaller()
		{
			if (Interlocked.Decrement(ref RpcConnectionPool.currentReferenceCount) == 0)
			{
				lock (RpcConnectionPool.lockObject)
				{
					SearchServiceRpcClient searchServiceRpcClient;
					while (RpcConnectionPool.TryGetRpcClientFromCache(out searchServiceRpcClient))
					{
						searchServiceRpcClient.Dispose();
					}
				}
			}
		}

		private static bool TryGetRpcClientFromCache(out SearchServiceRpcClient rpcClient)
		{
			rpcClient = null;
			List<int> list = new List<int>(RpcConnectionPool.instance.rpcClientCache.Keys);
			if (list.Count <= 0)
			{
				RpcConnectionPool.tracer.TraceDebug((long)RpcConnectionPool.tracingContext, "RpcConnectionPool: Cache miss due to no available RpcClients in the cache.");
				return false;
			}
			rpcClient = RpcConnectionPool.instance.rpcClientCache.Remove(list[0]);
			if (rpcClient != null)
			{
				RpcConnectionPool.tracer.TraceDebug<int>((long)RpcConnectionPool.tracingContext, "RpcConnectionPool: RpcClient checked out from the cache. Current Cache size: {0}", RpcConnectionPool.instance.rpcClientCache.Count);
				return true;
			}
			throw new InvalidOperationException(string.Format("Null value was returned from RPC connection pool. Failed to Remove an RpcClient from the ExactTimeoutCache with a Key provided by the ExactTimeoutCache. Cache.Count: {0}, Cache.Keys.Count: {1}", RpcConnectionPool.instance.rpcClientCache.Count, RpcConnectionPool.instance.rpcClientCache.Keys.Count));
		}

		private static void RemoveFromCacheCallback(int key, SearchServiceRpcClient rpcClient, RemoveReason reason)
		{
			if (reason == RemoveReason.Removed)
			{
				return;
			}
			lock (RpcConnectionPool.lockObject)
			{
				RpcConnectionPool.tracer.TraceDebug<int>((long)RpcConnectionPool.tracingContext, "RpcConnectionPool: RpcClient discarded - Cache Timeout. Current Cache size: {0}", RpcConnectionPool.instance.rpcClientCache.Count);
			}
			rpcClient.Dispose();
		}

		private static readonly Trace tracer = ExTraceGlobals.SearchRpcClientTracer;

		private static readonly RpcConnectionPool instance = new RpcConnectionPool();

		private static readonly object lockObject = new object();

		private static int tracingContext;

		private static int globalCacheId;

		private static int maxCacheSize;

		private static int currentReferenceCount;

		private readonly SearchConfig searchConfig;

		private ExactTimeoutCache<int, SearchServiceRpcClient> rpcClientCache;
	}
}
